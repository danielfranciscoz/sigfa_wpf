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
    /// Lógica de interacción para UpdateMenu.xaml
    /// </summary>
    public partial class UpdateMenu : Window
    {
        AdministracionViewModel controller;
        Pantalla pantalla;
        Perfil perfil;
        ObservableCollection<PantallaToAccess> paccess;
        bool OnlyAccesoDirecto;

        public UpdateMenu()
        {
            InitializeComponent();
        }

        public UpdateMenu(Pantalla pantalla, Perfil perfil)
        {
            this.pantalla = pantalla;
            this.perfil = perfil;
            InitializeComponent();
            Inicializar();
        }

        public UpdateMenu(Pantalla pantalla, Perfil perfil, bool OnlyAccesoDirecto)
        {
            this.pantalla = pantalla;
            this.perfil = perfil;
            this.OnlyAccesoDirecto = OnlyAccesoDirecto;
            InitializeComponent();

            txtTitle.Text = "Crear Accesos Directos";
            txtTittle2.Text = "Pantallas disponibles para crear Acceso Directo";
            panelLista.Width = new GridLength(0);
            panelSeparador.Width = new GridLength(0);

            Inicializar();
        }

        private void Inicializar()
        {
            controller = new AdministracionViewModel();
            paccess = new ObservableCollection<PantallaToAccess>();
            Diseñar();
            CargarCombo();
        }

        private void Diseñar()
        {
            if (OnlyAccesoDirecto)
            {
                tblPantallasToAccess.Columns[0].Visibility = Visibility.Collapsed;
                tblPantallasToAccess.Columns[1].Visibility = Visibility.Collapsed;
            }
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void CargarCombo()
        {
            if (!OnlyAccesoDirecto)
            {
                cboRecinto.ItemsSource = controller.Recintos();
            }
        }

        private void btnDeletePantallaRol_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btnAddPantalla_Click(object sender, RoutedEventArgs e)
        {
            PantallaToAccess p = (PantallaToAccess)((Button)sender).DataContext;
            AgregarPantalla(p);
        }

        private void cboRecinto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRecinto.SelectedIndex != -1)
            {
                CargarPantallas();
            }
        }

        private void CargarPantallas()
        {

            if (OnlyAccesoDirecto)
            {
                paccess = new ObservableCollection<PantallaToAccess>(controller.FindPantallaToAccessDirecto(perfil));
            }
            else
            {
                byte idrecinto = byte.Parse(((vw_RecintosRH)cboRecinto.SelectedItem).IdRecinto.ToString());
                lstPantallas.ItemsSource = controller.FindPantallaToAccess(perfil.isWeb, idrecinto, perfil);
            }
        }

        private void AgregarPantalla(PantallaToAccess p)
        {
            if (!OnlyAccesoDirecto)
            {
                vw_RecintosRH r = (vw_RecintosRH)cboRecinto.SelectedItem;

                p.recinto = r;
                p.canAccess = true;
            }

            if (!paccess.Any(a => a.IdPantalla == p.IdPantalla && a.recinto.IdRecinto == p.recinto.IdRecinto))
            {
                paccess.Add(p);
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                
                if (validarSeleccionPantallas())
                {
                    clsutilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                }
                else
                {
                    if (OnlyAccesoDirecto)
                    {
                        clsutilidades.OpenMessage(new Operacion() { Mensaje = "Para poder continuar, deberá crear acceso directo al menos a una de las pantallas que se muestran en la tabla.", OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    }
                    else
                    {
                        clsutilidades.OpenMessage(new Operacion() { Mensaje = "Para poder continuar, deberá conceder acceso al menos a una pantalla.", OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    }
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (OnlyAccesoDirecto)
            {
                CargarPantallas();
                if (paccess.Count==0)
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = "Actualmente no es posible crear mas accesos directos para este perfil.", OperationType = clsReferencias.TYPE_MESSAGE_Error });
                    Close();
                }
            }
            tblPantallasToAccess.ItemsSource = paccess;
        }

        private void btnDeletePantallaToAccess_Click(object sender, RoutedEventArgs e)
        {
            paccess.Remove((PantallaToAccess)tblPantallasToAccess.CurrentItem);
        }

        private bool validarSeleccionPantallas()
        {
            if (OnlyAccesoDirecto)
            {
                if (!paccess.Any(a => a.createAD))
                {
                    return false;
                }

            }
            else
            {
                if (!paccess.Any(a => a.canAccess))
                {
                    return false;
                }

            }
            return true;
        }

        private Operacion Guardar()
        {
            List<PantallaToAccess> accesos = paccess.Where(w => w.canAccess).ToList();

            Administracion.selectedPerfil = controller.SaveUpdatePerfil(perfil, accesos, OnlyAccesoDirecto);

            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private void Finalizar()
        {
            Administracion.Cambios = true;
            Administracion.isCreated = true;

            frmMain.Refrescar();
            Close();
        }
    }
}
