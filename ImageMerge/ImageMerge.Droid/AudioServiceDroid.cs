using System.Threading.Tasks;
using Android.Media;
using ImageMerge.Droid;
using ImageMerge.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioServiceDroid))]
namespace ImageMerge.Droid
{
    public class AudioServiceDroid : IAudio
    {
        public async Task PlayAudioFile(string fileName)
        {
            await Task.Run(() =>
            {
                var player = new MediaPlayer();
                var fd = global::Android.App.Application.Context.Assets.OpenFd(fileName);
                player.Prepared += (s, e) =>
                {
                    player.Start();
                };
                player.SetDataSource(fd.FileDescriptor, fd.StartOffset, fd.Length);
                player.Prepare();
            });
        }
    }
}