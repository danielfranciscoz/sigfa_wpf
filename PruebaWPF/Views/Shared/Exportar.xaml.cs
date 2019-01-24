using Microsoft.Win32;
using PruebaWPF.Clases;
using PruebaWPF.Model;
using PruebaWPF.Referencias;
using PruebaWPF.ViewModel;
using PruebaWPF.Views.Main;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;



namespace PruebaWPF.Views.Shared
{
    /// <summary>
    /// Lógica de interacción para Exportar.xaml
    /// </summary>
    public partial class Exportar : Window
    {
        ExportaViewModel exporta;
        private ObservableCollection<ListaExporta> listado { get; set; }
        internal static Operacion Operacion { get; set; }

        private DataTable data;
        private string FileName;

        public Exportar(DataTable datatable)
        {
            InitializeComponent();
            this.Owner = frmMain.principal;
            this.data = datatable;
            Operacion = null;
            LoadColumns();
            Diseñar();
        }

        private void Diseñar()
        {
            clsutilidades.Dialog_Perfomance(this);
        }

        private void LoadColumns()
        {
            exporta = new ExportaViewModel(data);
            listado = exporta.FindChecks(false); //Agrego los checks sin ser checkeados por defecto
            lstColumnas.ItemsSource = listado;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            SolidColorBrush colorFondo = (SolidColorBrush)FindResource("CloseButton_Dark");
            btnClose.Background = new SolidColorBrush(colorFondo.Color);

        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            btnClose.Background = new SolidColorBrush(Colors.Transparent);
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (NoSelection())
            {
                SaveDialog();
            }
            else
            {
                Operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Advertencia, clsReferencias.MESSAGE_NoSelection);
                new BoxMessage(Operacion).ShowDialog();
                Operacion = null;
            }

        }

        private async void SaveDialog()
        {
            await OpenSaveDialogAsync();
        }

        private async Task OpenSaveDialogAsync()
        {
            SaveFileDialog save = new SaveFileDialog();
            save.Filter = "Archivo de Excel (*.xlsx)|*.xlsx";
            save.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            if (save.ShowDialog() == true)
            {
                FileName = save.FileName;
                await IniciarProceso();
            }

        }

        private void ExportToExcel(String ruta)
        {
            ObtenerColumnasExportar();

            Microsoft.Office.Interop.Excel._Application excel = new Microsoft.Office.Interop.Excel.Application();

            Microsoft.Office.Interop.Excel._Workbook workbook = excel.Workbooks.Add(Type.Missing);

            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;

            try
            {

                worksheet = workbook.ActiveSheet;
                worksheet.Name = "Registros";

                int ColumnIndex = 1;

                System.Drawing.Color primary = System.Drawing.Color.FromArgb(78, 129, 189);
                System.Drawing.Color background1 = System.Drawing.Color.FromArgb(184, 204, 228);
                System.Drawing.Color background2 = System.Drawing.Color.FromArgb(220, 230, 241);

                foreach (DataColumn item in data.Columns)
                {
                    //Agrego el encabezado
                    worksheet.Cells[1, ColumnIndex] = item.Caption;

                    //Estilo de la celda
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, ColumnIndex]).Font.Bold = true;
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, ColumnIndex]).Interior.Color = System.Drawing.ColorTranslator.ToOle(primary);
                    ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[1, ColumnIndex]).Font.ColorIndex = 2;

                    //Agrego la fila

                    int i = 0;
                    foreach (DataRow row in data.Rows)
                    {
                        double value;
                        if (!double.TryParse(row[item].ToString(), out value))
                        {
                            worksheet.Cells[i + 2, ColumnIndex].NumberFormat = "@";
                        }
                        worksheet.Cells[i + 2, ColumnIndex] = row[item];

                        //Estilo de la celda
                        //Borde
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[i + 2, ColumnIndex]).Borders.ColorIndex = 2;
                        //Relleno
                        ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[i + 2, ColumnIndex]).Interior.Color = System.Drawing.ColorTranslator.ToOle(background1);

                        if ((i % 2) > 0)
                        {
                            //Relleno
                            ((Microsoft.Office.Interop.Excel.Range)worksheet.Cells[i + 2, ColumnIndex]).Interior.Color = System.Drawing.ColorTranslator.ToOle(background2);
                        }
                        i++;
                    }

                    ColumnIndex++;
                }

                worksheet.Columns.AutoFit(); // Todas las columnas auto redimensionadas
                excel.DisplayAlerts = false; // Esto es para evitar el cuadro de dialogo de sobreescribir archivo
                workbook.SaveAs(ruta, Local: true);
                Operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Exito, clsReferencias.MESSAGE_Exito_Export);
            }
            catch (Exception ex)
            {
                //Enviar esta información para otro lado
                Operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Error, new clsException(ex).ErrorMessage(), clsReferencias.MESSAGE_Error_Title);
            }
            finally
            {
                excel.Quit();
                workbook = null;
                excel = null;
            }
        }

        private void ObtenerColumnasExportar()
        {
            foreach (ListaExporta item in lstColumnas.Items)
            {
                if (!item.isChecked)
                {
                    data.Columns.Remove(item.Name);
                }
            }
        }

        private void CheckBox_Click(object sender, RoutedEventArgs e)
        {
            SeleccionDeseleccion(chkAll.IsChecked.Value);
        }

        private bool NoSelection()
        {
            bool flag = false;
            foreach (ListaExporta item in lstColumnas.Items)
            {
                if (item.isChecked)
                {
                    flag = true;
                    break;
                }
            }
            return flag;
        }

        private void SeleccionDeseleccion(bool value)
        {
            listado = exporta.FindChecks(value);
            lstColumnas.ItemsSource = listado;
        }

        private async Task IniciarProceso()
        {

            this.Visibility = Visibility.Collapsed;
            frmMain.Exportando_Label((frmMain)frmMain.principal);
            await Task.Run(() =>
            {
                ExportToExcel(FileName);
            });

            if (Operacion.OperationType == clsReferencias.TYPE_MESSAGE_Exito) //Si todo salió bien abrimos el Excel
            {
                try
                {
                System.Diagnostics.Process.Start(FileName);
                }
                catch (Exception ex)
                {
                    Operacion = new Operacion(clsReferencias.TYPE_MESSAGE_Error, new clsException(ex).ErrorMessage(), "El documento fue exportado, sin embargo no pudimos abrirlo.");
                }
            }

            frmMain.AddNotification((frmMain)frmMain.principal, Operacion);

            this.Close();
        }

        private void LoadProgressWindow()
        {
            Loading v = new Loading();

            v.ShowDialog();
            System.Windows.Threading.Dispatcher.Run();
        }
    }
}
