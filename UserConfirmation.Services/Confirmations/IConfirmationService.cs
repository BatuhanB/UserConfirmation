namespace UserConfirmation.Services.Confirmations;
public interface IConfirmationService
{
    Task GenerateAndSendConfirmationCodeAsync(string userId);
}
