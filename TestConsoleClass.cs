namespace BlazorHtmlString.Generator;
internal class TestConsoleClass(ImmutableArray<ResultsModel> results, SourceProductionContext context)
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
        builder.StartPartialClassConsoleWriter(item, w =>
        {
            w.ConsoleWriteLine($"The assembly namespace will be {item.Assembly}");
            w.ConsoleWriteLine($"Generic Will Be global::{item.Namespace}.{item.ClassName}");
            w.ConsoleWriteLine($"The method will be called GetBlazorStringFrom{item.ClassName}Async");
            w.ConsoleWriteLine("Here are the properties");
            foreach (var p in item.Properties)
            {
                w.ConsoleWriteLine($"{p.Name} has the following type information to be written:  {p.Type}.  When writing to parameters for the method, the name to use is {p.Name.ChangeCasingForVariable(EnumVariableCategory.ParameterCamelCase)}");
                //w.ConsoleWriteLine($"{p.PropertyName} from {p.UnderlyingSymbolName} underlying symbol name with {p.ContainingNameSpace} containing namespace.  The variable custom category is {p.VariableCustomCategory}.  Nullable is {p.Nullable}");
            }

        });
        context.AddSource($"{item.ClassName}.Sample.g.cs", builder.ToString()); //figure out what you want for the name for printing.
    }
}