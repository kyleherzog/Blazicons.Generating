﻿using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Blazicons.Generating.Internals;
using CodeCasing;
using HtmlAgilityPack;
using Microsoft.CodeAnalysis;

namespace Blazicons.Generating;

public static class GeneratorExecutionContextExtensions
{
    public static void WriteIconsClass(
        this GeneratorExecutionContext context,
        string className,
        string svgFolder,
        string searchPattern = "*.svg",
        Func<string, string>? propertyNameFromFileName = null,
        Func<string, bool>? isFileNameOk = null
        )
    {
        propertyNameFromFileName ??= GetMemberName;

        var attributesCollection = new AttributesCollection();

        var builder = new StringBuilder();

        builder.AppendLine("// <autogenerated/>");
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
        var attributesBuilder = new StringBuilder();
        var iconMembersBuilder = new StringBuilder();
        foreach (var file in files)
        {
            var svg = File.ReadAllText(Path.Combine(svgFolder, file));
            var svgDoc = new SvgDocument(svg);
            svgDoc.Scrub();
            var attributes = svgDoc.GetAttributes();
            var attributesIndex = attributesCollection.FindOrAdd(attributes);
            var svgContent = svgDoc.Document.DocumentNode.InnerHtml;

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
            iconMembersBuilder.Append($"SvgIcon {propertyName} => SvgIcon.FromContent(\"{svgContent}\", attributeSet{attributesIndex});");
        }

        builder.AppendLine(attributesBuilder.ToString());
        builder.AppendLine();
        builder.AppendLine(iconMembersBuilder.ToString());
        builder.AppendLine("}");
        context.AddSource($"{className}.g.cs", builder.ToString());
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