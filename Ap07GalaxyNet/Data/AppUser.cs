namespace Ap07GalaxyNet.Data;

public class AppUser
{
    public int Id { get; set; }

    public string Username { get; set; } = "";

    public string Email { get; set; } = "";

    public string ImagePath { get; set; } = "";

    public byte[] PasswordHash { get; set; } = Array.Empty<byte>();

    public byte[] Salt { get; set; } = Array.Empty<byte>();

    //Navigation Properties
    public List<Post> Posts { get; set; } = new List<Post>();
  public List<Like> Likes { get; set; } = new List<Like>();

}
