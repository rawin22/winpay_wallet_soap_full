using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSG.Models.APIModels
{
    public class ApiErrorModel
    {
        public int Code { get; set; }
        public ApiErrorType Type { get; set; }
        public string Message { get; set; }
        public string MessageDetails { get; set; }
        public string FieldName { get; set; }
        public string FieldValue { get; set; }
    }

    public enum ApiErrorType
    {
        Information = 0,
        Error = 1,
        Warning = 2,
        Validation = 3
    }
}