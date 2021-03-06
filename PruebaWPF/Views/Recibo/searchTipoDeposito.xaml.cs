﻿using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.AgenteExterno;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace PruebaWPF.Views.Recibo
{
    /// <summary>
    /// Lógica de interacción para searchTipoDeposito.xaml
    /// </summary>
    public partial class searchTipoDeposito : Window
    {
        private int TipoDeposito;
        private int IdTipoArancel;

        private ObservableCollection<fn_ConsultarInfoExterna_Result> items { get; set; }
        private SearchTipoDepositoViewModel controller;

        public fn_ConsultarInfoExterna_Result SelectedResult = null;
        private bool Busqueda = false;

        public searchTipoDeposito()
        {
            InitializeComponent();
            Inicializar();
        }


        public searchTipoDeposito(int TipoDeposito, string Tipo, int IdTipoArancel)
        {
            this.TipoDeposito = TipoDeposito;
            this.IdTipoArancel = IdTipoArancel;

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
            if (tblData.SelectedItem != null)
            {
                Seleccionar();
            }
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
                    CambiarHeaderTabla(new String[] { "No.Trabajador", "Trabajador", "Código", "Area" });
                    break;
                case 3:
                    CambiarHeaderTabla(new String[] { "Identificación", "Agente Externo", "Procedencia" });
                    break;
                case 4:
                    CambiarHeaderTabla(new String[] { "RUC", "Razón Social", "Nombre Comercial" });
                    break;
                case 5:
                    CambiarHeaderTabla(new String[] { "Identificación", "Nombre", "Primera Opción" });
                    break;
                case 6:
                    CambiarHeaderTabla(new String[] { "Código", "Área", "Ubicación" });
                    break;
                case 7:
                    CambiarHeaderTabla(new String[] { "Carnet", "Estudiante", "Estudio","Tipo de Estudio" });
                    break;

            }

        }
        private void IdentificarBusqueda()
        {
            switch (TipoDeposito)
            {
                case 1://Estudiante
                    if (ValidarCampos(new TextBox[] { txtCarnet, txtApellidos }))
                    {
                        BuscarInformacion(txtCarnet.Text, txtApellidos.Text);
                    }
                    break;
                case 2://Trabajador
                    if (ValidarCampos(new TextBox[] { txtNoInterno, txtTrabajador }))
                    {
                        BuscarInformacion(txtNoInterno.Text, txtTrabajador.Text);
                    }
                    break;
                case 3://Agente Externo
                    if (ValidarCampos(new TextBox[] { txtCedula, txtNombre }))
                    {
                        BuscarInformacion(txtCedula.Text, txtNombre.Text);
                        Busqueda = true;
                    }
                    break;

                case 4://Proveedor
                    if (ValidarCampos(new TextBox[] { txtRUC, txtProveedor }))
                    {
                        BuscarInformacion(txtRUC.Text, txtProveedor.Text);
                    }
                    break;

                case 5://Candidato
                    if (ValidarCampos(new TextBox[] { txtPrematricula, txtCandidato }))
                    {
                        BuscarInformacion(txtPrematricula.Text, txtCandidato.Text);
                    }
                    break;
                case 6://Area
                    if (ValidarCampos(new TextBox[] { txtArea, txtCodArea }))
                    {
                        BuscarInformacion(txtCodArea.Text, txtArea.Text);
                    }
                    break;

                case 7://Estudiante Modalidad Especial
                    if (ValidarCampos(new TextBox[] { txtCarnetME, txtApellidosME}))
                    {
                        BuscarInformacion(txtCarnetME.Text, txtApellidosME.Text);
                    }
                    break;

            }

        }

        private bool ValidarCampos(TextBox[] campos)
        {
            bool flag = true;
            int contador = 0;
            for (int i = 0; i < campos.Length; i++)
            {
                if (string.IsNullOrWhiteSpace(campos[i].Text))
                {
                    contador++;
                }
            }
            if (contador == campos.Length)
            {
                flag = false;
            }
            return flag;
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
                var informacion = controller.ObtenerTipoDeposito(TipoDeposito, Criterio, false, texto, clsConfiguration.Actual().TopRow,true, IdTipoArancel);
                items = new ObservableCollection<fn_ConsultarInfoExterna_Result>(informacion);

                if (informacion.Any())
                {
                HeaderIdentificador.Header = informacion.First().IdentificatorType.ToString();
                }
                else
                {
                HeaderIdentificador.Header = "";
                }
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
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = clsReferencias.MESSAGE_NoSelection, OperationType = clsReferencias.TYPE_MESSAGE_Advertencia }, this);
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

        private void btnAddAgente_Click(object sender, RoutedEventArgs e)
        {
            if (Busqueda)
            {
                try
                {
                    Pantalla pantalla = new PantallaViewModel().FindById(new AgenteExterno.AgenteExterno().Uid);

                    if (controller.AuthorizePantallaIncrustada(pantalla, ((Button)sender).Tag.ToString()))
                    {
                        GestionarAgenteExterno ga = new GestionarAgenteExterno(pantalla, btnAddAgente.Tag.ToString());
                        ga.ShowDialog();
                    }
                }
                catch (Exception ex)
                {
                    clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error }, this);
                }
            }
            else
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = "¡Espera un momento!\n\nAntes de agregar un nuevo registro, por favor asegúrate que este no existe mediante una breve búsqueda.\nLos registros duplicados no permiten obtener información adecuada para análisis.", OperationType = clsReferencias.TYPE_MESSAGE_Wait_a_Moment }, this);
            }
        }

        private void RbPrematricula_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtCandidato, txtPrematricula);
        }
        private void RbCandidato_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtCandidato, txtPrematricula);
        }

        private void TxtPrematricula_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtCandidato, rbPrematricula);
        }


        private void TxtCandidato_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtPrematricula, rbCandidato);
        }

        private void RbCodArea_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtArea, txtCodArea);
        }

        private void TxtCodArea_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtArea, rbCodArea);
        }

        private void RbArea_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtArea, txtCodArea);

        }

        private void TxtArea_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtCodArea, rbArea);
        }

        private void RbCarnetME_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(true, txtApellidosME, txtCarnetME);
        }

        private void TxtCarnetME_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtApellidosME, rbCarnetME);
        }

        private void RbApellidosME_Checked(object sender, RoutedEventArgs e)
        {
            setChecked(false, txtApellidosME, txtCarnetME);
        }

        private void TxtApellidosME_KeyDown(object sender, KeyEventArgs e)
        {
            VerificarEnter(e);
            CambiarRadioButton(txtCarnetME, rbApellidosME);
        }
    }
}
