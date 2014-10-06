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

		public IEnumerable<SkillWithListOfDetails> QueryString(string term)
		{
			var node = new Uri("http://localhost:9200");
			var settings = new ConnectionSettings(node, defaultIndex: "skillwithlistofdetailss");
			var client = new ElasticClient(settings);

			if (term != null)
			{
				var names = term.Replace("+", " OR *");
				var searchResults = client.Search<SkillWithListOfDetails>(s => s
					.From(0)
					.Size(10)
					.Query(q => q.QueryString(f => f.Query(names + "*")))
					);

				return searchResults.Documents;
			}

			var defaultResults = client.Search<SkillWithListOfDetails>(s => s
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

		public void UpdateSkill(long updateId, string updateName, string updateDescription)
		{
			using (var context = new ElasticSearchContext(ConnectionString, _elasticSearchMappingResolver))
			{
				var skill = context.GetEntity<SkillWithListOfDetails>(updateId);
				skill.Updated = DateTime.UtcNow;
				skill.Name = updateName;
				skill.Description = updateDescription;
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