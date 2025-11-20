using System.Collections.Generic;
using System.Net;
using Tsg.Business.Model.TsgGPWebService;

namespace TSG.Models.APIModels
{
    /// <summary>
    /// 
    /// </summary>
    public class InfoBlock
    {
        public ApiErrors.ErrorCodeState Code { get; set; }
        public string UserMessage { get; set; }
        public string DeveloperMessage { get; set; }
        public InfoBlock() { }
        public InfoBlock(string commonMessage) { UserMessage = commonMessage; DeveloperMessage = commonMessage; }
        public InfoBlock(string userMessage, string developerMessage) { UserMessage = userMessage; DeveloperMessage = developerMessage; }
    }


    public class ExtendedStandartResponse : StandartResponse
    {
        public bool IsWarning { get; set; }

        public ExtendedStandartResponse() : base () { }
        public ExtendedStandartResponse(string message) : base (message) { }
        public ExtendedStandartResponse(bool success, string commonMessage) : base(success, commonMessage) { }
        public ExtendedStandartResponse(string userMessage, string developerMessage) : base(userMessage, developerMessage) { }
        public ExtendedStandartResponse(bool success, string userMessage, string developerMessage) : base(success, userMessage, developerMessage) { }
    }

    /// <summary>
    /// 
    /// </summary>
    public class StandartResponse
    {
        public bool Success { get; set; }
        //public object Data { get; set; }
        public InfoBlock InfoBlock { get; set; }
        public IList<ApiErrorModel> Errors { get; set; }

        public StandartResponse() {
            Errors = new List<ApiErrorModel>();
        }

        public StandartResponse(bool success, string commonMessage)
        {
            Success = success; InfoBlock = new InfoBlock() { UserMessage = commonMessage, DeveloperMessage = commonMessage };
        }
        public StandartResponse(string commonMessage)
        {
            InfoBlock = new InfoBlock() { UserMessage = commonMessage, DeveloperMessage = commonMessage };
        }
        public StandartResponse(string userMessage, string developerMessage)
        {
            InfoBlock = new InfoBlock() { UserMessage = userMessage, DeveloperMessage = developerMessage };
        }
        public StandartResponse(bool success, string userMessage, string developerMessage)
        {
            Success = success; InfoBlock = new InfoBlock() { UserMessage = userMessage, DeveloperMessage = developerMessage };
        }

        public void GetErrorMessages(ServiceResponseData[] responses)
        {

            foreach (var error in responses)
            {
                this.Errors.Add(new ApiErrorModel
                {
                    Code = error.ResponseCode,
                    Type = (ApiErrorType)error.ResponseType,
                    Message = error.Message,
                    MessageDetails = error.MessageDetails,
                    FieldName = error.FieldName,
                    FieldValue = error.FieldValue
                });
            }
        }
    }

    public class StandartResponse<T> : StandartResponse
    {
        public T Obj { get; set; }

        public StandartResponse() : base() { }

        public StandartResponse(T obj, bool success, string commonMessage) : base(success, commonMessage)
        {
            Obj = obj;
        }

        public StandartResponse(T obj, string commonMessage) : base(commonMessage)
        {
            Obj = obj;
        }

        public StandartResponse(T obj, string userMessage, string developerMessage) : base(userMessage, developerMessage)
        {
            Obj = obj;
        }

        public StandartResponse(T obj, bool success, string userMessage, string developerMessage) : base(success, userMessage, developerMessage)
        {
            Obj = obj;
        }
    }
}