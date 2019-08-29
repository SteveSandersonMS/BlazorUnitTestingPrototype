using Microsoft.AspNetCore.Components.Testing;
using SampleApp.Pages;
using System;
using Xunit;

namespace SampleApp.Tests
{
    public class MyTests
    {
        private TestHost host = new TestHost();

        [Fact]
        public void CounterWorks()
        {
            var component = host.AddComponent<Counter>();
            Func<string> countValue = () => component.Find("#count").InnerText;

            Assert.Equal("Counter", component.Find("h1").InnerText);
            Assert.Equal("Current count: 0", countValue());

            component.Find("button.inc").Click();
            Assert.Contains("Current count: 1", countValue());

            component.Find("button.dec").Click();
            Assert.Contains("Current count: 0", countValue());
        }

        [Fact]
        public void FetchDataWorks()
        {
            // Initially shows loading state
            var req = host.AddMockHttp().Capture("/sample-data/weather.json");
            var component = host.AddComponent<FetchData>();
            Assert.Contains("Loading...", component.GetMarkup());

            // When the server responds, we display the data
            host.WaitForNextRender(() => req.SetResult(new[]
            {
                new FetchData.WeatherForecast { Summary = "First" },
                new FetchData.WeatherForecast { Summary = "Second" },
            }));
            Assert.DoesNotContain("Loading...", component.GetMarkup());
            Assert.Collection(component.FindAll("tbody tr"),
                row => Assert.Contains("First", row.OuterHtml),
                row => Assert.Contains("Second", row.OuterHtml));
        }
    }
}
