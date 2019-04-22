using System;
using System.Diagnostics;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;
using File = System.IO.File;

namespace Memorizer.Utility
{
    internal static class Logger
    {
        public static string LogFilePath { get; set; }
        public static RichTextBox LogControl { get; set; }

        static Logger()
        {
            LogFilePath = "Logs.txt";
        }

        public static void Log(string logMessage, SolidColorBrush color = null)
        {
            try
            {
                if (LogControl != null)
                    InvokeIfRequired(LogControl, () =>
                    {
                        LogControl.Document.Blocks.Add(new Paragraph(new Run($"{DateTime.Now:HH:mm:ss} {logMessage}")) { Foreground = color ?? Brushes.Black });
                    });

                lock (LogFilePath)
                {
                    File.AppendAllText(LogFilePath, $"{DateTime.Now:dd.MM.yyyy HH:mm:ss} {logMessage}{Environment.NewLine}");
                }
            }
            catch (Exception ex)
            {
                ErrorLog(ex);
            }
        }

        public static void ErrorLog(Exception ex)
        {
            var stackTrace = new StackTrace(ex, true);
            var frame = stackTrace.GetFrame(stackTrace.FrameCount - 1);
            var lineNumber = frame?.GetFileLineNumber();
            var source = frame?.GetMethod().DeclaringType;

            Log($"Error in {source?.ToString() ?? "null"} on {lineNumber?.ToString() ?? "null"}: {ex.Message}", Brushes.Red);

            if (ex.InnerException != null)
                ErrorLog(ex.InnerException);
        }

        private static void InvokeIfRequired(DispatcherObject textBox, Action action)
        {
            textBox.Dispatcher.BeginInvoke(DispatcherPriority.Normal, action);
        }
    }
}