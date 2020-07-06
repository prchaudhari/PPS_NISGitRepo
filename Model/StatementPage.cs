using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class StatementPage
    {
        public long Identifier { get; set; }
        public long ReferencePageId { get; set; }
        public long StatementId { get; set; }
        public long SequenceNumber { get; set; }
        public string TenantCode { get; set; }

        public string PageName { get; set; }
    }
}
