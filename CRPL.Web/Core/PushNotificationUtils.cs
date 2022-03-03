using FirebaseAdmin.Messaging;

namespace CRPL.Web.Core;

public static class PushNotificationUtils
{
    public static Message NewNotification(string token, string title, string body, string? imageUrl = null)
    {
        return new Message
        {
            Apns = new ApnsConfig
            {
                Aps = new Aps
                {
                    Alert = new ApsAlert
                    {
                        Title = title,
                        Body = body
                    }
                }
            },
            Notification = new Notification
            {
                Title = title,
                Body = body,
                ImageUrl = imageUrl
            },
            Webpush = new WebpushConfig
            {
                Notification = new WebpushNotification
                {
                    Title = title,
                    Body = body,
                    Image = imageUrl
                }
            },
            Android = new AndroidConfig
            {
                Notification = new AndroidNotification
                {
                    Title = title,
                    Body = body,
                    ImageUrl = imageUrl
                }
            },
            Token = token
        };
    }
}