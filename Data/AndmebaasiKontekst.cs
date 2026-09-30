using FunktsiooniUurija.Models;
using Microsoft.EntityFrameworkCore;
namespace FunktsiooniUurija.Data;

public class AndmebaasiKontekst : DbContext
{
    public AndmebaasiKontekst(DbContextOptions<AndmebaasiKontekst> options) : base(options) { }
    public DbSet<FunktsiooniUurimine> FunktsiooniUurimised => Set<FunktsiooniUurimine>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        var aeg = new DateTime(2026, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        mb.Entity<FunktsiooniUurimine>().HasData(
            new FunktsiooniUurimine { Id = 1, Valem = "x^3 - 3*x", Maaramispiirkond = "(-∞; ∞)",
                Nullkohad = "x = -1.7321; x = 0; x = 1.7321", Tuletis = "-3 + 3*x^2",
                KriitilisedPunktid = "x = -1; x = 1", Ekstreemumid = "max: f(-1) = 2; min: f(1) = -2", LuodudAeg = aeg },
            new FunktsiooniUurimine { Id = 2, Valem = "x^2 - 4*x + 3", Maaramispiirkond = "(-∞; ∞)",
                Nullkohad = "x = 1; x = 3", Tuletis = "-4 + 2*x",
                KriitilisedPunktid = "x = 2", Ekstreemumid = "min: f(2) = -1", LuodudAeg = aeg });
    }
}
