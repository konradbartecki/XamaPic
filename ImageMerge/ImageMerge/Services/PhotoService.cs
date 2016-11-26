using System;
using System.Threading.Tasks;
using ImageMerge.Exceptions;
using Plugin.Media;
using Plugin.Media.Abstractions;

namespace ImageMerge.Services
{
    public class PhotoService
    {
        public async Task<MediaFile> TakePhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            if (!CrossMedia.Current.IsTakePhotoSupported || !CrossMedia.Current.IsCameraAvailable)
            {
                return await PickPhotoAsync();
            }
                
            var cameraResult = await CrossMedia.Current.TakePhotoAsync(
                new StoreCameraMediaOptions
                {
                    Name = Guid.NewGuid() + ".jpg",
                    Directory = "SampleImages",
                });

            if (cameraResult == null)
            {
                throw new PhotoNotTakenException();
            }

            return cameraResult;
        }

        public async Task<MediaFile> PickPhotoAsync()
        {
            await CrossMedia.Current.Initialize();

            var cameraResult = await CrossMedia.Current.PickPhotoAsync();

            if (cameraResult == null)
            {
                throw new PhotoNotTakenException();
            }

            return cameraResult;
        }
    }
}