
namespace UI.Models;

public class LoginModel
{
    public int UserId { get; set; }
    public string UserToken { get; set; }
    public List<string> Roles { get; set; }
}