using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public abstract class EnemyAttackSelector: MonoBehaviour
    {
        public abstract EnemySkill SelectAction();
    }
}
