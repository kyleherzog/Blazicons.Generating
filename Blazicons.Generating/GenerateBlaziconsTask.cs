using Microsoft.Build.Framework;
using Microsoft.VisualStudio.Threading;
using MSBuildTask = Microsoft.Build.Utilities.Task;

namespace Blazicons.Generating;

/// <summary>
/// MSBuild task that generates Blazicons classes from SVG files in a repository.
/// </summary>
public class GenerateBlaziconsTask : MSBuildTask, ICancelableTask, IDisposable
{
    private readonly CancellationTokenSource cancellationTokenSource = new();
    private bool hasDisposed;

    ~GenerateBlaziconsTask()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: false);
    }

    /// <summary>
    /// Gets or sets the name of the generated class.
    /// Default: Icon
    /// </summary>
    [Required]
    public string ClassName { get; set; } = "Icon";

    /// <summary>
    /// Gets or sets the generator namespace/path structure for the generated file.
    /// If not specified, defaults to "Blazicons.Generating".
    /// </summary>
    public string? GeneratorPath { get; set; }

    /// <summary>
    /// Gets or sets the output directory path where the generated file will be written.
    /// </summary>
    [Required]
    public string? OutputPath { get; set; }

    /// <summary>
    /// Gets or sets a semicolon-delimited string of property name removal patterns.
    /// DEPRECATED: Use PropertyNameRemovalPatterns instead. This property is maintained for backwards compatibility.
    /// Supports the following pattern types:
    /// - prefix:value - Removes a literal prefix from the beginning
    /// - suffix:pattern - Removes a regex pattern from the end
    /// - pattern:regex - Removes a regex pattern anywhere
    ///
    /// Example: "prefix:Ic_Fluent_;suffix:_24_\w*$;pattern:-(original|plain)"
    /// </summary>
    public string? PropertyNameRemovalPattern { get; set; }

    /// <summary>
    /// Gets or sets a semicolon-delimited string of property name removal patterns.
    /// Supports the following pattern types:
    /// - prefix:value - Removes a literal prefix from the beginning
    /// - suffix:pattern - Removes a regex pattern from the end
    /// - pattern:regex - Removes a regex pattern anywhere
    ///
    /// Example: "prefix:Ic_Fluent_;suffix:_24_\w*$;pattern:-(original|plain)"
    /// </summary>
    public string? PropertyNameRemovalPatterns { get; set; }

    /// <summary>
    /// Gets or sets the local path to a repository containing SVG icons.
    /// Either this or RepoUrl must be specified.
    /// </summary>
    public string? RepoPath { get; set; }

    /// <summary>
    /// Gets or sets the URL of the .zip file containing the repository with SVG icons.
    /// Either this or RepoPath must be specified.
    /// </summary>
    public string? RepoUrl { get; set; }

    /// <summary>
    /// Gets or sets the relative path within the extracted repository to the SVG folder.
    /// </summary>
    public string? SvgFolderPath { get; set; }

    /// <summary>
    /// Gets or sets the regex pattern to filter SVG files in the repository.
    /// Default: \.svg$
    /// </summary>
    public string SvgPattern { get; set; } = @"\.svg$";

    /// <summary>
    /// Cancels the task execution.
    /// </summary>
    public void Cancel()
    {
        if (!hasDisposed)
        {
            cancellationTokenSource.Cancel();
        }
    }

    public void Dispose()
    {
        // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        Dispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Executes the task to generate Blazicons classes.
    /// </summary>
    /// <returns>True if successful, false otherwise.</returns>
    public override bool Execute()
    {
        var result = false;
        var taskContext = new JoinableTaskContext();
        var taskFactory = new JoinableTaskFactory(taskContext);
        taskFactory.Run(async () =>
        {
            result = await ExecuteAsync(cancellationTokenSource.Token).ConfigureAwait(true);
        });
        return result;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!hasDisposed)
        {
            if (disposing)
            {
                cancellationTokenSource.Dispose();
            }

            hasDisposed = true;
        }
    }

    /// <summary>
    /// Executes the task asynchronously to generate Blazicons classes.
    /// </summary>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if successful, false otherwise.</returns>
    private async Task<bool> ExecuteAsync(CancellationToken cancellationToken)
    {
        try
        {
            // Validate inputs
            if (string.IsNullOrWhiteSpace(RepoUrl) && string.IsNullOrWhiteSpace(RepoPath))
            {
                Log.LogError("Either RepoUrl or RepoPath must be specified.");
                return false;
            }

            if (string.IsNullOrWhiteSpace(OutputPath))
            {
                Log.LogError("OutputPath is required.");
                return false;
            }

            Log.LogMessage(MessageImportance.High, $"Generating {ClassName} icons to: {OutputPath}");

            // Handle repository download if URL is provided
            bool success;
            if (!string.IsNullOrWhiteSpace(RepoUrl))
            {
                success = await GenerateFromRepoUrlAsync(cancellationToken).ConfigureAwait(false);
            }
            else
            {
                success = GenerateFromLocalPath();
            }

            if (!success)
            {
                return false;
            }

            Log.LogMessage(MessageImportance.High, "Icon generation completed successfully.");
            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex, showStackTrace: true);
            return false;
        }
    }

    private bool GenerateFromLocalPath()
    {
        if (string.IsNullOrWhiteSpace(RepoPath))
        {
            Log.LogError("RepoPath must be specified when RepoUrl is not provided.");
            return false;
        }

        var repoPath = RepoPath!;

        Log.LogMessage(MessageImportance.Normal, $"Using local repository at: {repoPath}");

        var svgFolder = !string.IsNullOrWhiteSpace(SvgFolderPath)
            ? Path.Combine(repoPath, SvgFolderPath)
            : repoPath;

        if (!Directory.Exists(svgFolder))
        {
            Log.LogError($"SVG folder not found: {svgFolder}");
            return false;
        }

        var svgFiles = Directory.GetFiles(svgFolder, "*.svg", SearchOption.AllDirectories);
        Log.LogMessage(MessageImportance.Normal, $"Found {svgFiles.Length} SVG files.");

        GenerateIconsClass(svgFolder);
        return true;
    }

    private async Task<bool> GenerateFromRepoUrlAsync(CancellationToken cancellationToken)
    {
        RepoDownloader? downloader = null;
        try
        {
            Log.LogMessage(MessageImportance.Normal, $"Downloading repository from: {RepoUrl}");

            downloader = new RepoDownloader(new Uri(RepoUrl!));
            var files = await downloader.Download(SvgPattern).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();

            Log.LogMessage(MessageImportance.Normal, $"Downloaded {files.Count} SVG files.");

            // Build the path to the SVG folder
            string svgFolder;
            if (!string.IsNullOrWhiteSpace(SvgFolderPath))
            {
                svgFolder = Path.Combine(
                    downloader.ExtractedFolder,
                    $"{downloader.RepoName}-{downloader.BranchName}",
                    SvgFolderPath);
            }
            else
            {
                svgFolder = Path.Combine(
                    downloader.ExtractedFolder,
                    $"{downloader.RepoName}-{downloader.BranchName}");
            }

            if (!Directory.Exists(svgFolder))
            {
                Log.LogError($"SVG folder not found: {svgFolder}");
                return false;
            }

            // Generate the icons class file BEFORE cleanup
            GenerateIconsClass(svgFolder);
            return true;
        }
        catch (Exception ex)
        {
            Log.LogErrorFromException(ex);
            return false;
        }
        finally
        {
            // Clean up downloaded files
            try
            {
                downloader?.CleanUp();
            }
            catch (Exception cleanupEx)
            {
                Log.LogWarning($"Failed to clean up temporary files: {cleanupEx.Message}");
            }
        }
    }

    private void GenerateIconsClass(string svgFolder)
    {
        // Use custom generator path if specified, otherwise use default
        string generatorPath;
        if (!string.IsNullOrWhiteSpace(GeneratorPath))
        {
            // Replace forward slashes with correct path separator
            var pathParts = GeneratorPath!.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            generatorPath = Path.Combine(new[] { OutputPath! }.Concat(pathParts).ToArray());
        }
        else
        {
            // Default path structure
            generatorPath = Path.Combine(
                OutputPath!,
                "Blazicons.Generating");
        }

        var outputFilePath = Path.Combine(generatorPath, $"{ClassName}.g.cs");

        Log.LogMessage(MessageImportance.Normal, $"Generating class file: {outputFilePath}");

        // Ensure output directory exists
        Directory.CreateDirectory(generatorPath);

        // Use PropertyNameRemovalPatterns if set, otherwise fall back to PropertyNameRemovalPattern for backwards compatibility
        var patternsToUse = !string.IsNullOrWhiteSpace(PropertyNameRemovalPatterns)
            ? PropertyNameRemovalPatterns
            : PropertyNameRemovalPattern;

        // Generate the code
        BlaziconsClassGenerator.GenerateClassFile(
            outputFilePath,
            ClassName,
            svgFolder,
            propertyNameRemovalPatterns: patternsToUse);
    }
}