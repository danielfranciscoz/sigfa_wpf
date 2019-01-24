using MaterialDesignThemes.Wpf;
using PruebaWPF.Referencias;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PruebaWPF.Clases
{
    public class Operacion
    {
        private int _OperationType;
        private string _Mensaje;

        private string _Titulo;

        public Operacion() { }

        public Operacion(int OperationType, string Mensaje)
        {
            this.OperationType = OperationType;
            this._Mensaje = Mensaje;
        }

        public Operacion(int OperationType, string Mensaje, string Titulo)
        {
            this.OperationType = OperationType;
            this._Mensaje = Mensaje;
            this._Titulo = Titulo;
        }

        public string Mensaje { get => _Mensaje; set => _Mensaje = value; }
        public string Titulo { get => _Titulo; set => _Titulo = value; }
        public int OperationType { get => _OperationType; set => _OperationType = value; }

        public PackIconKind Icon()
        {

            switch (OperationType)
            {
                case clsReferencias.TYPE_MESSAGE_Exito:
                    {
                        return PackIconKind.Check;
                    }
                case clsReferencias.TYPE_MESSAGE_Error:
                    {
                        return PackIconKind.AlertCircle;
                    }
                case clsReferencias.TYPE_MESSAGE_Advertencia:
                    {
                        return PackIconKind.Alert;
                    }
                case clsReferencias.TYPE_MESSAGE_Question:
                    {
                        return PackIconKind.HelpCircle;
                    }
                case clsReferencias.TYPE_MESSAGE_Information:
                    {
                        return PackIconKind.InformationOutline;
                    }
                case clsReferencias.TYPE_MESSAGE_Wait_a_Moment:
                    {
                        return PackIconKind.DatabaseSearch;
                    }
                default:
                    {
                        return PackIconKind.Close;
                    }

            }

        }
    }
}
