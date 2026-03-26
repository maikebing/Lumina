using SkiaSharp;
using Svg.Skia;

var taskRunner = new AssetGenerator();
return taskRunner.Run(args);

internal sealed class AssetGenerator
{
    private static readonly AssetDefinition[] Assets =
    [
        new("lumina-small.svg", "Square44x44Logo.png", 44, 44),
        new("lumina-small.svg", "StoreLogo.png", 50, 50),
        new("lumina-glass.svg", "Square150x150Logo.png", 150, 150),
        new("lumina-wide.svg", "Wide310x150Logo.png", 310, 150),
        new("lumina-splash.svg", "SplashScreen.png", 620, 300),
    ];

    public int Run(string[] args)
    {
        if (args.Length is < 1 or > 2)
        {
            Console.Error.WriteLine("Usage: GenAssets <pkgDir> [assetsDir]");
            return 1;
        }

        var packageDirectory = Path.GetFullPath(args[0]);
        var assetsDirectory = args.Length == 2
            ? Path.GetFullPath(args[1])
            : ResolveAssetsDirectory();

        Directory.CreateDirectory(Path.Combine(packageDirectory, "Assets"));

        foreach (var asset in Assets)
        {
            Generate(asset, assetsDirectory, packageDirectory);
            Console.WriteLine($"Generated {asset.OutputFileName} ({asset.Width}x{asset.Height})");
        }

        Console.WriteLine("All assets generated.");
        return 0;
    }

    private static string ResolveAssetsDirectory()
    {
        var candidates = new[]
        {
            Path.Combine(Directory.GetCurrentDirectory(), "Assets"),
            Path.Combine(AppContext.BaseDirectory, "Assets"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Assets"),
            Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "Assets"),
        };

        foreach (var candidate in candidates.Select(Path.GetFullPath).Distinct(StringComparer.OrdinalIgnoreCase))
        {
            if (Assets.All(asset => File.Exists(Path.Combine(candidate, asset.SourceFileName))))
            {
                return candidate;
            }
        }

        throw new DirectoryNotFoundException("Could not locate the SVG Assets directory.");
    }

    private static void Generate(AssetDefinition asset, string assetsDirectory, string packageDirectory)
    {
        var sourcePath = Path.Combine(assetsDirectory, asset.SourceFileName);
        if (!File.Exists(sourcePath))
        {
            throw new FileNotFoundException("SVG asset was not found.", sourcePath);
        }

        using var svgStream = File.OpenRead(sourcePath);
        var svg = new SKSvg();
        var picture = svg.Load(svgStream) ?? throw new InvalidOperationException($"Failed to load SVG '{sourcePath}'.");

        var bounds = picture.CullRect;
        if (bounds.Width <= 0 || bounds.Height <= 0)
        {
            throw new InvalidOperationException($"SVG '{sourcePath}' has invalid bounds.");
        }

        var destinationPath = Path.Combine(packageDirectory, "Assets", asset.OutputFileName);
        using var surface = SKSurface.Create(new SKImageInfo(asset.Width, asset.Height, SKColorType.Rgba8888, SKAlphaType.Premul))
            ?? throw new InvalidOperationException($"Failed to create a drawing surface for '{destinationPath}'.");

        var canvas = surface.Canvas;
        canvas.Clear(SKColors.Transparent);
        canvas.Scale(asset.Width / bounds.Width, asset.Height / bounds.Height);
        canvas.Translate(-bounds.Left, -bounds.Top);
        canvas.DrawPicture(picture);
        canvas.Flush();

        using var image = surface.Snapshot();
        using var data = image.Encode(SKEncodedImageFormat.Png, quality: 100)
            ?? throw new InvalidOperationException($"Failed to encode '{destinationPath}' as PNG.");
        using var output = File.Open(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
        data.SaveTo(output);
    }
}

internal sealed record AssetDefinition(string SourceFileName, string OutputFileName, int Width, int Height);
