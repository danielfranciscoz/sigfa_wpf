using PruebaWPF.ViewModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using System.Collections.Specialized;
using PruebaWPF.Helper;
using PruebaWPF.Clases;
using PruebaWPF.Referencias;
using PruebaWPF.Views.Main;

namespace PruebaWPF.Views.VariacionCambiaria
{
    /// <summary>
    /// Lógica de interacción para GestionarVariacionCambiaria.xaml
    /// </summary>
    public partial class GestionarVariacionCambiaria : Window
    {
        private VariacionCambiariaViewModel controller;
        private ObservableCollection<VariacionCambiariaSon> items;
        private Operacion operacion;

        private double MaxTableHeight = 0;
        public GestionarVariacionCambiaria()
        {
            controller = new VariacionCambiariaViewModel();
            InitializeComponent();

            this.Owner = frmMain.principal;
            CargarMonedas();
            ActivarValidadorCampos();
            items = new ObservableCollection<VariacionCambiariaSon>();
            items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(items_CollectionChanged);
            tblVariaciones.ItemsSource = items;
            Diseñar();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_Perfomance(this);
        }

        private void ActivarValidadorCampos()
        {
            clsValidateInput.Validate(txtAño, clsValidateInput.OnlyNumber);
            clsValidateInput.Validate(txtMonto, clsValidateInput.DecimalNumber);
        }

        private void items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == System.Collections.Specialized.NotifyCollectionChangedAction.Remove)
            {
                items.Remove((VariacionCambiariaSon)tblVariaciones.CurrentItem);
            }
            lblCantidadRegistros.Text = items.Count.ToString();
        }

        private void CargarMonedas()
        {
            var monedas = controller.ObtenerMonedas();
            cboMonedaDia.ItemsSource = monedas;
            cboMonedaPeriodo.ItemsSource = monedas;
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (items.Any())
            {
                clsutilidades.OpenMessage(Guardar());
            }
            else
            {
                operacion = new Operacion();
                operacion.Mensaje = clsReferencias.MESSAGE_Cero_Registro;
                operacion.OperationType = clsReferencias.TYPE_MESSAGE_Advertencia;
                clsutilidades.OpenMessage(operacion);
            }
        }

        private Operacion Guardar()
        {
            Operacion o = new Operacion();
            try
            {
                int conteo = controller.Guardar(items.ToList());
                if (conteo == 0)
                {
                    o.Mensaje = clsReferencias.MESSAGE_Cero_Save;
                    o.OperationType = clsReferencias.TYPE_MESSAGE_Advertencia;
                }
                else
                {
                    o.Mensaje = clsReferencias.MESSAGE_Exito_Save_COUNT + conteo;
                    o.OperationType = clsReferencias.TYPE_MESSAGE_Exito;
                    RefrescarTablaIndex();
                }
                CleanList();
            }
            catch (Exception ex)
            {
                o.Mensaje = new clsException(ex).ErrorMessage();
                o.OperationType = clsReferencias.TYPE_MESSAGE_Error;
            }
            return o;
        }

        private void RefrescarTablaIndex()
        {
            VariacionCambiaria.Cambios = true;
            frmMain.Refrescar();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            items.Remove((VariacionCambiariaSon)tblVariaciones.CurrentItem);
        }

        private void btnAddMonth_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCamposMonth())
                {
                    AddTable(controller.GetTCPeriodoBCN(int.Parse(txtAño.Text), int.Parse(cboMesPeriodo.SelectedValue.ToString()), (Model.Moneda)cboMonedaPeriodo.SelectedItem).Where(w => items.All(a => a.Fecha != w.Fecha || a.Moneda != w.Moneda)).ToList());
                    CallPanelError(Visibility.Collapsed);
                }
                else
                {
                    CallPanelError(Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                CallPanelError(new clsException(ex).ErrorMessage());
            }
        }

        private void btnAddDay_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCamposDay())
                {
                    if (isBCN.IsChecked.Value)
                    {
                        AddTable(controller.GetTCDiaBCN(txtFecha.SelectedDate.Value, (Model.Moneda)cboMonedaDia.SelectedItem));
                    }
                    else
                    {
                        AddTable(new VariacionCambiariaSon { Fecha = txtFecha.SelectedDate.Value, Moneda = (Model.Moneda)cboMonedaDia.SelectedItem, Valor = Decimal.Parse(txtMonto.Text), LoginCreacion = clsSessionHelper.usuario.Login });
                    }
                    CallPanelError(Visibility.Collapsed);
                }
                else
                {
                    CallPanelError(Visibility.Visible);
                }
            }
            catch (Exception ex)
            {
                CallPanelError(ex.Message);
            }
        }

        private bool ValidarCamposMonth()
        {
            txtError.Text = "";
            Boolean flag = true;

            if (txtAño.Text.Equals(""))
            {
                DescripcionErrores("Año", "Debe Ingresar el año para obtener la información.");
                flag = false;
            }
            if (cboMesPeriodo.SelectedIndex == -1)
            {
                DescripcionErrores("Mes", "Debe seleccionar el mes para obtener la información.");
                flag = false;
            }
            if (cboMonedaPeriodo.SelectedIndex == -1)
            {
                DescripcionErrores("Moneda", "Debe seleccionar una moneda.");
                flag = false;
            }

            return flag;
        }

        private bool ValidarCamposDay()
        {
            txtError.Text = "";
            Boolean flag = true;

            if (txtFecha.Text.Equals(""))
            {
                DescripcionErrores("Fecha", clsReferencias.SinFecha);
                flag = false;
            }
            else
            {
                if (items.Any(a => a.Fecha == txtFecha.SelectedDate.Value && a.Moneda.IdMoneda == int.Parse(cboMonedaDia.SelectedValue.ToString())))
                {
                    DescripcionErrores("Fecha", "No se puede ingresar una fecha con la misma moneda.");
                    flag = false;
                }
            }
            if (!isBCN.IsChecked.Value)
            {
                if (txtMonto.Text.Equals(""))
                {
                    DescripcionErrores("Monto", "Debe digitar el monto al cual asciende la variación.");
                    flag = false;
                }
                else
                {
                    double value = 0.00;
                    if (!double.TryParse(txtMonto.Text, out value))
                    {
                        DescripcionErrores("Monto", clsReferencias.NumeroMal);
                    }
                }
            }
            if (cboMonedaDia.SelectedIndex == -1)
            {
                DescripcionErrores("Moneda", "Debe seleccionar una moneda.");
                flag = false;
            }
            return flag;

        }

        private void DescripcionErrores(string v1, string v2)
        {
            txtError.Inlines.Add("Error en el Campo: ");
            txtError.Inlines.Add(new Bold(new Run(v1)));
            txtError.Inlines.Add(", descripción: ");
            txtError.Inlines.Add(new Italic(new Run(v2)));
            txtError.Inlines.Add(Environment.NewLine);
        }

        private void CallPanelError(Visibility visible)
        {
            panelError.Visibility = visible;
            if (visible == Visibility.Visible)
            {
                tblVariaciones.MaxHeight = MaxTableHeight - panelError.Height;
            }
            else
            {
                tblVariaciones.MaxHeight = MaxTableHeight;
            }
        }

        private void CallPanelError(string mensaje)
        {
            txtError.Text = "Error: ";
            txtError.Inlines.Add(new Italic(new Run(mensaje)));
            CallPanelError(Visibility.Visible);
        }

        private void AddTable(VariacionCambiariaSon v)
        {
            items.Add(v);
        }

        private void AddTable(List<VariacionCambiariaSon> v)
        {
            foreach (VariacionCambiariaSon item in v)
            {
                items.Add(item);
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaxTableHeight = panel_contenedorTbl.ActualHeight;
            tblVariaciones.MaxHeight = MaxTableHeight;
        }

        private void CleanList()
        {
            items.Clear();
        }
    }
}
