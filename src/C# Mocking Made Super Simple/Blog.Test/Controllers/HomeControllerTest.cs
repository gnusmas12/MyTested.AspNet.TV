namespace Blog.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blog.Controllers;
    using Blog.Controllers.Models;
    using Blog.Services.Data;
    using Blog.Services.Models;
    using Extensions;
    using FakeItEasy;
    using Fakes;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;

    public class HomeControllerTest
    {
        [Fact]
        public async Task IndexShouldReturnViewResultWithCorrectArticleModel()
        {
            // Arrange
            var fakeArticleService = A.Fake<IArticleService>();

            A
                .CallTo(() => fakeArticleService.All(
                    A<int>.Ignored, 
                    A<int>.Ignored, 
                    A<bool>.Ignored))
                .Returns(A.CollectionOfFake<ArticleListingServiceModel>(3));

            var homeController = new HomeController(fakeArticleService);

            // Act
            var result = await homeController.Index();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsAssignableFrom<IEnumerable<ArticleListingServiceModel>>(viewResult.Model);
            Assert.Equal(3, model.Count());
        }

        [Fact]
        public void AboutShouldReturnViewResult()
        {
            // Arrange
            var homeController = new HomeController(null);

            // Act
            var result = homeController.About();

            // Assert
            Assert.IsType<ViewResult>(result);
        }

        [Fact]
        public void PrivacyShouldReturnViewResultWithCorrectUsername()
        {
            // Arrange
            var fakeHomeController = A.Fake<HomeController>();
           // A.CallTo(() => fakeHomeController.Privacy());
            A.CallTo(() => fakeHomeController.User.Identity.Name).Returns(TestConstants.TestUsername);
            // Act
            var result = fakeHomeController.Privacy();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<PrivacyViewModel>(viewResult.Model);
            Assert.Equal(TestConstants.TestUsername, model.Username);
        }
    }
}
