using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace TaskTester.DesktopTester
{
    [DataContract]
    public sealed class ViewModelLocator
    {
        public ViewModelLocator() { }

        [JsonIgnore]
        readonly ConcurrentDictionary<string, object> PersistentSingletons =
            new ConcurrentDictionary<string, object>();

        public T GetPersistentSingleton<T>() where T : ViewModelBase, new() =>
            (T)PersistentSingletons.GetOrAdd(typeof(T).FullName, (key) => new T());

        [JsonProperty]
        public Dictionary<string, JObject> PersistentSingletonsData
        {
            get =>
                PersistentSingletons.Count == 0 ? null :
                PersistentSingletons.ToDictionary(
                x => x.Key,
                x => JObject.FromObject(x.Value)
            );

            set
            {
                foreach (KeyValuePair<string, JObject> entry in value)
                {
                    Type type = Type.GetType(typeName: entry.Key);
                    object obj = entry.Value.ToObject(type);
                    PersistentSingletons.TryAdd(entry.Key, obj);
                }
            }
        }
    }
}
