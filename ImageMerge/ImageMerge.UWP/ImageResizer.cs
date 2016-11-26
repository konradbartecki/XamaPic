using System;
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
        public async Task<byte[]> ResizeImage(byte[] imageData, float size, int quality)
        {
            using (var sourceStream = await ConvertToRandomAccessStream(imageData))
            {
                var decoder = await BitmapDecoder.CreateAsync(sourceStream);
                var scale = size / decoder.PixelHeight;
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
                encoder.SetPixelData(
                    BitmapPixelFormat.Rgba8,
                    BitmapAlphaMode.Premultiplied,
                    finalWidth,
                    finalHeight,
                    96,
                    96,
                    pixelData.DetachPixelData());
                await encoder.FlushAsync();

                var resizedData = new byte[outputStream.Size];
                await outputStream.ReadAsync(resizedData.AsBuffer(), (uint)outputStream.Size, InputStreamOptions.None);

                return resizedData;
            }
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