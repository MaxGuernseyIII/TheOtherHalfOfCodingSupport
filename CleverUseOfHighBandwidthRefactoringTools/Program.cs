// MIT License
// 
// Copyright (c) 2025-2025 Producore LLC
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

using CleverUseOfHighBandwidthRefactoringTools;
using Microsoft.Extensions.Configuration;

var Config = new ConfigurationBuilder()
  .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
  .AddEnvironmentVariables()
  .Build();

Console.WriteLine("""
                  ALERT: This is not an endorsement of WeatherStack.com.
                  
                  I have no relationship with them whatsoever.
                  
                  I selected them based on the following criteria:
                    1. Their platform works
                    2. Their platform was free at the time I wrote "The Other Half of Coding"
                    
                  Use at your own risk.
                  
                  ---
                  
                  """);

var Key = Config["WeatherStack:AppKey"] ?? throw new ApplicationException("Missing a key. Get one from weatherstack.com");
var Service = new WeatherManager(Key, new WeatherService(ApplicationKey));

var MyLocation = new Site("89138");

await Service.Poll(MyLocation);