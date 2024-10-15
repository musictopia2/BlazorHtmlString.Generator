namespace BlazorHtmlString.Generator;
internal record ResultsModel : ICustomResult
{
    public string ClassName { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string Assembly { get; set; } = ""; //this is for the assembly.  best name i can come up with
    public BasicList<PropertyInformation> Properties { get; set; } = [];
}