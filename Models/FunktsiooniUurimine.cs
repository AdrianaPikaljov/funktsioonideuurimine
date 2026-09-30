using System.ComponentModel.DataAnnotations;
namespace FunktsiooniUurija.Models;

public class FunktsiooniUurimine
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Valem on kohustuslik")]
    [StringLength(200, ErrorMessage = "Valem on liiga pikk (max 200)")]
    [Display(Name = "Valem f(x)")]
    public string Valem { get; set; } = string.Empty;

    [Display(Name = "Määramispiirkond")]
    public string Maaramispiirkond { get; set; } = string.Empty;

    [Display(Name = "Nullkohad")]
    public string Nullkohad { get; set; } = string.Empty;

    [Display(Name = "Tuletis f'(x)")]
    public string Tuletis { get; set; } = string.Empty;

    [Display(Name = "Kriitilised punktid")]
    public string KriitilisedPunktid { get; set; } = string.Empty;

    [Display(Name = "Ekstreemumid")]
    public string Ekstreemumid { get; set; } = string.Empty;

    [Display(Name = "Kasvab")]
    public string Kasvamine { get; set; } = string.Empty;

    [Display(Name = "Kahaneb")]
    public string Kahanemine { get; set; } = string.Empty;

    [Display(Name = "Käänupunkt")]
    public string Kaanupunkt { get; set; } = string.Empty;

    [Display(Name = "Lõige y-teljega")]
    public string LoigeYTeljega { get; set; } = string.Empty;

    [Display(Name = "Loodud")]
    public DateTime LuodudAeg { get; set; } = DateTime.UtcNow;
}
