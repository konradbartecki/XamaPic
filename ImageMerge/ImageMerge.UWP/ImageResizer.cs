using System;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Graphics.Imaging;
using Windows.Storage.Streams;
using ImageMerge.Services.Abstract;
using ImageMerge.UWP;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace ImageMerge.UWP
{
    public class ImageResizer : IImageResizer
    {
        public async Task<ImageData> ResizeImage(byte[] imageData, float size, int quality)
        {
            using (var sourceStream = await ConvertToRandomAccessStream(imageData))
            {
                var decoder = await BitmapDecoder.CreateAsync(sourceStream);
                var scale = size / decoder.PixelWidth;
                var finalHeight = (uint)(decoder.PixelHeight * scale);
                var finalWidth = (uint)(decoder.PixelWidth * scale);
                var transform = new BitmapTransform
                {
                    ScaledHeight = finalHeight,
                    ScaledWidth = finalWidth
                };
                var pixelData = await decoder.GetPixelDataAsync(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Straight,
                    transform,
                    ExifOrientationMode.RespectExifOrientation,
                    ColorManagementMode.DoNotColorManage);
                
                var outputStream = new InMemoryRandomAccessStream();

                var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, outputStream);

                var pixels = pixelData.DetachPixelData();
                pixels = ConverToGrayscale(pixels);

                encoder.SetPixelData(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Premultiplied,
                    finalWidth,
                    finalHeight,
                    96,
                    96,
                    pixels);
                await encoder.FlushAsync();

                var resizedData = new byte[outputStream.Size];
                await outputStream.ReadAsync(resizedData.AsBuffer(), (uint)outputStream.Size, InputStreamOptions.None);
                

                return new ImageData(unchecked((int)finalWidth), unchecked((int)finalHeight), resizedData);
            }
        }

        private byte[] ConverToGrayscale(byte[] srcPixels)
        {
            byte[] dstPixels = new byte[srcPixels.Length];

            for (int i = 0; i < srcPixels.Length; i += 4)
            {
                double b = (double)srcPixels[i] / 255.0;
                double g = (double)srcPixels[i + 1] / 255.0;
                double r = (double)srcPixels[i + 2] / 255.0;

                byte a = srcPixels[i + 3];

                double e = (0.21 * r + 0.71 * g + 0.07 * b) * 255;
                byte f = Convert.ToByte(e);

                dstPixels[i] = f;
                dstPixels[i + 1] = f;
                dstPixels[i + 2] = f;
                dstPixels[i + 3] = a;
            }
            return dstPixels;
        }

        private static async Task<IRandomAccessStream> ConvertToRandomAccessStream(byte[] data)
        {
            var randomAccessStream = new InMemoryRandomAccessStream();
            var outputStream = randomAccessStream.GetOutputStreamAt(0);
            var dw = new DataWriter(outputStream);
            var task = Task.Factory.StartNew(() => dw.WriteBytes(data));

            await task;
            await dw.StoreAsync();
            await outputStream.FlushAsync();

            return randomAccessStream;
        }
    }

}