using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NIS.Repository.Entities
{
    [Table("NIS.View_ETLScheduleLog")]
    public partial class View_ETLScheduleLog
    {
        [Key, Column(Order = 0)]
        public long Id { get; set; }
        public string ETLSchedule { get; set; }
        public string BatchName { get; set; }
        public string ProcessingTime { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> ExecutionDate { get; set; }
        public string TenantCode { get; set; }
        public long ETLBatchId { get; set; }
        public long EtlScheduleId { get; set; }
        public string LogMessage { get; set; }
    }

}
