using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Helpers;
using System.Web.Http;
using Tsg.UI.Main.ApiMethods.Payments;
using Tsg.UI.Main.BlockchainIntegration;
using TSG.Models.APIModels.Fundings.BlockchainInfo;
using TSG.Models.APIModels.InstantPayment;
using TSG.ServiceLayer.Interfaces.Fundings;

namespace Tsg.UI.Main.APIControllers.Fundings.BlockchainInfo
{
    public class PaymentBlockChainInfoCallBackController : ApiController
    {
        private const string secretKeyForBlockChain = "4EFC770B-4D3B-490A-B5C3-56377995C374";
        private readonly IFundingsService _fundingsService;
        readonly log4net.ILog _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public PaymentBlockChainInfoCallBackController(IFundingsService fundingsService)
        {
            _fundingsService = fundingsService;
        }

        [HttpGet]
        public IHttpActionResult Get(string request_id, string secret)
        {
            return Ok("*bad*");
        }


        [HttpPost]
        public IHttpActionResult Post(string request_id, string secret, [FromBody] BlockchainInfoRecieveResporse model)
        {
            _logger.Info("*** Query entered ***");
            var greq_id = Guid.Parse(request_id);
            var gsec = Guid.Parse(secret);
            if (greq_id != default && gsec != default && gsec.ToString().ToLower() == secret.ToLower())
            {
                _logger.Info("Params parsed");
                if (model != null)
                {
                    _logger.Info($"request_id:{request_id}; secret:{secret}; {model.Address}, {model.Confirmations}, {model.CustomParameter}, {model.TransactionHash}, {model.Value}");
                    _logger.Info($"{Request.RequestUri.DnsSafeHost}");
                }
                else
                {
                    _logger.Info("BlockchainInfoRecieveResporse model is null");
                    return BadRequest("Empty model");
                }
                var fund = _fundingsService.GetBlockChainInfoFundingByTimeStamp(greq_id).Obj;
                if (fund != null)
                {
                    if (fund.AddFundsBlockChainInfo_NumberOfConfirmation == model.Confirmations && (fund.AddFundsBlockChainInfo_ConfirmatedTransaction == null || fund.AddFundsBlockChainInfo_ConfirmatedTransaction == 0))
                    {
                        var dnsAddress = Request.RequestUri.DnsSafeHost;
                        fund.AddFundsBlockChainInfo_ValueInSatoshi = model.Value;
                        fund.AddFundsBlockChainInfo_RequestUrl = dnsAddress;
                        fund.AddFundsBlockChainInfo_TransactionHash = model.TransactionHash;
                        fund.AddFundsBlockChainInfo_CustomParameter = model.CustomParameter;
                        fund.AddFundsBlockChainInfo_ConfirmatedTransaction = model.Confirmations;

                        var updateRes = _fundingsService.UpdateBlockChainInfoTransfer(fund);

                        if (updateRes.Success)
                        {
                            try
                            {
                                NewInstantPaymentMethods payment = new NewInstantPaymentMethods(
                                    new TSG.Models.APIModels.UserInfo()
                                    {
                                        UserId = ConfigurationManager.AppSettings["cryptoCallerId"],
                                        UserName = ConfigurationManager.AppSettings["cryptoUser"],
                                        Password = ConfigurationManager.AppSettings["cryptoPassword"]
                                    });
                                var res = payment.Create(new ApiNewInstantPaymentViewModel()
                                {
                                    FromCustomer = ConfigurationManager.AppSettings["senderAliasBlockchainInfo"],
                                    ToCustomer = fund.AddFundsBlockChainInfo_Alias,
                                    Amount = Convert.ToDecimal(Convert.ToDecimal(fund.AddFundsBlockChainInfo_ValueInSatoshi) / 100000000L),
                                    CurrencyCode = "BTC",
                                    Memo = "BlockChain.info ",
                                    Invoice = $"{fund.AddFundsBlockChainInfo_BlockChainAddress}"
                                });
                                _logger.Info(res.ServiceResponse.Responses[0].Message);
                                _logger.Info(res.ServiceResponse.Responses[0].MessageDetails);

                                if (res.PaymentInformation != null)
                                {
                                    var resPost = payment.Post(Guid.Parse(res.PaymentInformation.PaymentId));
                                    _logger.Info(resPost.ServiceResponse.Responses[0].Message);
                                    _logger.Info(resPost.ServiceResponse.Responses[0].MessageDetails);
                                }
                                return Ok("*ok*");
                            }
                            catch (Exception e) { _logger.Error("TSG " + e.Message); }

                        }
                        else return BadRequest(updateRes.Message);
                    }
                    return Ok("*bad*");

                }
                else return BadRequest("Doesn't find request_id");
            }
            else
            {
                return BadRequest("Incorrect params");
            }
        }
    }
}
