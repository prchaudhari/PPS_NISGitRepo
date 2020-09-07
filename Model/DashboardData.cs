using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class DashboardData
    {
        public string LastSCheduleRunDate { get; set; }
        public int GeneratedStatementsOfLastscheduleRun { get; set; }
        public int ActiveExceptionsOfLastscheduleRun { get; set; }
        public int TotalGeneratedStatements { get; set; }

    }
}
