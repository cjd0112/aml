using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using AS.Logger;
using Newtonsoft.Json;

namespace Logger
{
    public enum AsLogInfo
    {
        Error,
        Info
    };

    public class AsLogEntry 
    {
        public AsLogEntry()
        {
            Type = AsLogInfo.Info;
            Time = DateTime.Now;
            Module = "none";
            Message = "test message";

        }
        public AsLogEntry(AsLogInfo type, DateTime time, string module, string message)
        {
            Type = type;
            Time = time;
            Module = module;
            Message = message;
        }

        public AsLogInfo Type { get; set; }
        public DateTime Time { get; set; }
        public String Module { get; set; }
        public String Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }

    }

    public interface IExternalLogger
    {
        void Log(AsLogEntry entry);
    }


    public static class L
    {

        private static String ApplicationName = "";
        private static TextWriter Writer = null;

        private static bool ExceptionOnly = false;

        public static void SetExceptionOnly(bool onOrOff)
        {
            ExceptionOnly = onOrOff;

        }

        private static String Path;


        private static String traceFilePath;
        private static Timer RollLogTimer = null;

        private static IExternalLogger _externalLogger = null;

        private static AsLogInfo _externalFilter;

        public static String GetTraceFile()
        {
            return Path;
        }

        public static String FileName { get; set; }

        public static void SetExternalLogger(IExternalLogger logger, AsLogInfo filter)
        {
            _externalLogger = logger;
            _externalFilter = filter;
        }

        public static void SetExternalLoggerFilter(AsLogInfo filter)
        {
            _externalFilter = filter;
        }

        private static Thread myThread;

        private static LogSource DefaultLogSource;

        public static void InitLog(String applicationName, string traceFileP,LogSource defaultLogSource = LogSource.None)
        {
            DefaultLogSource = defaultLogSource;
            ApplicationName = applicationName;
            traceFilePath = traceFileP;

            int Hour = 0;
            int Minute = 1;
            String MarkTimeStr = "23:59";
            if (MarkTimeStr != null)
            {
                String[] Arr = MarkTimeStr.Split(new char[] {':'});
                Hour = int.Parse(Arr[0]);
                Minute = int.Parse(Arr[1]);
            }

            DateTime LogRollTime = DateTime.Now;
            LogRollTime = new DateTime(LogRollTime.Year, LogRollTime.Month, LogRollTime.Day, Hour, Minute, 0);

            if (LogRollTime < DateTime.Now)
            {
                LogRollTime = LogRollTime.AddDays(1);
            }

            RollLogTimer = new Timer(RollLog, null, LogRollTime - DateTime.Now, new TimeSpan(1, 0, 1, 0));

            CreateTextWriter();

            myThread = new Thread(LogThread);

            myThread.Start();
        }

        private static AutoResetEvent ev = new AutoResetEvent(false);

        private static bool finished = false;
        static Queue<string> logEntries = new Queue<string>();

        static void LogThread(Object o)
        {
            List<string> strings = new List<string>();
            while (!finished)
            {
                ev.WaitOne();
                lock (logEntries)
                {
                    strings.AddRange(logEntries);
                    logEntries.Clear();
                }

                foreach (var c in strings)
                {
                    Writer.Write(c);
                }
                strings.Clear();
                Writer.Flush();
            }

        }

        static void WriteAsync(String s)
        {
            lock (logEntries)
            {
                logEntries.Enqueue(s);
            }
            ev.Set();
        }

        static void WriteAsyncLine(String s)
        {
            lock (logEntries)
            {
                logEntries.Enqueue(s);
                logEntries.Enqueue("\n");
            }
            ev.Set();
        }

        public static void InitLogConsole(TextWriter o, String applicationName)
        {
            Writer = o;
            ApplicationName = applicationName;

            myThread = new Thread(LogThread);

            myThread.Start();
        }


        static void RollLog(object obj)
        {
            try
            {
                CreateTextWriter();

            }
            catch (Exception )
            {
            }

        }

        static void CreateTextWriter()
        {
            if (Writer != null)
                CloseLog();

            if (Directory.Exists(traceFilePath) == false)
                Directory.CreateDirectory(traceFilePath);

            String findExisting = String.Format("{0}_{1}*.txt", ApplicationName,
                DateTime.Now.ToUniversalTime().ToString("yyyyMMdd"));

            if ((traceFilePath + "\\" + findExisting).Length < 260)
            {
                var runCount = Directory.GetFiles(traceFilePath, findExisting).Count() + 1;

                Path = String.Format("{0}\\{1}_{2}{3:000}.txt", traceFilePath, ApplicationName,
                    DateTime.Now.ToUniversalTime().ToString("yyyyMMdd"), runCount);
            }
            else
            {
                var tFileLong = traceFilePath + "\\" + String.Format("{0}_{1}.txt", ApplicationName,
                                    DateTime.Now.ToUniversalTime().ToString("yyyyMMdd"));
                // if path is too long forget all that and give us a random ending that is within limits
                var toChopOff = tFileLong.Length - 280;
                Path = tFileLong.Substring(0, findExisting.Length - toChopOff);
                Path += LogRandomProvider.GetNextString(10000);
                Path += ".txt";

            }

            try
            {
                Writer = System.IO.File.CreateText(Path);
            }
            catch (Exception)
            {
                Console.WriteLine("Error occured opening log file path is : {0}", Path);

            }
            FileName = System.IO.Path.GetFileName(Path);
        }

        public static void CloseLog()
        {
            finished = true;
            ev.Set();
            if (Writer != null)
            {
                lock (Writer)
                {
                    Writer.Dispose();
                    Writer = null;
                }
            }
        }

        public static void Error(LogSource ls, String str)
        {
            if (Writer != null)
            {
                lock (Writer)
                {
                    WriteAsync($"{DateTime.UtcNow}\t{AsLogInfo.Error}\t{ls}\t{str}\r\n");

                    if (_externalLogger != null)
                        _externalLogger.Log(new AsLogEntry(AsLogInfo.Error, DateTime.UtcNow, ApplicationName, str));
                }
            }
        }

        public static void Exception(LogSource ls,Exception e)
        {
            String Message = e.ToString();
            Error(ls,Message);

            if (e.InnerException != null)
            {
                Message = String.Format("{0}\t - inner exception - {1}", DateTime.Now, e.InnerException.Message);
                Error(ls, Message);
            }
        }

        public static void Exception(Exception e)
        {
            String Message = e.ToString();
            Error(DefaultLogSource, Message);

            if (e.InnerException != null)
            {
                Message = String.Format("{0}\t - inner exception - {1}", DateTime.Now, e.InnerException.Message);
                Error(DefaultLogSource, Message);
            }
        }


        public static void Trace(LogSource session, String str)
        {
            if (ExceptionOnly)
                return;

            WriteAsync($"{DateTime.UtcNow}\t{AsLogInfo.Info}\t{session}\t{str}\r\n");

            if (_externalLogger != null)
                _externalLogger.Log(new AsLogEntry(AsLogInfo.Info, DateTime.UtcNow, session.ToString(), str));


        }

        public static void Trace(String str)
        {
            if (ExceptionOnly)
                return;

            WriteAsync($"{DateTime.UtcNow}\t{AsLogInfo.Info}\t{DefaultLogSource}\t{str}\r\n");

            if (_externalLogger != null)
                _externalLogger.Log(new AsLogEntry(AsLogInfo.Info, DateTime.UtcNow, DefaultLogSource.ToString(), str));


        }
    }

    public static class LogRandomProvider
    {
        private static int seed = Environment.TickCount;

        private static ThreadLocal<System.Random> randomWrapper = new ThreadLocal<System.Random>(() =>
            new System.Random(Interlocked.Increment(ref seed))
        );

        public static System.Random GetThreadRandom()
        {
            return randomWrapper.Value;
        }

        public static int GetNext()
        {
            return GetThreadRandom().Next();
        }

        public static int GetNext(int max)
        {
            return GetThreadRandom().Next(max);
        }

        public static String GetNextString(int max)
        {
            return toBase26(GetNext(max));
        }

        static String toBase26(int number)
        {
            number = Math.Abs(number);
            String converted = "";
            // Repeatedly divide the number by 26 and convert the
            // remainder into the appropriate letter.
            do
            {
                int remainder = number % 26;
                converted = (char) (remainder + 'A') + converted;
                number = (number - remainder) / 26;
            } while (number > 0);

            return converted;
        }
    }

}
