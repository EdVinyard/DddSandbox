using Framework;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Domain.Aggregate.Common
{
    public class Money : DDD.ValueType, IEquatable<Money>
    {
        private static readonly Regex Iso4217CurrencyCodePattern = 
            new Regex(@"\A[A-Z]{3}\Z", RegexOptions.IgnoreCase);

        public static readonly HashSet<string> Iso4217CurrencyCodes = new HashSet<string>(new[] {
            "AED",
            "AFN",
            "ALL",
            "AMD",
            "ANG",
            "AOA",
            "ARS",
            "AUD",
            "AWG",
            "AZN",
            "BAM",
            "BBD",
            "BDT",
            "BGN",
            "BHD",
            "BIF",
            "BMD",
            "BND",
            "BOB",
            "BOV",
            "BRL",
            "BSD",
            "BTN",
            "BWP",
            "BYN",
            "BZD",
            "CAD",
            "CDF",
            "CHE",
            "CHF",
            "CHW",
            "CLF",
            "CLP",
            "CNY",
            "COP",
            "COU",
            "CRC",
            "CUC",
            "CUP",
            "CVE",
            "CZK",
            "DJF",
            "DKK",
            "DOP",
            "DZD",
            "EGP",
            "ERN",
            "ETB",
            "EUR",
            "FJD",
            "FKP",
            "GBP",
            "GEL",
            "GHS",
            "GIP",
            "GMD",
            "GNF",
            "GTQ",
            "GYD",
            "HKD",
            "HNL",
            "HRK",
            "HTG",
            "HUF",
            "IDR",
            "ILS",
            "INR",
            "IQD",
            "IRR",
            "ISK",
            "JMD",
            "JOD",
            "JPY",
            "KES",
            "KGS",
            "KHR",
            "KMF",
            "KPW",
            "KRW",
            "KWD",
            "KYD",
            "KZT",
            "LAK",
            "LBP",
            "LKR",
            "LRD",
            "LSL",
            "LYD",
            "MAD",
            "MDL",
            "MGA",
            "MKD",
            "MMK",
            "MNT",
            "MOP",
            "MRU",
            "MUR",
            "MVR",
            "MWK",
            "MXN",
            "MXV",
            "MYR",
            "MZN",
            "NAD",
            "NGN",
            "NIO",
            "NOK",
            "NPR",
            "NZD",
            "OMR",
            "PAB",
            "PEN",
            "PGK",
            "PHP",
            "PKR",
            "PLN",
            "PYG",
            "QAR",
            "RON",
            "RSD",
            "RUB",
            "RWF",
            "SAR",
            "SBD",
            "SCR",
            "SDG",
            "SEK",
            "SGD",
            "SHP",
            "SLL",
            "SOS",
            "SRD",
            "SSP",
            "STN",
            "SVC",
            "SYP",
            "SZL",
            "THB",
            "TJS",
            "TMT",
            "TND",
            "TOP",
            "TRY",
            "TTD",
            "TWD",
            "TZS",
            "UAH",
            "UGX",
            "USD",
            "USN",
            "UYI",
            "UYU",
            "UZS",
            "VEF",
            "VND",
            "VUV",
            "WST",
            "XAF",
            "XAG",
            "XAU",
            "XBA",
            "XBB",
            "XBC",
            "XBD",
            "XCD",
            "XDR",
            "XOF",
            "XPD",
            "XPF",
            "XPT",
            "XSU",
            "XTS",
            "XUA",
            "XXX", // for transactions where no currency is involved
            "YER",
            "ZAR",
            "ZMW",
            "ZWL",
        });

        public static readonly Money Zero = new Money(decimal.Zero, "XXX");

        public Money(decimal amount, string currencyCode)
        {
            Amount = amount;
            CurrencyCode = ValidatedCurrencyCode(currencyCode);
        }

        /// <summary>shorthand for <c>new Money(amount, "USD")</c></summary>
        public static Money USD(decimal amount) => new Money(amount, "USD");

        /// <summary> FOR NHibernate ONLY! </summary>
        protected Money() { }

        private static string ValidatedCurrencyCode(string currencyCode)
        {
            Precondition.MustNotBeNull(currencyCode, nameof(currencyCode));

            currencyCode = currencyCode.ToUpper(); // ignore case

            if (!Iso4217CurrencyCodePattern.IsMatch(currencyCode))
            {
                throw new InvalidCurrencyCodeFormat(nameof(currencyCode));
            }

            if (!Iso4217CurrencyCodes.Contains(currencyCode))
            {
                throw new UnknownCurrencyCode(currencyCode, nameof(currencyCode));
            }

            return currencyCode;
        }

        [Serializable]
        public class InvalidCurrencyCodeFormat : ArgumentException
        {
            public InvalidCurrencyCodeFormat(string paramName) : base(
                "must be exactly 3 letters",
                paramName) { }
        }

        [Serializable]
        public class UnknownCurrencyCode : ArgumentException
        {
            // SECURITY: It's generally risky to echo strings back to the
            // user or into the logs indiscriminantly.  Here, it's safe 
            // because we'll only echo back 3 chars, already confirmed to be
            // letters only.

            public UnknownCurrencyCode(string unknownCode, string paramName) : base(
                "only values in Money.Iso4217CurrencyCodes are allowed, " +
                $"but '{unknownCode}' was supplied",
                paramName) { }
        }

        public decimal Amount { get; protected set; }

        /// <summary>the three character ISO 4217 code</summary>
        public string CurrencyCode { get; protected set; }

        public bool IsPositive    { get { return Amount  > 0m; } }
        public bool IsNonNegative { get { return Amount >= 0m; } }
        public bool IsZero        { get { return Amount == 0m; } }
        public bool IsNonPositive { get { return Amount <= 0m; } }
        public bool IsNegative    { get { return Amount  < 0m; } }

        public override bool Equals(object obj) => this.Equals(obj as Money);

        public bool Equals(Money other)
        {
            if (other == null) return false;
            if (object.ReferenceEquals(this, other)) return true;

            // Like distance, zero is equivalent regardless of units.
            if (this.IsZero && other.IsZero) return true;

            return this.Amount == other.Amount
                && this.CurrencyCode == other.CurrencyCode;
        }

        public static bool operator ==(Money l, Money r)
        {
            return ((l == null) && (r == null))
                || ((l != null) && (r != null) && l.Equals(r));
        }

        public static bool operator !=(Money l, Money r) => !(l == r);

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                hash = hash * 23 + Amount.GetHashCode();
                hash = hash * 23 + CurrencyCode.GetHashCode();
                return hash;
            }
        }
    }
}
