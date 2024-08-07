using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace PixShareLIB.Strategy
{
    public class ResizeImageStrategy : IImageProcessingStrategy
    {
        private readonly int _width;
        private readonly int _height;

        public ResizeImageStrategy(int width, int height)
        {
            _width = width;
            _height = height;
        }

        public void ProcessImage(string originalPath, string outputPath)
        {
            using (var image = Image.FromFile(originalPath))
            {
                var resized = new Bitmap(image, new Size(_width, _height));
                resized.Save(outputPath);
            }
        }
    }

    public class ConvertToPngStrategy : IImageProcessingStrategy
    {
        public void ProcessImage(string originalPath, string outputPath)
        {
            using (var image = Image.FromFile(originalPath))
            {
                image.Save(outputPath, ImageFormat.Png);
            }
        }
    }

    public class ConvertToJpgStrategy : IImageProcessingStrategy
    {
        public void ProcessImage(string originalPath, string outputPath)
        {
            using (var image = Image.FromFile(originalPath))
            {
                image.Save(outputPath, ImageFormat.Jpeg);
            }
        }
    }

    public class ConvertToBmpStrategy : IImageProcessingStrategy
    {
        public void ProcessImage(string originalPath, string outputPath)
        {
            using (var image = Image.FromFile(originalPath))
            {
                image.Save(outputPath, ImageFormat.Bmp);
            }
        }
    }
}
