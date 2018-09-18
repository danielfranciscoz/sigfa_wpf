using Confortex.Clases;
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
            clsutilidades.Dialog_ModalDesign(this);
            CamposNormales();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtMotivo });
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtMotivo });
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
                    clsutilidades.OpenMessage(Guardar(), this);
                    Finalizar();
                }
            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Finalizar()
        {
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            ReciboAnulado reciboAnulado= new ReciboAnulado() { IdRecibo=recibo.IdRecibo,Serie=recibo.Serie,Motivo=txtMotivo.Text};
            new ReciboViewModel().AnularRecibo(reciboAnulado);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Delete, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }
    }
}
