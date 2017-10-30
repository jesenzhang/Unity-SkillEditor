////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/ColorsAdjust/FullColors")]
public class CameraFilterPack_Colors_Adjust_FullColors : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(-200f, 200f)]
public float Red_R = 100f;
[Range(-200f, 200f)]
public float Red_G = 0f;
[Range(-200f, 200f)]
public float Red_B = 0f;
[Range(-200f, 200f)]
public float Red_Constant = 0;
[Range(-200f, 200f)]
public float Green_R = 0f;
[Range(-200f, 200f)]
public float Green_G = 100f;
[Range(-200f, 200f)]
public float Green_B = 0f;
[Range(-200f, 200f)]
public float Green_Constant = 0;
[Range(-200f, 200f)]
public float Blue_R = 0f;
[Range(-200f, 200f)]
public float Blue_G = 0f;
[Range(-200f, 200f)]
public float Blue_B = 100f;
[Range(-200f, 200f)]
public float Blue_Constant = 0;


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

SCShader = Shader.Find("CameraFilterPack/Colors_Adjust_FullColors");
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
material.SetFloat("_Red_R", Red_R/100);
material.SetFloat("_Red_G", Red_G/100);
material.SetFloat("_Red_B", Red_B/100);
material.SetFloat("_Green_R", Green_R/100);
material.SetFloat("_Green_G", Green_G/100);
material.SetFloat("_Green_B", Green_B/100);
material.SetFloat("_Blue_R", Blue_R/100);
material.SetFloat("_Blue_G", Blue_G/100);
material.SetFloat("_Blue_B", Blue_B/100);
material.SetFloat("_Red_C", Red_Constant/100);
material.SetFloat("_Green_C", Green_Constant/100);
material.SetFloat("_Blue_C", Blue_Constant/100);
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
if (Application.isPlaying)
{

}
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Colors_Adjust_FullColors");
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
