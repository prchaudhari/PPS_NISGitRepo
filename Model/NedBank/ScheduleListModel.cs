using System.Collections.Generic;

namespace nIS.NedBank
{
    public class ScheduleListModel
    {
        public ScheduleListModel()
        {
            ProductBatches = new List<ScheduleModel>();
        }
        public string ProductName { get; set; }
        public int ProductId { get; set; }
        public string ProductBatchName { get; set; }
        public string ScheduleNameByUser { get; set; }
        public bool IsDataReady { get; set; }
        public IList<ScheduleModel> ProductBatches { get; set; }
    }
}
