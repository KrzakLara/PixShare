using Xunit;
using Moq;
using PixShare.Controllers;
using PixShareLIB.DAL;
using PixShareLIB.Models;
using System.Web.Mvc;

public class RegistrationControllerTests
{
    [Fact]
    public void Index_Post_ValidModel_ShouldCreateUser()
    {
        // Arrange
        var mockRepo = new Mock<IRepo>();
        var controller = new RegistrationController(mockRepo.Object);
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
