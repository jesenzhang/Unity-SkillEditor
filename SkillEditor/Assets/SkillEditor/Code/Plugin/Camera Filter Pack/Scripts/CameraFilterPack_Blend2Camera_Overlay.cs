////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blend 2 Camera/Overlay")]
public class CameraFilterPack_Blend2Camera_Overlay : MonoBehaviour {
#region Variables
private string ShaderName="CameraFilterPack/Blend2Camera_Overlay";
public Shader SCShader;
public Camera Camera2; 
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 1f)]
public float SwitchCameraToCamera2 = 0f;
[Range(0f, 1f)]
public float BlendFX = 0.5f;

private RenderTexture Camera2tex;
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
Camera2tex=new RenderTexture(Screen.width,Screen.height, 24); 
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
material.SetFloat("_Value2", SwitchCameraToCamera2);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);
}
}
void OnValidate()
{	
if (Camera2 != null) 
{

Camera2tex=new RenderTexture(Screen.width,Screen.height, 24); 
Camera2.targetTexture = Camera2tex;
}

}
void Update ()
{

#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find(ShaderName);
}
#endif
}
void OnEnable ()
{
if (Camera2 !=null)
{
Camera2tex=new RenderTexture(Screen.width,Screen.height, 24); 
Camera2.targetTexture=Camera2tex;
}

}
void OnDisable ()
{

if (Camera2 !=null) {  Camera2.targetTexture=null; }

if(SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
