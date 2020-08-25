using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public  class PageWidgetVistorData
    {
        public IList<long> values = new List<long>();

        public IList<string> widgetNames = new List<string>();
    }

    public class VisitorForDay
    {
        public IList<long> values = new List<long>();

        public IList<string> widgetNames = new List<string>();
    }
}
