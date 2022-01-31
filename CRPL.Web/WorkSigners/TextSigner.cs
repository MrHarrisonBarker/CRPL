using CRPL.Data.Works;
using iTextSharp.text.pdf;
using iTextSharp.text.xml.xmp;
using iTextSharp.xmp;

namespace CRPL.Web.WorkSigners;

public class TextSigner : WorkSigner, IWorkSigner
{
    public TextSigner(string signature) : base(signature)
    {
    }

    public CachedWork Sign(CachedWork work)
    {
        using var saveStream = new MemoryStream();
        var pdfReader = new PdfReader(work.Work);

        using (PdfStamper stamper = new PdfStamper(pdfReader, saveStream))
        {
            Dictionary<String, String> info = pdfReader.Info;
            using (MemoryStream msXmp = new MemoryStream())
            {
                XmpWriter xmp = new XmpWriter(msXmp, info);
                xmp.SetProperty(XmpConst.NS_XMP, "Identifier", Signature);

                xmp.Close();
                stamper.XmpMetadata = msXmp.ToArray();
            }

            stamper.Close();
        }


        return new CachedWork
        {
            ContentType = work.ContentType,
            Work = saveStream.ToArray()
        };
    }
}