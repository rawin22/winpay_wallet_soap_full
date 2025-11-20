using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tsg.UI.Main.Models.MessagesObjects
{
    public interface IMessageable
    {
        IList<MessageObject> Messages { get; }
        bool AddCloseBtn { get;  }
        void AddError(string message);
        void AddWarning(string message);
        void AddSuccess(string message);
        bool ClearMessage(MessageObject mo);
        void ClearAll();
    }
}
