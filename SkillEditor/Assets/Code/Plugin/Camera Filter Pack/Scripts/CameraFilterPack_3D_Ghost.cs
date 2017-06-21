///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/3D/Ghost")]
public class CameraFilterPack_3D_Ghost : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 5f;  
[Range(-0.5f, 0.99f)]
public float Ghost_Near = 0.08f;
[Range(0f, 1f)]
public float Ghost_Far = 0.55f;

[Range(0f, 2f)]
public float Intensity = 1f;

[Range(0f, 1f)]
public float GhostWithoutObject = 1;


[Range(-1f, 1f)]
public float GhostPosX = 0f;
[Range(-1f, 1f)]
public float GhostPosY = 0f;
[Range(0.1f, 8f)]
public float GhostFade2 = 2f;
[Range(-1f, 1f)]
public float GhostFade = 0f;
[Range(0.5f, 1.5f)]
public float GhostSize = 0.9f;




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

SCShader = Shader.Find("CameraFilterPack/3D_Ghost");
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
material.SetFloat("_Value2", Intensity);
material.SetFloat("GhostPosX", GhostPosX);
material.SetFloat("GhostPosY", GhostPosY);
material.SetFloat("GhostFade", GhostFade);
material.SetFloat("GhostFade2", GhostFade2);
material.SetFloat("GhostSize", GhostSize);
           

material.SetFloat("_FixDistance",_FixDistance);

material.SetFloat("Drop_Near", Ghost_Near);
material.SetFloat("Drop_Far", Ghost_Far);
material.SetFloat("Drop_With_Obj", GhostWithoutObject);


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
			SCShader = Shader.Find("CameraFilterPack/3D_Ghost");

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
