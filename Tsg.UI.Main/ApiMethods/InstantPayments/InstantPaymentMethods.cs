using System;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Web.Hosting;
using System.Web.Mvc;
using StaticExtensions;
using Tsg.UI.Main.App_LocalResources;
using Tsg.UI.Main.Models;
using Tsg.UI.Main.Models.Security;
using Tsg.UI.Main.Repository;
using TSG.Models.APIModels;
using TSG.Models.APIModels.InstantPayment;

namespace Tsg.UI.Main.ApiMethods.Payments
{
    public class InstantPaymentMethods : BaseApiMethods
    {
        private UserInfo _userInfo { get; set; }

        public InstantPaymentMethods(UserInfo ui) : base(ui)
        {
            _userInfo = ui;
            _logger = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        }

        public string GetImageLink(ApiInstantPaymentDetailsViewModel model, string res, UserInfo ui)
        {
            string imageId;
            DateTime dt;
            PaymentRepository pr = new PaymentRepository();
            if (!Directory.Exists(HostingEnvironment.MapPath("~/UserFiles/PaymentDetails")))
            {
                Directory.CreateDirectory(HostingEnvironment.MapPath("~/UserFiles/PaymentDetails"));
            }
            var checkPrevPhoto = pr.GetSharedPhoto(model.PaymentId.ToString(), out imageId, out dt);
            if (checkPrevPhoto != null && Convert.ToBoolean(checkPrevPhoto.IsSuccess) 
                && !String.IsNullOrEmpty(imageId.Trim()) &&
                Guid.Parse(imageId) != default (Guid)  && dt != default (DateTime))
            {
                if (dt < DateTime.Now)
                    System.IO.File.Delete($"{HostingEnvironment.MapPath("~/UserFiles/PaymentDetails")}/{imageId}.png");

                else if (System.IO.File.Exists(
                    HostingEnvironment.MapPath("~/UserFiles/PaymentDetails/" + imageId + ".png")))
                {
                    return imageId;
                }
            }
            var createRecRes = pr.AddOrUpdateSharedPhoto(model.PaymentId.ToString(), out imageId);
            if (createRecRes.IsSuccess != null && Convert.ToBoolean(createRecRes.IsSuccess))
            {
                using (Bitmap image = new Bitmap(440, 530))
                {
                    using (Graphics g = Graphics.FromImage(image))
                    {
                        SolidBrush backBrush = new SolidBrush(Color.FromArgb(253, 253, 253));
                        SolidBrush headColorBrush = new SolidBrush(Color.FromArgb(0, 131, 193));
                        SolidBrush mainColorBrush = new SolidBrush(Color.FromArgb(51, 51, 51));
                        SolidBrush smothColorBrush = new SolidBrush(Color.FromArgb(249, 249, 249));
                        g.FillRectangle(backBrush, new Rectangle(0, 0, 440, 530));

                        Image logo = Image.FromFile(HostingEnvironment.MapPath("~/Content/assets/images/winstantpay-logo-notag.png"));
                        logo = logo.ResizeImage(90, 30);
                        g.DrawImage(logo, new Point(420 - logo.Width, 510 - logo.Height));

                        #region Draw horiz line

                        g.FillRectangle(headColorBrush, new Rectangle(20, 60, 250, 2));

                        #endregion

                        #region Draw Head Text

                        string instPayText = GlobalRes.Payment_PaymentController_GetImageLink_PaymentDetails;
                        Font drawHeadFont = new Font("Open Sans", 22);
                        Font drawBoldFont = new Font("Open Sans", 12, FontStyle.Bold);
                        Font drawRegFont = new Font("Open Sans", 10);
                        PointF stringPonit = new PointF(20, 20);
                        g.DrawString(instPayText, drawHeadFont, mainColorBrush, stringPonit,
                            StringFormat.GenericTypographic);

                        #endregion

                        Pen recPen = new Pen(Color.FromArgb(221, 221, 221));
                        //-------------------------------------------------------------------//
                        g.DrawRectangle(recPen, new Rectangle(20, 70, 200, 40));
                        g.FillRectangle(smothColorBrush, new Rectangle(21, 71, 199, 39));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_PaymentReference, drawBoldFont, mainColorBrush, new PointF(25, 78),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 110, 200, 40));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Status, drawBoldFont, mainColorBrush, new PointF(25, 118),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 150, 200, 40));
                        g.FillRectangle(smothColorBrush, new Rectangle(21, 151, 199, 39));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_From, drawBoldFont, mainColorBrush, new PointF(25, 158),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 190, 200, 40));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_To, drawBoldFont, mainColorBrush, new PointF(25, 198),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 230, 200, 40));
                        g.FillRectangle(smothColorBrush, new Rectangle(21, 231, 199, 39));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Date, drawBoldFont, mainColorBrush, new PointF(25, 238),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 270, 200, 40));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Currency, drawBoldFont, mainColorBrush, new PointF(25, 278),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 310, 200, 40));
                        g.FillRectangle(smothColorBrush, new Rectangle(21, 311, 199, 39));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Amount, drawBoldFont, mainColorBrush, new PointF(25, 318),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 350, 200, 40));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Invoice, drawBoldFont, mainColorBrush, new PointF(25, 358),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 390, 200, 90));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_Memo, drawBoldFont, mainColorBrush, new PointF(25, 398),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(20, 440, 200, 140));
                        g.DrawString(GlobalRes.Payment_PaymentController_GetImageLink_ReasonForPayment, drawBoldFont, mainColorBrush, new PointF(25, 448),
                            StringFormat.GenericTypographic);
                        //------------------------------------------------------------------------------------------------//

                        g.FillRectangle(smothColorBrush, new Rectangle(221, 71, 199, 38));
                        g.DrawRectangle(recPen, new Rectangle(220, 70, 199, 40));
                        g.DrawString(!String.IsNullOrEmpty(model.PaymentReference) ? model.PaymentReference : "", drawRegFont, mainColorBrush,
                            new RectangleF(225, 70, 199, 40), StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(220, 110, 199, 40));
                        g.DrawString(!String.IsNullOrEmpty(model.PaymentStatus) ? model.PaymentStatus : "", drawRegFont, mainColorBrush,
                            new RectangleF(225, 110, 199, 40), StringFormat.GenericTypographic);

                        g.FillRectangle(smothColorBrush, new Rectangle(221, 151, 199, 38));
                        g.DrawRectangle(recPen, new Rectangle(220, 150, 199, 40));
                        g.DrawString(!String.IsNullOrEmpty(model.FromCustomerAlias) ? model.FromCustomerAlias : "", drawRegFont, mainColorBrush,
                            new RectangleF(225, 150, 199, 40), StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(220, 190, 199, 40));
                        g.DrawString(!String.IsNullOrEmpty(model.ToCustomerAlias) ? model.ToCustomerAlias : "", drawRegFont, mainColorBrush,
                            new RectangleF(225, 190, 199, 40), StringFormat.GenericTypographic);

                        g.FillRectangle(smothColorBrush, new Rectangle(221, 231, 199, 38));
                        g.DrawRectangle(recPen, new Rectangle(220, 230, 199, 40));
                        g.DrawString(model.CreatedTime != default(DateTime) ? model.CreatedTime.ToString(CultureInfo.InvariantCulture) : "", drawRegFont, mainColorBrush,
                            new RectangleF(225, 230, 199, 40), StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(220, 270, 199, 40));
                        g.DrawString(!String.IsNullOrEmpty(model.Currency) ? model.Currency : "", drawRegFont, mainColorBrush, new RectangleF(225, 270, 199, 40),
                            StringFormat.GenericTypographic);

                        g.FillRectangle(smothColorBrush, new Rectangle(221, 311, 199, 40));
                        g.DrawRectangle(recPen, new Rectangle(220, 310, 199, 40));
                        if (model.Currency != "BTC")
                        {
                            g.DrawString(model.Amount != default(decimal) ? model.Amount.ToString("N2", CultureInfo.GetCultureInfo("en-US")) : "", drawRegFont, mainColorBrush,
                                new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);
                        }
                        else
                        {
                            g.DrawString(model.Amount.ToString("N8", CultureInfo.GetCultureInfo("en-US")), drawRegFont, mainColorBrush,
                                new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);
                        }
                        //g.DrawString(model.Amount != default(decimal) ? model.Amount.ToString(CultureInfo.InvariantCulture):"", drawRegFont, mainColorBrush,
                        //    new RectangleF(225, 310, 199, 40), StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(220, 350, 199, 40));
                        g.DrawString(!String.IsNullOrEmpty(model.Invoice) ? model.Invoice : "", drawRegFont, mainColorBrush, new RectangleF(225, 350, 199, 40),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(220, 390, 199, 90));
                        g.DrawString(!String.IsNullOrEmpty(model.BankMemo) ? model.BankMemo : "", drawRegFont, mainColorBrush, new RectangleF(225, 390, 199, 90),
                            StringFormat.GenericTypographic);

                        g.DrawRectangle(recPen, new Rectangle(220, 440, 199, 140));
                        g.DrawString(!String.IsNullOrEmpty(model.ReasonForPayment) ? model.ReasonForPayment : "", drawRegFont, mainColorBrush, new RectangleF(225, 440, 199, 140),
                            StringFormat.GenericTypographic);
                    }
                    MemoryStream ms = new MemoryStream();

                    image.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                    image.Save(String.Format("{0}/{1}.png", HostingEnvironment.MapPath("~/UserFiles/PaymentDetails"), imageId),
                        System.Drawing.Imaging.ImageFormat.Png);

                }
                return imageId;
            }

            return null;
        }

        public ApiInstantPaymentDetailsViewModel PrepareDetails(Guid paymentId)
        {
            var response = Service.GetInstantPaymentDetails(paymentId);

            if (!response.ServiceResponse.HasErrors)
            {
                
                return new ApiInstantPaymentDetailsViewModel()
                {
                    PaymentId = new Guid(response.Payment.PaymentId),
                    PaymentReference = response.Payment.PaymentReference,
                    FromCustomerAlias = response.Payment.FromCustomerAlias,
                    ToCustomerAlias = response.Payment.ToCustomerAlias,
                    FromCustomerName = response.Payment.FromCustomerName,
                    ToCustomerName = response.Payment.ToCustomerName,
                    FromCustomerId = response.Payment.FromCustomerId,
                    ToCustomerId = response.Payment.ToCustomerId,
                    PaymentStatus = response.Payment.PaymentStatus,
                    Amount = response.Payment.Amount,
                    AmountCurrencyScale = response.Payment.AmountCurrencyScale,
                    Currency = response.Payment.CCY,
                    ValueDate = Convert.ToDateTime(response.Payment.ValueDate),
                    ProcessingBranchCode = response.Payment.ProcessingBranchCode,
                    ProcessingBranchName = response.Payment.ProcessingBranchName,
                    CreatedTime = Convert.ToDateTime(response.Payment.CreatedTime),
                    CreatedByName = response.Payment.CreatedByName,
                    PostedTime = !String.IsNullOrEmpty(response.Payment.PostedTime)? Convert.ToDateTime(response.Payment.PostedTime) : (DateTime?) null,                    
                    PostedByName = response.Payment.PostedByName,
                    IsDeleted = response.Payment.IsDeleted,
                    BankMemo = response.Payment.BankMemo,
                    Invoice = response.Payment.ExternalReference,
                    ReasonForPayment = response.Payment.ReasonForPayment,
                };
            }

            return null;
        }
    }
}