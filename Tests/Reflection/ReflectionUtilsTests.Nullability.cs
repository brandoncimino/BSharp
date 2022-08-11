using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Reflection;
using FowlFever.Testing;

using JetBrains.Annotations;

using NUnit.Framework;

using Spectre.Console;

namespace BSharp.Tests.Reflection;

public partial class ReflectionUtilsTests {
    #region Nullability

#pragma warning disable CS8618
    [UsedImplicitly]
    public record NullEx2 {
        public enum Null {
            Unknown, Yes, No,
        }

        public class Nullity : Attribute {
            public readonly Null Read;
            public readonly Null Write;

            public Nullity(Null read) : this(read, read) { }

            public Nullity(Null read, Null write) {
                Read  = read;
                Write = write;
            }

            public void Deconstruct(out Null read, out Null write) {
                read  = Read;
                write = Write;
            }
        }

        [Nullity(Null.Yes)]             public int     IntField;
        [Nullity(Null.No)]              public int?    IntField_N;
        [Nullity(Null.Yes)] [MaybeNull] public int     IntField_Maybe;
        [Nullity(Null.Yes)]             public string  StrField;
        [Nullity(Null.Yes)]             public string? StrField_N;
        [Nullity(Null.No, Null.Yes)]
        [MaybeNull]
        private string StrField_Maybe;
        [Nullity(Null.No)]                    public int     IntProp                 { get; set; }
        [Nullity(Null.Yes)]                   public int?    IntProp_N               { get; set; }
        [Nullity(Null.No)] [MaybeNull]        public int     IntProp_Maybe           { get; set; }
        [Nullity(Null.No)] [field: MaybeNull] public int     IntProp_Field_AllowNull { get; set; }
        [Nullity(Null.No)]                    public string  StrProp                 { get; set; }
        [Nullity(Null.Yes)]                   public string? StrProp_N               { get; set; }
        [Nullity(Null.Yes, Null.No)]
        [field: MaybeNull]
        public string StrProp_N_AllowNull { get; set; }

        [Nullity(Null.No)] public int IntMeth() => default;

        [Nullity(Null.Yes)] public int? IntMeth_N() => default;

        [Nullity(Null.No)] public string StrMeth() => "";

        [Nullity(Null.Yes)] public string? StrMeth_N() => default;

        public void Parameters(
            [Nullity(Null.No)]  int     i,
            [Nullity(Null.Yes)] int?    i_n,
            [Nullity(Null.No)]  string  str,
            [Nullity(Null.Yes)] string? str_n
        ) { }
    }
#pragma warning restore CS8618

    [Test]
    [TestCase(typeof(NullEx2))]
    public void Demo(Type type) {
        Ignore.Unless(
            typeof(NullabilityInfo).Assembly.GetName().Name!,
            Is.EqualTo("CSharp10"),
            () => $"This test is only valid pre-.NET 6, because after that it relies solely on the built-in {typeof(NullabilityInfo)} implementation." +
                  $"\nPrevious to .NET 6, it is a hubric stolen implementation of those classes."
        );
        var fields = type.GetRuntimeFields();
        Console.WriteLine($"{typeof(NullabilityInfo)} Assembly: {typeof(NullabilityInfo).Assembly}");
        foreach (var field in fields) {
            var table = new Table {
                            Title  = new TableTitle(field.Name, new Style(foreground: Color.DarkOrange)),
                            Border = TableBorder.Simple
                        }
                        .AddColumn("")
                        .AddColumn("Actual")
                        .AddColumn("Expected")
                        .AddColumn("🙋");
            var nullability = field.GetNullability();
            var (read, write)       = (nullability.ReadState, nullability.WriteState);
            var (expRead, expWrite) = field.GetCustomAttribute<NullEx2.Nullity>().MustNotBeNull();
            table.AddRow(field.Name, read.ToString(),  expRead.ToString(),  (read.ToString()  == expRead.ToString()) ? "✅" : "❌");
            table.AddRow(field.Name, write.ToString(), expWrite.ToString(), (write.ToString() == expWrite.ToString()) ? "✅" : "❌");
            AnsiConsole.Write(table);
        }
    }

    #endregion
}