namespace System.Runtime.CompilerServices;

[AttributeUsage(AttributeTargets.Parameter, Inherited = default, AllowMultiple = false)]
public sealed class CallerArgumentExpressionAttribute : Attribute {
#if !NET6_0_OR_GREATER
    public CallerArgumentExpressionAttribute(string parameterName) {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
#endif
}