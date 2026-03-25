using System.Text.Json;
using Xunit;

namespace Lumina.Tests;

public class EffectProfileTests
{
    // ── 构造与默认值 ────────────────────────────────────────────

    [Fact]
    public void DefaultProfile_HasExpectedValues()
    {
        var p = new EffectProfile();
        Assert.Equal("Default", p.Name);
        Assert.Equal(EffectKind.Mica, p.Kind);
        Assert.Equal(0x80_00_00_00u, p.BlendColor);
        Assert.Equal(20, p.BlurRadius);
        Assert.Equal(0.8f, p.Opacity);
    }

    // ── ToOptions 映射 ──────────────────────────────────────────

    [Fact]
    public void ToOptions_MapsAllFields()
    {
        var p = new EffectProfile { BlendColor = 0xFF_00_FF_00u, BlurRadius = 42, Opacity = 0.5f };
        var opts = p.ToOptions();
        Assert.Equal(p.BlendColor, opts.BlendColor);
        Assert.Equal(p.BlurRadius, opts.BlurRadius);
        Assert.Equal(p.Opacity,    opts.Opacity);
    }

    // ── JSON 序列化往返 ─────────────────────────────────────────

    [Fact]
    public void ToJson_FromJson_RoundTrip()
    {
        var original = new EffectProfile
        {
            Name       = "MyProfile",
            Kind       = EffectKind.Acrylic,
            BlendColor = 0xAA_BB_CC_DDu,
            BlurRadius = 35,
            Opacity    = 0.6f,
        };

        string json     = original.ToJson();
        var    restored = EffectProfile.FromJson(json);

        Assert.Equal(original.Name,       restored.Name);
        Assert.Equal(original.Kind,       restored.Kind);
        Assert.Equal(original.BlendColor, restored.BlendColor);
        Assert.Equal(original.BlurRadius, restored.BlurRadius);
        Assert.Equal(original.Opacity,    restored.Opacity, precision: 4);
    }

    [Fact]
    public void FromJson_InvalidJson_ThrowsJsonException()
    {
        Assert.Throws<JsonException>(() => EffectProfile.FromJson("{not valid}"));
    }

    [Fact]
    public void FromJson_NullResult_ThrowsJsonException()
    {
        // JSON literal "null" deserializes to null → should throw
        Assert.Throws<JsonException>(() => EffectProfile.FromJson("null"));
    }

    // ── 文件持久化 ──────────────────────────────────────────────

    [Fact]
    public void SaveJson_LoadJson_RoundTrip()
    {
        var original = new EffectProfile { Name = "FileTest", Kind = EffectKind.Blur, BlurRadius = 50 };
        string path  = Path.Combine(Path.GetTempPath(), $"lumina_test_{Guid.NewGuid():N}.json");

        try
        {
            original.SaveJson(path);
            Assert.True(File.Exists(path));

            var loaded = EffectProfile.LoadJson(path);
            Assert.Equal(original.Name,       loaded.Name);
            Assert.Equal(original.Kind,       loaded.Kind);
            Assert.Equal(original.BlurRadius, loaded.BlurRadius);
        }
        finally
        {
            if (File.Exists(path)) File.Delete(path);
        }
    }
}
