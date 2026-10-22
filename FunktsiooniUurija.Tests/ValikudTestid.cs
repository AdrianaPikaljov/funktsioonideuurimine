using FunktsiooniUurija.Services;
using Xunit;

namespace FunktsiooniUurija.Tests;

/// <summary>Uuringute valikute loogika (Valikud.Parsi, Sulund, Salvesta).</summary>
public class ValikudTestid
{
    [Fact]
    public void Parsi_TuhiSone_ValibKoikArvutused()
    {
        Assert.Equal(Valikud.Koik.Length, Valikud.Parsi("").Count);
        Assert.Equal(Valikud.Koik.Length, Valikud.Parsi(null).Count);
    }

    [Fact]
    public void Parsi_TundmatuVoti_Ignoreeritakse()
    {
        Assert.Empty(Valikud.Parsi("hakkimine"));
    }

    [Fact]
    public void Sulund_Ekstreemumid_LisabKriitilisedPunktidJaTuletise()
    {
        var tulemus = Valikud.Sulund(new[] { "ekst" });
        Assert.Equal(new[] { "ekst", "krit", "tuletis" }.OrderBy(s => s), tulemus.OrderBy(s => s));
    }

    [Fact]
    public void Salvesta_Ekstreemumid_AnnabKanoonilisesJarjekorras()
    {
        Assert.Equal("tuletis,krit,ekst", Valikud.Salvesta(new[] { "ekst" }));
    }

    [Fact]
    public void Salvesta_JarjekordSisendisEiMangi()
    {
        Assert.Equal("maar,tuletis,krit,kasv", Valikud.Salvesta(new[] { "kasv", "maar" }));
    }

    [Fact]
    public void Salvesta_Null_AnnabTuhjaSone()
    {
        Assert.Equal("", Valikud.Salvesta(null));
    }
}
