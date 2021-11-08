using EventulaEntranceClient.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using ZXing;

namespace EventulaEntranceClient.Services
{
    public class ZXingBarcodeService : IBarcodeService
    {
        private readonly BarcodeReader _BarcodeReader = new BarcodeReader();
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
                    var img = (Bitmap)Image.FromStream(ms);

                    img.Save(@$"C:\temp\imgs\{Path.GetFileNameWithoutExtension(Path.GetTempFileName())}.bmp", ImageFormat.Bmp);

                    var result = _BarcodeReader.Decode(img);
                    // do something with the result
                    if (result != null)
                    {
                        return result.Text;
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
}
