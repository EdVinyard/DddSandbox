using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DomainDrivenDesignTest
{
    [TestFixture]
    public class EntityStructure
    {
        [Test]
        public void ThereMustBeSomeEntitiesDefined()
        {
            Assert.IsTrue(new AllDomainEntities().GetEnumerator().MoveNext());
        }

        [TestCaseSource(typeof(AllDomainEntities))]
        public void MustHaveExactlyOneProtectedZeroArgConstructor(Type entityType)
        {
            string rule = entityType.Name + @" must have exactly one, protected, 
zero-argument constructor.

Domain Entities must always be constructed using a stateless 'factory service'.
They should expose exactly one, protected, zero-argument constructor.  This 
guarantees:

    - no compiler warnings about virtual method calls in constructor, and
    - they're ready to add external dependencies to construction process

";
            var entityConstructors = entityType.GetConstructors(
                BindingFlags.Instance
                | BindingFlags.Public
                | BindingFlags.NonPublic);
            Assert.AreEqual(1, entityConstructors.Length, rule);
            ConstructorInfo entityConstructor = entityConstructors[0];
            Assert.IsFalse(entityConstructor.IsPublic, "PUBLIC CONSTRUCTOR " + rule);
            Assert.IsFalse(entityConstructor.IsAssembly, "INTERNAL CONSTRUCTOR " + rule);
            Assert.IsTrue(entityConstructor.IsFamily, "PROTECTED CONSTRUCTOR MISSING " + rule);
            Assert.AreEqual(0, entityConstructor.GetParameters().Length, "TOO MANY PARAMETERS " + rule);
        }

        [TestCaseSource(typeof(AllDomainEntities))]
        public void MustBeConstructedByNestedFactoryClass(Type entityType)
        {
            Assert.Multiple(() =>
            {
                Type entityFactoryType = null;
                Assert.DoesNotThrow(() => {
                    entityFactoryType = entityType
                        .GetNestedTypes()
                        .Single(c => c.Name == "Factory");
                },
                    $"{entityType.Name} must have a nested class named 'Factory'.");

                Assert.DoesNotThrow(() => entityFactoryType
                    .FindInterfaces(IsDddFactory, null)
                    .Single(),
                    $"{entityType.Name}.Factory must implement DDD.Factory.");

                var publicFactoryMethods = PublicMethods(entityFactoryType);
                Assert.Greater(publicFactoryMethods.Count(), 0,
                    $"{entityType.Name}.Factory must have at least one method named 'New'.");

                foreach (var m in publicFactoryMethods)
                {
                    Assert.AreEqual("New", m.Name);
                    Assert.AreSame(entityType, m.ReturnType,
                        $"Public method {entityType.Name}.Factory.{m.Name} " +
                        $"must return {entityType.Name}.");
                }
            });
        }

        public IEnumerable<MethodInfo> PublicMethods(Type t)
        {
            return t.GetMethods().Where(m => 
                m.IsPublic 
                && !ObjectMethodNames.Contains(m.Name));
        }

        private static readonly ISet<string> ObjectMethodNames = new HashSet<string>(
            typeof(object)
            .GetMethods()
            .Select(m => m.Name));

        private bool IsDddFactory(Type t, object filterCriteria) => 
            t.Implements<DDD.Factory>();

        [TestCaseSource(typeof(AllDomainEntityProperties))]
        public void MustNotHavePublicSetters(PropertyInfo property)
        {
            var rule = $@"

The {property.DeclaringType}.{property.Name} setter must NOT be public.

Domain Entities may expose public getters, but should allow mutation
only through methods.  For example, instead of exposing a public
property setter for a 'Deleted' flag, instead expose a public method
named 'Delete()'.  Although *exactly* the same work can be accomplished
in either case, this convention encourages us to conceptualize compound 
or complex operations as *controlled* by the Domain Entity, and distinct
from the simple data owned by the entity.

For example, prefer client code that looks like this:

    widget.Delete();

Avoid client code that directly manipulates properties that should 
be kept in sync like this:

    widget.Deleted = true;
    widget.DateUpdated = clock.Now;

            ";

            var setter = property.GetSetMethod(nonPublic: true);

            if (setter == null)
            {
                Assert.Pass("no setter defined");
            }
            else
            {
                Assert.IsFalse(setter.IsPublic, rule);
            }
        }
    }
}
