using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public partial class View_Page
    {
        public View_Page()
        {
            PageWidgetMap = new List<View_PageWidgetMap>();
        }
        [NotMapped]
        public IList<View_PageWidgetMap> PageWidgetMap { get; set; }
    }
}
