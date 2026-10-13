using FunktsiooniUurija.Data;
using FunktsiooniUurija.Models;
using FunktsiooniUurija.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace FunktsiooniUurija.Controllers;

public class FunktsioonidController : Controller
{
    private readonly AndmebaasiKontekst _kontekst;
    public FunktsioonidController(AndmebaasiKontekst kontekst) => _kontekst = kontekst;

    // ?lemmik=true näitab ainult lemmikuid
    public async Task<IActionResult> Index(bool lemmik = false)
    {
        var paring = _kontekst.FunktsiooniUurimised.AsQueryable();
        if (lemmik) paring = paring.Where(x => x.Lemmik);
        ViewBag.Lemmikud = lemmik;
        return View(await paring.OrderByDescending(x => x.LuodudAeg).ThenByDescending(x => x.Id).ToListAsync());
    }

    public async Task<IActionResult> Details(int id)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        try { ViewBag.Lahendus = Analuusija.Lahenduskaik(olem.Valem, Valikud.Parsi(olem.Valik)); } catch { }
        return View(olem);
    }

    public IActionResult Create() => View(new FunktsiooniUurimine { Valik = Valikud.Salvesta(Valikud.Koik.Select(s => s.Voti)) });

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(string? valem, string[]? valikud)
    {
        var olem = new FunktsiooniUurimine { Valem = valem ?? "", Valik = Valikud.Salvesta(valikud) };
        if (Kontrolli(olem))
        {
            olem.LuodudAeg = DateTime.UtcNow;
            await _kontekst.FunktsiooniUurimised.AddAsync(olem);
            await _kontekst.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id = olem.Id });
        }
        return View(olem);
    }

    public async Task<IActionResult> Edit(int id)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        // vanad salvestused (Valik tühi) = kõik arvutused
        olem.Valik = Valikud.Salvesta(Valikud.Parsi(olem.Valik));
        return View(olem);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string? valem, string[]? valikud)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        olem.Valem = valem ?? "";
        olem.Valik = Valikud.Salvesta(valikud);
        if (Kontrolli(olem))
        {
            await _kontekst.SaveChangesAsync();
            return RedirectToAction(nameof(Details), new { id });
        }
        return View(olem);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem != null) { _kontekst.FunktsiooniUurimised.Remove(olem); await _kontekst.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Lemmik(int id, string? tagasi)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem != null) { olem.Lemmik = !olem.Lemmik; await _kontekst.SaveChangesAsync(); }
        if (!string.IsNullOrEmpty(tagasi) && Url.IsLocalUrl(tagasi)) return LocalRedirect(tagasi);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Funktsioonid/Punktid/{id:int}")]
    public async Task<IActionResult> Punktid(int id, double min = -5, double max = 5, double samm = 0.1)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        if (min >= max || samm <= 0 || (max - min) / samm > 5000) return BadRequest("Vale vahemik");
        var f = Analuusija.Kompileeri(olem.Valem);
        var xs = new List<double>(); var ys = new List<double?>();
        for (double x = min; x <= max + 1e-9; x += samm)
        {
            var y = f(x);
            xs.Add(Math.Round(x, 4));
            ys.Add(double.IsFinite(y) ? Math.Round(y, 5) : null);
        }
        return Json(new { x = xs, y = ys });
    }

    // Kontrollib valemit ja arvutab valitud tulemused
    private bool Kontrolli(FunktsiooniUurimine olem)
    {
        if (string.IsNullOrWhiteSpace(olem.Valem)) { ModelState.AddModelError("valem", "Valem on kohustuslik"); return false; }
        if (olem.Valem.Length > 200) { ModelState.AddModelError("valem", "Valem on liiga pikk (max 200)"); return false; }
        if (string.IsNullOrEmpty(olem.Valik)) { ModelState.AddModelError("valikud", "Vali vähemalt üks asi, mida arvutada"); return false; }
        try { Analuusija.Taida(olem, Valikud.Parsi(olem.Valik)); return true; }
        catch (Exception)
        {
            ModelState.AddModelError("valem", "Valemit ei õnnestunud tõlgendada. Kasuta nt: x^3 - 3*x, sin(x), exp(x)/2");
            return false;
        }
    }
}
