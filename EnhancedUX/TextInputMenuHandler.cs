using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using SFS.UI;

namespace EnhancedUX
{
    public class TextInputMenuHandler : MonoBehaviour, IUpdateSelectedHandler, ISubmitHandler
    {
        private static TMP_InputField[] inputs;
        private static TextInputMenu menu;
        private static int index;

        public static void OnOpen(TextInputMenu inputMenu)
        {
            index = 0;
            inputs = (menu = inputMenu)
                .FieldRef<TextInputElement[]>("elements")
                .Select(e => e.textBox.TMProTextBox)
                .ToArray();
            foreach (TMP_InputField input in inputs)
            {
                TextInputMenuHandler handler = input.GetOrAddComponent<TextInputMenuHandler>();
            }
            UpdateSelected();
        }

        public static void UpdateSelected()
        {
            EventSystem.current.SetSelectedGameObject(inputs[index].gameObject);
        }

        public void OnUpdateSelected(BaseEventData eventData)
        {
            Debug.Log($"OnUpdateSelected from {index}");
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

        public void OnSubmit(BaseEventData eventData)
        {
            Debug.Log($"OnSubmit from {index}");
            menu.Confirm();
            eventData.Use();
        }
    }
}