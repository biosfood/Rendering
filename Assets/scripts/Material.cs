using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Material {
    public Color diffuseColor;
    [Range(0f, 1f)]
    public float specular;
    public Color specularColor;
    [Range(0f, 1f)]
    public float metal;
    public Color emissionColor;
    public float emissionStrength;
    public Color tint;
    [Range(1f, 2f)]
    public float refractiveIndex;
    [Range(0f, 1f)]
    public float refractionProbability;
    [Range(0f, 1f)]
    public float passProbability;
    public int next;
}