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
    <BlaziconsPropertyNameCleanupPattern>-(original|plain|line)</BlaziconsPropertyNameCleanupPattern>
    <BlaziconsGeneratedCodeOutputPath>Generated</BlaziconsGeneratedCodeOutputPath>
    <BlaziconsGeneratorPath>MyIcons.Generating/MyIcons.Generating.MyIconsGenerator</BlaziconsGeneratorPath>
  </PropertyGroup>
</Project>
```

**Property Name Cleanup Example:**  
When `BlaziconsPropertyNameCleanupPattern` is set to `-(original|plain|line)`, file names are transformed as follows:
- `react-plain.svg` → `React` (instead of `ReactPlain`)
- `angular-original.svg` → `Angular` (instead of `AngularOriginal`)
- `vue-line.svg` → `Vue` (instead of `VueLine`)

### Configuration Properties

| Property | Required | Description | Example |
|----------|----------|-------------|---------|
| `BlaziconsRepoUrl` | Yes* | URL to a .zip file containing the icon repository | `https://github.com/ionic-team/ionicons/archive/refs/heads/main.zip` |
| `BlaziconsRepoPath` | Yes* | Local path to a repository (alternative to RepoUrl) | `C:\Source\icons` |
| `BlaziconsSvgPattern` | Yes | Regex pattern to filter SVG files | `^src\/svg\/.*.svg$` |
| `BlaziconsClassName` | Yes | Name of the generated icon class | `Ionicon` |
| `BlaziconsSvgFolderPath` | No | Relative path within the repo to the SVG folder | `src/svg` |
| `BlaziconsPropertyNameRemovalPattern` | No | Regex pattern to remove from file names when generating property names | `-(original\|plain\|line)` |
| `BlaziconsGeneratedCodeOutputPath` | Yes | Output directory for generated code | `Generated` |
| `BlaziconsGeneratorPath` | No | Generator namespace/path structure for the generated file | `Blazicons.Ionicons.Generating/Blazicons.Ionicons.Generating.IoniconsGenerator` |

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