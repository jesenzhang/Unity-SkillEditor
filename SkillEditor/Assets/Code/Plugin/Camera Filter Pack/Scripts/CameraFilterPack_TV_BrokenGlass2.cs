///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/TV/Broken Glass2")]
public class CameraFilterPack_TV_BrokenGlass2 : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(0, 1)]
public float Bullet_1 = 0f;
[Range(0, 1)]
public float Bullet_2 = 0f;
[Range(0, 1)]
public float Bullet_3 = 0f;
[Range(0, 1)]
public float Bullet_4 = 1f;
[Range(0, 1)]
public float Bullet_5 = 0f;
[Range(0, 1)]
public float Bullet_6 = 0f;
[Range(0, 1)]
public float Bullet_7 = 0f;
[Range(0, 1)]
public float Bullet_8 = 0f;
[Range(0, 1)]
public float Bullet_9 = 0f;
[Range(0, 1)]
public float Bullet_10 = 0f;
[Range(0, 1)]
public float Bullet_11 = 0f;
[Range(0, 1)]
public float Bullet_12 = 0f;
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
Texture2 = Resources.Load ("CameraFilterPack_TV_BrokenGlass_2") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/TV_BrokenGlass2");
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
if (Bullet_1<0) Bullet_1=0;
if (Bullet_2<0) Bullet_2=0;
if (Bullet_3<0) Bullet_3=0;
if (Bullet_4<0) Bullet_4=0;
if (Bullet_5<0) Bullet_5=0;
if (Bullet_6<0) Bullet_6=0;
if (Bullet_7<0) Bullet_7=0;
if (Bullet_8<0) Bullet_8=0;
if (Bullet_9<0) Bullet_9=0;
if (Bullet_10<0) Bullet_10=0;
if (Bullet_11<0) Bullet_11=0;
if (Bullet_12<0) Bullet_12=0;
if (Bullet_1>1) Bullet_1=1;
if (Bullet_2>1) Bullet_2=1;
if (Bullet_3>1) Bullet_3=1;
if (Bullet_4>1) Bullet_4=1;
if (Bullet_5>1) Bullet_5=1;
if (Bullet_6>1) Bullet_6=1;
if (Bullet_7>1) Bullet_7=1;
if (Bullet_8>1) Bullet_8=1;
if (Bullet_9>1) Bullet_9=1;
if (Bullet_10>1) Bullet_10=1;
if (Bullet_11>1) Bullet_11=1;
if (Bullet_12>1) Bullet_12=1;
material.SetFloat("_Bullet_1", Bullet_1);
material.SetFloat("_Bullet_2", Bullet_2);
material.SetFloat("_Bullet_3", Bullet_3);
material.SetFloat("_Bullet_4", Bullet_4);
material.SetFloat("_Bullet_5", Bullet_5);
material.SetFloat("_Bullet_6", Bullet_6);
material.SetFloat("_Bullet_7", Bullet_7);
material.SetFloat("_Bullet_8", Bullet_8);
material.SetFloat("_Bullet_9", Bullet_9);
material.SetFloat("_Bullet_10", Bullet_10);
material.SetFloat("_Bullet_11", Bullet_11);
material.SetFloat("_Bullet_12", Bullet_12);
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
SCShader = Shader.Find("CameraFilterPack/TV_BrokenGlass2");
Texture2 = Resources.Load ("CameraFilterPack_TV_BrokenGlass_2") as Texture2D;
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