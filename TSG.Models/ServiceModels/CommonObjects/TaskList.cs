using System;
using System.Collections.Generic;

namespace TSG.Models.ServiceModels
{
    public class TaskList
    {
        public int TaskList_ID { get; set; }
        public string TaskList_Code { get; set; }
        public int TaskList_MailId { get; set; }
        public int? TaskList_Status { get; set; }
        public DateTime? TaskList_LastDateEdit { get; set; }
    }
}
