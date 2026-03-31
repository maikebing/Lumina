using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace Lumina.Forms;

/// <summary>
/// Represents an image box backed by a native static control.
/// </summary>
[SupportedOSPlatform("windows")]
public class PictureBox : Control, ISupportInitialize
{
    private Image? _image;
    private string? _imageLocation;
    private bool _initializing;
    private bool _applyingImage;
    private nint _bitmapHandle;
    private Size _sourceImageSize;
    private PictureBoxSizeMode _sizeMode;

    /// <summary>
    /// Occurs when the user activates the picture box.
    /// </summary>
    public event EventHandler? Click;

    /// <summary>
    /// Gets or sets the image displayed by the control.
    /// </summary>
    public Image? Image
    {
        get => _image;
        set
        {
            _image = value;
            if (value is not null)
            {
                _imageLocation = null;
            }

            if (!_initializing)
            {
                ApplyImage();
            }
        }
    }

    /// <summary>
    /// Gets or sets the file path used to load the displayed image.
    /// </summary>
    public string? ImageLocation
    {
        get => _imageLocation;
        set
        {
            _imageLocation = string.IsNullOrWhiteSpace(value) ? null : value;
            if (_imageLocation is not null)
            {
                _image = null;
            }

            if (!_initializing)
            {
                ApplyImage();
            }
        }
    }

    /// <summary>
    /// Gets or sets how the control sizes and positions the image.
    /// </summary>
    public PictureBoxSizeMode SizeMode
    {
        get => _sizeMode;
        set
        {
            if (_sizeMode == value)
            {
                return;
            }

            _sizeMode = value;
            AutoSize = value == PictureBoxSizeMode.AutoSize;

            if (!_initializing)
            {
                ApplyImage();
            }
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.SS_BITMAP | Win32.SS_NOTIFY;

    /// <inheritdoc />
    private protected override ThemeColorSlot DefaultBackgroundSlot => ThemeColorSlot.Surface;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
        ApplyImage();
    }

    /// <inheritdoc />
    protected override void OnBoundsChanged()
    {
        if (_initializing || _applyingImage || (_image is null && string.IsNullOrWhiteSpace(_imageLocation)))
        {
            return;
        }

        ApplyImage();
    }

    /// <inheritdoc />
    public void BeginInit()
    {
        _initializing = true;
    }

    /// <inheritdoc />
    public void EndInit()
    {
        _initializing = false;
        ApplyImage();
    }

    /// <summary>
    /// Loads the image referenced by <see cref="ImageLocation"/>.
    /// </summary>
    public void Load()
    {
        ApplyImage();
    }

    /// <inheritdoc />
    protected override void OnDisposing()
    {
        ReleaseBitmap();
    }

    /// <inheritdoc />
    protected override bool OnCommand(int notificationCode)
    {
        if (notificationCode != Win32.STN_CLICKED)
        {
            return false;
        }

        OnClick(EventArgs.Empty);
        return true;
    }

    /// <summary>
    /// Raises the <see cref="Click"/> event.
    /// </summary>
    /// <param name="e">The event arguments.</param>
    protected virtual void OnClick(EventArgs e)
    {
        Click?.Invoke(this, e);
    }

    private void ApplyImage()
    {
        _applyingImage = true;
        try
        {
            ReleaseBitmap();

            if (_image is null && (string.IsNullOrWhiteSpace(_imageLocation) || !File.Exists(_imageLocation)))
            {
                _sourceImageSize = Size.Empty;
                if (Handle != 0)
                {
                    _ = Win32.SendMessageW(Handle, Win32.STM_SETIMAGE, (nint)Win32.IMAGE_BITMAP, 0);
                }

                return;
            }

            _sourceImageSize = _image is not null
                ? new Size(_image.Width, _image.Height)
                : NativeBitmapLoader.GetImageSize(_imageLocation!);

            if (_sourceImageSize.Width <= 0 || _sourceImageSize.Height <= 0)
            {
                return;
            }

            if (SizeMode == PictureBoxSizeMode.AutoSize
                && (Width != _sourceImageSize.Width || Height != _sourceImageSize.Height))
            {
                SetBounds(Left, Top, _sourceImageSize.Width, _sourceImageSize.Height);
            }

            if (Handle == 0)
            {
                return;
            }

            Size canvasSize = SizeMode == PictureBoxSizeMode.AutoSize
                ? _sourceImageSize
                : new Size(Math.Max(1, Width), Math.Max(1, Height));

            Rectangle imageRectangle = CalculateImageRectangle(canvasSize, _sourceImageSize, SizeMode);

            nint newBitmapHandle = _image is not null
                ? NativeBitmapLoader.RenderBitmap(_image, canvasSize, imageRectangle)
                : NativeBitmapLoader.RenderBitmap(_imageLocation!, canvasSize, imageRectangle);

            nint previousHandle = Win32.SendMessageW(Handle, Win32.STM_SETIMAGE, (nint)Win32.IMAGE_BITMAP, newBitmapHandle);
            if (previousHandle != 0)
            {
                _ = Win32.DeleteObject(previousHandle);
            }

            _bitmapHandle = newBitmapHandle;
        }
        finally
        {
            _applyingImage = false;
        }
    }

    private void ReleaseBitmap()
    {
        if (_bitmapHandle == 0)
        {
            return;
        }

        _ = Win32.DeleteObject(_bitmapHandle);
        _bitmapHandle = 0;
    }

    private static class NativeBitmapLoader
    {
        private static readonly Lock s_syncRoot = new();
        private static nuint s_gdiplusToken;
        private static bool s_started;
        private const int PixelFormat32bppArgb = 0x26200A;
        private const int InterpolationModeHighQualityBicubic = 7;

        public static Size GetImageSize(string path)
        {
            EnsureStarted();

            if (GdipCreateBitmapFromFile(path, out nint bitmap) != 0 || bitmap == 0)
            {
                return Size.Empty;
            }

            try
            {
                _ = GdipGetImageWidth(bitmap, out uint width);
                _ = GdipGetImageHeight(bitmap, out uint height);
                return new Size((int)width, (int)height);
            }
            finally
            {
                _ = GdipDisposeImage(bitmap);
            }
        }

        public static nint RenderBitmap(string path, Size canvasSize, Rectangle imageRectangle)
        {
            EnsureStarted();

            if (GdipCreateBitmapFromFile(path, out nint sourceBitmap) != 0 || sourceBitmap == 0)
            {
                return 0;
            }

            try
            {
                if (GdipCreateBitmapFromScan0(canvasSize.Width, canvasSize.Height, 0, PixelFormat32bppArgb, 0, out nint renderedBitmap) != 0 || renderedBitmap == 0)
                {
                    return 0;
                }

                try
                {
                    if (GdipGetImageGraphicsContext(renderedBitmap, out nint graphics) != 0 || graphics == 0)
                    {
                        return 0;
                    }

                    try
                    {
                        _ = GdipGraphicsClear(graphics, 0xFFFFFFFF);
                        _ = GdipSetInterpolationMode(graphics, InterpolationModeHighQualityBicubic);
                        _ = GdipDrawImageRectI(graphics, sourceBitmap, imageRectangle.X, imageRectangle.Y, imageRectangle.Width, imageRectangle.Height);
                    }
                    finally
                    {
                        _ = GdipDeleteGraphics(graphics);
                    }

                    return GdipCreateHBITMAPFromBitmap(renderedBitmap, out nint hBitmap, 0) == 0
                        ? hBitmap
                        : 0;
                }
                finally
                {
                    _ = GdipDisposeImage(renderedBitmap);
                }
            }
            finally
            {
                _ = GdipDisposeImage(sourceBitmap);
            }
        }

        public static nint RenderBitmap(Image image, Size canvasSize, Rectangle imageRectangle)
        {
            using var renderedBitmap = new Bitmap(canvasSize.Width, canvasSize.Height, PixelFormat.Format32bppArgb);
            using var graphics = Graphics.FromImage(renderedBitmap);

            graphics.Clear(Color.White);
            graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphics.DrawImage(image, imageRectangle);

            return renderedBitmap.GetHbitmap();
        }

        private static void EnsureStarted()
        {
            lock (s_syncRoot)
            {
                if (s_started)
                {
                    return;
                }

                var startupInput = new GdiplusStartupInput
                {
                    GdiplusVersion = 1,
                    SuppressBackgroundThread = false,
                    SuppressExternalCodecs = false,
                };

                if (GdiplusStartup(out s_gdiplusToken, in startupInput, 0) == 0)
                {
                    s_started = true;
                }
            }
        }

        [DllImport("gdiplus.dll", CharSet = CharSet.Unicode)]
        private static extern int GdipCreateBitmapFromFile(string filename, out nint bitmap);

        [DllImport("gdiplus.dll")]
        private static extern int GdipCreateHBITMAPFromBitmap(nint bitmap, out nint hbmReturn, uint background);

        [DllImport("gdiplus.dll")]
        private static extern int GdipCreateBitmapFromScan0(int width, int height, int stride, int pixelFormat, nint scan0, out nint bitmap);

        [DllImport("gdiplus.dll")]
        private static extern int GdipGetImageGraphicsContext(nint image, out nint graphics);

        [DllImport("gdiplus.dll")]
        private static extern int GdipDeleteGraphics(nint graphics);

        [DllImport("gdiplus.dll")]
        private static extern int GdipGraphicsClear(nint graphics, uint color);

        [DllImport("gdiplus.dll")]
        private static extern int GdipDrawImageRectI(nint graphics, nint image, int x, int y, int width, int height);

        [DllImport("gdiplus.dll")]
        private static extern int GdipSetInterpolationMode(nint graphics, int interpolationMode);

        [DllImport("gdiplus.dll")]
        private static extern int GdipGetImageWidth(nint image, out uint width);

        [DllImport("gdiplus.dll")]
        private static extern int GdipGetImageHeight(nint image, out uint height);

        [DllImport("gdiplus.dll")]
        private static extern int GdipDisposeImage(nint image);

        [DllImport("gdiplus.dll")]
        private static extern int GdiplusStartup(out nuint token, in GdiplusStartupInput input, nint output);

        [StructLayout(LayoutKind.Sequential)]
        private struct GdiplusStartupInput
        {
            public uint GdiplusVersion;
            public nint DebugEventCallback;
            [MarshalAs(UnmanagedType.Bool)]
            public bool SuppressBackgroundThread;
            [MarshalAs(UnmanagedType.Bool)]
            public bool SuppressExternalCodecs;
        }
    }

    private static Rectangle CalculateImageRectangle(Size canvasSize, Size imageSize, PictureBoxSizeMode sizeMode)
    {
        if (imageSize.Width <= 0 || imageSize.Height <= 0)
        {
            return Rectangle.Empty;
        }

        return sizeMode switch
        {
            PictureBoxSizeMode.StretchImage => new Rectangle(0, 0, canvasSize.Width, canvasSize.Height),
            PictureBoxSizeMode.CenterImage => new Rectangle(
                Math.Max(0, (canvasSize.Width - imageSize.Width) / 2),
                Math.Max(0, (canvasSize.Height - imageSize.Height) / 2),
                imageSize.Width,
                imageSize.Height),
            PictureBoxSizeMode.Zoom => CalculateZoomRectangle(canvasSize, imageSize),
            PictureBoxSizeMode.AutoSize => new Rectangle(0, 0, imageSize.Width, imageSize.Height),
            _ => new Rectangle(0, 0, imageSize.Width, imageSize.Height),
        };
    }

    private static Rectangle CalculateZoomRectangle(Size canvasSize, Size imageSize)
    {
        double scale = Math.Min((double)canvasSize.Width / imageSize.Width, (double)canvasSize.Height / imageSize.Height);
        int width = Math.Max(1, (int)Math.Round(imageSize.Width * scale, MidpointRounding.AwayFromZero));
        int height = Math.Max(1, (int)Math.Round(imageSize.Height * scale, MidpointRounding.AwayFromZero));
        int x = Math.Max(0, (canvasSize.Width - width) / 2);
        int y = Math.Max(0, (canvasSize.Height - height) / 2);
        return new Rectangle(x, y, width, height);
    }
}
