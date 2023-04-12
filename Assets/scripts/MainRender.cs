using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainRender : MonoBehaviour {
    private RenderTexture renderTexture;
    public ComputeShader display, raytrace;
    private int doDisplay, doReset, doRender;
    public int samplesPerUpdate = 1;
    public int samples = 0;
    public int width = 400, height = 400;
    private ComputeBuffer light;
    private bool invalidate = true;
    private bool ready = false;
    public int bounces = 5;
    public Vector3 sun;
    public float sunStrength;

    void Start() {
        doDisplay = display.FindKernel("display");
        doRender = raytrace.FindKernel("trace");
        doReset = raytrace.FindKernel("reset");
        raytrace.SetVector("up", transform.up);
        raytrace.SetVector("sunDirection", sun.normalized);
        transform.hasChanged = true;
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
            display.SetInt("width", width);

            if (light != null) {
                light.Release();
            }
            light = new ComputeBuffer(width * height, 12);
            display.SetBuffer(doDisplay, "light", light);
            raytrace.SetBuffer(doRender, "light", light);
            raytrace.SetBuffer(doReset, "light", light);
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
