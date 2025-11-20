using Autofac;
using TSG.Dal.AutomaticExchange.DependencyLiquidForUserDalMethods;
using TSG.Dal.AutomaticExchange.LiquidCcyListDalMethods;
using TSG.Dal.AutomaticExchange.LiquidOverDraftUserDalMethods;
using TSG.Dal.Fundings;
using TSG.Dal.Interfaces.Fundings;
using TSG.Dal.Kyc;
using TSG.Dal.LimitPayment.DaPayLimitsDalMethods;
using TSG.Dal.LimitPayment.DaPayLimitsLogDalMethods;
using TSG.Dal.LimitPayment.DaPayLimitsTabDalMethods;
using TSG.Dal.LimitPayment.DaPayLimitsTypeDalMethods;
using TSG.Dal.LimitPayment.DaPaymentLimitSourceTypeDalMethods;
using TSG.Dal.RedEnvelope;
using TSG.Dal.SuperAdmin;
using TSG.Dal.Transfers;
using TSG.Dal.Transfers.Reports;
using TSG.Dal.Users;
using TSG.Dal.WinstantPayShopDal.CategoriesDal;
using TSG.Dal.WinstantPayShopDal.ShopInfoDal;
using TSG.Dal.WinstantPayShopDal.ShopOrderItemsDal;
using TSG.Dal.WinstantPayShopDal.ShopOrdersDal;
using TSG.Dal.WinstantPayShopDal.ShopOrdersLogDal;
using TSG.Dal.WinstantPayShopDal.ShopPaymentDal;
using TSG.Dal.WinstantPayShopDal.ShopProductImagesDal;
using TSG.Dal.WinstantPayShopDal.ShopProductsDal;
using TSG.ServiceLayer.AutomaticExchange.DependencyLiquidForUserServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidCcyListServiceMethods;
using TSG.ServiceLayer.AutomaticExchange.LiquidOverDraftUserServiceMethods;
using TSG.ServiceLayer.Funding;
using TSG.ServiceLayer.Interfaces.Fundings;
using TSG.ServiceLayer.Kyc;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsLogServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsSourceTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTabServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaPayLimitsTypeServiceMethods;
using TSG.ServiceLayer.LimitPayment.DaUserWPayIDSettingServiceMethods;
using TSG.ServiceLayer.RedEnvelope;
using TSG.ServiceLayer.SuperAdmin;
using TSG.ServiceLayer.Transfers;
using TSG.ServiceLayer.Transfers.Reports;
using TSG.ServiceLayer.Users;
using TSG.ServiceLayer.WinstantPayShop.Categories;
using TSG.ServiceLayer.WinstantPayShop.ShopInfoService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrderItemsService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrdersLogService;
using TSG.ServiceLayer.WinstantPayShop.ShopOrdersService;
using TSG.ServiceLayer.WinstantPayShop.ShopPaymentService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductImagesService;
using TSG.ServiceLayer.WinstantPayShop.ShopProductService;
using TSG.ServiceLayer.InstantPayment;
using TSG.Dal.InstantPayment;
using TSG.Dal.LimitPayment.DaUserWPayIDSettingDalMethods;

namespace TSG.ServiceLayer
{
    public class Injector
    {
        public static void Bootstrap(ContainerBuilder builder)
        {
            #region Service layer

            #region Fundings
            builder.RegisterType<FundingSourcesServices>().As<IFundingSourcesService>().SingleInstance();
            builder.RegisterType<FundingsServices>().As<IFundingsService>().SingleInstance();
            builder.RegisterType<FundingChangeService>().As<IFundingChangesService>().SingleInstance();
            #endregion

            #region Users service
            builder.RegisterType<UsersServiceMethods>().As<IUsersServiceMethods>().SingleInstance();
            #endregion

            #region WinstantPay in-shop service

            builder.RegisterType<ShopCategoriesService>().As<IShopCategoryService>().SingleInstance();
            builder.RegisterType<ShopInfoService>().As<IShopInfoService>().SingleInstance();
            builder.RegisterType<ShopProductService>().As<IShopProductService>().SingleInstance();
            builder.RegisterType<ShopProductImagesService>().As<IShopProductImagesService>().SingleInstance();
            builder.RegisterType<ShopOrdersService>().As<IShopOrdersService>().SingleInstance();
            builder.RegisterType<ShopOrderItemsService>().As<IShopOrderItemsService>().SingleInstance();
            builder.RegisterType<ShopOrdersLogService>().As<IShopOrdersLogService>().SingleInstance();
            builder.RegisterType<ShopPaymentService>().As<IShopPaymentService>().SingleInstance();

            #endregion

            #region Payment Limits
            builder.RegisterType<DaPayLimitsLogServiceMethods>().As<IDaPayLimitsLogServiceMethods>().SingleInstance();
            builder.RegisterType<DaPayLimitsServiceMethods>().As<IDaPayLimitsServiceMethods>().SingleInstance();
            builder.RegisterType<DaPayLimitsSourceTypeServiceMethods>().As<IDaPayLimitsSourceTypeServiceMethods>().SingleInstance();
            builder.RegisterType<DaPayLimitsTabServiceMethods>().As<IDaPayLimitsTabServiceMethods>().SingleInstance();
            builder.RegisterType<DaPayLimitsTypeServiceMethods>().As<IDaPayLimitsTypeServiceMethods>().SingleInstance();
            builder.RegisterType<DaUserWPayIDSettingServiceMethods>().As<IDaUserWPayIDSettingServiceMethods>().SingleInstance();

            #endregion

            #region Automatic Exchange service
            builder.RegisterType<LiquidCcyListServiceMethods>().As<ILiquidCcyListServiceMethods>().SingleInstance();
            builder.RegisterType<LiquidOverDraftUserServiceMethods>().As<ILiquidOverDraftUserServiceMethods>().SingleInstance();
            builder.RegisterType<DependencyLiquidForUserServiceMethods>().As<IDependencyLiquidForUserServiceMethods>().SingleInstance();
            #endregion

            #region Transfers
            builder.RegisterType<GetInboxListMethods>().As<IGetInboxListMethods>().SingleInstance();
            builder.RegisterType<TransfersServiceMethods>().As<ITransfersServiceMethods>().SingleInstance();
            #endregion

            #region Mini KYC
            builder.RegisterType<KycNewUserServiceMethods>().As<IKycNewUserServiceMethods>().SingleInstance();
            #endregion

            #region Red Envelope
            builder.RegisterType<RedEnvelopeRepository>().As<IRedEnvelopeRepository>().SingleInstance();
            #endregion

            #region SuperAdmin block
            builder.RegisterType<SaSharedLinkForAdminServiceMethods>().As<ISaSharedLinkForAdminServiceMethods>().SingleInstance();
            #endregion

            #region Instant Payment
            builder.RegisterType<InstantPaymentReceiveMethods>().As<IInstantPaymentReceiveMethods>().SingleInstance();
            builder.RegisterType<InstantPaymentReceiveMappingMethods>().As<IInstantPaymentReceiveMappingMethods>().SingleInstance();
            #endregion

            #endregion

            /******************************************************************************************/

            #region Repository layer

            #region Fundings
            builder.RegisterType<FundingSourcesRepository>().As<IFundingSourcesRepository>().SingleInstance();
            builder.RegisterType<FundingsRepository>().As<IFundingsRepository>().SingleInstance();
            builder.RegisterType<FundingChangesRepository>().As<IFundingChangesRepository>().SingleInstance();
            #endregion

            #region Users method
            builder.RegisterType<UsersDalMethods>().As<IUsersDalMethods>().SingleInstance();
            #endregion

            #region WinstantPay in-shop repository

            builder.RegisterType<ShopCategoriesDal>().As<IShopCategoryDal>().SingleInstance();
            builder.RegisterType<ShopInfoDal>().As<IShopInfoDal>().SingleInstance();
            builder.RegisterType<ShopProductsDal>().As<IShopProductsDal>().SingleInstance();
            builder.RegisterType<ShopProductImagesDal>().As<IShopProductImagesDal>().SingleInstance();
            builder.RegisterType<ShopOrdersDal>().As<IShopOrdersDal>().SingleInstance();
            builder.RegisterType<ShopOrdersItemsDal>().As<IShopOrderItemsDal>().SingleInstance();
            builder.RegisterType<ShopOrdersLogDal>().As<IShopOrdersLogDal>().SingleInstance();
            builder.RegisterType<ShopPaymentDal>().As<IShopPaymentDal>().SingleInstance();

            #endregion

            #region Payment Limits
            builder.RegisterType<DaPayLimitsDal>().As<IDaPayLimitsDal>().SingleInstance();
            builder.RegisterType<DaPayLimitsTypeDal>().As<IDaPayLimitsTypeDal>().SingleInstance();
            builder.RegisterType<DaPaymentLimitSourceTypeDal>().As<IDaPaymentLimitSourceTypeDal>().SingleInstance();
            builder.RegisterType<DaPayLimitsTabDal>().As<IDaPayLimitsTabDal>().SingleInstance();
            builder.RegisterType<DaPayLimitsLogDal>().As<IDaPayLimitsLogDal>().SingleInstance();
            builder.RegisterType<DaUserWPayIDSettingDal>().As<IDaUserWPayIDSettingDal>().SingleInstance();

            #endregion

            #region Automatic Exchange repository
            builder.RegisterType<LiquidCcyListDalMethods>().As<ILiquidCcyListDalMethods>().SingleInstance();
            builder.RegisterType<LiquidOverDraftUserDalMethods>().As<ILiquidOverDraftUserDalMethods>().SingleInstance();
            builder.RegisterType<DependencyLiquidForUserDalMethod>().As<IDependencyLiquidForUserDalMethod>().SingleInstance();
            #endregion

            #region Transfers
            builder.RegisterType<GetInboxReportRepository>().As<IGetInboxReportRepository>().SingleInstance();
            builder.RegisterType<TransfersRepository>().As<ITransfersRepository>().SingleInstance();
            #endregion

            #region Mini KYC
            builder.RegisterType<KycNewUserRepository>().As<IKycNewUserRepository>().SingleInstance();
            #endregion

            #region Red Envelope
            builder.RegisterType<RedEnvelopeServiceMethods>().As<IRedEnvelopeServiceMethods>().SingleInstance();
            #endregion
            
            #region SuperAdmin block
            builder.RegisterType<SaSharedLinkForAdminRepository>().As<ISaSharedLinkForAdminRepository>().SingleInstance();
            #endregion

            #region Instant Payment
            builder.RegisterType<InstantPaymentReceiveRepository>().As<IInstantPaymentReceiveRepository>().SingleInstance();
            builder.RegisterType<InstantPaymentReceiveMappingRepository>().As<IInstantPaymentReceiveMappingRepository>().SingleInstance();
            #endregion

            #endregion

        }
    }
}