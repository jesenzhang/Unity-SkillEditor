////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Colors/RgbClamp")]
public class CameraFilterPack_Colors_RgbClamp : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 1f)]
public float Red_Start = 0f;
[Range(0f, 1f)]
public float Red_End = 1f;
[Range(0f, 1f)]
public float Green_Start = 0f;
[Range(0f, 1f)]
public float Green_End = 1f;
[Range(0f, 1f)]
public float Blue_Start = 0f;
[Range(0f, 1f)]
public float Blue_End = 1f;
[Range(0f, 1f)]
public float RGB_Start = 0f;
[Range(0f, 1f)]
public float RGB_End = 1f;
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
SCShader = Shader.Find("CameraFilterPack/Colors_RgbClamp");
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
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Value", Red_Start);
material.SetFloat("_Value2", Red_End);
material.SetFloat("_Value3", Green_Start);
material.SetFloat("_Value4", Green_End);
material.SetFloat("_Value5", Blue_Start);
material.SetFloat("_Value6", Blue_End);
material.SetFloat("_Value7", RGB_Start);
material.SetFloat("_Value8", RGB_End);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void Update ()
{

#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Colors_RgbClamp");
}
#endif
}
void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
