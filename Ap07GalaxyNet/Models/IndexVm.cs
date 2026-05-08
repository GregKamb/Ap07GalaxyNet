using Ap07GalaxyNet.Data;

namespace Ap07GalaxyNet.Models;

public class IndexVm
{
    public List<GalaxyWord> LastGalaxyWords { get; set; } = new List<GalaxyWord>();
    public List<Post> RecentPosts { get; set; } = new List<Post>();
    public int? SelectedGalaxyWordId { get; set; }
}
