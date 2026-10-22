using FunktsiooniUurija.Services;
using Xunit;

namespace FunktsiooniUurija.Tests;

/// <summary>
/// Matemaatika testid. Oodatavad väärtused on juhendi tabeli 3 kontrollväärtused
/// ning testija valitud funktsioon x^4 - 2*x^2 (käsitsi arvutus lisas 3).
/// </summary>
public class AnaluusijaTestid
{
    // ---------- tuletis ----------

    [Fact]
    public void Tuletis_KuupFunktsioon_TagastabOigeVaartuse()
    {
        // f(x) = x^3 - 3x, seega f'(2) = 3 * 2^2 - 3 = 9
        var f1 = Abi.TuletiseFunktsioon(Abi.Uuri("x^3 - 3*x"));
        Assert.Equal(9, f1(2), 3);   // täpsus 3 komakohta
        Assert.Equal(-3, f1(0), 3);  // f'(0) = -3
        Assert.Equal(0, f1(1), 3);   // f'(1) = 0
    }

    [Fact]
    public void Tuletis_RuutFunktsioon_TagastabOigeVaartuse()
    {
        // f(x) = x^2 - 4x + 3, seega f'(x) = 2x - 4
        var d = Abi.TuletiseFunktsioon(Abi.Uuri("x^2 - 4*x + 3"));
        Assert.Equal(-4, d(0), 3);
        Assert.Equal(0, d(2), 3);
        Assert.Equal(6, d(5), 3);
    }

    [Fact]
    public void Tuletis_NeljandaAste_TagastabOigeVaartuse()
    {
        // f(x) = x^4 - 2x^2, seega f'(x) = 4x^3 - 4x, f'(2) = 32 - 8 = 24
        var d = Abi.TuletiseFunktsioon(Abi.Uuri("x^4 - 2*x^2"));
        Assert.Equal(24, d(2), 3);
        Assert.Equal(0, d(1), 3);
    }

    // ---------- nullkohad ----------

    [Theory]
    [InlineData("x^2 - 4*x + 3", "x = 1; x = 3")]
    [InlineData("x^3 - 3*x", "x = -1.7321; x = 0; x = 1.7321")]      // 0 ja ±√3
    [InlineData("x^3 - 12*x", "x = -3.4641; x = 0; x = 3.4641")]     // 0 ja ±2√3
    [InlineData("x^4 - 2*x^2", "x = -1.4142; x = 0; x = 1.4142")]    // 0 ja ±√2
    public void Nullkohad_KontrollFunktsioonid_OnOiged(string valem, string oodatud)
    {
        Assert.Equal(oodatud, Abi.Uuri(valem).Nullkohad);
    }

    [Fact]
    public void Nullkohad_FunktsioonIlmaNullkohtadeta_ArveteataPuuduvad()
    {
        // x^2 + 1 > 0 kõikjal
        Assert.StartsWith("puuduvad", Abi.Uuri("x^2 + 1").Nullkohad);
    }

    // ---------- kriitilised punktid ----------

    [Theory]
    [InlineData("x^2 - 4*x + 3", "x = 2")]
    [InlineData("x^3 - 3*x", "x = -1; x = 1")]
    [InlineData("x^3 - 12*x", "x = -2; x = 2")]
    [InlineData("x^4 - 2*x^2", "x = -1; x = 0; x = 1")]
    public void KriitilisedPunktid_KontrollFunktsioonid_OnOiged(string valem, string oodatud)
    {
        Assert.Equal(oodatud, Abi.Uuri(valem).KriitilisedPunktid);
    }

    // ---------- ekstreemumid ----------

    [Theory]
    [InlineData("x^2 - 4*x + 3", "min: (2; -1)")]
    [InlineData("x^3 - 3*x", "max: (-1; 2); min: (1; -2)")]
    [InlineData("x^3 - 12*x", "max: (-2; 16); min: (2; -16)")]
    [InlineData("x^4 - 2*x^2", "min: (-1; -1); max: (0; 0); min: (1; -1)")]
    public void Ekstreemumid_KontrollFunktsioonid_OnOiged(string valem, string oodatud)
    {
        Assert.Equal(oodatud, Abi.Uuri(valem).Ekstreemumid);
    }

    [Fact]
    public void Ekstreemumid_MonotoonneFunktsioon_Puuduvad()
    {
        // f(x) = x^3 + x, f'(x) = 3x^2 + 1 > 0
        Assert.Equal("puuduvad", Abi.Uuri("x^3 + x").Ekstreemumid);
    }

    // ---------- kasvamine, käänupunkt, y-lõige, määramispiirkond ----------

    [Fact]
    public void Kasvamine_KuupFunktsioon_OnOige()
    {
        var o = Abi.Uuri("x^3 - 3*x");
        Assert.Equal("(-∞; -1) ∪ (1; ∞)", o.Kasvamine);
        Assert.Equal("(-1; 1)", o.Kahanemine);
    }

    [Fact]
    public void Kaanupunkt_KuupFunktsioon_OnNullis()
    {
        // f''(x) = 6x, käänupunkt (0; 0)
        Assert.Equal("(0; 0)", Abi.Uuri("x^3 - 3*x").Kaanupunkt);
    }

    [Fact]
    public void LoigeYTeljega_RuutFunktsioon_OnF0()
    {
        // f(0) = 3
        Assert.Equal("(0; 3)", Abi.Uuri("x^2 - 4*x + 3").LoigeYTeljega);
    }

    [Fact]
    public void Maaramispiirkond_Polunoom_OnKoikReaalarvud()
    {
        Assert.Equal("(-∞; ∞)", Abi.Uuri("x^2 - 4*x + 3").Maaramispiirkond);
    }

    // ---------- valikuline arvutus ----------

    [Fact]
    public void Taida_AinultNullkohad_JatabTuletiseTuhjaks()
    {
        var o = Abi.Uuri("x^2 - 4*x + 3", "null");
        Assert.Equal("x = 1; x = 3", o.Nullkohad);
        Assert.Equal("", o.Tuletis);
        Assert.Equal("", o.Ekstreemumid);
    }

    [Fact]
    public void Taida_EkstreemumidVajavadKriitilisiPunkteJaTuletist_ArvutatakseAutomaatselt()
    {
        // Valikud.Sulund lisab eeldused: ekst → krit → tuletis
        var o = Abi.Uuri("x^3 - 3*x", "ekst");
        Assert.NotEqual("", o.Ekstreemumid);
        Assert.NotEqual("", o.KriitilisedPunktid);
        Assert.NotEqual("", o.Tuletis);
    }

    // ---------- vigane sisend ----------

    [Theory]
    [InlineData("x^^2")]
    [InlineData("sin(x, 3*")]
    [InlineData("")]
    public void Taida_ViganeValem_ViskabErindi(string valem)
    {
        // Kontroller püüab erindi kinni ja näitab kasutajale veateadet
        Assert.ThrowsAny<Exception>(() => Abi.Uuri(valem));
    }

    // ---------- kompileerimine ----------

    [Fact]
    public void Kompileeri_Ruut_TagastabOigeVaartuse()
    {
        Assert.Equal(9, Analuusija.Kompileeri("x^2")(3), 3);
    }

    [Fact]
    public void Kompileeri_KorrutusmarguPuudub_LisatakseAutomaatselt()
    {
        // "2x" → "2*x"
        Assert.Equal(8, Analuusija.Kompileeri("2x")(4), 3);
    }
}
