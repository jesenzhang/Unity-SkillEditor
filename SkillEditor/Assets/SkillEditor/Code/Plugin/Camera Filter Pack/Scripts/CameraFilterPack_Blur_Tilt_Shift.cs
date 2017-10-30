////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blur/Tilt_Shift")]
public class CameraFilterPack_Blur_Tilt_Shift : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0, 20)]
public float Amount = 3f;
[Range(2,16)]
public int FastFilter = 8;
[Range(0,1f)]
public float Smooth = 0.5f;
[Range(0,1f)]
public float Size = 0.5f;
[Range(-1,1f)]
public float Position = 0.5f;
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
SCShader = Shader.Find("CameraFilterPack/BlurTiltShift");
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
int DownScale=FastFilter;
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Amount", Amount);
material.SetFloat("_Value1", Smooth);
material.SetFloat("_Value2", Size);
material.SetFloat("_Value3", Position);
material.SetVector("_ScreenResolution",new Vector2(Screen.width/DownScale,Screen.height/DownScale));
int rtW = sourceTexture.width/DownScale;
int rtH = sourceTexture.height/DownScale;
if (FastFilter>1)
{
RenderTexture buffer = RenderTexture.GetTemporary(rtW, rtH, 0);
RenderTexture buffer2 = RenderTexture.GetTemporary(rtW, rtH, 0);
buffer.filterMode=FilterMode.Trilinear;
Graphics.Blit(sourceTexture, buffer, material,2);
Graphics.Blit(buffer, buffer2, material,0);
material.SetFloat("_Amount", Amount*2);
Graphics.Blit(buffer2, buffer, material,2);
Graphics.Blit(buffer, buffer2, material,0);
material.SetTexture("_MainTex2", buffer2);
RenderTexture.ReleaseTemporary(buffer);
RenderTexture.ReleaseTemporary(buffer2);
Graphics.Blit(sourceTexture, destTexture, material,1);
}
else
{
Graphics.Blit(sourceTexture, destTexture, material,0);
}
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
SCShader = Shader.Find("CameraFilterPack/BlurTiltShift");
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