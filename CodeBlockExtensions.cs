namespace BlazorHtmlString.Generator;
internal static class CodeBlockExtensions
{
    public static ICodeBlock PopulatePossibleDictionary(this ICodeBlock w, ResultsModel result)
    {
        if (result.Properties.Count == 0)
        {
            return w;
        }
        w.WriteLine("var dictionary = new Dictionary<string, object?>();");
        foreach (var p in result.Properties)
        {
            w.WriteLine($"""
                dictionary.Add("{p.Name}", {p.Name.ChangeCasingForVariable(EnumVariableCategory.ParameterCamelCase)});
                """);
        }
        w.WriteLine("var parameters = global::Microsoft.AspNetCore.Components.ParameterView.FromDictionary(dictionary);");
        return w;
    }
}