using System;
using System.IO;
using System.Text.Json;

namespace DeltaruneSaveConverter
{
    class Program
    {
        static void PrintUsage()
        {
            Console.WriteLine();
            Console.WriteLine("usage: DeltaruneSaveConverter [command] [path1] [path2]");
            Console.WriteLine();
            Console.WriteLine("commands:");
            Console.WriteLine("ConvertFromConsole: extracts a SAV file from path1 into path2 and converts the save files into PC format");
            Console.WriteLine("ConvertFromPC: converts save files from path1 into console format and packs them into a SAV at path2");
            Console.WriteLine("ExtractSAV: extracts a SAV file from path1 into path2 - supports UNDERTALE");
            Console.WriteLine("PackSAV: packs a SAV file with files from path1 into path2 - supports UNDERTALE");
            Console.WriteLine("ConvertFileFromConsole: converts the save file at path1 into PC format and saves it at path2");
            Console.WriteLine("ConvertFileFromPC: converts the save file at path1 into console format and saves it at path2");
            Console.WriteLine();
            Console.WriteLine("example:");
            Console.WriteLine("DeltaruneSaveConverter ConvertFromConsole deltarune_ch1.sav ./pcsave/");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            if (args.Length != 3)
            {
                PrintUsage();
                return;
            }

            switch (args[0].ToLower())
            {
                case "convertfromconsole":
                    ConvertFromConsole(args[1], args[2]);
                    break;
                case "convertfrompc":
                    ConvertFromPC(args[1], args[2]);
                    break;
                case "extractsav":
                    ExtractSAV(args[1], args[2]);
                    break;
                case "packsav":
                    PackSAV(args[1], args[2]);
                    break;
                case "convertfilefromconsole":
                    ConvertFileFromConsole(args[1], args[2]);
                    break;
                case "convertfilefrompc":
                    ConvertFileFromPC(args[1], args[2]);
                    break;
                default:
                    PrintUsage();
                    break;
            }
            return;
        }

        static void ConvertFromConsole(string consoleSAV, string pcPath)
        {
            Console.Write("Extracting console SAV file... ");
            try
            {
                Directory.CreateDirectory(pcPath); // create the PC directory in case it doesn't exist
                SAVFormat.Extract(consoleSAV, pcPath);
                Console.WriteLine("done!");
            } catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
                return;
            }
            foreach (string file in Directory.GetFiles(pcPath))
            {
                string filename = Path.GetFileName(file);
                if (filename.StartsWith("filech1")) // assume this is a console save file from Chapter 1
                {
                    try
                    {
                        Console.Write($"Converting save file {filename} from console to PC... ");
                        DeltaruneCh1 save = new();
                        save.ReadFromConsoleFile(file);
                        save.WriteToPCFile(file);
                        Console.WriteLine("done!");
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                } else
                {
                    Console.WriteLine($"Skipping conversion of {filename}...");
                }
            }
        }

        static void ConvertFromPC(string pcPath, string consoleSAV)
        {
            if (Directory.Exists("./temp/")) Directory.Delete("./temp/", true);
            Directory.CreateDirectory("./temp/"); // create temp directory for save output
            Console.WriteLine("Reading PC save files... ");
            foreach (string file in Directory.GetFiles(pcPath))
            {
                string filename = Path.GetFileName(file);
                if (filename.StartsWith("filech1")) // assume this is a PC save file from Chapter 1
                {
                    try
                    {
                        Console.Write($"Converting save file {filename} from PC to console... ");
                        DeltaruneCh1 save = new();
                        save.ReadFromPCFile(file);
                        save.WriteToConsoleFile($"./temp/{filename}");
                        Console.WriteLine("done!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
                else
                {
                    try
                    {

                        Console.Write($"Copying {filename} to temp directory... ");
                        File.Copy(file, $"./temp/{filename}");
                        Console.WriteLine("done!");
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
            }
            try
            {
                Console.Write("Writing console SAV file... ");
                SAVFormat.Pack(consoleSAV, "./temp/");
                Console.WriteLine("done!");
            } catch (Exception ex)
            {
                Console.WriteLine($"error: {ex.Message}");
                return;
            }
            if (Directory.Exists("./temp/")) Directory.Delete("./temp/", true);
        }

        static void ExtractSAV(string consoleSAV, string extractPath)
        {
            Console.WriteLine($"Extracting \"{consoleSAV}\" to \"{extractPath}\"...");
            SAVFormat.Extract(consoleSAV, extractPath);
        }

        static void PackSAV(string packPath, string consoleSAV)
        {
            Console.WriteLine($"Packing \"{packPath}\" into \"{consoleSAV}\"...");
            SAVFormat.Pack(consoleSAV, packPath);
        }

        static void ConvertFileFromConsole(string consolePath, string pcPath)
        {
            Console.WriteLine($"Converting \"{consolePath}\" from console format to PC format at \"{pcPath}\"...");
            DeltaruneCh1 save = new();
            save.ReadFromConsoleFile(consolePath);
            save.WriteToPCFile(pcPath);
        }

        static void ConvertFileFromPC(string pcPath, string consolePath)
        {
            Console.WriteLine($"Converting \"{pcPath}\" from PC format to console format at \"{consolePath}\"...");
            DeltaruneCh1 save = new();
            save.ReadFromPCFile(pcPath);
            save.WriteToConsoleFile(consolePath);
        }
    }
}
