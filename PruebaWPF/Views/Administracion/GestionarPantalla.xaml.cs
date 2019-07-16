using MaterialDesignThemes.Wpf;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Shared;
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
using System.Windows.Shapes;

namespace PruebaWPF.Views.Administracion
{
    /// <summary>
    /// Lógica de interacción para GestionarPantalla.xaml
    /// </summary>
    public partial class GestionarPantalla : Window
    {
        Pantalla pantalla;
        AdministracionViewModel controller;
        clsValidateInput validate = new clsValidateInput();
        int? idpadreinicial;
        public GestionarPantalla()
        {
            pantalla = new Pantalla();
            idpadreinicial = -1;
            InitializeComponent();
            Inicializar();
        }

        public GestionarPantalla(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            idpadreinicial = pantalla.IdPadre;
            InitializeComponent();
            Inicializar();
            txtTitle.Text = "Editar Pantalla";
            isWeb.IsEnabled = false;
            cboTipo.Text = pantalla.Tipo;
            txtUid.IsEnabled = controller.HasSons(pantalla);
        }
        private void Inicializar()
        {
            controller = new AdministracionViewModel();
            Diseñar();
            CargarCombo();
            DataContext = pantalla;
            CamposNormales();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtTitulo, cboPadre, txtUid, cboTipo, txtURL, cboOrden });
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void CargarCombo()
        {
            List<Pantalla> pantallas;

            if (pantalla.IdPantalla == 0)
            {
                pantallas = controller.FindPantallasPadre(pantalla.isWeb, null);
            }
            else
            {
                pantallas = controller.FindPantallasPadre(pantalla.isWeb, pantalla.IdPantalla);
            }

            pantallas.Insert(0, new Pantalla() { IdPantalla = 0, Titulo = "-Ninguno-" });
            cboPadre.ItemsSource = pantallas;

            cboPadre.SelectedValue = pantalla.IdPadre == null ? 0 : pantalla.IdPadre;
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
                    pantalla.IdPadre = int.Parse(cboPadre.SelectedValue.ToString());
                    if (!string.IsNullOrEmpty(pantalla.Uid))
                    {
                        pantalla.Tipo = cboTipo.Text;
                    }
                    if (pantalla.isMenu)
                    {
                        pantalla.Orden = byte.Parse(cboOrden.SelectedValue.ToString());
                    }

                    clsUtilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
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
            int maxPosition = ((Pantalla)cboOrden.Items.GetItemAt(0)).Orden;

            controller.SaveUpdatePantalla(pantalla, maxPosition);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private bool ValidarCampos()
        {
            bool flag = true;
            Control[] campos = new Control[6];

            campos[0] = txtTitulo;
            campos[1] = cboPadre;

            if (!string.IsNullOrEmpty(pantalla.Uid))
            {
                campos[2] = txtUid;
                campos[3] = cboTipo;
                campos[4] = txtURL;
            }
            if (pantalla.isMenu)
            {
                campos[5] = cboOrden;
            }

            flag = clsValidateInput.ValidateALL(campos);
            return flag;
        }

        private void cboPadre_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboPadre.SelectedItem != null)
            {
                CargarOrdenes(((Pantalla)cboPadre.SelectedItem).IdPantalla);
            }

        }

        private void CargarOrdenes(int v)
        {
            cboOrden.ItemsSource = controller.FindOrdenPantalla(v, pantalla.isWeb, pantalla.IdPantalla > 0 ? true : false, idpadreinicial);
            cboOrden.SelectedValue = pantalla.Orden;

        }

        private void chkisMenu_Checked(object sender, RoutedEventArgs e)
        {
            if (cboPadre.SelectedItem != null)
            {
                CargarOrdenes(int.Parse(cboPadre.SelectedValue.ToString()));
            }
        }


        private void isWeb_Click(object sender, RoutedEventArgs e)
        {
            CargarCombo();
        }

        private void btnFindIcon_Click(object sender, RoutedEventArgs e)
        {
            Iconos i = new Iconos();
            i.ShowDialog();
            if (!string.IsNullOrEmpty(i.selectedIcon))
            {
                pantalla.Icon = i.selectedIcon;
                clsUtilidades.UpdateControl(txtIcono);
            }
        }

        private void btnRemoveIcon_Click(object sender, RoutedEventArgs e)
        {
            pantalla.Icon = "";
            clsUtilidades.UpdateControl(txtIcono);
        }
    }


}
