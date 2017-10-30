////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Split Screen/Split 3D")]
public class CameraFilterPack_Blend2Camera_SplitScreen3D : MonoBehaviour {
#region Variables
private string ShaderName="CameraFilterPack/Blend2Camera_SplitScreen3D";
public Shader SCShader;
public Camera Camera2; 
private float TimeX = 1.0f;

private Material SCMaterial;
[Range(0f, 100f)]
public float _FixDistance = 1f;
[Range(-0.99f, 0.99f)]
public float _Distance = 0.5f;
    [Range(0f, 0.5f)]
    public float _Size = 0.1f;

    [Range(0f, 1f)]

public float SwitchCameraToCamera2 = 0f;
[Range(0f, 1f)] 
public float BlendFX = 1f;
[Range(-3f, 3f)]
public float SplitX = 0.5f;
[Range(-3f, 3f)]
public float SplitY = 0.5f;
[Range(0f, 2f)]
public float Smooth = 0.1f;
[Range(-3.14f, 3.14f)]
public float Rotation = 3.14f;
private bool ForceYSwap = false;

private RenderTexture Camera2tex;
private Vector2 ScreenSize;
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
void OnValidate()
{
        ScreenSize.x = Screen.width;
        ScreenSize.y = Screen.height;
}
void Start ()
{
if (Camera2 !=null)
{
Camera2tex=new RenderTexture((int)ScreenSize.x ,(int)ScreenSize.y, 24); 
Camera2.targetTexture=Camera2tex;
}

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
material.SetFloat("_Near", _Distance);
material.SetFloat("_Far", _Size);
material.SetFloat("_FixDistance", _FixDistance);
material.SetFloat("_TimeX", TimeX);
material.SetFloat("_Value", BlendFX);
material.SetFloat("_Value2", SwitchCameraToCamera2);
material.SetFloat("_Value3", SplitX);
material.SetFloat("_Value6", SplitY);
material.SetFloat("_Value4", Smooth);
material.SetFloat("_Value5", Rotation);
material.SetInt ("_ForceYSwap", ForceYSwap ? 0:1 );
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
ScreenSize.x = Screen.width;
ScreenSize.y = Screen.height;

#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find(ShaderName);
}
#endif
}
void OnEnable () { Start (); }
void OnDisable ()
{
if (Camera2 !=null) {  Camera2.targetTexture=null; }
if(SCMaterial)
{
DestroyImmediate(SCMaterial);
}
}
}
