// See https://aka.ms/new-console-template for more information
using WkHtmlToPdfDotNet;

using var converter = new SynchronizedConverter(new PdfTools());

var doc = new HtmlToPdfDocument()
{
    GlobalSettings = {
        ColorMode = ColorMode.Color,
        Orientation = Orientation.Portrait,
        PaperSize = PaperKind.A4,
        Margins = new MarginSettings() { Top = 10 },
        Out = @"wkhtmltopdf.pdf",
    },
    Objects = {
        new ObjectSettings()
        {
            Page = @"data\index.html",
            LoadSettings = new LoadSettings(){
                BlockLocalFileAccess=false,
            }
        },
         new ObjectSettings()
        {
            Page = @"data\2.html",
            LoadSettings = new LoadSettings(){
                BlockLocalFileAccess=false,
            }
        },
          new ObjectSettings()
        {
            //Page = @"data\index.html",
            HtmlContent = "<h1>HtmlContent</h1><image src= \"https://opengraph.githubassets.com/0b05666ace86999efbf24f4bcbe6c855dca1bf3551a2dc0338d5d8c9665398e6/jhonnymertz/java-wkhtmltopdf-wrapper/issues/26\"/>",
            LoadSettings = new LoadSettings(){
                BlockLocalFileAccess=false,
            }
        },
    }
};

converter.Convert(doc);