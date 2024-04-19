﻿using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FEngSpriteGenerator
{
    class Program
    {
        private static void CustomConsoleMessage(string message, ConsoleColor customColor = ConsoleColor.Red)
        {
            ConsoleColor originalColor = Console.ForegroundColor;
            Console.ForegroundColor = customColor;
            Console.Error.WriteLine(message);
            Console.ForegroundColor = originalColor;
        }

        static void Main(string[] args)
        {
            const string defaultOutputFileName = "sprite_result.png";
            const int defaultPadding = 10;
            const AlignSprite defaultAlign = AlignSprite.Vertical;
            
            CustomConsoleMessage("-----------------------------------------", ConsoleColor.Blue);
            CustomConsoleMessage("|\t\u00a9 Futura Solutions GmbH.\t|", ConsoleColor.Blue);
            CustomConsoleMessage("-----------------------------------------", ConsoleColor.Blue);
            
            Console.WriteLine("Usage: FEngSpriteGenerator <input_directory> <output_directory> "
                  + "[align] [output_filename] [padding_value]");
            Console.WriteLine("\t<input_directory>  \t- Path to the directory containing input images.");
            Console.WriteLine("\t<output_directory> \t- Path to the directory where the generated sprite will be saved.");
            Console.WriteLine($"\t-align            \t- Optional. Alignment of the generated sprite (V for vertical, H for horizontal). Defaults to '{defaultAlign}'.");
            Console.WriteLine($"\t-output_filename  \t- Optional. Name of the generated sprite file. Defaults to '{defaultOutputFileName}'.'");
            Console.WriteLine($"\t-padding_value    \t- Optional. Padding between images in the sprite. Defaults to '{defaultPadding}' pixels.");
            
            // Check that all required arguments were set.
            if (args.Length < 2)
            {
                CustomConsoleMessage("Not all required argument were set");
                return;
            }

            string inputDirectory = args[0];
            string outputDirectory = args[1];

            // Check exist and available input directory.
            if (string.IsNullOrEmpty(inputDirectory) || !Directory.Exists(inputDirectory))
            {
                CustomConsoleMessage($"Input '{inputDirectory}' directory does not exist.");
                return;
            }
            if (string.IsNullOrEmpty(outputDirectory))
            {
                CustomConsoleMessage($"Parameter of output directory does not specified. Value: '{outputDirectory}'.");
                return;
            }

            if (!Directory.Exists(outputDirectory))
            {
                Console.WriteLine($"Output directory not created. Folder will create by path: '{outputDirectory}'.");
                try
                {
                    Directory.CreateDirectory(outputDirectory);
                }
                catch (Exception e)
                {
                    CustomConsoleMessage(e?.Message ?? e?.ToString() ?? string.Empty);
                    return;
                }
            }

            AlignSprite align;

            if (args.Length >= 3 && !string.IsNullOrEmpty(args[2]) && Enum.TryParse(args[2], out align)) { }
            else
                align = defaultAlign;
            
            Console.WriteLine($"Sprite align used: '{align}'");

            string outputFileName = string.Empty;
            int padding = defaultPadding;

            if (args.Length >= 4 && !string.IsNullOrEmpty(args[3]) && args[3].EndsWith(".png"))
                outputFileName = args[3];
            else
                outputFileName = defaultOutputFileName;

            Console.WriteLine($"Output file generated by: '{defaultOutputFileName}'");

            if (args.Length >= 5 && !string.IsNullOrEmpty(args[4]) && int.TryParse(args[4], out padding))
            { }
            else
                padding = defaultPadding;

            Console.WriteLine($"Padding between images: '{padding}'px");

            // Get list of files in input folder
            string[] imageFiles = Array.Empty<string>();
            try
            {
                imageFiles = Directory.GetFiles(inputDirectory, "*.png");
            }
            catch (Exception e)
            {
                CustomConsoleMessage(e?.Message ?? e?.ToString() ?? string.Empty);
                return;
            }

            // Check available files.
            if (imageFiles.Length == 0)
            {
                CustomConsoleMessage("PNG files not found in the input directory.");
                return;
            }

            // Loading all input images.
            Image<Rgba32>[] images = Array.Empty<Image<Rgba32>>();
            try
            {
                images = imageFiles.Select(file => Image.Load<Rgba32>(file)).ToArray();
            }
            catch (Exception e)
            {
                CustomConsoleMessage(e?.Message ?? e?.ToString() ?? string.Empty);
                return;
            }

            // Calculate sprite size.
            int spriteWidth = (align == AlignSprite.Vertical) 
                    ? images.Max(image => image.Width) 
                    : images.Sum(image => image.Width) + (images.Length - 1) * padding;
            int spriteHeight = (align == AlignSprite.Vertical) 
                    ? images.Sum(image => image.Height) + (images.Length - 1) * padding 
                    : images.Max(image => image.Height);

            // Create new sprite image.
            using (Image<Rgba32> sprite = new Image<Rgba32>(spriteWidth, spriteHeight))
            {
                int x = 0, y = 0;

                foreach (var image in images)
                {
                    // Append image to the sprite.
                    sprite.Mutate(ctx => ctx.DrawImage(image, new SixLabors.ImageSharp.Point(x, y), 1));

                    // Update coordinates for append next image.
                    if (align == AlignSprite.Vertical)
                        y += image.Height + padding;
                    else
                        x += image.Width + padding;    
                }
                
                // Save sprite.
                string outputPath = Path.Combine(outputDirectory, outputFileName);
                try
                {
                    sprite.Save(outputPath);
                    CustomConsoleMessage($"Sprite {outputFileName} saved to '{outputPath}'", ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    CustomConsoleMessage($"Sprite can NOT be saved to {outputPath}");
                    CustomConsoleMessage(e?.Message ?? e?.ToString() ?? string.Empty);
                }
            }
        }
    }

    public enum AlignSprite
    {
        Vertical,
        Horizontal
    }
}