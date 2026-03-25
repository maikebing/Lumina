using Xunit;

namespace Lumina.Tests;

public class EffectKindTests
{
    [Fact]
    public void None_IsZero()
    {
        Assert.Equal(0, (int)EffectKind.None);
    }

    [Theory]
    [InlineData(EffectKind.None)]
    [InlineData(EffectKind.Acrylic)]
    [InlineData(EffectKind.Mica)]
    [InlineData(EffectKind.MicaAlt)]
    [InlineData(EffectKind.Aero)]
    [InlineData(EffectKind.Blur)]
    public void AllValues_AreDefined(EffectKind kind)
    {
        Assert.True(Enum.IsDefined(kind));
    }

    [Fact]
    public void TotalValueCount_IsSix()
    {
        // Guards against accidental addition/removal of enum members
        Assert.Equal(6, Enum.GetValues<EffectKind>().Length);
    }
}
