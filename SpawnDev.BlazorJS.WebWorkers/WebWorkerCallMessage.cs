﻿using Microsoft.JSInterop;
using SpawnDev.BlazorJS.JSObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace SpawnDev.BlazorJS.WebWorkers
{
    public interface IWebWorkerCallMessageBase
    {
        string TargetName { get; set; }
        string RequestId { get; set; }
        string TargetType { get; set; }
    }

    public class WebWorkerMessageBase : IWebWorkerCallMessageBase
    {
        public string TargetType { get; set; } = "";
        public string TargetName { get; set; } = "";
        public string RequestId { get; set; } = "";
    }

    internal class WebWorkerMessageOut : WebWorkerMessageBase
    {
        public object? Data { get; set; } = null;
    }
    public class WebWorkerMessageIn : WebWorkerMessageBase
    {
        [JsonIgnore]
        public MessageEvent? _msg { get; set; }
        public T? GetData<T>() => _msg == null ? default : _msg.JSRef.Get<T>("data.data");
    }
}