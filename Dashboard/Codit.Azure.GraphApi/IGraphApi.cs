using Codit.Azure.GraphApi.Models;
using Refit;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codit.Azure.GraphApi
{
    public class ApiQueryParams
    {
        [AliasAs("api-version")]
        public string Version { get; set; }
    }

    public interface IGraphApi
    {
        [Get("/me")]
        Task<User> GetUser(ApiQueryParams queryString);

        [Patch("/me")]
        Task PatchUser([Body] User user);

        [Get("/me/manager")]
        Task<User> GetManager();

        [Get("/me/$links/manager")]
        Task<Link> GetManagerLink();

        [Put("/me/$links/manager")]
        Task PutManager(Link mangerLink);

        [Post("/me/changePassword")]
        Task ChangeSignedInUserPassword([Body] ChangePassword passwordChange);

        [Get("/{organization}/users")]
        [Headers("Authorization: Bearer")]
        Task<ValueCollection<User>> GetUsers(string organization, ApiQueryParams queryString);

        [Post("/{organization}/users/{objectId}/changePassword")]
        Task ChangeUserPassword(string organization, Guid objectId, [Body]ChangePassword passwordChange);

        [Post("/{organization}/users/{userPrincipleName}/changePassword")]
        Task ChangeUserPassword(string organization, string userPrincipleName, [Body]ChangePassword passwordChange);

        [Post("/{tenantId}/users/{objectId}/changePassword")]
        Task ChangeUserPassword(Guid tenantId, Guid objectId, [Body]ChangePassword passwordChange);

        [Post("/{tenantId}/users/{userPrincipleName}/changePassword")]
        Task ChangeUserPassword(Guid tenantId, string userPrincipleName, [Body] ChangePassword passwordChange);

        [Post("/{tenantId}/users/{userId}/assignLicense")]
        Task AssignLicense(Guid tenantId, Guid userId, [Body] AssignLicenses licences);

        [Post("/{tenantId}/users/{userId}/checkMemberGroups")]
        Task<ValueCollection<Guid>> CheckUserMemberGroups(Guid tenantId, Guid userId, [Body] GroupIds licences);
    }


    
}
