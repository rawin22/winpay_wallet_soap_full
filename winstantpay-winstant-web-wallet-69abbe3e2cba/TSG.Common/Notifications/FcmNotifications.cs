using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;
using WinstantPay.Common.CommonObject;

namespace WinstantPay.Common.Notifications
{
    public class FcmNotifications
    {

        private GcmConfiguration gmcConfig { get; set; }
        private GcmServiceBroker gcmBroker { get; set; }
        public FcmNotifications()
        {
            gmcConfig = new GcmConfiguration(ConfigurationManager.AppSettings["senderFcmKey"], ConfigurationManager.AppSettings["senderFcmToken"], null)
            { GcmUrl = "https://fcm.googleapis.com/fcm/send" };
            // 1) SenderID // 2) API-KEY // 3) AppName
            // Create a new broker
            gcmBroker = new GcmServiceBroker(gmcConfig);
        }

        public Messages.MessageStructure SendNotify(string themeId, string tag = "", string message = "")
        {
            var result = new Messages.MessageStructure();

            if (gmcConfig == null)
                return Messages.AddError("Null Gmc config");
            
            
            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {
                aggregateEx.Handle(ex =>
                {
                    result.EventType = Messages.TypeOfEvent.Error.ToString();
                    result.IsSuccesifull = false;
                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        result.Message = $"GCM Notification Failed: ID={gcmNotification?.MessageId}, Desc={description}";
                    }
                    else if (ex is GcmMulticastResultException)
                    {
                        var multicastException = (GcmMulticastResultException)ex;

                        foreach (var succeededNotification in multicastException.Succeeded)
                        {
                            Debug.WriteLine($"GCM Notification Succeeded: ID={succeededNotification.MessageId}");
                        }

                        foreach (var failedKvp in multicastException.Failed)
                        {
                            var n = failedKvp.Key;
                            var e = failedKvp.Value;

                            result.Message = $"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}";

                        }

                    }
                    else if (ex is DeviceSubscriptionExpiredException)
                    {
                        var expiredException = (DeviceSubscriptionExpiredException)ex;

                        var oldId = expiredException.OldSubscriptionId;
                        var newId = expiredException.NewSubscriptionId;

                        Debug.WriteLine($"Device RegistrationId Expired: {oldId}");

                        if (!string.IsNullOrWhiteSpace(newId))
                        {
                            // If this value isn't null, our subscription changed and we should update our database
                            Debug.WriteLine($"Device RegistrationId Changed To: {newId}");
                        }
                    }
                    else if (ex is RetryAfterException)
                    {
                        var retryException = (RetryAfterException)ex;
                        // If you get rate limited, you should stop sending messages until after the RetryAfterUtc date
                        result.Message = $"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}";
                    }
                    else
                    {
                        result.Message = "GCM Notification Failed for some unknown reason";
                    }
                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) =>
            {
                result.EventType = Messages.TypeOfEvent.Info.ToString();
                result.IsSuccesifull = true;
            };

            // Start the broker
            gcmBroker.Start();

            var gcmNotification2 = new GcmNotification()
            {
                Data = JObject.Parse("{ \"tag\" : \""+tag+"\" }"),
                Notification = JObject.Parse("{\"title\": \""+ ConfigurationManager.AppSettings["OrganizationName"] + "\",\"body\": \"" + message + "\"  }"),
                To = $"/topics/{themeId}",
                Priority = GcmNotificationPriority.High
            };
            gcmBroker.QueueNotification(gcmNotification2);
            
            gcmBroker.Stop();

            return result;
        }
    }
}