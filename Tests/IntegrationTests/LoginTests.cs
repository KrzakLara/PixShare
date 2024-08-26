using System.Web;
using System.Web.Mvc;
using Moq;
using Xunit;
using PixShare.Controllers;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using System;

namespace PixShare.Tests.IntegrationTests
{
    public class LoginControllerIntegrationTests
    {
        private LoginController CreateLoginControllerWithMockContext(Mock<IRepo> mockRepo)
        {
            var controller = new LoginController();

            // Use reflection to set the private _repo field
            var repoField = typeof(LoginController).GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repoField.SetValue(controller, mockRepo.Object);

            // Mock the HttpContext and Session
            var context = new Mock<HttpContextBase>();
            var request = new Mock<HttpRequestBase>();
            var response = new Mock<HttpResponseBase>();
            var session = new Mock<HttpSessionStateBase>();
            var server = new Mock<HttpServerUtilityBase>();

            context.Setup(c => c.Request).Returns(request.Object);
            context.Setup(c => c.Response).Returns(response.Object);
            context.Setup(c => c.Session).Returns(session.Object);
            context.Setup(c => c.Server).Returns(server.Object);

            var contextAccessor = new Mock<IServiceProvider>();
            contextAccessor.Setup(s => s.GetService(typeof(HttpContextBase))).Returns(context.Object);

            var controllerContext = new ControllerContext(context.Object, new System.Web.Routing.RouteData(), controller);
            controller.ControllerContext = controllerContext;

            return controller;
        }

        [Fact]
        public void Login_ValidUser_ShouldRedirectToHomePage()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var user = new User
            {
                IDUser = 1,
                Username = "testuser",
                Email = "testuser@example.com",
                Password = "password"
            };
            mockRepo.Setup(repo => repo.AuthUser(user.Email, user.Password)).Returns(user);

            var controller = CreateLoginControllerWithMockContext(mockRepo);

            var loginUser = new User { Email = "testuser@example.com", Password = "password" };

            // Act
            var result = controller.Index(loginUser) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
            Assert.Equal("Home", result.RouteValues["controller"]);
        }

        [Fact]
        public void Login_InvalidUser_ShouldReturnViewWithErrorMessage()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            mockRepo.Setup(repo => repo.AuthUser(It.IsAny<string>(), It.IsAny<string>())).Returns((User)null);

            var controller = CreateLoginControllerWithMockContext(mockRepo);

            var loginUser = new User { Email = "invaliduser@example.com", Password = "wrongpassword" };

            // Act
            var result = controller.Index(loginUser) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.True(controller.ModelState.ContainsKey(""));
            var error = controller.ModelState[""].Errors[0];
            Assert.Equal("Invalid login attempt.", error.ErrorMessage);
        }
    }
}
