using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RestSharp;

namespace com.ketra.APITools.KetraCalibration
{
    public class KetraCalibrationCondorChecksBase : IDisposable
    {
        private static readonly string LOG_DIRECTORY = @"C:\CondorLogs";
        protected static readonly int MAX_ATTEMPTS = 3;
        private StreamWriter m_logger;

        public string SerialNumber { get; }
        public string SessionID { get; }
        public Endpoint Endpoint { get; }

        public KetraCalibrationCondorChecksBase(
            string logFile,
            string serialNumber,
            string sessionID,
            Endpoint endpoint)
        {
            SerialNumber = serialNumber;
            SessionID = sessionID;
            Endpoint = endpoint;
            if (!Directory.Exists(LOG_DIRECTORY))
            {
                Directory.CreateDirectory(LOG_DIRECTORY);
            }
            var filePath = Path.Combine(LOG_DIRECTORY, logFile);

            try
            {
                if (!File.Exists(filePath))
                {
                    m_logger = new StreamWriter(filePath);
                    m_logger.WriteLine(String.Join(",", "Serial Number", "Session ID", "Log Type", "Attempt", "Time", "Status"));
                }
                else
                {
                    m_logger = new StreamWriter(filePath, true);
                }
            }
            catch
            {
                //
                // If for some reason someone has opened the log file on the machine,
                // ensure that we still log the data to a backup file.
                //
                m_logger = GetBackupLogger(filePath, 1);
            }
        }

        #region Private/Protected Methods
        protected async Task<IRestResponse> ExecuteRestRequestAsync(
            RestSharp.Method method,
            Dictionary<string, string> header,
            Tuple<string, object, ParameterType>[] parameters,
            string resource)
        {
            int attempt = 1;
            ApiClientHelper apiHelper = new ApiClientHelper(Endpoint.LutronDSProduction);
            while (attempt <= MAX_ATTEMPTS)
            {
                CancellationToken token = new CancellationToken();
                IRestResponse response = await apiHelper.ExecuteRestRequestAsync(
                                            method,
                                            header,
                                            parameters,
                                            resource,
                                            token);

                if (response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    LogRequestFailure(response, attempt);
                    attempt++;
                }
                else
                {
                    LogRequestSuccess(attempt);
                    return response;
                }
            }

            return null;
        }

        private void LogRequestFailure(
            IRestResponse response,
            int attempt)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("Rest Request Failure. ");
            sb.Append($"Status Code: {response.StatusCode}; ");
            if (!String.IsNullOrEmpty(response.ErrorMessage))
            {
                sb.Append($"Error Message: {response.ErrorMessage}");
            }
            else
            {
                sb.Append($"Error Message: None");
            }

            if (response.ErrorException != null)
            {
                sb.Append($"; Error Exception Message: {response.ErrorException.Message}");
            }

            this.Log(sb.ToString(), "Request", attempt);
        }
        
        private void LogRequestSuccess(
            int attempt)
        {
            this.Log("Success", "Request",  attempt);
        }

        protected void Log(
            string message,
            string logType,
            int attempt)
        {
            if (m_logger == null)
            {
                return; 
            }

            if (message.Contains(","))
            {
                message = $"\"{message}\"";
            }
            m_logger.WriteLine(
                String.Join(
                    ",",
                    SerialNumber,
                    SessionID,
                    logType,
                    attempt.ToString(),
                    DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    message.Replace("\r\n", ";").Replace("\r", ";").Replace("\n", ";")));
        }

        private StreamWriter GetBackupLogger(
            string baseFilePath,
            int attempt)
        {
            string backupFilePath = "";
            if (attempt > 3)
            {
                throw new Exception("Could not get logger.");
            }

            try
            {
                backupFilePath = baseFilePath.Insert(baseFilePath.IndexOf(".csv"), $"_{attempt}");
                m_logger = new StreamWriter(backupFilePath, true);
                return m_logger;
            }
            catch
            {
                return GetBackupLogger(baseFilePath, ++attempt);
            }
        }

        #endregion Private/Protected Methods

        #region IDisposable Implementation
        public void Dispose()
        {
            if (m_logger != null)
            {
                m_logger.Flush();
                m_logger.Close();
                m_logger.Dispose();
                m_logger = null;
            }
        }
        #endregion IDisposable Implementation
    }
}
