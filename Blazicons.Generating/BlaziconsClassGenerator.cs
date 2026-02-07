using System.Text;
using Blazicons.Generating.Internals;
using CodeCasing;

namespace Blazicons.Generating;

/// <summary>
/// Provides functionality to generate a C# class file
/// containing properties that return SvgIcon instances
/// based on SVG files found in a specified folder.
/// </summary>
public static class BlaziconsClassGenerator
{
    public static readonly string[] ExcludedAttributes = ["class", "xmlns"];

    /// <summary>
    /// Generates a C# class file at the specified output path, with properties
    /// representing SVG icons found in the given folder. Each property returns
    /// an instance of SvgIcon created from the corresponding SVG file's content
    /// and attributes.
    /// </summary>
    /// <param name="outputFilePath">
    /// The file path where the generated C# class file will be saved. If the
    /// directory does not exist, it will be created.
    /// </param>
    /// <param name="className">
    /// The name of the class to be generated.
    /// </param>
    /// <param name="svgFolder">
    /// The folder path where the SVG files are located. The generator will search
    /// for SVG files in this folder and its subdirectories based on the specified
    /// search pattern.
    /// </param>
    /// <param name="searchPattern">
    /// An optional search pattern to filter the SVG files. The default value is "*.svg",
    /// </param>
    /// <param name="propertyNameFromFileName">
    /// An optional function that takes a file name as input and returns a string to be used
    /// as the property name for the corresponding SVG icon. If not provided, the generator will
    /// use the file name (without extension) converted to PascalCase as the property name.
    /// </param>
    /// <param name="isFileNameOk">
    /// An optional function that takes a file name as input and returns a boolean indicating whether
    /// the file name is valid for inclusion in the generated class. If not provided, all files will
    /// be included.
    /// </param>
    /// <param name="skipColorScrub">
    /// A boolean flag indicating whether to skip the color scrubbing process when generating the
    /// SVG content for the icons. If set to true, the original colors in the SVG files will be
    /// preserved; if false, colors may be scrubbed or modified as part of the generation process.
    /// The default value is false.
    /// </param>
    public static void GenerateClassFile(
        string outputFilePath,
        string className,
        string svgFolder,
        string searchPattern = "*.svg",
        Func<string, string>? propertyNameFromFileName = null,
        Func<string, bool>? isFileNameOk = null,
        bool skipColorScrub = false
        )
    {
        var generatedCode = GenerateClass(
            className,
            svgFolder,
            searchPattern,
            propertyNameFromFileName,
            isFileNameOk,
            skipColorScrub);

        var directory = Path.GetDirectoryName(outputFilePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(outputFilePath, generatedCode);
    }

    /// <summary>
    /// Generates a C# class, with properties representing SVG icons found in
    /// the given folder. Each property returns an instance of SvgIcon
    /// created from the corresponding SVG file's content and attributes.
    /// </summary>
    /// <param name="className">
    /// The name of the class to be generated.
    /// </param>
    /// <param name="svgFolder">
    /// The folder path where the SVG files are located. The generator will search
    /// for SVG files in this folder and its subdirectories based on the specified
    /// search pattern.
    /// </param>
    /// <param name="searchPattern">
    /// An optional search pattern to filter the SVG files. The default value is "*.svg",
    /// </param>
    /// <param name="propertyNameFromFileName">
    /// An optional function that takes a file name as input and returns a string to be used
    /// as the property name for the corresponding SVG icon. If not provided, the generator will
    /// use the file name (without extension) converted to PascalCase as the property name.
    /// </param>
    /// <param name="isFileNameOk">
    /// An optional function that takes a file name as input and returns a boolean indicating whether
    /// the file name is valid for inclusion in the generated class. If not provided, all files will
    /// be included.
    /// </param>
    /// <param name="skipColorScrub">
    /// A boolean flag indicating whether to skip the color scrubbing process when generating the
    /// SVG content for the icons. If set to true, the original colors in the SVG files will be
    /// preserved; if false, colors may be scrubbed or modified as part of the generation process.
    /// The default value is false.
    /// </param>
    public static string GenerateClass(
        string className,
        string svgFolder,
        string searchPattern = "*.svg",
        Func<string, string>? propertyNameFromFileName = null,
        Func<string, bool>? isFileNameOk = null,
        bool skipColorScrub = false
        )
    {
        propertyNameFromFileName ??= GetMemberName;

        var attributesCollection = new AttributesCollection();

        var builder = new StringBuilder();

        builder.AppendLine("// <autogenerated/>");
        builder.AppendLine("using System.Collections.ObjectModel;");
        builder.AppendLine("namespace Blazicons;"); // Use Target Namespace
        builder.AppendLine("/// <summary>");
        builder.AppendLine($"/// Provides icons from the {className} library.");
        builder.AppendLine("/// </summary>");
        builder.AppendLine($"public static class {className}");
        builder.AppendLine("{");

        var files = Directory.GetFiles(svgFolder, searchPattern, SearchOption.AllDirectories);

        if (isFileNameOk is not null)
        {
            files = files.Where(x => isFileNameOk(x)).OrderBy(x => x.ToLowerInvariant()).ToArray();
        }

        var propertyNames = new List<string>();
        var iconMembersBuilder = new StringBuilder();
        foreach (var file in files)
        {
            var svg = File.ReadAllText(Path.Combine(svgFolder, file));
            var svgDoc = new SvgDocument(svg);
            svgDoc.Scrub(skipColorScrub);
            var attributes = svgDoc.GetAttributes();
            foreach (var exclude in ExcludedAttributes)
            {
                attributes.Remove(exclude);
            }

            var attributesIndex = attributesCollection.FindOrAdd(attributes);
            var svgContent = svgDoc.SvgNode.InnerHtml.Replace("\"", "\\\"");
            var svgContentOneLine = svgContent.Replace("\r", "").Replace("\n", "");


            var propertyName = ScrubPropertyName(propertyNameFromFileName(file));
            propertyNames.Add(propertyName);
            iconMembersBuilder.AppendLine("/// <summary>");
            iconMembersBuilder.AppendLine($"/// Gets the {propertyName} SvgIcon from the {className} library.");
            iconMembersBuilder.AppendLine("/// </summary>");
            iconMembersBuilder.Append("public static ");
            if (propertyName == "Equals")
            {
                iconMembersBuilder.Append("new ");
            }
            iconMembersBuilder.AppendLine($"SvgIcon {propertyName} => SvgIcon.FromContent(\"{svgContentOneLine.Trim()}\", attributeSet{attributesIndex});");
        }

        builder.AppendLine(attributesCollection.ToCSharp());
        builder.AppendLine();
        builder.AppendLine(iconMembersBuilder.ToString());
        builder.AppendLine("}");

        return builder.ToString();
    }

    private static string ScrubPropertyName(string name)
    {
        var result = name;

        switch (result[0])
        {
            case '1':
                result = $"One{result.Substring(1)}";
                break;

            case '2':
                result = $"Two{result.Substring(1)}";
                break;

            case '3':
                result = $"Three{result.Substring(1)}";
                break;

            case '4':
                result = $"Four{result.Substring(1)}";
                break;

            case '5':
                result = $"Five{result.Substring(1)}";
                break;

            case '6':
                result = $"Six{result.Substring(1)}";
                break;

            case '7':
                result = $"Seven{result.Substring(1)}";
                break;

            case '8':
                result = $"Eight{result.Substring(1)}";
                break;

            case '9':
                result = $"Nine{result.Substring(1)}";
                break;

            case '0':
                result = $"Zero{result.Substring(1)}";
                break;

            default:
                break;
        }

        return result;
    }

    private static string GetMemberName(string fileName)
    {
        return Path.GetFileNameWithoutExtension(fileName).ToPascalCase();
    }
}
