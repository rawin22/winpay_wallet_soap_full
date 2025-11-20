using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Tsg.Business.Model.Classes;
using Tsg.Business.Model.TsgGPWebService;
using Tsg.UI.Main.Models.Helpers;
using Tsg.UI.Main.Models.Security;

namespace Tsg.UI.Main.Models
{
    public class ContactUsModel
    {
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; }
        public string DepartmentMail { get; set; }
        public int DepartmentWeight { get; set; }
        public bool ? IsSuccessResForAction { get; set; }
        public string ActionResultText { get; set; }
        public List<ContactUsModel> ListOfContactList { get; set; }
        public string UploadProofInternalMail { get; set; }
        public List<SettingModel> ListOfSettingModel { get; set; }
    }
}