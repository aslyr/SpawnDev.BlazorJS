﻿using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Implementation;
using SpawnDev.BlazorJS.JSObjects;
//using Microsoft.JSInterop.WebAssembly;
//using SpawnDev.JSInterop.JSObjects;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpawnDev.BlazorJS
{
    [JsonConverter(typeof(JSObjectConverter<JSObject>))]
    public class JSObject : IDisposable
    {
        public static readonly IJSInProcessObjectReference NullRef = null;
        public IJSInProcessObjectReference _ref { get; private set; }
        public bool IsWrapperDisposed { get; private set; } = false;
        public JSObject(ElementReference elementRef) => FromReference(JS.ReturnMe<IJSInProcessObjectReference>(elementRef));

        // !! IMPORTANT !!
        // all objects inheriting JSObject should at minimum support this constructor to enable deserialization
        public JSObject(IJSInProcessObjectReference _ref) => FromReference(_ref);
        // used to create a new instance of a javascript class
        public JSObject(string className) => FromReference(JS.CreateNewArgs(className));
        public JSObject(string className, object arg0) => FromReference(JS.CreateNewArgs(className, new object[] { arg0 }));
        public JSObject(string className, object arg0, object arg1) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1 }));
        public JSObject(string className, object arg0, object arg1, object arg2) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3, object arg4) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3, arg4 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3, arg4, arg5 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8 }));
        public JSObject(string className, object arg0, object arg1, object arg2, object arg3, object arg4, object arg5, object arg6, object arg7, object arg8, object arg9) => FromReference(JS.CreateNewArgs(className, new object[] { arg0, arg1, arg2, arg3, arg4, arg5, arg6, arg7, arg8, arg9 }));
        public virtual void FromReference(IJSInProcessObjectReference _ref)
        {
            if (IsWrapperDisposed) throw new Exception("IJSObject.FromReference error: IJSObject object already disposed.");
            if (this._ref != null) throw new Exception("IJSObject.FromReference error: _ref object already set.");
            this._ref = _ref;
        }
        protected void ReplaceReference(IJSInProcessObjectReference _ref)
        {
            if (IsWrapperDisposed) throw new Exception("IJSObject.FromReference error: IJSObject object already disposed.");
            this._ref?.Dispose();
            this._ref = null;
            FromReference(_ref);
        }
        // sourceDisposeExceptRef - should usually be true becuase otherwise they share a _ref object and either beign disposed will dispose that object makign it useless to the one that did not dispose it
        // by setting sourceDisposeExceptRef = true (default) the new IJSObject will have exclusive use of the _ref object
        public T ConvertToIJSObject<T>(bool sourceDisposeExceptRef = true) where T : JSObject
        {
            var ret = (T)Activator.CreateInstance(typeof(T), _ref);
            if (sourceDisposeExceptRef) DisposeExceptRef();
            return ret;
        }
        public void DisposeExceptRef()
        {
            _ref = null;
            Dispose();
        }
        protected virtual void Dispose(bool disposing)
        {
            if (IsWrapperDisposed) return;
            IsWrapperDisposed = true;
            if (disposing)
            {
                // managed assets
            }
            _ref?.Dispose();
            _ref = null;
        }
        public virtual void Dispose()
        {
            if (IsWrapperDisposed) return;
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        ~JSObject()
        {
#if DEBUG
            var thisType = this.GetType();
            var refDispsoed = _ref == null;
            if (!refDispsoed)
            {
                Console.WriteLine($"DEBUG WARNING: JSObject was not Disposed properly: {thisType.Name} - {thisType.FullName}");
            }
#endif
            Dispose(false);
        }
        public T CopyPropertiesTo<T>(bool camelCase = true)
        {
            var Ttype = typeof(T);
            T ret = (T)Activator.CreateInstance(Ttype);
            foreach (var p in Ttype.GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var propName = camelCase ? Char.ToLowerInvariant(p.Name[0]) + p.Name.Substring(1) : p.Name;
                //Console.WriteLine("propName: " + propName);
                try
                {
                    if (p.CanWrite && p.GetSetMethod(/*nonPublic*/ true).IsPublic)
                    {
                        if (!JS.IsUndefined(this, propName))
                        {
                            var value = _ref.Get(p.PropertyType, propName);
                            p.SetValue(ret, value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("ERROR: CopyPropertiesTo - " + ex.Message);
                    Console.WriteLine(">> propName: " + propName);
                    Console.WriteLine(">> proptype: " + JS.TypeOf(this, propName));
                    //Console.WriteLine("ERROR: CopyPropertiesTo - " + ex.StackTrace);
                }
            }
            return ret;
        }
    }
}