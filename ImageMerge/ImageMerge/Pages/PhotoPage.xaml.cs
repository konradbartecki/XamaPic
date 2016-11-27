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
        private readonly ImageData _data;
        private readonly Face _face;

        public PhotoPage(ImageData data, Face face)
        {
            _data = data;
            _face = face;
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            if (Device.OS == TargetPlatform.iOS)
            {
                Padding = new Thickness(0, 20, 0, 0);
                BackgroundColor = Color.Black;
            }
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            ThugImage.Source = ImageSource.FromStream(() => new MemoryStream(_data.ImageBytes));

            await Task.Delay(1000);

            AudioService.PlayAudio();

            await ShowCygaro(_face, _data);

            await ShowOksy(_face, _data);

            await ShowThugLife(_face, _data);
        }

        private async Task ShowOksy(Face face, ImageData byteImage)
        {
            var eyeLeft = face.FaceLandmarks.PupilLeft;
            var eyeRight = face.FaceLandmarks.PupilRight;

            double eyeOffset = 0;

            Device.OnPlatform(
                () => eyeOffset = eyeRight.X - eyeLeft.X,
                () => eyeOffset = eyeLeft.X - eyeRight.X,
                () => eyeOffset = eyeRight.X - eyeLeft.X);

            const int oksyCenterPoint = 161;
            var eyesCenterX = eyeLeft.X + eyeOffset / 2 - oksyCenterPoint;
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
