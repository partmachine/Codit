using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.Azure.GraphApi.Models
{
    public class ODataRoot
    {
        [JsonProperty("odata.metadata")]
        public string ODataMetadata { get; set; }

        [JsonProperty("odata.metadata")]
        public string ODataType { get; set; }
    }


    public class ODataError
    {
        [JsonProperty("odata.error")]
        public Error Error { get; set; }
    }

    public class Error
    {
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("message")]
        public Message Message { get; set; }
    }

    public class Message
    {
        [JsonProperty("lang")]
        public string Lang { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }


}
