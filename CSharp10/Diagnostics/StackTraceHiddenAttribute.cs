// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if !NET6_0_OR_GREATER
namespace System.Diagnostics {
    /// <summary>
    /// Types and Methods attributed with StackTraceHidden will be omitted from the stack trace text shown in StackTrace.ToString()
    /// and Exception.StackTrace
    /// </summary>
    /// <remarks>
    /// This is <b>Brandon's copy</b> of this class, used for CSharp10 compatibility.
    /// It was taken from <a href="https://source.dot.net/#System.Private.CoreLib/StackTraceHiddenAttribute.cs,ed25bd1a32997f62">source.dot.net</a>.
    ///<p/>
    /// <b>📎 NOTE:</b> I couldn't get conditional compilation to work, so instead, this class is just <c>public</c>,
    /// and it should be referenced in consuming <c>.csproj</c> files with the <c><![CDATA[<PrivateAssets>all</PrivateAssets>]]></c> option
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method | AttributeTargets.Constructor | AttributeTargets.Struct, Inherited = false)]
    public sealed class StackTraceHiddenAttribute : Attribute { }
}
#endif