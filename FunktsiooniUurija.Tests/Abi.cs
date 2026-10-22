using System.Globalization;
using System.Text.RegularExpressions;
using FunktsiooniUurija.Models;
using FunktsiooniUurija.Services;

namespace FunktsiooniUurija.Tests;

/// <summary>Abimeetodid: käivitavad rakenduse analüüsi ja loevad tekstilisi tulemusi arvudeks.</summary>
public static class Abi
{
    /// <summary>Arvutab kõik uuringud (Valik = tühi → kõik) ja tagastab täidetud olemi.</summary>
    public static FunktsiooniUurimine Uuri(string valem)
    {
        var o = new FunktsiooniUurimine { Valem = valem };
        Analuusija.Taida(o, Valikud.Parsi(""));
        return o;
    }

    /// <summary>Arvutab ainult etteantud uuringud (koos eeldustega).</summary>
    public static FunktsiooniUurimine Uuri(string valem, params string[] valik)
    {
        var o = new FunktsiooniUurimine { Valem = valem };
        Analuusija.Taida(o, Valikud.Sulund(valik));
        return o;
    }

    /// <summary>Teisendab rakenduse tuletise teksti (nt "-3 + 3*x^2") funktsiooniks.</summary>
    public static Func<double, double> TuletiseFunktsioon(FunktsiooniUurimine o) =>
        Analuusija.Kompileeri(o.Tuletis);
}
