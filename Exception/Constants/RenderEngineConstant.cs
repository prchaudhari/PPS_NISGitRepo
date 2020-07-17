// <copyright file="ExceptionConstant.cs" company="Websym Solutions Pvt. Ltd.">
//  Copyright (c) 2018 Websym Solutions Pvt. Ltd. 
// </copyright>

namespace nIS
{
    #region References
    #endregion

    /// <summary>
    /// This class represents constants for render engine entity.
    /// </summary>
    public partial class ExceptionConstant
    {
        /// <summary>
        /// The render engine exception section key
        /// </summary>
        public const string RENDERENGINE_EXCEPTION_SECTION = "RenderEngineException";

        /// <summary>
        /// The duplicate render engine found exception key
        /// </summary>
        //public const string DUPLICATE_RENDERENGINE_FOUND_EXCEPTION = "DuplicateRenderEngineFoundException";
        public const string DUPLICATE_RENDERENGINE_FOUND_EXCEPTION = "Duplicate render engine found";

        /// <summary>
        /// The render engine not found exception key
        /// </summary>
        //public const string RENDERENGINE_NOT_FOUND_EXCEPTION = "RenderEngineNotFoundException";
        public const string RENDERENGINE_NOT_FOUND_EXCEPTION = "RenderEngine not found exception";


        /// <summary>
        /// The invalid render engine exception key
        /// </summary>
        //public const string INVALID_RENDERENGINE_EXCEPTION = "InvalidRenderEngineLibraryException";
        public const string INVALID_RENDERENGINE_EXCEPTION = "Invalid render engine";
    }
}
