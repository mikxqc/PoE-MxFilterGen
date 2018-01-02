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
        public string api { get; set; }
        public string league { get; set; }
        public int confidence { get; set; }
        public int minimumValue { get; set; }
        public int chancingMinValue { get; set; }
        public bool verbose { get; set; }
    }

    class settings
    {
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
    }
}
