using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using AmadeusAI;
namespace AmadeusAI
{
  public class Logger
   {
   }
}

/*
public class Logger
{
    private static Logger instance;
    private static Queue<LogMessage> logQueue;
    private static string logDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Logs");
    private static string logFile = Path.Combine(logDir, "log.txt");
    private static FileStream logStream;

    // Ensures only one instance is created
    public static Logger Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new Logger();
            }
            return instance;
        }
    }

    private Logger()
    {
        // Create directory if it does not exist
        if (!Directory.Exists(logDir))
        {
            Directory.CreateDirectory(logDir);
        }

        logStream = File.Open(logFile, FileMode.Append);
        logQueue = new Queue<LogMessage>();
    }

    public void Log(string message)
    {
        lock (logQueue)
        {
            logQueue.Enqueue(new LogMessage(message));
            if (logQueue.Count >= 10 || message == "EXIT")
            {
                DumpLog();
            }
        }
    }

    public void DumpLog()
    {
        while (logQueue.Count > 0)
        {
            LogMessage entry = logQueue.Dequeue();
            string logEntry = string.Format("{0} {1}", entry.LogTime.ToString(), entry.Message);
            byte[] bytes = Encoding.ASCII.GetBytes(logEntry);
            logStream.Write(bytes, 0, bytes.Length);
        }
    }
}

public class LogMessage
{
    public string Message { get; set; }
    public DateTime LogTime { get; set; }

    public LogMessage(string message)
    {
        Message = message;
        LogTime = DateTime.Now;
    }
}
*/
