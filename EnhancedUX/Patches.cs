using System;
using System.Reflection;
using System.Reflection.Emit;
using System.Collections.Generic;
using HarmonyLib;
using SFS.UI;
using SFS.Input;
using SFS.UI.ModGUI;
using Type = System.Type;

namespace EnhancedUX.Patches
{
    public static class StopProcessingForTextInputs
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
                if (Settings.settings.StopProcessingForTextInputs)
                {
                    return !TextInputSelected;
                }
                else
                {
                    return true;
                }
            }
        }
    }

    public static class MenuGeneratorImprovements
    {
        private static Stack<CloseMode> closeModes = new Stack<CloseMode>();
        public static void PushCloseMode(CloseMode mode) => closeModes.Push(mode);
        private static CloseMode PopCloseMode() => closeModes.Count > 0 ? closeModes.Pop() : CloseMode.None;

        [HarmonyPatch(typeof(MenuGenerator), nameof(MenuGenerator.ShowChoices))]
        public static class MenuGenerator_ShowChoices
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
            {
                bool found_ldc_1 = false;
                bool found_ldc_2 = false;

                foreach (CodeInstruction code in instructions)
                {
                    if (!found_ldc_1)
                    {
                        if (code.opcode == OpCodes.Ldc_I4_0)
                        {
                            found_ldc_1 = true;
                        }
                        yield return code;
                    }
                    else if (!found_ldc_2)
                    {
                        if (code.opcode == OpCodes.Ldc_I4_0)
                        {
                            found_ldc_2 = true;
                            yield return CodeInstruction.Call(typeof(MenuGeneratorImprovements), nameof(PopCloseMode));
                        }
                        else
                        {
                            yield return code;
                        }
                    }
                    else
                    {
                        yield return code;
                    }
                }
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

    public static class AskOverwriteMenuImprovements
    {
        [HarmonyPatch(typeof(MenuGenerator), nameof(MenuGenerator.AskOverwrite))]
        public static class MenuGenerator_AskOverwrite
        {
            public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
            {
                LocalBuilder local = generator.DeclareLocal(typeof(ButtonBuilder[]));
                MethodInfo info = AccessTools.Method(typeof(MenuGenerator), nameof(MenuGenerator.ShowChoices));

                bool found_newarr = false;
                bool found_call = false;

                foreach (CodeInstruction code in instructions)
                {
                    if (!found_newarr)
                    {
                        yield return code;
                        if (code.opcode == OpCodes.Newarr)
                        {
                            found_newarr = true;
                            // * Store reference to newly-created `ButtonBuilder[]`.
                            yield return new CodeInstruction(OpCodes.Dup);
                            yield return new CodeInstruction(OpCodes.Stloc, local);
                        }
                    }
                    else if (!found_call)
                    {
                        if (code.Calls(info))
                        {
                            found_call = true;
                            // * Pass the elements of `ButtonBuilder[]` through to `AskOverwriteMenuHandler` via `OnOpen`.
                            yield return new CodeInstruction(OpCodes.Ldloc, local);
                            yield return CodeInstruction.Call(typeof(MenuGenerator_AskOverwrite), nameof(OnOpen));
                        }
                        yield return code;
                    }
                    else
                    {
                        yield return code;
                    }
                }
            }

            private static void OnOpen(ButtonBuilder[] array)
            {
                AskOverwriteMenuHandler.OnOpen
                (
                    overwriteButton: array[0],
                    newButton: array[1],
                    cancelButton: array[2]
                );
            }
        }
    }
}