using System;
using System.Threading.Tasks;
using ImageMerge.Enums;
using Plugin.Media.Abstractions;
using Xamarin.Forms;

namespace ImageMerge.Services
{
    public class ImageService
    {
        //todo to change MediaFile -> byte[] -> Unwrap image (source)
        public async Task<Result<MediaFile, ImageServiceError>> GetImageOrTakePhoto()
        {
            var decision = await ShowImageChooser();

            if (decision == Resources.Translations.Library)
            {
                return await TryGetImageData(async () => await new PhotoService().PickPhotoAsync());
            }

            if (decision == Resources.Translations.Camera)
            {
                return await TryGetImageData(async () => await new PhotoService().TakePhotoAsync());
            }

            return new Result<MediaFile, ImageServiceError>(ImageServiceError.CouldNotGetImage);
        }

        private async Task<Result<MediaFile, ImageServiceError>> TryGetImageData(Func<Task<MediaFile>> getPhoto)
        {
            try
            {
                var image = await getPhoto();

                return new Result<MediaFile, ImageServiceError>(image);
            }
            catch (Exception)
            {
                return new Result<MediaFile, ImageServiceError>(ImageServiceError.CouldNotGetImage);
            }
        }

        private async Task<string> ShowImageChooser()
        {
            return
                await
                    Application.Current.MainPage.DisplayActionSheet(
                        Resources.Translations.SelectImageSource,
                        Resources.Translations.Cancel,
                        null,
                        Resources.Translations.Library,
                        Resources.Translations.Camera);
        }
    }
}
