using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class Merchants
    {
        public int Merchants_ID { get; set; }
        public Guid Merchants_UniqueID { get; set; }
        public string Merchants_Name { get; set; }
        public string Merchants_Address { get; set; }
        public string Merchants_Phone { get; set; }
        public string Merchants_UserId { get; set; }
        public string Merchants_UserGuid { get; set; }
        public DateTime? Merchants_CreationDate { get; set; }
        public string Merchants_CallBackAddress { get; set; }
        public bool? Merchants_IsSandBox { get; set; }
        public string Merchants_PublicTokenKey { get; set; }
        public string Merchants_PrivateToken { get; set; }
        public string Merchants_MerchantAlias { get; set; }
    }
}
