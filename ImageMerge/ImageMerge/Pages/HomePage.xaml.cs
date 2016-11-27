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
using Xamarin.Forms;

namespace ImageMerge.Pages
{
    public partial class HomePage
    {
        public HomePage()
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

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

            ActivityIndicator.Opacity = 1;
            PhotoButton.Opacity = 0;
            PhotoButton.IsEnabled = false;

            var mediaFile = imageResult.Data;

            using (var stream = mediaFile.GetStream())
            {
                var screenWidth = (float)PhotoButton.Width;
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

                    await Application.Current.MainPage.Navigation.PushAsync(new PhotoPage(byteImage, face));
                }
                catch (Exception ex)
                {
                    throw;
                }
                finally
                {
                    ActivityIndicator.Opacity = 0;
                    PhotoButton.Opacity = 1;
                    PhotoButton.IsEnabled = true;
                }
            }
        }
    }
}
