using System;
using System.Collections;
using UnityEngine;
using SFS.UI;
using SFS.Input;

namespace EnhancedUX
{
    public class AskOverwriteMenuHandler : MonoBehaviour
    {
        private static AskOverwriteMenuHandler handler = null;
        private static Action action_overwrite = null;
        private static Action action_new = null;

        public static void OnOpen(ButtonBuilder overwriteButton, ButtonBuilder newButton, ButtonBuilder cancelButton)
        {
            if (Settings.settings.AskOverwriteMenu_EscapeForCancel)
            {
                Patches.MenuGeneratorImprovements.PushCloseMode(CloseMode.Current);
            }

            if (handler == null)
            {
                handler = new GameObject("Enhanced UX - Ask Overwrite Menu Handler").AddComponent<AskOverwriteMenuHandler>();
            }

            // * Delay the assignment of the overwrite/new buttons so that a player pressing enter in
            // * a `TextInputMenu` input doesn't also use the overwrite/new keybind on the same frame.

            void AddClear(Button button) => button.onClick += Clear;
            void AddAssign(Button button, string field) => handler.StartCoroutine(AssignDelayed(button, field));

            overwriteButton.CustomizeButton
            (
                button =>
                {
                    AddClear(button);
                    AddAssign(button, nameof(action_overwrite));
                }
            );
            newButton.CustomizeButton
            (
                button =>
                {
                    AddClear(button);
                    AddAssign(button, nameof(action_new));
                }
            );
            cancelButton.CustomizeButton(AddClear);
        }

        private static IEnumerator AssignDelayed(Button button, string field)
        {
            yield return new WaitForSecondsRealtime(0.5f);
            handler.FieldRef<Action>(field) = () => button.onClick.Invoke(null);
        }

        public static void Clear()
        {
            action_overwrite = null;
            action_new = null;
        }

        public static void Invoke_Overwrite()
        {
            if (Settings.settings.AskOverwriteMenu_KeybindsForConfirmation)
            {
                action_overwrite?.Invoke();
            }
        }
        public static void Invoke_New()
        {
            if (Settings.settings.AskOverwriteMenu_KeybindsForConfirmation)
            {
                action_new?.Invoke();
            }
        }
    }
}