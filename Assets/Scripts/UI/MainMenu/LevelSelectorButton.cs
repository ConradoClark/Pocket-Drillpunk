using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI.MainMenu
{
    public class LevelSelectorButton : MainMenuButton
    {
        public string LevelName;
        public string LevelDifficulty;
        public Material DifficultyTextMaterial;

        public UITextRenderer LevelNameRenderer;
        public UITextRenderer LevelDifficultyRenderer;

        public override void Select()
        {
            base.Select();
            LevelNameRenderer.Text = LevelName;
            LevelDifficultyRenderer.Text = LevelDifficulty;
            LevelDifficultyRenderer.DefaultMaterial = DifficultyTextMaterial;
        }

    }
}
