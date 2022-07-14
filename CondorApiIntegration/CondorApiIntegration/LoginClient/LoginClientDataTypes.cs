using System.Collections.Generic;

namespace Lutron.CondorApiIntegration.LoginClient
{

    /// <summary>
    /// Maps the string array of AreaTypes to the Integer array based on db
    /// </summary>
    public class LoginResult
    {
        /// <summary>
        /// 
        /// </summary>
        public Dictionary<string, string> Secrets { get; }
        /// <summary>
        /// 
        /// </summary>
        public LoginResult(Dictionary<string, string> secrets)
        {
            Secrets = secrets;
        }
    }

    /// <summary>
    /// Provides data about the area to the request
    /// Now contains only name, but it can be expanded in the future
    /// </summary>
    public class LoginData
    {
    }
}
