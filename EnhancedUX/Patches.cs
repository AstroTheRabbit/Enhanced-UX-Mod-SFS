using HarmonyLib;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;

namespace EnhancedUX.Patches
{
    public static class PreventProcessingForTextInputs
    {
        public static bool TextInputSelected { get; set; } = false;

        [HarmonyPatch(typeof(TextInput), nameof(TextInput.Init))]
        public static class TextInput_Init
        {
            public static void Postfix(TextInput __instance)
            {
                __instance.field.onSelect.AddListener(_ => TextInputSelected = true);
                __instance.field.onDeselect.AddListener(_ => TextInputSelected = false);
            }
        }

        [HarmonyPatch(typeof(Screen_Game), nameof(Screen_Game.ProcessInput))]
        public static class Screen_Game_ProcessInput
        {
            public static bool Prefix(Screen_Game __instance)
            {
                return !TextInputSelected;
            }
        }
    }

    public static class TextInputMenuImprovements
    {
        [HarmonyPatch(typeof(Screen_Menu), nameof(Screen_Menu.Open))]
        public static class Screen_Menu_Open
        {
            public static void Postfix(Screen_Menu __instance)
            {
                if (__instance is TextInputMenu menu)
                {
                    TextInputMenuHandler.OnOpen(menu);
                }
            }
        }
    }
}