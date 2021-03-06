﻿using PruebaWPF.Clases;
using PruebaWPF.Helper;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Windows;
using System.Windows.Controls;

namespace PruebaWPF.Views.Tesoreria
{
    /// <summary>
    /// Lógica de interacción para GestionarCaja.xaml
    /// </summary>
    public partial class GestionarCaja : Window
    {
        TesoreriaViewModel controller;
        Pantalla pantalla;
        CajaSon caja;
        clsValidateInput validate = new clsValidateInput();
        public GestionarCaja()
        {
            InitializeComponent();
            Diseñar();
        }

        public GestionarCaja(Pantalla pantalla, String PermisoName)
        {
            caja = new CajaSon();
            this.pantalla = pantalla;

            InitializeComponent();

            Inicializar(PermisoName);

        }

        public GestionarCaja(CajaSon caja, Pantalla pantalla, String PermisoName)
        {
            this.caja = caja;
            this.pantalla = pantalla;
            InitializeComponent();

            Inicializar(PermisoName);
        }

        private void Inicializar(String PermisoName)
        {
            controller = new TesoreriaViewModel(pantalla);
            Diseñar();
            CargarCombo(PermisoName);
            DataContext = caja;
            CamposNormales();
        }

        private void CamposNormales()
        {
            validate.AsignarBorderNormal(new Control[] { txtMAC, txtNombre, cboRecinto, cboSerie });
        }

        private bool ValidarCampos()
        {
            return clsValidateInput.ValidateALL(new Control[] { txtMAC, txtNombre, cboRecinto, cboSerie });
        }
        private void CargarCombo(string PermisoName)
        {
            cboRecinto.ItemsSource = controller.Recintos(PermisoName);
            cboRecinto.SelectedValue = caja.IdRecinto;

            cboSerie.ItemsSource = controller.FindAddSeries(caja.IdSerie);
            cboSerie.SelectedValue = caja.IdSerie;

        }

        private void Diseñar()
        {
            if (caja.IdCaja > 0)
            {
                btnSave.Visibility = btnEdit.Visibility;
                btnEdit.Visibility = Visibility.Visible;
                txtTitle.Text = "Editar Caja";
            }
            clsUtilidades.Dialog_ModalDesign(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {

        }
        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidarCampos())
                {
                    if (validarMac())
                    {
                        caja.IdRecinto = int.Parse(cboRecinto.SelectedValue.ToString());
                        caja.IdSerie = cboSerie.SelectedValue.ToString();
                        if (controller.Authorize_Recinto(((Button)sender).Tag.ToString(), caja.IdRecinto))
                        {
                            clsUtilidades.OpenMessage(Guardar(), this);
                            Finalizar();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                clsUtilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private bool validarMac()
        {
            Boolean flag = true;

            if (caja.MAC.Length != txtMAC.MaxLength)
            {
                clsValidateInput.ActivateBorderError(txtMAC);
                flag = false;
            }
            return flag;
        }

        private void Finalizar()
        {
            Tesoreria.Cambios = true;
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            controller.SaveUpdateCaja(caja);
            return new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };
        }


        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (caja.IdCaja > 0)
            {
                bool MacEditable = controller.VeriricarAperturasArquedas(caja);
                txtMAC.IsEnabled = MacEditable;
                if (!MacEditable)
                {
                    panelMAC.Visibility = Visibility.Collapsed;
                    lblInfo.Visibility = Visibility.Visible;
                }
            }
            lblMac.Text = clsSessionHelper.MACMemory;
        }

        private void btnEstablecer_Click(object sender, RoutedEventArgs e)
        {
            caja.MAC = lblMac.Text;
            clsUtilidades.UpdateControl(txtMAC);


        }
    }
}
