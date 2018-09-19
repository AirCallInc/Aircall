using System.Net;

namespace api.ErrorHelper
{
    public interface IApiExceptions
    {
        /// <summary>
        /// ErrorCode
        /// </summary>
        int ErrorCode { get; set; }
        /// <summary>
        /// ErrorDescription
        /// </summary>
        string ErrorDescription { get; set; }
        /// <summary>
        /// HttpStatus
        /// </summary>
        HttpStatusCode HttpStatus { get; set; }
        /// <summary>
        /// ReasonPhrase
        /// </summary>
        string ReasonPhrase { get; set; }
    }
}