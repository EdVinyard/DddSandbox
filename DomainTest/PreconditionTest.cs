using System;
using NUnit.Framework;
using Domain;

namespace DomainTest
{
    [TestFixture]
    public class PreconditionTest
    {
        [Test]
        public void ExceptionMessageShouldIncludeNameOfVariable()
        {
            // Act
            string actualExceptionMessage = null;
            try
            {
                Precondition.MustNotBeNull(null, "EXPECTED");
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
