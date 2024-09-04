namespace TKC.Data;

using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;

public class PlanningCenterService
{
    private readonly HttpClient _httpClient;

    public PlanningCenterService(string apiKey)
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.planningcenteronline.com")
        };
        _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
    }

    // Get Groups
    public async Task<List<LifeGroup>> GetLifeGroupsAsync()
    {
        var response = await _httpClient.GetAsync("/groups/v2/groups?include=events");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var groupsResponse = JsonConvert.DeserializeObject<GroupsResponse>(content);

        return groupsResponse.Data;
    }

    // Get Events
    public async Task<List<ChurchEvent>> GetEventsAsync()
    {
        var response = await _httpClient.GetAsync("/check-ins/v2/events");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var eventsResponse = JsonConvert.DeserializeObject<EventsResponse>(content);

        return eventsResponse.Data;
    }

    // Get Services
    public async Task<List<Service>> GetServicesAsync()
    {
        var response = await _httpClient.GetAsync("/services/v2/service_types");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var servicesResponse = JsonConvert.DeserializeObject<ServicesResponse>(content);

        return servicesResponse.Data;
    }

    // Get Check-Ins
    public async Task<List<CheckIn>> GetCheckInsAsync()
    {
        var response = await _httpClient.GetAsync("/check-ins/v2/check_ins");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        var checkInsResponse = JsonConvert.DeserializeObject<CheckInsResponse>(content);

        return checkInsResponse.Data;
    }
}

// Models
public class LifeGroup
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
}

public class GroupsResponse
{
    public List<LifeGroup> Data { get; set; }
}

public class ChurchEvent
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime StartDate { get; set; }
}

public class EventsResponse
{
    public List<ChurchEvent> Data { get; set; }
}

public class Service
{
    public string Id { get; set; }
    public string Name { get; set; }
}

public class ServicesResponse
{
    public List<Service> Data { get; set; }
}

public class CheckIn
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime CheckInTime { get; set; }
}

public class CheckInsResponse
{
    public List<CheckIn> Data { get; set; }
}
