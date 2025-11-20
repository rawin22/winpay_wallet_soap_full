namespace TSG.Models.APIModels
{
    /// <summary>
    /// Statuses for order
    /// </summary>
    public enum OrderStatuses
    {
        /// <summary>
        /// Initiated
        /// </summary>
        Initiated = 1,
        /// <summary>
        /// Unconfirmed
        /// </summary>
        Unconfirmed = 2,
        /// <summary>
        /// Confirmed
        /// </summary>
        Confirmed = 3,
        /// <summary>
        /// InsufficientBalance
        /// </summary>
        InsufficientBalance = 4,
        /// <summary>
        /// SuccessifulPayment
        /// </summary>
        SuccessifulPayment = 5,
        /// <summary>
        /// FaildPaymentTSG
        /// </summary>
        FaildPaymentTsg = 6,
        /// <summary>
        /// FaildPaymentEWallet
        /// </summary>
        FaildPaymentEWallet = 7
    }
}