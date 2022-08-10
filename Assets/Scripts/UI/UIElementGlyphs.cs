using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.UI
{
    [CreateAssetMenu(fileName = "ElementGlyphs", menuName = "Drillpunk/UI/ElementGlyphs", order = 1)]
    public class UIElementGlyphs : ScriptableObject
    {
        public Sprite NeutralSprite;
        public Sprite FireSprite;
        public Sprite IceSprite;
        public Sprite BioSprite;
        public Sprite CrystalSprite;
    }
}
