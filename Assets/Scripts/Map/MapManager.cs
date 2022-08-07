using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Licht.Unity.Objects;
using UnityEngine;

public class MapManager : BaseGameObject
{
    public static Vector3Int Get9X9Position(Vector3Int pos)
    {
        return (pos + new Vector3Int(pos.x < 0 ? +1 : 0, pos.y < 0 ? +1 : 0)) / 9
               + new Vector3Int(pos.x < 0 ? -1 : 0, pos.y < 0 ? -1 : 0);
    }
}
