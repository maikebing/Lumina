using System.Runtime.InteropServices;
using System.Text;

namespace Lumina.Forms;

/// <summary>
/// Represents a standard dialog box that prompts the user to save a file.
/// </summary>
public sealed class SaveFileDialog : IDisposable
{
    private const int MaxFileBuffer = 32768;

    /// <summary>
    /// Gets or sets the file name selected in the dialog box.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the filter string that determines what types of files are displayed.
    /// </summary>
    public string Filter { get; set; } = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*";

    /// <summary>
    /// Gets or sets the initial directory displayed by the dialog box.
    /// </summary>
    public string InitialDirectory { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the dialog box title.
    /// </summary>
    public string Title { get; set; } = "保存";

    /// <summary>
    /// Gets or sets whether the dialog prompts before overwriting an existing file.
    /// </summary>
    public bool OverwritePrompt { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the dialog checks that the selected path exists.
    /// </summary>
    public bool CheckPathExists { get; set; } = true;

    /// <summary>
    /// Shows the dialog with no explicit owner.
    /// </summary>
    public DialogResult ShowDialog() => ShowDialog(null);

    /// <summary>
    /// Shows the dialog with the specified owner.
    /// </summary>
    /// <param name="owner">The owner form.</param>
    public DialogResult ShowDialog(Form? owner)
    {
        nint filterPtr = 0;
        nint titlePtr = 0;
        nint initialDirPtr = 0;
        nint fileBufferPtr = 0;

        try
        {
            fileBufferPtr = Marshal.AllocHGlobal(sizeof(char) * MaxFileBuffer);
            ZeroMemory(fileBufferPtr, sizeof(char) * MaxFileBuffer);
            WriteStringToBuffer(fileBufferPtr, MaxFileBuffer, FileName);

            string normalizedFilter = NormalizeFilter(Filter);
            filterPtr = Marshal.StringToHGlobalUni(normalizedFilter);
            titlePtr = string.IsNullOrWhiteSpace(Title) ? 0 : Marshal.StringToHGlobalUni(Title);
            initialDirPtr = string.IsNullOrWhiteSpace(InitialDirectory) ? 0 : Marshal.StringToHGlobalUni(InitialDirectory);

            var ofn = new Win32.OPENFILENAMEW
            {
                lStructSize = (uint)Marshal.SizeOf<Win32.OPENFILENAMEW>(),
                hwndOwner = owner?.Handle ?? 0,
                lpstrFilter = filterPtr,
                lpstrFile = fileBufferPtr,
                nMaxFile = MaxFileBuffer,
                lpstrInitialDir = initialDirPtr,
                lpstrTitle = titlePtr,
                Flags = BuildFlags(),
                nFilterIndex = 1,
            };

            if (!Win32.GetSaveFileNameW(ref ofn))
            {
                int error = Win32.CommDlgExtendedError();
                return error == 0 ? DialogResult.Cancel : DialogResult.Cancel;
            }

            FileName = ReadNullTerminatedString(fileBufferPtr);
            return DialogResult.OK;
        }
        finally
        {
            FreeHGlobal(filterPtr);
            FreeHGlobal(titlePtr);
            FreeHGlobal(initialDirPtr);
            FreeHGlobal(fileBufferPtr);
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
    }

    private uint BuildFlags()
    {
        uint flags = Win32.OFN_EXPLORER | Win32.OFN_HIDEREADONLY | Win32.OFN_NOCHANGEDIR;

        if (CheckPathExists)
        {
            flags |= Win32.OFN_PATHMUSTEXIST;
        }

        if (OverwritePrompt)
        {
            flags |= Win32.OFN_OVERWRITEPROMPT;
        }

        return flags;
    }

    private static string NormalizeFilter(string? filter)
    {
        string value = string.IsNullOrWhiteSpace(filter) ? "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*" : filter;
        string normalized = value.Replace('|', '\0');
        return normalized.EndsWith("\0\0", StringComparison.Ordinal) ? normalized : normalized + "\0\0";
    }

    private static string ReadNullTerminatedString(nint buffer)
    {
        return Marshal.PtrToStringUni(buffer) ?? string.Empty;
    }

    private static void WriteStringToBuffer(nint buffer, int maxChars, string? value)
    {
        if (string.IsNullOrEmpty(value))
        {
            return;
        }

        int copyLength = Math.Min(value.Length, maxChars - 1);
        if (copyLength <= 0)
        {
            return;
        }

        ReadOnlySpan<char> source = value.AsSpan(0, copyLength);
        byte[] bytes = Encoding.Unicode.GetBytes(source.ToArray());
        Marshal.Copy(bytes, 0, buffer, bytes.Length);
    }

    private static void ZeroMemory(nint pointer, int size)
    {
        Span<byte> zero = stackalloc byte[Math.Min(size, 1024)];
        int remaining = size;
        nint current = pointer;

        while (remaining > 0)
        {
            int blockSize = Math.Min(remaining, zero.Length);
            zero.Slice(0, blockSize).Clear();
            Marshal.Copy(zero.Slice(0, blockSize).ToArray(), 0, current, blockSize);
            current += blockSize;
            remaining -= blockSize;
        }
    }

    private static void FreeHGlobal(nint pointer)
    {
        if (pointer != 0)
        {
            Marshal.FreeHGlobal(pointer);
        }
    }
}
