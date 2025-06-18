using System;
using System.IO;
using System.Runtime;
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
                case "buildini":
                    BuildINI(args[1], args[2]);
                    break;
                default:
                    PrintUsage();
                    break;
            }
            return;
        }

        /*
         * Okay, this one's annoying.
         * DELTARUNE Chapter 1&2 DEMO, at least on Switch, has leftover files that are outdated and broken.
         * Only deltarune_ch1.sav is good, EVEN FOR CHAPTER 2 SAVES. This has been the cause of too much confusion.
         * No, ChatGPT doesn't know this.
         */
        static bool DetectIfDemoSavegameWithOldStuff(string consoleSAV)
        {
            if (Path.GetFileName(consoleSAV) == "deltarune_ch2.sav")
            {
                string possibleCh1Path = consoleSAV.Replace("deltarune_ch2.sav", "deltarune_ch1.sav");
                if (File.Exists(possibleCh1Path))
                    return true;
            }
            else if (Path.GetFileName(consoleSAV) == "deltarune.sav")
            {
                string possibleCh1Path = consoleSAV.Replace("deltarune.sav", "deltarune_ch1.sav");
                if (File.Exists(possibleCh1Path))
                    return true;
            }
            return false;
        }

        static void ConvertFromConsole(string consoleSAV, string pcPath)
        {
            if (DetectIfDemoSavegameWithOldStuff(consoleSAV))
            {
                Console.WriteLine("!! WARNING !! If you're extracting from Chapter 1 & 2 DEMO, use \"deltarune_ch1.sav\" instead!");
                Console.WriteLine("Press Y to continue conversion, press any other button to cancel.");
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.KeyChar != 'Y' && cki.KeyChar != 'y')
                    return;
                Console.WriteLine("");
            }
            if (Directory.Exists(pcPath))
            {
                Console.WriteLine("!! WARNING !! You're extracting to a folder that already contains save files.");
                Console.WriteLine("You will overwrite any existing save data.");
                Console.WriteLine("Press Y to continue conversion, press any other button to cancel.");
                ConsoleKeyInfo cki = Console.ReadKey(true);
                if (cki.KeyChar != 'Y' && cki.KeyChar != 'y')
                    return;
                Console.WriteLine("");
            }
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
                        if (DeltaruneCh1.IsConsoleSaveFile(file)) {
                            Console.Write($"Converting save file {filename} from console to PC... ");
                            DeltaruneCh1 save = new();
                            save.ReadFromConsoleFile(file);
                            save.WriteToPCFile(file);
                            Console.WriteLine("done!");
                        } else {
                            Console.WriteLine($"{filename} is already PC format, skipping...!");
                        }
                    } catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
                else if (filename.StartsWith("filech2") || filename.StartsWith("filech3") || filename.StartsWith("filech4")) // assume this is a console save file from Chapter 2
                {
                    try
                    {
                        if (DeltaruneCh2.IsConsoleSaveFile(file)) {
                            Console.Write($"Converting save file {filename} from console to PC... ");
                            DeltaruneCh2 save = new();
                            save.ReadFromConsoleFile(file);
                            save.WriteToPCFile(file);
                            Console.WriteLine("done!");
                        } else {
                            Console.WriteLine($"{filename} is already PC format, skipping...!");
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
                else
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
                else if (filename.StartsWith("filech2") || filename.StartsWith("filech3") || filename.StartsWith("filech4")) // assume this is a PC save file from Chapter 2
                {
                    try
                    {
                        Console.Write($"Converting save file {filename} from PC to console... ");
                        DeltaruneCh2 save = new();
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
            if (!Directory.Exists(extractPath)) Directory.CreateDirectory(extractPath);
            SAVFormat.Extract(consoleSAV, extractPath);
        }

        static void PackSAV(string packPath, string consoleSAV)
        {
            Console.WriteLine($"Packing \"{packPath}\" into \"{consoleSAV}\"...");
            SAVFormat.Pack(consoleSAV, packPath);
        }

        static void ConvertFileFromConsole(string consolePath, string pcPath)
        {
            // todo: chapter1/chapter2 toggle or detection
            Console.WriteLine($"Converting \"{consolePath}\" from console format to PC format at \"{pcPath}\"...");
            DeltaruneCh2 save = new();
            save.ReadFromConsoleFile(consolePath);
            save.WriteToPCFile(pcPath);
        }

        static void ConvertFileFromPC(string pcPath, string consolePath)
        {
            // todo: chapter1/chapter2 toggle or detection
            Console.WriteLine($"Converting \"{pcPath}\" from PC format to console format at \"{consolePath}\"...");
            DeltaruneCh2 save = new();
            save.ReadFromPCFile(pcPath);
            save.WriteToConsoleFile(consolePath);
        }

        static void BuildINI(string pcPath, string iniPath)
        {
            Console.WriteLine("Opening INI file...");
            IniFile ini = new(iniPath);
            Console.WriteLine("Reading PC save files...");
            int status = 0;
            foreach (string file in Directory.GetFiles(pcPath))
            {
                string filename = Path.GetFileName(file);
                if (filename.StartsWith("filech1")) // assume this is a PC save file from Chapter 1
                {
                    try
                    {
                        Console.Write($"Reading {filename}... ");
                        DeltaruneCh1 save = new();
                        save.ReadFromPCFile(file);
                        Console.WriteLine("done!");
                        Console.Write($"Writing \"{save.truename}\" chapter 1 save to INI... ");
                        int saveid = int.Parse(filename.Replace("filech1_", ""));
                        if (saveid >= 3 && saveid < 6)
                            status = 1; // game complete, set console-only STATUS variable
                        ini.Write("Name", save.truename, $"G{saveid}");
                        ini.Write("Level", $"\"{save.lv}\"", $"G{saveid}");
                        ini.Write("Love", $"\"{save.llv}\"", $"G{saveid}");
                        ini.Write("Time", $"\"{save.time}\"", $"G{saveid}");
                        ini.Write("Date", $"\"0\"", $"G{saveid}"); // TODO: Get valid date value
                        ini.Write("Room", $"\"{save.currentroom}\"", $"G{saveid}");
                        ini.Write("InitLang", $"\"{save.flag[912]}\"", $"G{saveid}");
                        int uraboss = 0;
                        if (save.flag[241] == 6) uraboss = 1;
                        if (save.flag[241] == 7) uraboss = 2;
                        ini.Write("UraBoss", $"\"{uraboss}\"", $"G{saveid}");
                        Console.WriteLine("done!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
                else if (filename.StartsWith("filech2")) // assume this is a PC save file from Chapter 2
                {
                    try
                    {
                        Console.Write($"Reading {filename}... ");
                        DeltaruneCh2 save = new();
                        save.ReadFromPCFile(file);
                        Console.WriteLine("done!");
                        Console.Write($"Writing \"{save.truename}\" chapter 2 save to INI... ");
                        int saveid = int.Parse(filename.Replace("filech2_", ""));
                        if (saveid >= 3 && saveid < 6)
                            status = 1; // game complete, set console-only STATUS variable
                        ini.Write("Name", save.truename, $"G_2_{saveid}");
                        ini.Write("Level", $"\"{save.lv}\"", $"G_2_{saveid}");
                        ini.Write("Love", $"\"{save.llv}\"", $"G_2_{saveid}");
                        ini.Write("Time", $"\"{save.time}\"", $"G_2_{saveid}");
                        ini.Write("Date", $"\"0\"", $"G_2_{saveid}"); // TODO: Get valid date value
                        ini.Write("Room", $"\"{save.currentroom}\"", $"G_2_{saveid}");
                        ini.Write("InitLang", $"\"{save.flag[912]}\"", $"G_2_{saveid}");
                        int uraboss = (int)save.flag[571];
                        ini.Write("UraBoss", $"\"{uraboss}\"", $"G_2_{saveid}");
                        Console.WriteLine("done!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
                else if (filename.StartsWith("filech3")) // assume this is a PC save file from Chapter 3
                {
                    try
                    {
                        Console.Write($"Reading {filename}... ");
                        DeltaruneCh2 save = new();
                        save.ReadFromPCFile(file);
                        Console.WriteLine("done!");
                        Console.Write($"Writing \"{save.truename}\" chapter 3 save to INI... ");
                        int saveid = int.Parse(filename.Replace("filech3_", ""));
                        if (saveid >= 3 && saveid < 6)
                            status = 1; // game complete, set console-only STATUS variable
                        ini.Write("Name", save.truename, $"G_3_{saveid}");
                        ini.Write("Level", $"\"{save.lv}\"", $"G_3_{saveid}");
                        ini.Write("Love", $"\"{save.llv}\"", $"G_3_{saveid}");
                        ini.Write("Time", $"\"{save.time}\"", $"G_3_{saveid}");
                        ini.Write("Date", $"\"0\"", $"G_3_{saveid}"); // TODO: Get valid date value
                        ini.Write("Room", $"\"{save.currentroom}\"", $"G_3_{saveid}");
                        ini.Write("InitLang", $"\"{save.flag[912]}\"", $"G_3_{saveid}");
                        int uraboss = (int)save.flag[1047];
                        ini.Write("UraBoss", $"\"{uraboss}\"", $"G_3_{saveid}");
                        Console.WriteLine("done!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
                else if (filename.StartsWith("filech4")) // assume this is a PC save file from Chapter 4
                {
                    try
                    {
                        Console.Write($"Reading {filename}... ");
                        DeltaruneCh2 save = new();
                        save.ReadFromPCFile(file);
                        Console.WriteLine("done!");
                        Console.Write($"Writing \"{save.truename}\" chapter 4 save to INI... ");
                        int saveid = int.Parse(filename.Replace("filech4_", ""));
                        if (saveid >= 3 && saveid < 6)
                            status = 1; // game complete, set console-only STATUS variable
                        ini.Write("Name", save.truename, $"G_4_{saveid}");
                        ini.Write("Level", $"\"{save.lv}\"", $"G_4_{saveid}");
                        ini.Write("Love", $"\"{save.llv}\"", $"G_4_{saveid}");
                        ini.Write("Time", $"\"{save.time}\"", $"G_4_{saveid}");
                        ini.Write("Date", $"\"0\"", $"G_4_{saveid}"); // TODO: Get valid date value
                        ini.Write("Room", $"\"{save.currentroom}\"", $"G_4_{saveid}");
                        ini.Write("InitLang", $"\"{save.flag[912]}\"", $"G_4_{saveid}");
                        int uraboss = (int)save.flag[1629];
                        ini.Write("UraBoss", $"\"{uraboss}\"", $"G_4_{saveid}");
                        Console.WriteLine("done!");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"error: {ex.Message}");
                        return;
                    }
                }
            }
            ini.Write("STATUS", $"\"{status}\"", "STATUS");
        }
    }
}
