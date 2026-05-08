using Microsoft.EntityFrameworkCore;

namespace Ap07GalaxyNet.Data;

public class GalaxyContext : DbContext
{
    //Leere Konstruktor und DbContextOptions Konstruktor
    //notwendig um mit DI zu arbeiten
    public GalaxyContext()
    {
        
    }

    public GalaxyContext(DbContextOptions<GalaxyContext> opts) : base(opts)
    {
        
    }

    //Methode die aufgerufen wird, wenn EF das Datenbankmodell erzeugt
    //(Also wenn Migrations ausgeführt werden)
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        //Alle Foreign-Keys des Datenmodells abrufen
        var foreignKeys = modelBuilder.Model
            .GetEntityTypes()
            .SelectMany(et => et.GetForeignKeys());

        //Für jeden Foreign-Key das Kaskadieren beim Löschen deaktivieren
        foreach(var fk in foreignKeys)
        {
            fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
    }

    public DbSet<AppUser> AppUsers { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<PostGalaxyWord> PostsGalaxyWords { get; set; }
    public DbSet<GalaxyWord> GalaxyWords { get; set; }
}
