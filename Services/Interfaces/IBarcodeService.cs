namespace EventulaEntranceClient.Services.Interfaces
{
    public interface IBarcodeService
    {
       string BarcodeTextFromImage(byte[] image);
    }
}
