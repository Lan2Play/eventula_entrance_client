using EventulaEntranceClient.Services.Interfaces;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
// using ZXing.Windows.Compatibility;
// using ZXing;
using ZXing.ImageSharp;

namespace EventulaEntranceClient.Services;

public class ZXingBarcodeService : IBarcodeService
{
    private readonly BarcodeReader<Rgb24> _BarcodeReader = new BarcodeReader<Rgb24>();
    private readonly ILogger<ZXingBarcodeService> _Logger;

    public ZXingBarcodeService(ILogger<ZXingBarcodeService> logger)
    {
        _Logger = logger;
    }

    public string BarcodeTextFromImage(byte[] image)
    {
        try
        {
            using (var ms = new MemoryStream(image))
            {
                using (var img = (Image<Rgb24>)Image.Load(ms)) 
                {
                    var result = _BarcodeReader.Decode(img);
                    // do something with the result
                    if (result != null)
                    {
                        return result.Text;
                    }
                }

                // var img = Image<Rgba32>.(ms);


               
            }
        }
        catch(Exception ex)
        {
            _Logger.LogError(ex, "Exception parsing barcode");
        }

        return string.Empty;
    }
}
