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
        public string league { get; set; }
        public int confidence { get; set; }
        public int minimumValue { get; set; }
        public int chancingMinValue { get; set; }
        public bool verbose { get; set; }
        public bool strict { get; set; }
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

        public static string GetLeague()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.league;
        }

        public static int GetConfidence()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.confidence;
        }

        public static int GetMinimumValue()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.minimumValue;
        }

        public static int GetChancingMinValue()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.chancingMinValue;
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

        public static bool GetStrict()
        {
            SETTINGS j = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText("settings.json"));
            return j.strict;
        }

        public static void WriteSection(string section)
        {
            SETTINGS js = JsonConvert.DeserializeObject<SETTINGS>(File.ReadAllText($@"settings.json"));
            SETTINGS se = new SETTINGS
            {
                git = js.git,
                api = js.api,
                league = js.league,
                confidence = js.confidence,
                minimumValue = js.minimumValue,
                chancingMinValue = js.chancingMinValue,
                verbose = js.verbose,
                section = section
            };
            var raw = JsonConvert.SerializeObject(se, Formatting.Indented);
            File.WriteAllText($@"settings.json", raw);
        }
    }
}
