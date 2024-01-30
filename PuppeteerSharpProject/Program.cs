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

              var headerContent = File.ReadAllText($@"C:\UserFiles\HeaderFooters\FSP_header.html");
              var footerContent =  File.ReadAllText($@"C:\UserFiles\HeaderFooters\FSP_footer.html");

                  // Get the directory path without the file name
            string directoryPath = Path.GetDirectoryName($@"C:\UserFiles\HeaderFooters\FSP_footer.html");
          // Get the parent directory path
            string parentDirectoryPath = Directory.GetParent(directoryPath).FullName;

            // Read the local image file as base64
            var logoImgPath = parentDirectoryPath+ @"\common\images\logo3.jpg";
            var logoImgPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgPath));

            var logoFbPath = parentDirectoryPath+ @"\common\images\fb_foot.png";
            var logoFbPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoFbPath));

            var logoImgInstaPath = parentDirectoryPath+ @"\common\images\insta_foot.png";
            var logoImgInstaPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgInstaPath));

            var logoImgTwitterPath = parentDirectoryPath+ @"\common\images\twitter_foot.png";
            var logoImgTwitterPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgTwitterPath));

            var logoImgInPath = parentDirectoryPath+ @"\common\images\in_foot.png";
            var logoImgInBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgInPath));

            var logoImgYouPath = parentDirectoryPath+ @"\common\images\you_foot.png";
            var logoImgYouPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgYouPath));

            var logoImgTiktokPath = parentDirectoryPath+ @"\common\images\ticktok_foot.png";
            var logoImgTiktokPathBase64 = Convert.ToBase64String(File.ReadAllBytes(logoImgTiktokPath));

            headerContent = headerContent.Replace("{{logoImgPath}}", logoImgPathBase64);
            footerContent= footerContent.Replace("{{logoImgFbPath}}", logoFbPathBase64);
            footerContent= footerContent.Replace("{{logoImgInstaPath}}", logoImgInstaPathBase64);
            footerContent=footerContent.Replace("{{logoImgTwitterPath}}", logoImgTwitterPathBase64);
            footerContent = footerContent.Replace("{{logoImgInPath}}", logoImgInBase64);
            footerContent = footerContent.Replace("{{logoImgYouPath}}", logoImgYouPathBase64);
            footerContent = footerContent.Replace("{{logoImgTiktokPath}}", logoImgTiktokPathBase64);

           //footerContent += "<span class='pageNumber'></span> of<span class='totalPages'></span>";
           footerContent = footerContent.Replace("{{PageNumber}}", "<span class='pageNumber'></span>/<span class='totalPages'></span>");

            //   var htmlContent = @"
            //                     <html>
            //                     <head>
            //                         <style>
            //                             body {
            //                                 margin: 0;
            //                                 padding: 0;
            //                             }
            //                             .footer {
            //                                 -webkit-print-color-adjust: exact;
            //                                 display: flex;
            //                                 justify-content: space-between;
            //                                 background-color: blue;
            //                                 padding: 10px;
            //                                 width:100%;
            //                                 font-size:10px;

            //                             }
            //                         </style>
            //                     </head>
            //                     <body>
            //                         <div class='footer'>
            //                             <div>Left content</div>
            //                             <div>Right content</div>
            //                         </div>
            //                     </body>
            //                     </html>";

                               
 //var htmlContent="<div style=' font-size: 28px;  width:100%; -webkit-print-color-adjust: exact;height:180px; min-height: 69px;overflow: auto;clear: both;border-bottom: 1px solid #ddd;background: red;'> Footer</div>";
                  
        
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
            pdfOptions.MarginOptions = new PuppeteerSharp.Media.MarginOptions() { Bottom = "200px", Left = "100px", Right = "100px", Top = "100px" };
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
                      Headless = true
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

            
            




            
             await page.GoToAsync($@"C:\UserFiles\Statements\26detailed transaction_26detailedTransaction\Batch 1 of 26detailed transaction_26detailedTransaction\10021\Statement_10021_10039_27_01_2024_01_51_58.html");
        
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