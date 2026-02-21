# Blazicons.Generating
Provides support for generating [Blazicon](https://github.com/kyleherzog/Blazicons) library packages.

See the [changelog](CHANGELOG.md) for change history.  

![Nuget](https://img.shields.io/nuget/v/Blazicons.Generating)

[![Build Status](https://dev.azure.com/kyleherzog/Blazicons/_apis/build/status%2Fkyleherzog.Blazicons.Generating?branchName=main)](https://dev.azure.com/kyleherzog/Blazicons/_build/latest?definitionId=34&branchName=main)

## Overview
Blazicons.Generating is a library designed to assist in generating code for Blazicon icon library implementations. It does this by providing helper classes for reading the source repositories of open-source font libraries and for the generation of Blazicon libraries.

## Getting Started

### Installation
Add a reference to the Blazicons.Generating package in your project.

### Basic Configuration
Configure your `.csproj` file with the required properties to generate icon classes:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Blazicons code generation configuration -->
  <PropertyGroup>
    <BlaziconsRepoUrl>https://github.com/example/icons/archive/refs/heads/main.zip</BlaziconsRepoUrl>
    <BlaziconsSvgPattern>^src\/svg\/.*.svg$</BlaziconsSvgPattern>
    <BlaziconsClassName>MyIcon</BlaziconsClassName>
    <BlaziconsSvgFolderPath>src/svg</BlaziconsSvgFolderPath>
    <BlaziconsPropertyNameRemovalPatterns>^My_Icon_;_24_\w*$;-(original|plain|line)</BlaziconsPropertyNameRemovalPatterns>
    <BlaziconsSkipColorScrub>true</BlaziconsSkipColorScrub>
    <BlaziconsGeneratedCodeOutputPath>Generated</BlaziconsGeneratedCodeOutputPath>
    <BlaziconsGeneratorPath>MyIcons.Generating/MyIcons.Generating.MyIconsGenerator</BlaziconsGeneratorPath>
  </PropertyGroup>
</Project>
```

**Property Name Transformation Examples:**

Patterns are applied in order. Use regex anchors to control where matching occurs:
- `^pattern` - Removes from the start (prefix removal)
- `pattern$` - Removes from the end (suffix removal)
- `pattern` - Removes anywhere in the filename

Example with the above configuration:
- `Ic_Fluent_Activity_24_regular.svg` → `Activity` (prefix `^Ic_Fluent_` removed, then suffix `_24_\w*$` removed)
- `Ic_Fluent_Person_24_filled.svg` → `Person` (prefix `^Ic_Fluent_` removed, then suffix `_24_\w*$` removed)
- `react-plain.svg` → `React` (pattern `-(original|plain|line)` removed)
- `angular-original.svg` → `Angular` (pattern `-(original|plain|line)` removed)

### Configuration Properties

| Property | Required | Description | Example |
|----------|----------|-------------|---------|
| `BlaziconsRepoUrl` | Yes* | URL to a .zip file containing the icon repository | `https://github.com/my-team/myicons/archive/refs/heads/main.zip` |
| `BlaziconsRepoPath` | Yes* | Local path to a repository (alternative to RepoUrl) | `C:\Source\icons` |
| `BlaziconsSvgPattern` | Yes | Regex pattern to filter SVG files | `^src\/svg\/.*.svg$` |
| `BlaziconsClassName` | Yes | Name of the generated icon class | `MyIcon` |
| `BlaziconsSvgFolderPath` | No | Relative path within the repo to the SVG folder | `src/svg` |
| `BlaziconsPropertyNameRemovalPatterns` | No | Semicolon-delimited regex patterns to remove from file names: `^pattern` (prefix), `pattern$` (suffix), `pattern` (anywhere) | `^My_Icons_;_24_\w*$;-(original\|plain\|line)` |
| `BlaziconsSkipColorScrub` | No | Skip color scrubbing for SVG content | `true` |
| `BlaziconsGeneratedCodeOutputPath` | Yes | Output directory for generated code | `Generated` |
| `BlaziconsGeneratorPath` | No | Generator namespace/path structure for the generated file | `Blazicons.MyIcons.Generating/Blazicons.MyIcons.Generating.MyIconsGenerator` |

\* Either `BlaziconsRepoUrl` or `BlaziconsRepoPath` must be specified.

## Advanced Configuration

### Generating Multiple Icon Classes
For cases where multiple icon sets are desired in one package (e.g., different aspect ratios or styles), the recommended approach is to create separate non-packable generator projects that output to your main project.

my-icons/  
├─ MyIcons.csproj (main project, packable)  
├─ MyIcons.Filled/  
│   ├─ MyIcons.Filled.csproj (generator project, non-packable)  
├─ MyIcons.Outlined/  
│   ├─ MyIcons.Outlined.csproj (generator project, non-packable)  



### Build-Time Generation Control
Code generation is controlled by the `BlaziconsEnableCodeGeneration` property, which is automatically set based on configuration file presence. To manually control generation:

```xml
<PropertyGroup>
  <BlaziconsEnableCodeGeneration>true</BlaziconsEnableCodeGeneration>
</PropertyGroup>
```

## Troubleshooting

### Code not generating
1. Ensure `BlaziconsEnableCodeGeneration` is set to `true`
2. Verify all required properties are configured
3. Check that the `BlaziconsRepoUrl` or `BlaziconsRepoPath` is valid and accessible
4. Review build output for error messages from the generator

### Build errors after generation
1. Clean the solution and rebuild
2. Verify the generated code is in the expected output directory
3. Ensure the target frameworks match your project requirements