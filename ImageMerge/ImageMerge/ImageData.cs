namespace ImageMerge
{
    public class ImageData
    {
        public int ImageWidth { get; set; }
        public int ImageHeight { get; set; }
        
        public byte[] ImageBytes { get; set; }

        public ImageData(int imageWidth, int imageHeight, byte[] imageBytes)
        {
            ImageWidth = imageWidth;
            ImageHeight = imageHeight;
            ImageBytes = imageBytes;
        } 
    }
}