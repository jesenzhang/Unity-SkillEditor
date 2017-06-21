////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Chroma Key/Color Key")]
public class CameraFilterPack_Blend2Camera_ColorKey : MonoBehaviour {
#region Variables
private string ShaderName="CameraFilterPack/Blend2Camera_ColorKey";
public Shader SCShader;
public Camera Camera2; 
private float TimeX = 1.0f;

private Material SCMaterial;
[Range(0f, 1f)]
public float BlendFX = 1f;
public Color ColorKey;
[Range(-0.2f, 0.2f)]
public float Adjust = 0.0f;
[Range(-0.2f, 0.2f)]
public float Precision = 0.0f;
[Range(-0.2f, 0.2f)]
public float Luminosity = 0.0f;
[Range(-0.3f, 0.3f)]
public float Change_Red = 0.0f;
[Range(-0.3f, 0.3f)]
public float Change_Green = 0.0f;
[Range(-0.3f, 0.3f)]
public float Change_Blue = 0.0f;
private RenderTexture Camera2tex;
private Vector2 ScreenSize;

#endregion
#region Properties
Material material
{
get
{
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;
}
return SCMaterial;
}
}
#endregion

void Start ()
{

if (Camera2 !=null)
{

Camera2tex=new RenderTexture((int)ScreenSize.x,(int)ScreenSize.y, 24); 
Camera2.targetTexture=Camera2tex;
}

SCShader = Shader.Find(ShaderName);
if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}

void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
if (Camera2 != null) material.SetTexture("_MainTex2",Camera2tex);
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Value", BlendFX);
material.SetFloat("_Value2", Adjust);
material.SetFloat("_Value3", Precision);
material.SetFloat("_Value4", Luminosity);
material.SetFloat("_Value5", Change_Red);
material.SetFloat("_Value6", Change_Green);
material.SetFloat("_Value7", Change_Blue);
material.SetColor("_ColorKey", ColorKey);
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}

void Update ()
{
ScreenSize.x = Screen.width;
ScreenSize.y = Screen.height;

 if (Application.isPlaying)
{
}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find(ShaderName);
}
#endif
}
void OnEnable ()  { Start(); Update(); }
void OnDisable ()
{
        if (Camera2 != null)
        {
            if (Camera2.targetTexture != null)
            {
               Camera2.targetTexture = null;
            }
        }

        if (SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
