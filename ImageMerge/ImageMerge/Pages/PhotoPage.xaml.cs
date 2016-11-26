using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ImageMerge.Extensions;
using ImageMerge.Services;
using ImageMerge.Services.Abstract;
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
                var screenWidth = (float) ImageContainer.Width;
                var byteArray =  await DependencyService.Get<IImageResizer>().ResizeImage(stream.ToByteArray(), screenWidth, 90);


                ThugImage.Source = ImageSource.FromStream(() => new MemoryStream(byteArray));
            }
        }
    }
}
