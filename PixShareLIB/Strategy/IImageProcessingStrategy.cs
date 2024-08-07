using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PixShareLIB.Strategy
{
    public interface IImageProcessingStrategy
    {
        void ProcessImage(string originalPath, string outputPath);
    }

}
