using FunktsiooniUurija.Services;
using Xunit;

namespace FunktsiooniUurija.Tests;

/// <summary>Sisendi normaliseerimine (Analuusija.Normaliseeri).</summary>
public class NormaliseeriTestid
{
    [Theory]
    [InlineData("2x^2", "2*x^2")]
    [InlineData("f(x) = x^2 + 3x", "x^2+3*x")]
    [InlineData("y=x^2", "x^2")]
    [InlineData("0,5x", "0.5*x")]
    [InlineData("X^2", "x^2")]
    [InlineData("x(x+1)", "x*(x+1)")]
    [InlineData("(x+1)(x-1)", "(x+1)*(x-1)")]
    public void Normaliseeri_Sisend_ToodabKanoonilisseVormi(string sisend, string oodatud)
    {
        Assert.Equal(oodatud, Analuusija.Normaliseeri(sisend));
    }
}
