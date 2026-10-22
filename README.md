# Funktsioonide uurija
Vajalik: .NET 10 SDK.

    dotnet tool install --global dotnet-ef
    dotnet ef migrations add Algne
    dotnet run

Migratsioon rakendatakse käivitamisel automaatselt (Database.Migrate()). Ava aadress, mille `dotnet run` kuvab.

## Testid

xUnit ühiktestid asuvad kaustas `FunktsiooniUurija.Tests` (vt selle README).

    dotnet test

Märkus: `OtsitavadVeadTestid` on uurivad testid — need kirjeldavad õiget käitumist piirjuhtudel
ja võivad rakenduse praeguste vigade tõttu kukkuda (see on ette nähtud).
