using Xunit;
using Moq;
using PixShare.Controllers;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Web.Routing;
using System.Reflection;

namespace PixShare.Tests.UnitTests
{
    public class UploadPhotoControllerTests
    {
        [Fact]
        public void UploadPhoto_ValidFile_ShouldSaveFile()
        {
            // Arrange
            var controller = new UploadPhotoController();
            var fileMock = new Mock<HttpPostedFileBase>();
            var content = "Fake file content";
            var fileName = "test.jpg";
            var ms = new MemoryStream();
            var writer = new StreamWriter(ms);
            writer.Write(content);
            writer.Flush();
            ms.Position = 0;
            fileMock.Setup(_ => _.InputStream).Returns(ms);
            fileMock.Setup(_ => _.FileName).Returns(fileName);
            fileMock.Setup(_ => _.ContentLength).Returns((int)ms.Length);

            var serverMock = new Mock<HttpServerUtilityBase>();
            var contextMock = new Mock<HttpContextBase>();
            contextMock.Setup(ctx => ctx.Server).Returns(serverMock.Object);
            controller.ControllerContext = new ControllerContext(contextMock.Object, new RouteData(), controller);

            serverMock.Setup(s => s.MapPath(It.IsAny<string>())).Returns<string>(p => Path.Combine("C:\\Uploads", Path.GetFileName(p)));

            // Use reflection to set the private field if there are any (not shown here as UploadPhotoController might not have any private fields that need setting)

            // Act
            var result = controller.UploadPhoto(fileMock.Object) as RedirectToRouteResult;

            // Assert
            Assert.NotNull(result);
            Assert.Equal("UploadSuccess", result.RouteValues["action"]);
        }
    }
}
