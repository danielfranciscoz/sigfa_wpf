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
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Model;
using MaterialDesignThemes.Wpf;
using PruebaWPF.Clases;

namespace PruebaWPF.Views.Main
{
    /// <summary>
    /// Lógica de interacción para pgDashboard.xaml
    /// </summary>
    public partial class pgDashboard : Page
    {
        AccountViewModel controller;
        List<Pantalla> AccesosPerfil;
        List<AccesoDirectoUsuario> AccesosUsuario;
        public pgDashboard()
        {
            controller = new AccountViewModel();
            InitializeComponent();
            AgregarDashBoard();
        }

        private void AgregarDashBoard()
        {
            Card_AccesoDirecto AccesoDirecto;
            AccesosPerfil = controller.ObtenerAccesoDirectoPerfil();
            AccesosUsuario = controller.ObtenerAccesoDirectoUsuario();

            foreach (Pantalla a in AccesosPerfil)
            {
                AccesoDirecto = IniciarCard(a.Titulo, a.Icon, a.Abreviacion, "ADPerfil");
                AccesoDirecto.pantalla = a;
                MainContainer.Children.Add(AccesoDirecto);
            }

          

            foreach (AccesoDirectoUsuario a in AccesosUsuario)
            {
                AccesoDirecto = IniciarCard(a.Pantalla.Titulo, a.Pantalla.Icon, a.Pantalla.Abreviacion, string.IsNullOrEmpty(a.BackgroundCard) ? "AD_Gris" : "AD_" + a.BackgroundCard);
                AccesoDirecto.pantalla = a.Pantalla;
                MainContainer.Children.Add(AccesoDirecto);
            }

        }

        private Card_AccesoDirecto IniciarCard(string titulo, string icon, string abreviacion, string resource)
        {
            Card_AccesoDirecto ad = new Card_AccesoDirecto();
            ad.txtTitulo.Text = titulo;
            ad.icon.Kind = clsutilidades.GetIconFromString(icon);

            ad.txtAbreviacion.Text = abreviacion;

            SolidColorBrush colorFondo;
            try
            {
                colorFondo = (SolidColorBrush)FindResource(resource);
            }
            catch (ResourceReferenceKeyNotFoundException a)
            {
                colorFondo = (SolidColorBrush)FindResource("AD_Gris");
            }
            ad.CAccesoDirecto.Background = new SolidColorBrush(colorFondo.Color);

            return ad;
        }


    }
}
