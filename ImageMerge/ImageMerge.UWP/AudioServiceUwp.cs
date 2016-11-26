using System;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;
using ImageMerge.Services;
using ImageMerge.UWP;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioServiceUwp))]
namespace ImageMerge.UWP
{
    public class AudioServiceUwp : IAudio
    {
        public async Task PlayAudioFile(string fileName)
        {
            var element = new MediaElement();
            var folder = await Windows.ApplicationModel.Package.Current.InstalledLocation.GetFolderAsync("Assets");
            var file = await folder.GetFileAsync(fileName);
            var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
            element.SetSource(stream, "");
            element.Play();
        }
    }
}