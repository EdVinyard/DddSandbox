using Domain.Aggregate.Common;
using NUnit.Framework;

namespace DomainTest
{
    [TestFixture]
    public class MoneyTest
    {
        [TestCase("")]
        [TestCase("A")]
        [TestCase("AB")]
        [TestCase("123")]
        [TestCase("ABCD")]
        public void InvalidCurrencyCodeFormat(string invalidCurrencyCode)
        {
            Assert.Throws<Money.InvalidCurrencyCodeFormat>(() =>
            {
                new Money(1m, invalidCurrencyCode);
            });
        }

        public void UnknownCurrencyCode()
        {
            Assert.Throws<Money.UnknownCurrencyCode>(() =>
            {
                // it's not a currency code, but it's so money...
                new Money(1m, "YYZ");
            });
        }

        [Test]
        public void ConvenienceConstructorUsdSmokeTest()
        {
            var cNote = Money.USD(100.00m);

            Assert.AreEqual(100.00m, cNote.Amount);
            Assert.AreEqual("USD", cNote.CurrencyCode);
        }
    }
}
