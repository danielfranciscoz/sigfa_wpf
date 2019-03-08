using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

namespace PruebaWPF.Views.Administracion
{
    /// <summary>
    /// Lógica de interacción para AddPermiso.xaml
    /// </summary>
    public partial class AddPermiso : Window
    {
        AdministracionViewModel controller;
        Pantalla pantalla;
        Pantalla pantallaPadre;
        Perfil perfil;
        ObservableCollection<PermisoSon> permisos;


        public AddPermiso()
        {
            InitializeComponent();
        }

        public AddPermiso(Pantalla pantalla, Perfil perfil, Pantalla pantallaPadre)
        {
            this.pantalla = pantalla;
            this.perfil = perfil;
            this.pantallaPadre = pantallaPadre;
            InitializeComponent(); Inicializar();
        }

        private void Inicializar()
        {
            controller = new AdministracionViewModel(pantalla);
            permisos = new ObservableCollection<PermisoSon>();
            Diseñar();
            CargarCombo();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void CargarCombo()
        {
            cboRecinto.ItemsSource = controller.Recintos();
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void cboRecinto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRecinto.SelectedIndex != -1)
            {
                CargarPermisos();
            }
        }

        private void btnAddPermiso_Click(object sender, RoutedEventArgs e)
        {
            PermisoName p = (PermisoName)((Button)sender).DataContext;
            AgregarPermiso(p);
        }

        private void AgregarPermiso(PermisoName permiso)
        {
            PermisoSon p = new PermisoSon();
            vw_RecintosRH r = (vw_RecintosRH)cboRecinto.SelectedItem;
            p.Recinto = r.Siglas;
            p.IdRecinto = byte.Parse(r.IdRecinto.ToString());
            p.IdPermisoName = permiso.IdPermisoName;
            p.PermisoName = permiso;
            p.IdPantalla = pantallaPadre.IdPantalla;

            if (!permisos.Any(a => a.IdPermisoName == p.IdPermisoName && a.IdRecinto == p.IdRecinto))
            {
                permisos.Add(p);
            }

        }

        private void btnDeletePermiso_Click(object sender, RoutedEventArgs e)
        {
            permisos.Remove((PermisoSon)tblPermisos.CurrentItem);
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (validarPermisosTabla())
                {
                    clsutilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                }
                else
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = "Para poder continuar, deberá agregar al menos un permiso a la tabla.", OperationType = clsReferencias.TYPE_MESSAGE_Error });
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private bool validarPermisosTabla()
        {
            return permisos.Any();
        }

        private void CargarPermisos()
        {
            byte idrecinto = byte.Parse(((vw_RecintosRH)cboRecinto.SelectedItem).IdRecinto.ToString());
            lstPermisos.ItemsSource = controller.FindPermisosToAdd(pantallaPadre, perfil, idrecinto);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            tblPermisos.ItemsSource = permisos;
        }

        private Operacion Guardar()
        {
            Administracion.selectedPerfil = controller.SavePermiso(perfil, permisos.ToList());
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private void Finalizar()
        {
            Administracion.Cambios = true;
            Administracion.isAddPermiso = true;

            frmMain.Refrescar();
            Close();
        }
    }
}
