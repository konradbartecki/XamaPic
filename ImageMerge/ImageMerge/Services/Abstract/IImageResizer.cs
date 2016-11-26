using System.Threading.Tasks;

namespace ImageMerge.Services.Abstract
{
    public interface IImageResizer
    {
        Task<byte[]> ResizeImage(byte[] imageData, float size, int quality);
    }
}