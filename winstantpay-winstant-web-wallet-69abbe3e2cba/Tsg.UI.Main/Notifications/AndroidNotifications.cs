using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Newtonsoft.Json.Linq;
using PushSharp.Core;
using PushSharp.Google;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.Notifications
{
    public class AndroidNotifications
    {
        readonly static log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        private GcmConfiguration gmcConfig { get; set; }
        private GcmServiceBroker gcmBroker { get; set; }
        public AndroidNotifications()
        {
            gmcConfig = new GcmConfiguration(ConfigurationManager.AppSettings["senderFcmKey"], ConfigurationManager.AppSettings["senderFcmToken"],
                    null)
            { GcmUrl = "https://fcm.googleapis.com/fcm/send" };
            // 1) SenderID // 2) API-KEY // 3) AppName
            // Create a new broker
            gcmBroker = new GcmServiceBroker(gmcConfig);
        }

        public StandartResponse SendAndroidNotify(string userId, Guid rowId, string type = "", string message = "")
        {
            var result = new StandartResponse("");

            if (gmcConfig == null)
                return new StandartResponse("Null Gmc config");
            
            
            // Wire up events
            gcmBroker.OnNotificationFailed += (notification, aggregateEx) =>
            {
                aggregateEx.Handle(ex =>
                {
                    result.Success = false;
                    // See what kind of exception it was to further diagnose
                    if (ex is GcmNotificationException)
                    {
                        var notificationException = (GcmNotificationException)ex;

                        // Deal with the failed notification
                        var gcmNotification = notificationException.Notification;
                        var description = notificationException.Description;

                        result.InfoBlock.UserMessage = $"GCM Notification Failed: ID={gcmNotification?.MessageId}, Desc={description}";
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

                            result.InfoBlock.UserMessage = $"GCM Notification Failed: ID={n.MessageId}, Desc={e.Message}";

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
                        result.InfoBlock.UserMessage = $"GCM Rate Limited, don't send more until after {retryException.RetryAfterUtc}";
                    }
                    else
                    {
                        result.InfoBlock.UserMessage = "GCM Notification Failed for some unknown reason";
                    }
                    _logger.Info("Android send notification error");
                    _logger.Info(notification.To);
                    // Mark it as handled
                    return true;
                });
            };

            gcmBroker.OnNotificationSucceeded += (notification) =>
            {
                result.InfoBlock.DeveloperMessage = "Info";
                result.InfoBlock.UserMessage = "Ok";
                result.Success = true;
                _logger.Info("Android send notification success");
                _logger.Info(notification.To);
            };

            // Start the broker
            gcmBroker.Start();
            
            var gcmNotification2 = new GcmNotification()
            {
                Data = JObject.Parse("{\"type\" : \""+type+"\", \"recordId\":\""+ rowId +"\"}"),
                Notification = JObject.Parse("{\"title\":\""+ConfigurationManager.AppSettings["OrganizationName"] +"\",\"body\": \"" + message + "\"  }"),
                To = $"/topics/{userId}",
                //RegistrationIds = new List<string>() { "c74rE7f_whs:APA91bFyknNv0OvRYPMzg8qEp37tP2uTTwHQs06MiPVu6yuf3sMUSSPN7sQzbrzlrEZdbDf9Jh8dde3pnPsDKU3ePEZCskgiZ-clldv0Rgixh9V_PD8y795YMM7DlEHI39RMpv0FRuGL" },
                Priority = GcmNotificationPriority.High,    
            };
            gcmBroker.QueueNotification(gcmNotification2);
            
            gcmBroker.Stop();

            return result;
        }
    }
}