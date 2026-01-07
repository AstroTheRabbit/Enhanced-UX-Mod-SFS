using System;
using UnityEngine;
using UITools;
using SFS.IO;
using SFS.UI.ModGUI;
using LayoutType = SFS.UI.ModGUI.Type;

namespace EnhancedUX
{
    public class Settings : ModSettings<Settings.Data>
    {
        public static Settings main;
        protected override FilePath SettingsFile => new FolderPath(Main.main.ModFolder).ExtendToFile("Settings.txt");

        public static void Init()
        {
            main = new Settings();
            main.Initialize();
            main.AddUI();
        }

        private void AddUI()
        {
            ConfigurationMenu.Add
            (
                null,
                new (string, Func<Transform, GameObject>)[]
                {
                    ("Enhanced UX", CreateUI)
                }
            );
        }

        private GameObject CreateUI(Transform parent)
        {
            Vector2Int size = ConfigurationMenu.ContentSize;
            Box box = Builder.CreateBox(parent, size.x, size.y);
            box.CreateLayoutGroup(LayoutType.Vertical, TextAnchor.UpperLeft, padding: new RectOffset(15, 15, 15, 15));
            int width = size.x - 30;

            void CreateToggle(string name, Func<bool> getter, Action<bool> setter)
            {
                ToggleWithLabel toggle = Builder.CreateToggleWithLabel
                (
                    box,
                    width,
                    50,
                    getter,
                    () => setter(!getter()),
                    labelText: name
                );
                toggle.label.AutoFontResize = false;
                toggle.label.FontSize = 30;
            }

            Builder.CreateLabel
            (
                box,
                width,
                50,
                text: "General"
            );
            CreateToggle
            (
                "Stop Processing For Text Inputs",
                () => settings.StopProcessingForTextInputs,
                v => settings.StopProcessingForTextInputs = v
            );
            
            Builder.CreateLabel
            (
                box,
                width,
                50,
                text: "Text Input Menu"
            );
            CreateToggle
            (
                "Automatically Select First Input",
                () => settings.TextInputMenu_AutoSelectFirstInput,
                v => settings.TextInputMenu_AutoSelectFirstInput = v
            );
            CreateToggle
            (
                "'Tab' Key For Navigation",
                () => settings.TextInputMenu_TabForNavigation,
                v => settings.TextInputMenu_TabForNavigation = v
            );
            CreateToggle
            (
                "'Enter' Key For Confirmation",
                () => settings.TextInputMenu_EnterForConfirmation,
                v => settings.TextInputMenu_EnterForConfirmation = v
            );

            return box.gameObject;
        }

        protected override void RegisterOnVariableChange(Action onChange)
        {
            Application.quitting += onChange;
        }

        public class Data
        {
            public bool StopProcessingForTextInputs { get; set; } = true;

            public bool TextInputMenu_AutoSelectFirstInput { get; set; } = true;
            public bool TextInputMenu_TabForNavigation { get; set; } = true;
            public bool TextInputMenu_EnterForConfirmation { get; set; } = true;
        }
    }
}