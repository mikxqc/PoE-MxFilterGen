using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen.json
{
    public class SETTINGS
    {
        public string git { get; set; }
        public string api { get; set; }
        public int minimumValue { get; set; }
        public bool verbose { get; set; }
        public bool ssf { get; set; }
        public string section { get; set; }
    }

    class settings
    {
        public static string GetGIT()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.git;
        }

        public static string GetAPI()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.api;
        }

        public static int GetMinimumValue()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.minimumValue;
        }

        public static bool GetVerbose()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.verbose;
        }

        public static string GetSection()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.section;
        }

        public static bool GetSSF()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.ssf;
        }

        public static void WriteSection(string section)
        {
            SETTINGS js = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText($@"settings.json"));
            SETTINGS se = new SETTINGS
            {
                git = js.git,
                api = js.api,
                minimumValue = js.minimumValue,
                verbose = js.verbose,
                ssf = js.ssf,
                section = section
            };
            var raw = JsonConvert.SerializeObject(se, Formatting.Indented);
            File.WriteAllText($@"settings.json", raw);
        }
    }
}
