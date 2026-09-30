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

    public async Task<IActionResult> Index() =>
        View(await _kontekst.FunktsiooniUurimised.OrderByDescending(x => x.LuodudAeg).ToListAsync());

    public async Task<IActionResult> Details(int id)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        try { ViewBag.Lahendus = Analuusija.Lahenduskaik(olem.Valem); } catch { }
        return View(olem);
    }

    public IActionResult Create() => View(new FunktsiooniUurimine());

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(FunktsiooniUurimine olem)
    {
        olem.Id = 0;
        if (ModelState.IsValid && Arvuta(olem))
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
        return olem == null ? NotFound() : View(olem);
    }

    [HttpPost, ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, FunktsiooniUurimine vorm)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        olem.Valem = vorm.Valem;
        if (ModelState.IsValid && Arvuta(olem))
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

    [HttpGet("Funktsioonid/Punktid/{id:int}")]
    public async Task<IActionResult> Punktid(int id, double min = -5, double max = 5, double samm = 0.1)
    {
        var olem = await _kontekst.FunktsiooniUurimised.FindAsync(id);
        if (olem == null) return NotFound();
        if (min >= max || samm <= 0 || (max - min) / samm > 5000) return BadRequest("Vale vahemik");
        var (f, df) = Analuusija.Kompileeri(olem.Valem);
        var xs = new List<double>(); var ys = new List<double?>(); var dys = new List<double?>();
        for (double x = min; x <= max + 1e-9; x += samm)
        {
            var y = f(x); var dy = df(x);
            xs.Add(Math.Round(x, 4));
            ys.Add(double.IsFinite(y) ? Math.Round(y, 5) : null);
            dys.Add(double.IsFinite(dy) ? Math.Round(dy, 5) : null);
        }
        return Json(new { x = xs, y = ys, dy = dys });
    }

    private bool Arvuta(FunktsiooniUurimine olem)
    {
        try { Analuusija.Taida(olem); return true; }
        catch (Exception)
        {
            ModelState.AddModelError(nameof(olem.Valem), "Valemit ei õnnestunud tõlgendada. Kasuta nt: x^3 - 3*x, sin(x), exp(x)/2");
            return false;
        }
    }
}
