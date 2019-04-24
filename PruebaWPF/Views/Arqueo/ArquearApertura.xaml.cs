using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
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
        private BindingList<ArqueoEfectivoSon> efectivo;
        private BindingList<DocumentosEfectivo> documentos;
        private System.Windows.Forms.BindingSource ArqueoEfectivoBindingSource = new System.Windows.Forms.BindingSource();
        private System.Windows.Forms.BindingSource DocumentosEfectivoBindingSource = new System.Windows.Forms.BindingSource();

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
            string codigo = txtcodrecibo.Text;
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
            if (efectivo == null)
            {
                CargarEfectivoContabilizado();
            }

            tabmaterial.Continue();
        }

        private void CargarRecibosContabilizados()
        {
            if (recibos == null)
            {
                recibos = new ObservableCollection<Recibo1>(controller.FindRecibosContabilizados(arqueo));
                recibos.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Recibos_CollectionChanged);
                lstRecibos.ItemsSource = recibos;
                VerificarConteoFinalizado();
            }


        }





        private void Recibos_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                VerificarConteoFinalizado();
            }
        }

        private void Efectivo_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Add)
            {
                tblEfectivo.Items.Refresh();
            }
        }



        private void VerificarConteoFinalizado()
        {

            if (recibos.Count == apertura.Recibo1.Count)
            {
                txtcodrecibo.IsEnabled = false;
                btnConteoRecibos.IsEnabled = true;
            }

        }

        private void btnConteoEfectivo_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                controller.GuardarEfectivo(efectivo.ToList(), arqueo);
                efectivo = null;
                CargarEfectivoContabilizado();

                if (documentos == null)
                {
                    CargarDocumentosEfectivo();
                }

                tabmaterial.Continue();
            }
            catch (Exception ex)
            {
                lblErrorEfectivo.Text = new clsException(ex).ErrorMessage();
                panelErrorEfectivo.Visibility = Visibility.Visible;
            }
        }

        private void CargarEfectivoContabilizado()
        {
            if (efectivo == null)
            {
                efectivo = new BindingList<ArqueoEfectivoSon>(controller.FindConteoEfectivo(apertura));
                ArqueoEfectivoBindingSource.DataSource = efectivo;

                tblEfectivo.ItemsSource = ArqueoEfectivoBindingSource;
                efectivo.ListChanged += new ListChangedEventHandler(Ef_CollectionChanged);
                CalcularTotales();
            }
        }

        private void CargarDocumentosEfectivo()
        {
            if (documentos == null)
            {
                documentos = new BindingList<DocumentosEfectivo>(controller.FindDocumentosEfectivo(apertura));
                DocumentosEfectivoBindingSource.DataSource = documentos;

                tblDocumentosEfectivo.ItemsSource = DocumentosEfectivoBindingSource;
                documentos.ListChanged += new ListChangedEventHandler(Doc_CollectionChanged);
                //CalcularTotales();
            }
        }

        private void Ef_CollectionChanged(object sender, ListChangedEventArgs e)
        {
            CalcularTotales();
            tblEfectivo.CommitEdit();
            tblEfectivo.Items.Refresh();
        }

        private void Doc_CollectionChanged(object sender, ListChangedEventArgs e)
        {

            tblEfectivo.CommitEdit();
            tblEfectivo.Items.Refresh();
        }

        private void CalcularTotales()
        {
            var total = efectivo.GroupBy(g => new { g.Moneda.Moneda1, g.Moneda.Simbolo }).Select(s1 => new { TotalMoneda = s1.Key.Moneda1, TotalSimbolo = s1.Key.Simbolo, TotalEfectivo = s1.Sum(s => s.Total) });

            lstTotales.ItemsSource = total;
        }

        private void btnConteoNoEfectivo_Click(object sender, RoutedEventArgs e)
        {
        }
    }

}