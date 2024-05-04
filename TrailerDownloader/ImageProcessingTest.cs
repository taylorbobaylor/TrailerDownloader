using System;
using System.Drawing;
using System.IO;

namespace TrailerDownloader
{
    public static class ImageProcessingTest
    {
        public static void RunTest()
        {
            string imagePath = "test_image.png";
            using (var bitmap = new Bitmap(200, 100))
            using (var graphics = Graphics.FromImage(bitmap))
            {
                // Fill the background with white color
                graphics.FillRectangle(Brushes.White, 0, 0, bitmap.Width, bitmap.Height);

                // Create a font and brush for drawing
                using (var font = new Font("Arial", 20))
                using (var brush = new SolidBrush(Color.Black))
                {
                    // Set the string format
                    var format = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    };

                    // Draw the text in the center of the image
                    graphics.DrawString("Test Image", font, brush, new RectangleF(0, 0, bitmap.Width, bitmap.Height), format);
                }

                // Save the image to the file system
                bitmap.Save(imagePath, System.Drawing.Imaging.ImageFormat.Png);
            }

            Console.WriteLine($"Test image saved to {imagePath}");
        }
    }
}
