using Microsoft.SemanticKernel.SkillDefinition;

namespace PowerMatt.Sample.Plugin;

public class DocumentExpert
{
    [SKFunction("Will return a document from a URL. The INPUT requires a URL that came from a function that specifically returns URLs")]
    public string RetrieveDocumentFromUrl(string url)
    {
        return @"Semantic Kernel is an open-source SDK";
    }
}