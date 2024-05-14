using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Bmp;
using SixLabors.ImageSharp.Processing;

public static class Utilities
{

    public static Stream Convert(string inputFileName, double lightenBy)
    {

        var ms = new MemoryStream();
        using (var inStream = File.OpenRead(inputFileName))

        using (Image image = SixLabors.ImageSharp.Image.Load(inStream))
        {
            //pal_image.putpalette( (0,0,0,  255,255,255,  0,255,0,   0,0,255,  255,0,0,  255,255,0, 255,128,0) + (0,0,0)*249)
            var palette = new Color[]{
            Color.FromRgb(0,0,0),
            Color.FromRgb(255,255,255),
            Color.FromRgb(0,255,0),
            Color.FromRgb(0,0,255),
            Color.FromRgb(255,255,0),
            Color.FromRgb(255,128,0)
            };
            // var quantizer = new PaletteQuantizer(palette,new QuantizerOptions{
            //   //  Dither=KnownDitherings.FloydSteinberg
            // });

            image.Mutate(i =>
                i
                .Lightness(1f + (float)(lightenBy/100))
                .Resize(new ResizeOptions
                {
                    Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop,
                    Position = AnchorPositionMode.Center,
                    Size = new Size(800, 480)
                })
                .Dither(KnownDitherings.FloydSteinberg, palette)
                );

            var enc = new BmpEncoder()
            {
                //Quantizer=quantizer,

            };
            image.Save(ms, enc);

        }

        ms.Position = 0;
        return ms;
    }
    public static string Convert(string inputFileName, string outDir, double lightenBy)
    {
        var inputDir = Directory.GetParent(Path.GetFullPath(inputFileName)).FullName;
        var outputFileName = $"{outDir}/{Path.GetFileNameWithoutExtension(inputFileName)}_output.bmp";

        using (var outStream = File.OpenWrite(outputFileName))
        using (var ms = Convert(inputFileName, lightenBy))
        {
            ms.CopyTo(outStream);
        }
        // using (var inStream = File.OpenRead(inputFileName))
        // using (var outStream = File.OpenWrite(outputFileName))
        // using (Image image = SixLabors.ImageSharp.Image.Load(inStream))
        // {
        //     //pal_image.putpalette( (0,0,0,  255,255,255,  0,255,0,   0,0,255,  255,0,0,  255,255,0, 255,128,0) + (0,0,0)*249)
        //     var palette = new Color[]{
        //     Color.FromRgb(0,0,0),
        //     Color.FromRgb(255,255,255),
        //     Color.FromRgb(0,255,0),
        //     Color.FromRgb(0,0,255),
        //     Color.FromRgb(255,255,0),
        //     Color.FromRgb(255,128,0)
        //     };
        //     // var quantizer = new PaletteQuantizer(palette,new QuantizerOptions{
        //     //   //  Dither=KnownDitherings.FloydSteinberg
        //     // });

        //     image.Mutate(i =>
        //         i
        //         .Lightness(1.3f)
        //         .Resize(new ResizeOptions
        //         {
        //             Mode = SixLabors.ImageSharp.Processing.ResizeMode.Crop,
        //             Position = AnchorPositionMode.Center,
        //             Size = new Size(800, 480)
        //         })
        //         .Dither(KnownDitherings.FloydSteinberg, palette)
        //         );

        //     var enc = new BmpEncoder()
        //     {
        //         //Quantizer=quantizer,

        //     };
        //     image.Save(outStream, enc);

        // }
        return outputFileName;
    }

}