using ASP.Authentication.Data;

namespace ASP.Authentication.DTOs;

public class ProfileDto
{
    public string Email { get; set; } = null!;
    public string Name { get; set; } = null!;
    public string UserName { get; set; } = null!;
    public Gender Gender { get; set; }
    public string? ImageUrl { get; set; }
}
