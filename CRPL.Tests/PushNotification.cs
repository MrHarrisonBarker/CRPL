using System;
using System.Net.Http;
using System.Threading.Tasks;
using CorePush.Google;
using CRPL.Web.Core;
using FirebaseAdmin;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace CRPL.Tests;

[TestFixture]
public class PushNotification
{
    [Test]
    public async Task Should_Send_It()
    {
        var fcm = new FcmSender(new FcmSettings
        {
            SenderId = "30934328073",
            ServerKey = "AAAABzPUYwk:APA91bFTiBwd-6LNEsEX33tmjoV5IyPuhDWB1mP7GgOOCj19vvyHstEQoKUQwUB0KV_lLt27PQ7-aBTySoFlVgasgId5S4M6utdcg0I1YTDY0tPVqZlq8cM2wOIeQqTC_m88UinG6920"
        }, new HttpClient());

        var payload = new Notification
        {
            Notifi = new Notification.NotificationPayload
            {
                Title = "Hello world different",
                Body = "This is a body, this is new"
            }
        };

        JObject payload1 = JObject.FromObject(payload);
        payload1.Remove("token");
        payload1.Add("token",
            JToken.FromObject(
                (object)"fQNOEu8pNdZbzs8GRh2dS7:APA91bG57K-zW7KKWdWnHBWRNxltx0BOsQ88zFxjUVp5RWGHGxSlbNWMq3FrG0utzsH9Z5ngU-SSyPG7YSPWLAym3vm4Z6uAa834iMuO_ALtxFrXB-n8lGlCKdLcnp9tmoJ9QJdNGMZd"));

        await fcm.SendAsync(payload1);
    }

    [Test]
    public async Task Should_Work()
    {
        FirebaseApp.Create(new AppOptions
        {
            Credential = GoogleCredential.FromFile("/Users/harrison/Desktop/CRPL/CRPL.Web/crpl-c5132-firebase-adminsdk-pkggb-4bf090805b.json"),
        });

        // This registration token comes from the client FCM SDKs.
        var registrationToken = "fQNOEu8pNdZbzs8GRh2dS7:APA91bGRjWd9ec4DTyHTsRV52jc53_mlMDuOfw5nEVT08wvA5xPUvxfSvnL4IzL41RbVB3oHWHXuuXUvMJmskglIIxKQSNVyiZe5qxBKOafsM3k3FRE4OEchF8TE2yVFCVFnJw9MeI52";

        // See documentation on defining a message payload.
        var message = PushNotificationUtils.NewNotification(registrationToken, "Hello world", "this is notification","https://upload.wikimedia.org/wikipedia/commons/a/ac/BrownhillsCouncilHouse.jpg");

        // Send a message to the device corresponding to the provided
        // registration token.
        string response = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        // Response is a message ID string.
        Console.WriteLine("Successfully sent message: " + response);
    }
}

public class Notification
{
    [JsonProperty("notification")] public NotificationPayload Notifi { get; set; }

    public class NotificationPayload
    {
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("body")] public string Body { get; set; }
    }
}

public class GoogleNotification
{
    [JsonProperty("notification")] public NotificationPayload Notification { get; set; }

    public class NotificationPayload
    {
        [JsonProperty("title")] public string Title { get; set; }
        [JsonProperty("body")] public string Body { get; set; }
    }
}