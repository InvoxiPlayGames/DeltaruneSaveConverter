using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DeltaruneSaveConverter
{
    class SAVFormat
    {
        public static void Extract(string SAVFile, string OutputDir)
        {
            string contents = File.ReadAllText(SAVFile);
            contents = contents.Split('\0')[0]; // remove any null bytes
            Dictionary<string, string> json = JsonSerializer.Deserialize<Dictionary<string, string>>(contents);
            foreach (string file in json.Keys)
            {
                if (file == "default") continue;
                File.WriteAllText($"{OutputDir}/{file}", json[file]);
            }
        }

        public static void Pack(string SAVFile, string InputDir)
        {
            Dictionary<string, string> json = new();
            json.Add("default", "");
            foreach (string file in Directory.GetFiles(InputDir))
            {
                if (Path.GetFileName(file).ToLower() == "desktop.ini") return;
                json.Add(Path.GetFileName(file), File.ReadAllText(file));
            }
            string serialized = JsonSerializer.Serialize(json);
            serialized = serialized.Replace(@"\u0022", @"\""");
            File.WriteAllText(SAVFile, serialized);
        }
    }
}
