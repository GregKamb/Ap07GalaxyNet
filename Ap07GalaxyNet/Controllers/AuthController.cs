using Ap07GalaxyNet.Data;
using Ap07GalaxyNet.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace Ap07GalaxyNet.Controllers;

public class AuthController : Controller
{
    private readonly GalaxyContext _ctx;
    private readonly IWebHostEnvironment _env;

    public AuthController(GalaxyContext ctx, IWebHostEnvironment env)
    {
        _ctx = ctx;
        _env = env;
    }

    [HttpGet]
    public IActionResult Register()
    {
        var vm = new RegisterVm();
        return View(vm);
    }

    [HttpPost]
    public async Task<IActionResult> Register(
        string email,
        string username,
        string password,
        string passwordConfirm,
        IFormFile userImage
        )
    {
        //Validierung
        var vm = new RegisterVm()
        {
            Username = username,
            Email = email
        };

        vm.Errors = await ValidateInput(email, username, password, passwordConfirm);

        if (vm.Errors.Any())
        {
            return View(vm);
        }

        //Bildupload

        var relativeImagePath = "";

        if (userImage != null && userImage.Length > 0)
        {
            relativeImagePath = $"/images/{userImage.FileName}";

            var fullImagePath = @$"{_env.WebRootPath}{relativeImagePath}";

            using var fileStream = System.IO.File.Create(fullImagePath);

            await userImage.CopyToAsync(fileStream);
        }


        //Salt erzeugen und Hashen
        var saltBytes = new byte[256 / 8];
        RandomNumberGenerator.Fill(saltBytes);

        var hash = SaltAndHashPassword(password, saltBytes);

        //Benutzer abspeichern

        var newAppUser = new AppUser
        {
            Email = email,
            Username = username,
            PasswordHash = hash,
            Salt = saltBytes,
            ImagePath = relativeImagePath
        };

        _ctx.AppUsers.Add(newAppUser);

        await _ctx.SaveChangesAsync();

        return RedirectToAction(nameof(Login));
    }

    private async Task<List<string>> ValidateInput(
        string email,
        string username,
        string password,
        string passwordConfirm)
    {
        var errors = new List<string>();
        //todo: Prüfen, ob Username den Regeln entspricht

        errors.AddRange(ValidateUsername(username));

        //todo: Prüfen, ob Passwörter übereinstimmen

        if (!email.Contains("@"))
        {
            errors.Add("Geben Sie bitte eine gültige E-Mail-Adresse ein");
        }

        //Prüfen ob E-Mail oder Username schon existieren
        var emailExists = await _ctx.AppUsers.AnyAsync(au => au.Email == email);

        if (emailExists)
        {
            errors.Add("E-Mail-Adresse existiert bereits. Bitte eine andere wählen");
        }
        var usernameExists = await _ctx.AppUsers.AnyAsync(au => au.Username == username);

        if (usernameExists)
        {
            errors.Add("Benutzername existiert bereits. Bitte einen anderen wählen");
        }

        if (password.Length < 12)
        {
            errors.Add("Passwort zu unsicher. Bitte mindestens 12 Zeichen");
        }

        if (password != passwordConfirm)
        {
            errors.Add("Passwörter stimmen nicht überein!");
        }

        return errors;
    }

    private List<string> ValidateUsername(string username)
    {
        var errors = new List<string>();
        /*
         *Wichtig: der Benutzername darf aus maximal 16 Zeichen bestehen, darf keine Sonderzeichen
            beinhalten und muss mit einem Buchstaben anfangen.
         */

        string validFirstCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
        string validCharacters = validFirstCharacters + "0123456789";

        if (!validFirstCharacters.Contains(username[0]))
        {
            errors.Add("Benutzername muss mit einem Buchstaben beginnen!");
        }

        foreach (var usernameCharacter in username)
        {
            if (!validCharacters.Contains(usernameCharacter))
            {
                errors.Add("Benutzername darf nur alphanumerische Zeichen beinhalten");
                break;
            }
        }

        if (username.Length > 16)
        {
            errors.Add("Benutzername darf maximal 16 Zeichen beinhalten");
        }

        return errors;
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(string userid, string password)
    {
        //Benutzer suchen aufgrund Email/Username

        var appUser = await _ctx.AppUsers.FirstOrDefaultAsync(au =>
                au.Email == userid
           || au.Username == userid
        );

        if (appUser is null) return View();


        //Wenn gefunden:
        //Login-Passwort mit Salt aus DB kombinieren und Hashen
        var hash = SaltAndHashPassword(password, appUser.Salt);

        //Hash mit Db-Hash vergleichen
        //(Achtung: mit == oder != erreicht man das Ziel NICHT,
        //da byte[] Wertetypen sind)

        var passwordsDoNotMatch = !hash.SequenceEqual(appUser.PasswordHash);

        if (passwordsDoNotMatch) return View();

        //Bei Übereinstimmung: in Anwendung anmelden
        await LogUserIntoAppAsync(appUser);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    private byte[] SaltAndHashPassword(string password, byte[] saltBytes)
    {
        //Passwort in byte[] umwandeln
        var passwordBytes = Encoding.UTF8.GetBytes(password);

        //Passwort und Salt zusammenhängen
        var saltedPasswordBytes = passwordBytes.Concat(saltBytes).ToArray();

        //Passwort und Salt gemeinsam hashen
        return SHA256.HashData(saltedPasswordBytes);
    }

    private async Task LogUserIntoAppAsync(AppUser appUser)
    {
        var usernameClaim = new Claim("name", appUser.Username);
        var userIdClaim = new Claim("userid", appUser.Id.ToString());

        var claimList = new List<Claim>();
        claimList.Add(usernameClaim);
        claimList.Add(userIdClaim);

        //Diese Claims werden verwendet um eine Identity und einen Principal zu erzeugen
        var claimsIdentity = new ClaimsIdentity(claimList, CookieAuthenticationDefaults.AuthenticationScheme);
        var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

        //Ein Principal ist innerhalb der .NET Welt ein Rechteinhaber

        //Dieser wird in der Anwendung angemeldet
        await HttpContext.SignInAsync(claimsPrincipal);
    }
}
