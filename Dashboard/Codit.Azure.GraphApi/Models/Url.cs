using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.Azure.GraphApi.Models
{
    public class Link :ODataRoot
    {
        [JsonProperty("url")]
        public string Url { get; set; }
    }
}
