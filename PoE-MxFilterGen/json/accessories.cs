using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen.json
{
    public class SparklineAccessory
    {
        public List<double?> data { get; set; }
        public double? totalChange { get; set; }
    }

    public class ExplicitModifierAccessory
    {
        public string text { get; set; }
        public bool optional { get; set; }
    }

    public class LineAccessory
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
        public SparklineAccessory sparkline { get; set; }
        public List<object> implicitModifiers { get; set; }
        public List<ExplicitModifierAccessory> explicitModifiers { get; set; }
        public string flavourText { get; set; }
        public string itemType { get; set; }
        public double chaosValue { get; set; }
        public double exaltedValue { get; set; }
        public int count { get; set; }
    }

    public class RootAccessory
    {
        public List<LineAccessory> lines { get; set; }
    }

    class accessories
    {
        private static string iB;

        public static void GenAccessories()
        {
            List<string> itemBase = new List<string>();
            RootAccessory j = JsonConvert.DeserializeObject<RootAccessory>(File.ReadAllText("data/ninja.accessory.json", Encoding.UTF8));

            foreach (var ln in j.lines)
            {
                // Check if the item count is at least equal to the desired confidence level
                if (ln.count >= json.settings.GetConfidence())
                {
                    // Check if the item value is equal or superior to the minimum value
                    if (ln.chaosValue >= json.settings.GetMinimumValue())
                    {
                        msg.CMW(string.Format("[{0}][{1}c] Added to the list.", ln.baseType, ln.chaosValue), true, 1);
                        if (!itemBase.Contains(ln.baseType))
                        {
                            itemBase.Add(ln.baseType);
                            iB = iB + string.Format(@" ""{0}""", ln.baseType);
                        }
                    }
                }
            }

            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "## Accessories Gen" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "Show" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    BaseType" + iB + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    Rarity = Unique" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    SetTextColor 222 95 0" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    SetBackgroundColor 255 255 255" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    SetBorderColor 180 96 0" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    SetFontSize 45" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "    PlayAlertSound 8 300" + Environment.NewLine, Encoding.UTF8);
            File.AppendAllText(@"gen\" + main.fDate + "_gen.txt", "## END #######" + Environment.NewLine + Environment.NewLine, Encoding.UTF8);
        }
    }
}
