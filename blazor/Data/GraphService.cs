using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;

namespace blazor.Data
{
    [Authorize]
    public class GraphService
    {
        private readonly ILogger<GraphService> _logger;
        private readonly IDownstreamWebApi _downstreamWebApi;
        public GraphService(ILogger<GraphService> logger,
                              IDownstreamWebApi downstreamWebApi)
        {
            _logger = logger;
            _downstreamWebApi = downstreamWebApi;
        }
        [AuthorizeForScopes(ScopeKeySection = "GraphApi:Scopes")]
        public async Task<string> GetUserAsync()
        {
            // Call GraphApi to get user information
            using var response = await _downstreamWebApi.CallWebApiForUserAsync("GraphApi").ConfigureAwait(false);
            return await response.Content.ReadAsStringAsync();
        }
    }
}
