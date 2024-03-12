﻿using Microsoft.JSInterop;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace SpawnDev.BlazorJS.JsonConverters
{
    public class DynamicJSObjectConverterFactory : JsonConverterFactory
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DynamicJSObject);
        }

        public override JsonConverter? CreateConverter(Type typeToConvert, JsonSerializerOptions options)
        {
            return new DynamicJSObjectConverter();
        }
    }

    public class DynamicJSObjectConverter : JsonConverter<DynamicJSObject>, IJSInProcessObjectReferenceConverter
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert == typeof(DynamicJSObject);
        }
        public override DynamicJSObject? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var _ref = JsonSerializer.Deserialize<IJSInProcessObjectReference?>(ref reader, options);
            return _ref == null ? null : new DynamicJSObject(_ref);
        }

        public override void Write(Utf8JsonWriter writer, DynamicJSObject value, JsonSerializerOptions options)
        {
            var obj = value.JSObjectRef;
            if (obj.IsJSRefUndefined)
            {
                // cast to JSInProcessObjectReferenceUndefined so it gets picked up by the JSInProcessObjectReferenceUndefinedConverter
                JsonSerializer.Serialize(writer, (JSInProcessObjectReferenceUndefined)obj.JSRef, options);
            }
            else
            {
                var _ref = obj == null ? null : obj.JSRef;
                JsonSerializer.Serialize(writer, _ref, options);
            }
        }
    }
}