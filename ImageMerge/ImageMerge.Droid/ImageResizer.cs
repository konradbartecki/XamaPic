using System;
using System.IO;
using System.Threading.Tasks;
using Android.Graphics;
using ExifLib;
using ImageMerge.Droid;
using ImageMerge.Services.Abstract;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace ImageMerge.Droid
{
    public class ImageResizer : IImageResizer
    {
        public Task<byte[]> ResizeImage(byte[] imageData, float size, int quality)
        {
            var exifReader = new ExifReader(new MemoryStream(imageData));
            ushort orientation;

            exifReader.GetTagValue(274, out orientation);

            var rotation = GetRotation(orientation);

            var options = new BitmapFactory.Options
            {
                InJustDecodeBounds = true
            };

            BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            options.InJustDecodeBounds = false;

            options.InSampleSize = GetScaleFactor(size, options);

            var resizedImage = BitmapFactory.DecodeByteArray(imageData, 0, imageData.Length, options);

            using (var ms = new MemoryStream())
            {
                var matrix = new Matrix();
                matrix.PostRotate(rotation);
                var rotatedImage = rotation == 0
                    ? Bitmap.CreateBitmap(resizedImage, 0, 0, resizedImage.Width, resizedImage.Height)
                    : Bitmap.CreateBitmap(resizedImage, 0, 0, resizedImage.Width, resizedImage.Height, matrix, true);

                rotatedImage.Compress(Bitmap.CompressFormat.Jpeg, quality, ms);
                resizedImage.Recycle();

                return Task.FromResult(ms.ToArray());
            }
        }

        private static int GetRotation(int orientation)
        {
            try
            {
                switch ((Orientation)orientation)
                {
                    case Orientation.Undefined:
                        return 0;
                    case Orientation.Normal:
                        return 0;
                    case Orientation.FliHorizontal:
                        return 0;
                    case Orientation.Rotate180:
                        return 180;
                    case Orientation.FlipVertical:
                        return 0;
                    case Orientation.Transpose:
                        return 0;
                    case Orientation.Rotate90:
                        return 90;
                    case Orientation.Transverse:
                        return 0;
                    case Orientation.Rotate270:
                        return 270;
                    default:
                        return 90;
                }
            }
            catch (Exception)
            {
                return 0;
            }
        }

        private static int GetScaleFactor(float size, BitmapFactory.Options options)
        {
            var biggerSide = options.OutHeight > options.OutWidth ? options.OutHeight : options.OutWidth;

            var scale = size / biggerSide;
            var height = (int)(options.OutHeight * scale);
            var width = (int)(options.OutWidth * scale);
            return CalculateInSampleSize(options, width, height);
        }

        private static int CalculateInSampleSize(BitmapFactory.Options options, int reqWidth, int reqHeight)
        {
            // Raw height and width of image
            int height = options.OutHeight;
            int width = options.OutWidth;
            int inSampleSize = 1;

            if (height > reqHeight || width > reqWidth)
            {
                int halfHeight = height / 2;
                int halfWidth = width / 2;

                // Calculate the largest inSampleSize value that is a power of 2 and keeps both
                // height and width larger than the requested height and width.
                while (halfHeight / inSampleSize > reqHeight && halfWidth / inSampleSize > reqWidth)
                {
                    inSampleSize *= 2;
                }
            }

            return inSampleSize;
        }

        private enum Orientation
        {
            Undefined,
            Normal,
            FliHorizontal,
            Rotate180,
            FlipVertical,
            Transpose,
            Rotate90,
            Transverse,
            Rotate270
        }
    }
}