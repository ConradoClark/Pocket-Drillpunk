using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Interfaces.Generation;

namespace Assets.Scripts.Map
{
    public interface ITilePropGenerator : IWeighted<float>
    {
        void Populate(string seed);
    }
}
