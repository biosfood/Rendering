using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sphere {
    public Vector3 position;
    public float radius;
    public int material;
}

public class RenderSphere : RenderObject<Sphere> {
    public float radius;

    override public void register() {
        render.spheres.Add(this);
    }

    override public Sphere prepare(List<Material> materials) {
        Sphere result = new Sphere();
        result.position = transform.position;
        result.radius = radius;
        result.material = materials.IndexOf(materialStack[0]);
        return result;
    }
}
