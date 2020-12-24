using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class CustomerMedia
    {
        public long Identifier { get; set; }
        public long CustomerId { get; set; }
        public long BatchId { get; set; }
        public long StatementId { get; set; }
        public long PageId { get; set; }
        public long WidgetId { get; set; }
        public string ImageURL { get; set; }
        public string VideoURL { get; set; }
        public string TenantCode { get; set; }
    }
}
