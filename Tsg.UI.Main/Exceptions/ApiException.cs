using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.Exceptions
{
    public class ApiException : Exception
    {
        public InfoBlock CustomInfoBlock { get; set; }

        public ApiException(InfoBlock infoBlock)
        {
            CustomInfoBlock = infoBlock;
        }
    }
}