using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebSearchWithElasticsearchNestedDocuments.Search
{
	public class SkillWithListOfDetails
	{
		[Required]
		[Range(1, long.MaxValue)]
		public long Id { get; set; }
		[Required]
		public string Name { get; set; }
		[Required]
		public string Description { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }

		public List<SkillDetail> SkillDetails { get; set; }
	}

	public class SkillDetail
	{
		[Required]
		[Range(1, long.MaxValue)]
		public long Id { get; set; }
		[Required]
		public string SkillLevel { get; set; }
		[Required]
		public string Details { get; set; }
		public DateTimeOffset Created { get; set; }
		public DateTimeOffset Updated { get; set; }
	}
}