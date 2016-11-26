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

            duda.PropertyChanged += Duda_PropertyChanged;
        }

        protected async override void OnAppearing()
        {
            base.OnAppearing();

            var dudaWidth = duda.Width;

            var realPos = dudaWidth * 92 / 400;


            var dudaHeight = duda.Height;

            var realHeight = dudaHeight * 114 / 400;

            await cygaro.TranslateTo((int)realPos, (int)realHeight, 1000u, Easing.BounceIn);
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
