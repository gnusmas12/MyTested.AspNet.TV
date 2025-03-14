﻿namespace Blog.Test.Controllers
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Blog.Controllers;
    using Blog.Controllers.Models;
    using Blog.Services.Data;
    using Blog.Services.Machine;
    using Blog.Services.Models;
    using Extensions;
    using FakeItEasy;
    using Fakes;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Xunit;

    public class ArticlesControllerTest
    {
        [Fact]
        public async Task AllShouldReturnViewWithCorrectModel()
        {
            // Arrange
            const int pageSize = 2;
            var articleService = new FakeArticleService();
            var fakeArticleSerbice = A.Fake<FakeArticleService>();

            var articlesController = this.GetArticlesController(fakeArticleSerbice, pageSize);


            // Act
            var result = await articlesController.All();

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var model = Assert.IsType<ArticleListingViewModel>(viewResult.Model);
            Assert.Equal(pageSize, model.Articles.Count());
            Assert.Equal(await articleService.Total(), model.Total);
            Assert.Equal(1, model.Page);
        }

        [Fact]
        public async Task RandomShouldReturnViewWithCorrectArticle()
        {
            // Arrange
            const int index = 0;
            const int id = 1;

            var articlesController = this.GetArticlesController()
                .WithTestUser();

            var fakeRandomService = A.Fake<IRandomService>();

            A
                .CallTo(() => fakeRandomService.Next(A<int>.Ignored, A<int>.Ignored))
                .Returns(index);

            // Act
            var result = await articlesController.Random(fakeRandomService);

            // Assert
            var viewResult = Assert.IsType<ViewResult>(result);
            var article = Assert.IsType<ArticleDetailsServiceModel>(viewResult.Model);
            Assert.Equal(id, article.Id);
        }

        private ArticlesController GetArticlesController(IArticleService articleService = null, int pageSize = 10)
        {
            articleService ??= new FakeArticleService();
            var configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(new[]
                {
                    new KeyValuePair<string, string>("Articles:PageSize", pageSize.ToString())
                })
                .Build();

            return new ArticlesController(articleService, null, configuration);
        }
    }
}
