using System.Net.Http;
using System.Threading.Tasks;
using CorePush.Google;
using Newtonsoft.Json;
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

        await fcm.SendAsync("fQNOEu8pNdZbzs8GRh2dS7:APA91bFCLSG6-saskycAooHu9NG_zQ2zGxfTa1-2RIRrd1A9yUCmyJOl2pZdl4hqpFyHP--v54TxBiaIBX9PAWSwM8Mk0TrgFz8dR1KHEokRH_RXuwPbWMa4StZ3vctzgBw4YvyjQSZs", new Notification()
            {
                Notifi = new Notification.NotificationPayload()
                {
                    Title = "Hello world",
                    Body = "This is a body"
                }
            });
    }
}

public class Notification
{
    [JsonProperty("collapseKey")] public string Collapse { get; set; }
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