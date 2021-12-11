using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Hierarchic;

using NUnit.Framework;

namespace BrandonUtils.Tests.Standalone.Hierarchic {
    public class DependantTests {
        public class Parent : Guardian<Parent, Child> { }

        public class Child : Dependant<Parent, Child> {
            public Child(Parent guardian) : base(guardian) { }
        }

        [Test]
        public void ChildrenNeedParents() {
            var brandon = new Parent();
            var nicole  = new Parent();

            var happy = new Child(nicole);
            var louie = new Child(brandon);

            Assert.Throws<BrandonException>(() => brandon.Adopt(happy));
        }
    }
}