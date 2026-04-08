using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Prompts the user to select a folder.
/// </summary>
public sealed class FolderBrowserDialog : IDisposable
{
    /// <summary>
    /// Gets or sets the text shown above the tree view in the dialog box.
    /// </summary>
    public string Description { get; set; } = "选择文件夹";

    /// <summary>
    /// Gets or sets the selected path.
    /// </summary>
    public string SelectedPath { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the dialog allows creating new folders.
    /// </summary>
    public bool ShowNewFolderButton { get; set; } = true;

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
        nint titlePtr = 0;
        nint displayNameBufferPtr = 0;
        nint pidl = 0;

        try
        {
            displayNameBufferPtr = Marshal.AllocHGlobal(sizeof(char) * Win32.MAX_PATH);
            ZeroMemory(displayNameBufferPtr, sizeof(char) * Win32.MAX_PATH);
            titlePtr = string.IsNullOrWhiteSpace(Description) ? 0 : Marshal.StringToHGlobalUni(Description);

            uint flags = Win32.BIF_RETURNONLYFSDIRS | Win32.BIF_NEWDIALOGSTYLE | Win32.BIF_EDITBOX;
            if (!ShowNewFolderButton)
            {
                flags |= Win32.BIF_NONEWFOLDERBUTTON;
            }

            var browseInfo = new Win32.BROWSEINFOW
            {
                hwndOwner = owner?.Handle ?? 0,
                pszDisplayName = displayNameBufferPtr,
                lpszTitle = titlePtr,
                ulFlags = flags,
            };

            pidl = Win32.SHBrowseForFolderW(ref browseInfo);
            if (pidl == 0)
            {
                return DialogResult.Cancel;
            }

            char[] pathBuffer = new char[Win32.MAX_PATH];
            if (!Win32.SHGetPathFromIDListW(pidl, pathBuffer))
            {
                return DialogResult.Cancel;
            }

            SelectedPath = new string(pathBuffer).TrimEnd('\0');
            return DialogResult.OK;
        }
        finally
        {
            if (pidl != 0)
            {
                Win32.CoTaskMemFree(pidl);
            }

            if (titlePtr != 0)
            {
                Marshal.FreeHGlobal(titlePtr);
            }

            if (displayNameBufferPtr != 0)
            {
                Marshal.FreeHGlobal(displayNameBufferPtr);
            }
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
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
}
