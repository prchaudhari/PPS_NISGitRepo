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
    public class LanguageManager
    {
        #region Private Members

        /// <summary>
        /// The language repository
        /// </summary>
        private ILanguageRepository languageRepository = null;

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
        public LanguageManager(IUnityContainer unityContainer)
        {
            try
            {
                this.unityContainer = unityContainer;
                this.languageRepository = this.unityContainer.Resolve<ILanguageRepository>();
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
        public IList<Language> GetAllLanguages(string tenantCode)
        {
            IList<Language> languages = new List<Language>();
            try
            {
                languages = this.languageRepository.GetAllLanguages(tenantCode);
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
        public bool AddLanguages(IList<Language> languages, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.languageRepository.AddLanguages(languages, tenantCode);
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
        public bool UpdateLanguages(Language language, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.languageRepository.UpdateLanguages(language, tenantCode);
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
        public bool DeleteLanguages(IList<Language> languages, string tenantCode)
        {
            bool result = false;
            try
            {
                result = this.languageRepository.DeleteLanguages(languages, tenantCode);
            }
            catch (Exception exception)
            {
                throw exception;
            }

            return result;
        }
    }
}
