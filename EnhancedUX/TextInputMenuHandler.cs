using System.Linq;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using SFS.UI;

namespace EnhancedUX
{
    public class TextInputMenuHandler : MonoBehaviour, IUpdateSelectedHandler
    {
        private static int index;
        private static TextInputMenu menu;
        private static TMP_InputField[] inputs;

        public static void OnOpen(TextInputMenu inputMenu)
        {
            index = 0;
            menu = inputMenu;
            inputs = menu
                .FieldRef<TextInputElement[]>("elements")
                .Select(e => e.textBox.TMProTextBox)
                .ToArray();
            foreach (TMP_InputField input in inputs)
            {
                input.onSubmit.AddListener(OnSubmit);
                input.GetOrAddComponent<TextInputMenuHandler>();
            }
            menu.StartCoroutine(SelectFirstNextFrame());
        }

        private static IEnumerator SelectFirstNextFrame()
        {
            yield return new WaitForEndOfFrame();
            UpdateSelected();
        }

        public static void UpdateSelected()
        {
            inputs[index].Select();
        }

        private static void OnSubmit(string _)
        {
            menu.Confirm();
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
                {
                    index = (index - 1 + inputs.Length) % inputs.Length;
                }
                else
                {
                    index = (index + 1) % inputs.Length;
                }
                UpdateSelected();
                eventData.Use();
            }
        }
    }
}