using System.Runtime.InteropServices;

namespace NativeFormsDemo
{
    public partial class frmMain : Form
    {
        private const uint MfString = 0x00000000;
        private const uint MfSeparator = 0x00000800;
        private const uint TpmRightButton = 0x0002;
        private const uint TpmReturnCmd = 0x0100;
        private const uint ShowCommandId = 1001;
        private const uint ExitCommandId = 1002;

        public frmMain()
        {
            InitializeComponent();
        }
    }
}
