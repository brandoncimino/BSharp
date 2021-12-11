using System;
using System.Collections.Generic;

using FowlFever.BSharp.Collections;

using NUnit.Framework;

namespace BSharp.Tests.Collections {
    public class CombinatorialTests {
        public static List<string>    A => new List<string>() { "A1", "A2", "A3" };
        public static List<DayOfWeek> B => new List<DayOfWeek>() { (DayOfWeek.Monday), DayOfWeek.Friday };
        public static List<float>     C => new List<float>() { 0.3f };

        public static List<double> D => new List<double>() { 4.1, 4.2 };

        public static Combinatorial<string, DayOfWeek>                    AB => Combinatorial.Of(A, B);
        public static Type[]                                              AB_Types = new[] { A.ItemType(), B.ItemType() };
        public static Combinatorial<string, DayOfWeek, float>             ABC        => Combinatorial.Of(A, B, C);
        public static Type[]                                              ABC_Types  => new[] { A.ItemType(), B.ItemType(), C.ItemType() };
        public static Combinatorial<string, DayOfWeek, float, double>     ABCD       => Combinatorial.Of(A, B, C, D);
        public static Type[]                                              ABCD_Types => new[] { A.ItemType(), B.ItemType(), C.ItemType(), D.ItemType() };
        public static Combinatorial<string, DayOfWeek, string, DayOfWeek> ABAB       => Combinatorial.Of(A, B, A, B);
        public static Type[]                                              ABAB_Types => new[] { A.ItemType(), B.ItemType(), A.ItemType(), B.ItemType() };
        public static Combinatorial<float, double>                        CD         => Combinatorial.Of(C, D);
        public static Type[]                                              CD_Types   => new[] { C.ItemType(), D.ItemType() };

        [Test]
        public void ABxC() {
            var abc = AB * C;
            Assert.That(abc,                                 Is.EqualTo(ABC));
            Assert.That(abc.GetType().GetGenericArguments(), Is.EquivalentTo(ABC_Types));
        }

        [Test]
        public void ABxB() {
            var ab_b          = AB * B;
            var expectedTypes = new[] { A.ItemType(), B.ItemType(), B.ItemType() };
            Assert.That(ab_b,                                 Is.EqualTo(Combinatorial.Of(A, B, B)));
            Assert.That(ab_b.GetType().GetGenericArguments(), Is.EquivalentTo(expectedTypes));
        }

        [Test]
        public void ABxDecimal() {
            var decimals      = new List<decimal> { 1, 2, 3 };
            var ab_dec        = AB.Multiply(decimals);
            var expectedTypes = new[] { A.ItemType(), B.ItemType(), decimals.ItemType() };

            Assert.That(ab_dec,                                 Is.EqualTo(Combinatorial.Of(A, B, decimals)));
            Assert.That(ab_dec.GetType().GetGenericArguments(), Is.EquivalentTo(expectedTypes));
        }

        [Test]
        public void ABxCD() {
            var abcd = AB.Multiply(CD);
            Assert.That(abcd,                                 Is.EqualTo(ABCD));
            Assert.That(abcd.GetType().GetGenericArguments(), Is.EquivalentTo(ABCD_Types));
        }

        [Test]
        public void ABxC_D() {
            var abcd = AB.Multiply(C, D);
            Assert.That(abcd,                                 Is.EqualTo(ABCD));
            Assert.That(abcd.GetType().GetGenericArguments(), Is.EquivalentTo(ABCD_Types));
        }

        [Test]
        public void ABxCxD() {
            var abcd = AB.Multiply(C).Multiply(D);
            Assert.That(abcd,                                 Is.EqualTo(ABCD));
            Assert.That(abcd.GetType().GetGenericArguments(), Is.EquivalentTo(ABCD_Types));
        }
    }
}