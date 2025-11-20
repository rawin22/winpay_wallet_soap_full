using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TSG.Models.APIModels.Common
{
    public class ApiBankDirectorySearchRequest
    {
        /// <summary>
        /// Bank code for searching
        /// </summary>
        public string BankCode { get; set; }
        /// <summary>
        /// Bank code type
        /// </summary>
        public string CodeType { get; set; }
    }
}
