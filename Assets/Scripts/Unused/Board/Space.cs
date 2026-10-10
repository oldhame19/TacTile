using System;
using System.Collections.Generic;
using UnityEngine;

public enum SpaceKind
{
    Normal
}

[Serializable]
public class Space
{
    public int id;
    public string displayName;
    public Vector2 position;
    public SpaceKind spaceKind;
    public List<int> neighborIds = new List<int>();
}