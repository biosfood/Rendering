using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Box {
    public Vector3 min, max;
    public int material;
}

public class RenderBox : RenderObject<Box> {
    override public void register() {
        render.boxes.Add(this);
    }

    override public Box prepare(List<Material> materials) {
        Box result = new Box();
        result.min = transform.position - transform.localScale * 0.5f;
        result.max = transform.position + transform.localScale * 0.5f;
        result.material = materials.IndexOf(materialStack[0]);
        print(result.material);
        return result;
    }
}
