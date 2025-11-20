using System.Collections.Generic;
using Newtonsoft.Json;

namespace WinstantPay.Common.CommonObject
{
    public class Messages
    {
        public class MessageStructure
        {
            [JsonProperty("succesifull")]
            public bool IsSuccesifull { get; set; }
            [JsonProperty("event_type")]
            public string EventType { get; set; }
            [JsonProperty("message")]
            public string Message { get; set; }
        }

        public class MessagesStructure
        {
            [JsonProperty("succesifull")]
            public bool IsSuccesifull { get; set; }

            [JsonProperty("messages")]
            public List<MessageStructure> Messages { get; set; }

            public MessagesStructure()
            {
                IsSuccesifull = false;
                Messages = new List<MessageStructure>();
            }
        }

        public enum TypeOfEvent
        {
            // Info status is equal as Success type
            Info,
            Error,
            Warning
        }

        public static MessageStructure AddInfo()
        {
            return AddInfo("Ok");
        }
        public static MessageStructure AddInfo(string message)
        {
            return new MessageStructure() { EventType = TypeOfEvent.Info.ToString(), IsSuccesifull = true, Message = message };
        }
        public static MessageStructure AddError(string message)
        {
            return new MessageStructure() { EventType = TypeOfEvent.Error.ToString(), IsSuccesifull = false, Message = message };
        }
        public static MessageStructure AddWarning(string message)
        {
            return AddWarning(message, false);
        }
        public static MessageStructure AddWarning(string message, bool isSuccesifful)
        {
            return new MessageStructure() { EventType = TypeOfEvent.Warning.ToString(), IsSuccesifull = isSuccesifful, Message = message };
        }
        
    }
}
