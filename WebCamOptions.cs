namespace EventulaEntranceClient
{
    public class WebCamOptions
    {
        public int Width { get; set; } = 320;
        public int Height { get; set; } = 240;
        public string image_format { get; set; } = "jpeg";
        public int jpeg_quality { get; set; } = 90;
    }
}
