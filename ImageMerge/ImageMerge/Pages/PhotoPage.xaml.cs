using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMerge.Extensions;
using ImageMerge.Services;
using ImageMerge.Services.Abstract;
using Microsoft.ProjectOxford.Face;
using Microsoft.ProjectOxford.Face.Contract;
using Xamarin.Forms;

namespace ImageMerge.Pages
{
    public partial class PhotoPage : ContentPage
    {
        public PhotoPage()
        {
            InitializeComponent();

            if (Device.OS == TargetPlatform.iOS)
            {
                Padding = new Thickness(0, 20, 0, 0);
                BackgroundColor = Color.Black;
            }
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            var imageService = new ImageService();

            var imageResult = await imageService.GetImageOrTakePhoto();

            if (imageResult.Failure)
            {
                return;
            }

            var mediaFile = imageResult.Data;

            using (var stream = mediaFile.GetStream())
            {
                var screenWidth = (float)ImageContainer.Width;
                var byteImage = (await DependencyService.Get<IImageResizer>().ResizeImage(stream.ToByteArray(), screenWidth, 90)); ;

                var faceServiceClient = new FaceServiceClient("f7ce003bec174227ac399308e8a1575e");

                try
                {
                    var faces = await faceServiceClient.DetectAsync(new MemoryStream(byteImage.ImageBytes), true, true);

                    var face = faces.FirstOrDefault();

                    if (face == null)
                    {
                        return;
                    }

                    ThugImage.Source = ImageSource.FromStream(() => new MemoryStream(byteImage.ImageBytes));

                    await Task.Delay(1000);

                    AudioService.PlayAudio();

                    await ShowCygaro(face, byteImage);

                    await ShowOksy(face, byteImage);

                    await ShowThugLife(face, byteImage);
                }
                catch (Exception ex)
                {
                    throw;
                }
            }
        }

        private async Task ShowOksy(Face face, ImageData byteImage)
        {
            var eyeLeft = face.FaceLandmarks.PupilLeft;
            var eyeRight = face.FaceLandmarks.PupilRight;

            const int oksyCenterPoint = 161;
            var eyesCenterX = eyeLeft.X + (eyeLeft.X - eyeRight.X) / 2 - oksyCenterPoint;
            var eyesCenterY = eyeRight.Y;

            var xx = eyesCenterX * ThugImage.Width / byteImage.ImageWidth;

            var yy = ThugImage.Height - eyesCenterY * ThugImage.Height / byteImage.ImageHeight + Cygaro.Height + 50;
            Oksy.Opacity = 1;
            await TranslateScaled(Oksy, (int)xx, (int)yy);


        }

        private async Task ShowCygaro(Face face, ImageData byteImage)
        {
            var mouth = face.FaceLandmarks.MouthRight;

            var mouthX = mouth.X * ThugImage.Width / byteImage.ImageWidth;

            var mouthY = ThugImage.Height - mouth.Y * ThugImage.Height / byteImage.ImageHeight;

            Cygaro.Opacity = 1;
            await TranslateScaled(Cygaro, (int)mouthX, (int)mouthY);
        }

        private async Task ShowThugLife(Face face, ImageData byteImage)
        {
            var mouth = face.FaceRectangle;
            int y = 0;
            if (mouth.Top > byteImage.ImageHeight - (mouth.Top + mouth.Height))
            {
                y = (int)ThugImage.Height - 20;
            }
            else
            {
                y = 70 + (int)TsTekst.Height;
            }
            var mouthX = ThugImage.Width / 2 - TsTekst.Width / 2;

            y += (int)Cygaro.Height + (int)Oksy.Height;

            TsTekst.Opacity = 1;
            await TranslateScaled(TsTekst, (int)mouthX, (int)y);
        }


        private async Task TranslateScaled(Image item, int x, int y)
        {
            var cygaroX = x  /*Cygaro.Width/2*/;


            var cygaroY = y; /*+ Cygaro.Height/2*/;

            await item.TranslateTo((int)cygaroX, (int)-cygaroY, 1000u, Easing.Linear);

        }
    }
}
