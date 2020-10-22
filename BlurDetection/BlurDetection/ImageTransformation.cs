
using System;
using System.Drawing;
using System.IO;
using NumSharp;
using Accord.Math;
using System.Linq;

namespace BlurDetect

{
    public class ImageTransformation
    {
        public static (int[,] redChannel, int[,] greenChannel, int[,] blueChannel) ConvertBitmaptoRGB(Bitmap Image)
        {

            /// <summary>
            ///     Creates a tuple consisting of <see cref="int[,],int[,],int[,]"/> that store the red, green, 
            ///  and blue pixel data from the given <see cref="Bitmap"/>.
            /// </summary>                                          
            /// <param name="image">The image to load data from.</param>
            /// <returns>Three integer arrays that store the red, green, and blue pixel data from the given bitmap</returns>


            Bitmap image = Image;

            Color col = new Color();
            int width = Int32.Parse(image.Width.ToString());
            int height = Int32.Parse(image.Height.ToString());
            int[,] redChannel = new int[width, height];
            int[,] greenChannel = new int[width, height];
            int[,] blueChannel = new int[width, height];
            int red = 0, green = 0, blue = 0;

            for (int i = 0; i < width; i
                ++)
            {
                for (int j = 0; j < height; j++)
                {
                    col = image.GetPixel(i, j);
                    red = col.R;
                    green = col.G;
                    blue = col.B;
                    redChannel[i, j] = red;
                    greenChannel[i, j] = green;
                    blueChannel[i, j] = blue;
                }
            }

            return (redChannel, greenChannel, blueChannel);
        }


        public static double[] ConvertRGBtoGray(int[,] redChannel, int[,] greenChannel, int[,] blueChannel)
        {

            /// <summary>
            ///     Creates <see cref="double[]"/> that stores the gray channel from the corresponding red, green, and blue pixel 
            ///     <see cref="array"/>.
            /// </summary>
            /// <param name="redChannel">2d integer array storing red pixels.</param>
            /// <param name="greenChannel">2d integer array storing green pixels.</param>
            /// <param name="blueChannel">2d integer array storing blue pixels.</param>
            /// <returns>1d (flattened) double that stores the gray pixels from the RGB data</returns>

            int[,] redChannelOut = redChannel;
            int[,] greenChannelOut = greenChannel;
            int[,] blueChannelOut = blueChannel;


            NDArray redChannelND = NumSharp.np.array(redChannelOut);
            NDArray greenChannelND = NumSharp.np.array(greenChannelOut);
            NDArray blueChannelND = NumSharp.np.array(blueChannelOut);

            // https://stackoverflow.com/questions/2265910/convert-an-image-to-grayscale 
            NDArray grayChannelND = 0.2989 * redChannelND + 0.5870 * greenChannelND + 0.1140 * blueChannelND;
            // Flattens the array to make dot product operation more efficient
            NDArray grayChannelFlat = grayChannelND.flat;
            double[] grayChannel = (double[])grayChannelFlat;

            return grayChannel;

        }


        public static void MakeBlurrynessClassification(int focusTreshold, double[] gray)
        {

            /// <summary>
            ///     Creates <see cref="double[]"/> that represents the focus metric to characterize the degree of blurryness
            ///     that is present in the picture based on the following method:
            ///     https://www.pyimagesearch.com/2015/09/07/blur-detection-with-opencv/
            /// </summary>
            /// <param name="gray">1d double storing gray pixels from the entire bitmap.</param>
            /// <param name="focusTreshold">integer specifying the treshold for the focus metric, e.g. could be 3000.</param>
            /// <returns>double that represents the focus metric, which is the variance of the image convolved with the
            /// laplacian filter</returns>

            int Treshold = focusTreshold;
            double[] grayChannel = gray;

            // Normally it's a 3x3 filter
            double[] laplacianFilter = { 0, 1, 0, 1, -4, 1, 0, 1, 0 };

            double[] convolutionArray = new double[grayChannel.Length - laplacianFilter.Length];

            /* Calculates the dot product between the image patch and the filter and slides the patch by
             * one element. The dot products are stored in a new array  */
            for (int i = laplacianFilter.Length; i < grayChannel.Length; i++)
            {
                int lowerBound = i - laplacianFilter.Length;
                double[] grayChannelPatch = grayChannel[lowerBound..i];
                convolutionArray[lowerBound] = Matrix.Dot(laplacianFilter, grayChannelPatch);
            }

            /* Calculates the variance of the array containing dot products. This variance denotes the focus metric,
             * which is compared against the treshold. */
            double average = convolutionArray.Average();
            double sumOfSquaresOfDifferences = convolutionArray.Sum(val => (val - average) * (val - average));
            double variance = sumOfSquaresOfDifferences / (convolutionArray.Length - 1);

            if (variance < focusTreshold)
            {
                Console.WriteLine("blurry/dark: YES");
            }

            else
            {
                Console.WriteLine("blurry/dark: NO");
            }


        }


    }
}
