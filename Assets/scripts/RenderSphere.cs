using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Sphere {
    public Vector3 position;
    public float radius;
    public int material;
}

public class RenderSphere : MonoBehaviour {
    public Material material;
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
        materials.Add(material);
    }

    public Sphere prepare(List<Material> materials) {
        Sphere result = new Sphere();
        result.position = transform.position;
        result.radius = radius;
        result.material = materials.IndexOf(material);
        return result;
    }
}
