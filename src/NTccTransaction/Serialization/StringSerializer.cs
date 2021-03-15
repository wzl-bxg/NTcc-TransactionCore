using Newtonsoft.Json;
using NTccTransaction.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;

namespace NTccTransaction
{
    public class StringSerializer : ISerializer
    {
        public T Deserialize<T>(string content)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NTccContractResolver(),
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore
            };
            
            return JsonConvert.DeserializeObject<T>(content, settings);
        }

        public string Serialize<T>(T obj)
        {
            var settings = new JsonSerializerSettings()
            {
                ContractResolver = new NTccContractResolver(),
                TypeNameHandling = TypeNameHandling.All,
                NullValueHandling = NullValueHandling.Ignore
            };
            return JsonConvert.SerializeObject(obj, settings);
        }
    }
}
