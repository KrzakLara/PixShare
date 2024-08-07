using Xunit;
using Moq;
using PixShare.Controllers;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Web.Mvc;
using System.Reflection;

namespace PixShare.Tests.UnitTests
{
    public class RegistrationControllerTests
    {
        [Fact]
        public void Index_Post_ValidModel_ShouldCreateUser()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var controller = new RegistrationController();

            // Use reflection to set the private _repo field
            var field = typeof(RegistrationController).GetField("_repo", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(controller, mockRepo.Object);

            var user = new User
            {
                Username = "newuser",
                Password = "password",
                Email = "newuser@example.com"
            };

            // Act
            var result = controller.Index(user) as RedirectToRouteResult;

            // Assert
            mockRepo.Verify(r => r.CreateUser(It.IsAny<User>()), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
            Assert.Equal("Home", result.RouteValues["controller"]);
        }
    }
}
