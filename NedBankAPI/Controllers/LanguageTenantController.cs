namespace NedbankAPI.Controllers
{
    #region References

    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using Unity;
    using NedBankManager;
    using NedbankModel;

    #endregion
    /// <summary>
    /// The Language Controller.
    /// </summary>
    /// <seealso cref="System.Web.Http.ApiController" />
    [RoutePrefix("LanguageTenant")]
    public class LanguageTenantController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The language manager
        /// </summary>
        private LanguageTenantManager languageTenantManager = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private readonly IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="LanguageController" /> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public LanguageTenantController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.languageTenantManager = new LanguageTenantManager(this.unityContainer);
        }

        #endregion

        #region List
        /// <summary>
        /// Lists the specified user search parameter.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("List")]
        public IList<LanguageTenant> List()
        {
            IList<LanguageTenant> languages = new List<LanguageTenant>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                languages = this.languageTenantManager.GetAllLanguages(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return languages;
        }

        #endregion

        #region Add
        /// <summary>
        /// Adds the specified languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Add")]
        public bool Add(IList<LanguageTenant> languages)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.languageTenantManager.AddLanguages(languages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Update
        /// <summary>
        /// Updates the specified languages.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Update")]
        public bool Update(LanguageTenant language)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.languageTenantManager.UpdateLanguages(language, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion

        #region Delete
        /// <summary>
        /// Deletes the specified languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <returns></returns>
        [HttpPost]
        [Route("Delete")]
        public bool Delete(IList<LanguageTenant> languages)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.languageTenantManager.DeleteLanguages(languages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        #endregion
    }
}