using PuppeteerSharp;
 
public class PdfGenerator

{

    public async Task GeneratePdfWithCustomHeaderFooter(string htmlContent, string outputPdfPath)

    {

        // Launch a headless Chromium browser

        await new BrowserFetcher().DownloadAsync(BrowserFetcher.DefaultRevision);

        using var browser = await Puppeteer.LaunchAsync(new LaunchOptions

        {

            Headless = true

        });

        // Create a new page

        using var page = await browser.NewPageAsync();

        // Set content and format options

        await page.SetContentAsync(htmlContent);

        await page.PdfAsync(new PdfOptions

        {

            Path = outputPdfPath,

            Format = PaperFormat.A4,

            MarginOptions = new MarginOptions

            {

                Top = "2cm",    // Adjust top margin as needed

                Bottom = "2cm", // Adjust bottom margin as needed

            },

            HeaderTemplate = "<div style='text-align: center; font-size: 10px;'>Header - Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>",

            FooterTemplate = "<div style='text-align: center; font-size: 10px;'>Footer - Page <span class='pageNumber'></span> of <span class='totalPages'></span></div>"

        });

    }

}
