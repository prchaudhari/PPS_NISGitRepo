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
    /// <seealso cref="NedbankRepository.ILanguageRepository" />
    public class SQLLanguageRepository : ILanguageRepository
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
        public SQLLanguageRepository(IUnityContainer unityContainer)
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
        public IList<Language> GetAllLanguages(string tenantCode)
        {
            IList<Language> languages = new List<Language>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    languages = nedbankEntities.LanguageMasters.Where(a => a.IsDeleted == false).Select(language => new Language()
                    {
                        Id = language.Id,
                        Code = language.Code,
                        Description = language.Description,
                        IsDeleted = language.IsDeleted,
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
        public bool AddLanguages(IList<Language> languages, string tenantCode)
        {
            IList<LanguageMaster> languageRecords = new List<LanguageMaster>();
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                languages.ToList().ForEach(l =>
                {
                    languageRecords.Add(new LanguageMaster()
                    {
                        Code = l.Code,
                        Description = l.Description,
                        IsDeleted = true,
                    });
                });

                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    nedbankEntities.LanguageMasters.AddRange(languageRecords);
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
        public bool UpdateLanguages(Language language, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    IList<LanguageMaster> languageRecords = nedbankEntities.LanguageMasters.Where(a => a.Id == language.Id).Select(item => item).AsQueryable().ToList();

                    if (languageRecords == null || languageRecords.Count <= 0 || languageRecords.Count() != string.Join(",", languageRecords.Select(item => item.Id).Distinct()).ToString().Split(',').Length)
                    {
                        throw new LanguageNotFoundException(tenantCode);
                    }

                    LanguageMaster languageRecord = nedbankEntities.LanguageMasters.FirstOrDefault(a => a.Id == language.Id);
                    if (languageRecord != null)
                    {
                        languageRecord.Code = language.Code;
                        languageRecord.Description = language.Description;
                        languageRecord.IsDeleted = language.IsDeleted;
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
        public bool DeleteLanguages(IList<Language> languages, string tenantCode)
        {
            try
            {
                this.SetAndValidateConnectionString(tenantCode);
                using (NedbankEntities nedbankEntities = new NedbankEntities(this.connectionString))
                {
                    StringBuilder query = new StringBuilder();
                    query.Append("(" + string.Join("or ", string.Join(",", languages.Select(item => item.Id).Distinct()).ToString().Split(',').Select(item => string.Format("Id.Equals({0}) ", item))) + ") ");
                    query.Append("and IsDeleted.Equals(false)");

                    IList<LanguageMaster> languageRecords = nedbankEntities.LanguageMasters.Where(query.ToString()).Select(item => item).AsQueryable().ToList();

                    languageRecords.ToList().ForEach(l =>
                    {
                        l.IsDeleted = true;
                    });

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
