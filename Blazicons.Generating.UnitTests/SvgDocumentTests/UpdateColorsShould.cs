using Blazicons.Generating.Internals;

namespace Blazicons.Generating.UnitTests.SvgDocumentTests;

[TestClass]
public class UpdateColorsShould : VerifyBase
{
    [TestMethod]
    public Task NotUpdateGivenDifferentFillAndStroke()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='#000000' stroke='#ffffff'><path d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task NotUpdateGivenFillNone()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='#000'><path fill='none' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task NotUpdateGivenMultipleFillColors()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='#000000'><path fill='#FFFFFF' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenColor()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path color='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenFill()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path fill='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenFillWithAncestorColorAttribute()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><g color='#000000'><path fill='none' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/><path fill='#60be86' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/></g></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenMultipleMatchingFillColors()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path fill='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/><path fill='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenMultipleMatchingStrokeColors()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path stroke='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/><path stroke='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenNoColor()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenRootColorSet()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' color='#000000'><path d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenRootFillColor()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' fill='#000000'><path d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenRootStrokeColor()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24' stroke='#000000'><path d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'></path></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }

    [TestMethod]
    public Task UpdateGivenStroke()
    {
        var svg = "<svg xmlns='http://www.w3.org/2000/svg' viewBox='0 0 24 24'><path stroke='#000000' d='M12 2C6.48 2 2 6.48 2 12s4.48 10 10 10 10-4.48 10-10S17.52 2 12 2zm0 18c-4.41 0-8-3.59-8-8s3.59-8 8-8 8 3.59 8 8-3.59 8-8 8z'/></svg>";
        var doc = new SvgDocument(svg);

        doc.UpdateColors();
        var output = doc.Document.DocumentNode.OuterHtml;
        return Verify(output);
    }
}