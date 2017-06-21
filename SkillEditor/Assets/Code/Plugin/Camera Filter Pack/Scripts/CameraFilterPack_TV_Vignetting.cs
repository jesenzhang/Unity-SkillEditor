///////////////////////////////////////////
//  CameraFilterPack - by VETASOFT 2016 ///
///////////////////////////////////////////

using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/TV/Vignetting")]
public class CameraFilterPack_TV_Vignetting : MonoBehaviour {
#region Variables
public Shader SCShader;
private Material SCMaterial;
private Texture2D Vignette;

[Range(0, 1)] public float Vignetting = 1f;
[Range(0, 1)] public float VignettingFull = 0f;
[Range(0, 1)] public float VignettingDirt = 0f;
    public Color VignettingColor = new Color(0,0,0,1);
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
SCShader = Shader.Find("CameraFilterPack/TV_Vignetting");
Vignette = Resources.Load ("CameraFilterPack_TV_Vignetting1") as Texture2D;

if (!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}
}

void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if(SCShader != null)
{
material.SetTexture("Vignette", Vignette);
material.SetFloat("_Vignetting", Vignetting);
material.SetFloat("_Vignetting2", VignettingFull);
material.SetColor("_VignettingColor", VignettingColor);
material.SetFloat("_VignettingDirt", VignettingDirt);

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
SCShader = Shader.Find("CameraFilterPack/TV_Vignetting");
Vignette = Resources.Load ("CameraFilterPack_TV_Vignetting1") as Texture2D;
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