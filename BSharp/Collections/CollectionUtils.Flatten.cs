using System.Collections.Generic;
using System.Linq;

namespace FowlFever.BSharp.Collections;

public static partial class CollectionUtils {
    /// <summary>
    /// Backing method for <see cref="Flatten{T}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{T}})"/> overloads to prevent clobbering
    /// and recursion with those overloads.
    /// </summary>
    private static IEnumerable<T> _flatten<T>(this IEnumerable<IEnumerable<T>> source) => source.SelectMany(it => it);

    /// <summary>
    /// Reduces jagged <see cref="IEnumerable{T}"/>s into a single <see cref="IEnumerable{T}"/>.
    /// </summary>
    /// <param name="матрёшка">a jagged <a href="https://en.wikipedia.org/wiki/Matryoshka_doll">матрёшка</a> of <see cref="IEnumerable{T}"/>s</param>
    /// <typeparam name="T">the "smallest" item</typeparam>
    /// <returns>an <see cref="IEnumerable{T}"/> containing <b>all</b> of the original items</returns>
    /// <remarks>
    /// When using concrete <see cref="IEnumerable{T}"/> types, such as <c>int[][][]</c>, C# seems to prefer (probably for speed reasons) the less-specific overloads.
    /// <p/>
    /// This results in <see cref="Flatten{T}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{T}})"/> methods against concrete types only performing
    /// "one layer" of flattening.
    /// <p/>
    /// This can be overcome by specifying <typeparamref name="T"/> explicitly:
    /// <code><![CDATA[
    /// public static void Example() {
    ///     int[][][][] array = { };
    ///     IEnumerable<IEnumerable<IEnumerable<IEnumerable<int>>>> enumerable = array;
    /// 
    ///     var flatArray      = array.Flatten();      // => IEnumerable<int[][]>
    ///     var flatEnumerable = enumerable.Flatten(); // => IEnumerable<int>
    ///     var flatExplicit   = array.Flatten<int>(); // => IEnumerable<int>
    /// }
    /// ]]></code>
    /// </remarks>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<T>> матрёшка) => матрёшка._flatten();

    /// <inheritdoc cref="Flatten{T}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{T}})"/>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<IEnumerable<T>>> матрёшка) => матрёшка
                                                                                                       ._flatten()
                                                                                                       ._flatten();

    /// <inheritdoc cref="Flatten{T}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{T}})"/>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>> матрёшка) => матрёшка
                                                                                                                    ._flatten()
                                                                                                                    ._flatten()
                                                                                                                    ._flatten();

    /// <inheritdoc cref="Flatten{T}(System.Collections.Generic.IEnumerable{System.Collections.Generic.IEnumerable{T}})"/>
    public static IEnumerable<T> Flatten<T>(this IEnumerable<IEnumerable<IEnumerable<IEnumerable<IEnumerable<T>>>>> матрёшка) => матрёшка
                                                                                                                                 ._flatten()
                                                                                                                                 ._flatten()
                                                                                                                                 ._flatten()
                                                                                                                                 ._flatten();
}