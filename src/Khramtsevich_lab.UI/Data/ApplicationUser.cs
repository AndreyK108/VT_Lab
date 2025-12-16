using Microsoft.AspNetCore.Identity;

namespace Khramtsevich_lab.Data;

public class ApplicationUser : IdentityUser
{
    public byte[]? AvatarImage { get; set; }
    public string? AvatarContentType { get; set; }
}
