using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class PageWidget
    {
        public long Identifier { get; set; }
        public long WidgetId { get; set; }
        public string WidgetName { get; set; }
        public long PageId { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }
        public int Xposition { get; set; }
        public int Yposition { get; set; }
        public string WidgetSetting { get; set; }
        public string TenantCode { get; set; }
        public bool IsDynamicWidget { get; set; }
    }
}
