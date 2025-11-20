using System;
using System.Reflection;
using log4net;

namespace TSGWebApi.Logic
{
    public static class CommonMethods
    {
        private static readonly ILog Logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public static void PrintError(string msg)
        {
            Logger.Info(string.Format("Date: [{0}] | Msg: {1} |", DateTime.Now, msg));
        }

        public static void PrintError(Exception exception)
        {
            Logger.Info(string.Format(
                "Date: [{0}] | Msg Error: {1}; \n\r --- Internal msg{2}; \n\r --- Stack trace{3};", DateTime.Now,
                exception.Message,
                exception.InnerException != null ? exception.InnerException.Message : "<No inner exception>",
                exception.StackTrace));
        }

        public static void PrintInfo(string msg, bool isNewMethod = false)
        {
            if (isNewMethod)
                Logger.Info(string.Format("--------------- Date: [{0}] | Calling method -> {1}  ---------------",
                    DateTime.Now, msg));
            else Logger.Info(string.Format("Date: [{0}] | Msg: {1} |", DateTime.Now, msg));
        }
    }
}