namespace Blazicons.Generating.IntegrationTests.RepoDownloaderTests;

[TestClass]
public class DownloadShould
{
    [TestMethod]
    [DataRow("https://github.com/Templarian/MaterialDesign-SVG/archive/refs/heads/master.zip")]
    public async Task DownloadAndExtractAllFilesGivenValidUrl(string url)
    {
        var downloader = new RepoDownloader(new Uri(url));

        var downloaded = await downloader.Download();

        Assert.IsNotEmpty(downloaded, "No files were downloaded.");
        Assert.IsTrue(downloaded.All(x => File.Exists(x)), "Not all files exist locally.");

        downloader.CleanUp();
    }
}