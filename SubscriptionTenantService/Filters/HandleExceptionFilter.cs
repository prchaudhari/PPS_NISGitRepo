

namespace SubscriptionTenantService
{
    #region References

    using System;
    using System.Collections;
    using System.Net;
    using System.Net.Http;
    using System.Web.Http;
    using System.Web.Http.Filters;

    #endregion

    public class HandleExceptionFilter : ExceptionFilterAttribute, IExceptionFilter
    {
        /// <summary>
        /// This method will convert exception into serializable format
        /// </summary>
        /// <param name="context">The http application executed context instance</param>
        public override void OnException(HttpActionExecutedContext context)
        {
            var exception = context.Exception;
            string message = string.Empty;
            if (context.Exception.Message.Contains("~"))
            {
                message = context.Exception.Message.Split('~')[1];
            }

            if (string.IsNullOrWhiteSpace(message))
            {
                message = context.Exception.Message;
            }

            HttpError error = new HttpError(message);

            context.Response = context.Request.CreateErrorResponse(HttpStatusCode.BadRequest, error);
            base.OnException(context);
        }
    }
}