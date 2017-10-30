///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Old Film/Cutting 2")]
public class CameraFilterPack_OldFilm_Cutting2 : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(0, 10)]
public float Speed = 5f;
[Range(0, 2)]
public float Luminosity = 1f;
[Range(0, 1)]
public float Vignette = 1.0f;
[Range(0, 1)]
public float Negative = 0.0f;
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
Texture2 = Resources.Load ("CameraFilterPack_OldFilm2") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/OldFilm_Cutting2");
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
material.SetFloat("_Value", 2-Luminosity);
material.SetFloat("_Value2", 1-Vignette);
material.SetFloat("_Value3", Negative);
material.SetFloat("_Speed", Speed);
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
SCShader = Shader.Find("CameraFilterPack/OldFilm_Cutting2");
Texture2 = Resources.Load ("CameraFilterPack_OldFilm2") as Texture2D;
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