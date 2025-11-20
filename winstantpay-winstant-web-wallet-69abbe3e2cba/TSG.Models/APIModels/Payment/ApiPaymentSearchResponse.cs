using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace TSG.Models.APIModels.Payment
{
    public class PayoutSearchResponse : StandartResponse
    {
        public IList<ApiPayoutSearchDataModel> PayoutsList { get; set; } = new List<ApiPayoutSearchDataModel>();
    }

    public class ApiPayoutSearchDataModel
    {
        /// <summary>
        /// Payment GUID
        /// </summary>
        public string PaymentId { get; set; }
        /// <summary>
        /// Payment amount
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// Payment amount currency code
        /// </summary>
        public string AmountCurrencyCode { get; set; }
        /// <summary>
        /// Amount Currency Id
        /// </summary>
        // public int AmountCurrencyId { get; set; }
        /// <summary>
        /// Payment amount text
        /// </summary>
        public string AmountTextBare { get; set; }
        /// <summary>
        /// Payment amount text SWIFT
        /// </summary>
        public string AmountTextBareSWIFT { get; set; }
        /// <summary>
        /// payment amount text with currency code
        /// </summary>
        public string AmountTextWithCurrencyCode { get; set; }
        /// <summary>
        /// Approved By
        /// </summary>
        public string ApprovedBy { get; set; }
        /// <summary>
        /// Approved By Full Name
        /// </summary>
        public string ApprovedByFullName { get; set; }
        /// <summary>
        /// Approved Time
        /// </summary>
        public string ApprovedTime { get; set; }
        /// <summary>
        /// Beneficiary Name
        /// </summary>
        public string BeneficiaryName { get; set; }
        /// <summary>
        /// Created By
        /// </summary>
        public string CreatedBy { get; set; }
        /// <summary>
        /// Created By Full Name
        /// </summary>
        public string CreatedByFullName { get; set; }
        /// <summary>
        /// Created Time
        /// </summary>
        public string CreatedTime { get; set; }
        /// <summary>
        /// Customer Id
        /// </summary>
        public string CustomerId { get; set; }
        /// <summary>
        /// Customer Name
        /// </summary>
        public string CustomerName { get; set; }
        /// <summary>
        /// Deleted By
        /// </summary>
        public string DeletedBy { get; set; }
        /// <summary>
        /// Deleted By Full Name
        /// </summary>
        public string DeletedByFullName { get; set; }
        /// <summary>
        /// Deleted Time
        /// </summary>
        public string DeletedTime { get; set; }
        /// <summary>
        /// FXDeal Id
        /// </summary>
        public string FXDealId { get; set; }
        /// <summary>
        /// FX Deal Reference
        /// </summary>
        public string FXDealReference { get; set; }
        /// <summary>
        /// Fee Amount
        /// </summary>
        public decimal FeeAmount { get; set; }
        /// <summary>
        /// Fee Amount Currency Code
        /// </summary>
        public string FeeAmountCurrencyCode { get; set; }
        /// <summary>
        /// Fee Amount Currency Id
        /// </summary>
        // public int FeeAmountCurrencyId { get; set; }
        /// <summary>
        /// Fee Amount Text Bare
        /// </summary>
        public string FeeAmountTextBare { get; set; }
        /// <summary>
        /// Fee Amount Text With Currency Code
        /// </summary>
        public string FeeAmountTextWithCurrencyCode { get; set; }
        /// <summary>
        /// Is Deleted
        /// </summary>
        public bool IsDeleted { get; set; }
        /// <summary>
        /// Number Of Prior Payments To Same Beneficiary
        /// </summary>
        public int NumberOfPriorPaymentsToSameBeneficiary { get; set; }
        /// <summary>
        /// Payment Reference
        /// </summary>
        public string PaymentReference { get; set; }
        /// <summary>
        /// Payment Status Type Id
        /// </summary>
        public int PaymentStatusTypeId { get; set; }
        /// <summary>
        /// Payment Status Type Name
        /// </summary>
        public string PaymentStatusTypeName { get; set; }
        /// <summary>
        /// Processing Branch Id
        /// </summary>
        public string ProcessingBranchId { get; set; }
        /// <summary>
        /// Processing Branch Name
        /// </summary>
        public string ProcessingBranchName { get; set; }
        /// <summary>
        /// Released By
        /// </summary>
        public string ReleasedBy { get; set; }
        /// <summary>
        /// Released By Full Name
        /// </summary>
        public string ReleasedByFullName { get; set; }
        /// <summary>
        /// Released Time
        /// </summary>
        public string ReleasedTime { get; set; }
        /// <summary>
        /// Submitted By
        /// </summary>
        public string SubmittedBy { get; set; }
        /// <summary>
        /// Submitted By Full Name
        /// </summary>
        public string SubmittedByFullName { get; set; }
        /// <summary>
        /// Submitted Time
        /// </summary>
        public string SubmittedTime { get; set; }
        /// <summary>
        /// Swift UETR
        /// </summary>
        public string SwiftUETR { get; set; }
        /// <summary>
        /// Value Date
        /// </summary>
        public string ValueDate { get; set; }
        /// <summary>
        /// Verified By
        /// </summary>
        public string VerifiedBy { get; set; }
        /// <summary>
        /// Verified By Full Name
        /// </summary>
        public string VerifiedByFullName { get; set; }
        /// <summary>
        /// Verified Time
        /// </summary>
        public string VerifiedTime { get; set; }
        /// <summary>
        /// WKYC Status Type Id
        /// </summary>
        public int WKYCStatusTypeId { get; set; }
        /// <summary>
        /// WKYC Status Type Name
        /// </summary>
        public string WKYCStatusTypeName { get; set; }
    }
}