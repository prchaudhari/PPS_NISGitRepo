using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class MultiTenantUserRoleAccess
    {
        public long Identifier { get; set; }
        public long UserId { get; set; }
        public string UserName { get; set; }
        public string EmailAddress { get; set; }
        public long AssociatedTenantId { get; set; }
        public string AssociatedTenantCode { get; set; }
        public string AssociatedTenantName { get; set; }
        public string AssociatedTenantType { get; set; }
        public long OtherTenantId { get; set; }
        public string OtherTenantCode { get; set; }
        public string OtherTenantName { get; set; }
        public string OtherTenantType { get; set; }
        public long RoleId { get; set; }
        public string RoleName { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public long LastUpdatedBy { get; set; }
        public string LastUpdatedByUserName { get; set; }
        public DateTime LastUpdatedDate { get; set; }

        #region

        public void IsValid()
        {
            Exception exception = new Exception();
            IUtility utility = new Utility();
            ValidationEngine validationEngine = new ValidationEngine();
            if (exception.Data.Count > 0)
            {
                throw exception;
            }
        }
        #endregion
    }
}
