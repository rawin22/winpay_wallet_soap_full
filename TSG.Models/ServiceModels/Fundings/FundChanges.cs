using System;

namespace TSG.Models.ServiceModels
{
    public class FundChanges
    {
        public int FundChanges_FndStatChangeId { get; set; }
        public DateTime FundChanges_ChangedDate { get; set; }
        public string FundChanges_ChangedBy { get; set; }
        public string FundChanges_Notes { get; set; }
        public Guid FundChanges_FundingId { get; set; }
        public int? FundChanges_FundingToStatus { get; set; }
        public int? FundChanges_FundingFromStatus { get; set; }

        // dbo.FundChanges.fndId -> dbo.Funding.fndId (FK_StatusChangeHistory_User)
        public virtual Fundings FundChanges_Funding { get; set; }
    }
}
