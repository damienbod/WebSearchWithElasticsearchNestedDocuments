using System.Collections.Generic;

namespace WebSearchWithElasticsearchNestedDocuments.Search
{
	public interface ISearchProvider
	{
		IEnumerable<SkillWithListOfDetails> QueryString(string term);

		void AddUpdateEntity(SkillWithListOfDetails skillWithListOfDetails);
		void UpdateSkill(long updateId, string updateName, string updateDescription, List<SkillDetail> updateSkillDetailsLis);
		void DeleteSkill(long updateId);
	}
}