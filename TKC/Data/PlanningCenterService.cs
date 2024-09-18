namespace TKC.Data;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.DataProtection;
using Newtonsoft.Json;
using TKC.Models.PlanningCenter;

/// <summary>
/// https://api.planningcenteronline.com/explorer/
/// </summary>
public class PlanningCenterService
{
    private readonly HttpClient _httpClient;

    public PlanningCenterService(string appId, string secret)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.planningcenteronline.com")
        };

        var byteArray = System.Text.Encoding.ASCII.GetBytes($"{appId}:{secret}");
        var authHeader = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));
        _httpClient.DefaultRequestHeaders.Authorization = authHeader;
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    public async Task<EnrollmentResponse?> GetEnrollmentForGroupAsync(int groupId)
    {
        var response = await _httpClient.GetAsync($"/groups/v2/groups/{groupId}/enrollment");
        var content = await response.Content.ReadAsStringAsync();

        var groupsResponse = JsonConvert.DeserializeObject<EnrollmentResponse>(content);
        return groupsResponse;
    }

    // Get Groups
    public async Task<GroupsResponse?> GetGroupsResponseAsync()
    {
        var response = await _httpClient.GetAsync("/groups/v2/groups?include=enrollment&order=name&per_page=50");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var groupsResponse = JsonConvert.DeserializeObject<GroupsResponse>(content);

        return groupsResponse;
    }
    
}
