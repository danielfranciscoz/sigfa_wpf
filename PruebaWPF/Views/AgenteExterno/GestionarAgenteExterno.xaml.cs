using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
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
using System.Windows.Shapes;

namespace PruebaWPF.Views.AgenteExterno
{
    /// <summary>
    /// Lógica de interacción para GestionarAgenteExterno.xaml
    /// </summary>
    public partial class GestionarAgenteExterno : Window
    {
        //AgenteExternoViewModel controller;
        AgenteExternoCat agente;
        AgenteExternoViewModel controller;
        Pantalla pantalla;
        clsValidateInput validate = new clsValidateInput();

        public GestionarAgenteExterno()
        {
            InitializeComponent();
        }
        public GestionarAgenteExterno(Pantalla pantalla, String PermisoName)
        {
            agente = new AgenteExternoCat();
            this.pantalla = pantalla;
            InitializeComponent();
            Diseñar();
            Inicializar();
        }

        public GestionarAgenteExterno(AgenteExternoCat agente, Pantalla pantalla, String PermisoName)
        {
            this.agente = agente;
            this.pantalla = pantalla;
            InitializeComponent();
            Diseñar();
            Inicializar();
        }

        private void Inicializar()
        {
            controller = new AgenteExternoViewModel(pantalla);
            Diseñar();
            CargarCombo();
            DataContext = agente;
            CamposNormales();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtNombre, txtIdentificacion, cboIdentificacion, txtProcedencia });
        }

        private void CargarCombo()
        {
            cboIdentificacion.ItemsSource = controller.FindIdentificaciones(agente.IdIdentificacion);
            cboIdentificacion.SelectedValue = agente.IdIdentificacion;

        }

        private void Diseñar()
        {
            if (agente.IdAgenteExterno > 0)
            {
                btnSave.Visibility = Visibility.Collapsed;
                btnEdit.Visibility = Visibility.Visible;
                txtTitle.Text = "Editar Agente Externo";
            }

            clsUtilidades.Dialog_ModalDesign(this);
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
                    if (ValidarLength())
                    {
                        agente.IdIdentificacion = int.Parse(cboIdentificacion.SelectedValue.ToString());
                        clsUtilidades.OpenMessage(Guardar(), this);
                        Finalizar();
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private bool ValidarLength()
        {
            IdentificacionAgenteExterno a = (IdentificacionAgenteExterno)cboIdentificacion.SelectedItem;

            return clsValidateInput.ValidateLength(txtIdentificacion, a.MaxCaracteres, a.isMaxMin);
        }

        private void Finalizar()
        {
            AgenteExterno.Cambios = true;
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            if (agente.IdAgenteExterno == 0)
            {
                controller.Guardar(agente);
            }
            else
            {
                controller.Modificar(agente);
            }

            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtNombre, cboIdentificacion, txtIdentificacion, txtProcedencia });
        }
    }
}
