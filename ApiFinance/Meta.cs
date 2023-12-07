namespace ApiFinance
{    public class Meta
    {
        public string symbol { get; set; }
        public double regularMarketPrice { get; set; }        
        public double variacaoDiaAnterior { get; set; }
        public double variacaoDesdePrimeiraData { get; set; }
        public DateTime dateTime { get; set; }
    }
}
