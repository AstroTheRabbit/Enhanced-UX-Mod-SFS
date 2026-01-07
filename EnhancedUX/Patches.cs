using System;
using System.Reflection;
using System.Collections.Generic;
using HarmonyLib;
using SFS.Input;
using SFS.UI;
using SFS.UI.ModGUI;
using Type = System.Type;
using UnityEngine.UI;
using System.Reflection.Emit;

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
            public static bool Prefix()
            {
                return !TextInputSelected;
            }
        }
    }

    public static class TextInputMenuImprovements
    {
        [
            HarmonyPatch
            (
                typeof(TextInputMenu),
                nameof(TextInputMenu.Open),
                new Type[]
                {
                    typeof(string),
                    typeof(string),
                    typeof(Action<string[]>),
                    typeof(CloseMode),
                    typeof(CloseMode),
                    typeof(TextInputElement[]),
                }
            )
        ]
        public static class TextInputMenu_Open
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                MethodInfo info = AccessTools.Method(typeof(Screen_Menu), nameof(Screen_Menu.Open));

                foreach (CodeInstruction code in instructions)
                {
                    yield return code;
                    if (code.Calls(info))
                    {
                        yield return new CodeInstruction(OpCodes.Ldarg_0);
                        yield return CodeInstruction.Call(typeof(TextInputMenuHandler), nameof(TextInputMenuHandler.OnOpen));
                        yield return new CodeInstruction(OpCodes.Ret);
                        break;
                    }
                }
            }
        }
    }
}