using UnityEngine;
using ModLoader;
using static SFS.Input.KeybindingsPC;

namespace EnhancedUX
{
    public class KeyBindings : ModKeybindings
    {
        private static KeyBindings main;

        public Key AskOverwriteMenu_Overwrite { get; set; } = KeyCode.Return;
        public Key AskOverwriteMenu_New { get; set; } = Key.Ctrl_(KeyCode.Return);

        public static void Init()
        {
            main = SetupKeybindings<KeyBindings>(Main.main);
            
            AddOnKeyDown(main.AskOverwriteMenu_Overwrite, AskOverwriteMenuHandler.Invoke_Overwrite);
            AddOnKeyDown(main.AskOverwriteMenu_New, AskOverwriteMenuHandler.Invoke_New);
        }

        public override void CreateUI()
        {
            KeyBindings defaults = new KeyBindings();
            
            CreateUI_Space();
            CreateUI_Text("Enhanced UX");

            CreateUI_Text("Ask Overwrite Menu");
            CreateUI_Keybinding(AskOverwriteMenu_Overwrite, defaults.AskOverwriteMenu_Overwrite, "Overwrite Save");
            CreateUI_Keybinding(AskOverwriteMenu_New, defaults.AskOverwriteMenu_New, "New Save");
        }
    }
}