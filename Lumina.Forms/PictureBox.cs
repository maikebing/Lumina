using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Lumina.Forms;

/// <summary>
/// Represents an image box backed by a native static control.
/// </summary>
public class PictureBox : Control, ISupportInitialize
{
    private string? _imageLocation;
    private bool _initializing;
    private nint _bitmapHandle;

    /// <summary>
    /// Gets or sets the file path used to load the displayed image.
    /// </summary>
    public string? ImageLocation
    {
        get => _imageLocation;
        set
        {
            _imageLocation = string.IsNullOrWhiteSpace(value) ? null : value;

            if (!_initializing)
            {
                ApplyImage();
            }
        }
    }

    /// <inheritdoc />
    protected override string ClassName => "STATIC";

    /// <inheritdoc />
    protected override uint Style => Win32.WS_CHILD | Win32.WS_VISIBLE | Win32.SS_BITMAP;

    /// <inheritdoc />
    protected override void OnHandleCreated()
    {
        base.OnHandleCreated();
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

    private void ApplyImage()
    {
        if (Handle == 0)
        {
            return;
        }

        nint newBitmapHandle = 0;
        if (!string.IsNullOrWhiteSpace(_imageLocation) && File.Exists(_imageLocation))
        {
            newBitmapHandle = NativeBitmapLoader.LoadBitmap(_imageLocation);
        }

        nint previousHandle = Win32.SendMessageW(Handle, Win32.STM_SETIMAGE, (nint)Win32.IMAGE_BITMAP, newBitmapHandle);
        if (previousHandle != 0)
        {
            _ = Win32.DeleteObject(previousHandle);
        }

        _bitmapHandle = newBitmapHandle;
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

        public static nint LoadBitmap(string path)
        {
            EnsureStarted();

            if (GdipCreateBitmapFromFile(path, out nint bitmap) != 0 || bitmap == 0)
            {
                return 0;
            }

            try
            {
                return GdipCreateHBITMAPFromBitmap(bitmap, out nint hBitmap, 0) == 0
                    ? hBitmap
                    : 0;
            }
            finally
            {
                _ = GdipDisposeImage(bitmap);
            }
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
}