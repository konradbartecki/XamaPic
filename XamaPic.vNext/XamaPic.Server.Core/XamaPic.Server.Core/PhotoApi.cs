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
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Plugins.Cair;
using ImageProcessor.Plugins.Cair.Imaging;
using ImageProcessor.Processors;
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
        private static Random random = new Random();
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

        //public async Task Test()
        //{
        //    var stream = File.OpenRead(@"C:\Users\KonradBartecki\Pictures\Camera Roll\4.jpg");
        //    Stream croppedPhoto = await GetCroppedPhoto(stream);
        //    var ms = new MemoryStream(croppedPhoto.CopyToBytes(true));
        //    var ms2 = new MemoryStream(croppedPhoto.CopyToBytes(true));
        //    List<Emotion> emotions = await GetEmotionInfo(ms);
        //    string mostValuedEmotion = ProcessEmotions(emotions);
        //    GetFilteredImage(ms2, emotions.First().FaceRectangle, mostValuedEmotion);
        //}
        
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
            var request = new HttpRequestMessage(HttpMethod.Post, "https://api.projectoxford.ai/vision/v1.0/generateThumbnail?width=300&height=300&smartCropping=true");
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

        //public void GetFilteredImage(Stream img, Microsoft.ProjectOxford.Common.Rectangle rect, string emotion)
        //{
        //    new ImageProcessor.ImageFactory(true)
        //        .Load(img)
        //        .GetOverlayFilter(emotion, rect)
        //        .Save(@"C:\Users\KonradBartecki\Pictures\Camera Roll\out2.jpg");
        //}

        public Stream GetFilteredImageStream(Stream img, Microsoft.ProjectOxford.Common.Rectangle rect, string emotion, string fullRootPath)
        {
            var ms = new MemoryStream(5*1024*1024);
            new ImageProcessor.ImageFactory(true)
                .Load(img)
                .GetOverlayFilter(emotion, rect, fullRootPath)
                .Save(ms);
            return ms;
        }
    }

    public static class ImageProcessorExtensions
    {
        private static Random random = new Random();
        private static Dictionary<string, int> EmotionToMaxTypeDict = new Dictionary<string, int>
        {
            {"Neutral", 4},
            {"Anger", 3},
            {"Disgust", 1},
            {"Fear",  0},
            {"Happiness", 3},
            {"Sadness", 2}           
        };

        private static Dictionary<string, string> UnknownEmotionsMapDict = new Dictionary<string, string>
        {
            {"Contempt", "Neutral"},
            {"Surprise", "Neutral"},
            {"Disgust", "Neutral"}
        };
        public static ImageFactory GetOverlayFilter(this ImageFactory imageFactory, string emotion, Microsoft.ProjectOxford.Common.Rectangle rect, string fullPathRoot)
        {
            if (UnknownEmotionsMapDict.ContainsKey(emotion))
                emotion = UnknownEmotionsMapDict[emotion];
            int maxOverlayType = EmotionToMaxTypeDict[emotion] + 1;
           
            int overlayType = random.Next(1, maxOverlayType);
            string filename = $"{emotion}{overlayType}.png";
            filename = Path.Combine(fullPathRoot, filename);
            if (emotion == "Neutral")
            {
                if (overlayType == 2)
                {
                    //thug life
                    imageFactory = imageFactory
                        .OverlayFullImage(filename)
                        .Saturation(0);
                    return imageFactory;
                }
            }
            if (emotion == "Sadness")
            {
                if (overlayType == 2)
                {
                    imageFactory = imageFactory.OverlayFullImage(filename);
                    return imageFactory;                   
                }
            }
            //if (emotion == "Anger")
            //{
            //    if (overlayType = 2)
            //    {
                    
            //    }
            //}

            imageFactory = imageFactory.OverlayFilterOnFace(filename, rect);

            return imageFactory;
        }

        static ImageFactory OverlayFullImage(this ImageFactory imageFactory, string filename)
        {
            var sadLayerStream = File.OpenRead(filename);
            var sadBitmap = new Bitmap(sadLayerStream);
            var sadLayer = new ImageLayer()
            {
                Image = sadBitmap
            };
            imageFactory.Overlay(sadLayer);
            return imageFactory;

        }

        static ImageFactory OverlayFilterOnFace(this ImageFactory imageFactory, string filename, Microsoft.ProjectOxford.Common.Rectangle rect)
        {
            var sadLayerStream = File.OpenRead(filename);
            var ms = new MemoryStream(5 * 1024 * 1024);
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
                Position = new Point() { X = rect.Left, Y = rect.Top }
            };
            sadLayer.Image = sadBitmap;
            imageFactory.Overlay(sadLayer);
            return imageFactory;
        }
    }
}
