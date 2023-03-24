using EventulaEntranceClient.Services.Interfaces;

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
                    if (result != null)
                    {
                        return result.Text;
                    }
                }               
            }
        }
        catch(Exception ex)
        {
            _Logger.LogError(ex, "Exception parsing barcode");
        }

        return string.Empty;
    }
}
