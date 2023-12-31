﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.Identity.Web;

namespace blazor.Data
{
    [Authorize]
    public class AzureService
    {
        private readonly ILogger<GraphService> _logger;
        private readonly IDownstreamWebApi _downstreamWebApi;
        public AzureService(ILogger<GraphService> logger,
                              IDownstreamWebApi downstreamWebApi)
        {
            _logger = logger;
            _downstreamWebApi = downstreamWebApi;
        }
        [AuthorizeForScopes(ScopeKeySection = "AzureApi:Scopes")]
        public async Task<string> GetRolesAsync()
        {
            using var response = await _downstreamWebApi.CallWebApiForUserAsync("AzureApi",
                (opts) => opts.RelativePath = "providers/Microsoft.Authorization/roleAssignments?api-version=2015-07-01").ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
