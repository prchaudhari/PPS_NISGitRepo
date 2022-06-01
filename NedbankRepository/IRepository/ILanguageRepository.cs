namespace NedbankRepository
{
    #region References

    using NedbankModel;
    using System.Collections.Generic;
    #endregion
    /// <summary>
    /// The language repository
    /// </summary>
    public interface ILanguageRepository
    {
        /// <summary>
        /// Gets all languages.
        /// </summary>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        IList<Language> GetAllLanguages(string tenantCode);

        /// <summary>
        /// Adds the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool AddLanguages(IList<Language> languages, string tenantCode);

        /// <summary>
        /// Updates the languages.
        /// </summary>
        /// <param name="language">The language.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool UpdateLanguages(Language language, string tenantCode);

        /// <summary>
        /// Deletes the languages.
        /// </summary>
        /// <param name="languages">The languages.</param>
        /// <param name="tenantCode">The tenant code.</param>
        /// <returns></returns>
        bool DeleteLanguages(IList<Language> languages, string tenantCode);

    }
}
