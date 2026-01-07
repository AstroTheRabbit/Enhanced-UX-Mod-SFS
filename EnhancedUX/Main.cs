using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;
using UITools;
using SFS.IO;
using ModLoader;

namespace EnhancedUX
{
    public class Main : Mod // ! , IUpdatable
    {
        public static Main main;
        public override string ModNameID => "enhanceduX";
        public override string DisplayName => "Enhanced UX";
        public override string Author => "Astro The Rabbit";
        public override string MinimumGameVersionNecessary => "1.6.0";
        public override string ModVersion => "1.0";
        public override string Description => "Various PC-focused improvements to SFS's UI and other systems.";

        public override Dictionary<string, string> Dependencies { get; } = new Dictionary<string, string>
        {
            { "UITools", "1.1.5" }
        };
        public Dictionary<string, FilePath> UpdatableFiles => new Dictionary<string, FilePath>()
        {
            {
                "https://github.com/AstroTheRabbit/Enhanced-UI-Mod-SFS/releases/latest/download/EnhancedUI.dll",
                new FolderPath(ModFolder).ExtendToFile("EnhancedUI.dll")
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
        }
    }
}
