using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using TSG.Models.APIModels.Fundings;

namespace TSG.Models.APIModels.Fundings.Pipit
{
    public class ApiPipItCreateFundingModel
    {
        public class PipItCreateFundingPageModelResponse : StandartResponse
        {
            public SendedDataByPipItCreateFunding Data { get; set; }
        }

        public class SendedDataByPipItCreateFunding
        {
            public IList<string> ListOfAliases { get; set; }
            public IList<string> CurrencyList { get; set; }
        }

        public class ApiPipItCreateFundingPostBody
        {
            [Required]
            public string UserAlias { get; set; }
            [Required]
            public string Email { get; set; }
            [Required]
            public decimal Amount { get; set; }
            [Required]
            public string CurrencyCode { get; set; }
        }

        public class PipItCreateFundingResponse : StandartResponse
        {
            public ApiPipitContent PipItContent { get; set; }
        }

        public class PipItFieldError
        {
            [JsonProperty("fieldName")]public string FieldName { get; set; }
            [JsonProperty("code")]public string Code { get; set; }
            [JsonProperty("message")]public string FieldMessage { get; set; }
        }

        public class PipItCreateOrderException
        {
            [JsonProperty("timestamp")] public DateTime Timestamp { get; set; }
            [JsonProperty("status")] public int Status { get; set; }
            [JsonProperty("path")] public string Path { get; set; }
            [JsonProperty("code")] public string Code { get; set; }
            [JsonProperty("message")] public string Message { get; set; }
            [JsonProperty("fieldErrors")] public List<PipItFieldError> fieldErrors { get; set; }
        }
    }
}