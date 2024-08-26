using System;
using System.Web;
using System.Web.Mvc;
using Moq;
using Xunit;
using PixShare.Controllers;
using PixShareLIB.DAL;
using PixShareLIB.Models;

namespace PixShare.Tests.IntegrationTests
{
    public class RegistrationControllerIntegrationTests
    {
        private RegistrationController CreateRegistrationControllerWithMockContext(Mock<IRepo> mockRepo)
        {
            var controller = new RegistrationController();

            // Use reflection to set the private _repo field
            var repoField = typeof(RegistrationController).GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repoField.SetValue(controller, mockRepo.Object);

            // Mock the HttpContext
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(c => c.Request).Returns(request.Object);
            context.Setup(c => c.Response).Returns(response.Object);
            context.Setup(c => c.Session).Returns(session.Object);
            context.Setup(c => c.Server).Returns(server.Object);

            var controllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);
            controller.ControllerContext = controllerContext;

            return controller;
        }

        [Fact]
        public void Registration_ValidUser_ShouldRedirectToHomePage()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var user = new User
            {
                Username = "newuser",
                Password = "password",
                Email = "newuser@example.com",
                PackageType = "FREE"
            };
            mockRepo.Setup(repo => repo.CreateUser(It.IsAny<User>()));

            var controller = CreateRegistrationControllerWithMockContext(mockRepo);

            // Act
            var result = controller.Index(user) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
            Assert.Equal("Home", result.RouteValues["controller"]);
        }

        [Fact]
        public void Registration_InvalidUser_ShouldReturnViewWithModel()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();

            var controller = CreateRegistrationControllerWithMockContext(mockRepo);
            controller.ModelState.AddModelError("Email", "Email is required");

            var user = new User
            {
                Username = "newuser",
                Password = "password",
                // Email is missing to simulate an invalid model
            };

            // Act
            var result = controller.Index(user) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user, result.Model);
            Assert.True(controller.ModelState.ContainsKey("Email"));
        }
    }
}
