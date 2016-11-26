using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XamaPic.Server.Core;

namespace XamaPic.Utility
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                // Do any async anything you need here without worry
                await new PhotoApi().Test();

            }).Wait();
        }
    }
}
