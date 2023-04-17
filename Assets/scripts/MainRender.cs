using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRender : MonoBehaviour {
    private RenderTexture renderTexture;
    public ComputeShader display, raytrace;
    private int doDisplay, doReset, doRender;
    public int samplesPerUpdate = 1;
    public int samples = 0;
    private int width = 400, height = 400;
    private ComputeBuffer lightData, materialBuffer, sphereBuffer, boxBuffer;
    private bool invalidate = true;
    private bool ready = false;
    public int bounces = 5;
    public Vector3 sun;
    public float sunStrength;
    public Material floorMaterial1, floorMaterial2;

    public List<RenderObject<Sphere>> spheres = new List<RenderObject<Sphere>>();
    public List<RenderObject<Box>> boxes = new List<RenderObject<Box>>();

    void Start() {
        doDisplay = display.FindKernel("display");
        doRender = raytrace.FindKernel("trace");
        doReset = raytrace.FindKernel("reset");
        raytrace.SetVector("up", Vector3.up);
        transform.hasChanged = true;
        unsafe {
            materialBuffer = new ComputeBuffer(sizeof(Material)*20, sizeof(Material));
            raytrace.SetBuffer(doRender, "materials", materialBuffer);
            sphereBuffer = new ComputeBuffer(sizeof(Sphere)*20, sizeof(Sphere));
            raytrace.SetBuffer(doRender, "spheres", sphereBuffer);
            boxBuffer = new ComputeBuffer(sizeof(Box)*20, sizeof(Box));
            raytrace.SetBuffer(doRender, "boxes", boxBuffer);
        }
    }

    private void OnValidate() {
        invalidate = true;
    }

    private List<Material> updateMaterials() {
        List<Material> materials = new List<Material>();
        materials.Add(floorMaterial1);
        materials.Add(floorMaterial2);
        foreach (RenderSphere sphere in spheres) {
            sphere.addMaterials(materials);
        }
        foreach (RenderObject<Box> box in boxes) {
            box.addMaterials(materials);
        }
        materialBuffer.SetData(materials);
        return materials;
    }

    private void updateType<T>(List<Material> materials, List<RenderObject<T>> renderObjects, ComputeBuffer buffer, string countName) where T: struct {
        List<T> data = new List<T>();
        foreach (RenderObject<T> obj in renderObjects) {
            data.Add(obj.prepare(materials));
        }
        buffer.SetData(data);
        raytrace.SetInt(countName, data.Count);
    }

    private void updateObjects(List<Material> materials) {
        updateType<Sphere>(materials, spheres, sphereBuffer, "sphereCount");
        updateType<Box>(materials, boxes, boxBuffer, "boxCount");
    }

    void Update() {
        if (transform.hasChanged) {
            transform.hasChanged = false;
            raytrace.SetVector("position", transform.position);
            raytrace.SetVector("viewDirection", transform.forward);
            invalidate = true;
        }
        if (invalidate) {
            raytrace.SetInt("width", width);
            raytrace.SetInt("height", height);
            raytrace.SetInt("bounces", bounces);
            raytrace.SetInt("iterationCount", samplesPerUpdate);
            raytrace.SetFloat("sunStrength", sunStrength);
            raytrace.SetVector("sunDirection", sun.normalized);
            List<Material> materials = updateMaterials();
            updateObjects(materials);
            display.SetInt("width", width);
            display.SetInt("height", height);

            if (lightData != null) {
                lightData.Release();
            }
            lightData = new ComputeBuffer(width * height, 12);
            display.SetBuffer(doDisplay, "light", lightData);
            raytrace.SetBuffer(doRender, "light", lightData);
            raytrace.SetBuffer(doReset, "light", lightData);
            raytrace.Dispatch(doReset, width/8+1, height/8+1, 1);

            if (renderTexture != null) {
                renderTexture.Release();
            }
            renderTexture = new RenderTexture(width, height, 0);
            renderTexture.enableRandomWrite = true;
            renderTexture.Create();
            display.SetTexture(doDisplay, "destination", renderTexture);

            samples = 0;
            invalidate = false;
            ready = true;
        }
        if (!ready) return;
        samples += samplesPerUpdate;
        raytrace.SetInt("startSeed", Random.Range(int.MinValue, int.MaxValue));
        raytrace.Dispatch(doRender, width/16 + 1, height/16 + 1, 1);
    }

    private void OnRenderImage(RenderTexture before, RenderTexture destination) {
        if (width != before.width) {
            width = before.width;
            invalidate = true;
        }
        if (height != before.height) {
            height = before.height;
            invalidate = true;
        }
        if (invalidate) {
            Graphics.Blit(before, destination);
            return;
        }
        display.SetFloat("scale", 1f / samples);
        display.Dispatch(doDisplay, width / 16 + 1, height / 16 + 1, 1);
        Graphics.Blit(renderTexture, destination); 
    }

    public void doInvalidate() {
        invalidate = true;
    }
}
