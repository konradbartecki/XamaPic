using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Xamarin.Forms;
using XLabs.Platform.Services.Media;
using CameraDevice = Plugin.Media.Abstractions.CameraDevice;

namespace XamaPic.Mobile
{
    public partial class MainPage : ContentPage
    {
        private static MobileClient client = new MobileClient();
        public MainPage()
        {
            InitializeComponent();
        }

        private async void Button_OnClicked(object sender, EventArgs e)
        {
            try
            {
                StatusLabel.Text = "Status: Taking photo...";
                var device = FrontCameraSwitch.IsToggled ? CameraDevice.Front : CameraDevice.Rear;
                var photo = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions()
                {
                    DefaultCamera = device,
                    AllowCropping = false

                });
                //Img.Source = ImageSource.FromStream(() => photo.GetStream());
                StatusLabel.Text = "Status: Communicating with backend";
                var newPhoto = await client.GetFilteredImage(photo.GetStream());
                Img.Source = ImageSource.FromStream(() => newPhoto);
                StatusLabel.Text = $"Status: OK {DateTime.Now:T}";
            }
            catch (Exception exception)
            {
                StatusLabel.Text = $"Status: {exception.GetType()} - {exception.Message}";
            }

        }
    }
}
