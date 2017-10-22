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

        public static string version = "1.0.3";
        public static string release = "stable";
        public static string fDate = string.Format("{0}-{1}-{2}",dt.Day,dt.Month,dt.Year);

        static void Main(string[] args)
        {
            msg.Splash();
            msg.CMW(string.Format("API: {0}", json.settings.GetAPI()),true,1);
            msg.CMW(string.Format("League: {0}", json.settings.GetLeague()), true, 1);
            msg.CMW(string.Format("Confidence: {0}", json.settings.GetConfidence().ToString()), true, 1);
            msg.CMW(string.Format("Minimum Value: {0}c", json.settings.GetMinimumValue().ToString()), true, 1);

            // Clean all generated data
            DirectoryInfo dataDir = new DirectoryInfo(@"data\");
            DirectoryInfo genDir = new DirectoryInfo(@"gen\");
            foreach (FileInfo file in dataDir.GetFiles())
            {
                file.Delete();
            }
            foreach (FileInfo file in genDir.GetFiles())
            {
                file.Delete();
            }

            // Get latest poe.ninja api
            web.SaveString(json.settings.GetAPI() + "GetUniqueArmourOverview?league=" + json.settings.GetLeague(), "data/ninja.armour.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueWeaponOverview?league=" + json.settings.GetLeague(), "data/ninja.weapon.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueAccessoryOverview?league=" + json.settings.GetLeague(), "data/ninja.accessory.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueMapOverview?league=" + json.settings.GetLeague(), "data/ninja.map.json");
            web.SaveString(json.settings.GetAPI() + "GetDivinationCardsOverview?league=" + json.settings.GetLeague(), "data/ninja.card.json");

            msg.CMW("########## Generating Weapons  ##########",true,1);
            json.weapons.GenWeapons();
            msg.CMW("########## Generating Armours ##########", true, 1);
            json.armours.GenArmours();
            msg.CMW("########## Generating Accessories ##########", true, 1);
            json.accessories.GenAccessories();
            msg.CMW("########## Generating Maps ##########", true, 1);
            json.maps.GenMaps();
            msg.CMW("########## Generating Cards ##########", true, 1);
            json.cards.GenCards();
        }
    }
}
