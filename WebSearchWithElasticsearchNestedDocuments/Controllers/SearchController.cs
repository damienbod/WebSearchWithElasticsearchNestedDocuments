using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
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
		public ActionResult Index(SkillWithListOfDetails model)
		{
			if (ModelState.IsValid)
			{
				model.Created = DateTime.UtcNow;
				model.Updated = DateTime.UtcNow;

				// TEST CODE To be removed
				var skillDetailSeniorDepartment = new SkillDetail
				{
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					Id = 1,
					Details = "This skill is required by all people in department X",
					SkillLevel = "Senior"
				};

				var skillDetailCommunication = new SkillDetail
				{
					Created = DateTime.UtcNow,
					Updated = DateTime.UtcNow,
					Id = 1,
					Details = "This skill is required by consultants",
					SkillLevel = "Senior"
				};
				model.SkillDetails = new List<SkillDetail> {skillDetailSeniorDepartment, skillDetailCommunication};


				_searchProvider.AddUpdateEntity(model);
				return Redirect("Search/Index");
			}

			return View("Index", model);
		}

		[HttpPost]
		[Route("Update")]
		public ActionResult Update(long updateId, string updateName, string updateDescription)
		{
			_searchProvider.UpdateSkill(updateId, updateName, updateDescription);
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
