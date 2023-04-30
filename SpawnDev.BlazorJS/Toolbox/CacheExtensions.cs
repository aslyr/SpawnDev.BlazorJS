﻿using Microsoft.Extensions.Options;
using SpawnDev.BlazorJS.JSObjects;
using System.Text;
using System.Text.Json;

namespace SpawnDev.BlazorJS.Toolbox
{
    public static class CacheExtensions
    {
        private static JsonSerializerOptions JsonSerializerDeserializeOptions { get; set; } = new JsonSerializerOptions { PropertyNameCaseInsensitive = true, AllowTrailingCommas = true, ReadCommentHandling = JsonCommentHandling.Skip };

        public static Task WriteBytes(this Cache _this, string url, byte[] content, string contentType = "")
        {
            using var uint8 = new Uint8Array(content);
            using var uint8Buffer = uint8.Buffer;
            using (var response = new Response(uint8Buffer))
            {
                if (!string.IsNullOrEmpty(contentType)) response.HeaderSet("Content-Type", contentType);
                response.HeaderSet("Content-Length", uint8Buffer.ByteLength.ToString());
                return _this.Put(url, response);
            }
        }

        public static Task WriteUint8Array(this Cache _this, string url, Uint8Array uint8Array, string contentType = "")
        {
            using var buffer = uint8Array.Buffer;
            return _this.WriteArrayBuffer(url, buffer);
        }

        public static Task WriteArrayBuffer(this Cache _this, string url, ArrayBuffer buffer, string contentType = "")
        {
            using (var response = new Response(buffer))
            {
                if (!string.IsNullOrEmpty(contentType)) response.HeaderSet("Content-Type", contentType);
                response.HeaderSet("Content-Length", buffer.ByteLength.ToString());
                return _this.Put(url, response);
            }
        }

        public static Task WriteText(this Cache _this, string url, string content, string contentType = "")
        {
            return _this.WriteBytes(url, Encoding.UTF8.GetBytes(content), contentType);
        }

        public static Task WriteJSON(this Cache _this, string url, object value, string contentType = "")
        {
            return _this.WriteText(url, JsonSerializer.Serialize(value), contentType);
        }

        public static async Task<byte[]?> ReadBytes(this Cache _this, string url, CacheMatchOptions? options = null)
        {
            using var response = options == null ? await _this.Match(url) : await _this.Match(url, options);
            return response == null ? null : await response.ReadBytes();
        }

        public static async Task<ArrayBuffer?> ReadArrayBuffer(this Cache _this, string url, CacheMatchOptions? options = null)
        {
            using var response = options == null ? await _this.Match(url) : await _this.Match(url, options);
            return response == null ? null : await response.ArrayBuffer();
        }

        public static async Task<string?> ReadText(this Cache _this, string url, CacheMatchOptions? options = null)
        {
            using var response = options == null ? await _this.Match(url) : await _this.Match(url, options);
            return response == null ? null : await response.Text();
        }

        public static async Task<T?> ReadJSON<T>(this Cache _this, string url, CacheMatchOptions? options = null, JsonSerializerOptions? jsonSerializerOptions = null)
        {
            var tmp = await _this.ReadText(url, options);
            if (tmp == null) return default;
            return JsonSerializer.Deserialize<T>(tmp, jsonSerializerOptions != null ? jsonSerializerOptions : JsonSerializerDeserializeOptions) ?? default;
        }

        public static async Task<string?> GetContentType(this Cache _this, string url, CacheMatchOptions? options = null)
        {
            using var response = options == null ? await _this.Match(url) : await _this.Match(url, options);
            if (response == null) return null;
            using var headers = response.JSRef.Get<JSObject>("headers");
            return headers == null ? "" : headers.JSRef.Get<string>("content-type") ?? "";
        }

        public static async Task<long?> GetSize(this Cache _this, string url, CacheMatchOptions? options = null)
        {
            using var response = options == null ? await _this.Match(url) : await _this.Match(url, options);
            if (response == null) return null;
            using var headers = response.JSRef.Get<JSObject>("headers");
            var contentLength = headers.JSRef.Get<string>("content-length");
            if (!string.IsNullOrEmpty(contentLength) && int.TryParse(contentLength, out var val))
            {
                return val;
            }
            else
            {
                using var buffer = await response.ArrayBuffer();
                return buffer.ByteLength;
            }
        }
    }
}