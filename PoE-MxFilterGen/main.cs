using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace PoE_MxFilterGen
{
    class main
    {
        private static DateTime dt = DateTime.Now;

        public static string version = "5.2.1";
        public static string fDate = string.Format("{0}-{1}-{2}", dt.Day, dt.Month, dt.Year);

        public static string section = "";
        public static string league = "";
        public static string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

        public static int fprog = 0;
        public static int ftotal = 0;

        public static int sprog = 0;
        public static int stotal = 0;

        private static string giturl = "";

        public class REMVAR
        {
            public string league { get; set; }
        }

        public class REMSND
        {
            public List<string> sound { get; set; }
        }

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
            // Check for the settings json
            if (!File.Exists("settings.json"))
            {
                msg.CMW("ERROR: settings.json not found! Downloading a template...", true, 3);
                web.DownloadFile("https://data.mxnet.xyz/poe/json/mxfiltergen_temp_settings.json", "settings.json");
            }

            web.DownloadFile("https://data.mxnet.xyz/poe/bin/mxfiltergen_updater.exe", "PoE-MxFilterGen-Updater.exe");

            msg.Splash();

            // Get current league from MxD
            var ls = web.ReadString("https://data.mxnet.xyz/poe/json/mxfiltergen_var.json");
            REMVAR lj = JsonConvert.DeserializeObject<REMVAR>(ls);
            league = lj.league;           

            msg.CMW(string.Format("GIT: {0}", json.settings.GetGIT()), true, 1);
            msg.CMW(string.Format("API: {0}", json.settings.GetAPI()), true, 1);
            msg.CMW(string.Format("League: {0}", league), true, 1);
            msg.CMW(string.Format("Confidence: {0}", json.settings.GetConfidence().ToString()), true, 1);
            msg.CMW(string.Format("Minimum Value: {0}c", json.settings.GetMinimumValue().ToString()), true, 1);
            msg.CMW(string.Format("Chancing Min. Value: {0}c", json.settings.GetChancingMinValue().ToString()), true, 1);
            msg.CMW(string.Format("Verbose: {0}", json.settings.GetVerbose().ToString()), true, 1);
            msg.CMW(string.Format("Strict: {0}", json.settings.GetStrict().ToString()), true, 1);

            giturl = json.settings.GetGIT();

            // Check for updates
            string remote_version = web.ReadString(@"https://data.mxnet.xyz/poe/txt/mxfiltergen_version.txt");
            if (version != remote_version)
            {                  
                Process.Start("PoE-MxFilterGen-Updater.exe");
                //Process.GetCurrentProcess().Kill();
            } else
            {                
                // Check if all the required dir exists
                msg.CMW($"Checking for required dirs...",true,1);
                if (!Directory.Exists(@"data\"))
                {
                    Directory.CreateDirectory(@"data\");
                }
                if (!Directory.Exists(@"gen\"))
                {
                    Directory.CreateDirectory(@"gen\");
                }
                if (!Directory.Exists(@"structure\"))
                {
                    Directory.CreateDirectory(@"structure\");
                }

                // Clean all generated data
                msg.CMW($"Cleaning the base dirs...", true, 1);
                DirectoryInfo dataDir = new DirectoryInfo(@"data\");
                DirectoryInfo genDir = new DirectoryInfo(@"gen\");
                DirectoryInfo strucDir = new DirectoryInfo(@"structure\");
                foreach (FileInfo file in dataDir.GetFiles())
                {
                    file.Delete();
                }
                foreach (FileInfo file in genDir.GetFiles())
                {
                    file.Delete();
                }
                foreach (FileInfo file in strucDir.GetFiles())
                {
                    file.Delete();
                }

                // Clean the latest generated filter from settings path
                msg.CMW($"Cleaning the last filter from path...", true, 1);
                File.Delete($@"{path}\My Games\Path of Exile\MxFilter.filter");

                // Get latest poe.ninja api
                msg.CMW($"Downloading the latest API data from poe.ninja...", true, 1);
                web.SaveString(json.settings.GetAPI() + "GetUniqueArmourOverview?league=" + league, "data/ninja.armour.json");
                web.SaveString(json.settings.GetAPI() + "GetUniqueWeaponOverview?league=" + league, "data/ninja.weapon.json");
                web.SaveString(json.settings.GetAPI() + "GetUniqueAccessoryOverview?league=" + league, "data/ninja.accessory.json");
                web.SaveString(json.settings.GetAPI() + "GetUniqueMapOverview?league=" + league, "data/ninja.map.json");
                web.SaveString(json.settings.GetAPI() + "GetDivinationCardsOverview?league=" + league, "data/ninja.card.json");

                // Setup basic variable
                string structure_name = $"{league}";
                string filter_name = "MxFilter";

                // Get the structure list
                var js = web.ReadString($@"{giturl}/PoE-MxFilter-Structure/master/{structure_name}.json");
                RootStructure j = JsonConvert.DeserializeObject<RootStructure>(js);

                // Get Theme File(s)
                web.DownloadFile($@"{giturl}/PoE-MxFilter-Structure/master/{structure_name}/Chancing.json", @"structure\Chancing.json");

                // Read the structure one by one to process gen
                // Generator (dlls) are downloaded from the web and executed in a separate AppDomain before the AD is unloaded to execute a new generator.
                // As we CAN'T unload an assembly, using AppDomains is the only way we can load/unload multiple assembly in a row.
                msg.CMW($@"Generating the filter using {j.structures.Count} source(s)...", true, 1);
                ftotal = j.structures.Count;
                foreach (var sec in j.structures)
                {
                    if (sec.gen == true)
                    {
                        fprog = fprog + 1;
                        msg.drawProgress(fprog, ftotal);
                        //msg.CMW(string.Format("REMOTE_GEN {0}", sec.section), true, 1);
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
                        fprog = fprog + 1;
                        msg.drawProgress(fprog, ftotal);
                        //msg.CMW($@"REMOTE_GET {sec.section}", true, 1);
                        web.SaveString($@"{giturl}/PoE-MxFilter-Structure/master/{structure_name}/{sec.section}.filter", $"structure/{sec.section}.filter");
                    }
                }

                // Create the final filter.
                msg.CMW($@"Creating the final filter...", true, 1);
                foreach (var sec in j.structures)
                {
                    if (sec.gen == true)
                    {
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", File.ReadAllText(string.Format("gen\\{0}.filter", sec.section)));
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", "" + Environment.NewLine);
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", "" + Environment.NewLine);
                    }
                    else
                    {
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", string.Format("# Section: {0}", sec.section) + Environment.NewLine);
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", "" + Environment.NewLine);
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", File.ReadAllText(string.Format("structure\\{0}.filter", sec.section)));
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", "" + Environment.NewLine);
                        File.AppendAllText($@"{path}\My Games\Path of Exile\{filter_name}.filter", "" + Environment.NewLine);
                    }
                }

                // Download the sounds from the remote list
                msg.CMW($@"Downloading the latest sound...", true, 1);
                var sl = web.ReadString("https://data.mxnet.xyz/poe/json/mxfiltergen_sound.json");
                REMSND slj = JsonConvert.DeserializeObject<REMSND>(sl);
                stotal = slj.sound.Count;
                foreach (string s in slj.sound)
                {
                    sprog = sprog + 1;
                    msg.drawProgress(sprog, stotal);
                    if (File.Exists($@"{path}\My Games\Path of Exile\{s}")) { File.Delete($@"{path}\My Games\Path of Exile\{s}"); }
                    web.DownloadFile($"https://data.mxnet.xyz/poe/mp3/{s}",$@"{path}\My Games\Path of Exile\{s}");
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
            Object[] ob = { json.settings.GetSection(), json.settings.GetAPI(), main.league, json.settings.GetMinimumValue(), json.settings.GetChancingMinValue(), json.settings.GetConfidence() };
            mi.Invoke(o, ob);
        }
    }
}
