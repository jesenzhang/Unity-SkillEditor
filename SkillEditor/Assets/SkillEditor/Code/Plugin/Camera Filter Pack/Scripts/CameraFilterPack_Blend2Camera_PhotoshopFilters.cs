////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blend 2 Camera/PhotoshopFilters")]
public class CameraFilterPack_Blend2Camera_PhotoshopFilters : MonoBehaviour {
#region Variables
private string ShaderName="CameraFilterPack/Blend2Camera_Darken";
public Shader SCShader;
public Camera Camera2; 
public enum filters 
{
Darken,
Multiply,
ColorBurn, 
LinearBurn,
DarkerColor,
Lighten,
Screen,
ColorDodge,
LinearDodge,
LighterColor,
Overlay,
SoftLight,
HardLight,
VividLight,
LinearLight,
PinLight,
HardMix,
Difference,
Exclusion,
Subtract,
Divide,
Hue,
Saturation,
Color,
Luminosity
};

public filters filterchoice = filters.Darken;
private filters filterchoicememo;
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

void ChangeFilters()
{
if (filterchoice == filters.Darken) {  ShaderName= "CameraFilterPack/Blend2Camera_Darken"; }
if (filterchoice == filters.Multiply) {  ShaderName= "CameraFilterPack/Blend2Camera_Multiply"; }
if (filterchoice == filters.ColorBurn) {  ShaderName= "CameraFilterPack/Blend2Camera_ColorBurn"; }
if (filterchoice == filters.LinearBurn) {  ShaderName= "CameraFilterPack/Blend2Camera_LinearBurn"; }
if (filterchoice == filters.DarkerColor) {  ShaderName= "CameraFilterPack/Blend2Camera_DarkerColor"; }
if (filterchoice == filters.Lighten) {  ShaderName= "CameraFilterPack/Blend2Camera_Lighten"; }
if (filterchoice == filters.Screen) {  ShaderName= "CameraFilterPack/Blend2Camera_Screen"; }
if (filterchoice == filters.ColorDodge) {  ShaderName= "CameraFilterPack/Blend2Camera_ColorDodge"; }
if (filterchoice == filters.LinearDodge) {  ShaderName= "CameraFilterPack/Blend2Camera_LinearDodge"; }
if (filterchoice == filters.LighterColor) {  ShaderName= "CameraFilterPack/Blend2Camera_LighterColor"; }
if (filterchoice == filters.Overlay) {  ShaderName= "CameraFilterPack/Blend2Camera_Overlay"; }
if (filterchoice == filters.SoftLight) {  ShaderName= "CameraFilterPack/Blend2Camera_SoftLight"; }
if (filterchoice == filters.HardLight) {  ShaderName= "CameraFilterPack/Blend2Camera_HardLight"; }
if (filterchoice == filters.VividLight) {  ShaderName= "CameraFilterPack/Blend2Camera_VividLight"; }
if (filterchoice == filters.LinearLight) {  ShaderName= "CameraFilterPack/Blend2Camera_LinearLight"; }
if (filterchoice == filters.PinLight) {  ShaderName= "CameraFilterPack/Blend2Camera_PinLight"; }
if (filterchoice == filters.HardMix) {  ShaderName= "CameraFilterPack/Blend2Camera_HardMix"; }
if (filterchoice == filters.Difference) {  ShaderName= "CameraFilterPack/Blend2Camera_Difference"; }
if (filterchoice == filters.Exclusion) {  ShaderName= "CameraFilterPack/Blend2Camera_Exclusion"; }
if (filterchoice == filters.Subtract) {  ShaderName= "CameraFilterPack/Blend2Camera_Subtract"; }
if (filterchoice == filters.Divide) {  ShaderName= "CameraFilterPack/Blend2Camera_Divide"; }
if (filterchoice == filters.Hue) {  ShaderName= "CameraFilterPack/Blend2Camera_Hue"; }
if (filterchoice == filters.Saturation) {  ShaderName= "CameraFilterPack/Blend2Camera_Saturation"; }
if (filterchoice == filters.Color) {  ShaderName= "CameraFilterPack/Blend2Camera_Color"; }
if (filterchoice == filters.Luminosity) {  ShaderName= "CameraFilterPack/Blend2Camera_Luminosity"; }

}

void Start ()
{
filterchoicememo = filterchoice;
if (Camera2 !=null)
{

Camera2tex=new RenderTexture(Screen.width,Screen.height, 24); 
Camera2.targetTexture=Camera2tex;
}


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
if (filterchoice != filterchoicememo) 
{
ChangeFilters();
SCShader = Shader.Find(ShaderName);
DestroyImmediate(SCMaterial);
if(SCMaterial == null)
{
SCMaterial = new Material(SCShader);
SCMaterial.hideFlags = HideFlags.HideAndDontSave;
}

}


filterchoicememo = filterchoice;
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
