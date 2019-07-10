using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace WorkhoursCalculator
{
    /// <summary>
    /// Interaktionslogik für "App.xaml"
    /// </summary>
    public partial class App : Application
    {
        public void StartApp(object sender, StartupEventArgs args)
        {
            var config = Config.Load();
            var repository = new WorkHoursRepository(config);
            var viewModel = new MainViewModel(repository, config);
            var window = new MainWindow(viewModel);
            DispatcherUnhandledException += OnUnhandledException;
            window.Show();
        }

        public void OnUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs args)
        {
            MessageBox.Show(args.Exception.Message + "   " + args.Exception.InnerException != null ? args.Exception.InnerException.Message : "");
            //args.
        }
    }
}
