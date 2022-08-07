using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class GameTilemap : MonoBehaviour
    {
        [Serializable]
        public struct TileDefinition
        {
            public ScriptIdentifier Identifier;
            public ProcGenMapReference Reference;
        }

        public TileDefinition[] TileDefinitions;
        public Dictionary<ScriptIdentifier, TileDefinition> TileDefinitionsDictionary;

        private void Awake()
        {
            TileDefinitionsDictionary = new Dictionary<ScriptIdentifier, TileDefinition>(
                TileDefinitions.Select(def => new KeyValuePair<ScriptIdentifier, TileDefinition>(def.Identifier, def)));
        }
    }
}
