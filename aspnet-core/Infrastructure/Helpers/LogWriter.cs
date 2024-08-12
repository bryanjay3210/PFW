using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Helpers
{
    public sealed class LogWriter
    {
        private string m_exePath = string.Empty;
        private readonly IConfiguration _configuration;
        private readonly string _fileNamePath;
        string _filePath;
        string _fileName;
        public LogWriter(IConfiguration configuration)
        {
            _configuration = configuration;
            _filePath = configuration.GetSection("AppSettings:BellFlowerFilePath").Value;
            _fileName = configuration.GetSection("AppSettings:BellFlowerFileName").Value;
            _fileNamePath = $"{_filePath}{string.Format(_fileName, DateTime.Now.ToString("yyyyMMdd"))}";
        }

        public void LogWrite(string logMessage)
        {

            m_exePath = _fileNamePath;
            try
            {
                using (StreamWriter w = File.AppendText($"{m_exePath}_log.txt"))
                {
                    Log(logMessage, w);
                }
            }
            catch (Exception ex)
            {
            }
        }

        public void Log(string logMessage, TextWriter txtWriter)
        {
            try
            {
                txtWriter.WriteLine($"Log Entry : {DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()} -> {logMessage}");
            }
            catch (Exception ex)
            {
            }
        }
    }
}
