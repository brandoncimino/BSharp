using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

using FowlFever.BSharp.Collections;

namespace FowlFever.BSharp.Exceptions;

public record StackTraceFilter {
    public Func<StackFrame, bool>? FrameExclusion     { get; init; }
    public Func<MethodBase, bool>? MethodExclusion    { get; init; }
    public Func<Module, bool>?     ModuleExclusion    { get; init; }
    public Func<Assembly, bool>?   AssemblyExclusion  { get; init; }
    public Func<Type, bool>?       TypeExclusion      { get; init; }
    public Func<Attribute, bool>?  AttributeExclusion { get; init; }

    /// <summary>
    /// Whether we should exclude things annotated with the built-in <see cref="StackTraceHiddenAttribute"/>.
    /// </summary>
    public bool RespectStackTraceHiddenAttribute { get; init; } = true;

    public bool ShouldExclude(StackFrame? stackFrame) => stackFrame != null && (FrameExclusion.OrFalse(stackFrame) || ShouldExclude(stackFrame.GetMethod()));

    public bool ShouldExclude(MethodBase? method) {
        if (method == null) {
            return false;
        }

        return MethodExclusion.OrFalse(method)     ||
               ShouldExclude(method.DeclaringType) ||
               ShouldExclude(method.Module)        ||
               ShouldExclude(method.GetCustomAttributes());
    }

    public bool ShouldExclude(IEnumerable<Attribute?>? attributes) => AttributeExclusion != null && attributes.OrEmpty().Any(ShouldExclude);
    public bool ShouldExclude(Attribute?               attribute)  => attribute          != null && AttributeExclusion.OrFalse(attribute);
    public bool ShouldExclude(Type?                    type)       => type               != null && (TypeExclusion.OrFalse(type)         || ShouldExclude(type.GetCustomAttributes()));
    public bool ShouldExclude(Module?                  module)     => module             != null && (ModuleExclusion.OrFalse(module)     || ShouldExclude(module.Assembly) || ShouldExclude(module.GetCustomAttributes()));
    public bool ShouldExclude(Assembly?                assembly)   => assembly           != null && (AssemblyExclusion.OrFalse(assembly) || ShouldExclude(assembly.GetCustomAttributes()));
}