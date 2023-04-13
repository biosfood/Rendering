using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sphere {
    public Vector3 position;
    public float radius;
    public int material;
}

public class RenderSphere : MonoBehaviour {
    public Material[] materialStack;
    public float radius;
    public MainRender render;

    void Start() {
        render = Camera.main.gameObject.GetComponent<MainRender>();
        render.spheres.Add(this);
    }

    void Update() {
        if (transform.hasChanged) {
            render.doInvalidate();
            transform.hasChanged = false;
        }
    }

    private void OnValidate() {
        if (render != null) {
            render.doInvalidate();
        }
    }

    public void addMaterials(List<Material> materials) {
        for (int i = 0; i < materialStack.Length - 1; i++) {
            materialStack[i].next = materials.Count + i + 1;
        }
        materials.AddRange(materialStack);
    }

    public Sphere prepare(List<Material> materials) {
        Sphere result = new Sphere();
        result.position = transform.position;
        result.radius = radius;
        result.material = materials.IndexOf(materialStack[0]);
        return result;
    }
}
