using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper.Contrib.Extensions;

namespace TSG.Models.DTO
{
    public class AddFunds_BlockChainInfoDto
    {
        [System.ComponentModel.DataAnnotations.Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [MaxLength(500)]
        public string Alias { get; set; } // User message for fundings

        [MaxLength(500)]
        public string Message { get; set; } // User message for fundings

        [MaxLength(500)]
        public string BlockChainAddress { get; set; } // After 'Recieve' method response - Generated Address

        [MaxLength(500)]
        public string CallBackAddress { get; set; } // CallbackAddress for send on BlockChain.info API

        public int Index { get; set; } // After 'Recieve' method response - Generated Index

        public Guid TimeStampPayment { get; set; } // Unique field for fix funding

        public Guid ParentId { get; set; } // link to parent funds

        public decimal TotalValue { get; set; }

        [MaxLength(100)]
        public string TransactionHash { get; set; }

        [MaxLength(100)]
        public string DestinatedBitcoinAddress { get; set; }

        public int? NumberOfConfirmation { get; set; }

        public long? ValueInSatoshi { get; set; }

        
        [MaxLength(500)]
        public string CustomParameter { get; set; }

        public int? ConfirmatedTransaction { get; set; }
        
        public int? TransactionId { get; set; }

        [Write(false)]
        [MaxLength(50)]
        public string RequestUrl { get; set; } 
        

        [Write(false)]
        public string CurrencyCode { get; set; }

        [Write(false)]
        public int CurrencyIndex { get; set; }

        [Write(false)]
        public string Operation { get; set; }
    }
}
