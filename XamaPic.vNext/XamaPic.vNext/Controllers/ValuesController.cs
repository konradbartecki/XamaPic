using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using ImageProcessor.Imaging;

namespace XamaPic.vNext.Controllers
{
    public class PhotosController : ApiController
    {
        // GET api/values
        //[HttpPost]
        //public async Task<HttpResponseMessage> RetrievePhotoWithFilter(StreamContent content)
        //{




        //    var imageFactory = new ImageProcessor.ImageFactory(true);
        //    imageFactory.Load(await content.ReadAsStreamAsync())
        //        .Watermark(GetWatermark())
        //        .Crop(
        //}


        private TextLayer GetWatermark()
        {
            return new TextLayer()
            {
                FontColor = Color.DeepSkyBlue,
                FontSize = 12,
                Text = "XamaPic"
            };
        }
    }
}
