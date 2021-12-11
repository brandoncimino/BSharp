using System;
using System.Collections.Generic;

namespace FowlFever.BSharp.Collections {
    /// <summary>
    /// Represents a pair of arbitrary, strongly-typed values.
    /// <p/>
    /// <code>
    /// TODO: Create an annotation to remove the label. (Maybe this works with existing attributes?)
    /// TODO: Create an annotation to set custom labels to either of the 2 pairs.
    /// TODO: Create an annotation to make the "primary" value read-only in the editor.
    /// TODO: Make a special kind of collection for Pairs, OR a custom renderer for Lists of pairs - UPDATE 6/1/2021: List{Pair{TX,TY}} already seems to work, so I don't think this is needed anymore
    ///
    /// 6/1/21:
    /// TODO: Should TX and TY be restricted to ISerializable?
    /// TODO: Should this be changed from a `struct` to an interface + concrete class? (That would allow for implicit conversion to Vector2)
    ///
    /// 7/22/21:
    /// TODO: Need to see what functionality, if any, is lost by replacing Pair{TX,TY} with with Tuple{T1,T2}:
    ///     - Loses IPrimaryKeyed interface
    /// </code>
    /// </summary>
    /// <remarks>
    /// This is essentially a <see cref="KeyValuePair{TKey,TValue}"/> version of Unity's Vector2. (We cannot extend the "UnityEngine" class directly because it is <see langword="sealed"/>)
    ///
    /// It also implements <see cref="IPrimaryKeyed{T}"/>, where <see cref="X"/> is the <see cref="PrimaryKey"/>.
    /// </remarks>
    /// <example>
    /// <see cref="Pair{TX,TY}"/>s are primarily useful when you'd like to take advantage of <see cref="SerializableAttribute"/> fields in the Unity editor.
    ///
    /// For example:
    /// <code><![CDATA[
    /// public Pair<DayOfWeek,Color> MondayColor;
    /// ]]></code>
    ///
    /// Will render inspector UI buttons for a <see cref="DayOfWeek"/> dropdown and a color picker.
    /// </example>
    /// <typeparam name="TX"></typeparam>
    /// <typeparam name="TY"></typeparam>
    [Serializable]
    public struct Pair<TX, TY> : IPrimaryKeyed<TX> {
        public TX X;
        public TY Y;

        public Pair(TX x = default, TY y = default) {
            X = x;
            Y = y;
        }

        public override string ToString() {
            return $"[{X},{Y}]";
        }

        public TX PrimaryKey => X;
    }
}