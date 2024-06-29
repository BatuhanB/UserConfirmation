using Microsoft.AspNetCore.Identity;

namespace UserConfirmation.Data.Models;
public class ApplicationUser : IdentityUser
{
    public string ConfirmationCode { get; set; }
}
