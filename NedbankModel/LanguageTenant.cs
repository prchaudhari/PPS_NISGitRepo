/// <summary>
/// The Language Model.
/// </summary>
namespace NedbankModel
{
    /// <summary>
    /// The language model
    /// </summary>
    public class LanguageTenant
    {
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>
        /// The identifier.
        /// </value>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the language code.
        /// </summary>
        /// <value>
        /// The language code.
        /// </value>
        public string LanguageCode { get; set; }
        /// <summary>
        /// Gets or sets the tenant code.
        /// </summary>
        /// <value>
        /// The tenant code.
        /// </value>
        public string TenantCode { get; set; }
    }
}