using System.Collections.Generic;
using HarmonyLib;
using UITools;
using SFS.IO;
using ModLoader;

namespace EnhancedUX
{
    public class Main : Mod, IUpdatable
    {
        public static Main main;
        public override string ModNameID => "enhancedux";
        public override string DisplayName => "Enhanced UX";
        public override string Author => "Astro The Rabbit";
        public override string MinimumGameVersionNecessary => "1.6.0";
        public override string ModVersion => "1.0";
        public override string Description => "A PC-focused quality of life mod.";

        public override Dictionary<string, string> Dependencies { get; } = new Dictionary<string, string>
        {
            { "UITools", "1.1.5" }
        };
        public Dictionary<string, FilePath> UpdatableFiles => new Dictionary<string, FilePath>()
        {
            {
                "https://github.com/AstroTheRabbit/Enhanced-UX-Mod-SFS/releases/latest/download/EnhancedUX.dll",
                new FolderPath(ModFolder).ExtendToFile("EnhancedUX.dll")
            }
        };

        public override void Early_Load()
        {
            new Harmony(ModNameID).PatchAll();
            main = this;
        }

        public override void Load()
        {
            Settings.Init();
            KeyBindings.Init();
        }
    }
}
