using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS.PPS
{
    public class ETLScheduleListModel
    {
        public ETLScheduleListModel()
        {
            ProductBatches = new List<ETLScheduleModel>();
        }

        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public IList<ETLScheduleModel> ProductBatches { get; set; }
    }
}
