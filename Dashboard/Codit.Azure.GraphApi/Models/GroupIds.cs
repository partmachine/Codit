using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.Azure.GraphApi.Models
{
    public class GroupIds
    {    
        [JsonProperty("groupIds")]
        public string[] groupIds { get; set; }
    }

    
}
