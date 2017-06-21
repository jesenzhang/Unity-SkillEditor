////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/ColorsAdjust/Photo Filters")]
public class CameraFilterPack_Colors_Adjust_PreFilters : MonoBehaviour {
#region Variables
private string ShaderName="CameraFilterPack/Colors_Adjust_PreFilters";
public Shader SCShader;

public enum filters 
{
Normal,
BlueLagoon,
BlueMoon,
RedWhite,
NashVille,
VintageYellow,
GoldenPink,
DarkPink,
PopRocket,
RedSoftLight,
YellowSunSet,
Walden,
WhiteShine,
Fluo,
MarsSunRise,
Amelie,
BlueJeans,
NightVision,
BlueParadise,
Blindness_Deuteranomaly,
Blindness_Protanopia, 
Blindness_Protanomaly,
Blindness_Deuteranopia,
Blindness_Tritanomaly,
Blindness_Achromatopsia,
Blindness_Achromatomaly,
Blindness_Tritanopia,
BlackAndWhite_Blue,
BlackAndWhite_Green,
BlackAndWhite_Orange,
BlackAndWhite_Red,
BlackAndWhite_Yellow,
};

public filters filterchoice = filters.Normal;
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
if (filterchoice == filters.Normal) {  Matrix9= new float[12] {100,0,0 ,0,100,0 ,0,0,100,0,0,0}; }
if (filterchoice == filters.Blindness_Deuteranomaly) {  Matrix9= new float[12] {80,20,0,25.833f,74.167f,0,0,14.167f,85.833f,0,0,0}; }
if (filterchoice == filters.Blindness_Protanopia) {  Matrix9= new float[12] {56.667f,43.333f,0,55.833f,44.167f,0,0,24.167f,75.833f,0,0,0}; }
if (filterchoice == filters.Blindness_Protanomaly) {  Matrix9= new float[12] {81.667f,18.333f,0,33.333f,66.667f,0,0,12.5f,87.5f,0,0,0}; }
if (filterchoice == filters.Blindness_Deuteranopia) {  Matrix9= new float[12] {62.5f,37.5f,0,70,30,0,0,30,70,0,0,0}; }
if (filterchoice == filters.Blindness_Tritanomaly) {  Matrix9= new float[12] {96.667f,3.333f,0,0,73.333f,26.667f,0,18.333f,81.667f,0,0,0}; }
if (filterchoice == filters.Blindness_Achromatopsia) {  Matrix9= new float[12] {29.9f,58.7f,11.4f,29.9f,58.7f,11.4f,29.9f,58.7f,11.4f,0,0,0}; }
if (filterchoice == filters.Blindness_Achromatomaly) {  Matrix9= new float[12] {61.8f,32,6.2f,16.3f,77.5f,6.2f,16.3f,32.0f,51.6f,0,0,0}; }
if (filterchoice == filters.Blindness_Tritanopia) {  Matrix9= new float[12] {95,5,0,0,43.333f,56.667f,0,47.5f,52.5f,0,0,0}; }
if (filterchoice == filters.BlueLagoon) { Matrix9 = new float[12] { 100,102,0,18,100,4, 28,-26,100, -64,0,12 }; }
if (filterchoice == filters.GoldenPink) { Matrix9 = new float[12] { 70,200,0, 0,100,8, 6,12,110,   0, 0,-6 } ;}
if (filterchoice == filters.BlueMoon) {  Matrix9= new float[12] { 200,98,-116,24,100,2,30,52,20,-48,-20,12};}
if (filterchoice == filters.DarkPink) {  Matrix9= new float[12] { 60,112,36,24,100,2, 0,-26,100  ,-56,-20,12 } ; }
if (filterchoice == filters.RedWhite) {  Matrix9= new float[12] { -42,68,108,-96,100,116,-92,104,96 ,0,2,4 } ; }
if (filterchoice == filters.VintageYellow) {  Matrix9= new float[12] { 200,109,-104,42,126,-1, -40,121,-31,-48,-20,12 };  }
if (filterchoice == filters.NashVille) {  Matrix9= new float[12] { 130,8,7,19,89,3,-1,11,57 ,  10,19,47 } ; }
if (filterchoice == filters.PopRocket) {  Matrix9= new float[12] { 100,6,-17,0,107,0,10,21,100,   40,0,8} ; }
if (filterchoice == filters.RedSoftLight) {  Matrix9= new float[12] { -4,200,-30 , -58,200,-30,-58,200,-30,   -11,0,0 } ; }
if (filterchoice == filters.BlackAndWhite_Blue) {  Matrix9= new float[12] { 0,0,100, 0,0,100, 0,0,100,0,0,0 } ; }
if (filterchoice == filters.BlackAndWhite_Green) {  Matrix9= new float[12] { 0,100,0,0,100,0,0,100,0,0,0,0} ; }
if (filterchoice == filters.BlackAndWhite_Orange) {  Matrix9= new float[12] { 50,50,0,50,50,0,50,50,0,0,0,0 } ; }
if (filterchoice == filters.BlackAndWhite_Red) {  Matrix9= new float[12] { 100,0,0,100,0,0,100,0,0,0,0,0 } ;}
if (filterchoice == filters.BlackAndWhite_Yellow) {  Matrix9= new float[12] { 34,66,0, 34,66,0, 34,66,0,0,0,0 } ; }
if (filterchoice == filters.YellowSunSet) {  Matrix9= new float[12] {  117,-6,53,-68,135,19,-146,-61,200   ,0,0,0 } ; }
if (filterchoice == filters.Walden) {  Matrix9= new float[12] { 99,2,13, 100,1,40, 50,8,71,  0,2, 7 } ; }
if (filterchoice == filters.WhiteShine) {  Matrix9= new float[12]  { 190,24,-33, 2,200,-6,-10,27,200, -6,-13,15} ; }
if (filterchoice == filters.Fluo) {  Matrix9= new float[12] { 100,0,0, 0,113,0, 200,-200,-200, 0,0,36 } ; }
if (filterchoice == filters.MarsSunRise) {  Matrix9= new float[12] { 50,141,-81,-17,62,29, -159,-137,-200, 7,-34,-6 } ; }
if (filterchoice == filters.Amelie) {  Matrix9= new float[12] { 100,11,39,1,63,53, -24,71,20,  -25,-10,-24 } ; }
if (filterchoice == filters.BlueJeans) {  Matrix9= new float[12] { 181,11,15,40,40,20,40,40,20,  -59,0,0 } ; }
if (filterchoice == filters.NightVision) {  Matrix9= new float[12] { 200,-200,-200, 195,4,-160,200,-200,-200,   -200,10,-200} ; }
if (filterchoice == filters.BlueParadise) {  Matrix9= new float[12] { 66,200,-200,25,38,36,30,150,114   ,17,0,65} ; }





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
