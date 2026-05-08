namespace Ap07GalaxyNet.Data;

public class Like
{
    public int Id { get; set; }

    //Navigation Properties
    public AppUser User { get; set; } = null!;
    public Post Post { get; set; } = null!;


    //Foreign Keys
    public int AppUserId { get; set; }

    public int PostId { get; set; }
}
