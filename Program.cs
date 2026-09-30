using FunktsiooniUurija.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AndmebaasiKontekst>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("Vaikimisi") ?? "Data Source=funktsioonid.db"));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
    scope.ServiceProvider.GetRequiredService<AndmebaasiKontekst>().Database.Migrate();

app.UseStaticFiles();
app.UseRouting();
app.MapControllerRoute("default", "{controller=Funktsioonid}/{action=Index}/{id?}");
app.Run();
