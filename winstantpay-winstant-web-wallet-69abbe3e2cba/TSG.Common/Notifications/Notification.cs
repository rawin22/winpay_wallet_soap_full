using System;
using WinstantPay.Common.CommonObject;

namespace WinstantPay.Common.Notifications
{
    public static class Notification
    {
        public static Messages.MessageStructure SendNotification(string deviceType, string ticketThemeId, string tag = "")
        {
            var result = new Messages.MessageStructure();
            try
            {
                var androidNotifications = new FcmNotifications();
                result = androidNotifications.SendNotify(ticketThemeId, tag);
            }
            catch (Exception e)
            {
                result.EventType = Messages.TypeOfEvent.Error.ToString();
                result.Message = $"{e.Message} {e.InnerException?.Message}";
            }

            return result;
        }
    }
}