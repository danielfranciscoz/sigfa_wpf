namespace PruebaWPF.Model
{
    class POSBanpro
    {
        public string Baudrate { get; set; }
        public string DataBits { get; set; }
        public string Parity { get; set; }
        public string StopBits { get; set; }
        public int Timeout { get; set; }
        public bool ProcessBIN => false;
        public string ComPort { get; set; }

    }

    public class ConfiguracionPOS
    {
        public string Atributo { get; set; }
        public string Valor { get; set; }
    }
}
