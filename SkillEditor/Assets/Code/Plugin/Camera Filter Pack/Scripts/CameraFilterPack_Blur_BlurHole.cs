////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Blur/Blur Hole")]
public class CameraFilterPack_Blur_BlurHole : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
[Range(1, 16)] public float Size = 10;
[Range(0, 1)] public float _Radius = 0.25f;
[Range(0, 4)] public float _SpotSize = 1;
[Range(0, 1)] public float _CenterX = 0.5f;
[Range(0, 1)] public float _CenterY = 0.5f;
[Range(0, 1)] public float _AlphaBlur = 1.0f;
[Range(0, 1)] public float _AlphaBlurInside = 0.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;


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

SCShader = Shader.Find("CameraFilterPack/BlurHole");

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
material.SetFloat("_Distortion", Size);
material.SetFloat("_Radius", _Radius);
material.SetFloat("_SpotSize", _SpotSize);
material.SetFloat("_CenterX", _CenterX);
material.SetFloat("_CenterY", _CenterY);
material.SetFloat("_Alpha", _AlphaBlur);
material.SetFloat("_Alpha2", _AlphaBlurInside);

material.SetVector("_ScreenResolution",new Vector2(Screen.width,Screen.height));
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
SCShader = Shader.Find("CameraFilterPack/BlurHole");

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