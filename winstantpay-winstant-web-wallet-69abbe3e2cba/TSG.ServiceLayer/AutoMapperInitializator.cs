using System.Globalization;
using AutoMapper;
using TSG.Models.APIModels.Fundings.Wire_Instructions;
using TSG.Models.DTO;
using TSG.Models.DTO.AutomaticExchanges;
using TSG.Models.DTO.LimitPayment;
using TSG.Models.DTO.Shop;
using TSG.Models.DTO.Transfers;
using TSG.Models.DTO.UsersDataBlock;
using TSG.Models.ServiceModels;
using TSG.Models.ServiceModels.AutomaticExchange;
using TSG.Models.ServiceModels.LimitPayment;
using TSG.Models.ServiceModels.Shop;
using TSG.Models.ServiceModels.Transfers;
using TSG.Models.ServiceModels.UsersDataBlock;
using TSG.Models.Enums;
using System.Collections.Generic;
using TSG.Models.ServiceModels.Transfers.Reports;
using TSG.Models.DTO.Transfers.Report;
using TSG.Models.ServiceModels.Transfers.RedEnvelope;
using TSG.Models.ServiceModels.SuperUserModels;

namespace TSG.ServiceLayer
{
    public class AutoMapperInitializator
    {
        public static void InitMap()
        {
            AutoMapper.Mapper.Initialize(am =>
            {
                #region FundingSource
                am.RecognizeDestinationPrefixes("FundingSources_");
                am.RecognizePrefixes("FundingSources_");

                am.CreateMap<FundingSourcesDto, FundingSources>()
                    .ForMember(f => f.FundingSources_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.FundingSources_SourceName, x => x.MapFrom(src => src.SourceName))
                    .ForMember(f => f.FundingSources_DesignName, x => x.MapFrom(src => src.DesignName))
                    .ForMember(f => f.FundingSources_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<FundingSources, FundingSourcesDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.FundingSources_ID))
                    .ForMember(f => f.SourceName, x => x.MapFrom(src => src.FundingSources_SourceName))
                    .ForMember(f => f.DesignName, x => x.MapFrom(src => src.FundingSources_DesignName))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.FundingSources_IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region AddFundsWire
                am.RecognizeDestinationPrefixes("AddFundsWire_");
                am.RecognizePrefixes("AddFundsWire_");

                am.CreateMap<AddFunds_WireDto, AddFundsWire>()
                    .ForMember(f => f.AddFundsWire_ProofDocId, x => x.MapFrom(src => src.proofDocId))
                    .ForMember(f => f.AddFundsWire_BankCcyId, x => x.MapFrom(src => src.bankCcyId))
                    .ForMember(f => f.AddFundsWire_BankName, x => x.MapFrom(src => src.bankName))
                    .ForMember(f => f.AddFundsWire_CustName, x => x.MapFrom(src => src.custName))
                    .ForMember(f => f.AddFundsWire_FileName, x => x.MapFrom(src => src.fileName))
                    .ForMember(f => f.AddFundsWire_FilePath, x => x.MapFrom(src => src.filePath))
                    .ForMember(f => f.AddFundsWire_LastFourDigits, x => x.MapFrom(src => src.lastFourDigits))
                    .ForMember(f => f.AddFundsWire_Other, x => x.MapFrom(src => src.other))
                    .ForMember(f => f.AddFundsWire_PaymentDate, x => x.MapFrom(src => src.paymentDate))
                    .ForMember(f => f.AddFundsWire_ParentId, x => x.MapFrom(src => src.ParentId))
                    .ForMember(f => f.AddFundsWire_Amount, x => x.MapFrom(src => src.Amount))
                    .ForMember(f => f.AddFundsWire_PaymentDateString, x => x.MapFrom(src => src.paymentDate.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"))))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<AddFundsWire, AddFunds_WireDto>()
                    .ForMember(f => f.proofDocId, x => x.MapFrom(src => src.AddFundsWire_ProofDocId))
                    .ForMember(f => f.bankCcyId, x => x.MapFrom(src => src.AddFundsWire_BankCcyId))
                    .ForMember(f => f.bankName, x => x.MapFrom(src => src.AddFundsWire_BankName))
                    .ForMember(f => f.custName, x => x.MapFrom(src => src.AddFundsWire_CustName))
                    .ForMember(f => f.fileName, x => x.MapFrom(src => src.AddFundsWire_FileName))
                    .ForMember(f => f.filePath, x => x.MapFrom(src => src.AddFundsWire_FilePath))
                    .ForMember(f => f.lastFourDigits, x => x.MapFrom(src => src.AddFundsWire_LastFourDigits))
                    .ForMember(f => f.other, x => x.MapFrom(src => src.AddFundsWire_Other))
                    .ForMember(f => f.paymentDate, x => x.MapFrom(src => src.AddFundsWire_PaymentDate))
                    .ForMember(f => f.ParentId, x => x.MapFrom(src => src.AddFundsWire_ParentId))
                    .ForMember(f => f.Amount, x => x.MapFrom(src => src.AddFundsWire_Amount))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region AddFundWire -> WireInsruction
                am.CreateMap<AddFundsWire, WireDetails>()
                    .ForMember(f => f.WireDetails_ProofDocId, x => x.MapFrom(src => src.AddFundsWire_ProofDocId))
                    .ForMember(f => f.WireDetails_Amount, x => x.MapFrom(src => src.AddFundsWire_Fundings.Fundings_Amount))
                    .ForMember(f => f.WireDetails_BankCcyId, x => x.MapFrom(src => src.AddFundsWire_BankCcyId))
                    .ForMember(f => f.WireDetails_BankName, x => x.MapFrom(src => src.AddFundsWire_BankName))
                    .ForMember(f => f.WireDetails_CustName, x => x.MapFrom(src => src.AddFundsWire_CustName))
                    .ForMember(f => f.WireDetails_FileName, x => x.MapFrom(src => src.AddFundsWire_FileName))
                    .ForMember(f => f.WireDetails_FilePath, x => x.MapFrom(src => src.AddFundsWire_FilePath))
                    .ForMember(f => f.WireDetails_LastFourDigits, x => x.MapFrom(src => src.AddFundsWire_LastFourDigits))
                    .ForMember(f => f.WireDetails_Other, x => x.MapFrom(src => src.AddFundsWire_Other))
                    .ForMember(f => f.WireDetails_ParentId, x => x.MapFrom(src => src.AddFundsWire_ParentId))
                    .ForMember(f => f.WireDetails_PaymentDateString, x => x.MapFrom(src => src.AddFundsWire_PaymentDate.ToString("dd/MM/yyyy", CultureInfo.GetCultureInfo("en-US"))))
                    .ForMember(f => f.Fundings_Currency, x => x.MapFrom(src => src.AddFundsWire_Fundings.Fundings_Currency))
                    .ForMember(f => f.Fundings_FundChange, x => x.MapFrom(src => src.AddFundsWire_Fundings.Fundings_FundChange))
                    .ForMember(f => f.WireDetails_Status, x => x.MapFrom(src => src.AddFundsWire_Fundings.Fundings_StatusByFund))
                    .ForMember(f => f.WireDetails_IsDeleted, x => x.MapFrom(src => src.AddFundsWire_Fundings.Fundings_IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region AddFundsBlockchainInfo
                am.RecognizeDestinationPrefixes("AddFundsBlockChainInfo_");
                am.RecognizePrefixes("AddFundsBlockChainInfo_");

                am.CreateMap<AddFunds_BlockChainInfoDto, AddFunds_BlockChainInfo>()
                    .ForMember(f => f.AddFundsBlockChainInfo_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.AddFundsBlockChainInfo_Alias, x => x.MapFrom(src => src.Alias))
                    .ForMember(f => f.AddFundsBlockChainInfo_BlockChainAddress, x => x.MapFrom(src => src.BlockChainAddress))
                    .ForMember(f => f.AddFundsBlockChainInfo_CallBackAddress, x => x.MapFrom(src => src.CallBackAddress))
                    .ForMember(f => f.AddFundsBlockChainInfo_Index, x => x.MapFrom(src => src.Index))
                    .ForMember(f => f.AddFundsBlockChainInfo_Message, x => x.MapFrom(src => src.Message))
                    .ForMember(f => f.AddFundsBlockChainInfo_TimeStampPayment, x => x.MapFrom(src => src.TimeStampPayment))
                    .ForMember(f => f.AddFundsBlockChainInfo_ParentId, x => x.MapFrom(src => src.ParentId))
                    .ForMember(f => f.AddFundsBlockChainInfo_TotalValue, x => x.MapFrom(src => src.TotalValue))
                    .ForMember(f => f.AddFundsBlockChainInfo_CurrencyIndex, x => x.MapFrom(src => src.CurrencyIndex))
                    .ForMember(f => f.AddFundsBlockChainInfo_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForMember(f => f.AddFundsBlockChainInfo_TransactionHash, x => x.MapFrom(src => src.TransactionHash))
                    .ForMember(f => f.AddFundsBlockChainInfo_DestinatedBitcoinAddress, x => x.MapFrom(src => src.DestinatedBitcoinAddress))
                    .ForMember(f => f.AddFundsBlockChainInfo_NumberOfConfirmation, x => x.MapFrom(src => src.NumberOfConfirmation))
                    .ForMember(f => f.AddFundsBlockChainInfo_ValueInSatoshi, x => x.MapFrom(src => src.ValueInSatoshi))
                    .ForMember(f => f.AddFundsBlockChainInfo_CustomParameter, x => x.MapFrom(src => src.CustomParameter))
                    .ForMember(f => f.AddFundsBlockChainInfo_RequestUrl, x => x.MapFrom(src => src.RequestUrl))
                    .ForMember(f => f.AddFundsBlockChainInfo_TransactionId, x => x.MapFrom(src => src.TransactionId))
                    .ForMember(f => f.AddFundsBlockChainInfo_ConfirmatedTransaction, x => x.MapFrom(src => src.ConfirmatedTransaction))
                    .ForMember(f => f.AddFundsBlockChainInfo_Operation, x => x.MapFrom(src => src.Operation))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<AddFunds_BlockChainInfo, AddFunds_BlockChainInfoDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.AddFundsBlockChainInfo_Id))
                    .ForMember(f => f.Alias, x => x.MapFrom(src => src.AddFundsBlockChainInfo_Alias))
                    .ForMember(f => f.BlockChainAddress, x => x.MapFrom(src => src.AddFundsBlockChainInfo_BlockChainAddress))
                    .ForMember(f => f.CallBackAddress, x => x.MapFrom(src => src.AddFundsBlockChainInfo_CallBackAddress))
                    .ForMember(f => f.Index, x => x.MapFrom(src => src.AddFundsBlockChainInfo_Index))
                    .ForMember(f => f.Message, x => x.MapFrom(src => src.AddFundsBlockChainInfo_Message))
                    .ForMember(f => f.TimeStampPayment, x => x.MapFrom(src => src.AddFundsBlockChainInfo_TimeStampPayment))
                    .ForMember(f => f.ParentId, x => x.MapFrom(src => src.AddFundsBlockChainInfo_ParentId))
                    .ForMember(f => f.TotalValue, x => x.MapFrom(src => src.AddFundsBlockChainInfo_TotalValue))
                    .ForMember(f => f.CurrencyIndex, x => x.MapFrom(src => src.AddFundsBlockChainInfo_CurrencyIndex))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.AddFundsBlockChainInfo_CurrencyCode))
                    .ForMember(f => f.TransactionHash, x => x.MapFrom(src => src.AddFundsBlockChainInfo_TransactionHash))
                    .ForMember(f => f.DestinatedBitcoinAddress, x => x.MapFrom(src => src.AddFundsBlockChainInfo_DestinatedBitcoinAddress))
                    .ForMember(f => f.NumberOfConfirmation, x => x.MapFrom(src => src.AddFundsBlockChainInfo_NumberOfConfirmation))
                    .ForMember(f => f.ValueInSatoshi, x => x.MapFrom(src => src.AddFundsBlockChainInfo_ValueInSatoshi))
                    .ForMember(f => f.CustomParameter, x => x.MapFrom(src => src.AddFundsBlockChainInfo_CustomParameter))
                    .ForMember(f => f.RequestUrl, x => x.MapFrom(src => src.AddFundsBlockChainInfo_RequestUrl))
                    .ForMember(f => f.TransactionId, x => x.MapFrom(src => src.AddFundsBlockChainInfo_TransactionId))
                    .ForMember(f => f.ConfirmatedTransaction, x => x.MapFrom(src => src.AddFundsBlockChainInfo_ConfirmatedTransaction))
                    .ForMember(f => f.Operation, x => x.MapFrom(src => src.AddFundsBlockChainInfo_Operation))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region AddFundsPipit
                am.RecognizeDestinationPrefixes("AddFundsPipit_");
                am.RecognizePrefixes("AddFundsPipit_");

                am.CreateMap<AddFunds_PipitDto, AddFundsPipit>()
                    .ForMember(f => f.AddFundsPipit_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.AddFundsPipit_BarCode, x => x.MapFrom(src => src.BarCode))
                    .ForMember(f => f.AddFundsPipit_CreatedDate, x => x.MapFrom(src => src.CreatedDate))
                    .ForMember(f => f.AddFundsPipit_ExpiredDate, x => x.MapFrom(src => src.ExpiredDate))
                    .ForMember(f => f.AddFundsPipit_OrderValue, x => x.MapFrom(src => src.OrderValue))
                    .ForMember(f => f.AddFundsPipit_TotalValue, x => x.MapFrom(src => src.TotalValue))
                    .ForMember(f => f.AddFundsPipit_ParentId, x => x.MapFrom(src => src.ParentId))
                    .ForMember(f => f.AddFundsPipit_Status, x => x.MapFrom(src => src.Status))
                    .ForMember(f => f.AddFundsPipit_VendorReference, x => x.MapFrom(src => src.VendorReference))
                    .ForMember(f => f.AddFundsPipit_Reference, x => x.MapFrom(src => src.Reference))
                    .ForMember(f => f.AddFundsPipit_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForMember(f => f.AddFundsPipit_PaymentDate, x => x.MapFrom(src => src.PaymentDate))
                    .ForMember(f => f.AddFundsPipit_Alias, x => x.MapFrom(src => src.Alias))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<AddFundsPipit, AddFunds_PipitDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.AddFundsPipit_Id))
                    .ForMember(f => f.BarCode, x => x.MapFrom(src => src.AddFundsPipit_BarCode))
                    .ForMember(f => f.CreatedDate, x => x.MapFrom(src => src.AddFundsPipit_CreatedDate))
                    .ForMember(f => f.ExpiredDate, x => x.MapFrom(src => src.AddFundsPipit_ExpiredDate))
                    .ForMember(f => f.OrderValue, x => x.MapFrom(src => src.AddFundsPipit_OrderValue))
                    .ForMember(f => f.TotalValue, x => x.MapFrom(src => src.AddFundsPipit_TotalValue))
                    .ForMember(f => f.ParentId, x => x.MapFrom(src => src.AddFundsPipit_ParentId))
                    .ForMember(f => f.Status, x => x.MapFrom(src => src.AddFundsPipit_Status))
                    .ForMember(f => f.VendorReference, x => x.MapFrom(src => src.AddFundsPipit_VendorReference))
                    .ForMember(f => f.Reference, x => x.MapFrom(src => src.AddFundsPipit_Reference))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.AddFundsPipit_CurrencyCode))
                    .ForMember(f => f.PaymentDate, x => x.MapFrom(src => src.AddFundsPipit_PaymentDate))
                    .ForMember(f => f.Alias, x => x.MapFrom(src => src.AddFundsPipit_Alias))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region FundChanges
                am.RecognizeDestinationPrefixes("FundChanges_");
                am.RecognizePrefixes("FundChanges_");

                am.CreateMap<FundChangesDto, FundChanges>()
                    .ForMember(f => f.FundChanges_FndStatChangeId, x => x.MapFrom(src => src.fndStatChangeId))
                    .ForMember(f => f.FundChanges_FundingId, x => x.MapFrom(src => src.FundingId))
                    .ForMember(f => f.FundChanges_ChangedBy, x => x.MapFrom(src => src.changedBy))
                    .ForMember(f => f.FundChanges_ChangedDate, x => x.MapFrom(src => src.changedDate))
                    .ForMember(f => f.FundChanges_FundingFromStatus, x => x.MapFrom(src => src.FundingFromStatus))
                    .ForMember(f => f.FundChanges_FundingToStatus, x => x.MapFrom(src => src.FundingToStatus))
                    .ForMember(f => f.FundChanges_Notes, x => x.MapFrom(src => src.notes))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<FundChanges, FundChangesDto>()
                    .ForMember(f => f.fndStatChangeId, x => x.MapFrom(src => src.FundChanges_FndStatChangeId))
                    .ForMember(f => f.FundingId, x => x.MapFrom(src => src.FundChanges_FundingId))
                    .ForMember(f => f.changedBy, x => x.MapFrom(src => src.FundChanges_ChangedBy))
                    .ForMember(f => f.changedDate, x => x.MapFrom(src => src.FundChanges_ChangedDate))
                    .ForMember(f => f.FundingFromStatus, x => x.MapFrom(src => src.FundChanges_FundingFromStatus))
                    .ForMember(f => f.FundingToStatus, x => x.MapFrom(src => src.FundChanges_FundingToStatus))
                    .ForMember(f => f.notes, x => x.MapFrom(src => src.FundChanges_Notes))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region Fundings
                am.RecognizeDestinationPrefixes("Fundings_");
                am.RecognizePrefixes("Fundings_");

                am.CreateMap<FundingsDto, Fundings>()
                    .ForMember(f => f.Fundings_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.Fundings_Amount, x => x.MapFrom(src => src.Amount))
                    .ForMember(f => f.Fundings_CreateDate, x => x.MapFrom(src => src.CreateDate))
                    .ForMember(f => f.Fundings_CurrencyId, x => x.MapFrom(src => src.CurrencyId))
                    .ForMember(f => f.Fundings_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForMember(f => f.Fundings_LastActivityId, x => x.MapFrom(src => src.LastActivityId))
                    .ForMember(f => f.Fundings_Username, x => x.MapFrom(src => src.Username))
                    .ForMember(f => f.Fundings_SourceType, x => x.MapFrom(src => src.SourceType))
                    .ForMember(f => f.Fundings_StatusByFund, x => x.MapFrom(src => src.StatusByFund))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<Fundings, FundingsDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.Fundings_Id))
                    .ForMember(f => f.Amount, x => x.MapFrom(src => src.Fundings_Amount))
                    .ForMember(f => f.CreateDate, x => x.MapFrom(src => src.Fundings_CreateDate))
                    .ForMember(f => f.CurrencyId, x => x.MapFrom(src => src.Fundings_CurrencyId))
                    .ForMember(f => f.LastActivityId, x => x.MapFrom(src => src.Fundings_LastActivityId))
                    .ForMember(f => f.SourceType, x => x.MapFrom(src => src.Fundings_SourceType))
                    .ForMember(f => f.Username, x => x.MapFrom(src => src.Fundings_Username))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.Fundings_IsDeleted))
                    .ForMember(f => f.StatusByFund, x => x.MapFrom(src => src.Fundings_StatusByFund))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region User
                am.RecognizeDestinationPrefixes("User_");
                am.RecognizePrefixes("User_");

                am.CreateMap<UserDto, User>()
                    .ForMember(f => f.User_CurLoginDate, x => x.MapFrom(src => src.curLoginDate))
                    .ForMember(f => f.User_FirstName, x => x.MapFrom(src => src.firstName))
                    .ForMember(f => f.User_IsLocal, x => x.MapFrom(src => src.isLocal))
                    .ForMember(f => f.User_IsViewedChangeLog, x => x.MapFrom(src => src.IsViewedChangeLog))
                    .ForMember(f => f.User_KYCLink, x => x.MapFrom(src => src.KYCLink))
                    .ForMember(f => f.User_LastLoginDate, x => x.MapFrom(src => src.lastLoginDate))
                    .ForMember(f => f.User_LastName, x => x.MapFrom(src => src.lastName))
                    .ForMember(f => f.User_NeedToSearchWelcomeMessage, x => x.MapFrom(src => src.needToSearchWelcomeMessage))
                    .ForMember(f => f.User_Password, x => x.MapFrom(src => src.password))
                    .ForMember(f => f.User_RoleId, x => x.MapFrom(src => src.roleId))
                    .ForMember(f => f.User_UserIdByTSG, x => x.MapFrom(src => src.userIdByTSG))
                    .ForMember(f => f.User_UserMail, x => x.MapFrom(src => src.userMail))
                    .ForMember(f => f.User_UserUiVersion, x => x.MapFrom(src => src.UserUiVersion))
                    .ForMember(f => f.User_Username, x => x.MapFrom(src => src.username))
                    .ForMember(f => f.User_WlcMsgId, x => x.MapFrom(src => src.wlcMsgId))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region Currency
                am.RecognizeDestinationPrefixes("Currency_");
                //am.RecognizePrefixes("Currency_");

                am.CreateMap<CurrencyDto, Currency>()
                    .ForMember(f => f.Currency_CcyCode, x => x.MapFrom(src => src.ccyCode))
                    .ForMember(f => f.Currency_CcyId, x => x.MapFrom(src => src.ccyId))
                    .ForMember(f => f.Currency_CcyName, x => x.MapFrom(src => src.ccyName))
                    .ForMember(f => f.Currency_CcySymbol, x => x.MapFrom(src => src.ccySymbol))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region In Shop

                #region Categories
                am.RecognizeDestinationPrefixes("ShopCategories_");
                am.RecognizePrefixes("ShopCategories_");

                am.CreateMap<ShopCategories, ShopCategoriesDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopCategories_ID))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.ShopCategories_IsDeleted))
                    .ForMember(f => f.IsPublished, x => x.MapFrom(src => src.ShopCategories_IsPublished))
                    .ForMember(f => f.Name, x => x.MapFrom(src => src.ShopCategories_Name))
                    .ForMember(f => f.Parent, x => x.MapFrom(src => src.ShopCategories_Parent))
                    .ForMember(f => f.Order, x => x.MapFrom(src => src.ShopCategories_Order))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopCategoriesDto, ShopCategories>()
                    .ForMember(f => f.ShopCategories_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopCategories_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForMember(f => f.ShopCategories_IsPublished, x => x.MapFrom(src => src.IsPublished))
                    .ForMember(f => f.ShopCategories_Name, x => x.MapFrom(src => src.Name))
                    .ForMember(f => f.ShopCategories_Parent, x => x.MapFrom(src => src.Parent))
                    .ForMember(f => f.ShopCategories_Order, x => x.MapFrom(src => src.Order))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopInfo
                am.RecognizeDestinationPrefixes("ShopInfo_");
                am.RecognizePrefixes("ShopInfo_");

                am.CreateMap<ShopInfo, ShopInfoDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopInfo_ID))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.ShopInfo_IsDeleted))
                    .ForMember(f => f.IsPublished, x => x.MapFrom(src => src.ShopInfo_IsPublished))
                    .ForMember(f => f.Name, x => x.MapFrom(src => src.ShopInfo_Name))
                    .ForMember(f => f.LogoPath, x => x.MapFrom(src => src.ShopInfo_LogoPath))
                    .ForMember(f => f.OwnerWpayId, x => x.MapFrom(src => src.ShopInfo_OwnerWpayId))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopInfoDto, ShopInfo>()
                    .ForMember(f => f.ShopInfo_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopInfo_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForMember(f => f.ShopInfo_IsPublished, x => x.MapFrom(src => src.IsPublished))
                    .ForMember(f => f.ShopInfo_Name, x => x.MapFrom(src => src.Name))
                    .ForMember(f => f.ShopInfo_LogoPath, x => x.MapFrom(src => src.LogoPath))
                    .ForMember(f => f.ShopInfo_OwnerWpayId, x => x.MapFrom(src => src.OwnerWpayId))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopProduct
                am.RecognizeDestinationPrefixes("ShopProducts_");
                am.RecognizePrefixes("ShopProducts_");

                am.CreateMap<ShopProducts, ShopProductsDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopProducts_ID))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.ShopProducts_IsDeleted))
                    .ForMember(f => f.IsPublished, x => x.MapFrom(src => src.ShopProducts_IsPublished))
                    .ForMember(f => f.Name, x => x.MapFrom(src => src.ShopProducts_Name))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.ShopProducts_CurrencyCode))
                    .ForMember(f => f.CategoryID, x => x.MapFrom(src => src.ShopProducts_CategoryID))
                    .ForMember(f => f.FullDescription, x => x.MapFrom(src => src.ShopProducts_FullDescription))
                    .ForMember(f => f.ShortDescription, x => x.MapFrom(src => src.ShopProducts_ShortDescription))
                    .ForMember(f => f.ShopID, x => x.MapFrom(src => src.ShopProducts_ShopID))
                    .ForMember(f => f.Price, x => x.MapFrom(src => src.ShopProducts_Price))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopProductsDto, ShopProducts>()
                    .ForMember(f => f.ShopProducts_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopProducts_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForMember(f => f.ShopProducts_IsPublished, x => x.MapFrom(src => src.IsPublished))
                    .ForMember(f => f.ShopProducts_Name, x => x.MapFrom(src => src.Name))
                    .ForMember(f => f.ShopProducts_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForMember(f => f.ShopProducts_CategoryID, x => x.MapFrom(src => src.CategoryID))
                    .ForMember(f => f.ShopProducts_FullDescription, x => x.MapFrom(src => src.FullDescription))
                    .ForMember(f => f.ShopProducts_ShortDescription, x => x.MapFrom(src => src.ShortDescription))
                    .ForMember(f => f.ShopProducts_ShopID, x => x.MapFrom(src => src.ShopID))
                    .ForMember(f => f.ShopProducts_Price, x => x.MapFrom(src => src.Price))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopProductImages
                am.RecognizeDestinationPrefixes("ShopProductImages_");
                am.RecognizePrefixes("ShopProductImages_");

                am.CreateMap<ShopProductImages, ShopProductImagesDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopProductImages_ID))
                    .ForMember(f => f.ProductID, x => x.MapFrom(src => src.ShopProductImages_ProductID))
                    .ForMember(f => f.Path, x => x.MapFrom(src => src.ShopProductImages_Path))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopProductImagesDto, ShopProductImages>()
                    .ForMember(f => f.ShopProductImages_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopProductImages_ProductID, x => x.MapFrom(src => src.ProductID))
                    .ForMember(f => f.ShopProductImages_Path, x => x.MapFrom(src => src.Path))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopOrders
                am.RecognizeDestinationPrefixes("ShopOrders_");
                am.RecognizePrefixes("ShopOrders_");

                am.CreateMap<ShopOrders, ShopOrdersDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopOrders_ID))
                    .ForMember(f => f.BuyerWpayId, x => x.MapFrom(src => src.ShopOrders_BuyerWpayId))
                    .ForMember(f => f.LastOrderHistoryRec, x => x.MapFrom(src => src.ShopOrders_LastLogItem))
                    .ForMember(f => f.OrderCounter, x => x.MapFrom(src => src.ShopOrders_OrderCounter))
                    .ForMember(f => f.CreateDate, x => x.MapFrom(src => src.ShopOrders_CreateDate))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopOrdersDto, ShopOrders>()
                    .ForMember(f => f.ShopOrders_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopOrders_BuyerWpayId, x => x.MapFrom(src => src.BuyerWpayId))
                    .ForMember(f => f.ShopOrders_LastLogItem, x => x.MapFrom(src => src.LastOrderHistoryRec))
                    .ForMember(f => f.ShopOrders_OrderCounter, x => x.MapFrom(src => src.OrderCounter))
                    .ForMember(f => f.ShopOrders_CreateDate, x => x.MapFrom(src => src.CreateDate))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopOrderItems
                am.RecognizeDestinationPrefixes("ShopOrderItems_");
                am.RecognizePrefixes("ShopOrderItems_");

                am.CreateMap<ShopOrderItems, ShopOrderItemsDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopOrderItems_ID))
                    .ForMember(f => f.Position, x => x.MapFrom(src => src.ShopOrderItems_Position))
                    .ForMember(f => f.Price, x => x.MapFrom(src => src.ShopOrderItems_Price))
                    .ForMember(f => f.Quantity, x => x.MapFrom(src => src.ShopOrderItems_Quantity))
                    .ForMember(f => f.Timestamp, x => x.MapFrom(src => src.ShopOrderItems_Timestamp))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.ShopOrderItems_CurrencyCode))
                    .ForMember(f => f.OrderId, x => x.MapFrom(src => src.ShopOrderItems_OrderId))
                    .ForMember(f => f.ProductId, x => x.MapFrom(src => src.ShopOrderItems_ProductId))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopOrderItemsDto, ShopOrderItems>()
                    .ForMember(f => f.ShopOrderItems_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopOrderItems_Position, x => x.MapFrom(src => src.Position))
                    .ForMember(f => f.ShopOrderItems_Price, x => x.MapFrom(src => src.Price))
                    .ForMember(f => f.ShopOrderItems_Quantity, x => x.MapFrom(src => src.Quantity))
                    .ForMember(f => f.ShopOrderItems_Timestamp, x => x.MapFrom(src => src.Timestamp))
                    .ForMember(f => f.ShopOrderItems_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForMember(f => f.ShopOrderItems_OrderId, x => x.MapFrom(src => src.OrderId))
                    .ForMember(f => f.ShopOrderItems_ProductId, x => x.MapFrom(src => src.ProductId))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopOrderLog
                am.RecognizeDestinationPrefixes("ShopOrderLog_");
                am.RecognizePrefixes("ShopOrderLog_");

                am.CreateMap<ShopOrderLog, ShopOrderLogDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopOrderLog_ID))
                    .ForMember(f => f.OrderID, x => x.MapFrom(src => src.ShopOrderLog_OrderID))
                    .ForMember(f => f.Timestamp, x => x.MapFrom(src => src.ShopOrderLog_Timestamp))
                    .ForMember(f => f.Status, x => x.MapFrom(src => src.ShopOrderLog_Status))
                    .ForMember(f => f.Reason, x => x.MapFrom(src => src.ShopOrderLog_Reason))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopOrderLogDto, ShopOrderLog>()
                    .ForMember(f => f.ShopOrderLog_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopOrderLog_OrderID, x => x.MapFrom(src => src.OrderID))
                    .ForMember(f => f.ShopOrderLog_Timestamp, x => x.MapFrom(src => src.Timestamp))
                    .ForMember(f => f.ShopOrderLog_Status, x => x.MapFrom(src => src.Status))
                    .ForMember(f => f.ShopOrderLog_Reason, x => x.MapFrom(src => src.Reason))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region ShopPayments
                am.RecognizeDestinationPrefixes("ShopPayment_");
                am.RecognizePrefixes("ShopPayment_");

                am.CreateMap<ShopPayment, ShopPaymentDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ShopPayment_ID))
                    .ForMember(f => f.OrderId, x => x.MapFrom(src => src.ShopPayment_OrderId))
                    .ForMember(f => f.OrderItemId, x => x.MapFrom(src => src.ShopPayment_OrderItemId))
                    .ForMember(f => f.PaymentAttemptDate, x => x.MapFrom(src => src.ShopPayment_PaymentAttemptDate))
                    .ForMember(f => f.PaymentId, x => x.MapFrom(src => src.ShopPayment_PaymentId))
                    .ForMember(f => f.Reason, x => x.MapFrom(src => src.ShopPayment_Reason))
                    .ForMember(f => f.Status, x => x.MapFrom(src => src.ShopPayment_Status))
                    .ForMember(f => f.PaymentNumber, x => x.MapFrom(src => src.ShopPayment_PaymentNumber))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<ShopPaymentDto, ShopPayment>()
                    .ForMember(f => f.ShopPayment_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.ShopPayment_OrderId, x => x.MapFrom(src => src.OrderId))
                    .ForMember(f => f.ShopPayment_OrderItemId, x => x.MapFrom(src => src.OrderItemId))
                    .ForMember(f => f.ShopPayment_PaymentAttemptDate, x => x.MapFrom(src => src.PaymentAttemptDate))
                    .ForMember(f => f.ShopPayment_PaymentId, x => x.MapFrom(src => src.PaymentId))
                    .ForMember(f => f.ShopPayment_Reason, x => x.MapFrom(src => src.Reason))
                    .ForMember(f => f.ShopPayment_Status, x => x.MapFrom(src => src.Status))
                    .ForMember(f => f.ShopPayment_PaymentNumber, x => x.MapFrom(src => src.PaymentNumber))
                    .ForAllOtherMembers(f => f.Ignore());

                #endregion

                #endregion

                #region WelcomeMessage
                am.RecognizeDestinationPrefixes("WelcomeMessage_");
                am.RecognizePrefixes("WelcomeMessage_");

                am.CreateMap<WelcomeMessage, WelcomeMessageDto>()
                    .ForMember(f => f.wlcMsgId, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgId))
                    .ForMember(f => f.wlcMsgText, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgText))
                    .ForMember(f => f.isDefault, x => x.MapFrom(src => src.WelcomeMessage_IsDefault))
                    .ForMember(f => f.wlcMsgTextRu, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextRu))
                    .ForMember(f => f.wlcMsgTextFr, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextFr))
                    .ForMember(f => f.wlcMsgTextPh, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextPh))
                    .ForMember(f => f.wlcMsgTextTh, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextTh))
                    .ForMember(f => f.wlcMsgTextAe, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextAe))
                    .ForMember(f => f.wlcMsgTextKh, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextKh))
                    .ForMember(f => f.wlcMsgTextCn, x => x.MapFrom(src => src.WelcomeMessage_WlcMsgTextCn))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<WelcomeMessageDto, WelcomeMessage>()
                    .ForMember(f => f.WelcomeMessage_WlcMsgId, x => x.MapFrom(src => src.wlcMsgId))
                    .ForMember(f => f.WelcomeMessage_WlcMsgText, x => x.MapFrom(src => src.wlcMsgText))
                    .ForMember(f => f.WelcomeMessage_IsDefault, x => x.MapFrom(src => src.isDefault))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextRu, x => x.MapFrom(src => src.wlcMsgTextRu))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextFr, x => x.MapFrom(src => src.wlcMsgTextFr))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextPh, x => x.MapFrom(src => src.wlcMsgTextPh))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextTh, x => x.MapFrom(src => src.wlcMsgTextTh))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextAe, x => x.MapFrom(src => src.wlcMsgTextAe))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextKh, x => x.MapFrom(src => src.wlcMsgTextKh))
                    .ForMember(f => f.WelcomeMessage_WlcMsgTextCn, x => x.MapFrom(src => src.wlcMsgTextCn))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region Role
                am.RecognizeDestinationPrefixes("Role_");
                am.RecognizePrefixes("Role_");

                am.CreateMap<Role, RoleDto>()
                    .ForMember(f => f.roleId, x => x.MapFrom(src => src.Role_RoleId))
                    .ForMember(f => f.roleName, x => x.MapFrom(src => src.Role_RoleName))
                    .ForMember(f => f.roleDesc, x => x.MapFrom(src => src.Role_RoleDesc))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<RoleDto, Role>()
                    .ForMember(f => f.Role_RoleId, x => x.MapFrom(src => src.roleId))
                    .ForMember(f => f.Role_RoleName, x => x.MapFrom(src => src.roleName))
                    .ForMember(f => f.Role_RoleDesc, x => x.MapFrom(src => src.roleDesc))
                    .ForAllOtherMembers(f => f.Ignore());

                
                #endregion

                #region LimitationPayment

                #region DaPayLimitsType

                am.RecognizeDestinationPrefixes("DaPayLimitsType_");
                am.RecognizePrefixes("DaPayLimitsType_");

                am.CreateMap<DaPayLimitsTypeDto, DaPayLimitsTypeSo>()
                    .ForMember(f => f.DaPayLimitsType_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.DaPayLimitsType_LimitType, x => x.MapFrom(src => src.LimitType))
                    .ForMember(f => f.DaPayLimitsType_NameOfPaymentLimit, x => x.MapFrom(src => src.NameOfPaymentLimit))
                    .ForMember(f => f.DaPayLimitsType_SysNameOfPaymentLimit, x => x.MapFrom(src => src.SysNameOfPaymentLimit))
                    .ForMember(f => f.DaPayLimitsType_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DaPayLimitsTypeSo, DaPayLimitsTypeDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.DaPayLimitsType_ID))
                    .ForMember(f => f.LimitType, x => x.MapFrom(src => src.DaPayLimitsType_LimitType))
                    .ForMember(f => f.NameOfPaymentLimit, x => x.MapFrom(src => src.DaPayLimitsType_NameOfPaymentLimit))
                    .ForMember(f => f.SysNameOfPaymentLimit, x => x.MapFrom(src => src.DaPayLimitsType_SysNameOfPaymentLimit))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.DaPayLimitsType_IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());

                #endregion

                #region DaPaymentLimitSourceType

                am.RecognizeDestinationPrefixes("DaPaymentLimitSourceType_");
                am.RecognizePrefixes("DaPaymentLimitSourceType_");

                am.CreateMap<DaPaymentLimitSourceTypeDto, DaPaymentLimitSourceTypeSo>()
                    .ForMember(f => f.DaPaymentLimitSourceType_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.DaPaymentLimitSourceType_SysName, x => x.MapFrom(src => src.SysName))
                    .ForMember(f => f.DaPaymentLimitSourceType_EnumNumber, x => x.MapFrom(src => src.EnumNumber))
                    .ForMember(f => f.DaPaymentLimitSourceType_IsAcceptableOnWeb, x => x.MapFrom(src => src.IsAcceptableOnWeb))
                    .ForMember(f => f.DaPaymentLimitSourceType_IsAcceptableOnMobDevice, x => x.MapFrom(src => src.IsAcceptableOnMobDevice))
                    .ForMember(f => f.DaPaymentLimitSourceType_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DaPaymentLimitSourceTypeSo, DaPaymentLimitSourceTypeDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.DaPaymentLimitSourceType_ID))
                    .ForMember(f => f.SysName, x => x.MapFrom(src => src.DaPaymentLimitSourceType_SysName))
                    .ForMember(f => f.EnumNumber, x => x.MapFrom(src => src.DaPaymentLimitSourceType_EnumNumber))
                    .ForMember(f => f.IsAcceptableOnWeb, x => x.MapFrom(src => src.DaPaymentLimitSourceType_IsAcceptableOnWeb))
                    .ForMember(f => f.IsAcceptableOnMobDevice, x => x.MapFrom(src => src.DaPaymentLimitSourceType_IsAcceptableOnMobDevice))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.DaPaymentLimitSourceType_IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region DaPayLimits

                am.RecognizeDestinationPrefixes("DaPayLimits_");
                am.RecognizePrefixes("DaPayLimits_");

                am.CreateMap<DaPayLimitsDto, DaPayLimitsSo>()
                    .ForMember(f => f.DaPayLimits_ID, x => x.MapFrom(src => src.ID))
                    //.ForMember(f => f.DaPayLimits_CcyCode, x => x.MapFrom(src => src.CcyCode))
                    .ForMember(f => f.DaPayLimits_CreationDate, x => x.MapFrom(src => src.CreationDate))
                    .ForMember(f => f.DaPayLimits_DateOfExpire, x => x.MapFrom(src => src.DateOfExpire))
                    .ForMember(f => f.DaPayLimits_LastModifiedDate, x => x.MapFrom(src => src.LastModifiedDate))
                    .ForMember(f => f.DaPayLimits_SourceType, x => x.MapFrom(src => src.SourceType))
                    .ForMember(f => f.DaPayLimits_LimitCodeInitialization, x => x.MapFrom(src => src.LimitCodeInitialization))
                    //.ForMember(f => f.DaPayLimits_PinCode, x => x.MapFrom(src => src.PinCode))
                    .ForMember(f => f.DaPayLimits_IsPinProtected, x => x.MapFrom(src => src.IsPinProtected))
                    .ForMember(f => f.DaPayLimits_StatusByLimit, x => x.MapFrom(src => src.StatusByLimit))
                    .ForMember(f => f.DaPayLimits_UserData, x => x.MapFrom(src => src.UserData))
                    .ForMember(f => f.DaPayLimits_UserName, x => x.MapFrom(src => src.UserName))
                    .ForMember(f => f.DaPayLimits_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForMember(f => f.DaPayLimits_IsTransfered, x => x.MapFrom(src => src.IsTransfered))
                    .ForMember(f => f.DaPayLimits_SecretCode, x => x.MapFrom(src => src.SecretCode))
                    .ForMember(f => f.DaPayLimits_MediaName, x => x.MapFrom(src => src.MediaName))
                    .ForMember(f => f.DaPayLimits_WPayId, x => x.MapFrom(src => src.WPayId))
                    //.ForMember(f => f.DaPayLimits_RequiredPinAmount, x => x.MapFrom(src => src.RequiredPinAmount))
                    //.ForMember(f => f.DaPayLimits_IsRestrictedToSelectedCurrency, x => x.MapFrom(src => src.IsRestrictedToSelectedCurrency))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DaPayLimitsSo, DaPayLimitsDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.DaPayLimits_ID))
                    //.ForMember(f => f.CcyCode, x => x.MapFrom(src => src.DaPayLimits_CcyCode))
                    .ForMember(f => f.CreationDate, x => x.MapFrom(src => src.DaPayLimits_CreationDate))
                    .ForMember(f => f.DateOfExpire, x => x.MapFrom(src => src.DaPayLimits_DateOfExpire))
                    .ForMember(f => f.LastModifiedDate, x => x.MapFrom(src => src.DaPayLimits_LastModifiedDate))
                    .ForMember(f => f.SourceType, x => x.MapFrom(src => src.DaPayLimits_SourceType))
                    .ForMember(f => f.LimitCodeInitialization, x => x.MapFrom(src => src.DaPayLimits_LimitCodeInitialization))
                    //.ForMember(f => f.PinCode, x => x.MapFrom(src => src.DaPayLimits_PinCode))
                    .ForMember(f => f.IsPinProtected, x => x.MapFrom(src => src.DaPayLimits_IsPinProtected))
                    .ForMember(f => f.StatusByLimit, x => x.MapFrom(src => src.DaPayLimits_StatusByLimit))
                    .ForMember(f => f.UserData, x => x.MapFrom(src => src.DaPayLimits_UserData))
                    .ForMember(f => f.UserName, x => x.MapFrom(src => src.DaPayLimits_UserName))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.DaPayLimits_IsDeleted))
                    .ForMember(f => f.IsTransfered, x => x.MapFrom(src => src.DaPayLimits_IsTransfered))
                    .ForMember(f => f.SecretCode, x => x.MapFrom(src => src.DaPayLimits_SecretCode))
                    .ForMember(f => f.MediaName, x => x.MapFrom(src => src.DaPayLimits_MediaName))
                    .ForMember(f => f.WPayId, x => x.MapFrom(src => src.DaPayLimits_WPayId))
                    //.ForMember(f => f.RequiredPinAmount, x => x.MapFrom(src => src.DaPayLimits_RequiredPinAmount))
                    //.ForMember(f => f.IsRestrictedToSelectedCurrency, x => x.MapFrom(src => src.DaPayLimits_IsRestrictedToSelectedCurrency))

                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region DaPayLimitsLog

                am.RecognizeDestinationPrefixes("DaPayLimitsLog_");
                am.RecognizePrefixes("DaPayLimitsLog_");

                am.CreateMap<DaPayLimitsLogDto, DaPayLimitsLogSo>()
                    .ForMember(f => f.DaPayLimitsLog_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.DaPayLimitsLog_DaPayParentId, x => x.MapFrom(src => src.DaPayParentId))
                    .ForMember(f => f.DaPayLimitsLog_Info, x => x.MapFrom(src => src.Info))
                    .ForMember(f => f.DaPayLimitsLog_Amount, x => x.MapFrom(src => src.Amount))
                    .ForMember(f => f.DaPayLimitsLog_CreateDate, x => x.MapFrom(src => src.CreateDate))
                    .ForMember(f => f.DaPayLimitsLog_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForMember(f => f.DaPayLimitsLog_UserName, x => x.MapFrom(src => src.UserName))
                    .ForMember(f => f.DaPayLimitsLog_AmountInLimitsCurrency, x => x.MapFrom(src => src.AmountInLimitsCurrency))
                    .ForMember(f => f.DaPayLimitsLog_LimitsCurrencyCode, x => x.MapFrom(src => src.LimitsCurrencyCode))


                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DaPayLimitsLogSo, DaPayLimitsLogDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.DaPayLimitsLog_ID))
                    .ForMember(f => f.DaPayParentId, x => x.MapFrom(src => src.DaPayLimitsLog_DaPayParentId))
                    .ForMember(f => f.Info, x => x.MapFrom(src => src.DaPayLimitsLog_Info))
                    .ForMember(f => f.Amount, x => x.MapFrom(src => src.DaPayLimitsLog_Amount))
                    .ForMember(f => f.CreateDate, x => x.MapFrom(src => src.DaPayLimitsLog_CreateDate))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.DaPayLimitsLog_CurrencyCode))
                    .ForMember(f => f.UserName, x => x.MapFrom(src => src.DaPayLimitsLog_UserName))
                    .ForMember(f => f.AmountInLimitsCurrency, x => x.MapFrom(src => src.DaPayLimitsLog_AmountInLimitsCurrency))
                    .ForMember(f => f.LimitsCurrencyCode, x => x.MapFrom(src => src.DaPayLimitsLog_LimitsCurrencyCode))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region DaPayLimitsTab

                am.RecognizeDestinationPrefixes("DaPayLimitsTab_");
                am.RecognizePrefixes("DaPayLimitsTab_");

                am.CreateMap<DaPayLimitsTabDto, DaPayLimitsTabSo>()
                    .ForMember(f => f.DaPayLimitsTab_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.DaPayLimitsTab_TypeOfLimit, x => x.MapFrom(src => src.TypeOfLimit))
                    .ForMember(f => f.DaPayLimitsTab_ParentDaPayId, x => x.MapFrom(src => src.ParentDaPayId))
                    .ForMember(f => f.DaPayLimitsTab_Amount, x => x.MapFrom(src => src.Amount))
                    .ForMember(f => f.DaPayLimitsTab_IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForMember(f => f.DaPayLimitsTab_ExpireDate, x => x.MapFrom(src => src.ExpireDate))
                    .ForMember(f => f.DaPayLimitsTab_UserId, x => x.MapFrom(src => src.UserId))
                    .ForMember(f => f.DaPayLimitsTab_WPayId, x => x.MapFrom(src => src.WPayId))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DaPayLimitsTabSo, DaPayLimitsTabDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.DaPayLimitsTab_ID))
                    .ForMember(f => f.TypeOfLimit, x => x.MapFrom(src => src.DaPayLimitsTab_TypeOfLimit))
                    .ForMember(f => f.ParentDaPayId, x => x.MapFrom(src => src.DaPayLimitsTab_ParentDaPayId))
                    .ForMember(f => f.Amount, x => x.MapFrom(src => src.DaPayLimitsTab_Amount))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.DaPayLimitsTab_IsDeleted))
                    .ForMember(f => f.ExpireDate, x => x.MapFrom(src => src.DaPayLimitsTab_ExpireDate))
                    .ForMember(f => f.UserId, x => x.MapFrom(src => src.DaPayLimitsTab_UserId))
                    .ForMember(f => f.WPayId, x => x.MapFrom(src => src.DaPayLimitsTab_WPayId))
                    .ForAllOtherMembers(f => f.Ignore());

                #endregion

                #region DaUserWPayIDSetting

                //am.RecognizeDestinationPrefixes("DaPayLimits_");
                //am.RecognizePrefixes("DaPayLimits_");

                am.CreateMap<DaUserWPayIDSettingDto, DaUserWPayIDSettingSo>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.UserID, x => x.MapFrom(src => src.UserId))
                    .ForMember(f => f.WPayId, x => x.MapFrom(src => src.WPayId))
                    .ForMember(f => f.CcyCode, x => x.MapFrom(src => src.CcyCode))
                    .ForMember(f => f.IsPinRequired, x => x.MapFrom(src => src.IsPinRequired))
                    .ForMember(f => f.PinCode, x => x.MapFrom(src => src.PinCode))
                    .ForMember(f => f.ExceedingAmount, x => x.MapFrom(src => src.ExceedingAmount))
                    .ForMember(f => f.IsRestrictedToSelectedCurrency, x => x.MapFrom(src => src.IsRestrictedToSelectedCurrency))
                    .ForMember(f => f.CreationDate, x => x.MapFrom(src => src.CreationDate))
                    .ForMember(f => f.LastModifiedDate, x => x.MapFrom(src => src.LastModifiedDate))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DaUserWPayIDSettingSo, DaUserWPayIDSettingDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.UserId, x => x.MapFrom(src => src.UserID))
                    .ForMember(f => f.WPayId, x => x.MapFrom(src => src.WPayId))
                    .ForMember(f => f.CcyCode, x => x.MapFrom(src => src.CcyCode))
                    .ForMember(f => f.IsPinRequired, x => x.MapFrom(src => src.IsPinRequired))
                    .ForMember(f => f.PinCode, x => x.MapFrom(src => src.PinCode))
                    .ForMember(f => f.ExceedingAmount, x => x.MapFrom(src => src.ExceedingAmount))
                    .ForMember(f => f.IsRestrictedToSelectedCurrency, x => x.MapFrom(src => src.IsRestrictedToSelectedCurrency))
                    .ForMember(f => f.CreationDate, x => x.MapFrom(src => src.CreationDate))
                    .ForMember(f => f.LastModifiedDate, x => x.MapFrom(src => src.LastModifiedDate))
                    .ForMember(f => f.IsDeleted, x => x.MapFrom(src => src.IsDeleted))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #endregion

                #region Automatic Exhange

                #region DependencyLiquidForUser
                am.RecognizeDestinationPrefixes("DependencyLiquidForUser_");
                am.RecognizePrefixes("DependencyLiquidForUser_");

                am.CreateMap<DependencyLiquidForUserDto, DependencyLiquidForUserSo>()
                    .ForMember(f => f.DependencyLiquidForUser_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.DependencyLiquidForUser_UserId, x => x.MapFrom(src => src.UserId))
                    .ForMember(f => f.DependencyLiquidForUser_LiquidCcyId, x => x.MapFrom(src => src.LiquidCcyId))
                    .ForMember(f => f.DependencyLiquidForUser_LiquidOrder, x => x.MapFrom(src => src.LiquidOrder))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<DependencyLiquidForUserSo, DependencyLiquidForUserDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.DependencyLiquidForUser_Id))
                    .ForMember(f => f.UserId, x => x.MapFrom(src => src.DependencyLiquidForUser_UserId))
                    .ForMember(f => f.LiquidCcyId, x => x.MapFrom(src => src.DependencyLiquidForUser_LiquidCcyId))
                    .ForMember(f => f.LiquidOrder, x => x.MapFrom(src => src.DependencyLiquidForUser_LiquidOrder))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region LiquidCcyList
                am.RecognizeDestinationPrefixes("LiquidCcyList_");
                am.RecognizePrefixes("LiquidCcyList_");

                am.CreateMap<LiquidCcyListDto, LiquidCcyListSo>()
                    .ForMember(f => f.LiquidCcyList_Id, x => x.MapFrom(src => src.Id))
                    //.ForMember(f => f.LiquidCcyList_CurrencyId, x => x.MapFrom(src => src.CurrencyId))
                    .ForMember(f => f.LiquidCcyList_IsLiquidCurrency, x => x.MapFrom(src => src.IsLiquidCurrency))
                    .ForMember(f => f.LiquidCcyList_LiquidOrder, x => x.MapFrom(src => src.LiquidOrder))
                    .ForMember(f => f.LiquidCcyList_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<LiquidCcyListSo, LiquidCcyListDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.LiquidCcyList_Id))
                    // .ForMember(f => f.CurrencyId, x => x.MapFrom(src => src.LiquidCcyList_CurrencyId))
                    .ForMember(f => f.IsLiquidCurrency, x => x.MapFrom(src => src.LiquidCcyList_IsLiquidCurrency))
                    .ForMember(f => f.LiquidOrder, x => x.MapFrom(src => src.LiquidCcyList_LiquidOrder))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.LiquidCcyList_CurrencyCode))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region LiquidOverDraftUserList
                am.RecognizeDestinationPrefixes("LiquidOverDraftUserList_");
                am.RecognizePrefixes("LiquidOverDraftUserList_");

                am.CreateMap<LiquidOverDraftUserListDto, LiquidOverDraftUserListSo>()
                    .ForMember(f => f.LiquidOverDraftUserList_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.LiquidOverDraftUserList_UserId, x => x.MapFrom(src => src.UserId))
                    .ForMember(f => f.LiquidOverDraftUserList_UserName, x => x.MapFrom(src => src.UserName))
                    .ForMember(f => f.LiquidOverDraftUserList_AccountRep, x => x.MapFrom(src => src.AccountRep))
                    .ForMember(f => f.LiquidOverDraftUserList_CreationDate, x => x.MapFrom(src => src.CreationDate))
                    .ForMember(f => f.LiquidOverDraftUserList_FullName, x => x.MapFrom(src => src.FullName))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<LiquidOverDraftUserListSo, LiquidOverDraftUserListDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.LiquidOverDraftUserList_Id))
                    .ForMember(f => f.UserId, x => x.MapFrom(src => src.LiquidOverDraftUserList_UserId))
                    .ForMember(f => f.UserName, x => x.MapFrom(src => src.LiquidOverDraftUserList_UserName))
                    .ForMember(f => f.AccountRep, x => x.MapFrom(src => src.LiquidOverDraftUserList_AccountRep))
                    .ForMember(f => f.CreationDate, x => x.MapFrom(src => src.LiquidOverDraftUserList_CreationDate))
                    .ForMember(f => f.FullName, x => x.MapFrom(src => src.LiquidOverDraftUserList_FullName))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #endregion
                
                #region UserInfo block - Aliases
                am.RecognizeDestinationPrefixes("Wpay_");
                am.RecognizePrefixes("Wpay_");

                am.CreateMap<UserAliasesDto, UserAliasesSo>()
                    .ForMember(f => f.Wpay_Ids, x => x.MapFrom(src => Newtonsoft.Json.JsonConvert.DeserializeObject<List<string>>(src.UserAliases)))
                    .ForMember(f => f.Wpay_UserId, x => x.MapFrom(src => src.UserId))
                    .ForMember(f => f.Wpay_UserName, x => x.MapFrom(src => src.UserName))
                    .ForAllOtherMembers(f => f.Ignore());
                am.CreateMap<UserAliasesSo, UserAliasesDto>()
                    .ForMember(f => f.UserAliases, x => x.MapFrom(src => Newtonsoft.Json.JsonConvert.SerializeObject(src.Wpay_Ids).ToLower()))
                    .ForMember(f => f.UserId, x => x.MapFrom(src => src.Wpay_UserId))
                    .ForMember(f => f.UserName, x => x.MapFrom(src => src.Wpay_UserName))
                    .ForAllOtherMembers(f => f.Ignore());

                #endregion

                #region Transfers
                am.RecognizeDestinationPrefixes("Transfers_");
                am.RecognizePrefixes("Transfers_");

                am.CreateMap<TransfersDto, TransfersSo>()
                    .ForMember(f => f.Transfers_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.Transfers_TransferParent, x => x.MapFrom(src => src.TransferParent))
                    .ForMember(f => f.Transfers_TransferRecipient, x => x.MapFrom(src => src.TransferRecipient))
                    .ForMember(f => f.Transfers_CreatedDate, x => x.MapFrom(src => src.CreatedDate))
                    .ForMember(f => f.Transfers_AcceptedDate, x => x.MapFrom(src => src.AcceptedDate))
                    .ForMember(f => f.Transfers_IsKycCreated, x => x.MapFrom(src => src.IsKycCreated))
                    .ForMember(f => f.Transfers_KycLinkId, x => x.MapFrom(src => src.KycLinkId))
                    .ForMember(f => f.Transfers_SourceType, x => x.MapFrom(src => (TransfersSourceTypeEnum) src.SourceType))
                    .ForMember(f => f.Transfers_Source, x => x.MapFrom(src => src.Source))
                    .ForMember(f => f.Transfers_LinkToSourceRow, x => x.MapFrom(src => src.LinkToSourceRow))
                    .ForMember(f => f.Transfers_IsRejected, x => x.MapFrom(src => src.IsRejected))
                    .ForMember(f => f.Transfers_Info, x => x.MapFrom(src => src.Info))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<TransfersSo, TransfersDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.Transfers_Id))
                    .ForMember(f => f.TransferParent, x => x.MapFrom(src => src.Transfers_TransferParent))
                    .ForMember(f => f.TransferRecipient, x => x.MapFrom(src => src.Transfers_TransferRecipient))
                    .ForMember(f => f.CreatedDate, x => x.MapFrom(src => src.Transfers_CreatedDate))
                    .ForMember(f => f.AcceptedDate, x => x.MapFrom(src => src.Transfers_AcceptedDate))
                    .ForMember(f => f.IsKycCreated, x => x.MapFrom(src => src.Transfers_IsKycCreated))
                    .ForMember(f => f.KycLinkId, x => x.MapFrom(src => src.Transfers_KycLinkId))
                    .ForMember(f => f.SourceType, x => x.MapFrom(src => (int)src.Transfers_SourceType))
                    .ForMember(f => f.Source, x => x.MapFrom(src => src.Transfers_Source))
                    .ForMember(f => f.LinkToSourceRow, x => x.MapFrom(src => src.Transfers_LinkToSourceRow))
                    .ForMember(f => f.IsRejected, x => x.MapFrom(src => src.Transfers_IsRejected))
                    .ForMember(f => f.Info, x => x.MapFrom(src => src.Transfers_Info))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region GetInboxList
                am.RecognizeDestinationPrefixes("GetInboxList_");
                am.RecognizePrefixes("GetInboxList_");

                am.CreateMap<GetInboxListDto, GetInboxListSo>()
                    .ForMember(f => f.GetInboxList_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.GetInboxList_TransferParent, x => x.MapFrom(src => src.TransferParent))
                    .ForMember(f => f.GetInboxList_TransferRecipient, x => x.MapFrom(src => src.TransferRecipient))
                    .ForMember(f => f.GetInboxList_CreatedDate, x => x.MapFrom(src => src.CreatedDate))
                    .ForMember(f => f.GetInboxList_AcceptedDate, x => x.MapFrom(src => src.AcceptedDate))
                    .ForMember(f => f.GetInboxList_IsKycCreated, x => x.MapFrom(src => src.IsKycCreated))
                    .ForMember(f => f.GetInboxList_KycLinkId, x => x.MapFrom(src => src.KycLinkId))
                    .ForMember(f => f.GetInboxList_SourceType, x => x.MapFrom(src => (TransfersSourceTypeEnum)src.SourceType))
                    .ForMember(f => f.GetInboxList_Source, x => x.MapFrom(src => src.Source))
                    .ForMember(f => f.GetInboxList_LinkToSourceRow, x => x.MapFrom(src => src.LinkToSourceRow))
                    .ForMember(f => f.GetInboxList_IsRejected, x => x.MapFrom(src => src.IsRejected))
                    .ForMember(f => f.GetInboxList_TypeRecBySource, x => x.MapFrom(src => (DelegatedAuthorirySourceLimitationTypeEnum) src.TypeRecBySource))
                    .ForMember(f => f.GetInboxList_MediaNameByTransferToken, x => x.MapFrom(src => src.MediaNameByTransferToken))
                    .ForMember(f => f.GetInboxList_Status, x => x.MapFrom(src =>(TransferStatusesEnum) src.Status))
                    .ForMember(f => f.GetInboxList_Info, x => x.MapFrom(src => src.Info))
                    .ForAllOtherMembers(f => f.Ignore());

                #endregion
                
                #region RedEnvelope
                am.RecognizeDestinationPrefixes("RedEnvelope_");
                am.RecognizePrefixes("RedEnvelope_");

                am.CreateMap<RedEnvelopeDto, RedEnvelopeSo>()
                    .ForMember(f => f.RedEnvelope_Id, x => x.MapFrom(src => src.Id))
                    .ForMember(f => f.RedEnvelope_CurrencyCode, x => x.MapFrom(src => src.CurrencyCode))
                    .ForMember(f => f.RedEnvelope_Amount, x => x.MapFrom(src => src.Amount))
                    .ForMember(f => f.RedEnvelope_Note, x => x.MapFrom(src => src.Note))
                    .ForMember(f => f.RedEnvelope_FilePath, x => x.MapFrom(src => src.FilePath))
                    .ForMember(f => f.RedEnvelope_IsSuccessTransferToRedEnvelopeAcc, x => x.MapFrom(src => src.IsSuccessTransferToRedEnvelopeAcc))
                    .ForMember(f => f.RedEnvelope_IsNeedToNotifyByEmail, x => x.MapFrom(src => src.IsNeedToNotifyByEmail))
                    .ForMember(f => f.RedEnvelope_RedEnvelopePaymentId, x => x.MapFrom(src => src.RedEnvelopePaymentId))
                    .ForMember(f => f.RedEnvelope_RecipientPaymentId, x => x.MapFrom(src => src.RecipientPaymentId))
                    .ForMember(f => f.RedEnvelope_IsSuccessTransferToRecipient, x => x.MapFrom(src => src.IsSuccessTransferToRecipient))
                    .ForMember(f => f.RedEnvelope_DateTransferedToRedEnvelope, x => x.MapFrom(src => src.DateTransferedToRedEnvelope))
                    .ForMember(f => f.RedEnvelope_RecipientUserName, x => x.MapFrom(src => src.RecipientUserName))
                    .ForMember(f => f.RedEnvelope_RejectionNote, x => x.MapFrom(src => src.RejectionNote))
                    .ForMember(f => f.RedEnvelope_WPayIdTo, x => x.MapFrom(src => src.WPayIdTo))
                    .ForMember(f => f.RedEnvelope_WPayIdFrom, x => x.MapFrom(src => src.WPayIdFrom))
                    .ForMember(f => f.RedEnvelope_DateTransferedToRecipient, x => x.MapFrom(src => src.DateTransferedToRecipient))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<RedEnvelopeSo, RedEnvelopeDto>()
                    .ForMember(f => f.Id, x => x.MapFrom(src => src.RedEnvelope_Id))
                    .ForMember(f => f.CurrencyCode, x => x.MapFrom(src => src.RedEnvelope_CurrencyCode))
                    .ForMember(f => f.Amount, x => x.MapFrom(src => src.RedEnvelope_Amount))
                    .ForMember(f => f.Note, x => x.MapFrom(src => src.RedEnvelope_Note))
                    .ForMember(f => f.FilePath, x => x.MapFrom(src => src.RedEnvelope_FilePath))
                    .ForMember(f => f.IsSuccessTransferToRedEnvelopeAcc, x => x.MapFrom(src => src.RedEnvelope_IsSuccessTransferToRedEnvelopeAcc))
                    .ForMember(f => f.IsNeedToNotifyByEmail, x => x.MapFrom(src => src.RedEnvelope_IsNeedToNotifyByEmail))
                    .ForMember(f => f.RedEnvelopePaymentId, x => x.MapFrom(src => src.RedEnvelope_RedEnvelopePaymentId))
                    .ForMember(f => f.RecipientPaymentId, x => x.MapFrom(src => src.RedEnvelope_RecipientPaymentId))
                    .ForMember(f => f.IsSuccessTransferToRecipient, x => x.MapFrom(src => src.RedEnvelope_IsSuccessTransferToRecipient))
                    .ForMember(f => f.DateTransferedToRedEnvelope, x => x.MapFrom(src => src.RedEnvelope_DateTransferedToRedEnvelope))
                    .ForMember(f => f.DateTransferedToRecipient, x => x.MapFrom(src => src.RedEnvelope_DateTransferedToRecipient))
                    .ForMember(f => f.WPayIdTo, x => x.MapFrom(src => src.RedEnvelope_WPayIdTo))
                    .ForMember(f => f.WPayIdFrom, x => x.MapFrom(src => src.RedEnvelope_WPayIdFrom))
                    .ForMember(f => f.RecipientUserName, x => x.MapFrom(src => src.RedEnvelope_RecipientUserName))
                    .ForMember(f => f.RejectionNote, x => x.MapFrom(src => src.RedEnvelope_RejectionNote))
                    .ForAllOtherMembers(f => f.Ignore());
                #endregion

                #region Super Admin block

                am.RecognizeDestinationPrefixes("SharedAdminLink_");
                am.RecognizePrefixes("SharedAdminLink_");

                am.CreateMap<SharedAdminLinkDto, SharedAdminLinkSo>()
                    .ForMember(f => f.SharedAdminLink_ID, x => x.MapFrom(src => src.ID))
                    .ForMember(f => f.SharedAdminLink_UserName, x => x.MapFrom(src => src.UserName))
                    .ForMember(f => f.SharedAdminLink_FirstName, x => x.MapFrom(src => src.FirstName))
                    .ForMember(f => f.SharedAdminLink_LastName, x => x.MapFrom(src => src.LastName))
                    .ForMember(f => f.SharedAdminLink_Email, x => x.MapFrom(src => src.Email))
                    .ForMember(f => f.SharedAdminLink_CreationDate, x => x.MapFrom(src => src.CreationDate))
                    .ForMember(f => f.SharedAdminLink_LinkAddress, x => x.MapFrom(src => src.LinkAddress))
                    .ForMember(f => f.SharedAdminLink_StatusLink, x => x.MapFrom(src => src.StatusLink))
                    .ForMember(f => f.SharedAdminLink_ActivationDate, x => x.MapFrom(src => src.ActivationDate))
                    .ForAllOtherMembers(f => f.Ignore());

                am.CreateMap<SharedAdminLinkSo, SharedAdminLinkDto>()
                    .ForMember(f => f.ID, x => x.MapFrom(src => src.SharedAdminLink_ID))
                    .ForMember(f => f.UserName, x => x.MapFrom(src => src.SharedAdminLink_UserName))
                    .ForMember(f => f.FirstName, x => x.MapFrom(src => src.SharedAdminLink_FirstName))
                    .ForMember(f => f.LastName, x => x.MapFrom(src => src.SharedAdminLink_LastName))
                    .ForMember(f => f.Email, x => x.MapFrom(src => src.SharedAdminLink_Email))
                    .ForMember(f => f.CreationDate, x => x.MapFrom(src => src.SharedAdminLink_CreationDate))
                    .ForMember(f => f.LinkAddress, x => x.MapFrom(src => src.SharedAdminLink_LinkAddress))
                    .ForMember(f => f.StatusLink, x => x.MapFrom(src => src.SharedAdminLink_StatusLink))
                    .ForMember(f => f.ActivationDate, x => x.MapFrom(src => src.SharedAdminLink_ActivationDate))
                    .ForAllOtherMembers(f => f.Ignore());


                #endregion
            });
            Mapper.AssertConfigurationIsValid();
        }
    }
}