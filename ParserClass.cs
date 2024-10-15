namespace BlazorHtmlString.Generator;
internal class ParserClass(IEnumerable<ClassDeclarationSyntax> list, Compilation compilation)
{
    public BasicList<ResultsModel> GetResults()
    {
        BasicList<ResultsModel> output = [];
        foreach (var item in list)
        {
            ResultsModel results = GetResult(item);
            //class names cannot be repeating.
            //that ended up being a good idea.
            if (output.Any(x => x.ClassName == results.ClassName) == false)
            {
                output.Add(results);
            }
        }
        return output;
    }
    private ResultsModel GetResult(ClassDeclarationSyntax classDeclaration)
    {
        ResultsModel output;
        //SemanticModel compilationSemanticModel = classDeclaration.GetSemanticModel(compilation);
        INamedTypeSymbol symbol = compilation.GetClassSymbol(classDeclaration)!;
        output = symbol.GetStartingResults<ResultsModel>();
        output.Assembly = $"{compilation.AssemblyName}.HtmlStringGenerators";
        var list = symbol.GetAllPublicProperties();
        foreach (var item in list)
        {
            if (item.IsParameter())
            {
                output.Properties.Add(GetPropertyDetails(item));
            }
        }
        return output;
    }
    //can eventually try to put something into the global since this is a very common situation.

    private string GetPropertyType(IPropertySymbol symbol, TemporaryModel temp)
    {
        string firsts = GetGenericType(symbol.Type);
        if (firsts != "")
        {
            return firsts; //there was a serious bug here.
        }
        return GetCategoryType(temp.VariableCustomCategory, temp.ContainingNameSpace, temp.UnderlyingSymbolName);
    }
    private string GetCategoryType(EnumSimpleTypeCategory category, string containingNamespace, string underlyingName)
    {
        string output;
        if (category == EnumSimpleTypeCategory.StandardEnum || category == EnumSimpleTypeCategory.None || category == EnumSimpleTypeCategory.CustomEnum)
        {
            output = $"global::{containingNamespace}.{underlyingName}";
        }
        else if (category == EnumSimpleTypeCategory.DateOnly || category == EnumSimpleTypeCategory.TimeOnly || category == EnumSimpleTypeCategory.DateTime)
        {
            output = category.ToString();
        }
        else
        {
            output = category.ToString().ToLower();
        }
        return output;
    }
    private PropertyInformation GetPropertyDetails(IPropertySymbol symbol)
    {
        TemporaryModel temp = symbol.GetStartingPropertyInformation<TemporaryModel>();
        PropertyInformation output = new();
        output.Name = temp.PropertyName;
        output.Type = GetPropertyType(symbol, temp);
        string possibleListName = output.Type.ToLower();
        if (possibleListName.EndsWith("list") || possibleListName.EndsWith("ienumerable")) //to support both lists and even ienumerable.
        {
            var mm = symbol.Type.GetSingleGenericTypeUsed();
            string firsts = output.Type;
            string nexts = GetGenericType(mm!);
            if (nexts == "")
            {
                EnumSimpleTypeCategory category = mm!.GetVariableCategory();
                nexts = GetCategoryType(category, mm!.ContainingNamespace.ToDisplayString(), mm.Name);
            }
            output.Type = $"{firsts}<{nexts}>";
        }
        return output;
    }
    //for now, this is repeating from somewhere else.  eventually will do more sharing once i know more.  since this is very common.
    private string GetGenericType(ITypeSymbol symbol)
    {
        string output;
        //for now.  can expand upon this.
        if (symbol.Name.ToLower() == "guid")
        {
            output = "Guid";
        }
        else if (symbol.Name.ToLower() == "int64")
        {
            output = "long";
        }
        else if (symbol.Name.ToLower() == "int32")
        {
            output = "int"; //has to be int in this case.
        }
        else
        {
            output = "";
        }
        return output;
    }
}