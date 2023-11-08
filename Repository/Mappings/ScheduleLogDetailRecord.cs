using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public partial class ScheduleLogDetailRecord
    {
        [NotMapped]
        public IList<StatementMetadataRecord> StatementMetadataRecords { get; set; }
    }
}
