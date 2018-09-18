using PruebaWPF.Referencias;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace PruebaWPF.UserControls
{
    /// <summary>
    /// Lógica de interacción para MessageCard.xaml
    /// </summary>
    public partial class MessageCard : UserControl
    {
        // Tipo de Mensaje: 0 = Exito, 1= Error, 2 = Advertencia, 3 = Pregunta.
        public string Mensaje { get; set; }
        public int MessageType { get; set; }

        //public EventHandler ButtonClick;

        public MessageCard()
        {
            InitializeComponent();

        }

        public MessageCard(string Msj, int type)
        {
            this.Mensaje = Msj;
            this.MessageType = type;

            InitializeComponent();
            
        }

        //public event EventHandler Click
        //{
        //    add { ButtonClick += value; }
        //    remove { ButtonClick -= value; }
        //}

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            //EventHandler handler = ButtonClick;
            //if (ButtonClick != null)
            //{
            //    ButtonClick(this, e);
            //}
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            //EventHandler handler = ButtonClick;
            //if (ButtonClick != null)
            //{
            //    ButtonClick(this, e);
            //}
        }

        private void btnYes_Click(object sender, RoutedEventArgs e)
        {
            //EventHandler handler = ButtonClick;
            //if (ButtonClick != null)
            //{
            //    ButtonClick(this, e);
            //}
        }

        private void btnNo_Click(object sender, RoutedEventArgs e)
        {
            //EventHandler handler = ButtonClick;
            //if (ButtonClick != null)
            //{
            //    ButtonClick(this, e);
            //}
        }
    }
}
