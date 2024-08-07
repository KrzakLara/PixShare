using System;
using System.IO;

namespace PixShareLIB.Observer
{
    public class Logger : IObserver
    {
        private readonly string logFilePath;

        public Logger()
        {
            string logDirectory = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Log");
            logFilePath = Path.Combine(logDirectory, "log.txt");

            if (!Directory.Exists(logDirectory))
            {
                Directory.CreateDirectory(logDirectory);
            }

            if (!File.Exists(logFilePath))
            {
                using (File.Create(logFilePath)) { }
            }
        }

        public void Update(string action, string user)
        {
            LogAction(action, user);
        }

        private void LogAction(string action, string user)
        {
            string logEntry = $"{DateTime.Now}: {user} performed {action}";
            Console.WriteLine(logEntry);

            try
            {
                using (StreamWriter writer = new StreamWriter(logFilePath, true))
                {
                    writer.WriteLine(logEntry);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to log action: {ex.Message}");
            }
        }
    }
}
