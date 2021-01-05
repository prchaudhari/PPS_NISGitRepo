using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class StatementMetadata
    {
        public long Identifier { get; set; }
        public long ScheduleId { get; set; }
        public long ScheduleLogId { get; set; }
        public long StatementId { get; set; }
        public DateTime? StatementDate { get; set; }
        public string StatementPeriod { get; set; }
        public long CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string AccountNumber { get; set; }
        public string AccountType { get; set; }
        public string StatementURL { get; set; }
        public string TenantCode { get; set; }
        public bool IsPasswordGenerated { get; set; }
        public string Password { get; set; }
    }
}
