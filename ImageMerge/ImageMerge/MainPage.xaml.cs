using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace ImageMerge
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();

            //PropertyChanged += MainPage_PropertyChanged;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();


          /*  cont.HeightRequest = Height;
            cont.WidthRequest = WidthRequest;*/

           


//            await cygaro.TranslateTo((int)oksy, (int)cygaroY, 1000u, Easing.Linear);


        }

        protected override async void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width <= 0 && height <= 0)
                return;
            var d = duda.Width;

            cont.WidthRequest = width;
//            cont.HeightRequest = height



            await CygaroY(cygaro, 207, 230);
            await CygaroY(oksy, 100, 116);
        }

        private async Task<double> CygaroY(StackLayout item, int x, int y)
        {
            var cygaroX = x;


            var cygaroY = y;

            await item.TranslateTo((int) cygaroX, (int) cygaroY, 1000u, Easing.Linear);
            return cygaroY;
        }


        /*  protected async override void OnAppearing()
        {
            base.OnAppearing();

           /* PropertyChanged += MainPage_PropertyChanged;

            var dudaWidth = duda.Width;

            var realPos = dudaWidth * 92 / 400;


            var dudaHeight = duda.Height;

            var realHeight = dudaHeight * 114 / 400;

            await cygaro.TranslateTo((int)realPos, (int)realHeight, 1000u, Easing.BounceIn);#1#
        }*/

        private async void MainPage_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Height" && Width > 0 && Height > 0)
            {
                PropertyChanged += MainPage_PropertyChanged;

                cont.HeightRequest = Height;
                cont.WidthRequest = WidthRequest;

                var dudaWidth = duda.Width;

                var realPos = dudaWidth * 209 / 400;


                var dudaHeight = duda.Height;

                var realHeight = dudaHeight * 231 / 400;

                await cygaro.TranslateTo((int)realPos, (int)realHeight, 1000u, Easing.Linear);
            }
        }

        private async void Duda_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Width" && duda.Width > 0 && duda.Height > 0)
            {
                var dudaWidth = duda.Width;

                var realPos = dudaWidth * 92 / 400;


                var dudaHeight = duda.Height;

                var realHeight = dudaHeight * 130 / 400;

                await cygaro.TranslateTo((int)realPos, (int)realHeight,1000u, Easing.BounceIn);
            }
        }
    }
}
