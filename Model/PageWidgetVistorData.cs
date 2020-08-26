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

    public class DatewiseVisitorSeries
    {
        public string name;
        public IList<long> data = new List<long>();
    }
    public class DatewiseVisitor
    {
        public IList<string> dates = new List<string>();

        public IList<DatewiseVisitorSeries> datewiseVisitorSeries=new List<DatewiseVisitorSeries>();

    }
}
