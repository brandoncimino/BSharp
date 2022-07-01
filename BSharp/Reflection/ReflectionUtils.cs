using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Strings;

using JetBrains.Annotations;

namespace FowlFever.BSharp.Reflection {
    /// <summary>
    /// Contains utilities for <see cref="System.Reflection"/>.
    ///
    /// TODO: Split this class up. For example, the <see cref="Type"/> extensions should be in their own class
    /// </summary>
    [PublicAPI]
    public static partial class ReflectionUtils {
        #region Binding Flags

        /// <summary>
        /// <see cref="BindingFlags"/> that correspond to all "variables",
        /// which should be all <see cref="PropertyInfo"/>s and <see cref="FieldInfo"/>s
        /// (including <see cref="BindingFlags.NonPublic"/> and <see cref="BindingFlags.Static"/>)
        ///<br/>
        /// TODO: Why does this not include <see cref="BindingFlags.GetProperty"/>, <see cref="BindingFlags.SetProperty"/>, <see cref="BindingFlags.GetField"/> and <see cref="BindingFlags.SetField"/>?
        ///     If I include the property and field flags, will the flags still work if I pass them to <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>?
        ///     With the current implementation, what is returned when I use the flags with <see cref="Type.GetMembers()"/>?
        /// </summary>
        /// <remarks>
        /// These binding flags will also return <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing fields</a>.
        /// Some methods, such as <see cref="VariableInfoExtensions.GetVariables"/>, specifically filter out backing fields.
        /// </remarks>
        public const BindingFlags VariablesBindingFlags = VariableInfo.VariableBindingFlags;

        /// <summary>
        /// <see cref="BindingFlags"/> that can find <see cref="ConstructorInfo"/> methods.
        /// </summary>
        /// <remarks>
        /// These are insanely stupid:
        /// <ul>
        /// <li>If you don't need <see cref="BindingFlags.CreateInstance"/> to find <see cref="ConstructorInfo"/>, what is it even for?!</li>
        /// <li>How are constructors <see cref="BindingFlags.Instance"/> and not <see cref="BindingFlags.Static"/>?! You clearly don't have an instance yet!</li>
        /// </ul>
        /// </remarks>
        public const BindingFlags ConstructorBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public   |
            BindingFlags.NonPublic;

        #endregion

        internal const string PropertyCaptureGroupName = "property";

        #region Variables

        [Obsolete]
        public static T __ResetAllVariables<T>(this T objectWithVariables)
            where T : class {
            throw new NotImplementedException();
        }

        #endregion

        #region Constructors

        /// <summary>
        /// Returns a <see cref="ConstructorInfo"/> with parameter types matching <see cref="parameterTypes"/> if one exists;
        /// otherwise throws a <see cref="MissingMethodException"/>.
        /// </summary>
        /// <remarks>
        /// This is a wrapper around <see cref="Type.GetConstructor(Type[])"/>
        /// that:
        /// <ul>
        ///     <li>Will throw an exception on failure rather than returning null</li>
        /// <li>Accepts <see cref="Type"/>s as <a href="https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/params">params</a> (aka <a href="https://docs.oracle.com/javase/8/docs/technotes/guides/language/varargs.html">varargs</a> aka <a href="https://en.wikipedia.org/wiki/Variadic_function">variadic</a> aka arbitrary <a href="https://en.wikipedia.org/wiki/Arity">arity</a>)</li>
        /// </ul>
        ///
        /// </remarks>
        /// <param name="type"></param>
        /// <param name="parameterTypes"></param>
        /// <returns></returns>
        /// <exception cref="MissingMethodException"></exception>
        [Pure]
        public static ConstructorInfo MustGetConstructor(this Type type, params Type[] parameterTypes) {
            return type.GetConstructor(ConstructorBindingFlags, null, parameterTypes, null) ?? throw new MissingMethodException($"Could not retrieve a constructor for {type}");
        }

        /// <summary>
        /// <inheritdoc cref="Construct"/>
        /// </summary>
        /// <param name="parameters">the parameters of the constructor to be called</param>
        /// <typeparam name="T">the desired <see cref="Type"/> to be constructed</typeparam>
        /// <returns>an instance of <see cref="T"/> constructed with <paramref name="parameters"/></returns>
        /// <remarks>
        /// <inheritdoc cref="Construct"/>
        /// </remarks>
        public static T Construct<T>(params object?[] parameters) {
            return (T)Construct(typeof(T), parameters);
        }

        /// <summary>
        /// Constructs a new instance of this <see cref="Type"/> using a <see cref="ConstructorInfo"/> that <b>exactly</b> matches the given <see cref="parameters"/>.
        /// </summary>
        /// <remarks>
        /// <ul>
        /// <li>📎 This is based on <see cref="Activator.CreateInstance(System.Type)"/>.</li>
        /// <li>📎 This can call <see cref="BindingFlags.NonPublic"/> constructors.</li>
        /// <li>⚠ Parameters can be <c>null</c>, however this is much more likely to cause an <see cref="AmbiguousMatchException"/>.</li>
        /// <li>⚠ You <b>must</b> specify optional parameters.</li>
        /// <li>📎 To construct a <see cref="Type.ContainsGenericParameters"/>, you must first assign the generic type parameters using <see cref="Type.MakeGenericType"/>.</li>
        /// </ul>
        /// </remarks>
        /// <param name="type">the <see cref="Type"/> to be constructed</param>
        /// <param name="parameters">the parameters of the constructor to be called</param>
        /// <returns>an instance of this <see cref="Type"/> constructed with <paramref name="parameters"/></returns>
        public static object Construct(this Type type, params object?[]? parameters) {
            parameters = parameters.OrEmpty();
            var obj = Activator.CreateInstance(
                type: type,
                bindingAttr: ConstructorBindingFlags,
                binder: null,
                args: parameters,
                culture: null
            );
            return obj;
        }

        #endregion

        #region Generics

        #region Type.Implements(Type)

        /// <summary>
        /// Determines whether this <see cref="Type"/> implements the given interface <see cref="Type"/>.
        /// </summary>
        /// <param name="self">this <see cref="Type"/></param>
        /// <param name="interfaceType">the <see cref="Type.IsInterface"/> that this <see cref="Type"/> might implement</param>
        /// <returns>true if this <see cref="Type"/>, or one of its ancestors, implements <paramref name="interfaceType"/></returns>
        public static bool Implements(this Type self, Type interfaceType) {
            if (interfaceType.IsInterface != true) {
                throw new ArgumentException(nameof(interfaceType), $"The type {interfaceType.Prettify()} was not an interface!");
            }

            // when self is an interface, we have to do some extra checks, because GetInterface() won't return itself
            if (self.IsInterface && SelfImplements(self, interfaceType)) {
                return true;
            }

            // if the interface is a constructed generic, we need to be more specific with the check, because GetInterface() doesn't take generic type parameters into account
            if (interfaceType.IsConstructedGenericType) {
                return interfaceType.IsAssignableFrom(self);
            }

            // finally, if we didn't trigger any special cases, we use GetInterface()
            return self.GetInterface(interfaceType.Name) != null;
        }

        /// <summary>
        /// Used when both <paramref name="self"/> and <paramref name="other"/> <see cref="Type.IsInterface"/>s to see if <paramref name="self"/> implements <paramref name="other"/>.
        /// </summary>
        /// <param name="self"></param>
        /// <param name="other"></param>
        /// <returns></returns>
        private static bool SelfImplements(Type self, Type other) {
            if (self == other) {
                return true;
            }

            if (self.IsGenericType != other.IsGenericType) {
                return false;
            }

            if (self.IsConstructedGenericType && other.IsGenericTypeDefinition) {
                self = self.GetGenericTypeDefinition();
            }

            return other.IsAssignableFrom(self);
        }

        #endregion

        [Pure]
        public static bool IsEnumerable(this Type type) {
            return type.Implements(typeof(IEnumerable<>));
        }

        private static readonly ImmutableHashSet<Type> TupleTypes = ImmutableHashSet.Create(
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            typeof(ITuple),
#endif
            // 📝 typeof(ValueTuple) is a static utility class
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
            // 📝 typeof(Tuple) is a static utility class
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>)
        );

        #endregion

        #region Type Ancestry

        internal static Type CommonType([InstantHandle] IEnumerable<Type?>? types) {
            if (types == null) {
                return typeof(object);
            }

            types = types.ToList();
            var baseClass = CommonBaseClass(types);
            if (baseClass != typeof(object)) {
                return baseClass;
            }

            var commonInterfaces = CommonInterfaces(types);
            return commonInterfaces.FirstOrDefault() ?? typeof(object);
        }

        /// <summary>
        /// Attempts to find the most specific <see cref="Type.BaseType"/> these <see cref="Type"/>s have in common. 
        /// </summary>
        /// <param name="a">a <see cref="Type"/></param>
        /// <param name="b">another <see cref="Type"/></param>
        /// <returns>the most specific <see cref="Type.BaseType"/> these <see cref="Type"/>s have in common</returns>
        /// <remarks>
        /// <b>⚠ WARNING ⚠</b>
        /// <br/>
        /// This is a fairly naive implementation, only going through <see cref="Type.BaseType"/>s.
        /// In particular, it doesn't check for <see cref="CommonInterfaces(System.Type?,System.Type?)"/>,
        /// which is still fairly experimental.</remarks>
        public static Type CommonBaseClass(Type? a, Type? b) {
            Type CommonAncestor(IEnumerable<Type> ancestors, Type y) {
                foreach (var x in ancestors) {
                    if (x.IsParentOf(y)) {
                        return x;
                    }

                    if (y.IsParentOf(x)) {
                        return y;
                    }
                }

                return typeof(object);
            }

            return (a, b) switch {
                (null, null) => typeof(object),
                (_, null)    => a,
                (null, _)    => b,
                _            => CommonAncestor(a.GetAncestors(), b),
            };
        }

        public static Type CommonBaseClass(IEnumerable<Type?> types) {
            Type? mostCommonClass = default;

            foreach (var t in types) {
                mostCommonClass = CommonBaseClass(mostCommonClass, t);
                if (mostCommonClass == typeof(object)) {
                    return mostCommonClass;
                }
            }

            return mostCommonClass ?? typeof(object);
        }

        // [CanBeNull]
        // internal static Type CommonInterface([CanBeNull] Type a, [CanBeNull] Type b) {
        //     var overlap = CommonInterfaces(a, b).ToList();
        //     return overlap.FirstOrDefault();
        // }

        internal static IEnumerable<Type> CommonInterfaces(Type? a, Type? b) {
            var aInts = a.GetAllInterfaces();
            var bInts = b.GetAllInterfaces();
            return aInts.Intersect(bInts);
        }

        [Pure]
        internal static IEnumerable<Type> CommonInterfaces(IEnumerable<Type?> types) {
            return types.Select(it => it.GetAllInterfaces()).Intersection();
        }

        public static IEnumerable<Type> GetAllInterfaces(this Type? type) {
            return _GetAllInterfaces(type);
        }

        [Pure]
        public static Type[] FindInterfaces(this Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return type.FindInterfaces((type1, criteria) => true, default);
        }

        private static IEnumerable<Type> _GetAllInterfaces(this Type? type, IEnumerable<Type>? soFar = default) {
            var ints = (type?.GetInterfaces()).NonNull().ToList();
            soFar = soFar.NonNull();
            if (type?.IsInterface == true) {
                soFar = soFar.Union(type);
            }

            return (soFar ?? new List<Type>())
                   .Union(ints)
                   .Concat(ints.SelectMany(ti => _GetAllInterfaces(ti, soFar)))
                   .Distinct();
        }

        /// <summary>
        /// An idiomatic inverse of <see cref="Type.IsAssignableFrom"/> because I always get confused by that.
        /// </summary>
        /// <param name="valueType"></param>
        /// <param name="variableType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [PublicAPI]
        [Pure]
        public static bool CanBeAssignedTo(this Type valueType, Type variableType) => variableType.IsAssignableFrom(valueType);

        /// <summary>
        /// Determines whether this <see cref="Type"/> is an inheritor of any of the <paramref name="possibleParents"/> via <see cref="Type.IsAssignableFrom"/>
        /// </summary>
        /// <param name="child">this <see cref="Type"/></param>
        /// <param name="possibleParents">the <see cref="Type"/>s that might be <see cref="Type.IsAssignableFrom"/> <paramref name="child"/></param>
        /// <returns>true if any of the <paramref name="possibleParents"/> <see cref="Type.IsAssignableFrom"/> <paramref name="child"/></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="child"/> or <paramref name="possibleParents"/> is null</exception>
        [Pure]
        public static bool IsChildOf(this Type child, [InstantHandle] IEnumerable<Type> possibleParents) => possibleParents.Any(it => it.IsAssignableFrom(child));

        /**
         * <inheritdoc cref="IsChildOf(System.Type,System.Collections.Generic.IEnumerable{System.Type})"/>
         */
        [Pure]
        public static bool Extends(this Type child, [InstantHandle] IEnumerable<Type> possibleParents) => IsChildOf(child, possibleParents);

        /**
         * <inheritdoc cref="IsChildOf(System.Type,System.Collections.Generic.IEnumerable{System.Type})"/>
         */
        [Pure]
        public static bool IsChildOf(this Type child, Type possibleParentType, params Type[] possibleParents) => IsChildOf(child, possibleParents.AsEnumerable().Prepend(possibleParentType));

        /**
         * <inheritdoc cref="IsChildOf(System.Type,System.Collections.Generic.IEnumerable{System.Type})"/>
         */
        [Pure]
        public static bool Extends(this Type child, Type possibleParentType, params Type[] possibleParents) => IsChildOf(child, possibleParentType, possibleParents);

        /// <inheritdoc cref="Type.IsAssignableFrom"/>
        /// <remarks>
        /// This is an alias for <see cref="Type.IsAssignableFrom"/> that isn't horribly confusing.
        /// </remarks>
        [Pure]
        public static bool IsParentOf(this Type parent, Type possibleChild) => parent.CanHoldValueOf(possibleChild);

        /// <summary>
        /// Determines whether a <b>variable</b> of this <see cref="Type"/> is capable of holding a <b>value</b> of <paramref name="valueType"/>.
        /// </summary>
        /// <remarks>
        /// An idiomatic alias for <see cref="Type.IsAssignableFrom"/> because I always get confused by that.
        /// </remarks>
        /// <param name="variableType"></param>
        /// <param name="valueType"></param>
        /// <returns>true if <paramref name="variableType"/> <see cref="Type.IsAssignableFrom"/> <paramref name="valueType"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [PublicAPI]
        [Pure]
        [ContractAnnotation("variableType:null => stop")]
        [ContractAnnotation("valueType:null => stop")]
        public static bool CanHoldValueOf(this Type variableType, Type valueType) {
            if (variableType == null) {
                throw new ArgumentNullException(nameof(variableType));
            }

            if (valueType == null) {
                throw new ArgumentNullException(nameof(valueType));
            }

            return variableType.IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Determines whether a variable of this <see cref="Type"/> is capable of being assigned the given value.
        /// </summary>
        /// <remarks>
        /// This is similar to <see cref="Type.IsInstanceOfType"/>, except that it handles null <paramref name="obj"/> differently by checking
        /// <paramref name="variableType"/> <see cref="Type.IsValueType"/>:
        /// <code><![CDATA[
        /// typeof(int).IsInstanceOfType(null);     // false
        /// typeof(int).CanHold(null);              // false (because int is a value type)
        /// ]]></code>
        ///
        /// <code><![CDATA[
        /// typeof(string).IsInstanceOfType(null);  // false
        /// typeof(string).CanHold(null);           // true  (because string is a reference type)
        /// ]]></code>
        /// </remarks>
        /// <param name="variableType">this <see cref="Type"/></param>
        /// <param name="obj">the <see cref="object"/> that this <see cref="Type"/> might hold</param>
        /// <returns>true if a variable of <paramref name="variableType"/> could hold <paramref name="obj"/></returns>
        /// <exception cref="ArgumentNullException"></exception>
        [Pure]
        public static bool CanHold(this Type variableType, object? obj) {
            if (variableType == null) {
                throw new ArgumentNullException(nameof(variableType));
            }

            if (obj == null) {
                return variableType.IsValueType == false;
            }

            return variableType.IsInstanceOfType(obj);
        }

        [Pure]
        public static bool IsInstanceOf(
            this object obj,
            [InstantHandle]
            IEnumerable<Type> possibleTypes
        ) {
            if (obj == null) {
                throw new ArgumentNullException(nameof(obj));
            }

            if (possibleTypes == null) {
                throw new ArgumentNullException(nameof(possibleTypes));
            }

            return possibleTypes.Any(it => it.IsInstanceOfType(obj));
        }

        [Pure]
        public static bool IsInstanceOf(
            this   object obj,
            params Type[] possibleTypes
        ) => IsInstanceOf(obj, possibleTypes.AsEnumerable());

        #endregion

        #region Type Keywords

        private static readonly Dictionary<Type, string> TypeKeywords = new Dictionary<Type, string>() {
            [typeof(int)]     = "int",
            [typeof(uint)]    = "uint",
            [typeof(short)]   = "short",
            [typeof(ushort)]  = "ushort",
            [typeof(long)]    = "long",
            [typeof(ulong)]   = "ulong",
            [typeof(double)]  = "double",
            [typeof(float)]   = "float",
            [typeof(bool)]    = "bool",
            [typeof(byte)]    = "byte",
            [typeof(decimal)] = "decimal",
            [typeof(sbyte)]   = "sbyte",
            [typeof(char)]    = "char",
            [typeof(object)]  = "object",
            [typeof(string)]  = "string"
        };

        [Pure]
        [ContractAnnotation("null => stop")]
        public static string NameOrKeyword(this Type type) {
            if (type == null) {
                throw new ArgumentNullException(nameof(type));
            }

            return TypeKeywords.GetOrDefault(type, () => type.Name) ?? throw new InvalidOperationException();
        }

        #endregion

        #region Compiler Generation

        public static bool IsCompilerGenerated(this MethodInfo methodInfo) {
            return methodInfo.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
        }

        public static bool IsCompilerGenerated(this Type type) {
            return type.GetCustomAttribute<CompilerGeneratedAttribute>() != null;
        }

        public static bool IsCompilerGenerated(this Delegate dgate) {
            return dgate.Method.DeclaringType.IsCompilerGenerated();
        }

        #endregion

        #region ToString

        public enum Inheritance { Inherited, DeclaredOnly }

        /// <summary>
        /// Returns this <see cref="Type"/>'s override of <see cref="object.ToString"/>, if present.
        /// </summary>
        /// <remarks>
        /// By default, this will return any <see cref="object.ToString"/> method with a <see cref="MemberInfo.DeclaringType"/> other than <see cref="object"/>.
        /// This means that a <see cref="object.ToString"/> method declared in a <b>parent class</b> will be returned.
        /// This behavior can be controlled by specifying <see cref="Inheritance.Inherited"/> or <see cref="Inheritance.DeclaredOnly"/>.
        ///
        /// <p/><b>📝 Note:</b><br/> This will not return <see cref="MethodBase.IsAbstract"/> methods, which includes:
        /// <ul>
        /// <li>Methods declared <c>abstract</c> inside of <c>abstract</c> classes</li>
        /// <li>Methods declared inside of interfaces</li>
        /// </ul>
        /// </remarks>
        /// <example>
        /// Say we have the following classes, where <c>Parent</c> declares an override of <see cref="object.ToString"/>:
        /// <code><![CDATA[
        /// class Parent {
        ///     public override ToString() => "Parent.ToString";
        /// }
        ///
        /// class Child : Parent { }
        /// ]]></code>
        ///
        /// The <see cref="Inheritance"/> parameter determines whether <c>Child.GetToStringOverride()</c> will return <c>Parent</c>'s <see cref="object.ToString"/> method:
        /// <code><![CDATA[
        /// public static void Example(){
        ///     typeof(Parent).GetToStringOverride();               // -> Parent.ToString
        ///     typeof(Parent).GetToStringOverride(Inherited);      // -> Parent.ToString
        ///     typeof(Parent).GetToStringOverride(DeclaredOnly);   // -> Parent.ToString
        ///
        ///     typeof(Child).GetToStringOverride();                // -> Parent.ToString
        ///     typeof(Child).GetToStringOverride(Inherited);       // -> Parent.ToString
        ///     typeof(Child).GetToStringOverride(DeclaredOnly);    // -> null
        /// }
        /// ]]>
        /// </code>
        /// </example>
        /// <param name="type">this <see cref="Type"/></param>
        /// <param name="inheritance">whether to include <see cref="Inheritance.Inherited"/> or <see cref="Inheritance.DeclaredOnly"/> methods</param>
        /// <returns>a non-default override of <see cref="object.ToString"/></returns>
        public static MethodInfo? GetToStringOverride(this Type? type, Inheritance inheritance) {
            if (type == null || type == typeof(object)) {
                return null;
            }

            var toString = type.GetMethod(nameof(ToString), new Type[] { });
            return toString?.IsAbstract == true || toString?.DeclaringType == typeof(object) ? null : toString;
        }

        /// <inheritdoc cref="GetToStringOverride(System.Type?,FowlFever.BSharp.Reflection.ReflectionUtils.Inheritance)"/>
        public static MethodInfo? GetToStringOverride(this Type? type) {
            return GetToStringOverride(type, Inheritance.Inherited);
        }

        #endregion

        /// <summary>
        /// Returns a <see cref="Range"/> describing a <see cref="MethodBase"/>'s <a href="https://en.wikipedia.org/wiki/Arity">arity</a>.
        /// </summary>
        /// <example>
        /// <code><![CDATA[
        /// | Parameters                    | Arity |
        /// |-------------------------------|-------|
        /// | (int a, int b)                | 2..2  |
        /// | ()                            | 0..0  |
        /// | (int a, int b = 0)            | 1..2  |
        /// | (int a = 0, params int[] a)   | 1..^0 |
        /// ]]></code>
        /// </example>
        /// <param name="method"></param>
        /// <returns></returns>
        public static (int min, int? max) Arity(this MethodBase method) {
            var parameters = method.GetParameters();

            if (parameters.Length == 0) {
                return default;
            }

            var  min = 0;
            int? max = 0;

            foreach (var p in parameters) {
                if (p.IsDefined(typeof(ParamArrayAttribute))) {
                    max = null;
                    continue;
                }

                max += 1;
                var isOptional = p.Attributes.HasFlag(ParameterAttributes.Optional) || p.Attributes.HasFlag(ParameterAttributes.HasDefault);
                min += isOptional ? 0 : 1;
            }

            return (min, max);
        }

        /// <summary>
        /// A <see cref="MemberFilter"/> for <see cref="MemberInfo.IsDefined"/>. 
        /// </summary>
        public static readonly MemberFilter FilterWithAttribute = (info, criteria) => info.IsDefined(criteria as Type ?? throw new InvalidOperationException($"{nameof(MemberFilter)} criteria [{criteria}] was not a {nameof(Type)}!"));

        private static readonly IImmutableDictionary<Type, MemberTypes> MemberInfoType_To_MemberTypesFlag = new Dictionary<Type, MemberTypes> {
            [typeof(MemberInfo)]      = MemberTypes.All,
            [typeof(ConstructorInfo)] = MemberTypes.Constructor,
            [typeof(MethodInfo)]      = MemberTypes.Method,
            [typeof(FieldInfo)]       = MemberTypes.Field,
            [typeof(PropertyInfo)]    = MemberTypes.Property,
            [typeof(EventInfo)]       = MemberTypes.Event,
            [typeof(Type)]            = MemberTypes.TypeInfo | MemberTypes.NestedType,
            [typeof(VariableInfo)]    = MemberTypes.Property | MemberTypes.Field,
        }.ToImmutableDictionary();

        internal static MemberTypes GetMemberInfoMemberTypes<T>()
            where T : MemberInfo => MemberInfoType_To_MemberTypesFlag[typeof(T)];

        internal static MemberTypes? GetMemberInfoMemberTypes(Type memberType)
            => MemberInfoType_To_MemberTypesFlag.TryGetValue(memberType, out var mt) ? mt : null;

        internal static BindingFlags GetMemberInfoBindingFlags<T>()
            where T : MemberInfo => GetMemberInfoMemberTypes<T>().GetBindingFlags();

        internal static BindingFlags? GetMemberInfoBindingFlags(Type memberType)
            => GetMemberInfoMemberTypes(memberType)?.GetBindingFlags();

        public static Type MustGetDeclaringType(this MemberInfo member, [CallerArgumentExpression("member")] string? parameterName = default) => member.DeclaringType ?? throw ReflectionException.NoDeclaringTypeException(member, parameterName);
        public static Type MustGetReflectedType(this MemberInfo member, [CallerArgumentExpression("member")] string? parameterName = default) => member.ReflectedType ?? throw ReflectionException.NoReflectedTypeException(member, parameterName);
    }
}