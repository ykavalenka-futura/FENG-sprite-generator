using System.ComponentModel;
using System.Text;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace FEngSpriteGenerator
{
    class Program
    {
        private static void CustomConsoleMessage(string message, ConsoleColor customColor = ConsoleColor.Red)
        {
            Console.ForegroundColor = customColor;
            Console.Error.WriteLine(message);
            Console.ResetColor();
        }

        enum ArgumentKey
        {
            InputDir,
            OutputDir,
            Align,
            OutputFileName,
            PaddingValue,
            SpriteCssClassName,
            SpriteCssItemFormat,
            OutputCssFileName,
            OutputCssDir
        }

        static void Main(string[] args)
        {
            
            const string defaultOutputFileName = "sprite_result.png";
            const string defaultOutputCssFileName = "sprite_result.css";
            const string defaultCssSpriteClass = "sprite";
            const string defaultCssSpriteNameFormat = ".sprite-icon-{0}";
            const int defaultPadding = 10;
            const AlignSprite defaultAlign = AlignSprite.Vertical;
            
            CustomConsoleMessage("-----------------------------------------", ConsoleColor.Blue);
            CustomConsoleMessage("|\t\u00a9 Futura Solutions GmbH.\t|", ConsoleColor.Blue);
            CustomConsoleMessage("-----------------------------------------", ConsoleColor.Blue);
            
            Console.WriteLine($"Usage: FEngSpriteGenerator --{ArgumentKey.InputDir}=<value> --{ArgumentKey.OutputDir}=<value> "
                  + $"[--{ArgumentKey.Align}=<value>] [--{ArgumentKey.OutputFileName}=<value>] [--{ArgumentKey.PaddingValue}=<value>] "
                  + $"[--{ArgumentKey.SpriteCssClassName}=<value>] [--{ArgumentKey.SpriteCssItemFormat}=<value>] "
                  + $"[--{ArgumentKey.OutputCssFileName}=<value>] [--{ArgumentKey.OutputCssDir}=<value>]");
            Console.WriteLine($"\t--{ArgumentKey.InputDir}              \t- Path to the directory containing input images.");
            Console.WriteLine($"\t--{ArgumentKey.OutputDir}             \t- Path to the directory where the generated sprite will be saved.");
            Console.WriteLine($"\t--{ArgumentKey.Align}                 \t- Optional. Alignment of the generated sprite (V for vertical, H for horizontal). Defaults to '{defaultAlign}'.");
            Console.WriteLine($"\t--{ArgumentKey.OutputFileName}        \t- Optional. Name of the generated sprite file. Defaults to '{defaultOutputFileName}'.'");
            Console.WriteLine($"\t--{ArgumentKey.PaddingValue}          \t- Optional. Padding between images in the sprite. Defaults to '{defaultPadding}' pixels.");
            Console.WriteLine($"\t--{ArgumentKey.SpriteCssClassName}    \t- Optional. Name of base sprite css class. Defaults to '{defaultCssSpriteClass}'.");
            Console.WriteLine($"\t--{ArgumentKey.SpriteCssItemFormat}   \t- Optional. Name of generated sprite class format. Defaults to '{defaultCssSpriteNameFormat}'.");
            Console.WriteLine($"\t--{ArgumentKey.OutputCssFileName}     \t- Optional. Exported css file name. Defaults to '{defaultOutputCssFileName}'.");
            Console.WriteLine($"\t--{ArgumentKey.OutputCssDir}          \t- Optional. Path to the directory to export generated css. Defaults to output directory.");
            
            // Check that all required arguments were set.
            if (args.Length < 2)
            {
                CustomConsoleMessage("Not all required argument were set");
                return;
            }

            string inputDirectory = string.Empty;
            string outputDirectory = string.Empty;
            AlignSprite align = defaultAlign;
            string outputFileName = defaultOutputFileName;
            int padding = defaultPadding;
            string cssClass = defaultCssSpriteClass;
            string cssSpriteNameFormat = defaultCssSpriteNameFormat;
            string outputCssFileName = defaultOutputCssFileName;
            string outputCssDirectory = string.Empty;

            foreach (var arg in args)
            {
                if (string.IsNullOrEmpty(arg))
                    break;
                string[] keyValuePair = arg.Split("=");

                if (keyValuePair?.Length < 2 || string.IsNullOrEmpty(keyValuePair?[0]) || string.IsNullOrEmpty(keyValuePair[1]))
                    break;

                string  argumentKey = keyValuePair[0].TrimStart('-'), 
                        argumentValue = keyValuePair[1].TrimStart('\"').TrimEnd('\"');

                if (!Enum.TryParse(argumentKey, out ArgumentKey argumentKeyEnum))
                    break;

                switch (argumentKeyEnum)
                {
                    case ArgumentKey.InputDir:
                    {
                        if (!HasArgument(argumentValue, string.Empty, out inputDirectory) || string.IsNullOrEmpty(inputDirectory) || !Directory.Exists(inputDirectory))
                            CustomConsoleMessage($"Input '{inputDirectory}' directory does not exist.");
                        break;
                    }
                    case ArgumentKey.OutputDir:
                    {
                        if (!HasArgument(argumentValue, string.Empty, out outputDirectory) || string.IsNullOrEmpty(outputDirectory))
                            CustomConsoleMessage($"Parameter of output directory does not specified. Value: '{outputDirectory}'.");
                        break;
                    }
                    case ArgumentKey.Align: 
                    {
                        HasArgument(argumentValue, defaultAlign, out align);
                        break; 
                    }
                    case ArgumentKey.OutputFileName:
                    {
                        HasArgument(argumentValue, defaultOutputFileName, out outputFileName);
                        break;
                    }
                    case ArgumentKey.PaddingValue:
                    {
                        HasArgument(argumentValue, defaultPadding, out padding);
                        break;
                    }
                    case ArgumentKey.SpriteCssClassName:
                    {
                        HasArgument(argumentValue, defaultCssSpriteClass, out cssClass);
                        break;
                    }
                    case ArgumentKey.SpriteCssItemFormat:
                    {
                        HasArgument(argumentValue, defaultCssSpriteNameFormat, out cssSpriteNameFormat);
                        break;
                    }
                    case ArgumentKey.OutputCssFileName:
                    {
                        HasArgument(argumentValue, defaultOutputCssFileName, out outputCssFileName);
                        break;
                    }
                    case ArgumentKey.OutputCssDir:
                    {
                        HasArgument(argumentValue, outputDirectory, out outputCssDirectory);
                        break;
                    }
                }
            }

            // Check exist and available input directory.
            
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
            
            CustomConsoleMessage($"Input directory is: '{inputDirectory}'", Console.ForegroundColor);
            CustomConsoleMessage($"Output directory is: '{outputDirectory}'", Console.ForegroundColor);
            CustomConsoleMessage($"Sprite align used: '{align}'", Console.ForegroundColor);
            CustomConsoleMessage($"Output file generated by: '{defaultOutputFileName}'", Console.ForegroundColor);
            CustomConsoleMessage($"Padding between images: '{padding}'px", Console.ForegroundColor);
            CustomConsoleMessage($"Css class is: '.{cssClass}'", Console.ForegroundColor);
            CustomConsoleMessage($"Css class sprite name format is: '{cssSpriteNameFormat}'", Console.ForegroundColor);
            CustomConsoleMessage($"Output css file name: '{outputCssFileName}'", Console.ForegroundColor);
            CustomConsoleMessage($"Output css directory: '{outputCssDirectory}'", Console.ForegroundColor);

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

                StringBuilder cssDocument = new StringBuilder();
                CreateCssDocument(cssDocument, cssClass, outputCssFileName);

                for (int i = 0; i < images.Length; i++)
                {
                    var image = images[i];
                    var imageFile = Path.GetFileName(imageFiles[i]);

                    AppendCss(cssDocument, imageFile, cssSpriteNameFormat, x, y);
                    
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
                
                // Save css.
                string outputCssPath = Path.Combine(outputCssDirectory, outputCssFileName);
                try
                {
                    if (File.Exists(outputCssPath))
                        File.Delete(outputCssPath);
                    File.WriteAllText(outputCssPath, cssDocument.ToString());
                    CustomConsoleMessage($"Css file {outputCssFileName} was generated to '{outputCssPath}'", ConsoleColor.Green);
                }
                catch (Exception e)
                {
                    CustomConsoleMessage($"Css can NOT be generated to {outputCssPath}");
                    CustomConsoleMessage(e?.Message ?? e?.ToString() ?? string.Empty);
                }
            }
        }

        private static void CreateCssDocument(StringBuilder output, string spriteClassName, string outputFileName)
        {
            output.AppendLine("/* FEngSpriteGenerator: \u00a9 Futura Solutions GmbH. */");
            output.AppendLine(string.Empty);
            output.AppendLine($".{spriteClassName} {{");
            output.AppendLine($"\tbackground-image:  url('{outputFileName}');");
            output.AppendLine($"\tbackground-repeat: no-repeat;");
            output.AppendLine($"\t/* Append here your custom styles... */");
            output.AppendLine($"}}");
        }

        private static void AppendCss(StringBuilder output, string imageFileName, string cssPrefix, int x, int y)
        {
            var classImageFileName = Path.GetFileNameWithoutExtension(imageFileName).ToLower().Replace(" ", "");
            output.AppendLine($"/* Original image file '{imageFileName}' */");
            output.AppendLine($"{string.Format(cssPrefix, classImageFileName)} {{");
            output.Append("\t");
            output.Append($"background-position: {x * -1}px {y * -1}px;");
            output.AppendLine("");
            output.AppendLine($"}}");
        }

        private static bool HasArgument<T>(string argumentValue, T defaultValue, out T value)
        {
            value = defaultValue;
            return argumentValue.TryParse(defaultValue, out value);
        }
    }
    
    static class GenericValueConverter
    {
        public static bool TryParse<T>(this string input, T defaultValue, out T result)
        {
            result = defaultValue;

            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
            {
                try
                {
                    var convertedValue = (T)converter.ConvertFromString(input);
                    if (convertedValue == null)
                        return false;

                    result = convertedValue;
                    return true;
                }
                catch { }
            }
            return false;
        }
    }

    public enum AlignSprite
    {
        Vertical,
        Horizontal
    }
}