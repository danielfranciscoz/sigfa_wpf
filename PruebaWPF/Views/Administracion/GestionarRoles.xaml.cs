using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PruebaWPF.Views.Administracion
{
    /// <summary>
    /// Lógica de interacción para GestionarRoles.xaml
    /// </summary>
    public partial class GestionarRoles : Window
    {
        AdministracionViewModel controller;
        Pantalla pantalla;
        clsValidateInput validate = new clsValidateInput();

        ObservableCollection<PantallaToAccess> pantallas;
        ObservableCollection<ListaExporta> recintos;
        Perfil perfil;

        public GestionarRoles()
        {
            InitializeComponent();
        }

        public GestionarRoles(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            perfil = new Perfil();
            InitializeComponent();
            Inicializar();
        }

        public GestionarRoles(Pantalla pantalla, Perfil perfil)
        {
            this.pantalla = pantalla;
            this.perfil = perfil;
            InitializeComponent();
            OcultarPaneles(Visibility.Collapsed);
            Inicializar();

        }

        private void OcultarPaneles(Visibility visibilidad)
        {
            panelRecintos.Height = new GridLength(0);
            panelSeparador.Width = new GridLength(0);
            panelPermisos.Width = new GridLength(0);
            body.Height = new GridLength(200);
            txtTitle.Text = "Editar Perfil";
            btnSave.Visibility = btnEdit.Visibility;
            btnEdit.Visibility = Visibility.Visible;
        }

        private void Inicializar()
        {
            controller = new AdministracionViewModel();

            Diseñar();
            this.DataContext = perfil;
            CamposNormales();
        }
        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtNombre, txtDescripcion });
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    if (validarSeleccionRecintos())
                    {
                        if (validarSeleccionPantallas())
                        {
                            clsutilidades.OpenMessage(Guardar(), this);
                            Finalizar();
                        }
                        else
                        {
                            clsutilidades.OpenMessage(new Operacion() { Mensaje = "Para poder continuar, deberá conceder acceso al menos a una de las pantallas que se muestran en la tabla.", OperationType = clsReferencias.TYPE_MESSAGE_Error });
                        }
                    }
                    else
                    {
                        clsutilidades.OpenMessage(new Operacion() { Mensaje = "Para poder continuar, deberá seleccionar al menos un recinto al cual se le crearán los accesos.", OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    }
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Finalizar()
        {
            Administracion.Cambios = true;
            Administracion.isCreated = true;

            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            List<PantallaToAccess> accesos = pantallas.Where(w => w.canAccess).ToList();
            List<vw_RecintosRH> recintosaccess = recintos.Where(w => w.isChecked).Select(s => new vw_RecintosRH() { IdRecinto = int.Parse(s.Name), Siglas = s.Caption }).ToList();

            Administracion.selectedPerfil = controller.SaveUpdatePerfil(perfil, recintosaccess, accesos);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private bool validarSeleccionPantallas()
        {
            if (!pantallas.Any(a => a.canAccess) && perfil.IdPerfil==0)
            {
                return false;
            }
            return true;
        }

        private bool validarSeleccionRecintos()
        {
            if (!recintos.Any(a => a.isChecked) && perfil.IdPerfil == 0)
            {
                return false;
            }
            return true;
        }

        private void isWeb_Click(object sender, RoutedEventArgs e)
        {
            CargarPantallas();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            CargarDatos();
        }

        private void CargarDatos()
        {
            CargarRecintos();
            CargarPantallas();
        }

        private void CargarPantallas()
        {
            pantallas = new ObservableCollection<PantallaToAccess>(controller.FindPantallaToAccess(isWeb.IsChecked.Value));
            tblPantallasToAccess.ItemsSource = pantallas;
        }

        private void CargarRecintos()
        {
            recintos = new ObservableCollection<ListaExporta>(
                controller.Recintos().ToList().Select(s => new ListaExporta(s.IdRecinto.ToString(), s.Siglas, false))
                );
            lstRecintos.ItemsSource = recintos;
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtNombre, txtDescripcion });
        }

    }
}
