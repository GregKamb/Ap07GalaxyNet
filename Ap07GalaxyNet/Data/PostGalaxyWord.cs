namespace Ap07GalaxyNet.Data;

public class PostGalaxyWord
{
    public int Id { get; set; }

    //Navigation Properties
    public Post Post { get; set; } = null!;
    public GalaxyWord GalaxyWord { get; set; } = null!;


    //Foreign Keys
    public int PostId { get; set; }
    public int GalaxyWordId { get; set; }
}
