// MIT License
// 
// Copyright (c) 2025-2025 Hexagon Software LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace Chapter_HighBandwidthRefactoringTools;

public sealed class WeatherResponse
{
  [JsonPropertyName("current")]
  public CurrentWeather Current { get; set; } = default!;
}

public sealed class CurrentWeather
{
  [JsonPropertyName("temperature")] public int Temperature { get; set; }
}

class Site(string Zip)
{
  public bool IsInEmergencyState { get; private set; }
  public string Zip { get; } = Zip;

  public Task ActivateEmergencyHeating()
  {
    Console.WriteLine("Activating emergency heating!");
    return Task.CompletedTask;
  }

  public Task ActivateEmergencyCooling()
  {
    Console.WriteLine("Activating emergency cooling!");

    return Task.CompletedTask;
  }

  public Task DeactivateEmergencyHeating()
  {
    Console.WriteLine("Deactivating emergency heating!");
    return Task.CompletedTask;
  }

  public Task DeactivateEmergencyCooling()
  {
    Console.WriteLine("Deactivating emergency cooling!");

    return Task.CompletedTask;
  }

  public Task AlertEmergencyConditions()
  {
    Console.WriteLine("ALERT: Emergency conditions!");
    IsInEmergencyState = true;

    return Task.CompletedTask;
  }

  public Task AllClear()
  {
    Console.WriteLine("ALERT: All clear!");
    IsInEmergencyState = false;

    return Task.CompletedTask;
  }
}

class WeatherManager(string Key)
{
  public async Task Poll(Site Site)
  {
    var AppKey = Key;
    using var Client = new HttpClient();
    using var Response = await Client.GetAsync(
      $"http://api.weatherstack.com/current" +
      $"?access_key={AppKey}" +
      $"&query={Site.Zip}" +
      $"&units=f");

    Response.EnsureSuccessStatusCode();

    var WeatherResponse =
      (await Response.Content.ReadFromJsonAsync<WeatherResponse>())!;

    var CurrentWeather = WeatherResponse.Current;
    var Temp = CurrentWeather.Temperature;

    switch (Temp)
    {
      case < 55:
        await Site.ActivateEmergencyHeating();
        await Site.AlertEmergencyConditions();
        break;
      case > 85:
        await Site.ActivateEmergencyCooling();
        await Site.AlertEmergencyConditions();
        break;
      default:
        if (Site.IsInEmergencyState)
        {
          await Site.DeactivateEmergencyCooling();
          await Site.DeactivateEmergencyHeating();
        }
        await Site.AllClear();
        break;
    }
  }
}