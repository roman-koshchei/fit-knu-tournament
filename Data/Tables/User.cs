using Microsoft.AspNetCore.Identity;

namespace Data.Tables;

public class User : IdentityUser
{
    public int Version { get; set; } = 1;

    public User(string email)
    {
        UserName = email;
        Email = email;
    }
}