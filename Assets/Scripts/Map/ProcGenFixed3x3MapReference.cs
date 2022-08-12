using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

namespace Assets.Scripts.Map
{
    public class ProcGenFixed3x3MapReference : ProcGenMapReference
    {
        public ScriptIdentifier Cell_1_1;
        public ScriptIdentifier Cell_1_2;
        public ScriptIdentifier Cell_1_3;
        public ScriptIdentifier Cell_2_1;
        public ScriptIdentifier Cell_2_2;
        public ScriptIdentifier Cell_2_3;
        public ScriptIdentifier Cell_3_1;
        public ScriptIdentifier Cell_3_2;
        public ScriptIdentifier Cell_3_3;
        public override Vector2Int RefSize => new (3, 3);

        public override void Populate(string seed, Vector2Int position)
        {
            PopulatedPositions.Add(position);

            var cells = new[]
            {
                new[] { Cell_1_1, Cell_1_2, Cell_1_3 },
                new[] { Cell_2_1, Cell_2_2, Cell_2_3 },
                new[] { Cell_3_1, Cell_3_2, Cell_3_3 },
            };

            for (var i = 0; i < RefSize.x; i++)
            {
                for (var j = 0; j < RefSize.y; j++)
                {
                    if (cells[j][i] == null) continue;
                    var newSeed = $"{seed}_group_X{position.x + i}_Y{position.y + j}";
                    Seed = newSeed.GetHashCode();
                    var reference = GameTilemap.TileDefinitionsDictionary[cells[j][i]].Reference;

                    if (reference == null)
                    {
                        throw new Exception("ProcGenMapReference 'Reference' is null for " + gameObject.name);
                    }

                    if (reference.RefSize.x <= 0 || reference.RefSize.y <= 0)
                    {
                        throw new Exception($"ProcGenMapReference 'Reference' cannot have zero/negative size ({gameObject.name})");
                    }

                    if (i % reference.RefSize.x != 0 || j % reference.RefSize.y != 0) continue;

                    reference.Populate(newSeed, position + new Vector2Int(i, RefSize.y-1-j));
                }
            }
        }
    }
}
