using System.Threading.Tasks;

namespace ImageMerge.Services.Abstract
{
    public interface IImageResizer
    {
        Task<ImageData> ResizeImage(byte[] imageData, float size, int quality);
    }
}