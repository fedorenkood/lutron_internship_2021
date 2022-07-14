using System;
using System.Collections.Generic;
using System.Text;

namespace Lutron.CondorApiIntegration.AreaTypePredictionClient
{
    /// <summary>
    /// 
    /// </summary>
    public enum ResponseResult
    {
        /// <summary>
        /// Code 200 and proper response
        /// </summary>
        Success,
        /// <summary>
        /// Code 400
        /// </summary>
        BadRequest,
        /// <summary>
        /// Code 401
        /// </summary>
        Unauthorized,
        /// <summary>
        /// Code 404
        /// </summary>
        NotFound,
        /// <summary>
        /// Code 500
        /// </summary>
        ServiceUnavailable,
        /// <summary>
        /// Code 503
        /// </summary>
        ServerError, 
        /// <summary>
        /// Bad response
        /// </summary>
        MalformedResponse,
        /// <summary>
        /// Any other Error
        /// </summary>
        Fail

    }
}