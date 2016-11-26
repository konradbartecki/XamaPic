using System;
using System.Threading.Tasks;
using CoreGraphics;
using ImageMerge.iOS;
using ImageMerge.Services.Abstract;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(ImageResizer))]
namespace ImageMerge.iOS
{
    public class ImageResizer : IImageResizer
    {
        public Task<byte[]> ResizeImage(byte[] imageData, float size, int quality)
        {
            var originalImage = ImageFromByteArray(imageData);
            var orientation = originalImage.Orientation;

            var scale = size / originalImage.Size.Width;

            var height = (int)(originalImage.Size.Height * scale);
            var width = (int)(originalImage.Size.Width * scale);

            // image rotation when in portrait mode
            if (orientation == UIImageOrientation.Right || orientation == UIImageOrientation.Left)
            {
                var temp = height;
                height = width;
                width = temp;
            }

            using (
                var context = new CGBitmapContext(
                    IntPtr.Zero,
                    width,
                    height,
                    8,
                    4 * width,
                    CGColorSpace.CreateDeviceRGB(),
                    CGImageAlphaInfo.PremultipliedFirst))
            {
                var imageRect = new CGRect(0, 0, width, height);

                // draw the image
                context.DrawImage(imageRect, originalImage.CGImage);

                var resizedImage = UIImage.FromImage(context.ToImage(), 0, orientation);

                var scaleAndRotateImage = ScaleAndRotateImage(resizedImage, orientation);

                // save the image as a jpeg
                return Task.FromResult(scaleAndRotateImage.AsJPEG(quality / 100.0f).ToArray());
            }
        }

        private static UIImage ImageFromByteArray(byte[] data)
        {
            if (data == null)
            {
                return null;
            }

            UIImage image;

            try
            {
                image = new UIImage(Foundation.NSData.FromArray(data));
            }
            catch (Exception)
            {
                return null;
            }

            return image;
        }

        private UIImage ScaleAndRotateImage(UIImage imageIn, UIImageOrientation orIn)
        {
            int kMaxResolution = 2048;

            CGImage imgRef = imageIn.CGImage;
            float width = imgRef.Width;
            float height = imgRef.Height;
            CGAffineTransform transform = CGAffineTransform.MakeIdentity();
            var bounds = new CGRect(0, 0, width, height);

            if (width > kMaxResolution || height > kMaxResolution)
            {
                float ratio = width / height;

                if (ratio > 1)
                {
                    bounds.Width = kMaxResolution;
                    bounds.Height = bounds.Width / ratio;
                }
                else
                {
                    bounds.Height = kMaxResolution;
                    bounds.Width = bounds.Height * ratio;
                }
            }

            var scaleRatio = bounds.Width / width;
            var imageSize = new CGSize(width, height);
            var orient = orIn;
            nfloat boundHeight;

            switch (orient)
            {
                case UIImageOrientation.Up: //EXIF = 1
                    transform = CGAffineTransform.MakeIdentity();
                    break;

                case UIImageOrientation.UpMirrored: //EXIF = 2
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, 0f);
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    break;

                case UIImageOrientation.Down: //EXIF = 3
                    transform = CGAffineTransform.MakeTranslation(imageSize.Width, imageSize.Height);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI);
                    break;

                case UIImageOrientation.DownMirrored: //EXIF = 4
                    transform = CGAffineTransform.MakeTranslation(0f, imageSize.Height);
                    transform = CGAffineTransform.MakeScale(1.0f, -1.0f);
                    break;

                case UIImageOrientation.LeftMirrored: //EXIF = 5
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, imageSize.Width);
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Left: //EXIF = 6
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(0.0f, imageSize.Width);
                    transform = CGAffineTransform.Rotate(transform, 3.0f * (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.RightMirrored: //EXIF = 7
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeScale(-1.0f, 1.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                case UIImageOrientation.Right: //EXIF = 8
                    boundHeight = bounds.Height;
                    bounds.Height = bounds.Width;
                    bounds.Width = boundHeight;
                    transform = CGAffineTransform.MakeTranslation(imageSize.Height, 0.0f);
                    transform = CGAffineTransform.Rotate(transform, (float)Math.PI / 2.0f);
                    break;

                default:
                    throw new Exception("Invalid image orientation");
            }

            UIGraphics.BeginImageContext(bounds.Size);

            CGContext context = UIGraphics.GetCurrentContext();

            if (orient == UIImageOrientation.Right || orient == UIImageOrientation.Left)
            {
                context.ScaleCTM(-scaleRatio, scaleRatio);
                context.TranslateCTM(-height, 0);
            }
            else
            {
                context.ScaleCTM(scaleRatio, -scaleRatio);
                context.TranslateCTM(0, -height);
            }

            context.ConcatCTM(transform);
            context.DrawImage(new CGRect(0, 0, width, height), imgRef);

            UIImage imageCopy = UIGraphics.GetImageFromCurrentImageContext();
            UIGraphics.EndImageContext();

            return imageCopy;
        }
    }
}