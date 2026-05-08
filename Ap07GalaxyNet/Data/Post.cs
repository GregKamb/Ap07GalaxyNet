namespace Ap07GalaxyNet.Data;

public class Post
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public DateTime PostedOn { get; set; }

    //Navigation Properties
    public AppUser AppUser { get; set; } = null!;


    public List<Like> Likes { get; set; } = new List<Like>();
    public List<PostGalaxyWord> PostGalaxyWords { get; set; } = new List<PostGalaxyWord>();


    //Foreign Keys
    public int AppUserId { get; set; }
}
