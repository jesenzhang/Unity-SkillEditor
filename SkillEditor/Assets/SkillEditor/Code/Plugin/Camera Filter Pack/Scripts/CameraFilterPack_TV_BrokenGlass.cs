///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/TV/Broken Glass")]
public class CameraFilterPack_TV_BrokenGlass : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(0, 128)]
public float Broken_Small = 0f;
[Range(0, 128)]
public float Broken_Medium = 0f;
[Range(0, 128)]
public float Broken_High = 0f;
[Range(0, 128)]
public float Broken_Big = 1f;
[Range(0, 0.004f)]
public float LightReflect = 0.002f;
private Material SCMaterial;
private Texture2D Texture2;
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
Texture2 = Resources.Load ("CameraFilterPack_TV_BrokenGlass1") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/TV_BrokenGlass");
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
material.SetFloat("_Value", LightReflect);
material.SetFloat("_Value2", Broken_Small);
material.SetFloat("_Value3", Broken_Medium);
material.SetFloat("_Value4", Broken_High);
material.SetFloat("_Value5", Broken_Big);
material.SetTexture("_MainTex2", Texture2);
Graphics.Blit(sourceTexture, destTexture, material);
}
else
{
Graphics.Blit(sourceTexture, destTexture);	
}
}
// Update is called once per frame
void Update () 
{
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/TV_BrokenGlass");
Texture2 = Resources.Load ("CameraFilterPack_TV_BrokenGlass1") as Texture2D;
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