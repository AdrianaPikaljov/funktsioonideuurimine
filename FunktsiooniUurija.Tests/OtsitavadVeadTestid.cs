using FunktsiooniUurija.Services;
using Xunit;

namespace FunktsiooniUurija.Tests;

/// <summary>
/// Uurivad testid: need kirjeldavad matemaatiliselt õiget käitumist piirjuhtudel.
/// Kui test kukub läbi, kantakse see vearaportisse (juhendi peatükk 2.8).
/// </summary>
public class OtsitavadVeadTestid
{
    [Fact]
    public void Maaramispiirkond_PoordArv_TuvastatakseKatkepunkt()
    {
        // f(x) = 1/x ei ole määratud punktis x = 0
        var o = Abi.Uuri("1/x");
        Assert.Contains("katkepunkt", o.Maaramispiirkond);
    }

    [Fact]
    public void Maaramispiirkond_Logaritm_EiOleKoikReaalarvud()
    {
        // ln(x) on määratud ainult x > 0, seega (-∞; ∞) on vale
        var o = Abi.Uuri("ln(x)");
        Assert.DoesNotContain("(-∞; ∞)", o.Maaramispiirkond);
    }

    [Fact]
    public void Normaliseeri_FunktsiooniNimiNumbriga_EiLisaTaheliKorrutusmarki()
    {
        // "log10(x)" peab jääma samaks, mitte muutuma kujule "log10*(x)"
        Assert.Equal("log10(x)", Analuusija.Normaliseeri("log10(x)"));
    }

    [Fact]
    public void Nullkohad_KahekordneJuurePoleRuudusKaugel_Leitakse()
    {
        // (x - 0.5)^2 puudutab telge punktis x = 0.5 ilma märki vahetamata
        Assert.Equal("x = 0.5", Abi.Uuri("(x - 0.5)^2").Nullkohad);
    }
}
