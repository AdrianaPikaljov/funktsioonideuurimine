using MathNet.Symbolics;
using System.Text;
using System.Text.RegularExpressions;
namespace FunktsiooniUurija.Services;

public record Samm(string Pealkiri, List<string> Read);

public static partial class Analuusija
{
    // ---------- abifunktsioonid ----------
    static string Asenda(string valem, double v) =>
        Regex.Replace(valem, @"(?<![a-z])x(?![a-z])", "(" + N(v) + ")");

    static string Mark(double v) => v > 0 ? "+" : "−";

    static (double a, double b, double c)? Ruutkuju(Func<double, double> g)
    {
        double c = g(0), p = g(1), m = g(-1);
        double a = (p + m) / 2 - c, b = (p - m) / 2;
        bool ok = Math.Abs(a) > 1e-12
               && Math.Abs(g(2) - (4 * a + 2 * b + c)) < 1e-9
               && Math.Abs(g(3) - (9 * a + 3 * b + c)) < 1e-9;
        return ok ? (a, b, c) : null;
    }

    // Kui f on polünoom (aste <= 8), tagastab kordajad c[k] (f = Σ c[k]·x^k), muidu null
    static double[]? Kordajad(SymbolicExpression e)
    {
        try
        {
            var x = SymbolicExpression.Variable("x");
            var cur = e; var c = new List<double>(); double fakt = 1;
            for (int k = 0; k <= 8; k++)
            {
                double v = cur.Compile("x")(0);
                if (!double.IsFinite(v)) return null;
                c.Add(v / fakt);
                cur = cur.Differentiate(x);
                fakt *= k + 1;
            }
            var chk = cur.Compile("x");
            foreach (var t in new[] { 0.0, 1.0, -1.7, 2.3 })
                if (Math.Abs(chk(t)) > 1e-9) return null;
            while (c.Count > 1 && Math.Abs(c[^1]) < 1e-12) c.RemoveAt(c.Count - 1);
            return c.ToArray();
        }
        catch { return null; }
    }

    static double[] Tuleta(double[] c)
    {
        if (c.Length <= 1) return new[] { 0.0 };
        var r = new double[c.Length - 1];
        for (int k = 1; k < c.Length; k++) r[k - 1] = c[k] * k;
        return r;
    }

    static string Murd(double v)
    {
        for (int q = 1; q <= 12; q++)
        {
            double p = v * q;
            if (Math.Abs(p - Math.Round(p)) < 1e-9)
                return q == 1 ? ((long)Math.Round(p)).ToString() : $"{(long)Math.Round(p)}/{q}";
        }
        return N(v);
    }

    static string Kords(double c) => (c < 0 ? "−" : "") + Murd(Math.Abs(c));

    static string Polu(double[] c)
    {
        var sb = new StringBuilder();
        for (int k = c.Length - 1; k >= 0; k--)
        {
            double v = c[k];
            if (Math.Abs(v) < 1e-12) continue;
            double av = Math.Abs(v);
            string koef = (Math.Abs(av - 1) < 1e-12 && k > 0) ? "" : Murd(av);
            string mid = k == 0 ? "" : (koef == "" ? "" : "·") + (k == 1 ? "x" : "x^" + k);
            string term = koef + mid;
            if (sb.Length == 0) sb.Append(v < 0 ? "−" : "").Append(term);
            else sb.Append(v < 0 ? " − " : " + ").Append(term);
        }
        return sb.Length == 0 ? "0" : sb.ToString();
    }

    static string Liige(double c, int k)
    {
        var a = new double[k + 1]; a[k] = c; return Polu(a);
    }

    static List<string> Liikmeti(double[] c)
    {
        var r = new List<string>();
        for (int k = c.Length - 1; k >= 0; k--)
        {
            if (Math.Abs(c[k]) < 1e-12) continue;
            string l = Liige(c[k], k);
            if (k == 0) r.Add($"({l})' = 0   (konstandi tuletis on null)");
            else if (k == 1) r.Add($"({l})' = {Kords(c[k])}");
            else r.Add($"({l})' = {k}·({Kords(c[k])})·{(k - 1 == 1 ? "x" : "x^" + (k - 1))} = {Liige(c[k] * k, k - 1)}");
        }
        return r;
    }

    // ---------- lahenduskäik ----------
    public static List<Samm> Lahenduskaik(string valem)
    {
        valem = Normaliseeri(valem);
        var x = SymbolicExpression.Variable("x");
        var expr = SymbolicExpression.Parse(valem);
        var d = expr.Differentiate(x);
        var d2 = d.Differentiate(x);
        var f = expr.Compile("x");
        var df = d.Compile("x");
        var d2f = d2.Compile("x");

        var kf = Kordajad(expr);
        var kd = kf == null ? null : Tuleta(kf);
        var kd2 = kd == null ? null : Tuleta(kd);
        string dStr = kd != null ? Polu(kd) : d.ToString();
        string d2Str = kd2 != null ? Polu(kd2) : d2.ToString();

        var sammud = new List<Samm>();
        var kokku = new List<string>();

        // 1. Määramispiirkond
        bool katkevus = false;
        for (double t = Min; t <= Max; t += 0.01) if (!double.IsFinite(f(t))) { katkevus = true; break; }
        var r1 = new List<string> { $"f(x) = {valem}" };
        if (kf != null) r1.Add($"See on polünoom (aste {kf.Length - 1}).");
        r1.Add(kf != null
            ? "Selgitus: Polünoomi väärtust saab arvutada iga x korral, seega piiranguid pole."
            : "Selgitus: Määramispiirkonda piiravad: nimetaja ≠ 0, juurealune ≥ 0, logaritmi argument > 0.");
        string xdom = katkevus ? "(-∞; ∞) miinus katkepunktid (kontrolli käsitsi)" : "(-∞; ∞)";
        r1.Add($"Tulemus: X = {xdom}");
        sammud.Add(new("Määramispiirkond", r1));
        kokku.Add($"Määramispiirkond: X = {xdom}");

        // 2. Tuletis
        var r2 = new List<string>();
        if (kf != null && kd != null)
        {
            r2.Add("Selgitus: Tuletame iga liikme eraldi: (c·xⁿ)' = c·n·xⁿ⁻¹, konstandi tuletis on 0.");
            r2.Add($"f(x) = {Polu(kf)}");
            r2.AddRange(Liikmeti(kf));
            r2.Add("Liidame liikmete tuletised kokku.");
        }
        else
        {
            r2.Add("Selgitus: Kasutame tuletise reegleid: (sin x)' = cos x, (eˣ)' = eˣ, (ln x)' = 1/x, korrutis-, jagatis- ja ketireeglit.");
        }
        r2.Add($"Tulemus: f'(x) = {dStr}");
        sammud.Add(new("Tuletis f'(x)", r2));
        kokku.Add($"Tuletis f'(x): {dStr}");

        // 3. Kriitilised punktid
        var krit = Nullkohad(df);
        var r3 = new List<string> {
            "Selgitus: Ekstreemum võib olla ainult seal, kus puutuja on horisontaalne, st f'(x) = 0. Lahendame selle võrrandi.",
            $"f'(x) = {dStr} = 0" };
        var rk = Ruutkuju(df);
        if (rk != null)
        {
            var (ka, kb, kc) = rk.Value;
            double D = kb * kb - 4 * ka * kc;
            r3.Add("See on ruutvõrrand kujul ax² + bx + c = 0");
            r3.Add($"a = {N(ka)},  b = {N(kb)},  c = {N(kc)}");
            r3.Add($"Diskriminant: D = b² − 4ac = ({N(kb)})² − 4·({N(ka)})·({N(kc)}) = {N(D)}");
            if (D > 1e-12)
            {
                double sd = Math.Sqrt(D);
                r3.Add($"D > 0 → kaks lahendit, √D = {N(sd)}");
                r3.Add($"x₁ = (−b − √D) / (2a) = ({N(-kb)} − {N(sd)}) / {N(2 * ka)} = {N((-kb - sd) / (2 * ka))}");
                r3.Add($"x₂ = (−b + √D) / (2a) = ({N(-kb)} + {N(sd)}) / {N(2 * ka)} = {N((-kb + sd) / (2 * ka))}");
            }
            else if (Math.Abs(D) <= 1e-12) r3.Add($"D = 0 → üks lahend x = −b / (2a) = {N(-kb / (2 * ka))}");
            else r3.Add("D < 0 → reaalseid lahendeid pole, kriitilisi punkte ei ole.");
        }
        else r3.Add("Lahendame võrrandi numbriliselt (märgimuutuse skaneerimine + poolitusmeetod) lõigus [-10; 10].");
        for (int i = 0; i < krit.Count; i++)
            r3.Add($"Kontroll: f'({N(krit[i])}) = {N(df(krit[i]))} ≈ 0 ✓");
        string kritS = krit.Count == 0 ? "puuduvad" : string.Join("; ", krit.Select(v => $"x = {N(v)}"));
        r3.Add($"Tulemus: kriitilised punktid: {kritS}");
        sammud.Add(new("Kriitilised punktid (f'(x) = 0)", r3));
        kokku.Add($"Kriitilised punktid: {kritS}");

        // 4. Märgitabel
        var punktid = new List<double> { double.NegativeInfinity };
        punktid.AddRange(krit);
        punktid.Add(double.PositiveInfinity);
        var r4 = new List<string> {
            "Selgitus: Kui f'(x) > 0, funktsioon kasvab; kui f'(x) < 0, kahaneb. Märk on igas intervallis sama, seega piisab ühest testpunktist." };
        var seg = new List<(double lo, double hi, bool up)>();
        for (int j = 0; j < punktid.Count - 1; j++)
        {
            double lo = punktid[j], hi = punktid[j + 1];
            double test = double.IsNegativeInfinity(lo) ? (double.IsPositiveInfinity(hi) ? 0 : hi - 1)
                        : double.IsPositiveInfinity(hi) ? lo + 1 : (lo + hi) / 2;
            double v = df(test);
            if (!double.IsFinite(v)) continue;
            r4.Add($"{Vahemik(lo, hi)}: testpunkt x = {N(test)}, f'({N(test)}) = {N(v)} → f'(x) {(v > 0 ? "> 0 → f kasvab ↗" : "< 0 → f kahaneb ↘")}");
            bool up = v > 0;
            if (seg.Count > 0 && seg[^1].up == up) seg[^1] = (seg[^1].lo, hi, up);
            else seg.Add((lo, hi, up));
        }
        var kasv = seg.Where(s => s.up).Select(s => Vahemik(s.lo, s.hi)).ToList();
        var kah = seg.Where(s => !s.up).Select(s => Vahemik(s.lo, s.hi)).ToList();
        string kasvS = kasv.Count == 0 ? "puudub" : string.Join(" ∪ ", kasv);
        string kahS = kah.Count == 0 ? "puudub" : string.Join(" ∪ ", kah);
        r4.Add($"Tulemus: kasvab {kasvS}");
        r4.Add($"Tulemus: kahaneb {kahS}");
        sammud.Add(new("Märgitabel: kasvamine ja kahanemine", r4));
        kokku.Add($"Kasvab: {kasvS}");
        kokku.Add($"Kahaneb: {kahS}");

        // 5. Ekstreemumid
        var r5 = new List<string> {
            "Selgitus: Kui f' muudab märki + → −, on maksimum; kui − → +, on miinimum. Kui märk ei muutu, ekstreemumit pole." };
        var maxid = new List<string>(); var minid = new List<string>();
        foreach (var k in krit)
        {
            double vasak = df(k - 0.01), parem = df(k + 0.01), y = f(k);
            string liik = vasak > 0 && parem < 0 ? "max" : vasak < 0 && parem > 0 ? "min" : "";
            r5.Add($"x = {N(k)}: f' märk {Mark(vasak)} → {Mark(parem)}"
                 + (liik == "max" ? " → maksimum" : liik == "min" ? " → miinimum" : " → märk ei muutu, ekstreemumit pole"));
            if (liik == "") continue;
            r5.Add($"f({N(k)}) = {Asenda(valem, k)} = {N(y)}");
            double s2 = d2f(k);
            if (double.IsFinite(s2))
                r5.Add($"Kontroll teise tuletisega: f''({N(k)}) = {N(s2)} {(s2 > 0 ? "> 0 → miinimum ✓" : s2 < 0 ? "< 0 → maksimum ✓" : "= 0 → test ei otsusta")}");
            r5.Add($"Tulemus: {(liik == "max" ? "maksimum" : "miinimum")} punktis ({N(k)}; {N(y)})");
            (liik == "max" ? maxid : minid).Add($"({N(k)}; {N(y)})");
        }
        if (krit.Count == 0) r5.Add("Tulemus: ekstreemumeid pole");
        sammud.Add(new("Ekstreemumid", r5));
        kokku.Add($"Maksimum: {(maxid.Count == 0 ? "puudub" : string.Join("; ", maxid))}");
        kokku.Add($"Miinimum: {(minid.Count == 0 ? "puudub" : string.Join("; ", minid))}");

        // 6. Teine tuletis ja käänupunkt
        var r6 = new List<string> {
            "Selgitus: Käänupunktis muutub graafiku kõverus. Seda otsime teise tuletise abil: f''(x) = 0 ja f'' muudab märki." };
        if (kd != null) { r6.Add($"f'(x) = {dStr}"); r6.AddRange(Liikmeti(kd)); }
        r6.Add($"f''(x) = {d2Str}");
        var kaanud = Nullkohad(d2f).Where(q => d2f(q - 0.01) * d2f(q + 0.01) < 0).ToList();
        var kaanS = new List<string>();
        foreach (var p in kaanud)
        {
            r6.Add($"f''(x) = 0 → x = {N(p)}; f'' märk {Mark(d2f(p - 0.01))} → {Mark(d2f(p + 0.01))}");
            r6.Add($"f({N(p)}) = {Asenda(valem, p)} = {N(f(p))}");
            r6.Add($"Tulemus: käänupunkt ({N(p)}; {N(f(p))})");
            kaanS.Add($"({N(p)}; {N(f(p))})");
        }
        if (kaanud.Count == 0) r6.Add("Tulemus: käänupunkti pole");
        var kp = new List<double> { double.NegativeInfinity };
        kp.AddRange(kaanud); kp.Add(double.PositiveInfinity);
        r6.Add("Selgitus: Kui f'' > 0, on graafik ∪-kujuline (avaneb ülespoole); kui f'' < 0, siis ∩-kujuline.");
        for (int j = 0; j < kp.Count - 1; j++)
        {
            double lo = kp[j], hi = kp[j + 1];
            double test = double.IsNegativeInfinity(lo) ? (double.IsPositiveInfinity(hi) ? 0 : hi - 1)
                        : double.IsPositiveInfinity(hi) ? lo + 1 : (lo + hi) / 2;
            double v = d2f(test);
            if (!double.IsFinite(v) || Math.Abs(v) < 1e-12) continue;
            r6.Add($"{Vahemik(lo, hi)}: f''({N(test)}) = {N(v)} → {(v > 0 ? "f'' > 0 → ∪-kujuline" : "f'' < 0 → ∩-kujuline")}");
        }
        sammud.Add(new("Teine tuletis ja käänupunkt", r6));
        kokku.Add($"Käänupunkt: {(kaanS.Count == 0 ? "puudub" : string.Join("; ", kaanS))}");

        // 7. Lõige y-teljega
        double y0 = f(0);
        string yS = double.IsFinite(y0) ? $"(0; {N(y0)})" : "puudub";
        sammud.Add(new("Lõige y-teljega", new() {
            "Selgitus: y-teljel on x = 0, seega arvutame f(0).",
            double.IsFinite(y0) ? $"f(0) = {Asenda(valem, 0)} = {N(y0)}" : "f(0) pole defineeritud.",
            $"Tulemus: lõikepunkt y-teljega {yS}" }));
        kokku.Add($"Lõige y-teljega: {yS}");

        // 8. Nullkohad
        var nullid = Nullkohad(f);
        var r8 = new List<string> {
            "Selgitus: x-telje lõikepunktid on seal, kus f(x) = 0. Otsime need numbriliselt (märgimuutus + poolitusmeetod) lõigus [-10; 10]." };
        foreach (var n in nullid) r8.Add($"Kontroll: f({N(n)}) = {N(f(n))} ≈ 0 ✓");
        string nullS = nullid.Count == 0 ? "puuduvad lõigus [-10; 10]" : string.Join("; ", nullid.Select(v => $"x ≈ {N(v)}"));
        r8.Add($"Tulemus: nullkohad: {nullS}");
        sammud.Add(new("Nullkohad (f(x) = 0)", r8));
        kokku.Add($"Nullkohad: {nullS}");

        // 9. Käitumine lõpmatuses (ainult polünoomile)
        if (kf != null && kf.Length > 1)
        {
            int n = kf.Length - 1; double cn = kf[n];
            bool paar = n % 2 == 0;
            string paremal = cn > 0 ? "+∞" : "−∞";
            string vasakul = (cn > 0) == paar ? "+∞" : "−∞";
            sammud.Add(new("Käitumine lõpmatuses", new() {
                "Selgitus: Suurtel |x| väärtustel domineerib kõrgeima astme liige.",
                $"Kõrgeima astme liige: {Liige(cn, n)}",
                $"Tulemus: kui x → +∞, siis f(x) → {paremal}",
                $"Tulemus: kui x → −∞, siis f(x) → {vasakul}" }));
        }

        // 10. Kokkuvõte
        sammud.Add(new("Kokkuvõte", kokku));
        return sammud;
    }
}