using PruebaWPF.Model;
using PruebaWPF.Views.Main;
using PruebaWPF.Views.OrdenPago;
using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace PruebaWPF.UserControls
{
    /// <summary>
    /// Lógica de interacción para Card_AccesoDirecto.xaml
    /// </summary>
    public partial class Card_AccesoDirecto : UserControl
    {
        //internal string urlPage;
        //internal bool IsPage;
        //internal bool IsDialog;
        Brush c;
        internal Pantalla pantalla;
        public Card_AccesoDirecto()
        {
            InitializeComponent();
            
        }

        private void btnOpen_Click(object sender, RoutedEventArgs e)
        {
            frmMain.AddWindowOrPage(pantalla);
        }

        private void Button_MouseEnter(object sender, MouseEventArgs e)
        {
            c = CAccesoDirecto.Background;
            var a = c as SolidColorBrush;
           Byte factor = Byte.Parse("80");
            //a.Color = Color.FromArgb((byte)(a.Color.A+factor), (byte)(a.Color.R + factor), (byte)(a.Color.G + factor), (byte)(a.Color.B + factor));

            CAccesoDirecto.Background = new SolidColorBrush(Color.FromArgb((byte)(a.Color.A * factor), a.Color.R, a.Color.G, a.Color.B));
        }

        private void Button_MouseLeave(object sender, MouseEventArgs e)
        {
            CAccesoDirecto.Background = c;
        }

        /*
         Color c1 = Color.Red;
    Color c2 = Color.FromArgb(c1.A,
        (int)(c1.R * 0.8), (int)(c1.G * 0.8), (int)(c1.B * 0.8));
         */
    }
}
