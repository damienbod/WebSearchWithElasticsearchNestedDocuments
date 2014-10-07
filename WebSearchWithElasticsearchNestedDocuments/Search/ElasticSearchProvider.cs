using System;
using System.Collections.Generic;
using Nest;
using ElasticsearchCRUD;

namespace WebSearchWithElasticsearchNestedDocuments.Search
{
	public class ElasticSearchProvider : ISearchProvider
	{
		private const string ConnectionString = "http://localhost:9200/";
		private readonly IElasticSearchMappingResolver _elasticSearchMappingResolver = new ElasticSearchMappingResolver();

		private static readonly Uri Node = new Uri(ConnectionString);
		private static readonly ConnectionSettings Settings = new ConnectionSettings(Node, defaultIndex: "skillwithlistofdetailss");
		readonly ElasticClient _client = new ElasticClient(Settings);

		public IEnumerable<SkillWithListOfDetails> QueryString(string term)
		{
			if (term != null)
			{
				var names = term.Replace("+", " OR *");
				var searchResults = _client.Search<SkillWithListOfDetails>(s => s
					.From(0)
					.Size(10)
					.Query(q => q.QueryString(f => f.Query(names + "*")))
					);

				return searchResults.Documents;
			}

			var defaultResults = _client.Search<SkillWithListOfDetails>(s => s
					.From(0)
					.Size(10)
					.Query(q => q.QueryString(f => f.Query("*")))
					);

			return defaultResults.Documents;
		}

		public void AddUpdateEntity(SkillWithListOfDetails skillWithListOfDetails)
		{
			using (var context = new ElasticSearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				context.AddUpdateEntity(skillWithListOfDetails, skillWithListOfDetails.Id);
				context.SaveChanges();
			}
		}

		public void UpdateSkill(long updateId, string updateName, string updateDescription, List<SkillDetail> updateSkillDetailsList)
		{
			using (var context = new ElasticSearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				var skill = context.GetEntity<SkillWithListOfDetails>(updateId);
				skill.Updated = DateTime.UtcNow;
				skill.Name = updateName;
				skill.Description = updateDescription;
				skill.SkillDetails = updateSkillDetailsList;

				foreach (var item in skill.SkillDetails)
				{
					item.Updated = DateTime.UtcNow;
				}

				context.AddUpdateEntity(skill, skill.Id);
				context.SaveChanges();
			}
		}

		public void DeleteSkill(long deleteId)
		{
			using (var context = new ElasticSearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				context.DeleteEntity<SkillWithListOfDetails>(deleteId);
				context.SaveChanges();
			}
		}
	}
}