using System;
using System.Reflection;

namespace WebSearchWithElasticsearchNestedDocuments.Areas.HelpPage.ModelDescriptions
{
    public interface IModelDocumentationProvider
    {
        string GetDocumentation(MemberInfo member);

        string GetDocumentation(Type type);
    }
}