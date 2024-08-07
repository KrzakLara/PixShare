using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace PixShareLIB.Builder
{
    public class PhotoBuilder
    {
        private Image _image;
        private string _path;

        public PhotoBuilder(string path)
        {
            _path = path;
            _image = Image.FromFile(path);
        }

        public PhotoBuilder Resize(int width, int height)
        {
            // Create a new Bitmap with the specified dimensions and draw the original image onto it
            var resized = new Bitmap(width, height);
            using (var graphics = Graphics.FromImage(resized))
            {
                graphics.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
                graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
                graphics.DrawImage(_image, 0, 0, width, height);
            }
            _image.Dispose();
            _image = resized;
            return this;
        }

        public PhotoBuilder ApplySepia()
        {
            // Convert the image to a sepia tone
            Bitmap sepia = new Bitmap(_image.Width, _image.Height);

            for (int y = 0; y < sepia.Height; y++)
            {
                for (int x = 0; x < sepia.Width; x++)
                {
                    Color pixelColor = ((Bitmap)_image).GetPixel(x, y);

                    // Calculate new color values
                    int tr = (int)(0.393 * pixelColor.R + 0.769 * pixelColor.G + 0.189 * pixelColor.B);
                    int tg = (int)(0.349 * pixelColor.R + 0.686 * pixelColor.G + 0.168 * pixelColor.B);
                    int tb = (int)(0.272 * pixelColor.R + 0.534 * pixelColor.G + 0.131 * pixelColor.B);

                    // Ensure values are within byte range
                    tr = tr > 255 ? 255 : tr;
                    tg = tg > 255 ? 255 : tg;
                    tb = tb > 255 ? 255 : tb;

                    sepia.SetPixel(x, y, Color.FromArgb(tr, tg, tb));
                }
            }

            _image.Dispose();
            _image = sepia;
            return this;
        }

        public PhotoBuilder ApplyBlur()
        {
            // Apply a simple box blur effect
            Bitmap blurred = new Bitmap(_image.Width, _image.Height);

            // Kernel for a simple box blur
            int blurSize = 5;
            for (int y = blurSize; y < _image.Height - blurSize; y++)
            {
                for (int x = blurSize; x < _image.Width - blurSize; x++)
                {
                    int avgR = 0, avgG = 0, avgB = 0;
                    int blurPixelCount = 0;

                    for (int ky = -blurSize; ky <= blurSize; ky++)
                    {
                        for (int kx = -blurSize; kx <= blurSize; kx++)
                        {
                            int px = x + kx;
                            int py = y + ky;

                            Color pixel = ((Bitmap)_image).GetPixel(px, py);

                            avgR += pixel.R;
                            avgG += pixel.G;
                            avgB += pixel.B;

                            blurPixelCount++;
                        }
                    }

                    avgR = avgR / blurPixelCount;
                    avgG = avgG / blurPixelCount;
                    avgB = avgB / blurPixelCount;

                    blurred.SetPixel(x, y, Color.FromArgb(avgR, avgG, avgB));
                }
            }

            _image.Dispose();
            _image = blurred;
            return this;
        }

        public PhotoBuilder ConvertToFormat(ImageFormat format)
        {
            // Convert the image to a specified format
            _path = Path.ChangeExtension(_path, format.ToString().ToLower());
            return this;
        }

        public string Save(string outputDirectory)
        {
            // Ensure the output directory exists
            if (!Directory.Exists(outputDirectory))
            {
                Directory.CreateDirectory(outputDirectory);
            }

            var outputPath = Path.Combine(outputDirectory, Path.GetFileName(_path));

            // Save the image to the specified path
            using (var bitmap = new Bitmap(_image))
            {
                bitmap.Save(outputPath, GetImageFormat(_path));
            }

            _image.Dispose();
            return outputPath;
        }

        private ImageFormat GetImageFormat(string path)
        {
            // Determine the image format based on file extension
            string extension = Path.GetExtension(path).ToLower();

            if (extension == ".bmp")
            {
                return ImageFormat.Bmp;
            }
            else if (extension == ".png")
            {
                return ImageFormat.Png;
            }
            else if (extension == ".jpeg" || extension == ".jpg")
            {
                return ImageFormat.Jpeg;
            }
            else
            {
                throw new ArgumentException("Unsupported image format.");
            }
        }

    }
}
