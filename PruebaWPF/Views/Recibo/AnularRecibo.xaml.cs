using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para AnularRecibo.xaml
    /// </summary>
    public partial class AnularRecibo : Window
    {
        clsValidateInput validate = new clsValidateInput();
        private ReciboSon recibo;
        public AnularRecibo()
        {
            InitializeComponent();
            Diseñar();
        }

        public AnularRecibo(ReciboSon recibo)
        {
            this.recibo = recibo;
            InitializeComponent();
            Diseñar();
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
            CamposNormales();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtMotivo, txtRecibo, txtIdentificacion });
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtMotivo, txtRecibo, txtIdentificacion });
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
                    if (ValidarCamposConfirmacion())
                    {
                    clsUtilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                    }
                    else
                    {
                        ReciboInvalido();
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void ReciboInvalido()
        {
            panelMensaje.Visibility = Visibility.Visible;
        }

        private bool ValidarCamposConfirmacion()
        {
            return txtRecibo.Text.ToUpper().Equals(recibo.IdRecibo + "-" + recibo.Serie) && txtIdentificacion.Text.ToUpper().Equals(recibo.IdOrdenPago==null?recibo.ReciboDatos.Identificador:recibo.OrdenPago.Identificador);
        }

        private void Finalizar()
        {
            Recibo.isOpening = true;
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            ReciboAnulado reciboAnulado = new ReciboAnulado() { IdRecibo = recibo.IdRecibo, Serie = recibo.Serie, Motivo = txtMotivo.Text };
            new ReciboViewModel().AnularRecibo(reciboAnulado);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Delete, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }
    }
}
