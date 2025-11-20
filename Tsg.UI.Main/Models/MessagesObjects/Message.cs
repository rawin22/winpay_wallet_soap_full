using System;

namespace Tsg.UI.Main.Models.MessagesObjects
{
    public class MessageObject
    {
        public bool IsSuccess { get; set; }
        public bool IsWarning { get; set; }
        public bool IsError { get; set; }
        public string Message { get; set; }
        public DateTime AddedDate { get; set; }
    }
}
