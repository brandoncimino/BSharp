using System;

using FowlFever.BSharp.Exceptions;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests;

public class ExceptionExtensionTests {
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

    public class MyDataException : Exception {
        public int MyInt {
            get => this.GetData<int>();
            set => this.SetData(value);
        }

        public string? MyString {
            get => this.GetData<string>();
            set => this.SetData(value);
        }
    }

    [Test]
    public void SetData_Property() {
        var mde = new MyDataException {
            MyInt    = 5,
            MyString = "yolo",
        };

        Asserter.Against(mde)
                .And(it => it.Data[nameof(mde.MyInt)],    Is.EqualTo(5))
                .And(it => it.Data[nameof(mde.MyString)], Is.EqualTo("yolo"))
                .Invoke();
    }

    [Test]
    public void GetData_Property() {
        var mde = new MyDataException {
            Data = {
                [nameof(MyDataException.MyInt)]    = 5,
                [nameof(MyDataException.MyString)] = "yolo",
            },
        };

        Asserter.Against(mde)
                .And(Has.Property(nameof(mde.MyInt)).EqualTo(5))
                .And(Has.Property(nameof(mde.MyString)).EqualTo("yolo"))
                .Invoke();
    }
}