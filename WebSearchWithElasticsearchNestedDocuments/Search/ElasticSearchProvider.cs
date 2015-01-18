using System;
using System.Collections.Generic;
using System.Linq;
using ElasticsearchCRUD.Model.SearchModel;
using ElasticsearchCRUD.Model.SearchModel.Queries;
using ElasticsearchCRUD;

namespace WebSearchWithElasticsearchNestedDocuments.Search
{
	public class ElasticSearchProvider : ISearchProvider
	{
		private const string ConnectionString = "http://localhost:9200/";
		private readonly IElasticsearchMappingResolver _elasticSearchMappingResolver = new ElasticsearchMappingResolver();

		private static readonly Uri Node = new Uri(ConnectionString);


		public IEnumerable<SkillWithListOfDetails> QueryString(string term)
		{
			var names = "";
			if (term != null)
			{
				names = term.Replace("+", " OR *");
			}

			var search = new ElasticsearchCRUD.Model.SearchModel.Search
			{
				From= 0,
				Size = 10,
				Query = new Query(new QueryStringQuery(names + "*"))
			};
			IEnumerable<SkillWithListOfDetails> results;
			using (var context = new ElasticsearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				results = context.Search<SkillWithListOfDetails>(search).PayloadResult.Hits.HitsResult.Select(t => t.Source);
			}
			return results;
		}

		public void AddUpdateEntity(SkillWithListOfDetails skillWithListOfDetails)
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				context.AddUpdateDocument(skillWithListOfDetails, skillWithListOfDetails.Id);
				context.SaveChanges();
			}
		}

		public void UpdateSkill(long updateId, string updateName, string updateDescription, List<SkillDetail> updateSkillDetailsList)
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				var skill = context.GetDocument<SkillWithListOfDetails>(updateId);
				skill.Updated = DateTime.UtcNow;
				skill.Name = updateName;
				skill.Description = updateDescription;
				skill.SkillDetails = updateSkillDetailsList;

				foreach (var item in skill.SkillDetails)
				{
					item.Updated = DateTime.UtcNow;
				}

				context.AddUpdateDocument(skill, skill.Id);
				context.SaveChanges();
			}
		}

		public void DeleteSkill(long deleteId)
		{
			using (var context = new ElasticsearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				context.DeleteDocument<SkillWithListOfDetails>(deleteId);
				context.SaveChanges();
			}
		}
	}
}