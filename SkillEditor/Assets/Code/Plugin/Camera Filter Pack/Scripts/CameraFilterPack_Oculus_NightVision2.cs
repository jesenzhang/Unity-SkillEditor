///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Night Vision/Night Vision 2")]
public class CameraFilterPack_Oculus_NightVision2 : MonoBehaviour {
#region Variables
private string ShaderName="CameraFilterPack/Oculus_NightVision2";
public Shader SCShader;

[Range(0f, 1f)]
public float FadeFX = 1f;

private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
private float[] Matrix9;

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

void ChangeFilters()
{
Matrix9= new float[12] { 200,-200,-200, 195,4,-160,200,-200,-200,   -200,10,-200} ;
}

void Start ()
{
ChangeFilters ();
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
material.SetFloat("_TimeX", TimeX);

material.SetFloat("_Red_R", Matrix9[0]/100f);
material.SetFloat("_Red_G", Matrix9[1]/100f);
material.SetFloat("_Red_B", Matrix9[2]/100f);
material.SetFloat("_Green_R", Matrix9[3]/100f);
material.SetFloat("_Green_G", Matrix9[4]/100f);
material.SetFloat("_Green_B", Matrix9[5]/100f);
material.SetFloat("_Blue_R", Matrix9[6]/100f);
material.SetFloat("_Blue_G", Matrix9[7]/100f);
material.SetFloat("_Blue_B", Matrix9[8]/100f);
material.SetFloat("_Red_C",  Matrix9[9]/100);
material.SetFloat("_Green_C", Matrix9[10]/100);
material.SetFloat("_Blue_C", Matrix9[11]/100);
material.SetFloat("_FadeFX", FadeFX);
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
ChangeFilters ();
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
void OnDisable ()
{
if(SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
