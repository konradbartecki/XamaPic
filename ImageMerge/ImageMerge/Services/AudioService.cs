using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageMerge.Services
{
    public interface IAudio
    {
        Task PlayAudioFile(string fileName);
    }
    public static class AudioService
    {
        public static async Task PlayAudio()
        {
            await DependencyService.Get<IAudio>().PlayAudioFile("thuglife.mp3");
        }
    }
}