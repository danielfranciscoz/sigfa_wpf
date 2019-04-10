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

        private ObservableCollection<Recibo1> recibos;

        public ArquearApertura()
        {

            InitializeComponent();
        }

        public ArquearApertura(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            controller = new ArqueoViewModel(pantalla);
            operacion = new Operacion();
            recibos = new ObservableCollection<Recibo1>();

            InitializeComponent();
            Diseñar();
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
                DetAperturaCaja apertura = controller.DetectarApertura();
                datosIniciales.DataContext = apertura;
                lblRecuento.DataContext = apertura;

                lstRecibos.ItemsSource = recibos;
            }
            catch (Exception ex)
            {
                panelInfo.Visibility = Visibility.Collapsed;

                lblErrorMesaje.Text = ex.Message;
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

            if (codigo.Contains("-"))
            {

                Recibo1 r = new Recibo1();
                String[] valores = codigo.Split('-');

                r.IdRecibo = int.Parse(valores[0]);
                r.Serie = valores[1];

                r.regAnulado = new Random().Next(100) <= 50 ? true : false;
                recibos.Insert(0,r);

                panelErrorRecibo.Visibility = Visibility.Hidden;
            }
            else
            {
                lblErrorRecibo.Text = "El código de recibo ingresado no es válido, por favor asegúrese de ingresar [No.Recibo]-[Serie]. No use corchetes y recuerde el guión de separación";
                panelErrorRecibo.Visibility = Visibility.Visible;
            }
        }
    }
}
