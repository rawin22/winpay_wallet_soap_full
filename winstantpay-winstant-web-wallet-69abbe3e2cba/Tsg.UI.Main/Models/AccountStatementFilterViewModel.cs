using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;

namespace Tsg.UI.Main.Models
{
    public class AccountStatementFilterViewModel : BaseViewModel
    {        
        private Guid AccountId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public CustomerAccountStatementGetResponse Data { get; set; }

        public AccountStatementFilterViewModel(Guid accountId)
        {            
            this.AccountId = accountId;
        }

        public void PrepareDetails()
        {
            this.Data = Service.GetAccontStatements(AccountId, DateTime.Now, DateTime.Now);
        }
    }
}