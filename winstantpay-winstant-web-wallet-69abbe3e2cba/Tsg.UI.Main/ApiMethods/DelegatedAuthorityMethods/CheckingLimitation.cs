using System;
using System.Collections.Generic;
using System.Linq;
using Tsg.UI.Main.Extensions;
using TSG.Models.APIModels;
using TSG.Models.APIModels.DelegatedAuthority;
using TSG.Models.Enums;
using TSG.Models.ServiceModels.LimitPayment;

namespace Tsg.UI.Main.ApiMethods.DelegatedAuthorityMethods
{
    public static class CheckingLimitation
    {
        public static StandartResponse CheckByType(List<DaPayLimitsLogSo> paymentLogs, JoinedLimitations limitation, DelegatedAuthoriryLimitationType type)
        {
            var currDate = DateTime.Now;
            switch (type)
            {
                case DelegatedAuthoriryLimitationType.SingleTransaction:
                    return limitation.AmountByOrder <= limitation.MaxAmountByLimitation
                        ? new StandartResponse(true, "Ok") 
                        : limitation.AmountByOrder == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Transaction limit exceeded! Please check your payment limit settings.");
                case DelegatedAuthoriryLimitationType.DailyLimit:
                    var daylySum = paymentLogs.Where(w => w.DaPayLimitsLog_LimitsCurrencyCode == limitation.CurrencyCode
                                           && w.DaPayLimitsLog_CreateDate >= currDate.GetDateLast24Hour()
                                           && w.DaPayLimitsLog_CreateDate < currDate)
                        .Select(s => s.DaPayLimitsLog_AmountInLimitsCurrency).Sum() + limitation.AmountByOrder;

                    return limitation.AmountByOrder <= limitation.MaxAmountByLimitation && 
                           daylySum <= limitation.MaxAmountByLimitation
                        ? new StandartResponse(true, "Ok")
                        : limitation.AmountByOrder == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Daily limit exceeded! Please check your payment limit settings.");
                case DelegatedAuthoriryLimitationType.WeekLimit:
                    var weeklySum = paymentLogs.Where(w => w.DaPayLimitsLog_LimitsCurrencyCode == limitation.CurrencyCode
                                                           && w.DaPayLimitsLog_CreateDate >= currDate.GetDateLast7Days()
                                                           && w.DaPayLimitsLog_CreateDate < currDate)
                        .Select(s => s.DaPayLimitsLog_AmountInLimitsCurrency).Sum() + limitation.AmountByOrder;

                    return limitation.AmountByOrder <= limitation.MaxAmountByLimitation &&
                           weeklySum <= limitation.MaxAmountByLimitation
                        ? new StandartResponse(true, "Ok")
                        : limitation.AmountByOrder == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Weekly limit exceeded! Please check your payment limit settings.");
                case DelegatedAuthoriryLimitationType.MonthLimit:
                    var monthlySum = paymentLogs.Where(w => w.DaPayLimitsLog_LimitsCurrencyCode == limitation.CurrencyCode
                                                           && w.DaPayLimitsLog_CreateDate >= currDate.GetDateLast30Days()
                                                           && w.DaPayLimitsLog_CreateDate < currDate)
                        .Select(s => s.DaPayLimitsLog_AmountInLimitsCurrency).Sum() + limitation.AmountByOrder;

                    return limitation.AmountByOrder <= limitation.MaxAmountByLimitation && 
                           monthlySum <= limitation.MaxAmountByLimitation
                        ? new StandartResponse(true, "Ok")
                        : limitation.AmountByOrder == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Monthly limit exceeded! Please check your payment limit settings.");
                case DelegatedAuthoriryLimitationType.YearLimit:
                    var yearSum = paymentLogs.Where(w => w.DaPayLimitsLog_LimitsCurrencyCode == limitation.CurrencyCode
                                                         && w.DaPayLimitsLog_CreateDate >= currDate.GetDateLast365Days()
                                                         && w.DaPayLimitsLog_CreateDate < currDate)
                                      .Select(s => s.DaPayLimitsLog_AmountInLimitsCurrency).Sum() + limitation.AmountByOrder;

                    return limitation.AmountByOrder <= limitation.MaxAmountByLimitation &&
                           yearSum <= limitation.MaxAmountByLimitation
                        ? new StandartResponse(true, "Ok")
                        : limitation.AmountByOrder == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Yearly limit exceeded! Please check your payment limit settings.");
                case DelegatedAuthoriryLimitationType.AllTime:
                    var allTimeSum = paymentLogs.Where(w => w.DaPayLimitsLog_LimitsCurrencyCode == limitation.CurrencyCode)
                                      .Select(s => s.DaPayLimitsLog_AmountInLimitsCurrency).Sum() + limitation.AmountByOrder;

                    return limitation.AmountByOrder <= limitation.MaxAmountByLimitation &&
                           allTimeSum <= limitation.MaxAmountByLimitation
                        ? new StandartResponse(true, "Ok")
                        : limitation.AmountByOrder == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("All time limit exceeded! Please check your payment limit settings.");

            }
            return new StandartResponse("Undefined payment limit");
        }

        public static List<StandartResponse> CheckByTypes(List<DaPayLimitsLogSo> paymentLogs, List<JoinedLimitations> limitations)
        {
            List<StandartResponse> result = new List<StandartResponse>();
            foreach (var limitation in limitations)
            {
                result.Add(CheckByType(paymentLogs, limitation, (DelegatedAuthoriryLimitationType)limitation.LimitType));
            }
            return result;
        }

        public static List<StandartResponse> CheckTypesByAmount(List<DaPayLimitsTabSo> paymentLimitTabs, List<CheckingLimitations> limitations)
        {
            List<StandartResponse> result = new List<StandartResponse>();
            foreach (var limitation in limitations)
            {
                var tab = paymentLimitTabs.FirstOrDefault(f => f.DaPayLimitsTab_TypeOfLimit == limitation.LimitTypeGuid) ;
                if (tab != null) result.Add(CheckTypeByAmount(limitations, tab, (DelegatedAuthoriryLimitationType)limitation.LimitType));
                else { result.Add(new StandartResponse("Undefined payment limit")); }
            }
            return result;
        }

        private static StandartResponse CheckTypeByAmount(List<CheckingLimitations> paymentLimits, DaPayLimitsTabSo tab, DelegatedAuthoriryLimitationType limitationLimitType)
        {
            switch (limitationLimitType)
            {
                case DelegatedAuthoriryLimitationType.SingleTransaction:
                    return tab.DaPayLimitsTab_Amount > 0 && tab.DaPayLimitsTab_Amount <= (paymentLimits.Where(w => w.LimitType > 1).Min(s => (decimal?)s.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount)
                        ? new StandartResponse(true, "Ok")
                        : tab.DaPayLimitsTab_Amount == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.DailyLimit:
                    return tab.DaPayLimitsTab_Amount > 0 && tab.DaPayLimitsTab_Amount <= (paymentLimits.Where(w=>w.LimitType>2).Min(s => (decimal?)s.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount)
                        ? new StandartResponse(true, "Ok")
                        : tab.DaPayLimitsTab_Amount == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.WeekLimit:
                    return tab.DaPayLimitsTab_Amount > 0 && tab.DaPayLimitsTab_Amount <= (paymentLimits.Where(w => w.LimitType > 3).Min(s => (decimal?)s.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount) &&
                           tab.DaPayLimitsTab_Amount >= (paymentLimits.Where(w => w.LimitType ==2).Max(s => (decimal?) s.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount)
                        ? new StandartResponse(true, "Ok")
                        : tab.DaPayLimitsTab_Amount == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.MonthLimit:
                    return tab.DaPayLimitsTab_Amount > 0 && tab.DaPayLimitsTab_Amount <= (paymentLimits.Where(w => w.LimitType > 4).Min(s => (decimal?) s.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount) &&
                           tab.DaPayLimitsTab_Amount >= (paymentLimits.Where(w => w.LimitType > 1 && w.LimitType < 4 ).Max(m => (decimal?) m.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount)
                        ? new StandartResponse(true, "Ok")
                        : tab.DaPayLimitsTab_Amount == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.YearLimit:
                    return tab.DaPayLimitsTab_Amount > 0 && tab.DaPayLimitsTab_Amount >= (paymentLimits.Where(w => w.LimitType > 1 && w.LimitType < 5).Max(m => (decimal?) m.AmountByLimitation)?? tab.DaPayLimitsTab_Amount)
                        ? new StandartResponse(true, "Ok")
                        : tab.DaPayLimitsTab_Amount == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.AllTime:
                    return tab.DaPayLimitsTab_Amount > 0 && tab.DaPayLimitsTab_Amount >= (paymentLimits.Where(w => w.LimitType > 1 && w.LimitType < 6).Max(m => (decimal?)m.AmountByLimitation) ?? tab.DaPayLimitsTab_Amount)
                        ? new StandartResponse(true, "Ok")
                        : tab.DaPayLimitsTab_Amount == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");

            }
            return new StandartResponse("Undefined payment limit");
        }

        public static List<StandartResponse> CheckTypesByAmount(List<CheckingLimitations> limitations)
        {
            List<StandartResponse> result = new List<StandartResponse>();
            if (limitations.Count == 0)
            {
                result.Add(new StandartResponse("Undefined payment limit"));
                return result;
            }

            foreach (var limitation in limitations)
            {
                result.Add(CheckTypeByAmount(limitations, limitation, (DelegatedAuthoriryLimitationType)limitation.LimitType));
            }

            return result;
        }

        private static StandartResponse CheckTypeByAmount(List<CheckingLimitations> paymentLimits, CheckingLimitations tab, DelegatedAuthoriryLimitationType limitationLimitType)
        {
            switch (limitationLimitType)
            {
                case DelegatedAuthoriryLimitationType.SingleTransaction:
                    return tab.AmountByLimitation > 0 && tab.AmountByLimitation <= (paymentLimits.Where(w => w.LimitType > 1).Min(s => (decimal?)s.AmountByLimitation) ?? tab.AmountByLimitation)
                        ? new StandartResponse(true, "Ok")
                        : tab.AmountByLimitation == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.DailyLimit:
                    return tab.AmountByLimitation > 0 && tab.AmountByLimitation <= (paymentLimits.Where(w => w.LimitType > 2).Min(s => (decimal?)s.AmountByLimitation) ?? tab.AmountByLimitation)
                        ? new StandartResponse(true, "Ok")
                        : tab.AmountByLimitation == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.WeekLimit:
                    return tab.AmountByLimitation > 0 && tab.AmountByLimitation <= (paymentLimits.Where(w => w.LimitType > 3).Min(s => (decimal?)s.AmountByLimitation) ?? tab.AmountByLimitation) &&
                           tab.AmountByLimitation >= (paymentLimits.Where(w => w.LimitType == 2).Max(s => (decimal?)s.AmountByLimitation) ?? tab.AmountByLimitation)
                        ? new StandartResponse(true, "Ok")
                        : tab.AmountByLimitation == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.MonthLimit:
                    return tab.AmountByLimitation > 0 && tab.AmountByLimitation <= (paymentLimits.Where(w => w.LimitType > 4).Min(s => (decimal?)s.AmountByLimitation) ?? tab.AmountByLimitation) &&
                           tab.AmountByLimitation >= (paymentLimits.Where(w => w.LimitType > 1 && w.LimitType < 4).Max(m => (decimal?)m.AmountByLimitation) ?? tab.AmountByLimitation)
                        ? new StandartResponse(true, "Ok")
                        : tab.AmountByLimitation == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.YearLimit:
                    return tab.AmountByLimitation > 0 && tab.AmountByLimitation >= (paymentLimits.Where(w => w.LimitType > 1 && w.LimitType < 5).Max(m => (decimal?)m.AmountByLimitation) ?? tab.AmountByLimitation)
                        ? new StandartResponse(true, "Ok")
                        : tab.AmountByLimitation == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
                case DelegatedAuthoriryLimitationType.AllTime:
                    return tab.AmountByLimitation > 0 && tab.AmountByLimitation >= (paymentLimits.Where(w => w.LimitType > 1 && w.LimitType < 6).Max(m => (decimal?)m.AmountByLimitation) ?? tab.AmountByLimitation)
                        ? new StandartResponse(true, "Ok")
                        : tab.AmountByLimitation == 0 ? new StandartResponse("Value can not be zero") : new StandartResponse("Incorrect range of limit.");
            }
            return new StandartResponse("Undefined payment limit");
        }
    }
}