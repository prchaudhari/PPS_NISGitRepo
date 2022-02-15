namespace NedBankManager
{
    #region References

    using System;
    using System.Collections.Generic;
    using Unity;
    using NedbankModel;
    using NedbankRepository;

    #endregion
    /// <summary>
    /// The Language Manager
    /// </summary>
    public class LanguageTenantManager
    {
        #region Private Members

        /// <summary>
        /// The language repository
        /// </summary>
        private ILanguageTenantRepository languageTenantRepository = null;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        #endregion

        #region Constructor

        /// <summary>
        /// This is constructor for role manager, which initialise
        /// role repository.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public LanguageTenantManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.languageTenantRepository = this.unityContainer.Resolve<ILanguageTenantRepository>();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion
        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public IList<LanguageTenant> GetAllLanguages(string tenantCode)
        {
            IList<LanguageTenant> languages = new List<LanguageTenant>();
            try
            {
                languages = this.languageTenantRepository.GetAllLanguages(tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return languages;
        }

        /// <summary>
        /// Adds the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public bool AddLanguages(IList<LanguageTenant> languages, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.languageTenantRepository.AddLanguages(languages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Updates the languages.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public bool UpdateLanguages(LanguageTenant language, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.languageTenantRepository.UpdateLanguages(language, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }

        /// <summary>
        /// Deletes the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public bool DeleteLanguages(IList<LanguageTenant> languages, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.languageTenantRepository.DeleteLanguages(languages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }
    }
}
