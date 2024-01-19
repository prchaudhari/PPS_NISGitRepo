using PuppeteerSharp;

namespace PuppeteerSharpProject
{
    class Program
    {
       


 

        static async Task Main(string[] args)
        {
 
                 //  var html = File.ReadAllText("./invoice.html");
              //  var htmlPath = "./invoice2/7/Statement_7_19_12_01_2024_20_20_55.html";
                  //   var pdfOptions = new PuppeteerSharp.PdfOptions();

              var headerContent = File.ReadAllText($@"C:\UserFiles\HeaderFooters\PPS_header.html");
              var footerContent =  File.ReadAllText($@"C:\UserFiles\HeaderFooters\PPS_footer.html");

                  
        
//     pdfOptions.DisplayHeaderFooter=true;
//   //  pdfOptions.HeaderTemplate = "<span style='font-size: 30px; width: 200px; height: 50px; background-color: black; color: white; margin: 20px;'>Header 1</span>"; //é¡µçææ¬

//    pdfOptions.HeaderTemplate = "<span style='font-size: 30px; width: 200px; height: 50px; background-color: black; color: white; margin: 20px;'>Header 1</span>"; //é¡µçææ¬


//     pdfOptions.Landscape = false; //çº¸å¼ æ¹å false-åç´ true-æ°´å¹³
//     pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "100px", Left = "20px", Right = "20px", Top = "100px" }; //çº¸å¼ è¾¹è·ï¼éè¦è®¾ç½®å¸¦åä½çå¼ï¼é»è®¤å¼æ¯None
//     pdfOptions.Scale = 1m; //PDFç¼©æ¾ï¼ä»0-1
//  pdfOptions.FooterTemplate = "<span style='font-size: 30px; width: 200px; height: 50px; background-color: black; color: white; margin: 20px;'>footer 1</span>"; //é¡µçææ¬

            var pdfOptions = new PuppeteerSharp.PdfOptions();
            pdfOptions.PrintBackground = true;
            pdfOptions.DisplayHeaderFooter = true;
            pdfOptions.HeaderTemplate = headerContent; 
            pdfOptions.Landscape = false; 
            pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "100px", Left = "10px", Right = "10px", Top = "100px" };
            pdfOptions.Scale = 1m; 
            pdfOptions.FooterTemplate = footerContent; 
            pdfOptions.PreferCSSPageSize = true;
            pdfOptions.Format = PuppeteerSharp.Media.PaperFormat.A4;
             



     


        // using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions
        //              {
        //                 Headless = true,
        //                 ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
        //             }))
        //         {
        //             using (var page = await browser.NewPageAsync())
        //             {
        //                 await page.SetContentAsync(html);
        //                await page.PdfAsync("invoice2.pdf", pdfOptions);
        //                //await page.PdfAsync("invoice2.html", pdfOptions);
        //             }
        //         }


        var browser = await PuppeteerSharp.Puppeteer.LaunchAsync(new LaunchOptions
                     {
                      Headless = false,
                      ExecutablePath = "C:\\Program Files\\Google\\Chrome\\Application\\chrome.exe"
                });

                var page = await browser.NewPageAsync(); //æå¼ä¸ä¸ªæ°æ ç­¾
                                                         // await page.SetContentAsync(html);
            
            
            //      //Add script tags to the page
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\jquery.min.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\popper.min.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\bootstrap.min.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\highcharts.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\jquery.min.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\series-label.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\exporting.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\export-data.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\accessibility.js" });
            // await page.AddScriptTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\js\\script.js" });
            // // Generate PDF from the page with specified options
            // // Add style tags to the page
            // await page.AddStyleTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\css\\bootstrap.min.css" });
            // await page.AddStyleTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\css\\font-awesome.min.css" });
            // await page.AddStyleTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\css\\site.css" });
            // await page.AddStyleTagAsync(new AddTagOptions { Url = "C:\\UserFiles\\common\\css\\ltr.css" });

            
            




            
             await page.GoToAsync("E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\invoice2\\7\\Statement_7_19_12_01_2024_20_20_55.html");
        
            // Add style tags to the page
        //     await page.AddStyleTagAsync("./common/css/bootstrap.min.css" );
        //     await page.AddStyleTagAsync("./common/css/font-awesome.min.css" );
        //     await page.AddStyleTagAsync("./common/css/site.css" );
        //     await page.AddStyleTagAsync("./common/css/ltr.css" );
        // await page.AddScriptTagAsync("./common/js/jquery.min.js" );
        //     await page.AddScriptTagAsync("./common/js/popper.min.js" );
        //     await page.AddScriptTagAsync("./common/js/bootstrap.min.js" );
        //     await page.AddScriptTagAsync("./common/js/highcharts.js" );
        //     await page.AddScriptTagAsync("./common/js/jquery.min.js" );
        //     await page.AddScriptTagAsync("./common/js/series-label.js" );
        //     await page.AddScriptTagAsync("./common/js/exporting.js" );
        //     await page.AddScriptTagAsync("./common/js/export-data.js" );
        //     await page.AddScriptTagAsync("./common/js/accessibility.js" );
        //     await page.AddScriptTagAsync("./common/js/script.js" );
            // Generate PDF from the page with specified options


         
 


                 //      //Add script tags to the page
            await page.AddScriptTagAsync(new AddTagOptions { Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\jquery.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\popper.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions { Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\bootstrap.min.js" });
            await page.AddScriptTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\highcharts.js" });
            await page.AddScriptTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\series-label.js" });
            await page.AddScriptTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\exporting.js" });
            await page.AddScriptTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\export-data.js" });
            await page.AddScriptTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\accessibility.js" });
            await page.AddScriptTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\js\\script.js" });
            // Generate PDF from the page with specified options
            // Add style tags to the page
            await page.AddStyleTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\css\\bootstrap.min.css" });
            await page.AddStyleTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\css\\font-awesome.min.css" });
            await page.AddStyleTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\css\\site.css" });
            await page.AddStyleTagAsync(new AddTagOptions {Path = "E:\\nis_pps\\phase3\\PuppeteerSharpProject\\bin\\Debug\\net8.0\\common\\css\\ltr.css" });
           // Generate PDF from the page with specified options
         
         await page.PdfAsync("invoice2.pdf", pdfOptions);

//                   using var browserFetcher = new BrowserFetcher();
//  await browserFetcher.DownloadAsync();
// var browser = await Puppeteer.LaunchAsync(new LaunchOptions
// {
//     Headless = true
// });
// var page = await browser.NewPageAsync();
// await page.GoToAsync("http://www.google.com");
// await page.PdfAsync("invoice2.pdf");

        }             


    }
    }