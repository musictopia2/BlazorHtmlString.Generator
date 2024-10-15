namespace BlazorHtmlString.Generator;
[Generator] //this is important so it knows this class is a generator which will generate code for a class using it.
public class MySourceGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(c =>
        {
            c.CreateCustomSource().BuildSourceCode();
        });
        IncrementalValuesProvider<ClassDeclarationSyntax> declares1 = context.SyntaxProvider.CreateSyntaxProvider(
            (s, _) => IsSyntaxTarget(s),
            (t, _) => GetTarget(t))
            .Where(m => m != null)!;
        var declares2 = context.CompilationProvider.Combine(declares1.Collect());
        var declares3 = declares2.SelectMany(static (x, _) =>
        {
            ImmutableHashSet<ClassDeclarationSyntax> start = [.. x.Right];
            return GetResults(start, x.Left);
        });
        var declares4 = declares3.Collect(); //if you need compilation, then look at past samples.  try to do without compilation at the end if possible
        context.RegisterSourceOutput(declares4, Execute);
    }
    //decided to not worry if its partial.  since if it needs to be partial, will get immediate compile errors anyways.
    private bool IsSyntaxTarget(SyntaxNode syntax)
    {
        return syntax is ClassDeclarationSyntax ctx &&
            ctx.IsPublic();
    }
    private bool ImplementsIBlazorHtmlString(INamedTypeSymbol? symbol)
    {
        if (symbol is null)
        {
            return false;
        }
        return symbol.Implements(nameof(IBlazorHtmlString));
    }
    private ClassDeclarationSyntax? GetTarget(GeneratorSyntaxContext context)
    {
        var ourClass = context.GetClassNode(); //can use the sematic model at this stage
        var symbol = context.GetClassSymbol(ourClass);
        if (symbol.IsAbstract)
        {
            return null; //because we cannot accept abstracts.
        }
        if (ImplementsIBlazorHtmlString(symbol) == false)
        {
            return null;
        }
        return ourClass;
    }
    private static ImmutableHashSet<ResultsModel> GetResults(
        ImmutableHashSet<ClassDeclarationSyntax> classes,
        Compilation compilation
        )
    {
        ParserClass parses = new(classes, compilation);
        BasicList<ResultsModel> output = parses.GetResults();
        return [.. output];
    }
    private void Execute(SourceProductionContext context, ImmutableArray<ResultsModel> list)
    {
        EmitClass emit = new(list, context);
        emit.Emit(); //start out with console.  later do reals once ready.
    }
}