using PruebaWPF.Model;
using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace PruebaWPF.Views.Informe
{
    /// <summary>
    /// Lógica de interacción para Informes.xaml
    /// </summary>
    public partial class Informes : Page
    {
        double starHeight = 0;
        double endHeight = 0;

        private InformesViewModel controller;
        private Pantalla pantalla;


        public Informes()
        {
            InitializeComponent();
        }

        public Informes(Pantalla pantalla)
        {
            this.pantalla = pantalla;
            controller = new InformesViewModel();
            //operacion = new Operacion();

            InitializeComponent();
            starHeight = panelResumen.ActualHeight;
        }

        private void SeeFilters_Checked(object sender, RoutedEventArgs e)
        {
            if (starHeight != 0 && endHeight != 0)
            {
                if (seeFilters.IsChecked.Value)
                {
                    panelResumen.Height = starHeight;
                }
                else
                {
                    panelResumen.Height = endHeight;
                }

            }
        }

        private void PanelFiltros_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (starHeight == 0 || endHeight == 0)
            {
                if (panelResumen?.ActualHeight != null)
                {
                    if (starHeight != panelResumen.ActualHeight && starHeight != 0)
                    {
                        endHeight = panelResumen.ActualHeight;
                    }
                    else
                    {
                        starHeight = panelResumen.ActualHeight;

                    }
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ObtenerInformacion();
        }

        private void ObtenerInformacion()
        {
            Tuple<List<Recibo1>,List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>> datos = controller.InformeIngresos();
            List<ObjetoResumen> recintosCount = datos.Item2;
            List<ObjetoResumen> cajasCount = datos.Item3;
            List<ObjetoResumen> areasCount = datos.Item4;

            lstRecintosCount.ItemsSource = recintosCount;
            lstCajasCount.ItemsSource = cajasCount;
            lstAreasCount.ItemsSource = areasCount;
        }
    }
}
