using System;
using System.Collections.Generic;
using System.Linq;

using FowlFever.BSharp.Collections;
using FowlFever.Testing;

using Newtonsoft.Json;

using NUnit.Framework;

using Is = NUnit.Framework.Is;

namespace BSharp.Tests.Collections {
    public class PrimaryKeyedListTests {
        private class NoInterface : IEquatable<NoInterface> {
            public DayOfWeek DayOfWeek;
            public string    Info;

            public NoInterface(DayOfWeek dayOfWeek) {
                this.DayOfWeek = dayOfWeek;
                this.Info      = $"DEFAULT {nameof(Info)} for {dayOfWeek}";
            }

            public bool Equals(NoInterface other) {
                return other != null && other.DayOfWeek == DayOfWeek && other.Info == Info;
            }
        }

        private class HasInterface : NoInterface, IPrimaryKeyed<DayOfWeek> {
            public DayOfWeek PrimaryKey => DayOfWeek;

            public HasInterface(DayOfWeek dayOfWeek) : base(dayOfWeek) { }

            public override string ToString() {
                return JsonConvert.SerializeObject(this);
            }
        }

        private class AlsoHasInterface : NoInterface, IPrimaryKeyed<DayOfWeek> {
            public DayOfWeek PrimaryKey => DayOfWeek;

            public AlsoHasInterface(DayOfWeek dayOfWeek) : base(dayOfWeek) { }
        }

        private struct StructWithInterface : IPrimaryKeyed<DayOfWeek> {
            public DayOfWeek DayOfWeek;
            public DayOfWeek PrimaryKey => DayOfWeek;
        }

        private static PrimaryKeyedList<DayOfWeek, HasInterface> NewListOfDays() => new PrimaryKeyedList<DayOfWeek, HasInterface> {
            new HasInterface(DayOfWeek.Monday),
            new HasInterface(DayOfWeek.Tuesday),
            new HasInterface(DayOfWeek.Wednesday),
            new HasInterface(DayOfWeek.Thursday),
            new HasInterface(DayOfWeek.Friday),
            new HasInterface(DayOfWeek.Saturday),
            new HasInterface(DayOfWeek.Sunday)
        };

        private class StringKey : IPrimaryKeyed<string>, IEquatable<StringKey> {
            public string Name;
            public int    Number;
            public string PrimaryKey => Name;

            public StringKey(string name, int number) {
                Name   = name;
                Number = number;
            }

            public bool Equals(StringKey other) {
                return other != null && other.Name == Name && other.Number == Number;
            }
        }

        private static KeyedList<string, StringKey> StringKeyedList = new PrimaryKeyedList<string, StringKey>() {
            new StringKey("one", 1), new StringKey("two", 2), new StringKey("three", 3)
        };

        private class HoldsPrimaryKeyedList : IEquatable<HoldsPrimaryKeyedList>, IEqualityComparer<HoldsPrimaryKeyedList> {
            public PrimaryKeyedList<DayOfWeek, StructWithInterface> MyKeyedList;

            public bool Equals(HoldsPrimaryKeyedList other) {
                if (ReferenceEquals(null, other)) {
                    return false;
                }

                if (ReferenceEquals(this, other)) {
                    return true;
                }

                // return Equals(MyKeyedList, other.MyKeyedList);
                return MyKeyedList.SequenceEqual(other.MyKeyedList);
            }

            public override bool Equals(object obj) {
                if (ReferenceEquals(null, obj)) {
                    return false;
                }

                if (ReferenceEquals(this, obj)) {
                    return true;
                }

                if (obj.GetType() != this.GetType()) {
                    return false;
                }

                return Equals((HoldsPrimaryKeyedList)obj);
            }

            public override int GetHashCode() {
                return (MyKeyedList != null ? MyKeyedList.GetHashCode() : 0);
            }

            public bool Equals(HoldsPrimaryKeyedList x, HoldsPrimaryKeyedList y) {
                if (ReferenceEquals(x, y)) {
                    return true;
                }

                if (ReferenceEquals(x, null)) {
                    return false;
                }

                if (ReferenceEquals(y, null)) {
                    return false;
                }

                if (x.GetType() != y.GetType()) {
                    return false;
                }

                return Equals(x.MyKeyedList, y.MyKeyedList);
            }

            public int GetHashCode(HoldsPrimaryKeyedList obj) {
                return (obj.MyKeyedList != null ? obj.MyKeyedList.GetHashCode() : 0);
            }
        }

        [Test]
        public void KeyedListSerializes() {
            string json = JsonConvert.SerializeObject(NewListOfDays(), Formatting.Indented);

            Console.WriteLine(json);

            var kl = JsonConvert.DeserializeObject<PrimaryKeyedList<DayOfWeek, HasInterface>>(json);

            Assert.That(NewListOfDays, Is.EqualTo(kl));
        }

        [Test]
        public void StringKeySerializes() {
            string json = JsonConvert.SerializeObject(StringKeyedList, Formatting.Indented);

            Console.WriteLine(json);

            var kl = JsonConvert.DeserializeObject<PrimaryKeyedList<string, StringKey>>(json);

            Assert.That(kl, Is.EqualTo(StringKeyedList));
        }

        [Test]
        public void KeyedListPassAsList() {
            GimmeAList(NewListOfDays());
        }

        private void GimmeAList<T>(IList<T> list) {
            Assert.Pass();
        }

        [Test]
        public void GetViaKey() {
            Assert.That(NewListOfDays()[DayOfWeek.Monday], Is.EqualTo(new HasInterface(DayOfWeek.Monday)));
        }

        [Test]
        public void Cloneable() {
            var ls = NewListOfDays();

            Assert.That(ls, Is.EqualTo(NewListOfDays()));
        }

        [Test]
        public void UpdateViaKey() {
            var ls = NewListOfDays();

            const string updatedInfo = "UPDATED";
            ls[DayOfWeek.Monday].Info = updatedInfo;

            for (int i = 0; i > NewListOfDays().Count; i++) {
                Assert.That(ls.ElementAt(i).Info, Is.EqualTo(NewListOfDays().ElementAt(i).PrimaryKey == DayOfWeek.Monday ? updatedInfo : NewListOfDays().ElementAt(i).Info));
            }
        }

        [Test]
        public void AddViaKey() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Monday)
            };

            int initialCount = kl.Count;

            kl[DayOfWeek.Tuesday] = new HasInterface(DayOfWeek.Tuesday);

            Assert.That(kl, Has.Property(nameof(kl.Count)).EqualTo(initialCount + 1));
            Assert.That(kl, Contains.Item(new HasInterface(DayOfWeek.Tuesday)));
        }

        [Test]
        public void ErrorAddingDuplicateKey() {
            var ls = NewListOfDays();
            Assert.Throws<ArgumentException>(() => ls.Add(new HasInterface(DayOfWeek.Monday)));
        }

        [Test]
        public void ErrorCreatingFromDuplicateList() {
            var ls = new List<HasInterface> {
                new HasInterface(DayOfWeek.Monday), new HasInterface(DayOfWeek.Tuesday), new HasInterface(DayOfWeek.Tuesday), new HasInterface(DayOfWeek.Tuesday)
            };
            Assert.Throws<ArgumentException>(() => new PrimaryKeyedList<DayOfWeek, HasInterface>(ls));
        }

        [Test]
        public void CanCreateFromValidList() {
            var ls = new List<HasInterface> {
                new HasInterface(DayOfWeek.Monday), new HasInterface(DayOfWeek.Tuesday), new HasInterface(DayOfWeek.Wednesday)
            };

            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface> {
                new HasInterface(DayOfWeek.Monday), new HasInterface(DayOfWeek.Tuesday), new HasInterface(DayOfWeek.Wednesday)
            };

            Assert.That(new PrimaryKeyedList<DayOfWeek, HasInterface>(ls), Is.EqualTo(kl));
        }

        [Test]
        public void ErrorOnMissingKey() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Tuesday)
            };

            Assert.Throws<KeyNotFoundException>(() => kl[DayOfWeek.Monday].Info = "yolo");
        }

        [Test]
        public void ErrorOnRemovingMissingKey() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Tuesday)
            };

            Assert.False(kl.Remove(DayOfWeek.Monday));
        }

        [Test]
        public void TestRemove() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface>() {
                new HasInterface(DayOfWeek.Monday), new HasInterface(DayOfWeek.Tuesday)
            };

            var initialLength = kl.Count;

            Assert.True(kl.Remove(DayOfWeek.Monday));
            Assert.That(kl, Contains.Item(new HasInterface(DayOfWeek.Tuesday)));
            Assert.That(kl, Does.Not.Contains(new HasInterface(DayOfWeek.Monday)));
            Assert.That(kl, Has.Property(nameof(kl.Count)).EqualTo(initialLength - 1));
        }

        [Test]
        public void TestJsonPopulateWithSameData() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface> {
                new HasInterface(DayOfWeek.Monday), new HasInterface(DayOfWeek.Tuesday)
            };

            var json = JsonConvert.SerializeObject(kl);
            JsonConvert.PopulateObject(json, kl);
        }

        [Test]
        public void TestJsonPopulateEmpty() {
            var kl = new PrimaryKeyedList<DayOfWeek, HasInterface> {
                new HasInterface(DayOfWeek.Monday), new HasInterface(DayOfWeek.Tuesday)
            };

            var json = JsonConvert.SerializeObject(kl);

            var kl2 = new PrimaryKeyedList<DayOfWeek, HasInterface>();

            Assert.That(kl2, Is.Not.EqualTo(kl));

            JsonConvert.PopulateObject(json, kl2);

            Assert.That(kl2, Is.EqualTo(kl));
        }

        [Test]
        public void SerializationOfObjectHoldingAKeyedList() {
            var holdingKeyedList = new HoldsPrimaryKeyedList() {
                MyKeyedList = new PrimaryKeyedList<DayOfWeek, StructWithInterface>() {
                    new StructWithInterface() {
                        DayOfWeek = DayOfWeek.Monday
                    },
                    new StructWithInterface() {
                        DayOfWeek = DayOfWeek.Tuesday
                    }
                }
            };

            var toJson = JsonConvert.SerializeObject(holdingKeyedList);

            var fromJson = JsonConvert.DeserializeObject<HoldsPrimaryKeyedList>(toJson);

            AssertAll.Of(
                () => Assert.That(holdingKeyedList.MyKeyedList, Is.EqualTo(fromJson.MyKeyedList)),
                () => Assert.That(holdingKeyedList,             Is.EqualTo(fromJson))
            );
        }
    }
}