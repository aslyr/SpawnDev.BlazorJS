﻿using Microsoft.JSInterop;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.WebGL.Extensions
{
    [JsonConverter(typeof(JSObjectConverter<WEBGL_lose_context>))]
    public class WEBGL_lose_context : JSObject
    {

        public WEBGL_lose_context(IJSInProcessObjectReference _ref) : base(_ref) { }

        public void LoseContext() => _ref.CallVoid("loseContext");
        public void RestoreContext() => _ref.CallVoid("restoreContext");

    }
}
