namespace FunktsiooniUurija.Services;

/// <summary>Mida uurimisel arvutada: valikute võtmed, nimed ja sõltuvused.</summary>
public static class Valikud
{
    public record Suvand(string Voti, string Nimi, string Kirjeldus, string[] Eeldab);

    // Järjekord = järjekord vormis ja lahenduskäigus
    public static readonly Suvand[] Koik =
    {
        new("maar",   "Määramispiirkond",          "Millised x väärtused on lubatud",           Array.Empty<string>()),
        new("tuletis","Tuletis f'(x)",             "Funktsiooni tuletis",                       Array.Empty<string>()),
        new("krit",   "Kriitilised punktid",       "Kus f'(x) = 0",                             new[] { "tuletis" }),
        new("kasv",   "Kasvamine ja kahanemine",   "Märgitabel f'(x) järgi",                    new[] { "krit" }),
        new("ekst",   "Ekstreemumid",              "Maksimumid ja miinimumid",                  new[] { "krit" }),
        new("kaan",   "Käänupunktid",              "Teine tuletis f''(x)",                      new[] { "tuletis" }),
        new("yloige", "Lõige y-teljega",           "f(0)",                                      Array.Empty<string>()),
        new("null",   "Nullkohad",                 "Lõige x-teljega, f(x) = 0",                 Array.Empty<string>()),
        new("lopmatus","Käitumine lõpmatuses",     "Mis juhtub, kui x → ±∞ (polünoomid)",       Array.Empty<string>()),
    };

    static readonly Dictionary<string, Suvand> Indeks = Koik.ToDictionary(s => s.Voti);

    /// <summary>Tühi sõne = kõik valitud (ka vanad salvestused).</summary>
    public static HashSet<string> Parsi(string? csv)
    {
        var tulemus = new HashSet<string>();
        if (string.IsNullOrWhiteSpace(csv)) { foreach (var s in Koik) tulemus.Add(s.Voti); return tulemus; }
        foreach (var v in csv.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            if (Indeks.ContainsKey(v)) tulemus.Add(v);
        return Sulund(tulemus);
    }

    /// <summary>Lisab valitud punktide eeldused (nt ekstreemumid vajavad kriitilisi punkte ja tuletist).</summary>
    public static HashSet<string> Sulund(IEnumerable<string>? valitud)
    {
        var tulemus = new HashSet<string>();
        void Lisa(string v)
        {
            if (!Indeks.TryGetValue(v, out var s) || !tulemus.Add(v)) return;
            foreach (var e in s.Eeldab) Lisa(e);
        }
        foreach (var v in valitud ?? Array.Empty<string>()) Lisa(v);
        return tulemus;
    }

    /// <summary>Vormi kaudu saadud valikud → andmebaasi salvestatav sõne (kanoonilises järjekorras).</summary>
    public static string Salvesta(IEnumerable<string>? valitud) =>
        string.Join(",", Koik.Select(s => s.Voti).Where(Sulund(valitud).Contains));
}
