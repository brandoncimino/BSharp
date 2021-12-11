using System;

using FowlFever.BSharp.Exceptions;
using FowlFever.Testing;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests {
    public class ExceptionUtilsTests {
        private const string Prepended = "PREPENDED";
        private const string Original  = "ORIGINAL";

        [Test]
        public void PrependMessage_KnownType() {
            try {
                throw new NullReferenceException(Original);
            }
            catch (NullReferenceException e) {
                var e2 = e.PrependMessage(Prepended);
                AssertAll.Of(
                    e2,
                    Is.Not.Null,
                    Has.Property(nameof(e2.Message)).StartsWith(Prepended),
                    Has.Property(nameof(e2.Message)).EndsWith(Original),
                    Is.TypeOf<NullReferenceException>()
                );
            }
        }

        [Test]
        public void PrependMessage_UnknownType() {
            try {
                throw new NullReferenceException(Original);
            }
            catch (Exception e) {
                var e2 = e.PrependMessage(Prepended);

                AssertAll.Of(
                    e2,
                    Is.Not.Null,
                    Is.TypeOf<NullReferenceException>(),
                    Is.InstanceOf<NullReferenceException>(),
                    Is.AssignableTo<NullReferenceException>(),
                    Has.Property(nameof(e.Message)).StartsWith(Prepended),
                    Has.Property(nameof(e.Message)).EndsWith(Original)
                );
            }
        }
    }
}