namespace UserConfirmation.Data.Models;
public sealed class ConfirmationCode
{
    public ConfirmationCode(string userId, string code)
    {
        UserId = userId;
        Code = code;
        CreatedDate = DateTime.UtcNow;
    }

    public ConfirmationCode()
    {
        
    }
    public string UserId { get; set; }
    public string Code { get; set; }
    public DateTime CreatedDate { get; set; }
}
