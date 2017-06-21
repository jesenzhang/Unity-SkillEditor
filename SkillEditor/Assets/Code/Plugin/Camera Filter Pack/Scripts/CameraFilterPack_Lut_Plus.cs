////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Lut/Plus")]
public class CameraFilterPack_Lut_Plus : MonoBehaviour {
#region Variables
public Shader SCShader;
private float TimeX = 1.0f;
private Vector4 ScreenResolution;
private Material SCMaterial;
public Texture2D LutTexture = null;
private Texture3D converted3DLut = null;
[Range(0f, 1f)] public float Blend = 1f;
private bool LutChange=false;
private string MemoPath;



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
SCShader = Shader.Find("CameraFilterPack/Lut_Plus");
if(!SystemInfo.supportsImageEffects)
{
enabled = false;
return;
}

}

    public void SetIdentityLut()
    {
        int dim = 16;
        var newC = new Color[dim * dim * dim];
        float oneOverDim = 1.0f / (1.0f * dim - 1.0f);

        for (int i = 0; i < dim; i++)
        {
            for (int j = 0; j < dim; j++)
            {
                for (int k = 0; k < dim; k++)
                {
                    newC[i + (j * dim) + (k * dim * dim)] = new Color((i * 1.0f) * oneOverDim, (j * 1.0f) * oneOverDim, (k * 1.0f) * oneOverDim, 1.0f);
                }
            }
        }

        if (converted3DLut) DestroyImmediate(converted3DLut);
        converted3DLut = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
        converted3DLut.SetPixels(newC);
        converted3DLut.Apply();
        
    }

    public bool ValidDimensions(Texture2D tex2d)
    {
        if (!tex2d) return false;
        int h = tex2d.height;
        if (h != Mathf.FloorToInt(Mathf.Sqrt(tex2d.width)))
        {
            return false;
        }
        return true;
    }

    public void Convert(Texture2D temp2DTex)
    {

        if (temp2DTex)
        {
            int dim = temp2DTex.width * temp2DTex.height;
            dim = temp2DTex.height;

            if (!ValidDimensions(temp2DTex))
            {
                Debug.LogWarning("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT.");

                return;
            }
            if (temp2DTex.GetPixel(5, 5) == null) { Debug.Log("bug"); return; }

#if UNITY_EDITOR
            if (Application.isPlaying != true)
            {
                string path = AssetDatabase.GetAssetPath(LutTexture);
                MemoPath = path;
                TextureImporter textureImporter = AssetImporter.GetAtPath(path) as TextureImporter;
                if (!textureImporter.isReadable)
                {
                    bool doImport = textureImporter.isReadable == false;
                if (textureImporter.mipmapEnabled == true)
                {
                    doImport = true;
                }
                if (textureImporter.textureFormat != TextureImporterFormat.AutomaticTruecolor)
                {
                    doImport = true;
                }
                textureImporter.isReadable = true;
                textureImporter.mipmapEnabled = false;
                textureImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
                AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
            }
                }
                #endif

            var c = temp2DTex.GetPixels();
                var newC = new Color[c.Length];

                for (int i = 0; i < dim; i++)
                {
                    for (int j = 0; j < dim; j++)
                    {
                        for (int k = 0; k < dim; k++)
                        {
                            int j_ = dim - j - 1;
                            newC[i + (j * dim) + (k * dim * dim)] = c[k * dim + i + j_ * dim * dim];
                        }
                    }
                }
            

            if (converted3DLut) DestroyImmediate(converted3DLut);
            converted3DLut = new Texture3D(dim, dim, dim, TextureFormat.ARGB32, false);
            converted3DLut.SetPixels(newC);
            converted3DLut.Apply();
           
        }
        else
        {
            SetIdentityLut();
     
        }
    }

    void OnRenderImage (RenderTexture sourceTexture, RenderTexture destTexture)
{
if ((SCShader != null) || (!SystemInfo.supports3DTextures))
{
TimeX+=Time.deltaTime;
if (TimeX>100)  TimeX=0;

            if (converted3DLut == null)
            {
                Convert(LutTexture);
            }
           
            int lutSize = converted3DLut.width;
            converted3DLut.wrapMode = TextureWrapMode.Clamp;
            material.SetFloat("_Blend", Blend);
            material.SetTexture("_LutTex", converted3DLut);
            Graphics.Blit(sourceTexture, destTexture, material, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);
}
else
{
Graphics.Blit(sourceTexture, destTexture);	
}
}
    void OnValidate()
    {
#if UNITY_EDITOR

        string path = AssetDatabase.GetAssetPath(LutTexture);
        if (MemoPath != path) Convert(LutTexture);
#endif
    }
    void Update () 
{
#if UNITY_EDITOR
if (Application.isPlaying!=true)
{
SCShader = Shader.Find("CameraFilterPack/Lut_Plus");
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
