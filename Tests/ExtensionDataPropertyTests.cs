using System;

using FowlFever.BSharp.Exceptions;
using FowlFever.Testing;

using NUnit.Framework;

namespace BSharp.Tests;

public class ExtensionDataPropertyTests {
    [Test]
    [TestCase(99)]
    [TestCase("yolo")]
    public void ExceptionDataProperty_Instance_Get<T>(T value) {
        const string key  = nameof(ExceptionDataProperty_Instance_Get);
        var          exc  = new Exception();
        var          prop = new ExceptionDataProperty<T>(exc, key);
        exc.Data[key] = value;
        Asserter.That(prop.Get(), Is.EqualTo(value));
    }

    [Test]
    [TestCase(99)]
    [TestCase("yolo")]
    public void ExceptionDataProperty_Instance_Set<T>(T value) {
        const string key  = nameof(ExceptionDataProperty_Instance_Set);
        var          exc  = new Exception();
        var          prop = new ExceptionDataProperty<T>(exc, key);
        prop.Set(value);
        Asserter.That(exc.Data[key], Is.EqualTo(value));
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