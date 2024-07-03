namespace UserConfirmation.Services.Confirmations;
public interface IConfirmationService
{
    Task<bool> ValidateConfirmationCodeAsync(string userId, string code);
    string SendConfirmationCode(string userId);
}
