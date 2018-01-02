using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen.json
{
    public class Sparkline
    {
        public List<double?> data { get; set; }
        public double? totalChange { get; set; }
    }

    public class ExplicitModifier
    {
        public string text { get; set; }
        public bool optional { get; set; }
    }

    public class LineWeapon
    {
        public int id { get; set; }
        public string name { get; set; }
        public string icon { get; set; }
        public int mapTier { get; set; }
        public int levelRequired { get; set; }
        public string baseType { get; set; }
        public int stackSize { get; set; }
        public object variant { get; set; }
        public object prophecyText { get; set; }
        public object artFilename { get; set; }
        public int links { get; set; }
        public int itemClass { get; set; }
        public Sparkline sparkline { get; set; }
        public List<object> implicitModifiers { get; set; }
        public List<ExplicitModifier> explicitModifiers { get; set; }
        public string flavourText { get; set; }
        public string itemType { get; set; }
        public double chaosValue { get; set; }
        public double exaltedValue { get; set; }
        public int count { get; set; }
    }

    public class RootWeapon
    {
        public List<LineWeapon> lines { get; set; }
    }

    class weapons
    {
        private static string iB;

        public static void GenWeapons(string section)
        {
            List<string> itemBase = new List<string>();
            RootWeapon j = JsonConvert.DeserializeObject<RootWeapon>(File.ReadAllText("data/ninja.weapon.json", Encoding.UTF8));

            foreach (var ln in j.lines)
            {
                // Check if the item count is at least equal to the desired confidence level
                if (ln.count >= json.settings.GetConfidence() && ln.links <= 4)
                {
                    // Check if the item value is equal or superior to the minimum value
                    if (ln.chaosValue >= json.settings.GetMinimumValue())
                    {
                        if (json.settings.GetVerbose())
                        {
                            msg.CMW(string.Format("[{0}][{1}c] Added to the list.", ln.name, ln.chaosValue), true, 1);
                        }
                        if (!itemBase.Contains(ln.baseType))
                        {
                            itemBase.Add(ln.baseType);
                            iB = iB + string.Format(@" ""{0}""", ln.baseType);
                        }
                    }
                }
            }

            string fn = @"gen\" + section + ".filter";
            File.AppendAllText(fn, string.Format("# Section: {0}", section) + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "Show" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    BaseType" + iB + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    Rarity = Unique" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    SetTextColor 222 95 0" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    SetBackgroundColor 255 255 255" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    SetBorderColor 180 96 0" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    SetFontSize 45" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(fn, "    PlayAlertSound 8 300", Encoding.UTF8);
        }
    }
}
