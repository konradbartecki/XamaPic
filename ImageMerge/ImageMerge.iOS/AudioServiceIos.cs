using System.IO;
using System.Threading.Tasks;
using AVFoundation;
using Foundation;
using ImageMerge.iOS;
using ImageMerge.Services;
using Xamarin.Forms;

[assembly: Dependency(typeof(AudioServiceIos))]
namespace ImageMerge.iOS
{
    public class AudioServiceIos : IAudio
    {

        public async Task PlayAudioFile(string fileName)
        {
            await Task.Run(() =>
            {
                string sFilePath = NSBundle.MainBundle.PathForResource(Path.GetFileNameWithoutExtension(fileName),
                    Path.GetExtension(fileName));
                var url = NSUrl.FromString(sFilePath);
                var _player = AVAudioPlayer.FromUrl(url);
                _player.FinishedPlaying += (object sender, AVStatusEventArgs e) => {
                                                                                       _player = null;
                };
                _player.PrepareToPlay();
                _player.Play();
            });
        }
    }
}