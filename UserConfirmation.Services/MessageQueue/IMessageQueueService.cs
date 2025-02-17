﻿using UserConfirmation.Shared.Models;

namespace UserConfirmation.Services.MessageQueue;
public interface IMessageQueueService
{
    void SendMessage(ConfirmationRequest request);
    //Task<string> RecieveMessage();
    void RecieveMessage();
    void Dispose();
}
