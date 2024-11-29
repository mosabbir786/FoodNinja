using Android.App;
using Android.Content;
using AndroidX.Core.App;
using Firebase.Messaging;
using FoodNinja.Model;
using FoodNinja.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodNinja.Platforms.Android
{
    [Service(Exported = true)]
    [IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]

    public class FirebaseService:FirebaseMessagingService
    {
        private static FirebaseManager? _firebaseManager;
        public FirebaseService()
        {
        }

        public static void Init(FirebaseManager? firebaseManager)
        {
            _firebaseManager = firebaseManager;
        }
        public override void OnNewToken(string token)
        {
            base.OnNewToken(token);
            if (Preferences.ContainsKey("DeviceToken"))
            {
                Preferences.Remove("DeviceToken");
            }
            Preferences.Set("DeviceToken", token);
        }

        public override async void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
            var notification = message.GetNotification();


            string messageBody = notification?.Body ?? string.Empty;
            string title = notification?.Title ?? string.Empty;
            if(!string.IsNullOrEmpty(messageBody) && !string.IsNullOrEmpty(title))
            {
                string userId = Preferences.Get("LocalId", string.Empty);
                var notificationModel = new SaveNotificationModel
                {
                    Id = new Random().Next(1, 1000),
                    Title = title,
                    Message = messageBody,
                    ReceivedAt = DateTime.Now.ToString("hh:mm tt dd-MM-yyyy")
                };
                await _firebaseManager.SaveNotificationAsync(userId, notificationModel);
            }
            SendNotification(notification.Body, notification.Title, message.Data);
        }

        private void SendNotification(string? messageBody, string? title, IDictionary<string, string> data)
        {
            if (string.IsNullOrEmpty(messageBody) || string.IsNullOrEmpty(title))
            {
                Console.WriteLine("Notification title or body is missing.");
                return;
            }
            var intent = new Intent(this, typeof(MainActivity));
            intent.AddFlags(ActivityFlags.ClearTop | ActivityFlags.SingleTop);

            foreach (var key in data.Keys)
            {
                string value = data[key];
                intent.PutExtra(key, value);
            }

            var pendingIntent = PendingIntent.GetActivity(this, MainActivity.NotificationID, intent, PendingIntentFlags.OneShot | PendingIntentFlags.Immutable);

            var notificationBuilder = new NotificationCompat.Builder(this, MainActivity.Channel_ID)
                .SetContentTitle(title)
                .SetSmallIcon(Resource.Mipmap.appicon)
                .SetContentText(messageBody)
                .SetChannelId(MainActivity.Channel_ID)
                .SetContentIntent(pendingIntent)
                .SetAutoCancel(true)
                .SetPriority((int)NotificationPriority.Max);

            try
            {
                var notificationManager = NotificationManagerCompat.From(this);
                notificationManager.Notify(MainActivity.NotificationID, notificationBuilder.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending notification: {ex.Message}");
            }
        }
    }
}
