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
    [RoutePrefix("Language")]
    public class LanguageController : ApiController
    {
        #region Private Members

        /// <summary>
        /// The language manager
        /// </summary>
        private LanguageManager languageManager = null;

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
        public LanguageController(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.languageManager = new LanguageManager(this.unityContainer);
        }

        #endregion

        #region List
        /// <summary>
        /// Lists the specified user search parameter.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("List")]
        public IList<Language> List()
        {
            IList<Language> languages = new List<Language>();
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                languages = this.languageManager.GetAllLanguages(tenantCode);
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
        public bool Add(IList<Language> languages)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.languageManager.AddLanguages(languages, tenantCode);
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
        public bool Update(Language language)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.languageManager.UpdateLanguages(language, tenantCode);
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
        public bool Delete(IList<Language> languages)
        {
            bool result = false;
            try
            {
                string tenantCode = Helper.CheckTenantCode(Request.Headers);
                result = this.languageManager.DeleteLanguages(languages, tenantCode);
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