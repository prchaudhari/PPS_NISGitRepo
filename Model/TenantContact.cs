using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace nIS
{
    public class TenantContact
    {
        public long Identifier { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public string ContactType { get; set; }
        public string EmailAddress { get; set; }
        public string Image { get; set; }
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsActivationLinkSent { get; set; }
        public string TenantCode { get; set; }
        public long CountryId { get; set; }

        /// <summary>
        /// The validation engine objecct
        /// </summary>
        private IValidationEngine validationEngine = new ValidationEngine();

        /// <summary>
        /// The utility object
        /// </summary>
        private IUtility utility = new Utility();

        public void IsValid()
        {
            try
            {
                Exception exception = new Exception();
                if (!this.validationEngine.IsValidText(this.FirstName))
                {
                    exception.Data.Add(this.utility.GetDescription("FirstName", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_NAME);
                }

                if (!this.validationEngine.IsValidText(this.LastName))
                {
                    exception.Data.Add(this.utility.GetDescription("LastName", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_NAME);
                }

                if (!this.validationEngine.IsValidText(this.EmailAddress))
                {
                    exception.Data.Add(this.utility.GetDescription("EmailAddress", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_EMAIL);
                }

                if (!this.validationEngine.IsValidText(this.ContactNumber))
                {
                    exception.Data.Add(this.utility.GetDescription("ContactNumber", typeof(User)), ModelConstant.USER_MODEL_SECTION + "~" + ModelConstant.INVALID_USER_CONTACT_NUMBER);
                }

                if (exception.Data.Count > 0)
                {
                    throw exception;
                }
            }
            catch (Exception exception)
            {
                throw exception;
            }
        }
    }
}
