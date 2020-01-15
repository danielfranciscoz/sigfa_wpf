using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para AsientoRecibo.xaml
    /// </summary>
    public partial class AsientoRecibo : Window
    {
        private ReciboSon recibo;
        private List<Asiento> asientos;
        public AsientoRecibo()
        {
            InitializeComponent();
        }

        public AsientoRecibo(ReciboSon recibo)
        {
            this.recibo = recibo;
            InitializeComponent();
            Diseñar();
        }

        private void Diseñar()
        {
            clsUtilidades.Dialog_ModalDesign(this);
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            asientos = new ReciboViewModel().FindAsientoRecibo(recibo);
            lstAsiento.ItemsSource = asientos;
            txtTitle.DataContext = recibo;
            CalcularTotales();
        }

        private void CalcularTotales()
        {
            var sumas = asientos.GroupBy(g => 0).Select(s => new { Debe = s.Sum(c => c.Debe ?? 0), Haber = s.Sum(c => c.Haber ?? 0) }).ToArray();
            if (sumas.Length != 0)
            {
                txtDebe.Text = Math.Round(double.Parse(sumas[0]?.Debe.ToString()), 2, MidpointRounding.ToEven).ToString("0,0.00");
                txtHaber.Text = Math.Round(double.Parse(sumas[0]?.Haber.ToString()), 2, MidpointRounding.ToEven).ToString("0,0.00");
            }
        }
    }
}
