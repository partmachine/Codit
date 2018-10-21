using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Codit.Azure.GraphApi.Models
{
    public class User 
    {
        [JsonProperty("objectType")]
        public string ObjectType { get; set; }

        [JsonProperty("objectId")]
        public string ObjectId { get; set; }

        [JsonProperty("deletionTimestamp")]
        public object DeletionTimestamp { get; set; }

        [JsonProperty("accountEnabled")]
        public bool AccountEnabled { get; set; }

        [JsonProperty("signInNames")]
        public object[] SignInNames { get; set; }

        [JsonProperty("assignedLicenses")]
        public Assignedlicens[] AssignedLicenses { get; set; }

        [JsonProperty("assignedPlans")]
        public Assignedplan[] AssignedPlans { get; set; }

        [JsonProperty("city")]
        public string City { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("creationType")]
        public object CreationType { get; set; }

        [JsonProperty("department")]
        public string Department { get; set; }

        [JsonProperty("dirSyncEnabled")]
        public object DirSyncEnabled { get; set; }

        [JsonProperty("displayName")]
        public string DisplayName { get; set; }

        [JsonProperty("facsimileTelephoneNumber")]
        public object FacsimileTelephoneNumber { get; set; }

        [JsonProperty("givenName")]
        public string GivenName { get; set; }

        [JsonProperty("immutableId")]
        public object ImmutableId { get; set; }

        [JsonProperty("jobTitle")]
        public string JobTitle { get; set; }

        [JsonProperty("lastDirSyncTime")]
        public object LastDirSyncTime { get; set; }

        [JsonProperty("mail")]
        public string Mail { get; set; }

        [JsonProperty("mailNickname")]
        public string MailNickname { get; set; }

        [JsonProperty("mobile")]
        public object Mobile { get; set; }

        [JsonProperty("onPremisesSecurityIdentifier")]
        public object OnPremisesSecurityIdentifier { get; set; }

        [JsonProperty("otherMails")]
        public object[] otherMails { get; set; }

        [JsonProperty("passwordPolicies")]
        public string PasswordPolicies { get; set; }

        [JsonProperty("passwordProfile")]
        public object PasswordProfile { get; set; }

        [JsonProperty("physicalDeliveryOfficeName")]
        public string PhysicalDeliveryOfficeName { get; set; }

        [JsonProperty("postalCode")]
        public string PostalCode { get; set; }

        [JsonProperty("preferredLanguage")]
        public string PreferredLanguage { get; set; }

        [JsonProperty("provisionedPlans")]
        public Provisionedplan[] ProvisionedPlans { get; set; }

        [JsonProperty("provisioningErrors")]
        public object[] ProvisioningErrors { get; set; }

        [JsonProperty("proxyAddresses")]
        public string[] ProxyAddresses { get; set; }

        [JsonProperty("sipProxyAddress")]
        public string SipProxyAddress { get; set; }

        [JsonProperty("state")]
        public string State { get; set; }

        [JsonProperty("streetAddress")]
        public string StreetAddress { get; set; }

        [JsonProperty("surname")]
        public string Surname { get; set; }

        [JsonProperty("telephoneNumber")]
        public string TelephoneNumber { get; set; }

        [JsonProperty("usageLocation")]
        public string UsageLocation { get; set; }

        [JsonProperty("userPrincipalName")]
        public string UserPrincipalName { get; set; }

        [JsonProperty("userType")]
        public string UserType { get; set; }
    }

    public class Assignedlicens
    {
        [JsonProperty("disabledPlans")]
        public object[] DisabledPlans { get; set; }

        [JsonProperty("skuId")]
        public string SkuId { get; set; }
    }

    public class Assignedplan
    {
        [JsonProperty("assignedTimestamp")]
        public DateTime AssignedTimestamp { get; set; }

        [JsonProperty("capabilityStatus")]
        public string CapabilityStatus { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }

        [JsonProperty("servicePlanId")]
        public string ServicePlanId { get; set; }
    }

    public class Provisionedplan
    {
        [JsonProperty("capabilityStatus")]
        public string CapabilityStatus { get; set; }

        [JsonProperty("provisioningStatus")]
        public string CrovisioningStatus { get; set; }

        [JsonProperty("service")]
        public string Service { get; set; }
    }

}
