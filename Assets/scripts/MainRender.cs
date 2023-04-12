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
    private ComputeBuffer lightData, materialBuffer;
    private bool invalidate = true;
    private bool ready = false;
    public int bounces = 5;
    public Vector3 sun;
    public float sunStrength;
    public Material floorMaterial1, floorMaterial2, sphereMaterial;

    void Start() {
        doDisplay = display.FindKernel("display");
        doRender = raytrace.FindKernel("trace");
        doReset = raytrace.FindKernel("reset");
        raytrace.SetVector("up", transform.up);
        transform.hasChanged = true;
        materialBuffer = new ComputeBuffer(20*20, 20);
        raytrace.SetBuffer(doRender, "materials", materialBuffer);
    }

    private void OnValidate() {
        invalidate = true;
    }

    private void updateMaterials() {
        List<Material> materials = new List<Material>();
        materials.Add(floorMaterial1);
        materials.Add(floorMaterial2);
        materials.Add(sphereMaterial);
        materialBuffer.SetData(materials);
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
            raytrace.SetFloat("sunStrength", sunStrength);
            raytrace.SetVector("sunDirection", sun.normalized);
            updateMaterials();
            display.SetInt("width", width);

            if (lightData != null) {
                lightData.Release();
            }
            lightData = new ComputeBuffer(width * height, 12);
            display.SetBuffer(doDisplay, "light", lightData);
            raytrace.SetBuffer(doRender, "light", lightData);
            raytrace.SetBuffer(doReset, "light", lightData);
            raytrace.Dispatch(doReset, width/8, height/8, 1);

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
        for (int i = 0; i < samplesPerUpdate; i++) {
            raytrace.SetInt("startSeed", Random.Range(int.MinValue, int.MaxValue));
            raytrace.Dispatch(doRender, width/8, height/8, 1);
        }
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
            return;
        }
        display.SetFloat("scale", 1f / samples);
        display.Dispatch(doDisplay, width / 8, height / 8, 1);
        Graphics.Blit(renderTexture, destination); 
    }
}
