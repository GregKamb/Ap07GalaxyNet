using Ap07GalaxyNet.Data;
using Ap07GalaxyNet.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Security.Claims;

namespace Ap07GalaxyNet.Controllers;

public class HomeController : Controller
{
    private readonly GalaxyContext _ctx;

    public HomeController(GalaxyContext ctx)
    {
        _ctx = ctx;
    }

    public async Task<IActionResult> Index(int? gwId)
    {
        int numPostsToShow = 5;

        if (User.Identity.IsAuthenticated)
        {
            
            numPostsToShow = 10;
        }
        else
        {
           
        }

        /*
            Auf der Startseite sind die neuesten 10 (5*) Beiträge
            ersichtlich – angemeldete Benutzer sehen die 100 (10*) neuesten Beiträge. Für jeden dieser Beiträge
            sollen Titel , Benutzername und -Bild des Verfassers, der Zeitpunkt an dem der Beitrag
            veröffentlicht wurde, und die Anzahl der Empfehlungen angezeigt werden. Durch einen Klick auf
            den Titel kann man dann auch den Inhalt lesen.
            * = Zum Testen
         */

        IQueryable<Post> recentPostsQry = _ctx.Posts
            .Include(p => p.AppUser)
            .Include(p => p.Likes);         

        //Wenn ein Galaxy-Word ausgewählt ist, dann...
        if(gwId != null)
        {
            //Filtern wir nur jene Beiträge, bei denen das ausgewählte
            //Galaxy-Word (gwId) in der Liste an Galaxy-Words vorkommt
            recentPostsQry = recentPostsQry
                .Include(p => p.PostGalaxyWords)
                .Where(p => p.PostGalaxyWords.Any(pgw => pgw.GalaxyWordId == gwId));
        }            
            
        var recentPosts = await recentPostsQry
            .OrderByDescending(p => p.PostedOn)
            .Take(numPostsToShow)
            .ToListAsync();

        var lastGalaxywords = await _ctx.GalaxyWords
            .OrderByDescending(p => p.LastUsedOn)
            .Take(5)
            .ToListAsync();

        var vm = new IndexVm();

        vm.LastGalaxyWords = lastGalaxywords;
        vm.RecentPosts = recentPosts;
        vm.SelectedGalaxyWordId = gwId;

        return View(vm);
    }

    [HttpGet]
    public async Task<IActionResult> Post(int postId)
    {
        var post = await _ctx.Posts
            .Include(p => p.AppUser)
            .Where(p => p.Id == postId)
            .FirstOrDefaultAsync();

        return View(post);
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> ToggleLike(int postId)
    {
        var userId = int.Parse(User.FindFirstValue("userid"));

        /*
         * Wenn der Benutzer den Post schon empfohlen hat, dann soll die Empfehlung entfernt werden
         * Ansonsten (der Benutzer hat den Post NICHT empfohlen) soll die Empfehlung gespeichert werden
         * 
         */

        var userLikedPost = await _ctx.Likes
            .Where(l => l.AppUserId == userId && l.PostId == postId)
            .AnyAsync();

        if (userLikedPost)
        {
            //Like löschen
            await _ctx.Likes
                .Where(l => l.AppUserId == userId && l.PostId == postId)
                .ExecuteDeleteAsync();
        }
        else
        {
            //Like setzen
            var newLike = new Like
            {
                AppUserId = userId,
                PostId = postId
            };
            _ctx.Likes.Add(newLike);
            await _ctx.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> UserProfile(int id)
    {
        var user = await _ctx.AppUsers
            .Include(u => u.Posts)
                .ThenInclude(p => p.Likes)
            .Include(u => u.Likes)
            .FirstOrDefaultAsync(u => u.Id == id);

        return View(user);
    }

    [Authorize]
    [HttpGet]
    public IActionResult NewPost()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    public async Task<IActionResult> NewPost(string title, string content)
    {
        var userId = int.Parse(User.FindFirstValue("userid"));

        var now = DateTime.UtcNow;

        //1. Post speichern

        var newPost = new Post
        {
            Title = title,
            Content = content,
            AppUserId = userId,
            PostedOn = now
        };

        _ctx.Posts.Add(newPost);
        await _ctx.SaveChangesAsync();

        //2. Inhalt d. Posts nach Galaxy-Wörtern absuchen
        var galaxyWords = GetGalaxyWords(content);


        //3. Galaxy Wörter abspeichern
        //Für jedes Galaxy im Post:
        //a) Überprüfen, ob es schon in der Liste der Galaxywörter aufgenommen wurde
        //      -Wenn nicht, abspeichern
        //      -Wenn schon, Id holen
        //b) Die Zuordnung in der Zwischentabelle machen (damit Post und Wort assoziiert werden können)

        foreach (var galaxyWord in galaxyWords)
        {
            var dbWord = await _ctx.GalaxyWords.FirstOrDefaultAsync(gw => gw.Word == galaxyWord);

            if (dbWord is null)
            {
                dbWord = new GalaxyWord
                {
                    Word = galaxyWord,
                    LastUsedOn = now
                };
                _ctx.GalaxyWords.Add(dbWord);
                await _ctx.SaveChangesAsync();
            }
            else //Falls das Wort schon existiert, speichern wir nur, wann es zuletzt verwendet wurde
            {
                dbWord.LastUsedOn = now;
            }

            var postGalaxyWord = new PostGalaxyWord
            {
                GalaxyWordId = dbWord.Id,
                PostId = newPost.Id
            };
            _ctx.PostsGalaxyWords.Add(postGalaxyWord);
            await _ctx.SaveChangesAsync();
        }

        return RedirectToAction(nameof(Index));
    }

    private string[] GetGalaxyWords(string str)
    {
        return str
            .Split(" ") //Aus einem String eine Liste mit Wörtern machen
            .Where(wort => wort.StartsWith("<")) //Nur Wörter die mit < beginnen berücksichtigen
            .SelectMany(wort => wort.Split("<")) //Wörter behandeln, die noch ein § beinhalten
            .Where(wort => wort.Length > 0) //Leere Wörter (die durch vorigen Schritt entstanden sind) entfernen
            .Select(wort => wort.Substring(0, FindIndexOfNonLetter(wort))) //Nicht-Buchstaben entfernen
            .Where(wort => wort.Length >= 5 && wort.Length <= 20) //Länge prüfen
            .ToArray();
    }

    private int FindIndexOfNonLetter(string s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (!char.IsLetter(s[i]))
            {
                return i;
            }
        }
        //Falls kein Nicht-Buchstabe enthalten ist, nehmen wir den gesamten String
        return s.Length;
    }

    [Authorize]
    public IActionResult Privacy()
    {
        //Code hier wird nur ausgeführt, wenn ein Benutzer angemeldet ist

        //Das erlaubt uns zB einfach einen Claim abzurufen
        //var username = User.Claims.First(c => c.Type == "name").Value;
        var username = User.FindFirstValue("name");

        

        return View();
    }

    


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}


