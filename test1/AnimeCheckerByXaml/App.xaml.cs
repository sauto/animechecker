using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using System.IO;

namespace AnimeCheckerByXaml
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
        const string LOG_FILE_NAME = "log.txt";
        string _logName = Path.GetDirectoryName(Environment.GetCommandLineArgs()[0]) + "\\" + LOG_FILE_NAME;

        private void Application_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            using (System.IO.StreamWriter sw =
                new System.IO.StreamWriter(_logName, false, System.Text.Encoding.GetEncoding("shift_jis")))
            {
                sw.WriteLine("メッセージ：" + e.Exception.Message);
                sw.WriteLine("スタックトレース：" + e.Exception.StackTrace);
            }

        }

    }
}
