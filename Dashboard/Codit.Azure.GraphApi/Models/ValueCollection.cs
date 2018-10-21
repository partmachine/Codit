using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.Azure.GraphApi.Models
{
    public class ValueCollection<T>
    {
        [JsonProperty("odata.metadata")]
        public string ODataMetadata { get; set; }

        [JsonProperty("value")]
        public T[] GroupIds { get; set; }
    }
}
