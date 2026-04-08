namespace Lumina.NotepadDemo;

public sealed class frmFindReplace : Form
{
    private readonly TextBox _findTextBox;
    private readonly TextBox _replaceTextBox;
    private readonly CheckBox _matchCaseCheckBox;
    private readonly CheckBox _wholeWordCheckBox;
    private readonly CheckBox _searchUpCheckBox;
    private readonly Button _findNextButton;
    private readonly Button _replaceButton;
    private readonly Button _replaceAllButton;
    private readonly Button _closeButton;

    public frmFindReplace()
    {
        Text = "查找和替换";
        Name = nameof(frmFindReplace);
        ClientSize = new Size(420, 190);

        Label findLabel = new()
        {
            Text = "查找内容:",
            AutoSize = true,
            Location = new Point(16, 20),
        };

        _findTextBox = new TextBox
        {
            Name = "findTextBox",
            Location = new Point(100, 16),
            Size = new Size(290, 23),
        };

        Label replaceLabel = new()
        {
            Text = "替换为:",
            AutoSize = true,
            Location = new Point(16, 56),
        };

        _replaceTextBox = new TextBox
        {
            Name = "replaceTextBox",
            Location = new Point(100, 52),
            Size = new Size(290, 23),
        };

        _matchCaseCheckBox = new CheckBox
        {
            Text = "区分大小写",
            AutoSize = true,
            Location = new Point(100, 86),
        };

        _wholeWordCheckBox = new CheckBox
        {
            Text = "匹配全字",
            AutoSize = true,
            Location = new Point(100, 108),
        };

        _searchUpCheckBox = new CheckBox
        {
            Text = "向上查找",
            AutoSize = true,
            Location = new Point(210, 108),
        };

        _findNextButton = new Button
        {
            Text = "查找下一个",
            Location = new Point(16, 144),
            Size = new Size(92, 30),
        };
        _findNextButton.Click += (sender, args) => FindNextRequested?.Invoke(this, BuildRequest());

        _replaceButton = new Button
        {
            Text = "替换",
            Location = new Point(118, 144),
            Size = new Size(92, 30),
        };
        _replaceButton.Click += (sender, args) => ReplaceRequested?.Invoke(this, BuildRequest());

        _replaceAllButton = new Button
        {
            Text = "全部替换",
            Location = new Point(220, 144),
            Size = new Size(92, 30),
        };
        _replaceAllButton.Click += (sender, args) => ReplaceAllRequested?.Invoke(this, BuildRequest());

        _closeButton = new Button
        {
            Text = "关闭",
            Location = new Point(322, 144),
            Size = new Size(68, 30),
        };
        _closeButton.Click += (sender, args) => Close();

        Controls.Add(findLabel);
        Controls.Add(_findTextBox);
        Controls.Add(replaceLabel);
        Controls.Add(_replaceTextBox);
        Controls.Add(_matchCaseCheckBox);
        Controls.Add(_wholeWordCheckBox);
        Controls.Add(_searchUpCheckBox);
        Controls.Add(_findNextButton);
        Controls.Add(_replaceButton);
        Controls.Add(_replaceAllButton);
        Controls.Add(_closeButton);
    }

    public event EventHandler<FindReplaceRequest>? FindNextRequested;

    public event EventHandler<FindReplaceRequest>? ReplaceRequested;

    public event EventHandler<FindReplaceRequest>? ReplaceAllRequested;

    public void FocusFindInput()
    {
        Show();
    }

    public void SetInitialFindText(string text)
    {
        _findTextBox.Text = text ?? string.Empty;
    }

    private FindReplaceRequest BuildRequest()
    {
        return new FindReplaceRequest(
            _findTextBox.Text,
            _replaceTextBox.Text,
            _matchCaseCheckBox.Checked,
            _wholeWordCheckBox.Checked,
            _searchUpCheckBox.Checked);
    }
}

public sealed record FindReplaceRequest(string FindText, string ReplaceText, bool MatchCase, bool MatchWholeWord, bool SearchUp);
