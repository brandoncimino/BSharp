using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

using FowlFever.BSharp.Attributes;
using FowlFever.BSharp.Collections;
using FowlFever.BSharp.Exceptions;
using FowlFever.BSharp.Strings;
using FowlFever.BSharp.Strings.Prettifiers;

using JetBrains.Annotations;

using Pure = System.Diagnostics.Contracts.PureAttribute;

namespace FowlFever.BSharp.Reflection {
    /// <summary>
    /// Contains utilities for <see cref="System.Reflection"/>.
    ///
    /// TODO: Split this class up. For example, the <see cref="Type"/> extensions should be in their own class
    /// </summary>
    [PublicAPI]
    public static class ReflectionUtils {
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
        /// Some methods, such as <see cref="GetVariables"/>, specifically filter out backing fields.
        /// </remarks>
        public const BindingFlags VariablesBindingFlags =
            BindingFlags.Default   |
            BindingFlags.Instance  |
            BindingFlags.NonPublic |
            BindingFlags.Public    |
            BindingFlags.Static;

        public const BindingFlags ConstructorBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Public   |
            BindingFlags.NonPublic;

        public const BindingFlags AutoPropertyBackingFieldBindingFlags =
            BindingFlags.Instance |
            BindingFlags.Static   |
            BindingFlags.NonPublic;

        #endregion

        private const string PropertyCaptureGroupName = "property";

        #region Variables

        /// <summary>
        /// Returns all of the <see cref="IsVariable"/> <see cref="MemberInfo"/>s from the given <paramref name="type"/>.
        /// </summary>
        /// <remarks>
        /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
        /// It does <b>not</b> include <see cref="IsAutoPropertyBackingField">backing fields</see>.
        /// </remarks>
        /// <param name="type">The <see cref="Type"/> to retrieve the fields and properties of.</param>
        /// <returns>
        /// </returns>
        public static IEnumerable<MemberInfo> GetVariables(this Type type) {
            var properties = type.GetProperties(VariablesBindingFlags).Cast<MemberInfo>();
            var fields     = type.GetFields(VariablesBindingFlags).Cast<MemberInfo>();
            return properties.Union(fields).Where(IsVariable);
        }

        /// <summary>
        /// Returns the <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> with the the <see cref="MemberInfo.Name"/> <paramref name="variableName"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> that should have a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></param>
        /// <param name="variableName">The expected <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> <see cref="MemberInfo.Name"/></param>
        /// <returns>The <see cref="FieldInfo"/> or <see cref="PropertyInfo"/> named <paramref name="variableName"/></returns>
        /// <exception cref="MissingMemberException">If <paramref name="variableName"/> couldn't be retrieved</exception>
        [Pure]
        public static MemberInfo GetVariableInfo(this Type type, string variableName) {
            var prop = type.GetProperty(variableName, VariablesBindingFlags);

            if (prop != null) {
                return prop;
            }

            var field = type.GetField(variableName, VariablesBindingFlags);
            return field ?? throw ReflectionException.VariableNotFoundException(type, variableName);
        }

        public static T ResetAllVariables<T>(this T objectWithVariables)
            where T : class {
            throw new NotImplementedException();

            var variables         = objectWithVariables.GetType().GetVariables();
            var settableVariables = variables.Where(IsSettable);
            foreach (var vInfo in settableVariables) {
                SetVariableValue(objectWithVariables, vInfo.Name, vInfo);
            }

            return objectWithVariables;
        }

        /// <param name="fieldOrProperty">either a <see cref="FieldInfo"/> or <see cref="PropertyInfo"/></param>
        /// <returns>true if the <paramref name="fieldOrProperty"/> <see cref="IsSettable(FieldInfo)"/> / <see cref="IsSettable(PropertyInfo)"/></returns>
        public static bool IsSettable(this MemberInfo fieldOrProperty) {
            return fieldOrProperty switch {
                PropertyInfo p => IsSettable(p),
                FieldInfo f    => IsSettable(f),
                _              => false
            };
        }

        /// <param name="propertyInfo">a <see cref="PropertyInfo"/></param>
        /// <returns><see cref="PropertyInfo.CanWrite"/></returns>
        private static bool IsSettable(this PropertyInfo propertyInfo) {
            return propertyInfo.CanWrite;
        }

        /// TODO: Should this have some way of actually checking for the <c>readonly</c> keyword...?
        private static bool IsSettable(this FieldInfo fieldInfo) {
            return !IsAutoPropertyBackingField(fieldInfo);
        }

        /// <summary>
        /// <inheritdoc cref="GetVariableInfo"/>
        /// </summary>
        /// <param name="obj">An object to infer <typeparamref name="T"/> from</param>
        /// <param name="variableName"><inheritdoc cref="GetVariableInfo"/></param>
        /// <typeparam name="T">The type to retrieve <paramref name="variableName"/> from</typeparam>
        /// <returns><inheritdoc cref="GetVariableInfo"/></returns>
        [Pure]
        public static MemberInfo GetVariableInfo<T>(T obj, string variableName) {
            return typeof(T).GetVariableInfo(variableName);
        }

        /// <summary>
        /// Checks if a <see cref="MemberInfo"/> is a "variable", and therefore appropriate for use with "variable"-referencing methods like <see cref="GetVariableValue{T}"/>.
        /// </summary>
        /// <remarks>
        /// "Variables" includes both <see cref="Type.GetProperties()"/> and <see cref="Type.GetFields()"/>.
        /// It does <b>not</b> include <see cref="IsAutoPropertyBackingField">backing fields</see>.
        /// </remarks>
        /// <param name="memberInfo">a <see cref="MemberInfo"/></param>
        /// <returns>true if the <see cref="MemberInfo"/> is a variable</returns>
        [Pure]
        public static bool IsVariable(this MemberInfo memberInfo) {
            return memberInfo switch {
                PropertyInfo _ => true,
                FieldInfo f    => !IsAutoPropertyBackingField(f),
                _              => false
            };
        }

        /// <summary>
        /// <inheritdoc cref="GetVariableInfo"/>
        /// </summary>
        /// <param name="variableName"><inheritdoc cref="GetVariableInfo"/></param>
        /// <typeparam name="T">The type to retrieve <paramref name="variableName"/> from</typeparam>
        /// <returns><inheritdoc cref="GetVariableInfo"/></returns>
        [Pure]
        public static MemberInfo GetVariableInfo<T>(string variableName) {
            return typeof(T).GetVariableInfo(variableName);
        }

        /// <summary>
        /// Returns the <b>value</b> (either <see cref="PropertyInfo"/>.<see cref="PropertyInfo.GetValue(object)"/> or <see cref="FieldInfo"/>.<see cref="FieldInfo.GetValue"/>)
        /// of the <see cref="GetVariableInfo"/> named <paramref name="variableName"/>.
        /// </summary>
        /// <param name="obj">The object to retrieve the value from</param>
        /// <param name="variableName">The name of the variable</param>
        /// <typeparam name="T">The <b>return type</b> of <b><paramref name="variableName"/></b></typeparam>
        /// <returns>The value of the <see cref="GetVariableInfo"/></returns>
        /// <exception cref="InvalidCastException">If the retrieved value cannot be cast to <typeparamref name="T"/></exception>
        /// <exception cref="MissingMemberException"><inheritdoc cref="GetVariableInfo"/></exception>
        [Pure]
        public static T GetVariableValue<T>(object obj, string variableName) {
            var variableInfo = obj.GetType().GetVariableInfo(variableName);
            var value = variableInfo switch {
                PropertyInfo p => p.GetValue(obj),
                FieldInfo f    => f.GetValue(obj),
                _              => throw ReflectionException.NotAVariableException(variableInfo)
            };

            try {
                return (T)value;
            }
            catch (InvalidCastException e) {
                throw e.ModifyMessage($"A member for {variableInfo.Prettify(TypeNameStyle.Full)} was found on the [{obj.GetType().Name}]'{obj}', but it couldn't be cast to a {typeof(T).PrettifyType(default)}!");
            }
        }

        /// <summary>
        /// <inheritdoc cref="GetVariableValue{T}"/>
        /// </summary>
        /// <param name="obj"><inheritdoc cref="GetVariableValue{T}"/></param>
        /// <param name="variableName"><inheritdoc cref="GetVariableValue{T}"/></param>
        /// <returns><inheritdoc cref="GetVariableValue{T}"/></returns>
        [Pure]
        public static object GetVariableValue(object obj, string variableName) {
            return GetVariableValue<object>(obj, variableName);
        }

        public static void SetVariableValue<T>(object obj, string variableName, T newValue) {
            var v = obj.GetType().GetVariableInfo(variableName);
            switch (v) {
                case PropertyInfo prop:
                    prop.SetValue(obj, newValue);
                    return;
                case FieldInfo field:
                    field.SetValue(obj, newValue);
                    return;
                default:
                    throw new MemberAccessException($"Unable to set the value of {v.Prettify()}!");
            }
        }

        #endregion

        #region Backing Fields

        #region Operations on Fields

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static IEnumerable<string> Get_BackingFieldFor_Names(FieldInfo fieldInfo) {
            return fieldInfo.GetCustomAttributes<BackingFieldForAttribute>().Select(it => it.BackedPropertyName);
        }

        [Pure]
        private static IEnumerable<PropertyInfo> Get_BackingFieldFor_Properties(FieldInfo fieldInfo, Type owningType = default) {
            owningType ??= GetOwningType(fieldInfo);
            var backedPropertyNames = Get_BackingFieldFor_Names(fieldInfo);
            return owningType.GetProperties()
                             .Where(it => backedPropertyNames.Contains(it.Name));
        }

        /// <param name="memberInfo"></param>
        /// <returns><c>true</c> if <paramref name="memberInfo"/> is an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property Backing Field</a></returns>
        [Pure]
        internal static bool IsAutoPropertyBackingField(this FieldInfo memberInfo) {
            return Regex.IsMatch(memberInfo.Name, GetAutoPropertyBackingFieldNamePattern());
        }

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static bool UsedIn_BackedBy(FieldInfo fieldInfo) {
            // TODO: pass to other method
            var owningType = fieldInfo.ReflectedType ?? fieldInfo.DeclaringType ?? throw ReflectionException.NoOwningTypeException(fieldInfo);
            return owningType.GetProperties(VariablesBindingFlags)
                             .Select(Get_BackedBy_BackingFieldName)
                             .Any(it => it == fieldInfo.Name);
        }

        [Pure]
        private static bool IsAnnotatedBackingField(FieldInfo fieldInfo) {
            return Get_BackingFieldFor_Names(fieldInfo).IsNotEmpty() || UsedIn_BackedBy(fieldInfo);
        }

        [PublicAPI]
        [Pure]
        public static bool IsBackingField(this MemberInfo fieldInfo) {
            return (fieldInfo as FieldInfo)?.IsBackingField() ?? throw ReflectionException.NotAFieldException(fieldInfo);
        }

        [PublicAPI]
        [Pure]
        public static bool IsBackingField(this FieldInfo fieldInfo) {
            return IsAutoPropertyBackingField(fieldInfo) || IsAnnotatedBackingField(fieldInfo);
        }

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        internal static string? Get_BackedAutoProperty_Name(FieldInfo fieldInfo) {
            var matches = Regex.Match(fieldInfo.Name, GetAutoPropertyBackingFieldNamePattern());
            return matches.Success ? matches.Groups[PropertyCaptureGroupName].Value : null;
        }

        [Pure]
        private static PropertyInfo? Get_BackedAutoProperty(FieldInfo fieldInfo, Type owningType = default) {
            owningType ??= GetOwningType(fieldInfo);
            var autoPropertyName = Get_BackedAutoProperty_Name(fieldInfo);
            return autoPropertyName == null ? null : owningType.GetProperty(autoPropertyName);
        }

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static IEnumerable<PropertyInfo> Get_PropertiesAnnotatedWith_BackedBy(FieldInfo fieldInfo, Type owningType = default) {
            owningType ??= GetOwningType(fieldInfo);
            return owningType.GetProperties(VariablesBindingFlags)
                             .Where(it => Get_BackedBy_BackingFieldName(it) == fieldInfo.Name);
        }

        [PublicAPI]
        public static IEnumerable<PropertyInfo> BackedProperties(this FieldInfo fieldInfo) {
            var owningType = GetOwningType(fieldInfo);

            var propertiesFromFieldAnnotations = Get_BackingFieldFor_Properties(fieldInfo, owningType);
            var propertiesReferencingThisField = Get_PropertiesAnnotatedWith_BackedBy(fieldInfo, owningType);
            var autoProperty                   = Get_BackedAutoProperty(fieldInfo, owningType);

            return propertiesFromFieldAnnotations
                   .Union(propertiesReferencingThisField)
                   .Append(autoProperty)
                   .NonNull();
        }

        #endregion

        #region Operations on Properties

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static string? Get_BackedBy_BackingFieldName(PropertyInfo propertyInfo) {
            return propertyInfo.GetCustomAttribute<BackedByAttribute>()?.BackingFieldName;
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static IEnumerable<FieldInfo> Get_FieldsAnnotatedWith_BackingFieldFor(PropertyInfo propertyInfo, Type owningType = default) {
            owningType ??= GetOwningType(propertyInfo);
            return owningType.GetFields(VariablesBindingFlags)
                             .Where(it => Get_BackingFieldFor_Names(it).Contains(propertyInfo.Name));
        }

        private static FieldInfo? Get_BackedBy_Field(PropertyInfo propertyInfo, Type owningType = default) {
            owningType ??= GetOwningType(propertyInfo);
            var backingFieldName = Get_BackedBy_BackingFieldName(propertyInfo);
            return backingFieldName == null ? null : owningType.GetField(backingFieldName, VariablesBindingFlags);
        }

        [Pure]
        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static string GetAutoPropertyBackingFieldName(PropertyInfo propertyInfo) {
            return $"<{propertyInfo.Name}>k__BackingField";
        }

        [Pure]
        [PublicAPI]
        public static FieldInfo? BackingField(this MemberInfo memberInfo) {
            return (memberInfo as PropertyInfo)?.BackingField() ?? throw ReflectionException.NotAPropertyException(memberInfo);
        }

        /// <summary>
        /// Returns the "backing field" for a <see cref="PropertyInfo"/>.
        ///
        /// A "backing field" can be:
        /// <ul>
        /// <li>an <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing field</a></li>
        /// <li><paramref name="propertyInfo"/>'s [<see cref="BackedByAttribute.BackingFieldName"/>].<see cref="BackedByAttribute"/></li>
        /// <li>any <see cref="BackingFieldForAttribute"/> whose [<see cref="BackingFieldForAttribute.BackedProperty"/>].<see cref="BackingFieldForAttribute"/> matches <paramref name="propertyInfo"/></li>
        /// </ul>
        /// TODO: Improve this so that the <see cref="PropertyInfo"/> can reference <see cref="FieldInfo"/>s, which can then "chain" down into a <see cref="FieldInfo"/>
        /// </summary>
        /// <param name="propertyInfo">this <see cref="FieldInfo"/></param>
        /// <returns>the <see cref="FieldInfo"/> that backs <paramref name="propertyInfo"/>, if found; otherwise, null</returns>
        [PublicAPI]
        [Pure]
        public static FieldInfo? BackingField(this PropertyInfo propertyInfo) {
            var owningType = GetOwningType(propertyInfo);

            return GetAutoPropertyBackingField(propertyInfo, owningType) ??
                   Get_BackedBy_Field(propertyInfo, owningType) ??
                   Get_FieldsAnnotatedWith_BackingFieldFor(propertyInfo, owningType).Single();
        }

        #endregion

        /// <remarks>
        /// <paramref name="propertyNamePattern"/> will be stored in a capture group named <see cref="PropertyCaptureGroupName"/>.
        /// </remarks>
        /// <param name="propertyNamePattern">A pattern to match the auto-property. Defaults to <c>.*</c></param>
        /// <returns>A <see cref="Regex"/> pattern that will match <a href="https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/auto-implemented-properties">Auto-Property backing names</a>.</returns>
        [Pure]
        private static string GetAutoPropertyBackingFieldNamePattern(
            [RegexPattern]
            string propertyNamePattern = ".*"
        ) {
            return $"<(?<{PropertyCaptureGroupName}>{propertyNamePattern})>k__BackingField";
        }

        [SuppressMessage("ReSharper", "SuggestBaseTypeForParameter")]
        private static FieldInfo? GetAutoPropertyBackingField(PropertyInfo propertyInfo, Type owningType = default) {
            owningType ??= GetOwningType(propertyInfo);
            return owningType.GetField(GetAutoPropertyBackingFieldName(propertyInfo), AutoPropertyBackingFieldBindingFlags);
        }

        #endregion

        private static Type GetOwningType(MemberInfo memberInfo) {
            return memberInfo.ReflectedType ?? memberInfo.DeclaringType ?? throw ReflectionException.NoOwningTypeException(memberInfo);
        }

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
        public static ConstructorInfo EnsureConstructor(this Type type, params Type[] parameterTypes) {
            return type.GetConstructor(ConstructorBindingFlags, null, parameterTypes, null) ?? throw new MissingMethodException($"Could not retrieve a constructor for {type}");
        }

        private static TOut Construct<TOut>(Type[] parameterTypes, object[] parametersValues) {
            try {
                var constructor = typeof(TOut).EnsureConstructor(parameterTypes);
                return (TOut)constructor.Invoke(parametersValues);
            }
            catch (Exception e) {
                throw e.PrependMessage($"Could not construct an instance of {typeof(TOut).Name} with the parameters {parametersValues}!");
            }
        }

        /// <summary>
        /// Constructs a new instance of <see cref="TOut"/> using a constructor that <b>exactly</b> matches the given <see cref="parameters"/>.
        ///
        /// <p/><b>NOTE:</b><br/>
        /// This must match the desired constructor <b>EXACTLY</b> - that means that the parameter types must be exact, and that optional parameters must be present!
        ///
        /// </summary>
        /// <remarks>
        /// I tried to make this an extension method of <see cref="Type"/>, but couldn't get it to work:
        /// Unfortunately, I don't think it's possible, in C#, to go from a <see cref="Type"/> to a generic type parameter in a way that the
        /// compiler knows about - i.e., in a way equivalent to Java's <![CDATA[Class<T>]]>.
        ///
        /// This means that making this a <see cref="Type"/> extension method is redundant with <typeparamref name="TOut"/>,
        /// other than allowing this to be an extension method and thus giving you a way to find it that is semantically similar
        /// to <see cref="Type.GetConstructor(System.Reflection.BindingFlags,System.Reflection.Binder,System.Reflection.CallingConventions,System.Type[],System.Reflection.ParameterModifier[])">Type.GetConstructor()</see>.
        /// </remarks>
        /// <param name="parameters">the parameters of the constructor to be called</param>
        /// <typeparam name="TOut">the desired type to be constructed</typeparam>
        /// <returns>an instance of <see cref="TOut"/> constructed with <paramref name="parameters"/></returns>
        public static TOut Construct<TOut>(params object[] parameters) {
            return Construct<TOut>(
                parameters.Select(it => it.GetType()).ToArray(),
                parameters
            );
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
        public static bool Implements(this Type? self, Type interfaceType) {
            if (interfaceType?.IsInterface != true) {
                throw new ArgumentException(nameof(interfaceType), $"The type {interfaceType.Prettify()} was not an interface!");
            }

            // null can't implement anything
            if (self == null) {
                return false;
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

        [ContractAnnotation("null => false")]
        public static bool IsEnumerable(this Type? type) {
            return type.Implements(typeof(IEnumerable<>));
        }

        private static readonly Type[] TupleTypes = {
            typeof(ValueTuple),
            typeof(ValueTuple<>),
            typeof(ValueTuple<,>),
            typeof(ValueTuple<,,>),
            typeof(ValueTuple<,,,>),
            typeof(ValueTuple<,,,,>),
            typeof(ValueTuple<,,,,,>),
            typeof(ValueTuple<,,,,,,>),
            typeof(ValueTuple<,,,,,,,>),
            // NOTE: typeof(Tuple) is a static utility class
            typeof(Tuple<>),
            typeof(Tuple<,>),
            typeof(Tuple<,,>),
            typeof(Tuple<,,,>),
            typeof(Tuple<,,,,>),
            typeof(Tuple<,,,,,>),
            typeof(Tuple<,,,,,,>),
            typeof(Tuple<,,,,,,,>),
        };

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

        internal static Type CommonBaseClass(IEnumerable<Type> types) {
            Type? mostCommonType = default;

            foreach (var t in types) {
                mostCommonType = CommonBaseClass(mostCommonType, t);
                if (mostCommonType == typeof(object)) {
                    return mostCommonType;
                }
            }

            return mostCommonType ?? typeof(object);
        }

        public static Type? CommonBaseClass(Type? a, Type? b) {
            if (a == null || b == null) {
                return a ?? b;
            }

            while (true) {
                if (a == b) {
                    return a;
                }

                if (a.IsAssignableFrom(b)) {
                    return a;
                }

                if (b.IsAssignableFrom(a)) {
                    return b;
                }

                if (a.BaseType == null) {
                    return typeof(object);
                }

                a = a.BaseType;
            }
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

        private static IEnumerable<Type> Ancestors(this Type type) {
            var ancestors = new List<Type>();

            while (type != null) {
                ancestors.Add(type);
                type = type.BaseType;
            }

            return ancestors;
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
        [ContractAnnotation("valueType:null => stop")]
        [ContractAnnotation("variableType:null => stop")]
        public static bool CanBeAssignedTo(this Type valueType, Type variableType) {
            if (valueType == null) {
                throw new ArgumentNullException(nameof(valueType));
            }

            if (variableType == null) {
                throw new ArgumentNullException(nameof(variableType));
            }

            return variableType.IsAssignableFrom(valueType);
        }

        /// <summary>
        /// Determines whether this <see cref="Type"/> is an inheritor of any of the <paramref name="possibleParents"/> via <see cref="Type.IsAssignableFrom"/>
        /// </summary>
        /// <param name="child">this <see cref="Type"/></param>
        /// <param name="possibleParents">the <see cref="Type"/>s that might be <see cref="Type.IsAssignableFrom"/> <paramref name="child"/></param>
        /// <returns>true if any of the <paramref name="possibleParents"/> <see cref="Type.IsAssignableFrom"/> <paramref name="child"/></returns>
        /// <exception cref="ArgumentNullException">if <paramref name="child"/> or <paramref name="possibleParents"/> is null</exception>
        [Pure]
        [ContractAnnotation("child:null => stop")]
        [ContractAnnotation("possibleParents:null => stop")]
        public static bool IsChildOf(this Type child, [InstantHandle] IEnumerable<Type> possibleParents) {
            if (child == null) {
                throw new ArgumentNullException(nameof(child));
            }

            if (possibleParents == null) {
                throw new ArgumentNullException(nameof(possibleParents));
            }

            return possibleParents.Any(it => it.IsAssignableFrom(child));
        }

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

        #region Type Types

        /// <remarks>
        /// This is only necessary in .NET Standard 2.0, because in .NET Standard 2.1, an <a href="https://docs.microsoft.com/en-us/dotnet/api/System.Runtime.CompilerServices.ITuple?view=netframework-4.7.1">ITuple</a> interface is available.
        /// </remarks>
        /// <param name="type">this <see cref="Type"/></param>
        /// <returns>true if this <see cref="Type"/> is one of the <see cref="Tuple{T}"/> or <see cref="ValueTuple{T1}"/> types</returns>
        public static bool IsTupleType(this Type type) {
            return type.IsGenericType && TupleTypes.Any(it => type.GetGenericTypeDefinition().IsAssignableFrom(it));
        }

        /// <param name="type">this <see cref="Type"/></param>
        /// <returns>true if this <see cref="Type"/> inherits from <see cref="Exception"/></returns>
        public static bool IsExceptionType(this Type type) {
            return typeof(Exception).IsAssignableFrom(type);
        }

        public static bool IsNullable(this Type type) {
            return type.IsNullableValueType() || type.IsNullableReferenceType();
        }

        public static bool IsNullableValueType(this Type type) {
            return Nullable.GetUnderlyingType(type) != null;
        }

        private static readonly HashSet<string> NullableAttributes = new HashSet<string>() {
            "System.Runtime.CompilerServices.NullableAttribute", "JetBrains.Annotations.CanBeNullAttribute"
        };

        public static bool IsNullableReferenceType(this Type type) {
            return !type.IsValueType && (type._IsNullableAnnotated() || type._IsNullableAnnotated());
        }

        /// <summary>
        /// The comment <a href="https://stackoverflow.com/a/58454489">here</a> has some big check for constructor arguments and bytes instead of just the presence of the attribute...is that necessary at all...?
        /// </summary>
        /// <param name="memberInfo"></param>
        /// <returns></returns>
        private static bool _IsNullableAnnotated(this MemberInfo memberInfo) {
            var attributes = memberInfo.GetCustomAttributesData().ToList();
            return attributes.FirstOrDefault(it => NullableAttributes.Contains(it.AttributeType.FullName))?._GetNullableByte() == 2;
        }

        private static bool _IsInsideNullableContext(this MemberInfo memberInfo) {
            var outerAttributes = memberInfo.DeclaringType?.GetCustomAttributesData().ToList();
            return outerAttributes?.FirstOrDefault(it => it.AttributeType.FullName == "System.Runtime.CompilerServices.NullableContextAttribute")?._GetNullableByte() == 2;
        }

        public static bool IsNullable(this ParameterInfo parameterInfo) {
            if (parameterInfo.ParameterType.IsValueType) {
                return parameterInfo.ParameterType.IsNullable();
            }

            return parameterInfo.CustomAttributes.Any(it => NullableAttributes.Contains(it.AttributeType.FullName)) ||
                   parameterInfo.ParameterType.IsNullable();
        }

        public static bool IsNullable(this MethodInfo methodInfo) {
            if (methodInfo.ReturnParameter == null) {
                return false;
            }

            return methodInfo._IsNullableAnnotated()     ||
                   methodInfo._IsInsideNullableContext() ||
                   methodInfo.ReturnParameter.IsNullable();
        }

        public static bool IsNullable(this MemberInfo memberInfo) {
            if (memberInfo._IsNullableAnnotated() || memberInfo._IsInsideNullableContext()) {
                return true;
            }

            return memberInfo switch {
                FieldInfo f    => f.FieldType.IsNullable(),
                PropertyInfo p => p.PropertyType.IsNullable(),
                MethodInfo m   => m.ReturnType.IsNullable(),
                Type t         => t.IsNullable(),
                _              => false
            };
        }

        private static byte? _GetNullableByte(this CustomAttributeData attribute) {
            var arg = attribute.ConstructorArguments.FirstOrDefault();

            if (arg.ArgumentType == typeof(byte)) {
                return arg.Value as byte?;
            }

            if (arg.ArgumentType == typeof(byte[])) {
                var argArgs = (ReadOnlyCollection<CustomAttributeTypedArgument>)arg.Value!;
                return argArgs.FirstOrDefault().Value as byte?;
            }

            return null;
        }

        #endregion
    }
}