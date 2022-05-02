namespace System.Runtime.CompilerServices; 

[AttributeUsage(AttributeTargets.Parameter, Inherited = default, AllowMultiple = false)]
internal sealed class CallerArgumentExpressionAttribute : Attribute
{
#if !NET6_0_OR_GREATER
    public CallerArgumentExpressionAttribute(string parameterName)
    {
        ParameterName = parameterName;
    }

    public string ParameterName { get; }
#endif
}