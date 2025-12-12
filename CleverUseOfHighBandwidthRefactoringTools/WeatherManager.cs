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

namespace CleverUseOfHighBandwidthRefactoringTools;

interface IWeatherService
{
  Task<CurrentWeather> GetCurrentWeather(Site Site);
}

class WeatherService(string ApplicationKey) : IWeatherService
{
  public async Task<CurrentWeather> GetCurrentWeather(Site Site)
  {
    using var Client = new HttpClient();
    using var Response = await Client.GetAsync(
      $"http://api.weatherstack.com/current" +
      $"?access_key={ApplicationKey}" +
      $"&query={Site.Zip}" +
      $"&units=f");

    Response.EnsureSuccessStatusCode();

    var WeatherResponse =
      (await Response.Content.ReadFromJsonAsync<WeatherResponse>())!;

    return WeatherResponse.Current;
  }
}

class WeatherManager(string Key, IWeatherService WeatherService)
{
  public async Task Poll(Site Site)
  {
    var ApplicationKey = Key;
    var CurrentWeather = await WeatherService.GetCurrentWeather(Site);
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