namespace BlazorHtmlString.Generator;
internal class EmitClass(ImmutableArray<ResultsModel> results, SourceProductionContext context)
{
    public void Emit()
    {
        foreach (var item in results)
        {
            WriteItem(item);
        }
    }
    private void WriteItem(ResultsModel item)
    {
        SourceCodeStringBuilder builder = new();
        builder.WriteLine("#nullable enable")
            .WriteLine(w =>
            {
                w.Write("namespace ")
                .Write(item.Assembly)
                .Write(";");
            })
            .WriteLine("public static partial class CompleteGeneratorClass")
            .WriteCodeBlock(w =>
            {
                PopulateDetails(w, item);
            });
        context.AddSource($"{item.ClassName}.BlazorHtmlString.g.cs", builder.ToString()); //change sample to what you want.
    }
    private void PopulateDetails(ICodeBlock w, ResultsModel result)
    {
        w.WriteLine(w =>
        {
            w.Write($"public static async Task<string> GetBlazorStringFrom{result.ClassName}Async(this global::Microsoft.AspNetCore.Components.Web.HtmlRenderer renderer")
            .PopulateFirstParameters(result).Write(")");

        }).WriteCodeBlock(w =>
        {
            w.WriteLine(w =>
            {
                w.Write("string html = ").AppendDoubleQuote().Write(";");
            })
            .WriteLine("await renderer!.Dispatcher.InvokeAsync(async () =>")
            .WriteLambaBlock(w =>
            {
                w.PopulatePossibleDictionary(result)
                .WriteLine(w =>
                {
                    w.Write($"var output = await renderer!.RenderComponentAsync<{result.Namespace}.{result.ClassName}>(")
                    .PopulateRenderParameters(result)
                    .Write(");");
                });
                w.WriteLine("html = output.ToHtmlString();"); //at the end no matter what
            });
            w.WriteLine("return html;");
        });
    }
}