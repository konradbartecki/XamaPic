using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using ImageProcessor.Imaging;
using ImageProcessor.Plugins.Cair;
using ImageProcessor.Plugins.Cair.Imaging;
using ImageResizer;
using ImageResizer.Configuration;
using ImageResizer.ExtensionMethods;
using ImageResizer.Plugins.CropAround;
using ImageResizer.Plugins.Faces;
using ImageResizer.Resizing;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Face.Contract;

namespace XamaPic.Server.Core
{
    public class PhotoApi
    {
        private static string key = "bd2eb023ff704a97b5de3818ab80a599";
        private static string emotionsKey = "cfc095b2b2bc4e30bd7d8b3c72b310aa";
        private HttpClient _client = null;
        private HttpClient Client
        {
            get
            {
                if (_client == null)
                    _client = CreateClient();
                return _client;
            }
        }

        private HttpClient CreateClient()
        {
            var c = new HttpClient();
            c.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", key);
            return c;
        }

        public async Task Test()
        {
            var stream = File.OpenRead(@"C:\Users\KonradBartecki\Pictures\Camera Roll\5.jpg");
            Stream croppedPhoto = await GetCroppedPhoto(stream);
            var ms = new MemoryStream(croppedPhoto.CopyToBytes(true));
            var ms2 = new MemoryStream(croppedPhoto.CopyToBytes(true));
            List<Emotion> emotions = await GetEmotionInfo(ms);
            GetFilteredImage(ms2, emotions.First().FaceRectangle);
            string mostValuedEmotion = ProcessEmotions(emotions);
            //using (var fileStream = File.Create(@"C:\Users\KonradBartecki\Pictures\Camera Roll\out.jpg"))
            //{
            //    croppedPhoto.Seek(0, SeekOrigin.Begin);
            //    croppedPhoto.CopyTo(fileStream);
            //}
        }
        
        public async Task<List<Emotion>> GetEmotionInfo(Stream img)
        {
            Console.WriteLine("Reading emotions");
            var emotionClient = new Microsoft.ProjectOxford.Emotion.EmotionServiceClient(emotionsKey);
            var info = await emotionClient.RecognizeAsync(img);
            Console.WriteLine("Emotions done");
            return info.ToList();
        }

        public async Task<Stream> GetCroppedPhoto(Stream img)
        {
            Console.WriteLine("Getting cropped photo");
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.projectoxford.ai/vision/v1.0/generateThumbnail?width=400&height=400&smartCropping=true");
            request.Content = new StreamContent(img);
            request.Headers.Add("Ocp-Apim-Subscription-Key", "bd2eb023ff704a97b5de3818ab80a599");
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            var response = await Client.SendAsync(request);
            Console.WriteLine("Cropped photo done");
            return await response.Content.ReadAsStreamAsync();
        }

        public string ProcessEmotions(List<Emotion> faces)
        {
            var mostValuedEmotion = faces
                .First()
                .Scores
                .ToRankedList()
                .OrderByDescending(x => x.Value)
                .First();
            return mostValuedEmotion.Key;
        }

        public void GetFilteredImage(Stream img, Microsoft.ProjectOxford.Common.Rectangle rect)
        {
            var sadLayerStream = File.OpenRead(@"C:\Users\KonradBartecki\Pictures\Camera Roll\layer\xd.png");
            var ms = new MemoryStream(5*1024*1024);
            new ImageProcessor.ImageFactory(true)
                .Load(sadLayerStream)
                .Resize(new Size()
                {
                    Height = rect.Height,
                    Width = rect.Width
                })
                .Save(ms);
            var sadBitmap = new Bitmap(ms);
            var sadLayer = new ImageLayer()
            {
                Image = new Bitmap(ms),
                Position = new Point() {X = rect.Left, Y = rect.Top}
            };
            sadLayer.Image = sadBitmap;

            new ImageProcessor.ImageFactory(true)
                .Load(img)
                .Overlay(sadLayer)
                .Save(@"C:\Users\KonradBartecki\Pictures\Camera Roll\out2.jpg");
        }
    }
}
