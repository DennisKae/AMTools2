using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serialization;

namespace AMTools.Web.Data.JsonStore
{
    // Quelle: https://gist.github.com/alexeyzimarev/c00b79c11c8cce6f6208454f7933ad24
    public class JsonNetSerializer : IRestSerializer
    {
        public string Serialize(object obj) => JsonConvert.SerializeObject(obj);

        public T Deserialize<T>(IRestResponse response) => JsonConvert.DeserializeObject<T>(response.Content);

        public string Serialize(Parameter parameter) => JsonConvert.SerializeObject(parameter.Value);

        public string[] SupportedContentTypes { get; } =
        {
                "application/json", "text/json", "text/x-json", "text/javascript", "*+json"
        };

        public string ContentType { get; set; } = "application/json";

        public DataFormat DataFormat { get; } = DataFormat.Json;
    }
}
