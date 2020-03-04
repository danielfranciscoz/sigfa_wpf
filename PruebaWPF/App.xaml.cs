using PruebaWPF.Clases;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;

namespace PruebaWPF
{
    /// <summary>
    /// Lógica de interacción para App.xaml
    /// </summary>
    public partial class App : Application
    {

        protected override void OnStartup(StartupEventArgs e)
        {
            // define application exception handler
            Application.Current.DispatcherUnhandledException +=
                App_DispatcherUnhandledException;

            // defer other startup processing to base class
            base.OnStartup(e);
        }

        private void ShowError(Exception ex)
        {
            if (ex != null)
            {
                //Show error in a message
                new SharedViewModel().SaveError(ex);
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });

            }

        }
        void App_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.ShowError(e.Exception);
            //Prevent default unhandled exception processing
            e.Handled = true;
        }

        void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.ShowError(e.Exception);

            // Prevent default unhandled exception processing
            e.Handled = true;
        }

        void Exception_AppDomain(object sender, UnhandledExceptionEventArgs e)
        {
            this.ShowError(e.ExceptionObject as Exception);
        }

        private void TaskScheduler_UnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            this.ShowError(e.Exception);
        }


    }
}
