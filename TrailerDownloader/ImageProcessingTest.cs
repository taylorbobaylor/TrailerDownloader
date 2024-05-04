using System;
using Emgu.CV;
using Emgu.CV.Structure;
using System.IO;

namespace TrailerDownloader
{
    public static class ImageProcessingTest
    {
        public static void RunTest()
        {
            string imagePath = "test_image.png";
            using (var image = new Image<Bgr, byte>(200, 100))
            {
                // Fill the background with white color
                image.SetValue(new Bgr(255, 255, 255));

                // Create a font and brush for drawing
                CvInvoke.PutText(
                    image,
                    "Test Image",
                    new System.Drawing.Point(image.Width / 2, image.Height / 2),
                    Emgu.CV.CvEnum.FontFace.HersheyDuplex,
                    1.0,
                    new Bgr(0, 0, 0).MCvScalar);

                // Save the image to the file system
                image.Save(imagePath);
            }

            Console.WriteLine($"Test image saved to {imagePath}");
        }
    }
}
