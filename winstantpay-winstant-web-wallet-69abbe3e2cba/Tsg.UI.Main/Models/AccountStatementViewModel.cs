using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;

namespace Tsg.UI.Main.Models
{
    public class AccountStatementViewModel : BaseViewModel
    {
        [Required]
        public Guid AccountId { get; set; }

        public DatePeriod Period { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string StartDate { get; set; }
        public DateTime SafetyStartDate { get; set; }

        [DataType(DataType.DateTime)]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:dd/MM/yyyy}")]
        public string EndDate { get; set; }
        public DateTime SafetyEndDate { get; set; }
        
        public bool IsFirstTime { get; set; }
        
        public AccountStatementViewModel()
        {
            this.Period = DatePeriod.LastMonth;
            IsFirstTime = true;
            this.SafetyStartDate = DateTime.Now.AddMonths(-1);
            this.StartDate = this.SafetyStartDate.ToShortDateString();
            this.SafetyEndDate = DateTime.Now.AddDays(5);
            this.EndDate = this.SafetyEndDate.ToShortDateString();
        }

        public CustomerAccountStatementGetResponse Data { get; set; }

        public AccountStatementViewModel(Guid accountId)
        {            
            this.AccountId = accountId;
            IsFirstTime = true;
            this.Period = DatePeriod.LastMonth;
            this.SafetyStartDate = DateTime.Now.AddMonths(-1);
            this.StartDate = this.SafetyStartDate.ToShortDateString();
            this.SafetyEndDate = DateTime.Now.AddDays(5);
            this.EndDate = this.SafetyEndDate.ToShortDateString();
        }

        public void PrepareDetails()
        {
            this.Data = Service.GetAccontStatements(AccountId, SafetyStartDate, SafetyEndDate);
        }
    }

    public enum DatePeriod
    {
        Today,
        LastWeek,
        LastMonth,
        Last3Months,
        Last6Months
    }
}