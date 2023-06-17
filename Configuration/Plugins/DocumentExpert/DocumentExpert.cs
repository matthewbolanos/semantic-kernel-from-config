using Microsoft.SemanticKernel.SkillDefinition;

namespace PowerMatt.Sample.Plugin;

public class DocumentExpert
{
    [SKFunction("Retrieve a document when provided a URL")]
    public string RetrieveDocumentWithUrl(string url)
    {
        return @"Semantic Kernel is an open-source SDK";
    }
}