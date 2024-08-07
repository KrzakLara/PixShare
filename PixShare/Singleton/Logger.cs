using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using PixShareLIB.Models; // Dodajte odgovarajući using za User

namespace PixShare.Singleton
{
    public class Logger
    {
        private static Logger instance;
        private static readonly object lockObject = new object();
        private string logFilePath = "C:\\Users\\larak\\Desktop\\NRAKO\\PixShare\\PixShare\\Log";

        private Logger()
        {
           
        }

        public static Logger Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (lockObject)
                    {
                        if (instance == null)
                        {
                            instance = new Logger();
                        }
                    }
                }
                return instance;
            }
        }

        public void Log(User user, string what)
        {
            string logMessage = $"{DateTime.Now} - {user.Username}: {what}";

            lock (lockObject)
            {
                using (StreamWriter sw = File.AppendText(logFilePath))
                {
                    sw.WriteLine(logMessage);
                }
            }
        }
    }
}
