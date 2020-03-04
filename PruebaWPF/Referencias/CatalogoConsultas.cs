namespace PruebaWPF.Referencias
{
    class CatalogoConsultas
    {
        public const string arancelesSIRA = "cat.sp_ArancelesSIRA @carnet";
        public const string areasMoney = "SELECT codigo,descripcion,Moneda,Monto  FROM ( SELECT idarea, SUM(monto) Monto, Moneda FROM ((SELECT ISNULL(rd.IdArea,o.IdArea) idarea, monto, m.Moneda, o.regAnulado FROM pay.ReciboPago p inner join pay.Recibo rp ON rp.IdRecibo =p.IdRecibo and rp.Serie = p.Serie inner join coin.Moneda m ON m.IdMoneda = p.IdMoneda left  join op.OrdenPago o ON rp.IdOrdenPago = o.IdOrdenPago left join pay.ReciboDatos rd ON rd.IdRecibo =p.IdRecibo and rd.Serie = p.Serie WHERE p.regAnulado=0 and rp.regAnulado = 0) ) a WHERE a.regAnulado=0 GROUP BY a.idarea,a.Moneda ) t inner join cat.vw_Areas a ON a.codigo = t.idarea";
        public const string areasCount = "";

        public const string recintosCount = "";
        public const string cajasCount = "";

        public const string recintosMoney = "";
        public const string cajasMoney = "";
    }
}
