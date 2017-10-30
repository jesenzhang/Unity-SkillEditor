////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Weather/Fog")]
public class CameraFilterPack_Atmosphere_Fog : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 1f)]
public float _Near = 0f;  
[Range(0f, 1f)]
public float _Far = 0.05f;  
public Color FogColor = new Color(0.4f,0.4f,0.4f,1);
[Range(0f, 1f)]
public float Fade = 1f;  

public static Color ChangeColorRGB;
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
Texture2 = Resources.Load ("CameraFilterPack_Atmosphere_Rain_FX") as Texture2D;
SCShader = Shader.Find("CameraFilterPack/Atmosphere_Fog");

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
material.SetFloat("_DepthLevel",Fade);
material.SetFloat("_Near",_Near);
material.SetFloat("_Far",_Far);
material.SetColor("_ColorRGB",FogColor);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;
material.SetTexture("Texture2", Texture2);

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
SCShader = Shader.Find("CameraFilterPack/Atmosphere_Fog");
Texture2 = Resources.Load ("CameraFilterPack_Atmosphere_Rain_FX") as Texture2D;

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