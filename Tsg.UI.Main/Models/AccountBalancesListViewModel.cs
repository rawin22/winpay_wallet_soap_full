using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Tsg.UI.Main.Models
{
    public class AccountBalancesListViewModel
    {
        public IList<AccountBalanceViewModel> Balances { get; set; }
        public string FavoriteCurrecy { get; set; }

        public AccountBalancesListViewModel()
        {
            Balances = new List<AccountBalanceViewModel>();
            FavoriteCurrecy = "";
        }
    }
}