
using Microsoft.EntityFrameworkCore;
using UserConfirmation.Services.MessageQueue;

namespace UserConfirmation.Services.Confirmations;
public class ConfirmationService(IMessageQueueService messageQueueService, Data.DbContext dbContext) : IConfirmationService
{
    private readonly IMessageQueueService _messageQueueService = messageQueueService;
    private readonly Data.DbContext _dbContext = dbContext;

    private static string GenerateCode()
    {
        return new Random().Next(100000, 999999).ToString();
    }

    public string SendConfirmationCode(string userId)
    {
        var code = GenerateCode();
        var message = $"{userId}:{code}";

        _messageQueueService.SendMessage(new Shared.Models.ConfirmationRequest(userId, code));

        Console.WriteLine(" [x] Sent {0}", message);
        return code;
    }

    public async Task<bool> ValidateConfirmationCodeAsync(string userId, string code)
    {
        var latestCode = await _dbContext
            .ConfirmationCodes
            .Where(x=>x.UserId == userId && x.Code == code)
            .OrderByDescending(x=>x.CreatedDate)
            .FirstOrDefaultAsync();

        return latestCode != null;
    }
}