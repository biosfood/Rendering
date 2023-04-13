using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RenderObject<T> : MonoBehaviour {
    protected MainRender render;
    public Material[] materialStack;

    public abstract void register();
    public abstract T prepare(List<Material> materials);

    void Start() {
        render = Camera.main.gameObject.GetComponent<MainRender>();
        register();
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
}
