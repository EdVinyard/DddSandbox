namespace Domain.Aggregate.Common
{
    public class Money : ValueType
    {
        public decimal Amount { get; protected set; }

        /// <summary>the three character ISO 4217 code</summary>
        public string CurrencyCode { get; protected set; }
    }
}
