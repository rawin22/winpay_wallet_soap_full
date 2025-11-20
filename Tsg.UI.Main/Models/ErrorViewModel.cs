using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class ErrorViewModel
    {
        public int Code { get; set; }
        public ErrorType Type { get; set; }
        public string Message { get; set; }
        public string MessageDetails { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public enum ErrorType
    {
        Information = 0,
        Error = 1,
        Warning = 2,
        Validation = 3
    }
}