using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Time;
using Licht.Unity.Objects;
using Licht.Unity.Pooling;
using UnityEngine;

namespace Assets.Scripts.Battle
{
    public abstract class BaseBattler : EffectPoolable
    {
        public Animator Animator;
        protected ITimer UITimer;
        public abstract event Action<int> OnHit;

        public abstract void Hit(int damage);

        protected override void OnAwake()
        {
            base.OnAwake();
            UITimer = SceneObject<DefaultUITimer>.Instance().TimerRef.Timer;
        }

        public override void OnActivation()
        {
            transform.localScale = Vector3.one;
        }

        public void PlayAnim(string anim)
        {
            Animator.Play(anim);
        }

        public virtual void Die()
        {

        }
    }
}
