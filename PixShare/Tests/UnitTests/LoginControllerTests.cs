using Xunit;
using Moq;
using PixShare.Controllers;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Web.Mvc;
using System.Web;
using System.Collections.Generic;
using System.Security.Principal;
using System.Web.Routing;
using System.Reflection;

namespace PixShare.Tests.UnitTests
{
    public class LoginControllerTests
    {
        [Fact]
        public void Index_Post_ValidCredentials_ShouldRedirectToHome()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var controller = new LoginController();

            // Use reflection to set the private _repo field
            var field = typeof(LoginController).GetField("_repo", BindingFlags.NonPublic | BindingFlags.Instance);
            field.SetValue(controller, mockRepo.Object);

            var user = new User
            {
                Email = "user@example.com",
                Password = "password"
            };

            var authenticatedUser = new User
            {
                IDUser = 1,
                Username = "user",
                Email = "user@example.com",
                UserType = "User"
            };

            mockRepo.Setup(r => r.AuthUser(user.Email, user.Password)).Returns(authenticatedUser);

            // Simulate HTTP context
            var httpContext = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();
            httpContext.Setup(ctx => ctx.Session).Returns(session.Object);
            controller.ControllerContext = new ControllerContext(httpContext.Object, new RouteData(), controller);

            // Act
            var result = controller.Index(user) as RedirectToRouteResult;

            // Assert
            mockRepo.Verify(r => r.AuthUser(user.Email, user.Password), Times.Once);
            Assert.NotNull(result);
            Assert.Equal("Index", result.RouteValues["action"]);
            Assert.Equal("Home", result.RouteValues["controller"]);
            session.VerifySet(s => s["UserID"] = authenticatedUser.IDUser);
            session.VerifySet(s => s["Username"] = authenticatedUser.Username);
            session.VerifySet(s => s["UserType"] = authenticatedUser.UserType);
        }
    }
}
