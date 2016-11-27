using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;
using ImageProcessor.Imaging;
using ImageResizer.ExtensionMethods;
using Microsoft.ProjectOxford.Emotion.Contract;
using XamaPic.Server.Core;

namespace XamaPic.vNext.Controllers
{
    public class PhotosController : ApiController
    {
        private PhotoApi photoApi = new PhotoApi();


        [HttpPost]
        [Route("api/photos/filtered")]
        public async Task<HttpResponseMessage> GetFiltered()
        {
            var ms0 = new MemoryStream(await Request.Content.ReadAsByteArrayAsync());
            Stream croppedPhoto = await photoApi.GetCroppedPhoto(ms0);
            var ms = new MemoryStream(croppedPhoto.CopyToBytes(true));
            var ms2 = new MemoryStream(croppedPhoto.CopyToBytes(true));
            List<Emotion> emotions = await photoApi.GetEmotionInfo(ms);
            string mostValuedEmotion = photoApi.ProcessEmotions(emotions);
            var pt = Path.Combine(System.Web.HttpRuntime.BinDirectory, "Content/Imgs");
            var resultStream = photoApi.GetFilteredImageStream(ms2, emotions.First().FaceRectangle, mostValuedEmotion, pt);
            var message = new HttpResponseMessage(HttpStatusCode.OK);
            message.Content = new StreamContent(resultStream);
            message.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            return message;
        }
    }
}
