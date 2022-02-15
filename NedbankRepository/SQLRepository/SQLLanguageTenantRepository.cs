namespace NedbankRepository
{
    #region References
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Dynamic;
    using System.Text;
    using Unity;
    using NedBankException;
    using NedbankModel;
    using NedBankValidationEngine;
    using NedbankUtility;
    #endregion

    /// <summary>
    /// This class represents the methods to perform operation with database for role entity.
    /// </summary>
    /// <seealso cref="NedbankRepository.ILanguageTenantRepository" />
    /// <seealso cref="NedbankRepository.ILanguageRepository" />
    public class SQLLanguageTenantRepository : ILanguageTenantRepository
    {
        #region Private Members

        /// <summary>
        /// The validation engine object
        /// </summary>
        INedBankValidationEngine validationEngine = null;

        /// <summary>
        /// The connection string
        /// </summary>
        private string connectionString = string.Empty;

        /// <summary>
        /// The unity container
        /// </summary>
        private IUnityContainer unityContainer = null;

        /// <summary>
        /// The utility object
        /// </summary>
        private IConfigurationUtility configurationutility = null;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLLanguageRepository" /> class.
        /// </summary>
        /// <param name="unityContainer">The unity container.</param>
        public SQLLanguageTenantRepository(IUnityContainer unityContainer)
        {
            this.unityContainer = unityContainer;
            this.validationEngine = new NedBankValidationEngine();
            this.configurationutility = new ConfigurationUtility(this.unityContainer);
        }

        #endregion

        #region Public Methods

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
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    languages = nedbankEntities.LanguageTenantMapping.Select(language => new LanguageTenant()
                    {
                        Id = language.Id,
                        LanguageCode = language.LanguageCode,
                        TenantCode = language.TenantCode,
                    }).ToList();
                }
            }

            catch (Exception ex)
            {
                throw ex;
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
            IList<LanguageTenantMapping> languageRecords = new List<LanguageTenantMapping>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    IList<LanguageTenantMapping> existingLanguages = nedbankEntities.LanguageTenantMapping.Select(item => item).ToList();

                    existingLanguages.ToList().ForEach(item =>
                    {
                        if (languages.Any(a => a.LanguageCode == item.LanguageCode && a.TenantCode == item.TenantCode))
                        {
                            throw new LanguageAlreadyExistsException(tenantCode);
                        }
                    });

                    languages.ToList().ForEach(l =>
                    {
                        languageRecords.Add(new LanguageTenantMapping()
                        {
                            LanguageCode = l.LanguageCode,
                            TenantCode = l.TenantCode,
                        });
                    });

                    nedbankEntities.LanguageTenantMapping.AddRange(languageRecords);
                    nedbankEntities.SaveChanges();
                }
                return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        /// <summary>
        /// Updates the languages.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        /// <exception cref="NedBankException.LanguageNotFoundException"></exception>
        /// <exception cref="NedBankException.LanguageAlreadyExistsException"></exception>
        public bool UpdateLanguages(LanguageTenant language, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    IList<LanguageTenantMapping> languageRecords = nedbankEntities.LanguageTenantMapping.Select(item => item).ToList();

                    IList<LanguageTenantMapping> languages = languageRecords.Where(a => a.Id == language.Id).ToList();
                    IList<LanguageTenantMapping> existingLanguages = languageRecords.Where(a => a.LanguageCode.Equals(language.LanguageCode) && a.Id != language.Id && a.TenantCode == language.TenantCode).ToList();
                    if (languages == null || languages.Count <= 0)
                    {
                        throw new LanguageNotFoundException(tenantCode);
                    }

                    if (existingLanguages.Any())
                    {
                        throw new LanguageAlreadyExistsException(tenantCode);
                    }

                    LanguageTenantMapping languageRecord = nedbankEntities.LanguageTenantMapping.FirstOrDefault(a => a.Id == language.Id);
                    if (languageRecord != null)
                    {
                        languageRecord.LanguageCode = language.LanguageCode;
                        languageRecord.TenantCode = language.TenantCode;
                    }

                    nedbankEntities.SaveChanges();
                }

                return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }

        /// <summary>
        /// Deletes the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        public bool DeleteLanguages(IList<LanguageTenant> languages, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", languages.Select(item => item.Id).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    IList<LanguageTenantMapping> languageRecords = nedbankEntities.LanguageTenantMapping.Where(query.ToString()).Select(item => item).ToList();

                    nedbankEntities.LanguageTenantMapping.RemoveRange(languageRecords);
                    nedbankEntities.SaveChanges();
                }

                return true;
            }
            catch (Exception exception)
            {
                throw exception;
            }

        }
        #endregion

        /// <summary>
        /// Sets the and validate connection string.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <exception cref="NedBankException.ConnectionStringNotFoundException"></exception>
        private void SetAndValidateConnectionString(string tenantCode)
        {
            try
            {
                this.connectionString = validationEngine.IsValidText(this.connectionString) ? this.connectionString : this.configurationutility.GetConnectionString(ModelConstant.COMMON_SECTION, ModelConstant.NIS_CONNECTION_STRING, ModelConstant.CONFIGURATON_BASE_URL, ModelConstant.TENANT_CODE_KEY, tenantCode);
                if (!this.validationEngine.IsValidText(this.connectionString))
                {
                    throw new ConnectionStringNotFoundException(tenantCode);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}
