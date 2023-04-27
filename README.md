# Rendering

This is a Unity-project to experiment with writing a ray-casting shader.
Implemented features include support for light sources, reflections and refraction.

The implementation relies on a compute shader which continously runs to update a cumulated light image, which has to be normalized to be displayed properly.

## Demo image:
![](img/demo.png)
