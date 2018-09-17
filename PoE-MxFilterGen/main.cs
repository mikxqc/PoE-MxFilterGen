using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace PoE_MxFilterGen
{
    class main
    {
        private static DateTime dt = DateTime.Now;

        public static string version = "4.3.0";
        public static string fDate = string.Format("{0}-{1}-{2}", dt.Day, dt.Month, dt.Year);

        public static string section = "";

        private static string giturl = json.settings.GetGIT();

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
            msg.CMW(string.Format("GIT: {0}", json.settings.GetGIT()), true, 1);
            msg.CMW(string.Format("API: {0}", json.settings.GetAPI()), true, 1);
            msg.CMW(string.Format("League: {0}", json.settings.GetLeague()), true, 1);
            msg.CMW(string.Format("Confidence: {0}", json.settings.GetConfidence().ToString()), true, 1);
            msg.CMW(string.Format("Minimum Value: {0}c", json.settings.GetMinimumValue().ToString()), true, 1);
            msg.CMW(string.Format("Chancing Min. Value: {0}c", json.settings.GetChancingMinValue().ToString()), true, 1);
            msg.CMW(string.Format("Verbose: {0}", json.settings.GetVerbose().ToString()), true, 1);
            msg.CMW(string.Format("Strict: {0}", json.settings.GetStrict().ToString()), true, 1);

            // Check if all the required dir exists
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
            if (!Directory.Exists(@"structure\"))
            {
                Directory.CreateDirectory(@"structure\");
            }

            // Clean all generated data
            DirectoryInfo dataDir = new DirectoryInfo(@"data\");
            DirectoryInfo genDir = new DirectoryInfo(@"gen\");
            DirectoryInfo filterDir = new DirectoryInfo(@"filter\");
            DirectoryInfo strucDir = new DirectoryInfo(@"structure\");
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
            foreach (FileInfo file in strucDir.GetFiles())
            {
                file.Delete();
            }

            // Get latest poe.ninja api
            web.SaveString(json.settings.GetAPI() + "GetUniqueArmourOverview?league=" + json.settings.GetLeague(), "data/ninja.armour.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueWeaponOverview?league=" + json.settings.GetLeague(), "data/ninja.weapon.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueAccessoryOverview?league=" + json.settings.GetLeague(), "data/ninja.accessory.json");
            web.SaveString(json.settings.GetAPI() + "GetUniqueMapOverview?league=" + json.settings.GetLeague(), "data/ninja.map.json");
            web.SaveString(json.settings.GetAPI() + "GetDivinationCardsOverview?league=" + json.settings.GetLeague(), "data/ninja.card.json");

            // Setup basic variable
            string structure_name = $"{json.settings.GetLeague()}";
            string filter_name = "MxFilter";

            // Get the structure list
            var js = web.ReadString($@"{giturl}/PoE-MxFilter-Structure/master/{structure_name}.json");
            RootStructure j = JsonConvert.DeserializeObject<RootStructure>(js);

            // Read the structure one by one to process gen
            // Generator (dlls) are downloaded from the web and executed in a separate AppDomain before the AD is unloaded to execute a new generator.
            // As we CAN'T unload an assembly, using AppDomains is the only way we can load/unload multiple assembly in a row.
            foreach (var sec in j.structures)
            {
                if (sec.gen == true)
                {
                    msg.CMW(string.Format("REMOTE_GEN {0}", sec.section), true, 1);
                    web.DownloadFile($@"{giturl}/PoE-MxFilter-Structure/master/{structure_name}/{sec.section}.dll", $@"structure\{sec.section}.dll");
                    json.settings.WriteSection(sec.section);
                    var bytes = GenerateAssemblyAndGetRawBytes(sec.section);

                    var appDomain = AppDomain.CreateDomain(sec.section, null, new AppDomainSetup
                    {
                        ShadowCopyFiles = "true",
                        LoaderOptimization = LoaderOptimization.MultiDomainHost
                    });

                    var assmblyLoaderType = typeof(AssmeblyLoader);
                    var assemblyLoader = (IAssemblyLoader)appDomain.CreateInstanceFromAndUnwrap(assmblyLoaderType.Assembly.Location, assmblyLoaderType.FullName);
                    assemblyLoader.Load(bytes);

                    AppDomain.Unload(appDomain);
                }
                else
                {
                    msg.CMW($@"REMOTE_GET {sec.section}", true, 1);
                    web.SaveString($@"{giturl}/PoE-MxFilter-Structure/master/{structure_name}/{sec.section}.filter", $"structure/{sec.section}.filter");
                }
            }

            // Create the final filter.
            foreach (var sec in j.structures)
            {
                if (sec.gen == true)
                {
                    File.AppendAllText($@"filter\{filter_name}.filter", File.ReadAllText(string.Format("gen\\{0}.filter", sec.section)));
                    File.AppendAllText($@"filter\{filter_name}.filter", "" + Environment.NewLine);
                    File.AppendAllText($@"filter\{filter_name}.filter", "" + Environment.NewLine);
                }
                else
                {
                    File.AppendAllText($@"filter\{filter_name}.filter", string.Format("# Section: {0}", sec.section) + Environment.NewLine);
                    File.AppendAllText($@"filter\{filter_name}.filter", "" + Environment.NewLine);
                    File.AppendAllText($@"filter\{filter_name}.filter", File.ReadAllText(string.Format("structure\\{0}.filter", sec.section)));
                    File.AppendAllText($@"filter\{filter_name}.filter", "" + Environment.NewLine);
                    File.AppendAllText($@"filter\{filter_name}.filter", "" + Environment.NewLine);
                }
            }

            // Clean all generated data
            foreach (FileInfo file in genDir.GetFiles())
            {
                file.Delete();
            }
            foreach (FileInfo file in strucDir.GetFiles())
            {
                file.Delete();
            }
        }

        private static byte[] GenerateAssemblyAndGetRawBytes(string dll)
        {
            string lp = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);
            var path = $@"{lp}\structure\{dll}.dll";
            return File.ReadAllBytes(path);
        }
    }

    public interface IAssemblyLoader
    {
        void Load(byte[] bytes);
    }

    public class AssmeblyLoader : MarshalByRefObject, IAssemblyLoader
    {
        public void Load(byte[] bytes)
        {
            string section = main.section;
            var assembly = AppDomain.CurrentDomain.Load(bytes);
            Type type = AppDomain.CurrentDomain.GetAssemblies().SelectMany(t => t.GetTypes()).Where(t => String.Equals(t.Name, "Generator", StringComparison.Ordinal)).First();
            object o = Activator.CreateInstance(type);
            MethodInfo mi = o.GetType().GetMethod("Gen");
            Object[] ob = { json.settings.GetSection(), json.settings.GetAPI(), json.settings.GetLeague(), json.settings.GetMinimumValue(), json.settings.GetChancingMinValue(), json.settings.GetConfidence() };
            mi.Invoke(o, ob);
        }
    }
}
