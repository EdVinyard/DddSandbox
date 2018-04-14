using System;
using NUnit.Framework;
using Framework;

namespace DomainTest
{
    [TestFixture]
    public class PreconditionTest
    {
        public object Precondition { get; private set; }

        [Test]
        public void ExceptionMessageShouldIncludeNameOfVariable()
        {
            // Act
            string actualExceptionMessage = null;
            try
            {
                object nul = null;
                nul.MustNotBeNull("EXPECTED");
                Assert.Fail("The previous statement should have thrown " +
                    "ArgumentNullException.");
            }
            catch (ArgumentNullException exc)
            {
                actualExceptionMessage = exc.Message;
            }

            // Assert
            Assert.That(actualExceptionMessage, 
                Contains.Substring("Parameter name: EXPECTED"));
        }
    }
}
