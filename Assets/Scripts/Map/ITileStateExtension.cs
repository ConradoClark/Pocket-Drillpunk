using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.Map
{
    public interface ITileStateExtension
    {
        bool Dirty { get; }
        void LoadState(Dictionary<string, object> dict);
        public void SaveState(Dictionary<string, object> dict);
    }
}
