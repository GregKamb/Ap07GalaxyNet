using Ap07GalaxyNet.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<GalaxyContext>(opts =>
{
    opts.UseSqlServer("name=DbConnection");
});

//Registriert alle notwendigen Authentifizierungsservices im DI Container
//um Benutzer mit Hilfe von Cookies zu authentifizieren
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(opts =>
    {
        opts.LoginPath = "/Auth/Login";
    });

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<GalaxyContext>();

    // 1. Δημιουργεί τη βάση αν δεν υπάρχει (τρέχει τα migrations)
    context.Database.Migrate();

    // 2. Ελέγχει αν η βάση είναι άδεια. Αν ναι, βάζει test δεδομένα!
    if (!context.AppUsers.Any())
    {
        // Δημιουργούμε έναν Test User (password: Password123!)
        var saltBytes = new byte[32];
        System.Security.Cryptography.RandomNumberGenerator.Fill(saltBytes);
        var passwordBytes = System.Text.Encoding.UTF8.GetBytes("Password123!");
        var saltedPasswordBytes = passwordBytes.Concat(saltBytes).ToArray();
        var hash = System.Security.Cryptography.SHA256.HashData(saltedPasswordBytes);

        var testUser = new Ap07GalaxyNet.Data.AppUser
        {
            Username = "TestExplorer",
            Email = "test@galaxy.net",
            PasswordHash = hash,
            Salt = saltBytes,
            ImagePath = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png"
        };
        context.AppUsers.Add(testUser);
        context.SaveChanges();

        // Δημιουργούμε 2 Test Posts
        var post1 = new Ap07GalaxyNet.Data.Post
        {
            Title = "Hello Universe!",
            Content = "This is my first test chirp. Exploring the unknown <Galaxy!",
            PostedOn = DateTime.UtcNow,
            AppUserId = testUser.Id
        };
        var post2 = new Ap07GalaxyNet.Data.Post
        {
            Title = "Found a new Nebula",
            Content = "The colors out here are amazing! Need to study this <Nebula more.",
            PostedOn = DateTime.UtcNow.AddMinutes(-30),
            AppUserId = testUser.Id
        };
        context.Posts.AddRange(post1, post2);
        context.SaveChanges();
        // Κάτω από το context.SaveChanges() των Posts:
        if (!context.GalaxyWords.Any())
        {
            context.GalaxyWords.AddRange(
                new Ap07GalaxyNet.Data.GalaxyWord { Word = "SpaceTravel", LastUsedOn = DateTime.UtcNow },
                new Ap07GalaxyNet.Data.GalaxyWord { Word = "Coding", LastUsedOn = DateTime.UtcNow },
                new Ap07GalaxyNet.Data.GalaxyWord { Word = "Nebula", LastUsedOn = DateTime.UtcNow },
                new Ap07GalaxyNet.Data.GalaxyWord { Word = "WebDev", LastUsedOn = DateTime.UtcNow }
            );
            context.SaveChanges();
        }
    }
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseRouting();

//Sorgt dafür, dass Requests authentifiziert werden
app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
