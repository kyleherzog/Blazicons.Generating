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
    <BlaziconsGeneratedCodeOutputPath>Generated</BlaziconsGeneratedCodeOutputPath>
    <BlaziconsGeneratorPath>MyIcons.Generating/MyIcons.Generating.MyIconsGenerator</BlaziconsGeneratorPath>
  </PropertyGroup>
</Project>
```

### Configuration Properties

| Property | Required | Description | Example |
|----------|----------|-------------|---------|
| `BlaziconsRepoUrl` | Yes* | URL to a .zip file containing the icon repository | `https://github.com/ionic-team/ionicons/archive/refs/heads/main.zip` |
| `BlaziconsRepoPath` | Yes* | Local path to a repository (alternative to RepoUrl) | `C:\Source\icons` |
| `BlaziconsSvgPattern` | Yes | Regex pattern to filter SVG files | `^src\/svg\/.*.svg$` |
| `BlaziconsClassName` | Yes | Name of the generated icon class | `Ionicon` |
| `BlaziconsSvgFolderPath` | No | Relative path within the repo to the SVG folder | `src/svg` |
| `BlaziconsGeneratedCodeOutputPath` | Yes | Output directory for generated code | `Generated` |
| `BlaziconsGeneratorPath` | No | Generator namespace/path structure for the generated file | `Blazicons.Ionicons.Generating/Blazicons.Ionicons.Generating.IoniconsGenerator` |

\* Either `BlaziconsRepoUrl` or `BlaziconsRepoPath` must be specified.

## Advanced Configuration

### Generating Multiple Icon Classes
If your icon library contains multiple icon sets (e.g., different aspect ratios or styles), you can override the default `GenerateBlazicons` target to generate multiple classes:

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <!-- Common configuration -->
  <PropertyGroup>
    <BlaziconsRepoUrl>https://github.com/lipis/flag-icons/archive/refs/heads/main.zip</BlaziconsRepoUrl>
    <BlaziconsGeneratedCodeOutputPath>Generated</BlaziconsGeneratedCodeOutputPath>
    <BlaziconsGeneratorPath>MyIcons.Generating/MyIcons.Generating.MyIconsGenerator</BlaziconsGeneratorPath>
  </PropertyGroup>

  <!-- Configuration for first icon set -->
  <PropertyGroup>
    <BlaziconsSvgPattern_4x3>^flags\/4x3\/.*.svg$</BlaziconsSvgPattern_4x3>
    <BlaziconsClassName_4x3>FlagIcon4x3</BlaziconsClassName_4x3>
    <BlaziconsSvgFolderPath_4x3>flags/4x3</BlaziconsSvgFolderPath_4x3>
  </PropertyGroup>

  <!-- Configuration for second icon set -->
  <PropertyGroup>
    <BlaziconsSvgPattern_1x1>^flags\/1x1\/.*.svg$</BlaziconsSvgPattern_1x1>
    <BlaziconsClassName_1x1>FlagIcon1x1</BlaziconsClassName_1x1>
    <BlaziconsSvgFolderPath_1x1>flags/1x1</BlaziconsSvgFolderPath_1x1>
  </PropertyGroup>

  <!-- Override the default target to generate multiple icon classes -->
  <Target Name="GenerateBlazicons"
          BeforeTargets="BeforeBuild"
          Condition="'$(BlaziconsEnableCodeGeneration)' == 'true' AND ( '$(TargetFramework)' == '' OR ( '$(TargetFrameworks)' != '' AND '$(TargetFramework)' == $([System.String]::Copy('$(TargetFrameworks)').Split(';')[0]) ) )"
          Inputs="$(MSBuildProjectFullPath)"
          Outputs="$(BlaziconsGeneratedCodeOutputPath)\**\*.g.cs">

    <PropertyGroup>
      <_BlaziconsGeneratedOutputDir>$([System.IO.Path]::Combine('$(MSBuildProjectDirectory)', '$(BlaziconsGeneratedCodeOutputPath)'))</_BlaziconsGeneratedOutputDir>
    </PropertyGroup>

    <Message Text="Running Blazicons generator for $(MSBuildProjectName)..." Importance="high" />

    <!-- Generate first icon class -->
    <GenerateBlaziconsTask
      RepoUrl="$(BlaziconsRepoUrl)"
      SvgPattern="$(BlaziconsSvgPattern_4x3)"
      ClassName="$(BlaziconsClassName_4x3)"
      OutputPath="$(_BlaziconsGeneratedOutputDir)"
      SvgFolderPath="$(BlaziconsSvgFolderPath_4x3)"
      GeneratorPath="$(BlaziconsGeneratorPath)" />

    <!-- Generate second icon class -->
    <GenerateBlaziconsTask
      RepoUrl="$(BlaziconsRepoUrl)"
      SvgPattern="$(BlaziconsSvgPattern_1x1)"
      ClassName="$(BlaziconsClassName_1x1)"
      OutputPath="$(_BlaziconsGeneratedOutputDir)"
      SvgFolderPath="$(BlaziconsSvgFolderPath_1x1)"
      GeneratorPath="$(BlaziconsGeneratorPath)" />

    <Message Text="Blazicons generation completed." Importance="high" />
  </Target>
</Project>
```

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