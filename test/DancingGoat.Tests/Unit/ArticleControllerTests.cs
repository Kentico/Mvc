using System.Net;

using CMS.DataEngine;
using CMS.DocumentEngine;
using CMS.DocumentEngine.Types.DancingGoatMvc;
using CMS.Tests;

using DancingGoat.Models.Articles;
using DancingGoat.Controllers;
using DancingGoat.Infrastructure;
using DancingGoat.Repositories;
using DancingGoat.Tests.Extensions;

using NSubstitute;
using NUnit.Framework;
using Tests.DocumentEngine;
using TestStack.FluentMVCTesting;

namespace DancingGoat.Tests.Unit
{
    [TestFixture]
    [Category("Unit")]
    public class ArticlesControllerTests : UnitTests
    {
        private ArticlesController mController;
        private Article mArticle;
        private IOutputCacheDependencies mDependencies;
        private const string ARTICLE_TITLE = "Article1";


        [SetUp]
        public void SetUp()
        {
            Fake().DocumentType<Article>(Article.CLASS_NAME);
            mArticle = TreeNode.New<Article>().With(a => a.Fields.Title = ARTICLE_TITLE);
            mDependencies = Substitute.For<IOutputCacheDependencies>();
            
            var repository = Substitute.For<IArticleRepository>();
            repository.GetArticle(1).Returns(mArticle);
            
            mController = new ArticlesController(repository, mDependencies);
        }


        [Test]
        public void Index_RendersDefaultView()
        {
            mController.WithCallTo(c => c.Index())
                .ShouldRenderDefaultView();
        }


        [Test]
        public void Show_WithExistingArticle_RendersDefaultViewWithCorrectModel()
        {
            mController.WithCallTo(c => c.Show(1, null))
                .ShouldRenderDefaultView()
                .WithModelMatchingCondition<ArticleViewModel>(x => x.Title == ARTICLE_TITLE);
        }


        [Test]
        public void Show_WithoutExistingArticle_ReturnsHttpNotFoundStatusCode()
        {
            mController.WithCallTo(c => c.Show(2, null))
                .ShouldGiveHttpStatus(HttpStatusCode.NotFound);
        }
    }
}