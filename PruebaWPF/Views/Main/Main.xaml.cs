using MaterialDesignThemes.Wpf;
using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Acceso;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace PruebaWPF.Views.Main
{
    /// <summary>
    /// Lógica de interacción para Main.xaml
    /// </summary>
    public partial class frmMain : Window
    {
        AccountViewModel controller;

        public static Frame Contenedor;
        private const String Namespace = "PruebaWPF.Views.";
        public static Window principal;
        public static frmMain frmmain;

        public frmMain()
        {
            controller = new AccountViewModel();
            InitializeComponent();

            //this.Left = SystemParameters.WorkArea.Left;
            //this.Top= SystemParameters.WorkArea.Top;

            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight;

            CargarMenu();
            IniciarFrame();
            CargarStatusBar();
            principal = this;
            frmmain = this;
            this.DataContext = this;

        }

        public static double Alto()
        {
            return principal.ActualHeight;
        }
        public static double Ancho()
        {
            return principal.ActualWidth;
        }

        private void CargarMenu()
        {
            List<Pantalla> pantallas = controller.ObtenerMenu();

            MenuItem mi;

            foreach (Pantalla item in pantallas.Where(w => w.IdPadre == null))
            {
                mi = new MenuItem();
                mi.Header = item.Titulo;
                mi.Height = Double.NaN;

                MenuOpciones.Items.Insert(item.Orden - 1, mi);
                CargarSubMenu(item, pantallas, mi);
            }
        }

        private void CargarSubMenu(Pantalla padre, List<Pantalla> pantallas, MenuItem mi)
        {
            MenuItem subMi;
            Separator sep = new Separator();
            foreach (Pantalla item in pantallas.Where(w => w.IdPadre == padre.IdPantalla))
            {
                subMi = new MenuItem();

                if (item.Tipo != null)
                {
                    if (!item.Tipo.Equals("Separador"))
                    {
                        subMi.Header = item.Titulo;
                        subMi.Icon = getIcon(item.Icon);

                        if (item.URL != null)
                        {
                            subMi.Click += (sender, e) => MenuDinamic_Click(item);
                        }

                        mi.Items.Add(subMi);
                        CargarSubMenu(item, pantallas, subMi);
                    }
                    else
                    {
                        mi.Items.Add(sep);
                    }
                }
                else
                {
                    subMi.Header = item.Titulo;
                    subMi.Height = Double.NaN;
                    mi.Items.Add(subMi);
                    CargarSubMenu(item, pantallas, subMi);
                }


            }

        }

        private PackIcon getIcon(string icon)
        {
            MaterialDesignThemes.Wpf.PackIcon ico = new PackIcon();
            ico.Kind = clsutilidades.GetIconFromString(icon);
            return ico;
        }

        private void MenuDinamic_Click(Pantalla p)
        {
            AddWindowOrPage(p);
        }

        private void IniciarFrame()
        {
            Contenedor = new Frame();
            Contenedor.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            Contenedor.VerticalContentAlignment = VerticalAlignment.Stretch;
            Contenedor.NavigationUIVisibility = System.Windows.Navigation.NavigationUIVisibility.Hidden;
            MainContainer.Children.Add(Contenedor);

            //Agrego los accesos directos por defecto
            AddPage(new pgDashboard());

        }

        private void CargarStatusBar()
        {
            try
            {
                String perfil = "";

                foreach (UsuarioPerfil up in clsSessionHelper.perfiles.Distinct())
                {
                    perfil = perfil.Equals("") ? up.Perfil.Perfil1 : perfil + "," + up.Perfil.Perfil1;

                }
                //for (int i = 0; i < clsSessionHelper.perfiles.Count; i++)
                //{
                //    perfil = "" + ((i == 0) ? clsSessionHelper.perfiles.ElementAt(i).Perfil.Perfil1 : perfil + "," + clsSessionHelper.perfiles.ElementAt(i).Perfil.Perfil1);
                //}

                lblPerfil.ToolTip = perfil;

                if (perfil.Length > 13)
                {
                    perfil = perfil.Substring(0, 13) + "...";
                }

                lblPerfil.Text = perfil;
                lblPeriodo.Text = clsSessionHelper.periodoEspecifico.FechaInicio.Month.ToString() + "/" + clsSessionHelper.periodoEspecifico.FechaInicio.Year.ToString() + " - " + clsSessionHelper.periodoEspecifico.Periodo;
                mnUsuario.Header = clsSessionHelper.usuario.Login;
                lblTipoCambio.Text = controller.ObtenerTipoCambio();
                lblMac.Text = clsSessionHelper.MACMemory;

            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void AddPage(Page page)
        {
            Contenedor.NavigationService.Navigate(page);
        }

        private static void AddPage(Pantalla p)
        {
            try
            {
                Type tipo = Type.GetType(Namespace + p.URL);
                Page page = (Page)System.Activator.CreateInstance(tipo, p);

                Contenedor.NavigationService.Navigate(page);
                //Contenedor.NavigationService.Navigate(new Uri(Url, UriKind.Relative));
                //TitleName = p.Titulo;
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        public static void AddWindowOrPage(Pantalla p)
        {
            if (p.Tipo.Equals("Pagina"))
            {
                AddPage(p);
            }
            else
            {

                OpenWindow(p);
            }
        }

        private static void OpenWindow(Pantalla p)
        {

            Type tipo = Type.GetType(Namespace + p.URL);
            //Window window = (Window)System.Windows.Application.LoadComponent(url);
            Window window = (Window)System.Activator.CreateInstance(tipo, p);
            window.Title = p.Titulo;
            if (p.Tipo.Equals("Dialogo"))
            {
                window.Owner = Window.GetWindow(principal);
                window.ShowDialog();
            }
            else
            {
                window.Show();
            }
        }

        public static void GoBack()
        {
            while (Contenedor.CanGoBack)
            {
                Contenedor.NavigationService.GoBack();
            }
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            CerrarSesion();
        }

        public static void AddNotification(frmMain valor, Operacion o)
        {
            valor.Label_Exportando.Visibility = Visibility.Collapsed;
            valor.AddItemNotificacion(o);
        }

        private void AddItemNotificacion(Operacion o)
        {
            MenuItem menu = new MenuItem();

            if (string.IsNullOrEmpty(o.Titulo))
            {
                menu.Header = o.Mensaje;
            }
            else
            {
                menu.Header = o.Titulo;
            }

            menu.Icon = getIcon(o.Icon().ToString());
            menu.Click += (sender, e) => MenuNotification_Click(sender, e, o);

            btnNotificaciones.ContextMenu.Items.Add(menu);

            btnNotifications_Visibilidad();

        }

        private void MenuNotification_Click(object sender, RoutedEventArgs e, Operacion o)
        {
            btnNotificaciones.ContextMenu.Items.Remove(sender);
            btnNotifications_Visibilidad();
            if (o.OperationType != clsReferencias.TYPE_MESSAGE_Exito)
            {
                clsutilidades.OpenMessage(o);
            }
        }

        internal static void Exportando_Label(frmMain principal)
        {
            principal.Label_Exportando.Visibility = Visibility.Visible;
        }

        private void btnNotifications_Visibilidad()
        {
            if (btnNotificaciones.ContextMenu.HasItems)
            {
                if (btnNotificaciones.ContextMenu.Items.Count > 1)
                {
                    txtNotificacion.Text = "Notificaciones";
                }
                else
                {
                    txtNotificacion.Text = "Notificación";
                }

                txtNotificacion.Text = btnNotificaciones.ContextMenu.Items.Count.ToString();
                btnNotificaciones.Visibility = Visibility.Visible;
            }
            else
            {
                btnNotificaciones.Visibility = Visibility.Hidden;
            }
        }

        private void CerrarSesion()
        {
            this.Hide();
            MainWindow login = new MainWindow();
            login.Show();
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                btnswitch.IsChecked = true;
            }
            Refrescar();
        }

        public static void Refrescar()
        {
            Contenedor.NavigationService.Refresh();
            //Contenedor.Refresh();
        }


        private void btnNotificaciones_Click(object sender, RoutedEventArgs e)
        {
            (sender as Button).ContextMenu.IsEnabled = true;
            (sender as Button).ContextMenu.PlacementTarget = (sender as Button);
            (sender as Button).ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            (sender as Button).ContextMenu.IsOpen = true;

        }

        private void minimize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void maximize_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Maximized;
            btnswitch.IsChecked = true;
        }

        private void restore_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Normal;
            btnswitch.IsChecked = false;

        }

        private void close_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void DockPanel_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            Views.Account.Configuracion cf = new Views.Account.Configuracion();
            cf.ShowDialog();
        }
    }
}
