using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsg.UI.Main.Models.MessagesObjects
{
    public abstract class AcMessageable : IMessageable
    {
        public AcMessageable() => Messages = new List<MessageObject>();

        public bool AddCloseBtn { get; set; } = false;

        public void AddError(string message)
        {
            Messages.Add(new MessageObject(){ AddedDate = DateTime.Now, Message = message, IsError = true, IsWarning = false, IsSuccess = false });
        }

        public void AddWarning(string message)
        {
            Messages.Add(new MessageObject() { AddedDate = DateTime.Now, Message = message, IsError = false, IsWarning = true, IsSuccess = false });
        }

        public void AddSuccess(string message)
        {
            Messages.Add(new MessageObject(){ AddedDate = DateTime.Now, Message = message, IsError = false, IsWarning = false, IsSuccess = true });
        }

        public bool ClearMessage(MessageObject mo)
        {
            return Messages.Remove(mo);
        }

        public void ClearAll()
        {
            Messages.Clear();
        }

        public IList<MessageObject> Messages { get; }
    }
}
