using CommunityToolkit.Mvvm.Messaging.Messages;

namespace NMC.Client.Messages
{
    public class ConfirmMessage : AsyncRequestMessage<bool>
    {
        public string Title { get; }
        public string Message { get; }

        public ConfirmMessage(string title, string message)
        {
            Title = title;
            Message = message;
        }
    }
}
