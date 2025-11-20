using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using System.Web.Mvc;

namespace Tsg.UI.Main.Models
{
    public class UserMailModel
    {
        public string MailTo { get; set; }
        [AllowHtml]        
        public string MailFrom { get; set; }
        public string MailCopy{ get; set; }
        [AllowHtml]        
        public string Subject { get; set; }
        [AllowHtml]
        public string MailText { get; set; }
        public bool? IsSuccessResForAction { get; set; }
        public string ActionResultText { get; set; }
        public List<SelectListItem> ListOfContactList { get; set; }
        [FileExtensions(Extensions = "*.jpg,*.gif,*.png", ErrorMessage = "Incorrect format")]
        public HttpPostedFileBase Attachments { get; set; }
    }
}