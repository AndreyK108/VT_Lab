using Microsoft.AspNetCore.Identity;

public class ApplicationUser : IdentityUser
{
    public byte[]? AvatarImage { get; set; }
    public string? AvatarContentType { get; set; }
}