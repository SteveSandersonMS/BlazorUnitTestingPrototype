using Microsoft.AspNetCore.Components.Testing;
using SampleApp.Pages;
using Xunit;

namespace SampleApp.Tests
{
    public class TodoListTest
    {
        private TestHost host = new TestHost();

        [Fact]
        public void InitiallyDisplaysNoItems()
        {
            var component = host.AddComponent<TodoList>();
            var items = component.FindAll("li");
            Assert.Empty(items);
        }

        [Fact]
        public void CanAddItems()
        {
            // Arrange
            var component = host.AddComponent<TodoList>();

            // Act
            component.Find("input").Change("First item");
            component.Find("form").Submit();

            component.Find("input").Change("Second item");
            component.Find("form").Submit();

            // Assert
            Assert.Collection(component.FindAll("li span"),
                li => Assert.Equal("First item", li.InnerText),
                li => Assert.Equal("Second item", li.InnerText));
            Assert.Empty(component.Find("input").GetAttributeValue("value", string.Empty));
        }

        [Fact]
        public void CanRemoveItems()
        {
            // Arrange
            var component = host.AddComponent<TodoList>();
            component.Find("input").Change("First item");
            component.Find("form").Submit();
            component.Find("input").Change("Second item");
            component.Find("form").Submit();

            // Act
            component.Find("li .delete").Click();

            // Assert
            Assert.Collection(component.FindAll("li span"),
                li => Assert.Equal("Second item", li.InnerText));
        }
    }
}
