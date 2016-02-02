using System;
using System.Linq;
using System.Web.Mvc;

using CMS.DocumentEngine.Types;

using DancingGoat.Infrastructure;
using DancingGoat.Models.Articles;
using DancingGoat.Repositories;

namespace DancingGoat.Controllers
{
    public class ArticlesController : Controller
    {
        private readonly IArticleRepository mArticleRepository;
        private readonly IOutputCacheDependencies mOutputCacheDependencies;


        public ArticlesController(IArticleRepository repository, IOutputCacheDependencies outputCacheDependencies)
        {
            mArticleRepository = repository;
            mOutputCacheDependencies = outputCacheDependencies;
        }


        // GET: Articles
        [OutputCache(CacheProfile = "Default", VaryByParam = "none")]
        public ActionResult Index()
        {
            var articles = mArticleRepository.GetArticles();
            mOutputCacheDependencies.AddDependencyOnPages<Article>();

            return View(articles.Select(ArticleViewModel.GetViewModel));
        }


        // GET: Articles/Show/{id}
        [OutputCache(CacheProfile = "Default", VaryByParam = "id")]
        public ActionResult Show(int id, string pageAlias)
        {
            var article = mArticleRepository.GetArticle(id);

            if (article == null)
            {
                return HttpNotFound();
            }

            // Redirect if page alias does not match
            if (!string.IsNullOrEmpty(pageAlias) && !article.NodeAlias.Equals(pageAlias, StringComparison.InvariantCultureIgnoreCase))
            {
                return RedirectToActionPermanent("Show", new { id = article.NodeID, pageAlias = article.NodeAlias });
            }

            mOutputCacheDependencies.AddDependencyOnPages<Article>();

            return View(ArticleViewModel.GetViewModel(article));
        }
    }
}