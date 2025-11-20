using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TSG.Models.APIModels.InstantPayment
{
    public class ApiAccountBalancesListViewModel
    {
        public IList<ApiAccountBalanceViewModel> Balances { get; set; } = new List<ApiAccountBalanceViewModel>();
        public string FavoriteCurrecy { get; set; }
    }
}