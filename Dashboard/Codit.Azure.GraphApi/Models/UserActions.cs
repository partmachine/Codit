using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.Azure.GraphApi.Models
{
    public class ChangePassword
    {
        [JsonProperty("currentPassword")]
        public string CurrentPassword { get; set; }

        [JsonProperty("newPassword")]
        public string NewPassword { get; set; }
    }


    public class AssignLicenses
    {
        [JsonProperty("addLicenses")]
        public Addlicenses[] AddLicenses { get; set; }

        [JsonProperty("removeLicenses")]
        public object[] RemoveLicenses { get; set; }
    }

    public class Addlicenses
    {
        [JsonProperty("disabledPlans")]
        public object[] DisabledPlans { get; set; }

        [JsonProperty("skuId")]
        public string SkuId { get; set; }
    }

}

