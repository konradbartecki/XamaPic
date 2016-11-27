using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ImageMerge.Pages;
using Xamarin.Forms;

namespace ImageMerge
{
    public partial class App
    {
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new HomePage());
        }

        protected override void OnStart()
        {
            // Handle when your app starts
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
