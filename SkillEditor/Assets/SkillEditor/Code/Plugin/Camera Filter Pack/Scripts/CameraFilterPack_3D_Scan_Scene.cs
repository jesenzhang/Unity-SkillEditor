////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/3D/Scan_Scene")]
public class CameraFilterPack_3D_Scan_Scene : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 1f;  
[Range(0f, 0.99f)]
public float _Distance = 1f;  
[Range(0f, 0.1f)]
public float _Size = 0.01f;  
public bool AutoAnimatedNear=false;
[Range(-5f, 5f)]
public float AutoAnimatedNearSpeed=1f;
public Color ScanColor = new Color(2.0f,0.0f,0.0f,1);
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
SCShader = Shader.Find("CameraFilterPack/3D_Scan_Scene");

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
if (AutoAnimatedNear)
{
_Distance+=Time.deltaTime*AutoAnimatedNearSpeed;
if (_Distance>1) _Distance=0;
if (_Distance<0) _Distance=1;
material.SetFloat("_Near",_Distance);
}
else
{
material.SetFloat("_Near",_Distance);
}
material.SetFloat("_Far",_Size);
material.SetColor("_ColorRGB",ScanColor);
material.SetFloat("_FixDistance",_FixDistance);

float _FarCamera = GetComponent<Camera>().farClipPlane; 
material.SetFloat("_FarCamera",1000/_FarCamera);
material.SetVector("_ScreenResolution",new Vector4(sourceTexture.width,sourceTexture.height,0.0f,0.0f));
GetComponent<Camera>().depthTextureMode = DepthTextureMode.Depth;

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
SCShader = Shader.Find("CameraFilterPack/3D_Scan_Scene");

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