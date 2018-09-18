using PruebaWPF.Views.Main;
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
    /// Lógica de interacción para Bar_Back.xaml
    /// </summary>
    public partial class Bar_Back : UserControl
    {
        public string Value
        {
            get;
            set;
        }
        public string ValueUpper => Value.ToUpper();

        //public object Value
        //{
        //    get { return (object)GetValue(ValueProperty); }
        //    set { SetValue(ValueProperty, value); }
        //}

        //public static readonly DependencyProperty ValueProperty =
        //    DependencyProperty.Register("Value", typeof(object),
        //    typeof(Bar_Back), new PropertyMetadata(null));

        public Bar_Back()
        {
            InitializeComponent();
            //LayoutRoot.DataContext = this;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            frmMain.GoBack();
        }
    }
}
