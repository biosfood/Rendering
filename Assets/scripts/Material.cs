using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Material {
    public Color color;
    public float specular;
    public float passProbability;
    public int next;
}