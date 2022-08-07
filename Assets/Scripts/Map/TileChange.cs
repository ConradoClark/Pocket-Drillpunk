using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public struct TileChange
{
    public bool Destroyed;
    public int Durability;
    public Dictionary<string, object> CustomProperties;
}
