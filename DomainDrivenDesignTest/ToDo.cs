using NUnit.Framework;

namespace DomainDrivenDesignTest
{
    public abstract class ToDo {
        [Test]
        [Ignore("Write more tests.")]
        public void WriteMoreTests() { Assert.Fail("Write more tests."); }
    }

    [TestFixture] public class AggregateRootStructure : ToDo { }
    [TestFixture] public class AggregateStructure : ToDo { }
    [TestFixture] public class FactoryStructure : ToDo { }
    [TestFixture] public class InterAggregateEventStructure : ToDo { }
    [TestFixture] public class RepositoryStructure : ToDo { }
    [TestFixture] public class ValueTypeStructure : ToDo { }
}
