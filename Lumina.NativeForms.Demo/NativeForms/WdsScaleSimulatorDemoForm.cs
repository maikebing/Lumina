using Lumina.NativeForms;

namespace Lumina.NativeForms.Demo;

internal sealed class WdsScaleSimulatorDemoForm : Form
{
    private readonly GroupBox _connectionGroup = new() { Text = "连接方式" };
    private readonly GroupBox _vehicleGroup = new() { Text = "车辆配置" };
    private readonly GroupBox _axleGroup = new() { Text = "轴数据" };
    private readonly GroupBox _actionsGroup = new() { Text = "模拟动作" };
    private readonly GroupBox _statusGroup = new() { Text = "运行状态" };
    private readonly GroupBox _faultGroup = new() { Text = "状态字开关" };
    private readonly GroupBox _logGroup = new() { Text = "通信日志" };

    private readonly Label _modeLabel = new() { Text = "模式" };
    private readonly Label _serialLabel = new() { Text = "串口" };
    private readonly Label _baudLabel = new() { Text = "波特率" };
    private readonly Label _ipLabel = new() { Text = "监听 IP" };
    private readonly Label _portLabel = new() { Text = "监听端口" };
    private readonly ComboBox _modeCombo = new();
    private readonly ComboBox _serialCombo = new();
    private readonly TextBox _baudEdit = new();
    private readonly TextBox _ipEdit = new();
    private readonly TextBox _portEdit = new();
    private readonly Button _refreshPortsButton = new() { Text = "刷新串口" };
    private readonly Button _startButton = new() { Text = "启动" };
    private readonly Button _stopButton = new() { Text = "停止" };

    private readonly Label _nextVehicleLabel = new() { Text = "下一车序号" };
    private readonly Label _nextVehicleValue = new() { Text = "000001" };
    private readonly Label _axleCountLabel = new() { Text = "轴数" };
    private readonly TextBox _axleCountEdit = new() { Text = "6" };
    private readonly CheckBox _autoFirstAxleCheck = new() { Text = "进车后自动进入首轴", Checked = true };
    private readonly Button _syncRowsButton = new() { Text = "同步轴表" };
    private readonly Button _sample6Button = new() { Text = "6轴示例" };
    private readonly Button _sample17Button = new() { Text = "17轴示例" };

    private readonly Label _axleFormatLabel = new() { Text = "每行格式: 轴重,轮胎数,轴距    最后一轴轴距可留空" };
    private readonly TextBox _axleEditor = new(multiline: true) { Text = Sample6 };

    private readonly Button _loadVehicleButton = new() { Text = "进车" };
    private readonly Label _advanceLabel = new() { Text = "推进轴数" };
    private readonly TextBox _advanceEdit = new() { Text = "1" };
    private readonly Button _advanceButton = new() { Text = "临时轴推进" };
    private readonly Button _advanceToEndButton = new() { Text = "临时轴拉满" };
    private readonly Button _finalizeButton = new() { Text = "整车收尾入缓存" };
    private readonly Label _reverseLabel = new() { Text = "倒车轴数" };
    private readonly TextBox _reverseEdit = new() { Text = "1" };
    private readonly Button _reverseButton = new() { Text = "倒车" };
    private readonly Button _resendButton = new() { Text = "重发上一帧" };
    private readonly Button _clearQueueButton = new() { Text = "清空正式缓存" };

    private readonly Label[] _statusLabels =
    [
        new Label { Text = "连接" },
        new Label { Text = "当前车辆" },
        new Label { Text = "临时轴" },
        new Label { Text = "正式缓存" },
        new Label { Text = "已离开" },
        new Label { Text = "状态字" },
    ];

    private readonly Label[] _statusValues =
    [
        new Label { Text = "未连接" },
        new Label { Text = "无" },
        new Label { Text = "0" },
        new Label { Text = "0" },
        new Label { Text = "否" },
        new Label { Text = "0x00" },
    ];

    private readonly CheckBox[] _faultChecks =
    [
        new CheckBox { Text = "称台传感器故障" },
        new CheckBox { Text = "光栅故障" },
        new CheckBox { Text = "线圈故障" },
        new CheckBox { Text = "通讯故障" },
        new CheckBox { Text = "长车/遮挡" },
        new CheckBox { Text = "轮胎识别故障" },
    ];

    private readonly TextBox _logEdit = new(multiline: true, readOnly: true);

    private bool _running;

    private const string Sample6 =
        "6500,2,1800\r\n" +
        "7200,4,1350\r\n" +
        "7300,4,1350\r\n" +
        "7100,4,1320\r\n" +
        "7000,4,1320\r\n" +
        "6800,4,\r\n";

    private const string Sample17 =
        "6200,2,1600\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,1320\r\n" +
        "6900,4,\r\n";

    public WdsScaleSimulatorDemoForm()
    {
        Text = "称台模拟器 - NativeForms Demo";
        ClientSize = new(1440, 960);

        BuildControls();
        WireEvents();
        LoadDefaults();
    }

    protected override void OnCreated()
    {
        RefreshTransportUi();
        RefreshStatus();
        AppendLog("已加载 NativeForms Demo 界面。");
    }

    protected override void OnLayout()
    {
        if (!TryGetClientSize(out var width, out var height))
        {
            return;
        }

        const int margin = 16;
        const int gap = 14;

        int leftWidth = (width - margin * 2 - gap) * 60 / 100;
        int rightWidth = width - margin * 2 - gap - leftWidth;
        int leftX = margin;
        int rightX = leftX + leftWidth + gap;

        int connectionHeight = 146;
        int vehicleHeight = 132;
        int actionsHeight = 138;
        int axleHeight = height - margin * 2 - connectionHeight - vehicleHeight - actionsHeight - gap * 3;

        _connectionGroup.SetBounds(leftX, margin, leftWidth, connectionHeight);
        _vehicleGroup.SetBounds(leftX, margin + connectionHeight + gap, leftWidth, vehicleHeight);
        _axleGroup.SetBounds(leftX, margin + connectionHeight + vehicleHeight + gap * 2, leftWidth, axleHeight);
        _actionsGroup.SetBounds(leftX, height - margin - actionsHeight, leftWidth, actionsHeight);

        int statusHeight = 226;
        int faultHeight = 130;
        int logHeight = height - margin * 2 - statusHeight - faultHeight - gap * 2;

        _statusGroup.SetBounds(rightX, margin, rightWidth, statusHeight);
        _faultGroup.SetBounds(rightX, margin + statusHeight + gap, rightWidth, faultHeight);
        _logGroup.SetBounds(rightX, margin + statusHeight + faultHeight + gap * 2, rightWidth, logHeight);

        LayoutConnectionGroup(leftX, margin, leftWidth);
        LayoutVehicleGroup(leftX, margin + connectionHeight + gap, leftWidth);
        LayoutAxleGroup(leftX, margin + connectionHeight + vehicleHeight + gap * 2, leftWidth, axleHeight);
        LayoutActionsGroup(leftX, height - margin - actionsHeight, leftWidth);
        LayoutStatusGroup(rightX, margin, rightWidth);
        LayoutFaultGroup(rightX, margin + statusHeight + gap, rightWidth);
        _logEdit.SetBounds(rightX + 20, margin + statusHeight + faultHeight + gap * 2 + 48, rightWidth - 40, logHeight - 68);
    }

    private void BuildControls()
    {
        Controls.Add(_connectionGroup);
        Controls.Add(_vehicleGroup);
        Controls.Add(_axleGroup);
        Controls.Add(_actionsGroup);
        Controls.Add(_statusGroup);
        Controls.Add(_faultGroup);
        Controls.Add(_logGroup);

        Controls.Add(_modeLabel);
        Controls.Add(_serialLabel);
        Controls.Add(_baudLabel);
        Controls.Add(_ipLabel);
        Controls.Add(_portLabel);
        Controls.Add(_modeCombo);
        Controls.Add(_serialCombo);
        Controls.Add(_baudEdit);
        Controls.Add(_ipEdit);
        Controls.Add(_portEdit);
        Controls.Add(_refreshPortsButton);
        Controls.Add(_startButton);
        Controls.Add(_stopButton);

        Controls.Add(_nextVehicleLabel);
        Controls.Add(_nextVehicleValue);
        Controls.Add(_axleCountLabel);
        Controls.Add(_axleCountEdit);
        Controls.Add(_autoFirstAxleCheck);
        Controls.Add(_syncRowsButton);
        Controls.Add(_sample6Button);
        Controls.Add(_sample17Button);

        Controls.Add(_axleFormatLabel);
        Controls.Add(_axleEditor);

        Controls.Add(_loadVehicleButton);
        Controls.Add(_advanceLabel);
        Controls.Add(_advanceEdit);
        Controls.Add(_advanceButton);
        Controls.Add(_advanceToEndButton);
        Controls.Add(_finalizeButton);
        Controls.Add(_reverseLabel);
        Controls.Add(_reverseEdit);
        Controls.Add(_reverseButton);
        Controls.Add(_resendButton);
        Controls.Add(_clearQueueButton);

        foreach (var label in _statusLabels)
        {
            Controls.Add(label);
        }

        foreach (var value in _statusValues)
        {
            Controls.Add(value);
        }

        foreach (var checkbox in _faultChecks)
        {
            Controls.Add(checkbox);
        }

        Controls.Add(_logEdit);
    }

    private void WireEvents()
    {
        _modeCombo.SelectedIndexChanged += (_, _) =>
        {
            RefreshTransportUi();
            AppendLog($"切换模式: {(_modeCombo.SelectedIndex == 0 ? "串口" : "TCP 服务端")}");
        };

        _refreshPortsButton.Click += (_, _) => AppendLog("已刷新串口列表。");
        _startButton.Click += (_, _) =>
        {
            _running = true;
            RefreshStatus();
            AppendLog("模拟连接已启动。");
        };
        _stopButton.Click += (_, _) =>
        {
            _running = false;
            RefreshStatus();
            AppendLog("模拟连接已停止。");
        };

        _sample6Button.Click += (_, _) =>
        {
            _axleCountEdit.Text = "6";
            _axleEditor.Text = Sample6;
            AppendLog("已载入 6 轴示例。");
        };

        _sample17Button.Click += (_, _) =>
        {
            _axleCountEdit.Text = "17";
            _axleEditor.Text = Sample17;
            AppendLog("已载入 17 轴示例。");
        };

        _syncRowsButton.Click += (_, _) => AppendLog("已根据轴数同步轴表。");
        _loadVehicleButton.Click += (_, _) =>
        {
            _statusValues[1].Text = $"车辆 {_nextVehicleValue.Text}";
            _statusValues[4].Text = "否";
            AppendLog("已执行进车。");
        };
        _advanceButton.Click += (_, _) =>
        {
            _statusValues[2].Text = _advanceEdit.Text;
            AppendLog($"临时轴推进 {_advanceEdit.Text}");
        };
        _advanceToEndButton.Click += (_, _) =>
        {
            _statusValues[2].Text = _axleCountEdit.Text;
            AppendLog("临时轴已拉满。");
        };
        _finalizeButton.Click += (_, _) =>
        {
            _statusValues[3].Text = _axleCountEdit.Text;
            AppendLog("整车已收尾入缓存。");
        };
        _reverseButton.Click += (_, _) => AppendLog($"执行倒车: {_reverseEdit.Text} 轴");
        _resendButton.Click += (_, _) => AppendLog("已重发上一帧。");
        _clearQueueButton.Click += (_, _) =>
        {
            _statusValues[3].Text = "0";
            AppendLog("正式缓存已清空。");
        };

        foreach (var checkbox in _faultChecks)
        {
            checkbox.CheckedChanged += (_, _) => RefreshStatus();
        }
    }

    private void LoadDefaults()
    {
        _modeCombo.Items.Add("串口");
        _modeCombo.Items.Add("TCP 服务端");
        _modeCombo.SelectedIndex = 0;

        _serialCombo.Items.Add("COM3");
        _serialCombo.Items.Add("COM5");
        _serialCombo.Items.Add("COM9");
        _serialCombo.SelectedIndex = 0;

        _baudEdit.Text = "9600";
        _ipEdit.Text = "0.0.0.0";
        _portEdit.Text = "9001";
        _logEdit.Text = "WDS Scale Simulator NativeForms Demo\r\n";
    }

    private void RefreshTransportUi()
    {
        bool serialMode = _modeCombo.SelectedIndex <= 0;
        _serialCombo.Enabled = serialMode;
        _baudEdit.Enabled = serialMode;
        _refreshPortsButton.Enabled = serialMode;
        _ipEdit.Enabled = !serialMode;
        _portEdit.Enabled = !serialMode;
    }

    private void RefreshStatus()
    {
        _statusValues[0].Text = _running ? "已连接" : "未连接";
        _statusValues[5].Text = $"0x{ComputeFaultFlags():X2}";
    }

    private byte ComputeFaultFlags()
    {
        byte value = 0;
        for (int index = 0; index < _faultChecks.Length; index++)
        {
            if (_faultChecks[index].Checked)
            {
                value |= (byte)(1 << index);
            }
        }

        return value;
    }

    private void AppendLog(string message)
    {
        _logEdit.AppendText($"{DateTime.Now:HH:mm:ss}  {message}\r\n");
    }

    private void LayoutConnectionGroup(int x, int y, int width)
    {
        int row1 = y + 56;
        int row2 = y + 100;
        int labelWidth = 70;
        int labelHeight = 24;
        int editHeight = 32;
        int buttonHeight = 34;

        int currentX = x + 20;
        _modeLabel.SetBounds(currentX, row1, labelWidth, labelHeight);
        currentX += labelWidth + 10;
        _modeCombo.SetBounds(currentX, row1 - 4, 150, 32);
        currentX += 162;
        _serialLabel.SetBounds(currentX, row1, labelWidth, labelHeight);
        currentX += labelWidth + 10;
        _serialCombo.SetBounds(currentX, row1 - 4, 164, 32);
        currentX += 176;
        _baudLabel.SetBounds(currentX, row1, labelWidth, labelHeight);
        currentX += labelWidth + 10;
        _baudEdit.SetBounds(currentX, row1 - 2, Math.Max(80, width - (currentX - x) - 20), editHeight);

        currentX = x + 20;
        _ipLabel.SetBounds(currentX, row2, labelWidth, labelHeight);
        currentX += labelWidth + 10;
        _ipEdit.SetBounds(currentX, row2 - 2, 184, editHeight);
        currentX += 196;
        _portLabel.SetBounds(currentX, row2, labelWidth, labelHeight);
        currentX += labelWidth + 10;
        _portEdit.SetBounds(currentX, row2 - 2, 98, editHeight);
        currentX += 110;
        _refreshPortsButton.SetBounds(currentX, row2 - 4, 106, buttonHeight);
        currentX += 118;
        _startButton.SetBounds(currentX, row2 - 4, 92, buttonHeight);
        currentX += 102;
        _stopButton.SetBounds(currentX, row2 - 4, 92, buttonHeight);
    }

    private void LayoutVehicleGroup(int x, int y, int width)
    {
        int row1 = y + 56;
        int row2 = y + 102;
        int labelHeight = 24;
        int editHeight = 32;
        int buttonHeight = 34;

        int currentX = x + 20;
        _nextVehicleLabel.SetBounds(currentX, row1, 88, labelHeight);
        currentX += 100;
        _nextVehicleValue.SetBounds(currentX, row1, 96, labelHeight);
        currentX += 108;
        _axleCountLabel.SetBounds(currentX, row1, 46, labelHeight);
        currentX += 56;
        _axleCountEdit.SetBounds(currentX, row1 - 2, 76, editHeight);
        currentX += 88;
        _sample6Button.SetBounds(currentX, row1 - 4, 98, buttonHeight);
        currentX += 108;
        _sample17Button.SetBounds(currentX, row1 - 4, 102, buttonHeight);

        _autoFirstAxleCheck.SetBounds(x + 20, row2 - 1, width - 170, 24);
        _syncRowsButton.SetBounds(x + width - 124, row2 - 4, 104, buttonHeight);
    }

    private void LayoutAxleGroup(int x, int y, int width, int height)
    {
        _axleFormatLabel.SetBounds(x + 20, y + 50, width - 40, 24);
        _axleEditor.SetBounds(x + 20, y + 84, width - 40, height - 108);
    }

    private void LayoutActionsGroup(int x, int y, int width)
    {
        int row1 = y + 56;
        int row2 = y + 102;
        int labelHeight = 24;
        int editHeight = 32;
        int buttonHeight = 34;

        _loadVehicleButton.SetBounds(x + 20, row1 - 4, 100, buttonHeight);
        _advanceLabel.SetBounds(x + 130, row1, 72, labelHeight);
        _advanceEdit.SetBounds(x + 208, row1 - 2, 68, editHeight);
        _advanceButton.SetBounds(x + 286, row1 - 4, 118, buttonHeight);
        _advanceToEndButton.SetBounds(x + 414, row1 - 4, 118, buttonHeight);

        _finalizeButton.SetBounds(x + 20, row2 - 4, 160, buttonHeight);
        _reverseLabel.SetBounds(x + 190, row2, 72, labelHeight);
        _reverseEdit.SetBounds(x + 270, row2 - 2, 68, editHeight);
        _reverseButton.SetBounds(x + 348, row2 - 4, 94, buttonHeight);
        _resendButton.SetBounds(x + 452, row2 - 4, 116, buttonHeight);
        _clearQueueButton.SetBounds(x + width - 126, row2 - 4, 106, buttonHeight);
    }

    private void LayoutStatusGroup(int x, int y, int width)
    {
        int labelWidth = 92;
        int rowHeight = 26;
        int valueWidth = width - 40 - labelWidth;
        int currentY = y + 48;

        for (int index = 0; index < _statusLabels.Length; index++)
        {
            _statusLabels[index].SetBounds(x + 20, currentY, labelWidth, rowHeight);
            _statusValues[index].SetBounds(x + 20 + labelWidth, currentY, valueWidth, rowHeight);
            currentY += 28;
        }
    }

    private void LayoutFaultGroup(int x, int y, int width)
    {
        int left = x + 20;
        int top = y + 48;
        int rowGap = 30;
        int columnWidth = (width - 48) / 2;

        _faultChecks[0].SetBounds(left, top, columnWidth, 24);
        _faultChecks[1].SetBounds(left + columnWidth, top, columnWidth, 24);
        _faultChecks[2].SetBounds(left, top + rowGap, columnWidth, 24);
        _faultChecks[3].SetBounds(left + columnWidth, top + rowGap, columnWidth, 24);
        _faultChecks[4].SetBounds(left, top + rowGap * 2, columnWidth, 24);
        _faultChecks[5].SetBounds(left + columnWidth, top + rowGap * 2, columnWidth, 24);
    }
}
