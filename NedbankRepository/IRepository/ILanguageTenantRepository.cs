namespace NedbankRepository
{
    #region References

    using NedbankModel;
    using System.Collections.Generic;
    #endregion
    /// <summary>
    /// The language repository
    /// </summary>
    public interface ILanguageTenantRepository
    {
        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        IList<LanguageTenant> GetAllLanguages(string tenantCode);

        /// <summary>
        /// Adds the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool AddLanguages(IList<LanguageTenant> languages, string tenantCode);

        /// <summary>
        /// Updates the languages.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool UpdateLanguages(LanguageTenant language, string tenantCode);

        /// <summary>
        /// Deletes the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool DeleteLanguages(IList<LanguageTenant> languages, string tenantCode);

    }
}
