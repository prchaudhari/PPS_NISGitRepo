using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public interface ISQLSystemActivityHistoryRepository
    {
        /// <summary>
        /// This method help to get system activity history records
        /// </summary>
        /// <param name="tenantCode"> the tenant code </param>
        /// <returns>list of system activity history records</returns>
        IList<SystemActivityHistory> GetSystemActivityHistories(string tenantCode);

        /// <summary>
        /// This method help to save system activity history
        /// </summary>
        /// <param name="activityHistories"> the list of activity history records </param>
        /// <param name="tenantCode"> the tenant code </param>
        /// <returns>true if save successfully, otherwise false</returns>
        bool SaveSystemActivityHistoryDetails(IList<SystemActivityHistory> activityHistories, string tenantCode);
    }
}
