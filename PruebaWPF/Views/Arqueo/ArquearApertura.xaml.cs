using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
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

namespace PruebaWPF.Views.Arqueo
{
    /// <summary>
    /// Lógica de interacción para ArquearApertura.xaml
    /// </summary>
    public partial class ArquearApertura : Window
    {
        private ArqueoViewModel controller;
        private Operacion operacion;
        private Pantalla pantalla;
        private DetAperturaCaja apertura;
        private Model.Arqueo arqueo;

        private ObservableCollection<Recibo1> recibos;
        MaterialDesignExtensions.Controllers.StepperController tabmaterial;


        public ArquearApertura()
        {

            InitializeComponent();
        }

        public ArquearApertura(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            controller = new ArqueoViewModel(pantalla);
            operacion = new Operacion();
            InitializeComponent();
            Diseñar();
            tabmaterial = tabParent.Controller.ActiveStepViewModel.Controller;
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Object[] datos = controller.DetectarApertura();

                apertura = (DetAperturaCaja)datos[0];


                if (datos.Length > 1)
                {
                    panelWarningRecibo.Visibility = Visibility.Visible;
                    lblWarning.Text = "El proceso de arqueo se encuentra iniciado, continúe con las siguientes operaciones para finalizarlo.";
                    txtPleaseVerify.Visibility = Visibility.Collapsed;
                    btnArqueo.Content = "CONTINUAR ARQUEO";
                    arqueo = (Model.Arqueo)datos[1];
                }
                else
                {
                    arqueo = new Model.Arqueo();
                    arqueo.IdArqueoDetApertura = apertura.IdDetAperturaCaja;
                }


                datosIniciales.DataContext = apertura;
                lblRecuento.DataContext = apertura;


            }
            catch (Exception ex)
            {
                panelInfo.Visibility = Visibility.Collapsed;

                lblErrorMesaje.Text = new clsException(ex).ErrorMessage();
                panelMensaje.Visibility = Visibility.Visible;
            }

        }

        private void codrecibo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                AddRecibo();
            }
        }

        private void AddRecibo()
        {
            string codigo = codrecibo.Text;
            try
            {
                Recibo1 agregado = controller.ContabilizarRecibo(codigo, apertura);
                recibos.Insert(0, agregado);

                if (panelErrorRecibo.Visibility == Visibility.Visible)
                {
                    panelErrorRecibo.Visibility = Visibility.Hidden;
                }
            }
            catch (Exception ex)
            {
                lblErrorRecibo.Text = new clsException(ex).ErrorMessage();
                panelErrorRecibo.Visibility = Visibility.Visible; ;
            }

        }


        private void btnArqueo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (arqueo.UsuarioArqueador.Equals(""))
                {
                    controller.Guardar(arqueo);

                }
                CargarRecibosContabilizados();
                tabmaterial.Continue();
            }
            catch (Exception ex)
            {
                lblErrorMesaje.Text = new clsException(ex).ErrorMessage();
                panelMensaje.Visibility = Visibility.Visible;
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            tabmaterial.Back();
        }

        private void btnConteoRecibos_Click(object sender, RoutedEventArgs e)
        {
            tabmaterial.Continue();
        }

        private void CargarRecibosContabilizados()
        {
            if (recibos == null)
            {
                recibos = new ObservableCollection<Recibo1>(controller.FindRecibosContabilizados(arqueo));
                lstRecibos.ItemsSource = recibos;
            }


        }
    }
}

/*
   private void TabControlStepper_ContinueNavigation(object sender, MaterialDesignExtensions.Controls.StepperNavigationEventArgs args)
        {


            //int currentStep = controller.Number;
            //int? nextStep = controller.IsLastStep ? -1 : controller.Number + 1;
            //int previewStep = controller.IsFirstStep ? -1 : controller.Number - 1;

            //controller.Controller.Continue();
            //GuardarCambios(currentStep);

        }

        private void GuardarCambios(int currentStep)
        {
            switch (currentStep)
            {

                case 1:
                    {
                        try
                        {
                            controller.Guardar(arqueo);
                        }
                        catch (Exception ex)
                        {
                            lblErrorMesaje.Text = new clsException(ex).ErrorMessage();
                            panelMensaje.Visibility = Visibility.Visible;
                        }
                    }
                    break;
                case 2:
                    {

                    }
                    break;
                case 3:
                    {

                    }
                    break;

                default:
                    break;
            }
        }
     */
