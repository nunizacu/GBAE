﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace GBAE
{

    // Both ITokenResolver and DynamicMethodTokenResolver classes 
    // thx to https://stackoverflow.com/questions/4148297/resolving-the-tokens-found-in-the-il-from-a-dynamic-method/35711376
    public interface ITokenResolver
    {
        MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);
        Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);
        FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);
        MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments);
        byte[] ResolveSignature(int metadataToken);
        string ResolveString(int metadataToken);
    }

    public sealed class DynamicMethodTokenResolver : ITokenResolver
    {
        private delegate void TokenResolver(int token, out IntPtr typeHandle, out IntPtr methodHandle, out IntPtr fieldHandle);
        private delegate string StringResolver(int token);
        private delegate byte[] SignatureResolver(int token, int fromMethod);
        private delegate Type GetTypeFromHandleUnsafe(IntPtr handle);

        private readonly TokenResolver tokenResolver;
        private readonly StringResolver stringResolver;
        private readonly SignatureResolver signatureResolver;
        private readonly GetTypeFromHandleUnsafe getTypeFromHandleUnsafe;
        private readonly MethodInfo getMethodBase;
        private readonly ConstructorInfo runtimeMethodHandleInternalCtor;
        private readonly ConstructorInfo runtimeFieldHandleStubCtor;
        private readonly MethodInfo getFieldInfo;

        public DynamicMethodTokenResolver(DynamicMethod dynamicMethod)
        {
            var resolver = typeof(DynamicMethod).GetField("m_resolver", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(dynamicMethod);
            if (resolver == null) throw new ArgumentException("The dynamic method's IL has not been finalized.");

            tokenResolver = (TokenResolver)resolver.GetType().GetMethod("ResolveToken", BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate(typeof(TokenResolver), resolver);
            stringResolver = (StringResolver)resolver.GetType().GetMethod("GetStringLiteral", BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate(typeof(StringResolver), resolver);
            signatureResolver = (SignatureResolver)resolver.GetType().GetMethod("ResolveSignature", BindingFlags.Instance | BindingFlags.NonPublic).CreateDelegate(typeof(SignatureResolver), resolver);

            getTypeFromHandleUnsafe = (GetTypeFromHandleUnsafe)typeof(Type).GetMethod("GetTypeFromHandleUnsafe", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { typeof(IntPtr) }, null).CreateDelegate(typeof(GetTypeFromHandleUnsafe), null);
            var runtimeType = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeType");

            var runtimeMethodHandleInternal = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeMethodHandleInternal");
            getMethodBase = runtimeType.GetMethod("GetMethodBase", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { runtimeType, runtimeMethodHandleInternal }, null);
            runtimeMethodHandleInternalCtor = runtimeMethodHandleInternal.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(IntPtr) }, null);

            var runtimeFieldInfoStub = typeof(RuntimeTypeHandle).Assembly.GetType("System.RuntimeFieldInfoStub");
            runtimeFieldHandleStubCtor = runtimeFieldInfoStub.GetConstructor(BindingFlags.Instance | BindingFlags.Public, null, new[] { typeof(IntPtr), typeof(object) }, null);
            getFieldInfo = runtimeType.GetMethod("GetFieldInfo", BindingFlags.Static | BindingFlags.NonPublic, null, new[] { runtimeType, typeof(RuntimeTypeHandle).Assembly.GetType("System.IRuntimeFieldInfo") }, null);
        }

        public Type ResolveType(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            IntPtr typeHandle, methodHandle, fieldHandle;
            tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

            return getTypeFromHandleUnsafe.Invoke(typeHandle);
        }

        public MethodBase ResolveMethod(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            IntPtr typeHandle, methodHandle, fieldHandle;
            tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

            return (MethodBase)getMethodBase.Invoke(null, new[]
            {
            typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
            runtimeMethodHandleInternalCtor.Invoke(new object[] { methodHandle })
        });
        }

        public FieldInfo ResolveField(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            IntPtr typeHandle, methodHandle, fieldHandle;
            tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

            return (FieldInfo)getFieldInfo.Invoke(null, new[]
            {
            typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
            runtimeFieldHandleStubCtor.Invoke(new object[] { fieldHandle, null })
        });
        }

        public MemberInfo ResolveMember(int metadataToken, Type[] genericTypeArguments, Type[] genericMethodArguments)
        {
            IntPtr typeHandle, methodHandle, fieldHandle;
            tokenResolver.Invoke(metadataToken, out typeHandle, out methodHandle, out fieldHandle);

            if (methodHandle != IntPtr.Zero)
            {
                return (MethodBase)getMethodBase.Invoke(null, new[]
                {
                typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
                runtimeMethodHandleInternalCtor.Invoke(new object[] { methodHandle })
            });
            }

            if (fieldHandle != IntPtr.Zero)
            {
                return (FieldInfo)getFieldInfo.Invoke(null, new[]
                {
                typeHandle == IntPtr.Zero ? null : getTypeFromHandleUnsafe.Invoke(typeHandle),
                runtimeFieldHandleStubCtor.Invoke(new object[] { fieldHandle, null })
            });
            }

            if (typeHandle != IntPtr.Zero)
            {
                return getTypeFromHandleUnsafe.Invoke(typeHandle);
            }

            throw new NotImplementedException("DynamicMethods are not able to reference members by token other than types, methods and fields.");
        }

        public byte[] ResolveSignature(int metadataToken)
        {
            return signatureResolver.Invoke(metadataToken, 0);
        }

        public string ResolveString(int metadataToken)
        {
            return stringResolver.Invoke(metadataToken);
        }
    }
}
