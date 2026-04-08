using System.Drawing;
using System.Text;
using System.Runtime.Versioning;

namespace Lumina.NotepadDemo;

[SupportedOSPlatform("windows6.1")]
public partial class frmNotepad : Form
{
    private string? _currentFilePath;
    private string _workingDirectory;
    private bool _isDirty;
    private bool _suspendDirtyTracking;
    private frmFindReplace? _findReplaceForm;
    private int _findStartIndex;
    private string _lastFindText = string.Empty;
    private Font _editorFont = CreateDefaultEditorFont();

    public frmNotepad()
    {
        InitializeComponent();
        _workingDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
        FormClosing += frmNotepad_FormClosing;
        SizeChanged += frmNotepad_SizeChanged;
        editorTextBox.SelectionChanged += editorTextBox_SelectionChanged;
        menuStrip1.Dock = DockStyle.Top;
        statusStrip1.Dock = DockStyle.Bottom;
        editorTextBox.Dock = DockStyle.None;
        AdjustEditorLayout();
        UpdateWindowCaption();
        UpdateMetricsStatus();
        UpdatePrimaryStatus("已就绪");
    }

    private static Font CreateDefaultEditorFont()
    {
        if (OperatingSystem.IsWindowsVersionAtLeast(6, 1))
        {
            Font? messageBoxFont = SystemFonts.MessageBoxFont;
            if (messageBoxFont is not null)
            {
                return (Font)messageBoxFont.Clone();
            }
        }

        return new Font("Segoe UI", 9f, FontStyle.Regular, GraphicsUnit.Point);
    }

    private void 新建ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (!EnsureDocumentCanBeDiscarded())
        {
            return;
        }

        _suspendDirtyTracking = true;
        editorTextBox.Text = string.Empty;
        _suspendDirtyTracking = false;

        _currentFilePath = null;
        _isDirty = false;
        UpdateWindowCaption();
        UpdateMetricsStatus();
        UpdatePrimaryStatus("已新建空白文档");
    }

    private void 打开ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using OpenFileDialog dialog = new()
        {
            Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
            InitialDirectory = ResolveInitialDirectory(),
            Title = "打开文件",
            CheckFileExists = true,
            CheckPathExists = true,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        string path = dialog.FileName;

        if (!File.Exists(path))
        {
            UpdatePrimaryStatus($"文件不存在: {path}");
            return;
        }

        try
        {
            string text = File.ReadAllText(path, Encoding.UTF8);
            _suspendDirtyTracking = true;
            editorTextBox.Text = text;
            _suspendDirtyTracking = false;

            _currentFilePath = path;
            UpdateWorkingDirectory(path);
            _isDirty = false;
            UpdateWindowCaption();
            UpdateMetricsStatus();
            UpdatePrimaryStatus($"已打开: {Path.GetFileName(path)}");
        }
        catch (Exception ex)
        {
            _suspendDirtyTracking = false;
            UpdatePrimaryStatus($"打开失败: {ex.Message}");
        }
    }

    private void 保存ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(_currentFilePath))
        {
            另存为ToolStripMenuItem_Click(sender, e);
            return;
        }

        SaveToPath(_currentFilePath);
    }

    private void 另存为ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using SaveFileDialog dialog = new()
        {
            Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
            InitialDirectory = ResolveInitialDirectory(),
            Title = "另存为",
            OverwritePrompt = true,
            CheckPathExists = true,
        };

        if (!string.IsNullOrWhiteSpace(_currentFilePath))
        {
            dialog.FileName = Path.GetFileName(_currentFilePath);
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        SaveToPath(dialog.FileName);
    }

    private void 打开文件夹ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using FolderBrowserDialog dialog = new()
        {
            Description = "选择默认工作目录",
            SelectedPath = ResolveInitialDirectory(),
            ShowNewFolderButton = true,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK || string.IsNullOrWhiteSpace(dialog.SelectedPath))
        {
            return;
        }

        _workingDirectory = dialog.SelectedPath;
        UpdatePrimaryStatus($"工作目录已切换: {_workingDirectory}");
    }

    private void 退出ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        Close();
    }

    private void 查找ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        EnsureFindReplaceWindow(selectReplaceTab: false);
    }

    private void 替换ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        EnsureFindReplaceWindow(selectReplaceTab: true);
    }

    private void 插入时间日期ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        string stamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        if (!string.IsNullOrEmpty(editorTextBox.Text))
        {
            editorTextBox.AppendText(Environment.NewLine);
        }

        editorTextBox.AppendText(stamp);
        UpdatePrimaryStatus("已插入时间/日期");
    }

    private void 统计ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        UpdateMetricsStatus();
        UpdatePrimaryStatus("已更新字数统计");
    }

    private void 自动换行ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        自动换行ToolStripMenuItem.Checked = true;
        UpdatePrimaryStatus("当前 Lumina.Forms 编辑器固定为自动换行模式");
    }

    private void 字体ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using FontDialog dialog = new()
        {
            Font = (Font)_editorFont.Clone(),
            Color = editorTextBox.ForeColor,
            ShowColor = true,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        _editorFont.Dispose();
        _editorFont = (Font)dialog.Font.Clone();
        editorTextBox.Font = _editorFont;
        editorTextBox.ForeColor = dialog.Color;
        UpdatePrimaryStatus($"字体已更新: {_editorFont.Name}, {_editorFont.SizeInPoints:0.#}pt");
    }

    private void 颜色ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        using ColorDialog dialog = new()
        {
            Color = editorTextBox.ForeColor,
            FullOpen = true,
            AnyColor = true,
        };

        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        editorTextBox.ForeColor = dialog.Color;
        UpdatePrimaryStatus("文本颜色已更新");
    }

    private void 系统关于ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _ = SystemAboutDialog.Show(this, "Lumina Notepad", "Lumina.Forms WinForms Compatible Demo", Icon);
    }

    private void 关于ToolStripMenuItem_Click(object? sender, EventArgs e)
    {
        _ = MessageBox.Show(
            this,
            "Lumina Notepad Demo\n基于 Lumina.Forms，支持 AOT。",
            "关于 Lumina Notepad",
            MessageBoxButtons.OK,
            MessageBoxIcon.Information);
    }

    private void editorTextBox_TextChanged(object? sender, EventArgs e)
    {
        if (_suspendDirtyTracking)
        {
            return;
        }

        _isDirty = true;
        _findStartIndex = 0;
        UpdateWindowCaption();
        UpdateMetricsStatus();
    }

    private void editorTextBox_SelectionChanged(object? sender, EventArgs e)
    {
        UpdateMetricsStatus();
    }

    private void frmNotepad_SizeChanged(object? sender, EventArgs e)
    {
        AdjustEditorLayout();
    }

    private void frmNotepad_FormClosing(object? sender, FormClosingEventArgs e)
    {
        if (!EnsureDocumentCanBeDiscarded())
        {
            e.Cancel = true;
        }
    }

    private void SaveToPath(string path)
    {
        try
        {
            string? directory = Path.GetDirectoryName(path);
            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(path, editorTextBox.Text, new UTF8Encoding(false));
            _currentFilePath = path;
            UpdateWorkingDirectory(path);
            _isDirty = false;
            UpdateWindowCaption();
            UpdateMetricsStatus();
            UpdatePrimaryStatus($"已保存: {Path.GetFileName(path)}");
        }
        catch (Exception ex)
        {
            UpdatePrimaryStatus($"保存失败: {ex.Message}");
        }
    }

    private void UpdateWindowCaption()
    {
        string fileName = string.IsNullOrWhiteSpace(_currentFilePath)
            ? "无标题"
            : Path.GetFileName(_currentFilePath);

        string dirtySuffix = _isDirty ? " *" : string.Empty;
        Text = $"{fileName}{dirtySuffix} - Lumina Notepad";
    }

    private void UpdatePrimaryStatus(string status)
    {
        primaryStatusLabel.Text = status;
    }

    private void UpdateMetricsStatus()
    {
        string text = editorTextBox.Text;
        int charCount = text.Length;
        int caretIndex = Math.Max(0, editorTextBox.SelectionStart);
        (int caretLine, int caretColumn) = GetLineColumnFromIndex(caretIndex);

        string normalized = text.Replace("\r\n", "\n", StringComparison.Ordinal);
        int lineCount = normalized.Length == 0 ? 1 : normalized.Split('\n').Length;

        int wordCount = 0;
        foreach (string part in normalized.Split(new[] { ' ', '\t', '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries))
        {
            if (!string.IsNullOrWhiteSpace(part))
            {
                wordCount++;
            }
        }

        metricsStatusLabel.Text = $"Ln {caretLine}, Col {caretColumn} | 行: {lineCount} | 字符: {charCount} | 词: {wordCount}";
    }

    private void EnsureFindReplaceWindow(bool selectReplaceTab)
    {
        if (_findReplaceForm is null)
        {
            _findReplaceForm = new frmFindReplace();
            _findReplaceForm.FindNextRequested += findReplaceForm_FindNextRequested;
            _findReplaceForm.ReplaceRequested += findReplaceForm_ReplaceRequested;
            _findReplaceForm.ReplaceAllRequested += findReplaceForm_ReplaceAllRequested;
            _findReplaceForm.Closed += findReplaceForm_Closed;
            _findReplaceForm.Show();
        }

        frmFindReplace form = _findReplaceForm;
        form.SetInitialFindText(_lastFindText);
        form.FocusFindInput();

        if (selectReplaceTab)
        {
            UpdatePrimaryStatus("替换模式已打开");
        }
    }

    private void findReplaceForm_Closed(object? sender, EventArgs e)
    {
        if (_findReplaceForm is not null)
        {
            _findReplaceForm.FindNextRequested -= findReplaceForm_FindNextRequested;
            _findReplaceForm.ReplaceRequested -= findReplaceForm_ReplaceRequested;
            _findReplaceForm.ReplaceAllRequested -= findReplaceForm_ReplaceAllRequested;
            _findReplaceForm.Closed -= findReplaceForm_Closed;
            _findReplaceForm = null;
        }
    }

    private void findReplaceForm_FindNextRequested(object? sender, FindReplaceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FindText))
        {
            UpdatePrimaryStatus("请输入查找内容");
            return;
        }

        _lastFindText = request.FindText;

        if (TryFindNextIndex(request.FindText, request.MatchCase, request.MatchWholeWord, request.SearchUp, out int index))
        {
            HighlightMatch(index, request.FindText.Length);
            _findStartIndex = request.SearchUp ? index : index + request.FindText.Length;
            (int line, int column) = GetLineColumnFromIndex(index);
            UpdatePrimaryStatus($"找到: 第 {line} 行, 第 {column} 列");
            return;
        }

        _findStartIndex = 0;
        UpdatePrimaryStatus("未找到匹配项");
    }

    private void findReplaceForm_ReplaceRequested(object? sender, FindReplaceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FindText))
        {
            UpdatePrimaryStatus("请输入查找内容");
            return;
        }

        _lastFindText = request.FindText;

        if (!TryFindNextIndex(request.FindText, request.MatchCase, request.MatchWholeWord, request.SearchUp, out int index))
        {
            _findStartIndex = 0;
            UpdatePrimaryStatus("未找到可替换项");
            return;
        }

        string text = editorTextBox.Text;
        string newText = string.Concat(
            text.AsSpan(0, index),
            request.ReplaceText,
            text.AsSpan(index + request.FindText.Length));

        _suspendDirtyTracking = true;
        editorTextBox.Text = newText;
        _suspendDirtyTracking = false;

        _isDirty = true;
        _findStartIndex = request.SearchUp ? index : index + request.ReplaceText.Length;
        HighlightMatch(index, request.ReplaceText.Length);
        UpdateWindowCaption();
        UpdateMetricsStatus();
        UpdatePrimaryStatus("已替换 1 项");
    }

    private void findReplaceForm_ReplaceAllRequested(object? sender, FindReplaceRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.FindText))
        {
            UpdatePrimaryStatus("请输入查找内容");
            return;
        }

        _lastFindText = request.FindText;

        StringComparison comparison = request.MatchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
        string source = editorTextBox.Text;
        int count = 0;
        int searchStart = 0;
        var builder = new System.Text.StringBuilder(source.Length);

        while (true)
        {
            int index = FindForward(source, request.FindText, searchStart, comparison, request.MatchWholeWord);
            if (index < 0)
            {
                builder.Append(source, searchStart, source.Length - searchStart);
                break;
            }

            builder.Append(source, searchStart, index - searchStart);
            builder.Append(request.ReplaceText);
            searchStart = index + request.FindText.Length;
            count++;
        }

        if (count == 0)
        {
            UpdatePrimaryStatus("未找到可替换项");
            return;
        }

        _suspendDirtyTracking = true;
        editorTextBox.Text = builder.ToString();
        _suspendDirtyTracking = false;
        _isDirty = true;
        _findStartIndex = 0;
        editorTextBox.Select(0, 0);
        UpdateWindowCaption();
        UpdateMetricsStatus();
        UpdatePrimaryStatus($"已全部替换: {count} 项");
    }

    private void HighlightMatch(int index, int length)
    {
        editorTextBox.Focus();
        editorTextBox.Select(index, Math.Max(0, length));
        editorTextBox.ScrollToCaret();
    }

    private bool TryFindNextIndex(string keyword, bool matchCase, bool matchWholeWord, bool searchUp, out int index)
    {
        string text = editorTextBox.Text;
        StringComparison comparison = matchCase ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

        if (text.Length == 0)
        {
            index = -1;
            return false;
        }

        if (!searchUp)
        {
            if (_findStartIndex > text.Length)
            {
                _findStartIndex = 0;
            }

            index = FindForward(text, keyword, _findStartIndex, comparison, matchWholeWord);
            if (index >= 0)
            {
                return true;
            }

            if (_findStartIndex > 0)
            {
                index = FindForward(text, keyword, 0, comparison, matchWholeWord);
            }

            return index >= 0;
        }

        int searchStart = _findStartIndex <= 0
            ? text.Length - 1
            : Math.Min(_findStartIndex - 1, text.Length - 1);

        index = FindBackward(text, keyword, searchStart, comparison, matchWholeWord);
        if (index >= 0)
        {
            return true;
        }

        index = FindBackward(text, keyword, text.Length - 1, comparison, matchWholeWord);
        return index >= 0;
    }

    private static int FindForward(string source, string value, int startIndex, StringComparison comparison, bool matchWholeWord)
    {
        if (startIndex < 0 || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
        {
            return -1;
        }

        int cursor = startIndex;
        while (cursor <= source.Length)
        {
            int candidate = source.IndexOf(value, cursor, comparison);
            if (candidate < 0)
            {
                return -1;
            }

            if (!matchWholeWord || IsWholeWord(source, candidate, value.Length))
            {
                return candidate;
            }

            cursor = candidate + 1;
        }

        return -1;
    }

    private static int FindBackward(string source, string value, int startIndex, StringComparison comparison, bool matchWholeWord)
    {
        if (startIndex < 0 || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(value))
        {
            return -1;
        }

        int cursor = Math.Min(startIndex, source.Length - 1);
        while (cursor >= 0)
        {
            int candidate = source.LastIndexOf(value, cursor, comparison);
            if (candidate < 0)
            {
                return -1;
            }

            if (!matchWholeWord || IsWholeWord(source, candidate, value.Length))
            {
                return candidate;
            }

            cursor = candidate - 1;
        }

        return -1;
    }

    private static bool IsWholeWord(string source, int index, int length)
    {
        int before = index - 1;
        int after = index + length;

        bool beforeBoundary = before < 0 || !IsWordChar(source[before]);
        bool afterBoundary = after >= source.Length || !IsWordChar(source[after]);
        return beforeBoundary && afterBoundary;
    }

    private static bool IsWordChar(char value)
    {
        return char.IsLetterOrDigit(value) || value == '_';
    }

    private void AdjustEditorLayout()
    {
        int width = Math.Max(1, ClientSize.Width);
        int height = Math.Max(1, ClientSize.Height);
        int menuHeight = Math.Max(0, menuStrip1.Height);
        int statusHeight = Math.Max(0, statusStrip1.Height);

        int editorHeight = Math.Max(1, height - menuHeight - statusHeight);

        editorTextBox.SetBounds(0, 0, width, editorHeight);
        statusStrip1.SetBounds(0, Math.Max(0, height - statusHeight), width, statusHeight);
    }

    private (int Line, int Column) GetLineColumnFromIndex(int index)
    {
        string text = editorTextBox.Text;
        int line = 1;
        int column = 1;

        for (int i = 0; i < Math.Min(index, text.Length); i++)
        {
            if (text[i] == '\n')
            {
                line++;
                column = 1;
            }
            else
            {
                column++;
            }
        }

        return (line, column);
    }

    private bool EnsureDocumentCanBeDiscarded()
    {
        if (!_isDirty)
        {
            return true;
        }

        DialogResult result = MessageBox.Show(
            this,
            "当前文档尚未保存，是否保存后再继续？",
            "Lumina Notepad",
            MessageBoxButtons.YesNoCancel,
            MessageBoxIcon.Question);

        if (result == DialogResult.Cancel)
        {
            return false;
        }

        if (result == DialogResult.No)
        {
            return true;
        }

        if (string.IsNullOrWhiteSpace(_currentFilePath))
        {
            using SaveFileDialog dialog = new()
            {
                Filter = "文本文件 (*.txt)|*.txt|所有文件 (*.*)|*.*",
                InitialDirectory = ResolveInitialDirectory(),
                Title = "保存",
                OverwritePrompt = true,
                CheckPathExists = true,
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return false;
            }

            SaveToPath(dialog.FileName);
            return !_isDirty;
        }

        SaveToPath(_currentFilePath);
        return !_isDirty;
    }

    private string ResolveInitialDirectory()
    {
        if (!string.IsNullOrWhiteSpace(_currentFilePath))
        {
            string? currentDir = Path.GetDirectoryName(_currentFilePath);
            if (!string.IsNullOrWhiteSpace(currentDir) && Directory.Exists(currentDir))
            {
                return currentDir;
            }
        }

        if (!string.IsNullOrWhiteSpace(_workingDirectory) && Directory.Exists(_workingDirectory))
        {
            return _workingDirectory;
        }

        return Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
    }

    private void UpdateWorkingDirectory(string path)
    {
        string? directory = Path.GetDirectoryName(path);
        if (!string.IsNullOrWhiteSpace(directory) && Directory.Exists(directory))
        {
            _workingDirectory = directory;
        }
    }
}
