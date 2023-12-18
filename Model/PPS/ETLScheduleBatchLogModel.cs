using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS.PPS
{
    public class ETLScheduleBatchLogModel
    {
        public long Identifier { get; set; }
        public string ETLSchedule { get; set; }
        public string Batch { get; set; }
        public string ProcessingTime { get; set; }
        public string Status { get; set; }
        public Nullable<System.DateTime> ExecutionDate { get; set; }
        public long EtlScheduleId { get; set; }
        public string LogMessage { get; set; }
    }
}
