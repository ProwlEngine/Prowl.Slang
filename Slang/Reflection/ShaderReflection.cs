// This file is part of the Prowl Game Engine
// Licensed under the MIT License. See the LICENSE file in the project root for details.

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Prowl.Slang.Native;

using static Prowl.Slang.Native.SlangNative_Deprecated;


namespace Prowl.Slang;


/// <summary>
/// Provides access to reflection information for a shader program.
/// </summary>
public unsafe struct ShaderReflection
{
    internal ComponentType _component;
    internal Native.ShaderReflection* _ptr;


    internal ShaderReflection(Native.ShaderReflection* ptr, ComponentType component)
    {
        ArgumentNullException.ThrowIfNull(ptr, nameof(ptr));

        _component = component;
        _ptr = ptr;
    }

    /// <summary>
    /// Gets the number of top-level parameters in the shader.
    /// </summary>
    public readonly uint ParameterCount =>
        spReflection_GetParameterCount(_ptr);

    /// <summary>
    /// Gets the number of type parameters in the shader.
    /// </summary>
    public readonly uint TypeParameterCount =>
        spReflection_GetTypeParameterCount(_ptr);

    /// <summary>
    /// Gets a specific type parameter by its index.
    /// </summary>
    public readonly TypeParameterReflection GetTypeParameterByIndex(uint index) =>
        new(spReflection_GetTypeParameterByIndex(_ptr, index), _component);

    /// <summary>
    /// Gets an enumeration of all type parameters in the shader.
    /// </summary>
    public readonly IEnumerable<TypeParameterReflection> TypeParameters =>
        Utility.For(TypeParameterCount, GetTypeParameterByIndex);

    /// <summary>
    /// Finds a type parameter by name.
    /// </summary>
    public readonly TypeParameterReflection FindTypeParameter(string name)
    {
        using U8Str str = U8Str.Alloc(name);
        return new(spReflection_FindTypeParameter(_ptr, str), _component);
    }

    /// <summary>
    /// Gets a specific shader parameter by its index.
    /// </summary>
    public readonly VariableLayoutReflection GetParameterByIndex(uint index) =>
        new(spReflection_GetParameterByIndex(_ptr, index), _component);

    /// <summary>
    /// Gets an enumeration of all shader parameters.
    /// </summary>
    public readonly IEnumerable<VariableLayoutReflection> Parameters =>
        Utility.For(ParameterCount, GetParameterByIndex);

    /// <summary>
    /// Gets the number of entry points in the shader.
    /// </summary>
    public readonly uint EntryPointCount =>
        (uint)spReflection_getEntryPointCount(_ptr);

    /// <summary>
    /// Gets a specific entry point by its index.
    /// </summary>
    public readonly EntryPointReflection GetEntryPointByIndex(uint index) =>
        new(spReflection_getEntryPointByIndex(_ptr, index), _component);

    /// <summary>
    /// Gets an enumeration of all entry points in the shader.
    /// </summary>
    public readonly IEnumerable<EntryPointReflection> EntryPoints =>
        Utility.For(EntryPointCount, GetEntryPointByIndex);

    /// <summary>
    /// Gets the binding index for the global constant buffer, if one exists.
    /// </summary>
    public readonly uint GlobalConstantBufferBinding =>
        (uint)spReflection_getGlobalConstantBufferBinding(_ptr);

    /// <summary>
    /// Gets the size in bytes of the global constant buffer, if one exists.
    /// </summary>
    public readonly uint GlobalConstantBufferSize =>
        (uint)spReflection_getGlobalConstantBufferSize(_ptr);

    /// <summary>
    /// Finds a type by its name in the shader.
    /// </summary>
    public readonly TypeReflection FindTypeByName(string name)
    {
        using U8Str str = U8Str.Alloc(name);
        return new(spReflection_FindTypeByName(_ptr, str), _component);
    }

    /// <summary>
    /// Finds a global function by its name in the shader.
    /// </summary>
    public readonly FunctionReflection FindFunctionByName(string name)
    {
        using U8Str str = U8Str.Alloc(name);
        return new(spReflection_FindFunctionByName(_ptr, str), _component);
    }

    /// <summary>
    /// Finds a function by its name within a specific type.
    /// </summary>
    public readonly FunctionReflection FindFunctionByNameInType(TypeReflection type, string name)
    {
        using U8Str str = U8Str.Alloc(name);
        return new(spReflection_FindFunctionByNameInType(_ptr, type._ptr, str), _component);
    }

    /// <summary>
    /// Finds a variable by its name within a specific type.
    /// </summary>
    public readonly VariableReflection FindVarByNameInType(TypeReflection type, string name)
    {
        using U8Str str = U8Str.Alloc(name);
        return new(spReflection_FindVarByNameInType(_ptr, type._ptr, str), _component);
    }

    /// <summary>
    /// Gets the layout information for a specific type according to the specified layout rules.
    /// </summary>
    public readonly TypeLayoutReflection GetTypeLayout(TypeReflection type, LayoutRules rules = LayoutRules.Default) =>
        new(spReflection_GetTypeLayout(_ptr, type._ptr, rules), _component);

    /// <summary>
    /// Finds an entry point by its name.
    /// </summary>
    public readonly EntryPointReflection FindEntryPointByName(string name)
    {
        using U8Str str = U8Str.Alloc(name);
        return new(spReflection_findEntryPointByName(_ptr, str), _component);
    }

    /// <summary>
    /// Specializes a generic type with the provided type arguments.
    /// </summary>
    public readonly TypeReflection SpecializeType(
        TypeReflection type,
        TypeReflection[] specializationArgs,
        out DiagnosticInfo diagnostics)
    {
        Native.TypeReflection** specializationArgsPtr = stackalloc Native.TypeReflection*[specializationArgs.Length];

        for (int i = 0; i < specializationArgs.Length; i++)
            specializationArgsPtr[i] = specializationArgs[i]._ptr;

        Native.TypeReflection* reflectionPtr = spReflection_specializeType(
            _ptr,
            type._ptr,
            specializationArgs.Length,
            specializationArgsPtr,
            out ISlangBlob* diagnosticsPtr);

        Utility.ValidatePtr(reflectionPtr, diagnosticsPtr, out diagnostics);

        return new(reflectionPtr, _component);
    }

    /// <summary>
    /// Specializes a generic declaration with the provided generic arguments.
    /// </summary>
    public readonly GenericReflection SpecializeGeneric(
        GenericReflection generic,
        GenericTypeArgument[] specializationArgs,
        out DiagnosticInfo diagnostics)
    {
        GenericArgType* specializationArgTypes = stackalloc GenericArgType[specializationArgs.Length];
        GenericArgReflection* specializationArgVals = stackalloc GenericArgReflection[specializationArgs.Length];

        for (int i = 0; i < specializationArgs.Length; i++)
        {
            specializationArgTypes[i] = specializationArgs[i].Type;
            specializationArgVals[i] = specializationArgs[i].ToArgReflection();
        }

        Native.GenericReflection* genericPtr = spReflection_specializeGeneric(
            _ptr,
            generic._ptr,
            specializationArgs.Length,
            specializationArgTypes,
            specializationArgVals,
            out ISlangBlob* diagnosticsPtr);

        Utility.ValidatePtr(genericPtr, diagnosticsPtr, out diagnostics);

        return new(genericPtr, _component);
    }

    /// <summary>
    /// Determines whether one type is a subtype of another.
    /// </summary>
    public readonly bool IsSubType(TypeReflection subType, TypeReflection superType) =>
        spReflection_isSubType(_ptr, subType._ptr, superType._ptr);

    /// <summary>
    /// Gets the count of hashed strings in the shader reflection data.
    /// </summary>
    public readonly uint HashedStringCount =>
        (uint)spReflection_getHashedStringCount(_ptr);

    /// <summary>
    /// Gets a hashed string by its index and returns additional count information.
    /// </summary>
    public readonly string GetHashedString(uint index, out uint outCount)
    {
        string hashed = spReflection_getHashedString(_ptr, index, out nuint nOutCount).String;
        outCount = (uint)nOutCount;
        return hashed;
    }

    /// <summary>
    /// Gets the type layout for global shader parameters.
    /// </summary>
    public readonly TypeLayoutReflection GlobalParamsTypeLayout =>
        new(spReflection_getGlobalParamsTypeLayout(_ptr), _component);

    /// <summary>
    /// Gets the variable layout for global shader parameters.
    /// </summary>
    public readonly VariableLayoutReflection GlobalParamsVarLayout =>
        new(spReflection_getGlobalParamsVarLayout(_ptr), _component);

    /// <summary>
    /// Converts the shader reflection information to a JSON string representation.
    /// </summary>
    /// <returns>A JSON string containing the reflection information.</returns>
    public readonly string ToJson()
    {
        spReflection_ToJson(_ptr, null, out ISlangBlob* outJsonPtr)
            .Throw("Failure converting shader reflection to JSON");

        return NativeComProxy.Create(outJsonPtr).GetString();
    }


    /// <inheritdoc/>
    public static bool operator ==(ShaderReflection a, ShaderReflection b)
    {
        return a._ptr == b._ptr;
    }


    /// <inheritdoc/>
    public static bool operator !=(ShaderReflection a, ShaderReflection b)
    {
        return a._ptr != b._ptr;
    }


    /// <inheritdoc/>
    public override bool Equals(object? obj)
    {
        if (obj is ShaderReflection other)
            return other._ptr == _ptr;

        return false;
    }


    /// <inheritdoc/>
    public override int GetHashCode() => ((nint)_ptr).ToInt32();
}
