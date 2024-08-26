using System.Web.Mvc;
using Moq;
using Xunit;
using PixShare.Controllers;
using PixShareLIB.DAL;
using PixShareLIB.Models;

namespace PixShare.Tests.UnitTests
{
    public class AdminControllerTests
    {
        [Fact]
        public void EditUser_Get_ExistingUser_ShouldReturnViewWithUser()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var user = new User { IDUser = 1, Username = "testuser", Email = "testuser@example.com" };
            mockRepo.Setup(repo => repo.GetUserById(1)).Returns(user);

            // Create an instance of the controller
            var controller = new AdminController();

            // Use reflection to set the private _repo field
            var repoField = typeof(AdminController).GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repoField.SetValue(controller, mockRepo.Object);

            // Act
            var result = controller.EditUser(1) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.IsType<ViewResult>(result);
            Assert.Equal(user, result.Model);
        }

        [Fact]
        public void EditUser_Get_NonExistingUser_ShouldReturnHttpNotFound()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            mockRepo.Setup(repo => repo.GetUserById(It.IsAny<int>())).Returns((User)null);

            // Create an instance of the controller
            var controller = new AdminController();

            // Use reflection to set the private _repo field
            var repoField = typeof(AdminController).GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repoField.SetValue(controller, mockRepo.Object);

            // Act
            var result = controller.EditUser(1);

            // Assert
            Assert.IsType<HttpNotFoundResult>(result);
        }

        [Fact]
        public void EditUser_Post_ValidModel_ShouldRedirectToUsers()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var user = new User { IDUser = 1, Username = "testuser", Email = "testuser@example.com" };
            mockRepo.Setup(repo => repo.UpdateUser(user));

            // Create an instance of the controller
            var controller = new AdminController();

            // Use reflection to set the private _repo field
            var repoField = typeof(AdminController).GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repoField.SetValue(controller, mockRepo.Object);

            // Act
            var result = controller.EditUser(user) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Users", result.RouteValues["action"]);
        }

        [Fact]
        public void EditUser_Post_InvalidModel_ShouldReturnViewWithModel()
        {
            // Arrange
            var mockRepo = new Mock<IRepo>();
            var user = new User { IDUser = 1, Username = "testuser", Email = "testuser@example.com" };

            // Create an instance of the controller
            var controller = new AdminController();
            controller.ModelState.AddModelError("Email", "Required");

            // Use reflection to set the private _repo field
            var repoField = typeof(AdminController).GetField("_repo", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            repoField.SetValue(controller, mockRepo.Object);

            // Act
            var result = controller.EditUser(user) as ViewResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal(user, result.Model);
            Assert.False(controller.ModelState.IsValid);
        }
    }
}
