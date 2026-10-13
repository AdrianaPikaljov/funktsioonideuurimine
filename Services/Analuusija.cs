using FunktsiooniUurija.Models;
using MathNet.Symbolics;
using System.Globalization;
using System.Text.RegularExpressions;
namespace FunktsiooniUurija.Services;

public static partial class Analuusija
{
    const double Min = -10, Max = 10, Samm = 0.001;

    static string N(double v)
    {
        var r = Math.Round(v, 4);
        if (r == 0) r = 0;
        return r.ToString("0.####", CultureInfo.InvariantCulture);
    }

    public static string Normaliseeri(string s)
    {
        s = s.Replace(" ", "").Replace(",", ".").ToLowerInvariant();
        s = Regex.Replace(s, @"^(f\(x\)|y)=", "");
        s = Regex.Replace(s, @"(\d)(x|\()", "$1*$2");
        s = Regex.Replace(s, @"(x|\))(\d|x|\()", "$1*$2");
        return s;
    }

    static string Vahemik(double a, double b) =>
        $"({(double.IsNegativeInfinity(a) ? "-∞" : N(a))}; {(double.IsPositiveInfinity(b) ? "∞" : N(b))})";

    public static Func<double, double> Kompileeri(string valem) =>
        SymbolicExpression.Parse(Normaliseeri(valem)).Compile("x");

    /// <summary>Arvutab ainult valitud (ja nende eelduseks olevad) tulemused; ülejäänud väljad jäävad tühjaks.</summary>
    public static void Taida(FunktsiooniUurimine o, ISet<string>? valik = null)
    {
        valik ??= Valikud.Parsi(o.Valik);
        o.Valem = Normaliseeri(o.Valem);
        var x = SymbolicExpression.Variable("x");
        var expr = SymbolicExpression.Parse(o.Valem);
        var d = expr.Differentiate(x);
        var d2 = d.Differentiate(x);
        var f = expr.Compile("x");
        var df = d.Compile("x");
        var d2f = d2.Compile("x");

        o.Maaramispiirkond = o.Tuletis = o.Nullkohad = o.KriitilisedPunktid = o.Ekstreemumid = "";
        o.Kasvamine = o.Kahanemine = o.Kaanupunkt = o.LoigeYTeljega = "";

        if (valik.Contains("tuletis")) o.Tuletis = d.ToString();

        if (valik.Contains("maar"))
        {
            bool katkevus = false;
            for (double t = Min; t <= Max; t += 0.01)
                if (!double.IsFinite(f(t))) { katkevus = true; break; }
            o.Maaramispiirkond = katkevus ? "(-∞; ∞) miinus katkepunktid (kontrolli käsitsi)" : "(-∞; ∞)";
        }

        if (valik.Contains("null"))
        {
            var nullid = Nullkohad(f);
            o.Nullkohad = nullid.Count == 0 ? "puuduvad lõigus [-10; 10]" : string.Join("; ", nullid.Select(v => $"x = {N(v)}"));
        }

        var krit = valik.Contains("krit") ? Nullkohad(df) : new List<double>();
        if (valik.Contains("krit"))
            o.KriitilisedPunktid = krit.Count == 0 ? "puuduvad" : string.Join("; ", krit.Select(v => $"x = {N(v)}"));

        if (valik.Contains("ekst"))
        {
            var ekst = new List<string>();
            foreach (var k in krit)
            {
                double h = 0.01, vasak = df(k - h), parem = df(k + h);
                if (vasak > 0 && parem < 0) ekst.Add($"max: ({N(k)}; {N(f(k))})");
                else if (vasak < 0 && parem > 0) ekst.Add($"min: ({N(k)}; {N(f(k))})");
            }
            o.Ekstreemumid = ekst.Count == 0 ? "puuduvad" : string.Join("; ", ekst);
        }

        if (valik.Contains("kasv"))
        {
            var punktid = new List<double> { double.NegativeInfinity };
            punktid.AddRange(krit);
            punktid.Add(double.PositiveInfinity);
            var seg = new List<(double lo, double hi, bool up)>();
            for (int i = 0; i < punktid.Count - 1; i++)
            {
                double lo = punktid[i], hi = punktid[i + 1];
                double test = double.IsNegativeInfinity(lo) ? (double.IsPositiveInfinity(hi) ? 0 : hi - 1)
                            : double.IsPositiveInfinity(hi) ? lo + 1 : (lo + hi) / 2;
                double v = df(test);
                if (!double.IsFinite(v) || Math.Abs(v) < 1e-12) continue;
                bool up = v > 0;
                if (seg.Count > 0 && seg[^1].up == up) seg[^1] = (seg[^1].lo, hi, up);
                else seg.Add((lo, hi, up));
            }
            var kasv = seg.Where(s => s.up).Select(s => Vahemik(s.lo, s.hi)).ToList();
            var kah = seg.Where(s => !s.up).Select(s => Vahemik(s.lo, s.hi)).ToList();
            o.Kasvamine = kasv.Count == 0 ? "puudub" : string.Join(" ∪ ", kasv);
            o.Kahanemine = kah.Count == 0 ? "puudub" : string.Join(" ∪ ", kah);
        }

        if (valik.Contains("kaan"))
        {
            var kaanud = Nullkohad(d2f)
                .Where(q => d2f(q - 0.01) * d2f(q + 0.01) < 0)
                .Select(q => $"({N(q)}; {N(f(q))})").ToList();
            o.Kaanupunkt = kaanud.Count == 0 ? "puudub" : string.Join("; ", kaanud);
        }

        if (valik.Contains("yloige"))
        {
            double y0 = f(0);
            o.LoigeYTeljega = double.IsFinite(y0) ? $"(0; {N(y0)})" : "puudub";
        }
    }

    static List<double> Nullkohad(Func<double, double> g)
    {
        var leitud = new List<double>();
        double a = Min, ga = g(a);
        for (double b = Min + Samm; b <= Max + 1e-9; b += Samm)
        {
            double gb = g(b);
            if (double.IsFinite(ga) && double.IsFinite(gb))
            {
                if (Math.Abs(ga) < 1e-12) leitud.Add(a);
                else if (ga * gb < 0)
                {
                    double l = a, r = b;
                    for (int i = 0; i < 60; i++)
                    {
                        double m = (l + r) / 2;
                        if (g(l) * g(m) <= 0) r = m; else l = m;
                    }
                    leitud.Add((l + r) / 2);
                }
            }
            a = b; ga = gb;
        }
        var tulemus = new List<double>();
        foreach (var v in leitud.Select(v => Math.Abs(v) < 1e-7 ? 0 : v).OrderBy(v => v))
            if (tulemus.Count == 0 || Math.Abs(tulemus[^1] - v) > 1e-3) tulemus.Add(v);
        return tulemus;
    }
}