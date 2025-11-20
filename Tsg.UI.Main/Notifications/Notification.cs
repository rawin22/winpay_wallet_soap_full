using System;
using System.Threading.Tasks;
using Tsg.UI.Main.Notifications;
using TSG.Models.APIModels;

namespace TSG.UI.Main.Notifications
{
    public static class Notification
    {
        public async static Task<StandartResponse> SendNotification(string userId, Guid rowId, string typeOfElement, string message)
        {
            var result = new StandartResponse("");
            try
            {
                var androidNotifications = new AndroidNotifications();
                result = androidNotifications.SendAndroidNotify(userId, rowId, typeOfElement, message);
            }
            catch (Exception e)
            {
                result.InfoBlock.UserMessage = e.Message;
                result.InfoBlock.DeveloperMessage = e.Message;
                result.Success = false;
            }

            return result;
        }
    }
}