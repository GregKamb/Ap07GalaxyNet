namespace Ap07GalaxyNet.Data;

public class GalaxyWord
{
    public int Id { get; set; }
    public string Word { get; set; } = "";
    public DateTime LastUsedOn { get; set; }


    //Navigation Properties
    public List<PostGalaxyWord> PostGalaxyWords { get; set; } = new List<PostGalaxyWord>();
}
