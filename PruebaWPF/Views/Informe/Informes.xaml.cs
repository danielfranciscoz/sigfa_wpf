using PruebaWPF.Model;
using PruebaWPF.UserControls;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private List<ReciboPago> pagos;

        private InformesViewModel controller;
        private Pantalla pantalla;
        ObjetoResumen recinto;
        ObjetoResumen caja;
        ObjetoResumen area;


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

      

        private void LoadTitle()
        {
            Bar_Back e = new Bar_Back
            {
                Value = pantalla.Titulo,
                AutoReload = false
            };
            this.layoutRoot.DataContext = e;
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
            cboArea.SelectedIndex = -1;
            cboRecinto.SelectedIndex = -1;
            cboCaja.SelectedIndex = -1;

            ObtenerInformacion();
        }

        private async void ObtenerInformacion(int? IdRecinto = null, string IdArea = null, int? IdCaja = null)
        {
            btnGenerar.IsEnabled = false;
            progressbar.Visibility = Visibility.Visible;
            DateTime? start = dtInicio.SelectedDate;
            DateTime? end = dtFin.SelectedDate;

            Tuple<List<Model.Recibo>,
                 List<ObjetoResumen>,
                 List<ObjetoResumen>,
                 List<ObjetoResumen>,
                 List<ObjetoResumen>,
                 List<ObjetoResumen>,
                 Tuple<List<ReciboPago>>
                 > datos = await FindAsync(start, end, IdRecinto, IdArea, IdCaja);

            List<Model.Recibo> recibos = datos.Item1;
            List<ObjetoResumen> recintosCount = datos.Item2;
            List<ObjetoResumen> AreasCount = datos.Item3;

            List<ObjetoResumen> recintosMoney = datos.Item4;
            List<ObjetoResumen> areasMoney = datos.Item5;
            List<ObjetoResumen> formapagoMoney = datos.Item6;

            Tuple<List<ObjetoResumen>, List<ObjetoResumen>> summaries = CalculateSummary(recintosCount, recintosMoney);
            List<ObjetoResumen> countresumen = summaries.Item1;
            List<ObjetoResumen> moneyresumen = summaries.Item2;

            List<ReciboPago> pago = datos.Item7.Item1;

            tblRecintosCount.ItemsSource = recintosCount;
            tblAreasCount.ItemsSource = AreasCount;

            tblRecintosMoney.ItemsSource = recintosMoney;
            tblAreasMoney.ItemsSource = areasMoney;
            tblFormaPagoMoney.ItemsSource = formapagoMoney;
            tblReciboMoney.ItemsSource = recibos;

            tblRecintosResumen.ItemsSource = countresumen;
            tblMoneyResumen.ItemsSource = moneyresumen;

            CargarRecintos(recintosCount);
            CargarAreas(AreasCount);
            CargarCajas(recintosMoney);


            this.pagos = pago;
            btnGenerar.IsEnabled = true;
            progressbar.Visibility = Visibility.Hidden;
        }

        private Tuple<List<ObjetoResumen>, List<ObjetoResumen>> CalculateSummary(List<ObjetoResumen> recintosCount, List<ObjetoResumen> recintosMoney)
        {

            var CountResumen = recintosCount.GroupBy(g => g.Name).Select(s => new ObjetoResumen
            {
                Name = s.Key,
                Count = s.Sum(s1 => s1.Count)
            }).ToList();


            var MoneyResumen = recintosMoney.GroupBy(g => g.Coin).Select(s => new ObjetoResumen
            {
                Name = s.Key,
                Total = s.Sum(s1 => s1.Total)
            }).ToList();

            return new Tuple<List<ObjetoResumen>, List<ObjetoResumen>>(CountResumen, MoneyResumen);
        }

        private void CargarRecintos(List<ObjetoResumen> recinto)
        {

            if (cboRecinto.SelectedIndex == -1 || cboRecinto.SelectedValue == null)
            {
                if (cboRecinto.ItemsSource != null)
                {
                    cboRecinto.ItemsSource = null;
                }
                cboRecinto.ItemsSource = recinto.Select(s => new { s.IdInt, s.Name }).Distinct().OrderBy(o => o.Name).Select(s => new ObjetoResumen() { IdInt = s.IdInt, Name = s.Name });
            }
            else
            {
                var items = (ObjetoResumen)cboRecinto.SelectedItem;
                List<ObjetoResumen> nuevo = new List<ObjetoResumen>();
                nuevo.Add(items);
                nuevo.Add(new ObjetoResumen() { Name = "Limpiar Filtro", IdInt = null });
                cboRecinto.ItemsSource = nuevo;

            }
        }

        private void CargarAreas(List<ObjetoResumen> areas)
        {
            if (cboArea.SelectedIndex == -1 || cboArea.SelectedValue == null)
            {
                if (cboArea.ItemsSource != null)
                {
                    cboArea.ItemsSource = null;
                }
                cboArea.ItemsSource = areas.Select(s => new { s.IdString, s.Name }).Distinct().OrderBy(o => o.Name).Select(s => new ObjetoResumen() { IdString = s.IdString, Name = s.Name });
            }
            else
            {
                var items = (ObjetoResumen)cboArea.SelectedItem;
                List<ObjetoResumen> nuevo = new List<ObjetoResumen>();
                nuevo.Add(items);
                nuevo.Add(new ObjetoResumen() { Name = "Limpiar Filtro", IdString = null });
                cboArea.ItemsSource = nuevo;
            }
        }

        private void CargarCajas(List<ObjetoResumen> cajas)
        {
            if (cboCaja.SelectedIndex == -1 || cboCaja.SelectedValue == null)
            {
                if (cboCaja.ItemsSource != null)
                {
                    cboCaja.ItemsSource = null;
                }
                cboCaja.ItemsSource = cajas.Select(s => new { s.IdInt, s.SecondName }).Distinct().OrderBy(o => o.SecondName).Select(s => new ObjetoResumen() { IdInt = s.IdInt, Name = s.SecondName });
            }
            else
            {
                var items = (ObjetoResumen)cboCaja.SelectedItem;
                List<ObjetoResumen> nuevo = new List<ObjetoResumen>();
                nuevo.Add(items);
                nuevo.Add(new ObjetoResumen() { Name = "Limpiar Filtro", IdInt = null });
                cboCaja.ItemsSource = nuevo;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            rptInforme cierre = new rptInforme(pagos, new ColumnasFiltro()
            {
                Area = ((ObjetoResumen)cboArea.SelectedItem)?.Name.ToUpper(),
                Caja = ((ObjetoResumen)cboCaja.SelectedItem)?.Name.ToUpper(),
                Recinto = ((ObjetoResumen)cboRecinto.SelectedItem)?.Name.ToUpper(),
                startdate = dtInicio.SelectedDate,
                enddate = dtFin.SelectedDate
            });
            cierre.ShowDialog();
        }

        private void CboRecinto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            recinto = (ObjetoResumen)cboRecinto.SelectedItem;
            if (recinto != null)
            {
                ObtenerInformacion(IdCaja: caja?.IdInt, IdRecinto: recinto?.IdInt, IdArea: area?.IdString);
            }
        }

        private void CboArea_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            area = (ObjetoResumen)cboArea.SelectedItem;
            if (area != null)
            {
                ObtenerInformacion(IdCaja: caja?.IdInt, IdRecinto: recinto?.IdInt, IdArea: area?.IdString);
            }
        }

        private void CboCaja_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            caja = (ObjetoResumen)cboCaja.SelectedItem;
            if (caja != null)
            {
                ObtenerInformacion(IdCaja: caja?.IdInt, IdRecinto: recinto?.IdInt, IdArea: area?.IdString);
            }
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadTitle();
        }

        private Task<Tuple<List<Model.Recibo>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, List<ObjetoResumen>, Tuple<List<ReciboPago>>>> FindAsync(DateTime? start, DateTime? end, int? IdRecinto = null, string IdArea = null, int? IdCaja = null)
        {
            return Task.Run(() =>
            {
                return controller.InformeIngresos(start, end, IdRecinto, IdArea, IdCaja);
            });

        }
    }
}

