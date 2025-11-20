using System.Linq;
using TSG.Models.APIModels;
using TSG.Models.APIModels.ReferalLinksModel;
using Tsg.UI.Main.Repository;

namespace Tsg.UI.Main.ApiMethods.ReferalLinks
{
    public class ReferalLinksMethods
    {
        public ReferalLinksModel GetReferalLinks()
        {
            ReferalLinkRepository referalLinkRepository = new ReferalLinkRepository();
            var localObj = referalLinkRepository.GetReferalLinksList().FirstOrDefault(f => f.IsDefault);

            if (localObj == null)
                return new ReferalLinksModel { Success = false, InfoBlock = new InfoBlock(){UserMessage = "Null response", DeveloperMessage = "Could not get referal links", Code = ApiErrors.ErrorCodeState.UnspecifiedError } };
            return new ReferalLinksModel { Success = true, InfoBlock = new InfoBlock(){UserMessage = "Ok", DeveloperMessage = "Ok", Code = ApiErrors.ErrorCodeState.Success }, ReferalLink = localObj.LinkText };
        }
    }
}