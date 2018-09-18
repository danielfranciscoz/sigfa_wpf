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

namespace PruebaWPF.Views.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para AddSerie.xaml
    /// </summary>
    public partial class AddSerie : Window
    {
        clsValidateInput validate = new clsValidateInput();
        Pantalla pantalla;
        SerieRecibo serie;
        public AddSerie()
        {
            InitializeComponent();
            Diseñar();
        }

        public AddSerie(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            InitializeComponent();
            Diseñar();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
            CamposNormales();
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

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtSerie });
        }
        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtSerie });
        }

        private Operacion Guardar()
        {
            serie = new SerieRecibo() { IdSerie = txtSerie.Text };
            new TesoreriaViewModel(pantalla).SaveSerie(serie);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }
    }
}
