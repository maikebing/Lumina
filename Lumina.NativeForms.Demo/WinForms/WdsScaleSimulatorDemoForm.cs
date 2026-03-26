using Lumina.WinForms;

namespace Lumina.NativeForms.Demo;

internal partial class WdsScaleSimulatorDemoForm : System.Windows.Forms.Form
{
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
        InitializeComponent();
        LoadDefaults();
        RefreshTransportUi();
        RefreshStatus();
        AppendLog("已加载 WinForms Demo 界面。");
    }

    private void WdsScaleSimulatorDemoForm_Shown(object? sender, EventArgs e)
    {
        try
        {
            this.SetMica();
        }
        catch
        {
        }
    }

    private void modeComboBox_SelectedIndexChanged(object? sender, EventArgs e)
    {
        RefreshTransportUi();
        AppendLog($"切换模式: {(modeComboBox.SelectedIndex == 0 ? "串口" : "TCP 服务端")}");
    }

    private void refreshPortsButton_Click(object? sender, EventArgs e)
    {
        AppendLog("已刷新串口列表。");
    }

    private void startButton_Click(object? sender, EventArgs e)
    {
        _running = true;
        RefreshStatus();
        AppendLog("模拟连接已启动。");
    }

    private void stopButton_Click(object? sender, EventArgs e)
    {
        _running = false;
        RefreshStatus();
        AppendLog("模拟连接已停止。");
    }

    private void sample6Button_Click(object? sender, EventArgs e)
    {
        axleCountTextBox.Text = "6";
        axleEditorTextBox.Text = Sample6;
        AppendLog("已载入 6 轴示例。");
    }

    private void sample17Button_Click(object? sender, EventArgs e)
    {
        axleCountTextBox.Text = "17";
        axleEditorTextBox.Text = Sample17;
        AppendLog("已载入 17 轴示例。");
    }

    private void syncRowsButton_Click(object? sender, EventArgs e)
    {
        AppendLog("已根据轴数同步轴表。");
    }

    private void loadVehicleButton_Click(object? sender, EventArgs e)
    {
        currentVehicleStatusValueLabel.Text = $"车辆 {nextVehicleValueLabel.Text}";
        leftTheScaleStatusValueLabel.Text = "否";
        AppendLog("已执行进车。");
    }

    private void advanceButton_Click(object? sender, EventArgs e)
    {
        tempAxleStatusValueLabel.Text = advanceAxleTextBox.Text;
        AppendLog($"临时轴推进 {advanceAxleTextBox.Text}");
    }

    private void advanceToEndButton_Click(object? sender, EventArgs e)
    {
        tempAxleStatusValueLabel.Text = axleCountTextBox.Text;
        AppendLog("临时轴已拉满。");
    }

    private void finalizeButton_Click(object? sender, EventArgs e)
    {
        queueStatusValueLabel.Text = axleCountTextBox.Text;
        AppendLog("整车已收尾入缓存。");
    }

    private void reverseButton_Click(object? sender, EventArgs e)
    {
        AppendLog($"执行倒车: {reverseAxleTextBox.Text} 轴");
    }

    private void resendButton_Click(object? sender, EventArgs e)
    {
        AppendLog("已重发上一帧。");
    }

    private void clearQueueButton_Click(object? sender, EventArgs e)
    {
        queueStatusValueLabel.Text = "0";
        AppendLog("正式缓存已清空。");
    }

    private void faultCheckBox_CheckedChanged(object? sender, EventArgs e)
    {
        RefreshStatus();
    }

    private void LoadDefaults()
    {
        modeComboBox.Items.AddRange(["串口", "TCP 服务端"]);
        modeComboBox.SelectedIndex = 0;

        serialComboBox.Items.AddRange(["COM3", "COM5", "COM9"]);
        serialComboBox.SelectedIndex = 0;

        baudRateTextBox.Text = "9600";
        ipAddressTextBox.Text = "0.0.0.0";
        portTextBox.Text = "9001";
        axleEditorTextBox.Text = Sample6;
        logTextBox.Text = "WDS Scale Simulator WinForms Demo\r\n";
    }

    private void RefreshTransportUi()
    {
        bool serialMode = modeComboBox.SelectedIndex <= 0;
        serialComboBox.Enabled = serialMode;
        baudRateTextBox.Enabled = serialMode;
        refreshPortsButton.Enabled = serialMode;
        ipAddressTextBox.Enabled = !serialMode;
        portTextBox.Enabled = !serialMode;
    }

    private void RefreshStatus()
    {
        connectionStatusValueLabel.Text = _running ? "已连接" : "未连接";
        faultStatusValueLabel.Text = $"0x{ComputeFaultFlags():X2}";
    }

    private byte ComputeFaultFlags()
    {
        byte value = 0;
        var checks = GetFaultCheckBoxes();
        for (int index = 0; index < checks.Length; index++)
        {
            if (checks[index].Checked)
            {
                value |= (byte)(1 << index);
            }
        }

        return value;
    }

    private System.Windows.Forms.CheckBox[] GetFaultCheckBoxes()
    {
        return
        [
            scaleSensorFaultCheckBox,
            lightCurtainFaultCheckBox,
            loopFaultCheckBox,
            communicationFaultCheckBox,
            longVehicleFaultCheckBox,
            tireRecognitionFaultCheckBox,
        ];
    }

    private void AppendLog(string message)
    {
        logTextBox.AppendText($"{DateTime.Now:HH:mm:ss}  {message}{Environment.NewLine}");
    }
}
