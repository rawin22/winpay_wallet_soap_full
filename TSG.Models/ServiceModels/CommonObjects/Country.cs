using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class Country
    {
        public int Country_ID { get; set; }
        public string Country_ISO { get; set; }
        public string Country_Name { get; set; }
        public string Country_Fullname { get; set; }
        public string Country_ISO3 { get; set; }
        public short? Country_numcode { get; set; }
        public int Country_phonecode { get; set; }

        // dbo.Bank.bankId -> dbo.Country.ID (FK_Bank_Country)
        public virtual Bank Bank { get; set; }
    }
}
