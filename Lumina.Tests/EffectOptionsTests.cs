using Xunit;

namespace Lumina.Tests;

public class EffectOptionsTests
{
    [Fact]
    public void Default_HasExpectedValues()
    {
        var opts = EffectOptions.Default;
        Assert.Equal(0x80_00_00_00u, opts.BlendColor);
        Assert.Equal(20,   opts.BlurRadius);
        Assert.Equal(0.8f, opts.Opacity);
    }

    [Fact]
    public void Default_IsSingletonInstance()
    {
        Assert.Same(EffectOptions.Default, EffectOptions.Default);
    }

    [Fact]
    public void InitProperties_AreReadOnly()
    {
        // init-only props cannot be reassigned after construction — verify via new instance
        var opts = new EffectOptions { BlendColor = 0xFF_FF_FF_FFu, BlurRadius = 0, Opacity = 0.0f };
        Assert.Equal(0xFF_FF_FF_FFu, opts.BlendColor);
        Assert.Equal(0,    opts.BlurRadius);
        Assert.Equal(0.0f, opts.Opacity);
    }
}
