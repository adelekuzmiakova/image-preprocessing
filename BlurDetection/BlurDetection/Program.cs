using System;
using System.Drawing;
using System.IO;
using NumSharp;
using Accord.Math;
using System.Linq;

namespace BlurDetect
{
    class Program
    {
        static void Main(string[] args)
        {
            Bitmap image = new Bitmap("vay-beta-release_storage_users_V1ZL6vpYJeNgQ0RZdS8sbdpRHc12_20-01-07_12-59-21_push_up1_pictures_12-59-26.048.jpg");

            // Treshold for focus metric, could be adjusted
            int focusTreshold = 3000;

            // RGB channels from the image
            var outChannels = ImageTransformation.ConvertBitmaptoRGB(image);
            int[,] redChannelOut = outChannels.redChannel;
            int[,] greenChannelOut = outChannels.greenChannel;
            int[,] blueChannelOut = outChannels.blueChannel;

            // Gray channel from RGB channels
            double[] grayChannel = ImageTransformation.ConvertRGBtoGray(redChannelOut, greenChannelOut, blueChannelOut);

            // Classification decision
            ImageTransformation.MakeBlurrynessClassification(focusTreshold, grayChannel);

        }


    }
}

