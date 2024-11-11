using CommunityToolkit.Mvvm.Messaging.Messages;

namespace FoodNinja
{
    public class PushNotificationReceived:ValueChangedMessage<string>
    {
        public PushNotificationReceived(string message) : base(message) 
        {
            
        }
    }
}