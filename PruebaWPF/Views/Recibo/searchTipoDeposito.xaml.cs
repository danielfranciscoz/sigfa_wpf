﻿using Confortex.Clases;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
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

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para searchTipoDeposito.xaml
    /// </summary>
    public partial class searchTipoDeposito : Window
    {
        private int TipoDeposito;

        private ObservableCollection<fn_ConsultarInfoExterna_Result> items { get; set; }
        private SearchTipoDepositoViewModel controller;

        public fn_ConsultarInfoExterna_Result SelectedResult = null;

        public searchTipoDeposito()
        {
            InitializeComponent();
            Inicializar();
            Diseñar();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_Perfomance(this);
        }

        public searchTipoDeposito(int TipoDeposito, string Tipo)
        {
            this.TipoDeposito = TipoDeposito;
            InitializeComponent();
            txtTitulo.Text = "Seleccionar Registro de " + Tipo;
            Inicializar();
        }

        private void Inicializar()
        {
            controller = new SearchTipoDepositoViewModel();
            items = new ObservableCollection<fn_ConsultarInfoExterna_Result>();
            CambiarCamposHeader();
        }

        private void CambiarCamposHeader()
        {
            //Selecciono la pestaña que corresponde al tipo de depósito
            foreach (TabItem item in tabControl.Items)
            {
                if (int.Parse(item.Tag.ToString()) == TipoDeposito)
                {
                    item.IsSelected = true;
                    break;
                }
            }

            //Asigno los encabezados a la tabla
            IdentificarCampos();
        }

        private void setChecked(Boolean isCriterio, TextBox texto, TextBox criterio)
        {
            if (texto != null && criterio != null)
            {
                if (isCriterio)
                {
                    texto.Text = "";
                    criterio.Focus();
                }
                else
                {
                    criterio.Text = "";
                    texto.Focus();
                }
            }
        }

        private void tblData_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Seleccionar();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush colorFondo = (SolidColorBrush)FindResource("CloseButton_Dark");
            btnClose.Background = new SolidColorBrush(colorFondo.Color);
            btnClose.Foreground = new SolidColorBrush(Colors.White);
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e)
        {
            Seleccionar();
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = new SolidColorBrush(Colors.Transparent);
            btnClose.Foreground = new SolidColorBrush(Colors.White);
        }

        private void ResizeGrid()
        {
            tblData.Height = panelGrid.ActualHeight;
        }

        private void ContarRegistros()
        {
            lblCantidadRegitros.Text = "" + tblData.Items.Count;
        }

        private void rbCarnet_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtApellidos, txtCarnet);
        }

        private void rbApellidos_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtApellidos, txtCarnet);
        }

        private void txtCarnet_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtApellidos, rbCarnet);
        }

        private void txtApellidos_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtCarnet, rbApellidos);

        }

        private void VerificarEnter(KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                IdentificarBusqueda();
            }
        }
        private void CambiarRadioButton(TextBox textbox, RadioButton radio)
        {

            if (!textbox.Text.Equals(""))
            {
                radio.IsChecked = true;
            }
            if (!radio.IsChecked.Value)
            {
                radio.IsChecked = true;
            }
        }
        private void IdentificarCampos()
        {
            switch (TipoDeposito)
            {
                case 1:
                    CambiarHeaderTabla(new String[] { "Carnet", "Estudiante", "Carrera", "Sede" });
                    break;
                case 2:
                    CambiarHeaderTabla(new String[] { "No.Interno", "Trabajador", "Código", "Área" });
                    break;
                case 3:
                    CambiarHeaderTabla(new String[] { "No.Registro", "Agente Externo", "RUC / Cédula", "Dirección" });
                    break;
                case 4:
                    CambiarHeaderTabla(new String[] { "RUC", "Razón Social", "Nombre Comercial", "Dirección" });
                    break;

            }

        }
        private void IdentificarBusqueda()
        {
            switch (TipoDeposito)
            {
                case 1:
                    BuscarInformacion(txtCarnet.Text, txtApellidos.Text);
                    break;
                case 2:
                    BuscarInformacion(txtNoInterno.Text, txtTrabajador.Text);

                    break;
                case 3:
                    BuscarInformacion(txtCedula.Text, txtNombre.Text);
                    break;

                case 4:
                    BuscarInformacion(txtRUC.Text, txtProveedor.Text);
                    break;

            }

        }
        private void CambiarHeaderTabla(String[] headers)
        {
            for (int i = 0; i < headers.Length; i++)
            {
                tblData.Columns[i].Header = headers[i];
            }
        }

        private void BuscarInformacion(string Criterio, string Texto)
        {
            try
            {
                string texto = Texto.Length > 0 ? "%" + Texto.Replace(' ', '%') + "%" : "";
                items = new ObservableCollection<fn_ConsultarInfoExterna_Result>(controller.ObtenerTipoDeposito(TipoDeposito, Criterio, false, texto, 1000));

                if (panelError.Visibility == Visibility.Visible)
                {
                    panelError.Visibility = Visibility.Collapsed;
                }
            }
            catch (Exception ex)
            {
                panelError.Visibility = Visibility.Visible;
                txtError.Text = new clsException(ex).ErrorMessage();
                items = null;
            }

            tblData.ItemsSource = items;
            ContarRegistros();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            IdentificarBusqueda();
        }

        private void Seleccionar()
        {
            if (tblData.SelectedItem != null)
            {
                SelectedResult = (fn_ConsultarInfoExterna_Result)tblData.SelectedItem;
                this.Close();
            }
            else
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
            }
        }

        private void txtTrabajador_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtNoInterno, rbTrabajador);
        }
        private void txtNoInterno_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtTrabajador, rbNoInterno);
        }

        private void rbTrabajador_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtTrabajador, txtNoInterno);
        }

        private void rbNoInterno_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtTrabajador, txtNoInterno);
        }

        private void txtCedula_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtNombre, rbCedula);
        }

        private void txtNombre_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtCedula, rbNombre);
        }

        private void rbCedula_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtNombre, txtCedula);
        }

        private void rbNombre_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtNombre, txtCedula);
        }

        private void txtRUC_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtProveedor, rbRUC);
        }

        private void txtProveedor_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtRUC, rbRazonSocial);
        }

        private void rbRUC_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtProveedor, txtRUC);
        }

        private void rbRazonSocial_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtProveedor, txtRUC);
        }
    }
}