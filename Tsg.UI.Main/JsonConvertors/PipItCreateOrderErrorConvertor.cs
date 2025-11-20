using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Tsg.UI.Main.APIModels;

namespace Tsg.UI.Main.JsonConvertors
{
    public class PipItCreateOrderErrorConvertor
    {
        public class PipItCreateOrderConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return objectType == typeof(ApiPipItCreateFundingModel.PipItCreateOrderException);
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {

                //ViewBag.Order = expiredOrders;
                while (reader.Read())
                {
                    if (reader.TokenType == JsonToken.PropertyName)
                    {
                        string value = reader.Value.ToString();

                        switch (reader.Value.ToString())
                        {
                        }
                    }
                }

                //var array = JsonConvert.DeserializeObject<ApiPipItCreateFundingModel.PipItCreateOrderException>(JObject.Load(reader).Root.ToString());
                //try
                //{
                //    if (array.ToString().Contains("INVALID_ORDER"))
                //    {
                //        return
                //            null; //);
                //    }
                //}
                //catch (Exception e)
                //{
                //    Console.WriteLine(e);
                //}
                return null;
            }

            public override bool CanWrite => false;

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}