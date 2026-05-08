namespace Ap07GalaxyNet.Models;

public class RegisterVm
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";

    public List<string> Errors { get; set; } = new List<string>();
}
