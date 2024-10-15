namespace BlazorHtmlString.Generator;
public static class CustomExtensions
{
    //private static int _count;
    public static bool IsParameter(this ISymbol symbol)
    {
        string attributeName = "Parameter";
        if (attributeName.EndsWith("Attribute") == false)
        {
            attributeName = $"{attributeName}Attribute";
        }
        foreach (var attribute in symbol.GetAttributes())
        {
            if (attribute.AttributeClass is null)
            {
                continue;
            }
            if (attribute.AttributeClass.Name == attributeName)
            {
                if (attribute.AttributeClass.ConstructedFrom.ContainingAssembly.Name == "Microsoft.AspNetCore.Components")
                {
                    return true;
                }
            }
        }
        return false;
    }
}