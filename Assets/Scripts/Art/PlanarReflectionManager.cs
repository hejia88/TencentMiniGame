using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanarReflectionManager : MonoBehaviour
{
    Camera m_ReflectionCamera;
    Camera m_MainCamera;

    public GameObject m_RelfectionPlane;
    public Material m_ReflectionMaterial;

    RenderTexture m_RenderTarget;

    [Range(0.0f, 1.0f)]
    public float m_RelfectionFactor = 0.5f;
    // Start is called before the first frame update
    void Start()
    {
        GameObject reflectionCameraGo = new GameObject("ReflectionCamera");
        m_ReflectionCamera = reflectionCameraGo.AddComponent<Camera>();
        m_ReflectionCamera.enabled = false;

        m_MainCamera = Camera.main;

        m_RenderTarget = new RenderTexture(Screen.width, Screen.height, 24);
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalFloat("_reflectionFactor", m_RelfectionFactor);
    }

    private void OnPostRender() {
        RenderReflection();
    }

    void RenderReflection()
    {
        m_ReflectionCamera.CopyFrom(m_MainCamera);

        Vector3 cameraDirectionWorldSpace = m_MainCamera.transform.forward;
        Vector3 cameraUpWorldSpace = m_MainCamera.transform.up;
        Vector3 cameraPositionWorldSpace = m_MainCamera.transform.position;

        // Transform to object space
        Vector3 cameraDirectionPlaneSpace = m_RelfectionPlane.transform.InverseTransformDirection(cameraDirectionWorldSpace);
        Vector3 cameraUpPlaneSpace = m_RelfectionPlane.transform.InverseTransformDirection(cameraUpWorldSpace);
        Vector3 cameraPositionPlaneSpace = m_RelfectionPlane.transform.InverseTransformDirection(cameraPositionWorldSpace);

        // Mirror the vectors
        cameraDirectionPlaneSpace.y *= -1.0f;
        cameraUpPlaneSpace.y *= -1.0f;
        cameraPositionPlaneSpace.y *= -1.0f;

        // Transform back to world space
        cameraDirectionWorldSpace = m_RelfectionPlane.transform.TransformDirection(cameraDirectionPlaneSpace);
        cameraUpWorldSpace = m_RelfectionPlane.transform.TransformDirection(cameraUpPlaneSpace);
        cameraPositionWorldSpace = m_RelfectionPlane.transform.TransformDirection(cameraPositionPlaneSpace);

        // set camera position and rotation
        m_ReflectionCamera.transform.position = cameraPositionWorldSpace;
        m_ReflectionCamera.transform.LookAt(cameraPositionWorldSpace + cameraDirectionWorldSpace, cameraUpWorldSpace);

        // Set render target for the reflection camera
        m_ReflectionCamera.targetTexture = m_RenderTarget;

        // Render the reflection camera
        m_ReflectionCamera.Render();

        // Draw full screen quad
        DrawQuad();

    }

    void DrawQuad(){
        GL.PushMatrix();

        // Use ground Matrail to draw the quad
        m_ReflectionMaterial.SetPass(0);
        m_ReflectionMaterial.SetTexture("_ReflectionTex", m_RenderTarget);


        GL.LoadOrtho();

        GL.Begin(GL.QUADS);

        GL.TexCoord2(1.0f, 0.0f);
        GL.Vertex3(0.0f, 0.0f, 0.0f);
        GL.TexCoord2(1.0f, 1.0f);
        GL.Vertex3(0.0f, 1.0f, 0.0f);
        GL.TexCoord2(0.0f, 1.0f);
        GL.Vertex3(1.0f, 1.0f, 0.0f);
        GL.TexCoord2(0.0f, 0.0f);
        GL.Vertex3(1.0f, 0.0f, 0.0f);

        GL.End();

        GL.PopMatrix();
    }
}
