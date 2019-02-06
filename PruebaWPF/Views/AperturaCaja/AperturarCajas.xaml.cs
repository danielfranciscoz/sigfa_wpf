using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
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

namespace PruebaWPF.Views.AperturaCaja
{
    /// <summary>
    /// Lógica de interacción para AperturarCajas.xaml
    /// </summary>
    public partial class AperturarCajas : Window
    {
        AperturaCajaViewModel controller;
        private Model.AperturaCaja apertura;
        private ObservableCollection<Caja> cajas;
        private ObservableCollection<Caja> cajas_aperturar;

        public AperturarCajas()
        {
            InitializeComponent();
        }

        public AperturarCajas(Pantalla pantalla, string permisoName)
        {
            controller = new AperturaCajaViewModel(pantalla);
            apertura = controller.InicializarAperturaCaja();

            InitializeComponent();

            Diseñar();
            CargarRecintos(permisoName);
            DataContext = apertura;
            cajas_aperturar = new ObservableCollection<Caja>();
            lstAperturar.ItemsSource = cajas_aperturar;
        }
        private void Diseñar()
        {
            clsutilidades.Dialog_ModalDesign(this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (cajas_aperturar.Count == 0)
                {
                    clsutilidades.OpenMessage(new Operacion() { Mensaje = "Debe especificar las cajas que serán aperturadas", OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                }
                else
                {
                    if (cajas.Count > 0)
                    {
                        if (clsutilidades.OpenDeleteQuestionMessage("No serán aperturadas todas las cajas de este recinto, ¿está seguro que desea continuar?"))
                        {
                            clsutilidades.OpenMessage(Guardar(), this);
                            Finalizar();
                        }
                    }
                    else
                    {
                        clsutilidades.OpenMessage(Guardar(), this);
                        Finalizar();
                    }
                }

            }
            catch (Exception ex)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = new clsException(ex).ErrorMessage(), OperationType = clsReferencias.TYPE_MESSAGE_Error });
            }
        }

        private void Finalizar()
        {
            AperturaCaja.Cambios = true;
            frmMain.Refrescar();
            Close();
        }

        private Operacion Guardar()
        {
            Operacion o;

            controller.Guardar(apertura, cajas_aperturar.ToList());
            o = new Operacion { Mensaje = clsReferencias.MESSAGE_Exito_Save, OperationType = clsReferencias.TYPE_MESSAGE_Exito };

            return o;
        }


        private void CargarRecintos(string PermisoName)
        {
            List<vw_RecintosRH> recintos = controller.Recintos(PermisoName);
            cboRecinto.ItemsSource = recintos;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (cboRecinto.Items.Count == 0)
            {
                clsutilidades.OpenMessage(new Operacion() { Mensaje = "La apertura no se encuentra disponible debido a que no se han encontrado recintos a los cuales usted tenga permiso de aperturar cajas, esta información es basada en sus permisos de usuario.", OperationType = clsReferencias.TYPE_MESSAGE_Advertencia });
                Close();
            }
        }

        private void cboRecinto_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cboRecinto.SelectedIndex != -1)
            {
                int IdRecinto = int.Parse(cboRecinto.SelectedValue.ToString());
                CargarListaCajas(IdRecinto);
                apertura.IdRecinto = IdRecinto;
            }
        }

        private void CargarListaCajas(int IdRecinto)
        {
            cajas = new ObservableCollection<Caja>(controller.FindCajas(IdRecinto));
            lstCajas.ItemsSource = cajas;

            while (cajas_aperturar.Count > 0)
            {
                cajas_aperturar.RemoveAt(0);
            }
        }

        private void btnAllCajas_Click(object sender, RoutedEventArgs e)
        {
            CargarListaAperturas();
        }

        private void CargarListaAperturas()
        {
            AddRemoveAll(cajas_aperturar, cajas);
        }

        private void btnAddCaja_Click(object sender, RoutedEventArgs e)
        {
            if (lstCajas.SelectedIndex != -1)
            {
                Caja c = (Caja)lstCajas.SelectedItem;
                cajas_aperturar.Add(c);
                cajas.Remove(c);

            }
        }

        private void btnRemoveCaja_Click(object sender, RoutedEventArgs e)
        {
            if (lstAperturar.SelectedIndex != -1)
            {
                Caja c = (Caja)lstAperturar.SelectedItem;
                cajas.Add(c);
                cajas_aperturar.Remove(c);

            }
        }

        private void btnRemoveAllCajas_Click(object sender, RoutedEventArgs e)
        {
            AddRemoveAll(cajas, cajas_aperturar);
        }

        private void AddRemoveAll(ObservableCollection<Caja> agregar, ObservableCollection<Caja> quitar)
        {

            if (quitar.Count > 0)
            {
                foreach (var item in quitar)
                {
                    agregar.Add(item);
                }
                while (quitar.Count > 0)
                {
                    quitar.RemoveAt(0);
                }
            }
        }

    }
}
