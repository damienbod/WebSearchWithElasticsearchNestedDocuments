using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Newtonsoft.Json;
using WebSearchWithElasticsearchNestedDocuments.Search;

namespace WebSearchWithElasticsearchNestedDocuments.Controllers
{
	[RoutePrefix("Search")]
	public class SearchController : Controller
	{
		readonly ISearchProvider _searchProvider = new ElasticSearchProvider();

		[HttpGet]
		public ActionResult Index()
		{
			return View();
		}

		[HttpPost]
		[Route("Index")]
		public ActionResult Index(SkillWithListOfDetails model, string createSkillDetailsList)
		{
			if (ModelState.IsValid)
			{
				model.Created = DateTime.UtcNow;
				model.Updated = DateTime.UtcNow;

				model.SkillDetails =
					JsonConvert.DeserializeObject(createSkillDetailsList, typeof(List<SkillDetail>)) as List<SkillDetail>;

				_searchProvider.AddUpdateEntity(model);
				return Redirect("Search/Index");
			}

			return View("Index", model);
		}

		[HttpPost]
		[Route("Update")]
		public ActionResult Update(long updateId, string updateName, string updateDescription, string updateSkillDetailsList)
		{
			_searchProvider.UpdateSkill(
				updateId,
				updateName, updateDescription, 
				JsonConvert.DeserializeObject(updateSkillDetailsList, typeof(List<SkillDetail>)) as List<SkillDetail>
				);

			return Redirect("Index");
		}

		[HttpPost]
		[Route("Delete")]
		public ActionResult Delete(long deleteId)
		{
			_searchProvider.DeleteSkill(deleteId);
			return Redirect("Index");
		}

		[Route("Search")]
		public JsonResult Search(string term)
		{
			return Json(_searchProvider.QueryString(term), "SkillWithListOfDetails", JsonRequestBehavior.AllowGet);
		}
	}
}
