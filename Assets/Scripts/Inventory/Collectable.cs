using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Impl.Orchestration;
using Licht.Unity.Objects;
using Licht.Unity.Physics;
using Licht.Unity.Pooling;

namespace Assets.Scripts.Inventory
{
    public abstract class Collectable : EffectPoolable
    {
        public LichtPhysicsObject PhysicsObject;
        public bool Collected { get; private set; }
        private void OnEnable()
        {
            Collected = false;
            PhysicsObject.AddCustomObject(this);
        }

        private void OnDisable()
        {
            PhysicsObject.RemoveCustomObject<Collectable>();
        }

        protected abstract IEnumerable<IEnumerable<Action>> CollectEffect();

        public void Collect()
        {
            Collected = true;
            DefaultMachinery.AddBasicMachine(CollectAction());
        }

        private IEnumerable<IEnumerable<Action>> CollectAction()
        {
            yield return CollectEffect().AsCoroutine();
            EndEffect();
        }

        public override void OnActivation()
        {
        }
    }
}
