using System;
using System.Collections.Generic;

namespace x16_png2bin
{
    class Program
    {
        static Dictionary<string, byte> colorDictionary = new Dictionary<string, byte>();

        static void Main(string[] args)
        {
            Image image;
            ColorPalette palette;
            Bitmap bitmap;

            if (args.Length == 0)
            {
                Console.WriteLine("Filename is missing.");
            }
            try
            {
                image = Image.FromFile(args[0]);
                palette = image.Palette;
                bitmap = new Bitmap(image);
                if (bitmap.Width % 2 == 1)
                {
                    throw new Exception("Width of image must be an even number.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return;
            }
            string colorName;
            for (byte i = 0; i < palette.Entries.Length; i++)
            {
                colorName = palette.Entries[i].Name;
                if (colorDictionary.ContainsKey(colorName))
                {
                    continue;
                }
                switch (colorName)
                {
                    case "ff000000":
                        colorDictionary.Add(colorName, 5);
                        break;
                    case "ffffffff":
                        colorDictionary.Add(colorName, 0);
                        break;
                    default:
                        colorDictionary.Add(palette.Entries[i].Name, i);
                        break;
                }
            }

            var path = Path.GetDirectoryName(args[0]);
            var name = Path.GetFileNameWithoutExtension(args[0]);
            var binfilename = name + ".bin";
            var textfilename = name + ".txt";
            var byteCount = WriteBitmapFile(bitmap, Path.Combine(path, binfilename));
            Console.WriteLine(byteCount + " bytes written to file " + binfilename);
            WritePaletteFile(palette, Path.Combine(path, textfilename));
            Console.WriteLine(palette.Entries.Length + " colors written to file " + textfilename);
        }

        private static int WriteBitmapFile(Bitmap bitmap, string filename)
        {
            int count = 0;
            try
            {
                using (var writer = new BinaryWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)))
                {
                    writer.Write(new byte[] { 0, 0 }); // add a two byte header
                    string p1, p2;
                    for (var y = 0; y < bitmap.Height; y++)
                    {
                        for (var x = 0; x < bitmap.Width; x += 2)
                        {
                            p1 = bitmap.GetPixel(x, y).Name;
                            p2 = bitmap.GetPixel(x + 1, y).Name;
                            writer.Write(HexStringToByte(p1, p2));
                            count++;
                        }
                    }
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            return count;
        }

        private static void WritePaletteFile(ColorPalette palette, string filename)
        {
            try
            {
                var sb = new StringBuilder("!word ");
                foreach (var color in palette.Entries)
                {
                    sb.Append(ColorToVERAColor(color) + ", ");
                }
                sb.Remove(sb.Length - 2, 2);
                using (var writer = new StreamWriter(new FileStream(filename, FileMode.Create, FileAccess.Write)))
                {
                    writer.WriteLine(sb.ToString());
                    writer.Close();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static string ColorToVERAColor(Color color)
        {
            var sb = new StringBuilder("$0");
            sb.Append(To4BitColor(color.R));
            sb.Append(To4BitColor(color.G));
            sb.Append(To4BitColor(color.B));
            return sb.ToString();
        }

        private static string To4BitColor(int value)
        {
            value = (value + 8) / 16;
            value = value > 15 ? 15 : value;
            return value.ToString("X");
        }

        private static byte HexStringToByte(string v1, string v2)
        {
            return (byte)((colorDictionary[v1] * 16) + colorDictionary[v2]);
        }
    }
}
