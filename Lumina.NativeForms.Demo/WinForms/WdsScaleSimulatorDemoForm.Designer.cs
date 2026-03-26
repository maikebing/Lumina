#nullable disable

namespace Lumina.NativeForms.Demo;

partial class WdsScaleSimulatorDemoForm
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && components is not null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        connectionGroupBox = new System.Windows.Forms.GroupBox();
        stopButton = new System.Windows.Forms.Button();
        startButton = new System.Windows.Forms.Button();
        refreshPortsButton = new System.Windows.Forms.Button();
        portTextBox = new System.Windows.Forms.TextBox();
        portLabel = new System.Windows.Forms.Label();
        ipAddressTextBox = new System.Windows.Forms.TextBox();
        ipAddressLabel = new System.Windows.Forms.Label();
        baudRateTextBox = new System.Windows.Forms.TextBox();
        baudRateLabel = new System.Windows.Forms.Label();
        serialComboBox = new System.Windows.Forms.ComboBox();
        serialLabel = new System.Windows.Forms.Label();
        modeComboBox = new System.Windows.Forms.ComboBox();
        modeLabel = new System.Windows.Forms.Label();
        vehicleGroupBox = new System.Windows.Forms.GroupBox();
        syncRowsButton = new System.Windows.Forms.Button();
        autoEnterFirstAxleCheckBox = new System.Windows.Forms.CheckBox();
        sample17Button = new System.Windows.Forms.Button();
        sample6Button = new System.Windows.Forms.Button();
        axleCountTextBox = new System.Windows.Forms.TextBox();
        axleCountLabel = new System.Windows.Forms.Label();
        nextVehicleValueLabel = new System.Windows.Forms.Label();
        nextVehicleLabel = new System.Windows.Forms.Label();
        axleGroupBox = new System.Windows.Forms.GroupBox();
        axleEditorTextBox = new System.Windows.Forms.TextBox();
        axleFormatLabel = new System.Windows.Forms.Label();
        actionsGroupBox = new System.Windows.Forms.GroupBox();
        clearQueueButton = new System.Windows.Forms.Button();
        resendButton = new System.Windows.Forms.Button();
        reverseButton = new System.Windows.Forms.Button();
        reverseAxleTextBox = new System.Windows.Forms.TextBox();
        reverseLabel = new System.Windows.Forms.Label();
        finalizeButton = new System.Windows.Forms.Button();
        advanceToEndButton = new System.Windows.Forms.Button();
        advanceButton = new System.Windows.Forms.Button();
        advanceAxleTextBox = new System.Windows.Forms.TextBox();
        advanceLabel = new System.Windows.Forms.Label();
        loadVehicleButton = new System.Windows.Forms.Button();
        statusGroupBox = new System.Windows.Forms.GroupBox();
        faultStatusValueLabel = new System.Windows.Forms.Label();
        faultStatusLabel = new System.Windows.Forms.Label();
        leftTheScaleStatusValueLabel = new System.Windows.Forms.Label();
        leftTheScaleStatusLabel = new System.Windows.Forms.Label();
        queueStatusValueLabel = new System.Windows.Forms.Label();
        queueStatusLabel = new System.Windows.Forms.Label();
        tempAxleStatusValueLabel = new System.Windows.Forms.Label();
        tempAxleStatusLabel = new System.Windows.Forms.Label();
        currentVehicleStatusValueLabel = new System.Windows.Forms.Label();
        currentVehicleStatusLabel = new System.Windows.Forms.Label();
        connectionStatusValueLabel = new System.Windows.Forms.Label();
        connectionStatusLabel = new System.Windows.Forms.Label();
        faultGroupBox = new System.Windows.Forms.GroupBox();
        tireRecognitionFaultCheckBox = new System.Windows.Forms.CheckBox();
        longVehicleFaultCheckBox = new System.Windows.Forms.CheckBox();
        communicationFaultCheckBox = new System.Windows.Forms.CheckBox();
        loopFaultCheckBox = new System.Windows.Forms.CheckBox();
        lightCurtainFaultCheckBox = new System.Windows.Forms.CheckBox();
        scaleSensorFaultCheckBox = new System.Windows.Forms.CheckBox();
        logGroupBox = new System.Windows.Forms.GroupBox();
        logTextBox = new System.Windows.Forms.TextBox();
        connectionGroupBox.SuspendLayout();
        vehicleGroupBox.SuspendLayout();
        axleGroupBox.SuspendLayout();
        actionsGroupBox.SuspendLayout();
        statusGroupBox.SuspendLayout();
        faultGroupBox.SuspendLayout();
        logGroupBox.SuspendLayout();
        SuspendLayout();
        // 
        // connectionGroupBox
        // 
        connectionGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        connectionGroupBox.Controls.Add(stopButton);
        connectionGroupBox.Controls.Add(startButton);
        connectionGroupBox.Controls.Add(refreshPortsButton);
        connectionGroupBox.Controls.Add(portTextBox);
        connectionGroupBox.Controls.Add(portLabel);
        connectionGroupBox.Controls.Add(ipAddressTextBox);
        connectionGroupBox.Controls.Add(ipAddressLabel);
        connectionGroupBox.Controls.Add(baudRateTextBox);
        connectionGroupBox.Controls.Add(baudRateLabel);
        connectionGroupBox.Controls.Add(serialComboBox);
        connectionGroupBox.Controls.Add(serialLabel);
        connectionGroupBox.Controls.Add(modeComboBox);
        connectionGroupBox.Controls.Add(modeLabel);
        connectionGroupBox.Location = new System.Drawing.Point(16, 16);
        connectionGroupBox.Name = "connectionGroupBox";
        connectionGroupBox.Size = new System.Drawing.Size(835, 146);
        connectionGroupBox.TabIndex = 0;
        connectionGroupBox.TabStop = false;
        connectionGroupBox.Text = "连接方式";
        // 
        // stopButton
        // 
        stopButton.Location = new System.Drawing.Point(693, 94);
        stopButton.Name = "stopButton";
        stopButton.Size = new System.Drawing.Size(92, 34);
        stopButton.TabIndex = 12;
        stopButton.Text = "停止";
        stopButton.UseVisualStyleBackColor = true;
        stopButton.Click += stopButton_Click;
        // 
        // startButton
        // 
        startButton.Location = new System.Drawing.Point(591, 94);
        startButton.Name = "startButton";
        startButton.Size = new System.Drawing.Size(92, 34);
        startButton.TabIndex = 11;
        startButton.Text = "启动";
        startButton.UseVisualStyleBackColor = true;
        startButton.Click += startButton_Click;
        // 
        // refreshPortsButton
        // 
        refreshPortsButton.Location = new System.Drawing.Point(475, 94);
        refreshPortsButton.Name = "refreshPortsButton";
        refreshPortsButton.Size = new System.Drawing.Size(106, 34);
        refreshPortsButton.TabIndex = 10;
        refreshPortsButton.Text = "刷新串口";
        refreshPortsButton.UseVisualStyleBackColor = true;
        refreshPortsButton.Click += refreshPortsButton_Click;
        // 
        // portTextBox
        // 
        portTextBox.Location = new System.Drawing.Point(367, 97);
        portTextBox.Name = "portTextBox";
        portTextBox.Size = new System.Drawing.Size(98, 27);
        portTextBox.TabIndex = 9;
        // 
        // portLabel
        // 
        portLabel.AutoSize = true;
        portLabel.Location = new System.Drawing.Point(291, 100);
        portLabel.Name = "portLabel";
        portLabel.Size = new System.Drawing.Size(69, 20);
        portLabel.TabIndex = 8;
        portLabel.Text = "监听端口";
        // 
        // ipAddressTextBox
        // 
        ipAddressTextBox.Location = new System.Drawing.Point(97, 97);
        ipAddressTextBox.Name = "ipAddressTextBox";
        ipAddressTextBox.Size = new System.Drawing.Size(184, 27);
        ipAddressTextBox.TabIndex = 7;
        // 
        // ipAddressLabel
        // 
        ipAddressLabel.AutoSize = true;
        ipAddressLabel.Location = new System.Drawing.Point(21, 100);
        ipAddressLabel.Name = "ipAddressLabel";
        ipAddressLabel.Size = new System.Drawing.Size(61, 20);
        ipAddressLabel.TabIndex = 6;
        ipAddressLabel.Text = "监听 IP";
        // 
        // baudRateTextBox
        // 
        baudRateTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        baudRateTextBox.Location = new System.Drawing.Point(617, 53);
        baudRateTextBox.Name = "baudRateTextBox";
        baudRateTextBox.Size = new System.Drawing.Size(168, 27);
        baudRateTextBox.TabIndex = 5;
        // 
        // baudRateLabel
        // 
        baudRateLabel.AutoSize = true;
        baudRateLabel.Location = new System.Drawing.Point(541, 56);
        baudRateLabel.Name = "baudRateLabel";
        baudRateLabel.Size = new System.Drawing.Size(54, 20);
        baudRateLabel.TabIndex = 4;
        baudRateLabel.Text = "波特率";
        // 
        // serialComboBox
        // 
        serialComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        serialComboBox.FormattingEnabled = true;
        serialComboBox.Location = new System.Drawing.Point(367, 52);
        serialComboBox.Name = "serialComboBox";
        serialComboBox.Size = new System.Drawing.Size(164, 28);
        serialComboBox.TabIndex = 3;
        // 
        // serialLabel
        // 
        serialLabel.AutoSize = true;
        serialLabel.Location = new System.Drawing.Point(291, 56);
        serialLabel.Name = "serialLabel";
        serialLabel.Size = new System.Drawing.Size(39, 20);
        serialLabel.TabIndex = 2;
        serialLabel.Text = "串口";
        // 
        // modeComboBox
        // 
        modeComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
        modeComboBox.FormattingEnabled = true;
        modeComboBox.Location = new System.Drawing.Point(97, 52);
        modeComboBox.Name = "modeComboBox";
        modeComboBox.Size = new System.Drawing.Size(150, 28);
        modeComboBox.TabIndex = 1;
        modeComboBox.SelectedIndexChanged += modeComboBox_SelectedIndexChanged;
        // 
        // modeLabel
        // 
        modeLabel.AutoSize = true;
        modeLabel.Location = new System.Drawing.Point(21, 56);
        modeLabel.Name = "modeLabel";
        modeLabel.Size = new System.Drawing.Size(39, 20);
        modeLabel.TabIndex = 0;
        modeLabel.Text = "模式";
        // 
        // vehicleGroupBox
        // 
        vehicleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        vehicleGroupBox.Controls.Add(syncRowsButton);
        vehicleGroupBox.Controls.Add(autoEnterFirstAxleCheckBox);
        vehicleGroupBox.Controls.Add(sample17Button);
        vehicleGroupBox.Controls.Add(sample6Button);
        vehicleGroupBox.Controls.Add(axleCountTextBox);
        vehicleGroupBox.Controls.Add(axleCountLabel);
        vehicleGroupBox.Controls.Add(nextVehicleValueLabel);
        vehicleGroupBox.Controls.Add(nextVehicleLabel);
        vehicleGroupBox.Location = new System.Drawing.Point(16, 176);
        vehicleGroupBox.Name = "vehicleGroupBox";
        vehicleGroupBox.Size = new System.Drawing.Size(835, 132);
        vehicleGroupBox.TabIndex = 1;
        vehicleGroupBox.TabStop = false;
        vehicleGroupBox.Text = "车辆配置";
        // 
        // syncRowsButton
        // 
        syncRowsButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        syncRowsButton.Location = new System.Drawing.Point(711, 89);
        syncRowsButton.Name = "syncRowsButton";
        syncRowsButton.Size = new System.Drawing.Size(104, 34);
        syncRowsButton.TabIndex = 7;
        syncRowsButton.Text = "同步轴表";
        syncRowsButton.UseVisualStyleBackColor = true;
        syncRowsButton.Click += syncRowsButton_Click;
        // 
        // autoEnterFirstAxleCheckBox
        // 
        autoEnterFirstAxleCheckBox.AutoSize = true;
        autoEnterFirstAxleCheckBox.Checked = true;
        autoEnterFirstAxleCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
        autoEnterFirstAxleCheckBox.Location = new System.Drawing.Point(25, 94);
        autoEnterFirstAxleCheckBox.Name = "autoEnterFirstAxleCheckBox";
        autoEnterFirstAxleCheckBox.Size = new System.Drawing.Size(151, 24);
        autoEnterFirstAxleCheckBox.TabIndex = 6;
        autoEnterFirstAxleCheckBox.Text = "进车后自动进入首轴";
        autoEnterFirstAxleCheckBox.UseVisualStyleBackColor = true;
        // 
        // sample17Button
        // 
        sample17Button.Location = new System.Drawing.Point(460, 48);
        sample17Button.Name = "sample17Button";
        sample17Button.Size = new System.Drawing.Size(102, 34);
        sample17Button.TabIndex = 5;
        sample17Button.Text = "17轴示例";
        sample17Button.UseVisualStyleBackColor = true;
        sample17Button.Click += sample17Button_Click;
        // 
        // sample6Button
        // 
        sample6Button.Location = new System.Drawing.Point(352, 48);
        sample6Button.Name = "sample6Button";
        sample6Button.Size = new System.Drawing.Size(98, 34);
        sample6Button.TabIndex = 4;
        sample6Button.Text = "6轴示例";
        sample6Button.UseVisualStyleBackColor = true;
        sample6Button.Click += sample6Button_Click;
        // 
        // axleCountTextBox
        // 
        axleCountTextBox.Location = new System.Drawing.Point(260, 52);
        axleCountTextBox.Name = "axleCountTextBox";
        axleCountTextBox.Size = new System.Drawing.Size(76, 27);
        axleCountTextBox.TabIndex = 3;
        // 
        // axleCountLabel
        // 
        axleCountLabel.AutoSize = true;
        axleCountLabel.Location = new System.Drawing.Point(213, 56);
        axleCountLabel.Name = "axleCountLabel";
        axleCountLabel.Size = new System.Drawing.Size(39, 20);
        axleCountLabel.TabIndex = 2;
        axleCountLabel.Text = "轴数";
        // 
        // nextVehicleValueLabel
        // 
        nextVehicleValueLabel.AutoSize = true;
        nextVehicleValueLabel.Location = new System.Drawing.Point(121, 56);
        nextVehicleValueLabel.Name = "nextVehicleValueLabel";
        nextVehicleValueLabel.Size = new System.Drawing.Size(57, 20);
        nextVehicleValueLabel.TabIndex = 1;
        nextVehicleValueLabel.Text = "000001";
        // 
        // nextVehicleLabel
        // 
        nextVehicleLabel.AutoSize = true;
        nextVehicleLabel.Location = new System.Drawing.Point(25, 56);
        nextVehicleLabel.Name = "nextVehicleLabel";
        nextVehicleLabel.Size = new System.Drawing.Size(84, 20);
        nextVehicleLabel.TabIndex = 0;
        nextVehicleLabel.Text = "下一车序号";
        // 
        // axleGroupBox
        // 
        axleGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        axleGroupBox.Controls.Add(axleEditorTextBox);
        axleGroupBox.Controls.Add(axleFormatLabel);
        axleGroupBox.Location = new System.Drawing.Point(16, 322);
        axleGroupBox.Name = "axleGroupBox";
        axleGroupBox.Size = new System.Drawing.Size(835, 468);
        axleGroupBox.TabIndex = 2;
        axleGroupBox.TabStop = false;
        axleGroupBox.Text = "轴数据";
        // 
        // axleEditorTextBox
        // 
        axleEditorTextBox.AcceptsReturn = true;
        axleEditorTextBox.AcceptsTab = true;
        axleEditorTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        axleEditorTextBox.Location = new System.Drawing.Point(25, 79);
        axleEditorTextBox.Multiline = true;
        axleEditorTextBox.Name = "axleEditorTextBox";
        axleEditorTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        axleEditorTextBox.Size = new System.Drawing.Size(790, 370);
        axleEditorTextBox.TabIndex = 1;
        // 
        // axleFormatLabel
        // 
        axleFormatLabel.AutoSize = true;
        axleFormatLabel.Location = new System.Drawing.Point(25, 42);
        axleFormatLabel.Name = "axleFormatLabel";
        axleFormatLabel.Size = new System.Drawing.Size(328, 20);
        axleFormatLabel.TabIndex = 0;
        axleFormatLabel.Text = "每行格式: 轴重,轮胎数,轴距    最后一轴轴距可留空";
        // 
        // actionsGroupBox
        // 
        actionsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        actionsGroupBox.Controls.Add(clearQueueButton);
        actionsGroupBox.Controls.Add(resendButton);
        actionsGroupBox.Controls.Add(reverseButton);
        actionsGroupBox.Controls.Add(reverseAxleTextBox);
        actionsGroupBox.Controls.Add(reverseLabel);
        actionsGroupBox.Controls.Add(finalizeButton);
        actionsGroupBox.Controls.Add(advanceToEndButton);
        actionsGroupBox.Controls.Add(advanceButton);
        actionsGroupBox.Controls.Add(advanceAxleTextBox);
        actionsGroupBox.Controls.Add(advanceLabel);
        actionsGroupBox.Controls.Add(loadVehicleButton);
        actionsGroupBox.Location = new System.Drawing.Point(16, 806);
        actionsGroupBox.Name = "actionsGroupBox";
        actionsGroupBox.Size = new System.Drawing.Size(835, 138);
        actionsGroupBox.TabIndex = 3;
        actionsGroupBox.TabStop = false;
        actionsGroupBox.Text = "模拟动作";
        // 
        // clearQueueButton
        // 
        clearQueueButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        clearQueueButton.Location = new System.Drawing.Point(709, 91);
        clearQueueButton.Name = "clearQueueButton";
        clearQueueButton.Size = new System.Drawing.Size(106, 34);
        clearQueueButton.TabIndex = 10;
        clearQueueButton.Text = "清空正式缓存";
        clearQueueButton.UseVisualStyleBackColor = true;
        clearQueueButton.Click += clearQueueButton_Click;
        // 
        // resendButton
        // 
        resendButton.Location = new System.Drawing.Point(457, 91);
        resendButton.Name = "resendButton";
        resendButton.Size = new System.Drawing.Size(116, 34);
        resendButton.TabIndex = 9;
        resendButton.Text = "重发上一帧";
        resendButton.UseVisualStyleBackColor = true;
        resendButton.Click += resendButton_Click;
        // 
        // reverseButton
        // 
        reverseButton.Location = new System.Drawing.Point(358, 91);
        reverseButton.Name = "reverseButton";
        reverseButton.Size = new System.Drawing.Size(89, 34);
        reverseButton.TabIndex = 8;
        reverseButton.Text = "倒车";
        reverseButton.UseVisualStyleBackColor = true;
        reverseButton.Click += reverseButton_Click;
        // 
        // reverseAxleTextBox
        // 
        reverseAxleTextBox.Location = new System.Drawing.Point(282, 95);
        reverseAxleTextBox.Name = "reverseAxleTextBox";
        reverseAxleTextBox.Size = new System.Drawing.Size(66, 27);
        reverseAxleTextBox.TabIndex = 7;
        // 
        // reverseLabel
        // 
        reverseLabel.AutoSize = true;
        reverseLabel.Location = new System.Drawing.Point(202, 99);
        reverseLabel.Name = "reverseLabel";
        reverseLabel.Size = new System.Drawing.Size(69, 20);
        reverseLabel.TabIndex = 6;
        reverseLabel.Text = "倒车轴数";
        // 
        // finalizeButton
        // 
        finalizeButton.Location = new System.Drawing.Point(25, 91);
        finalizeButton.Name = "finalizeButton";
        finalizeButton.Size = new System.Drawing.Size(160, 34);
        finalizeButton.TabIndex = 5;
        finalizeButton.Text = "整车收尾入缓存";
        finalizeButton.UseVisualStyleBackColor = true;
        finalizeButton.Click += finalizeButton_Click;
        // 
        // advanceToEndButton
        // 
        advanceToEndButton.Location = new System.Drawing.Point(422, 49);
        advanceToEndButton.Name = "advanceToEndButton";
        advanceToEndButton.Size = new System.Drawing.Size(118, 34);
        advanceToEndButton.TabIndex = 4;
        advanceToEndButton.Text = "临时轴拉满";
        advanceToEndButton.UseVisualStyleBackColor = true;
        advanceToEndButton.Click += advanceToEndButton_Click;
        // 
        // advanceButton
        // 
        advanceButton.Location = new System.Drawing.Point(294, 49);
        advanceButton.Name = "advanceButton";
        advanceButton.Size = new System.Drawing.Size(118, 34);
        advanceButton.TabIndex = 3;
        advanceButton.Text = "临时轴推进";
        advanceButton.UseVisualStyleBackColor = true;
        advanceButton.Click += advanceButton_Click;
        // 
        // advanceAxleTextBox
        // 
        advanceAxleTextBox.Location = new System.Drawing.Point(215, 53);
        advanceAxleTextBox.Name = "advanceAxleTextBox";
        advanceAxleTextBox.Size = new System.Drawing.Size(66, 27);
        advanceAxleTextBox.TabIndex = 2;
        // 
        // advanceLabel
        // 
        advanceLabel.AutoSize = true;
        advanceLabel.Location = new System.Drawing.Point(130, 56);
        advanceLabel.Name = "advanceLabel";
        advanceLabel.Size = new System.Drawing.Size(69, 20);
        advanceLabel.TabIndex = 1;
        advanceLabel.Text = "推进轴数";
        // 
        // loadVehicleButton
        // 
        loadVehicleButton.Location = new System.Drawing.Point(25, 49);
        loadVehicleButton.Name = "loadVehicleButton";
        loadVehicleButton.Size = new System.Drawing.Size(92, 34);
        loadVehicleButton.TabIndex = 0;
        loadVehicleButton.Text = "进车";
        loadVehicleButton.UseVisualStyleBackColor = true;
        loadVehicleButton.Click += loadVehicleButton_Click;
        // 
        // statusGroupBox
        // 
        statusGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        statusGroupBox.Controls.Add(faultStatusValueLabel);
        statusGroupBox.Controls.Add(faultStatusLabel);
        statusGroupBox.Controls.Add(leftTheScaleStatusValueLabel);
        statusGroupBox.Controls.Add(leftTheScaleStatusLabel);
        statusGroupBox.Controls.Add(queueStatusValueLabel);
        statusGroupBox.Controls.Add(queueStatusLabel);
        statusGroupBox.Controls.Add(tempAxleStatusValueLabel);
        statusGroupBox.Controls.Add(tempAxleStatusLabel);
        statusGroupBox.Controls.Add(currentVehicleStatusValueLabel);
        statusGroupBox.Controls.Add(currentVehicleStatusLabel);
        statusGroupBox.Controls.Add(connectionStatusValueLabel);
        statusGroupBox.Controls.Add(connectionStatusLabel);
        statusGroupBox.Location = new System.Drawing.Point(865, 16);
        statusGroupBox.Name = "statusGroupBox";
        statusGroupBox.Size = new System.Drawing.Size(559, 226);
        statusGroupBox.TabIndex = 4;
        statusGroupBox.TabStop = false;
        statusGroupBox.Text = "运行状态";
        // 
        // faultStatusValueLabel
        // 
        faultStatusValueLabel.AutoSize = true;
        faultStatusValueLabel.Location = new System.Drawing.Point(121, 183);
        faultStatusValueLabel.Name = "faultStatusValueLabel";
        faultStatusValueLabel.Size = new System.Drawing.Size(44, 20);
        faultStatusValueLabel.TabIndex = 11;
        faultStatusValueLabel.Text = "0x00";
        // 
        // faultStatusLabel
        // 
        faultStatusLabel.AutoSize = true;
        faultStatusLabel.Location = new System.Drawing.Point(25, 183);
        faultStatusLabel.Name = "faultStatusLabel";
        faultStatusLabel.Size = new System.Drawing.Size(54, 20);
        faultStatusLabel.TabIndex = 10;
        faultStatusLabel.Text = "状态字";
        // 
        // leftTheScaleStatusValueLabel
        // 
        leftTheScaleStatusValueLabel.AutoSize = true;
        leftTheScaleStatusValueLabel.Location = new System.Drawing.Point(121, 155);
        leftTheScaleStatusValueLabel.Name = "leftTheScaleStatusValueLabel";
        leftTheScaleStatusValueLabel.Size = new System.Drawing.Size(24, 20);
        leftTheScaleStatusValueLabel.TabIndex = 9;
        leftTheScaleStatusValueLabel.Text = "否";
        // 
        // leftTheScaleStatusLabel
        // 
        leftTheScaleStatusLabel.AutoSize = true;
        leftTheScaleStatusLabel.Location = new System.Drawing.Point(25, 155);
        leftTheScaleStatusLabel.Name = "leftTheScaleStatusLabel";
        leftTheScaleStatusLabel.Size = new System.Drawing.Size(54, 20);
        leftTheScaleStatusLabel.TabIndex = 8;
        leftTheScaleStatusLabel.Text = "已离开";
        // 
        // queueStatusValueLabel
        // 
        queueStatusValueLabel.AutoSize = true;
        queueStatusValueLabel.Location = new System.Drawing.Point(121, 127);
        queueStatusValueLabel.Name = "queueStatusValueLabel";
        queueStatusValueLabel.Size = new System.Drawing.Size(17, 20);
        queueStatusValueLabel.TabIndex = 7;
        queueStatusValueLabel.Text = "0";
        // 
        // queueStatusLabel
        // 
        queueStatusLabel.AutoSize = true;
        queueStatusLabel.Location = new System.Drawing.Point(25, 127);
        queueStatusLabel.Name = "queueStatusLabel";
        queueStatusLabel.Size = new System.Drawing.Size(69, 20);
        queueStatusLabel.TabIndex = 6;
        queueStatusLabel.Text = "正式缓存";
        // 
        // tempAxleStatusValueLabel
        // 
        tempAxleStatusValueLabel.AutoSize = true;
        tempAxleStatusValueLabel.Location = new System.Drawing.Point(121, 99);
        tempAxleStatusValueLabel.Name = "tempAxleStatusValueLabel";
        tempAxleStatusValueLabel.Size = new System.Drawing.Size(17, 20);
        tempAxleStatusValueLabel.TabIndex = 5;
        tempAxleStatusValueLabel.Text = "0";
        // 
        // tempAxleStatusLabel
        // 
        tempAxleStatusLabel.AutoSize = true;
        tempAxleStatusLabel.Location = new System.Drawing.Point(25, 99);
        tempAxleStatusLabel.Name = "tempAxleStatusLabel";
        tempAxleStatusLabel.Size = new System.Drawing.Size(54, 20);
        tempAxleStatusLabel.TabIndex = 4;
        tempAxleStatusLabel.Text = "临时轴";
        // 
        // currentVehicleStatusValueLabel
        // 
        currentVehicleStatusValueLabel.AutoSize = true;
        currentVehicleStatusValueLabel.Location = new System.Drawing.Point(121, 71);
        currentVehicleStatusValueLabel.Name = "currentVehicleStatusValueLabel";
        currentVehicleStatusValueLabel.Size = new System.Drawing.Size(24, 20);
        currentVehicleStatusValueLabel.TabIndex = 3;
        currentVehicleStatusValueLabel.Text = "无";
        // 
        // currentVehicleStatusLabel
        // 
        currentVehicleStatusLabel.AutoSize = true;
        currentVehicleStatusLabel.Location = new System.Drawing.Point(25, 71);
        currentVehicleStatusLabel.Name = "currentVehicleStatusLabel";
        currentVehicleStatusLabel.Size = new System.Drawing.Size(69, 20);
        currentVehicleStatusLabel.TabIndex = 2;
        currentVehicleStatusLabel.Text = "当前车辆";
        // 
        // connectionStatusValueLabel
        // 
        connectionStatusValueLabel.AutoSize = true;
        connectionStatusValueLabel.Location = new System.Drawing.Point(121, 43);
        connectionStatusValueLabel.Name = "connectionStatusValueLabel";
        connectionStatusValueLabel.Size = new System.Drawing.Size(54, 20);
        connectionStatusValueLabel.TabIndex = 1;
        connectionStatusValueLabel.Text = "未连接";
        // 
        // connectionStatusLabel
        // 
        connectionStatusLabel.AutoSize = true;
        connectionStatusLabel.Location = new System.Drawing.Point(25, 43);
        connectionStatusLabel.Name = "connectionStatusLabel";
        connectionStatusLabel.Size = new System.Drawing.Size(39, 20);
        connectionStatusLabel.TabIndex = 0;
        connectionStatusLabel.Text = "连接";
        // 
        // faultGroupBox
        // 
        faultGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
        faultGroupBox.Controls.Add(tireRecognitionFaultCheckBox);
        faultGroupBox.Controls.Add(longVehicleFaultCheckBox);
        faultGroupBox.Controls.Add(communicationFaultCheckBox);
        faultGroupBox.Controls.Add(loopFaultCheckBox);
        faultGroupBox.Controls.Add(lightCurtainFaultCheckBox);
        faultGroupBox.Controls.Add(scaleSensorFaultCheckBox);
        faultGroupBox.Location = new System.Drawing.Point(865, 256);
        faultGroupBox.Name = "faultGroupBox";
        faultGroupBox.Size = new System.Drawing.Size(559, 130);
        faultGroupBox.TabIndex = 5;
        faultGroupBox.TabStop = false;
        faultGroupBox.Text = "状态字开关";
        // 
        // tireRecognitionFaultCheckBox
        // 
        tireRecognitionFaultCheckBox.AutoSize = true;
        tireRecognitionFaultCheckBox.Location = new System.Drawing.Point(301, 101);
        tireRecognitionFaultCheckBox.Name = "tireRecognitionFaultCheckBox";
        tireRecognitionFaultCheckBox.Size = new System.Drawing.Size(136, 24);
        tireRecognitionFaultCheckBox.TabIndex = 5;
        tireRecognitionFaultCheckBox.Text = "轮胎识别故障";
        tireRecognitionFaultCheckBox.UseVisualStyleBackColor = true;
        tireRecognitionFaultCheckBox.CheckedChanged += faultCheckBox_CheckedChanged;
        // 
        // longVehicleFaultCheckBox
        // 
        longVehicleFaultCheckBox.AutoSize = true;
        longVehicleFaultCheckBox.Location = new System.Drawing.Point(25, 101);
        longVehicleFaultCheckBox.Name = "longVehicleFaultCheckBox";
        longVehicleFaultCheckBox.Size = new System.Drawing.Size(121, 24);
        longVehicleFaultCheckBox.TabIndex = 4;
        longVehicleFaultCheckBox.Text = "长车/遮挡";
        longVehicleFaultCheckBox.UseVisualStyleBackColor = true;
        longVehicleFaultCheckBox.CheckedChanged += faultCheckBox_CheckedChanged;
        // 
        // communicationFaultCheckBox
        // 
        communicationFaultCheckBox.AutoSize = true;
        communicationFaultCheckBox.Location = new System.Drawing.Point(301, 71);
        communicationFaultCheckBox.Name = "communicationFaultCheckBox";
        communicationFaultCheckBox.Size = new System.Drawing.Size(106, 24);
        communicationFaultCheckBox.TabIndex = 3;
        communicationFaultCheckBox.Text = "通讯故障";
        communicationFaultCheckBox.UseVisualStyleBackColor = true;
        communicationFaultCheckBox.CheckedChanged += faultCheckBox_CheckedChanged;
        // 
        // loopFaultCheckBox
        // 
        loopFaultCheckBox.AutoSize = true;
        loopFaultCheckBox.Location = new System.Drawing.Point(25, 71);
        loopFaultCheckBox.Name = "loopFaultCheckBox";
        loopFaultCheckBox.Size = new System.Drawing.Size(106, 24);
        loopFaultCheckBox.TabIndex = 2;
        loopFaultCheckBox.Text = "线圈故障";
        loopFaultCheckBox.UseVisualStyleBackColor = true;
        loopFaultCheckBox.CheckedChanged += faultCheckBox_CheckedChanged;
        // 
        // lightCurtainFaultCheckBox
        // 
        lightCurtainFaultCheckBox.AutoSize = true;
        lightCurtainFaultCheckBox.Location = new System.Drawing.Point(301, 41);
        lightCurtainFaultCheckBox.Name = "lightCurtainFaultCheckBox";
        lightCurtainFaultCheckBox.Size = new System.Drawing.Size(106, 24);
        lightCurtainFaultCheckBox.TabIndex = 1;
        lightCurtainFaultCheckBox.Text = "光栅故障";
        lightCurtainFaultCheckBox.UseVisualStyleBackColor = true;
        lightCurtainFaultCheckBox.CheckedChanged += faultCheckBox_CheckedChanged;
        // 
        // scaleSensorFaultCheckBox
        // 
        scaleSensorFaultCheckBox.AutoSize = true;
        scaleSensorFaultCheckBox.Location = new System.Drawing.Point(25, 41);
        scaleSensorFaultCheckBox.Name = "scaleSensorFaultCheckBox";
        scaleSensorFaultCheckBox.Size = new System.Drawing.Size(136, 24);
        scaleSensorFaultCheckBox.TabIndex = 0;
        scaleSensorFaultCheckBox.Text = "称台传感器故障";
        scaleSensorFaultCheckBox.UseVisualStyleBackColor = true;
        scaleSensorFaultCheckBox.CheckedChanged += faultCheckBox_CheckedChanged;
        // 
        // logGroupBox
        // 
        logGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        logGroupBox.Controls.Add(logTextBox);
        logGroupBox.Location = new System.Drawing.Point(865, 400);
        logGroupBox.Name = "logGroupBox";
        logGroupBox.Size = new System.Drawing.Size(559, 544);
        logGroupBox.TabIndex = 6;
        logGroupBox.TabStop = false;
        logGroupBox.Text = "通信日志";
        // 
        // logTextBox
        // 
        logTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) | System.Windows.Forms.AnchorStyles.Left) | System.Windows.Forms.AnchorStyles.Right)));
        logTextBox.Location = new System.Drawing.Point(25, 34);
        logTextBox.Multiline = true;
        logTextBox.Name = "logTextBox";
        logTextBox.ReadOnly = true;
        logTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
        logTextBox.Size = new System.Drawing.Size(514, 491);
        logTextBox.TabIndex = 0;
        // 
        // WdsScaleSimulatorDemoForm
        // 
        AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
        AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        ClientSize = new System.Drawing.Size(1440, 960);
        Controls.Add(logGroupBox);
        Controls.Add(faultGroupBox);
        Controls.Add(statusGroupBox);
        Controls.Add(actionsGroupBox);
        Controls.Add(axleGroupBox);
        Controls.Add(vehicleGroupBox);
        Controls.Add(connectionGroupBox);
        Font = new System.Drawing.Font("Microsoft YaHei UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 134);
        MinimumSize = new System.Drawing.Size(1280, 820);
        Name = "WdsScaleSimulatorDemoForm";
        StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
        Text = "称台模拟器 - WinForms Demo";
        Shown += WdsScaleSimulatorDemoForm_Shown;
        connectionGroupBox.ResumeLayout(false);
        connectionGroupBox.PerformLayout();
        vehicleGroupBox.ResumeLayout(false);
        vehicleGroupBox.PerformLayout();
        axleGroupBox.ResumeLayout(false);
        axleGroupBox.PerformLayout();
        actionsGroupBox.ResumeLayout(false);
        actionsGroupBox.PerformLayout();
        statusGroupBox.ResumeLayout(false);
        statusGroupBox.PerformLayout();
        faultGroupBox.ResumeLayout(false);
        faultGroupBox.PerformLayout();
        logGroupBox.ResumeLayout(false);
        logGroupBox.PerformLayout();
        ResumeLayout(false);
    }

    private System.Windows.Forms.GroupBox connectionGroupBox;
    private System.Windows.Forms.Button stopButton;
    private System.Windows.Forms.Button startButton;
    private System.Windows.Forms.Button refreshPortsButton;
    private System.Windows.Forms.TextBox portTextBox;
    private System.Windows.Forms.Label portLabel;
    private System.Windows.Forms.TextBox ipAddressTextBox;
    private System.Windows.Forms.Label ipAddressLabel;
    private System.Windows.Forms.TextBox baudRateTextBox;
    private System.Windows.Forms.Label baudRateLabel;
    private System.Windows.Forms.ComboBox serialComboBox;
    private System.Windows.Forms.Label serialLabel;
    private System.Windows.Forms.ComboBox modeComboBox;
    private System.Windows.Forms.Label modeLabel;
    private System.Windows.Forms.GroupBox vehicleGroupBox;
    private System.Windows.Forms.Button syncRowsButton;
    private System.Windows.Forms.CheckBox autoEnterFirstAxleCheckBox;
    private System.Windows.Forms.Button sample17Button;
    private System.Windows.Forms.Button sample6Button;
    private System.Windows.Forms.TextBox axleCountTextBox;
    private System.Windows.Forms.Label axleCountLabel;
    private System.Windows.Forms.Label nextVehicleValueLabel;
    private System.Windows.Forms.Label nextVehicleLabel;
    private System.Windows.Forms.GroupBox axleGroupBox;
    private System.Windows.Forms.TextBox axleEditorTextBox;
    private System.Windows.Forms.Label axleFormatLabel;
    private System.Windows.Forms.GroupBox actionsGroupBox;
    private System.Windows.Forms.Button clearQueueButton;
    private System.Windows.Forms.Button resendButton;
    private System.Windows.Forms.Button reverseButton;
    private System.Windows.Forms.TextBox reverseAxleTextBox;
    private System.Windows.Forms.Label reverseLabel;
    private System.Windows.Forms.Button finalizeButton;
    private System.Windows.Forms.Button advanceToEndButton;
    private System.Windows.Forms.Button advanceButton;
    private System.Windows.Forms.TextBox advanceAxleTextBox;
    private System.Windows.Forms.Label advanceLabel;
    private System.Windows.Forms.Button loadVehicleButton;
    private System.Windows.Forms.GroupBox statusGroupBox;
    private System.Windows.Forms.Label faultStatusValueLabel;
    private System.Windows.Forms.Label faultStatusLabel;
    private System.Windows.Forms.Label leftTheScaleStatusValueLabel;
    private System.Windows.Forms.Label leftTheScaleStatusLabel;
    private System.Windows.Forms.Label queueStatusValueLabel;
    private System.Windows.Forms.Label queueStatusLabel;
    private System.Windows.Forms.Label tempAxleStatusValueLabel;
    private System.Windows.Forms.Label tempAxleStatusLabel;
    private System.Windows.Forms.Label currentVehicleStatusValueLabel;
    private System.Windows.Forms.Label currentVehicleStatusLabel;
    private System.Windows.Forms.Label connectionStatusValueLabel;
    private System.Windows.Forms.Label connectionStatusLabel;
    private System.Windows.Forms.GroupBox faultGroupBox;
    private System.Windows.Forms.CheckBox tireRecognitionFaultCheckBox;
    private System.Windows.Forms.CheckBox longVehicleFaultCheckBox;
    private System.Windows.Forms.CheckBox communicationFaultCheckBox;
    private System.Windows.Forms.CheckBox loopFaultCheckBox;
    private System.Windows.Forms.CheckBox lightCurtainFaultCheckBox;
    private System.Windows.Forms.CheckBox scaleSensorFaultCheckBox;
    private System.Windows.Forms.GroupBox logGroupBox;
    private System.Windows.Forms.TextBox logTextBox;
}

#nullable restore
