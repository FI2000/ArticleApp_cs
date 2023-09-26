namespace ArticleApp.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string filePath;

        public FileLogger(string path)
        {
            filePath = path;
            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
        }

        public IDisposable BeginScope<TState>(TState state) => null;

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            if (!IsEnabled(logLevel))
            {
                return;
            }

            string logMessage = formatter(state, exception);
            if (exception != null)
            {
                logMessage += $"; Exception: {exception.Message}";
            }
            string logLine = $"{DateTime.UtcNow:O} [{logLevel}] {logMessage}";

            switch (logLevel)
            {
                case LogLevel.Trace:
                    File.AppendAllText(filePath.Replace(".txt", "_trace.txt"), logLine + Environment.NewLine);
                    break;
                case LogLevel.Debug:
                    File.AppendAllText(filePath.Replace(".txt", "_debug.txt"), logLine + Environment.NewLine);
                    break;
                case LogLevel.Warning:
                    File.AppendAllText(filePath.Replace(".txt", "_warn.txt"), logLine + Environment.NewLine);
                    break;
                case LogLevel.Error:
                    File.AppendAllText(filePath.Replace(".txt", "_error.txt"), logLine + Environment.NewLine);
                    break;
                case LogLevel.Critical:
                    File.AppendAllText(filePath.Replace(".txt", "_critical.txt"), logLine + Environment.NewLine);
                    break;
                default:
                    File.AppendAllText(filePath, logLine + Environment.NewLine);
                    break;
            }
        }
    }
}
