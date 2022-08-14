using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.UI.MainMenu
{
    public abstract class MainMenuAction : BaseUIObject
    {
        public abstract IEnumerable<IEnumerable<Action>> PerformAction();
    }
}
