using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Battle.EnemyAI
{
    public class AlternatingAttackPattern : EnemyAttackSelector
    {
        public EnemySkill[] Skills;
        private int _index;
        private void OnEnable()
        {
            _index = 0;
        }

        public override EnemySkill SelectAction()
        {
            var skill = Skills[_index];
            _index = (_index + 1) % Skills.Length;
            return skill;
        }
    }
}
