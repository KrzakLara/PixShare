using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Strategy
{
    public class ImageProcessor
    {
        private IImageProcessingStrategy _strategy;

        public void SetStrategy(IImageProcessingStrategy strategy)
        {
            _strategy = strategy;
        }

        public void ProcessImage(string originalPath, string outputPath)
        {
            _strategy?.ProcessImage(originalPath, outputPath);
        }
    }

}
