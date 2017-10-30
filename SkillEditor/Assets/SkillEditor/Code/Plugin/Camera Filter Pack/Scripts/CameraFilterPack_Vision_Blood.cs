///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Vision/Blood")]
public class CameraFilterPack_Vision_Blood : MonoBehaviour {
#region Variables
public Shader SCShader;
public bool autoAnimation = false;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0.01f, 1f)]
public float HoleSize = 0.6f;
[Range(-1f, 1f)]
public float HoleSmooth = 0.3f;
[Range(-2f, 2f)]
public float Color1 = 0.2f;
[Range(-2f, 2f)]
public float Color2 = 0.9f;

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

SCShader = Shader.Find("CameraFilterPack/Vision_Blood");
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

if (autoAnimation) {
	TimeX+=Time.deltaTime;
	if (TimeX>100)  TimeX=0;
}
				
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Value", HoleSize);
material.SetFloat("_Value2", HoleSmooth);
material.SetFloat("_Value3", Color1);
material.SetFloat("_Value4", Color2);
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
SCShader = Shader.Find("CameraFilterPack/Vision_Blood");
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
