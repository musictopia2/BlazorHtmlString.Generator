namespace BlazorHtmlString.Generator;
internal static class WriterExtensions
{
    public static IWriter PopulateFirstParameters(this IWriter w, ResultsModel result)
    {
        if (result.Properties.Count == 0)
        {
            return w;
        }
        //this means there are properties.
        foreach (var p in result.Properties)
        {
            w.Write(", ");
            w.Write(p.Type)
                .Write(" ")
                .Write(p.Name.ChangeCasingForVariable(EnumVariableCategory.ParameterCamelCase));
        }
        return w;
    }
    public static IWriter PopulateRenderParameters(this IWriter w, ResultsModel result)
    {
        if (result.Properties.Count == 0)
        {
            return w;
        }
        w.Write("parameters");
        //this means there are properties.
        return w;
    }
}