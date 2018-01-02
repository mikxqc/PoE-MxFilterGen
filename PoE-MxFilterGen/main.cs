using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen
{
    class main
    {
        private static DateTime dt = DateTime.Now;

        public static string version = "2.0.0";
        public static string fDate = string.Format("{0}-{1}-{2}",dt.Day,dt.Month,dt.Year);

        public class STRUCTURE
        {
            public string section { get; set; }
            public bool gen { get; set; }
            public bool @static { get; set; }
            public bool remote { get; set; }
            public string url { get; set; }
        }

        public class RootStructure
        {
            public List<STRUCTURE> structures { get; set; }
        }

        static void Main(string[] args)
        {
            msg.Splash();
            msg.CMW(string.Format("API: {0}", json.settings.GetAPI()),true,1);
            msg.CMW(string.Format("League: {0}", json.settings.GetLeague()), true, 1);
            msg.CMW(string.Format("Confidence: {0}", json.settings.GetConfidence().ToString()), true, 1);
            msg.CMW(string.Format("Minimum Value: {0}c", json.settings.GetMinimumValue().ToString()), true, 1);
            msg.CMW(string.Format("Chancing Min. Value: {0}c", json.settings.GetChancingMinValue().ToString()), true, 1);
            msg.CMW(string.Format("Verbose: {0}", json.settings.GetVerbose().ToString()), true, 1);

            // Check if data and gen exists
            if (!Directory.Exists(@"data\"))
            {
                Directory.CreateDirectory(@"data\");
            }
            if (!Directory.Exists(@"gen\"))
            {
                Directory.CreateDirectory(@"gen\");
            }
            if (!Directory.Exists(@"filter\"))
            {
                Directory.CreateDirectory(@"filter\");
            }

            // Clean all generated data
            DirectoryInfo dataDir = new DirectoryInfo(@"data\");
            DirectoryInfo genDir = new DirectoryInfo(@"gen\");
            DirectoryInfo filterDir = new DirectoryInfo(@"filter\");
            foreach (FileInfo file in dataDir.GetFiles())
            {
                file.Delete();
            }
            foreach (FileInfo file in genDir.GetFiles())
            {
                file.Delete();
            }
            foreach (FileInfo file in filterDir.GetFiles())
            {
                file.Delete();
            }

            // Get latest poe.ninja api
            web.SaveString(json.settings.GetAPI() + "GetUniqueArmourOverview?league=" + json.settings.GetLeague(), "data/ninja.armour.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueWeaponOverview?league=" + json.settings.GetLeague(), "data/ninja.weapon.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueAccessoryOverview?league=" + json.settings.GetLeague(), "data/ninja.accessory.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueMapOverview?league=" + json.settings.GetLeague(), "data/ninja.map.json");
            web.SaveString(json.settings.GetAPI() + "GetDivinationCardsOverview?league=" + json.settings.GetLeague(), "data/ninja.card.json");

            // Get the structure list
            RootStructure j = JsonConvert.DeserializeObject<RootStructure>(File.ReadAllText("structure.json", Encoding.UTF8));

            // Read the structure one by one to process gen
            // You specify a gen here by his structure name with the corresponding method.
            // I might add external DLL support in the future...
            foreach (var sec in j.structures)
            {
                if (sec.gen == true)
                {
                    switch (sec.section)
                    {
                        case "EXPENSIVE UNIQUE (GEN) (Maps)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.maps.GenMaps(sec.section);
                            break;

                        case "DIVINATION CARD (GEN)":
                            msg.CMW(string.Format("GEN: {0}",sec.section), true, 1);
                            json.cards.GenCards(sec.section);
                            break;

                        case "EXPENSIVE UNIQUE (GEN) (Weapons)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.weapons.GenWeapons(sec.section);
                            break;

                        case "EXPENSIVE UNIQUE (GEN) (Armours)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.armours.GenArmours(sec.section);
                            break;

                        case "EXPENSIVE UNIQUE (GEN) (Accessories)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.accessories.GenAccessories(sec.section);
                            break;

                        case "CHANCING (Body)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceBody.ChanceGenBody(sec.section);
                            break;

                        case "CHANCING (Boots)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceBoots.ChanceGenBoots(sec.section);
                            break;

                        case "CHANCING (Gloves)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceGloves.ChanceGenGloves(sec.section);
                            break;

                        case "CHANCING (Helmet)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceHelmet.ChanceGenHelmet(sec.section);
                            break;

                        case "CHANCING (Quiver)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceQuiver.ChanceGenQuiver(sec.section);
                            break;

                        case "CHANCING (Shield)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceShield.ChanceGenShield(sec.section);
                            break;

                        case "CHANCING (Belt)":
                            msg.CMW(string.Format("GEN: {0}", sec.section), true, 1);
                            json.chanceBelt.ChanceGenBelt(sec.section);
                            break;
                    }
                }
            }

            // Create the final filter.
            foreach (var sec in j.structures)
            {
                if (sec.gen == true)
                {
                    File.AppendAllText(@"filter\MxFilter.filter", File.ReadAllText(string.Format("gen\\{0}.filter", sec.section)));
                    File.AppendAllText(@"filter\MxFilter.filter", "" + Environment.NewLine);
                    File.AppendAllText(@"filter\MxFilter.filter", "" + Environment.NewLine);
                } else
                {
                    File.AppendAllText(@"filter\MxFilter.filter", string.Format("# Section: {0}",sec.section) + Environment.NewLine);
                    File.AppendAllText(@"filter\MxFilter.filter", "" + Environment.NewLine);
                    File.AppendAllText(@"filter\MxFilter.filter", File.ReadAllText(string.Format("structure\\{0}.filter", sec.section)));
                    File.AppendAllText(@"filter\MxFilter.filter", "" + Environment.NewLine);
                    File.AppendAllText(@"filter\MxFilter.filter", "" + Environment.NewLine);
                }
                
            }
        }
    }
}
