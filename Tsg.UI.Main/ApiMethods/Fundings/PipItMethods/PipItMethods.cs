using System.Linq;
using Newtonsoft.Json;
using Tsg.UI.Main.Models.Pipit;
using Tsg.UI.Main.Models.Security;
using TSG.Models.APIModels;

namespace Tsg.UI.Main.ApiMethods.Fundings.PipItMethods
{
    public class PipItMethods : BaseApiMethods
    {
        private UserInfo UserInfo { get; set; }

        public PipItMethods(UserInfo ui) : base(ui)
        {
            UserInfo = ui;
        }

        public PipitContent CreateNewPipitPayment(PipitNewOrder model)
        {
            var cookies = Service.LoginInPipIt();

            var createdOrderString = Service.PostCreateOrders(cookies.FirstOrDefault(), JsonConvert.SerializeObject(model));
            return JsonConvert.DeserializeObject<PipitContent>(createdOrderString);
        }
        
    }
}