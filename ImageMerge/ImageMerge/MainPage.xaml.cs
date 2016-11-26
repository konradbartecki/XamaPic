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
//            var d = duda.Width;

            cont.WidthRequest = width;
//            cont.HeightRequest = height



            await CygaroY(Cygaro, 207, 230);
//            await CygaroY(oksy, 100, 116);
        }

        private bool was;

        private async Task CygaroY(Image item, int x, int y)
        {
            if (was)
                return;
            was = true;

            var cygaroX = Duda.Width * x / 400  /*Cygaro.Width/2*/;


            var cygaroY = Duda.Height  - (Duda.Height*y/400); /*+ Cygaro.Height/2*/;
          
            await item.TranslateTo((int) cygaroX, (int) -cygaroY, 1000u, Easing.Linear);
           
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

     
    }
}
