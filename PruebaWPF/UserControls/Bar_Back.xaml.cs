using PruebaWPF.Clases;
using PruebaWPF.Referencias;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PruebaWPF.UserControls
{
    /// <summary>
    /// Lógica de interacción para Bar_Back.xaml
    /// </summary>
    public partial class Bar_Back : UserControl
    {
        private Boolean ReloadActive;
        public string Value
        {
            get; set;
        }
        public Boolean AutoReload
        {
            get; set;
        }

        public string ValueUpper => Value.ToUpper();
        public Visibility CanAutoReload => AutoReload ? Visibility.Visible : Visibility.Hidden;

        public Visibility SyncOn => ReloadActive ? Visibility.Visible : Visibility.Collapsed;
        public Visibility SyncOff => ReloadActive ? Visibility.Collapsed : Visibility.Visible;

        public Bar_Back()
        {
            this.ReloadActive = clsConfiguration.Actual().AutoLoad;
            InitializeComponent();
        }



        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frmMain.GoBack();
        }

        private void btnActiveAutoLoad_Click(object sender, RoutedEventArgs e)
        {
            if (clsConfiguration.Actual().AutoLoad) //En esta parte no funcionó el uso de la variable ReloadActive
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = "La carga automatica se encuentra ACTIVADA.\n\nLa información en esta ventana se actualiza automáticamente cuando se detectan cambios en la base de datos.\nPara modificar esta y otras opciones del sistema, deberá hacer uso de la funcionalidad CONFIGURACIÓN incluida en el menú de usuario", OperationType = clsReferencias.TYPE_MESSAGE_Information });
            }
            else
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = "La carga automatica se encuentra DESACTIVADA.\n\nLa información en esta ventana no se está actualizando automáticamente al existir cambios en la base de datos.\nPara modificar esta y otras opciones del sistema, deberá hacer uso de la funcionalidad CONFIGURACIÓN incluida en el menú de usuario", OperationType = clsReferencias.TYPE_MESSAGE_Information });
            }
        }
    }
}
