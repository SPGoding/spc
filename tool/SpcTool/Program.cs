using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace SpcTool
{
    class Program
    {
        private static Config config;
        private static readonly Dictionary<string, List<string>> jar1 = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, List<string>> jar2 = new Dictionary<string, List<string>>();
        private static readonly Dictionary<string, string> result = new Dictionary<string, string>();
        static void Main(string[] args)
        {
            initConsole();
            var time0 = 0;
            var time1 = 0;
            var time2 = 0;
            var time3 = 0;
            try
            {
                time0 = Environment.TickCount;

                config = readConfig(args);
                checkArgs(config);

                Console.WriteLine("Mapping Jar1...");
                mapFiles(jar1, Path.Combine(config.Jar1Path, @"assets\minecraft\textures\"), config.Jar1Path, 1);
                Console.WriteLine("Mapping Jar2...");
                mapFiles(jar2, Path.Combine(config.Jar2Path, @"assets\minecraft\textures\"), config.Jar2Path, 2);
                time1 = Environment.TickCount;

                Console.WriteLine("Matching Files...");
                matchFiles(result, jar1, jar2);
                time2 = Environment.TickCount;

                Console.WriteLine("Writing Files...");
                writeFiles(result, config.Type, config.OutputPath);
                time3 = Environment.TickCount;

                Console.WriteLine("========= Time Cost =========");
                Console.WriteLine($"Mapping:  {time1 - time0}ms");
                Console.WriteLine($"Matching: {time2 - time1}ms");
                Console.WriteLine($"Writing:  {time3 - time2}ms");
                Console.WriteLine($"Total:    {time3 - time0}ms");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine(e.StackTrace);
            }
            finally
            {
                Console.WriteLine("Press any key to exit...");
                Console.Read();
            }
        }

        private static void initConsole()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "SPC Tool";
            Console.WriteLine("======SPC Tool by SPGoding======");
        }

        private static Config readConfig(string[] args)
        {
            var arg = new Config
            {
                DetailMode = args[0] == "true",
                // DetailMode = true,
                Type = args[1],
                Jar1Path = args[2],
                Jar2Path = args[3]
            };
            
            if (arg.Jar1Path.ElementAt(arg.Jar1Path.Length - 1) != '\\')
            {
                arg.Jar1Path += @"\";
            }
            if (arg.Jar2Path.ElementAt(arg.Jar2Path.Length - 1) != '\\')
            {
                arg.Jar2Path += @"\";
            }

            if (args.Length == 4)
            {
                arg.OutputPath = AppDomain.CurrentDomain.BaseDirectory + @"Output\";
            }
            else if (args.Length == 5)
            {
                arg.OutputPath = args[4];
            }
            else
            {
                throw new ArgumentException("Unexpected args length.", "type");
            }

            return arg;
        }

        private static void checkArgs(Config conf)
        {
            if (!(new [] { "bb", "ts", "both" }).Contains(conf.Type)) {
                throw new ArgumentException($"Unexpected type: '{conf.Type}'.", "type");
            }
            else if (!Directory.Exists(conf.Jar1Path))
            {
                throw new ArgumentException($"Illegel directory path: '{conf.Jar1Path}'.", "Jar Path 1");
            }
            else if (!Directory.Exists(conf.Jar2Path))
            {
                throw new ArgumentException($"Illegel directory path: '{conf.Jar2Path}'.", "Jar Path 2");
            }
            else if (!Directory.Exists(conf.OutputPath))
            {
                Directory.CreateDirectory(conf.OutputPath);
            }
        }

        //private static void extractJars()
        //{
        //    Console.WriteLine("====== Extracting Jars ======");

        //    var target = Path.Combine(arg.OutputPath, "Extracts");
        //    Console.WriteLine($"Set target: '{target}'");

        //    Console.WriteLine($"Extracting: '{arg.Jar1Path}'");
        //    ZipFile.ExtractToDirectory(arg.Jar1Path, Path.Combine(target, "Jar1"));
        //    Console.WriteLine($"Extracted: '{arg.Jar1Path}'");

        //    Console.WriteLine($"Extracting: '{arg.Jar2Path}'");
        //    ZipFile.ExtractToDirectory(arg.Jar2Path, Path.Combine(target, "Jar2"));
        //    Console.WriteLine($"Extracted: '{arg.Jar2Path}'");

        //    Console.WriteLine("====== Finished ======");
        //}

        private static void mapFiles(Dictionary<string, List<string>> dict, string path, string prefix, int jarID)
        {
            foreach (var file in Directory.GetFiles(path))
            {
                var md5 = getMD5(file);
                var name = file.Replace(prefix, "").Replace(@"\", "/");
                if (config.DetailMode)
                {
                    Console.WriteLine($"Mapping: '{md5}' => '{name}'.");
                }
                if (!dict.ContainsKey(md5))
                {
                    var list = new List<string>
                    {
                        name
                    };
                    dict.Add(md5, list);
                }
                else
                {
                    dict[md5].Add(name);

                    if ((jarID == 1 && !specialCases.ContainsKey(name))) {
                        if (dict[md5].Count == 2)
                        {
                            Console.WriteLine("Same MD5. Please add them into 'specialCases'.");
                            Console.WriteLine(dict[md5][0]);
                        }
                        Console.WriteLine(name);
                    }
                }
            }
            foreach (var dir in Directory.GetDirectories(path))
            {
                mapFiles(dict, Path.Combine(path, dir), prefix, jarID);
            }
        }
        
        private static string getMD5(string file)
        {
            var fs = new FileStream(file, FileMode.Open);
            var md5 = new MD5CryptoServiceProvider();
            var retVal = md5.ComputeHash(fs);
            fs.Close();
            var sb = new StringBuilder();
            for (int i = 0; i < retVal.Length; i++)
            {
                sb.Append(retVal[i].ToString("x2"));
            }
            return sb.ToString();
        }

        private static void matchFiles(Dictionary<string, string> result, Dictionary<string, List<string>> jar1, Dictionary<string, List<string>> jar2)
        {
            foreach (var md5 in jar1.Keys)
            {
                var files1 = jar1[md5];
                if (jar2.ContainsKey(md5))
                {
                    var file2 = jar2[md5][0];
                    foreach (var file1 in files1)
                    {
                        if (config.DetailMode)
                        {
                            Console.WriteLine($"Matching: '{file1}' => '{file2}'.");
                        }
                        //result.Add(file1, file2);
                    }
                }
                else
                {
                    foreach (var file1 in files1)
                    {
                        if (config.DetailMode)
                        {
                            Console.WriteLine($"Matching: '{file1}' => 'NULL'.");
                        }
                        result.Add(file1, "NULL");
                    }
                }
            }
            var cnt = 0;
            foreach (var md5 in jar2.Keys)
            {
                if (!result.ContainsValue(jar2[md5][0]))
                {
                    var files2 = jar2[md5];
                    foreach (var file2 in files2)
                    {
                        if (config.DetailMode)
                        {
                            Console.WriteLine($"Matching: 'NULL' => '{file2}'.");
                        }
                        result.Add($"NULL{cnt++}", file2);
                    }
                }
            }
        }

        private static void writeFiles(Dictionary<string, string> result, string type, string outputPath)
        {
            if (type == "both" || type == "bb")
            {
                var bbcode = "[table]\n";
                foreach (var file1 in result.Keys)
                {
                    bbcode += $"[tr][td]{file1}[/td][td]{result[file1]}[/td][/tr]\n";
                }
                bbcode += "[/table]";
                Console.WriteLine($"Writing to {Path.Combine(outputPath, "bbcode.txt")}");
                File.WriteAllText(Path.Combine(outputPath, "bbcode.txt"), bbcode);
            }
            if (type == "both" || type == "ts")
            {
                var ts = "let source = new Map([\n";
                foreach (var file1 in result.Keys)
                {
                    ts += $@"    [
        '{file1}',
        '{result[file1]}'
    ],
";
                }
                ts += "])";
                Console.WriteLine($"Writing to {Path.Combine(outputPath, "source.ts")}");
                File.WriteAllText(Path.Combine(outputPath, "source.ts"), ts);
            }
        }

        private static readonly Dictionary<string, string> specialCases =  new Dictionary<string, string>() {
            { "assets/minecraft/textures/blocks/beetroots_stage_0.png", "assets/minecraft/textures/block/beetroots_stage0.png" },
            { "assets/minecraft/textures/blocks/carrots_stage_0.png", "assets/minecraft/textures/block/carrots_stage0.png" },
            { "assets/minecraft/textures/blocks/beetroots_stage_1.png", "assets/minecraft/textures/block/beetroots_stage1.png" },
            { "assets/minecraft/textures/blocks/carrots_stage_1.png", "assets/minecraft/textures/block/carrots_stage1.png" },
            { "assets/minecraft/textures/blocks/beetroots_stage_2.png", "assets/minecraft/textures/block/beetroots_stage2.png" },
            { "assets/minecraft/textures/blocks/carrots_stage_2.png", "assets/minecraft/textures/block/carrots_stage2.png" },
            { "assets/minecraft/textures/blocks/chain_command_block_back.png.mcmeta", "assets/minecraft/textures/block/chain_command_block_back.png.mcmeta" },
            { "assets/minecraft/textures/blocks/chain_command_block_conditional.png.mcmeta", "assets/minecraft/textures/block/chain_command_block_conditional.png.mcmeta" },
            { "assets/minecraft/textures/blocks/chain_command_block_front.png.mcmeta", "assets/minecraft/textures/block/chain_command_block_front.png.mcmeta" },
            { "assets/minecraft/textures/blocks/chain_command_block_side.png.mcmeta", "assets/minecraft/textures/block/chain_command_block_side.png.mcmeta" },
            { "assets/minecraft/textures/blocks/command_block_back.png.mcmeta", "assets/minecraft/textures/block/command_block_back.png.mcmeta" },
            { "assets/minecraft/textures/blocks/command_block_conditional.png.mcmeta", "assets/minecraft/textures/block/command_block_conditional.png.mcmeta" },
            { "assets/minecraft/textures/blocks/command_block_front.png.mcmeta", "assets/minecraft/textures/block/command_block_front.png.mcmeta" },
            { "assets/minecraft/textures/blocks/command_block_side.png.mcmeta", "assets/minecraft/textures/block/command_block_side.png.mcmeta" },
            { "assets/minecraft/textures/blocks/cauldron_inner.png", "assets/minecraft/textures/block/cauldron_inner.png" },
            { "assets/minecraft/textures/blocks/hopper_inside.png", "assets/minecraft/textures/block/hopper_inside.png" },
            { "assets/minecraft/textures/blocks/frosted_ice_0.png", "assets/minecraft/textures/block/frosted_ice_0.png" },
            { "assets/minecraft/textures/blocks/ice.png", "assets/minecraft/textures/block/ice.png" },
            { "assets/minecraft/textures/blocks/leaves_big_oak.png", "assets/minecraft/textures/block/dark_oak_leaves.png" },
            { "assets/minecraft/textures/blocks/leaves_birch.png", "assets/minecraft/textures/block/birch_leaves.png" },
            { "assets/minecraft/textures/blocks/leaves_oak.png", "assets/minecraft/textures/block/oak_leaves.png" },
            { "assets/minecraft/textures/blocks/jukebox_side.png", "assets/minecraft/textures/block/jukebox_side.png" },
            { "assets/minecraft/textures/blocks/noteblock.png", "assets/minecraft/textures/block/note_block.png" },
            { "assets/minecraft/textures/blocks/furnace_top.png", "assets/minecraft/textures/block/furnace_top.png" },
            { "assets/minecraft/textures/blocks/piston_bottom.png", "assets/minecraft/textures/block/piston_bottom.png" },
            { "assets/minecraft/textures/blocks/fire_layer_1.png.mcmeta", "assets/minecraft/textures/block/fire_1.png.mcmeta" },
            { "assets/minecraft/textures/blocks/portal.png.mcmeta", "assets/minecraft/textures/block/nether_portal.png.mcmeta" },
            { "assets/minecraft/textures/blocks/potatoes_stage_0.png", "assets/minecraft/textures/block/potatoes_stage0.png" },
            { "assets/minecraft/textures/blocks/potatoes_stage_1.png", "assets/minecraft/textures/block/potatoes_stage1.png" },
            { "assets/minecraft/textures/blocks/potatoes_stage_2.png", "assets/minecraft/textures/block/potatoes_stage2.png" },
            { "assets/minecraft/textures/blocks/melon_stem_connected.png", "assets/minecraft/textures/block/attached_melon_stem.png" },
            { "assets/minecraft/textures/blocks/pumpkin_stem_connected.png", "assets/minecraft/textures/block/attached_pumpkin_stem.png" },
            { "assets/minecraft/textures/blocks/melon_stem_disconnected.png", "assets/minecraft/textures/block/melon_stem.png" },
            { "assets/minecraft/textures/blocks/pumpkin_stem_disconnected.png", "assets/minecraft/textures/block/pumpkin_stem.png" },
            { "assets/minecraft/textures/blocks/quartz_block_side.png", "assets/minecraft/textures/block/quartz_block_side.png" },
            { "assets/minecraft/textures/blocks/quartz_block_top.png", "assets/minecraft/textures/block/quartz_block_top.png" },
            { "assets/minecraft/textures/blocks/repeating_command_block_back.png.mcmeta", "assets/minecraft/textures/block/repeating_command_block_back.png.mcmeta" },
            { "assets/minecraft/textures/blocks/repeating_command_block_conditional.png.mcmeta", "assets/minecraft/textures/block/repeating_command_block_conditional.png.mcmeta" },
            { "assets/minecraft/textures/blocks/repeating_command_block_front.png.mcmeta", "assets/minecraft/textures/block/repeating_command_block_front.png.mcmeta" },
            { "assets/minecraft/textures/blocks/repeating_command_block_side.png.mcmeta", "assets/minecraft/textures/block/repeating_command_block_side.png.mcmeta" },
            { "assets/minecraft/textures/blocks/water_flow.png.mcmeta", "assets/minecraft/textures/block/water_flow.png.mcmeta" },
            { "assets/minecraft/textures/entity/projectiles/arrow.png", "assets/minecraft/textures/entity/projectiles/arrow.png" },
            { "assets/minecraft/textures/entity/projectiles/tipped_arrow.png", "assets/minecraft/textures/entity/projectiles/tipped_arrow.png" },
            { "assets/minecraft/textures/blocks/sandstone_top.png", "assets/minecraft/textures/block/sandstone_top.png" },
            { "assets/minecraft/textures/gui/advancements/backgrounds/adventure.png", "assets/minecraft/textures/gui/advancements/backgrounds/adventure.png" },
            { "assets/minecraft/textures/blocks/end_stone.png", "assets/minecraft/textures/block/end_stone.png" },
            { "assets/minecraft/textures/gui/advancements/backgrounds/end.png", "assets/minecraft/textures/gui/advancements/backgrounds/end.png" },
            { "assets/minecraft/textures/blocks/farmland_dry.png", "assets/minecraft/textures/block/farmland.png" },
            { "assets/minecraft/textures/gui/advancements/backgrounds/husbandry.png", "assets/minecraft/textures/gui/advancements/backgrounds/husbandry.png" },
            { "assets/minecraft/textures/items/compass_07.png", "assets/minecraft/textures/item/compass_07.png" },
            { "assets/minecraft/textures/items/compass_08.png", "assets/minecraft/textures/item/compass_08.png" },
            { "assets/minecraft/textures/items/compass_09.png", "assets/minecraft/textures/item/compass_09.png" },
            { "assets/minecraft/textures/items/compass_23.png", "assets/minecraft/textures/item/compass_23.png" },
            { "assets/minecraft/textures/items/compass_24.png", "assets/minecraft/textures/item/compass_24.png" },
            { "assets/minecraft/textures/items/compass_25.png", "assets/minecraft/textures/item/compass_25.png" },
            { "assets/minecraft/textures/items/potion_bottle_drinkable.png", "assets/minecraft/textures/item/potion.png" },
            { "assets/minecraft/textures/items/potion_bottle_empty.png", "assets/minecraft/textures/item/glass_bottle.png" },
            { "assets/minecraft/textures/misc/unknown_pack.png", "assets/minecraft/textures/misc/unknown_pack.png" },
            { "assets/minecraft/textures/misc/unknown_server.png", "assets/minecraft/textures/misc/unknown_server.png" },
            { "assets/minecraft/textures/misc/pumpkinblur.png.mcmeta", "assets/minecraft/textures/misc/pumpkinblur.png.mcmeta" },
            { "assets/minecraft/textures/misc/vignette.png.mcmeta", "assets/minecraft/textures/misc/vignette.png.mcmeta" },
        };
    }

    struct Config
    {
        public string Type;
        public string Jar1Path;
        public string Jar2Path;
        public string OutputPath;
        public bool DetailMode;
    }
}
