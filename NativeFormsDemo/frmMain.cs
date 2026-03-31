namespace NativeFormsDemo
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();

#if !NET10_0_WINDOWS
            pictureBox1.ImageLocation = Path.Combine(AppContext.BaseDirectory, "Resources", "SplashScreen.png");
#endif
        }
    }
}
