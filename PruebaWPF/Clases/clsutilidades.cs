using MaterialDesignThemes.Wpf;
using Microsoft.Reporting.WinForms;
using PruebaWPF.Referencias;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace PruebaWPF.Clases
{
    class clsUtilidades
    {
        public static PackIconKind GetIconFromString(string icon)
        {
            if (icon != null)
            {
                foreach (var item in Enum.GetValues(typeof(MaterialDesignThemes.Wpf.PackIconKind)))
                {
                    if (item.ToString().Equals(icon))
                    {
                        return (MaterialDesignThemes.Wpf.PackIconKind)(int)item;
                    }
                }
            }
            return (MaterialDesignThemes.Wpf.PackIconKind)(-1);

        }
        public static void OpenMessage(Operacion operacion)
        {
            if (operacion != null)
            {
                BoxMessage bx = new BoxMessage(operacion);
                bx.Owner = frmMain.principal;
                bx.ShowDialog();
            }
        }

        public static void OpenMessage(Operacion operacion, Window owner)
        {
            if (operacion != null)
            {
                BoxMessage bx = new BoxMessage(operacion);
                bx.Owner = owner;
                bx.ShowDialog();
            }
        }

        public static bool OpenDeleteQuestionMessage()
        {
            BoxMessage bx = new BoxMessage(new Operacion { OperationType = clsReferencias.TYPE_MESSAGE_Question, Mensaje = clsReferencias.MESSAGE_Confirm_Delete });
            bx.Owner = frmMain.principal;
            bx.ShowDialog();

            return bx.result;
        }

        public static bool OpenDeleteQuestionMessage(string MensajePersonalizado)
        {
            BoxMessage bx = new BoxMessage(new Operacion { OperationType = clsReferencias.TYPE_MESSAGE_Question, Mensaje = MensajePersonalizado });
            bx.Owner = frmMain.principal;
            bx.ShowDialog();

            return bx.result;
        }

        public static SolidColorBrush BorderError()
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom("#d50000"));
        }

        public static SolidColorBrush BorderNormal()
        {
            return (SolidColorBrush)(new BrushConverter().ConvertFrom("#757575"));
        }

        public static void UpdateControl(Control control)
        {

            if (control is TextBox)
            {
                BindingOperations.GetBindingExpression(control, TextBox.TextProperty).UpdateTarget();
            }
            else if (control is ToggleButton)
            {
                BindingOperations.GetBindingExpression(control, ToggleButton.IsCheckedProperty).UpdateTarget();
            }
            else if (control is Slider)
            {
                BindingOperations.GetBindingExpression(control, Slider.ValueProperty).UpdateTarget();
            }
        }

        public static void Dialog_Perfomance(Window frame)
        {
            frame.Owner = frmMain.principal;
            frame.ShowInTaskbar = false;
            frame.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }

        public static void Dialog_ModalDesign(Window frame)
        {
            Dialog_Perfomance(frame);
            frame.Width = frmMain.Ancho();
            frame.Height = frmMain.Alto();
            frame.AllowsTransparency = true;

            SolidColorBrush color = new SolidColorBrush(System.Windows.Media.Color.FromRgb(0, 0, 0));
            color.Opacity = 0.3;

            frame.Background = color;
        }

        /// <summary>
        /// Genera imagen de código de barra, basado en el texto que se le asigna, las medidas no pixeleadas
        /// son de 400 ancho y 100 alto
        /// </summary>
        /// <param name="texto"></param>
        /// <param name="ancho"></param>
        /// <param name="alto"></param>
        /// <returns>Imagen en Arreglo de Bytes</returns>
        public static Byte[] CodigoBarra(string texto, int ancho, int alto)
        {
            BarcodeLib.Barcode codigo = new BarcodeLib.Barcode();

            System.Drawing.Image img = codigo.Encode(BarcodeLib.TYPE.CODE128, texto, ancho, alto);

            ImageConverter imageConverter = new ImageConverter();

            byte[] b = (byte[])imageConverter.ConvertTo(img, typeof(byte[]));

            //MemoryStream ms = new MemoryStream();
            //img.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
            //return ms.ToArray();

            return b;
        }

        public static Byte[] CodigoBarraRecibo(string texto)
        {
            //6.4cm; 1.83cm
            return CodigoBarra(texto, 241, 69);
        }

        public static void InformeDataSource(ReportViewer informe, ReportDataSource[] datasources)
        {
            informe.Reset();
            for (int i = 0; i < datasources.Length; i++)
            {
                informe.LocalReport.DataSources.Add(datasources[i]);
            }
        }

        public static string AppName()
        {
            return ((AssemblyTitleAttribute)Attribute.GetCustomAttribute(Assembly.GetExecutingAssembly(), typeof(AssemblyTitleAttribute), false)).Title;
        }

        public static string FindMacActual()
        {

            byte[] bytes = NetworkInterface.GetAllNetworkInterfaces().ToList().FirstOrDefault().GetPhysicalAddress().GetAddressBytes();

            string MAC = "";
            for (int i = 0; i < bytes.Length; i++)
            {

                MAC = MAC + "" + bytes[i].ToString("X2");

                if (i != bytes.Length - 1)
                {
                    MAC = MAC + "-";

                }
            }

            return MAC;
        }

        public static String[] GetSerialPorts()
        {
            //var n = SerialPort.GetPortNames();
            //foreach (String comport in SerialPort.GetPortNames())
            //{
            //    var a = comport;
            //}
            return SerialPort.GetPortNames();
        }
    }
}
