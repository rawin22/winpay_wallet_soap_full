using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class SharedPhoto
    {
        public int SharedPhoto_ID { get; set; }
        public string SharedPhoto_PaymentId { get; set; }
        public DateTime? SharedPhoto_CreationDate { get; set; }
        public DateTime? SharedPhoto_ExpiredDate { get; set; }
        public bool SharedPhoto_IsViewed { get; set; }
        public string SharedPhoto_SharedPhotoId { get; set; }
        public string SharedPhoto_Comment { get; set; }
    }
}
