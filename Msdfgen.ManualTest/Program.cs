using Msdfgen.IO;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Msdfgen.ManualTest
{
    internal static class Program
    {
        static void Main(string[] args)
        {
            double advance = 0;
            var ft = ImportFont.InitializeFreetype();
            var font = ImportFont.LoadFont(ft, "arial.otf");
            var shape = ImportFont.LoadGlyph(font, 'B', ref advance);
            var msdf = new Image<RgbaVector>(32, 32);
            var generator = Generate.Msdf();
            generator.Output = msdf;
            generator.Range = 0.5;
            generator.Scale = new Vector2(1);
            generator.Translate = new Vector2(2, 2);

            for (int i = 0; i < 5; ++i)
            {
                shape.Normalize();
                Coloring.EdgeColoringSimple(shape, 3.0);
                generator.Shape = shape;
                generator.Compute();
                if (i % 100 == 0)
                    System.Console.WriteLine(i);
            }
            
            msdf.Mutate(m => m.Flip(FlipMode.Vertical));
            msdf.SaveAsBmp("output.bmp");

            {
                var rast = new Image<Rg32>(1024, 1024);
                Render.RenderSdf(rast, msdf, 0.5f);
                rast.SaveAsBmp("rasterized.bmp");
            }

            ImportFont.DestroyFont(font);
            ImportFont.DeinitializeFreetype(ft);
        }
    }
}