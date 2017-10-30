////////////////////////////////////////////
// CameraFilterPack - by VETASOFT 2016 /////
////////////////////////////////////////////
using UnityEngine;
using System.Collections;
[ExecuteInEditMode]
[AddComponentMenu ("Camera Filter Pack/Pixelisation/Pixelisation_Sweater")]
public class CameraFilterPack_Pixelisation_Sweater : MonoBehaviour {

        #region Variables
        public Shader SCShader;
        private float TimeX = 1.0f;
        private Vector4 ScreenResolution;
        private Material SCMaterial;
        [Range(16f, 128f)]
        public float SweaterSize = 64f;
        [Range(0f, 2f)]
        public float _Intensity = 1.4f;
        [Range(0f, 1f)]
        public float Fade = 1f;
        private Texture2D Texture2;
        #endregion
        #region Properties
        Material material
        {
            get
            {
                if (SCMaterial == null)
                {
                    SCMaterial = new Material(SCShader);
                    SCMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                return SCMaterial;
            }
        }
        #endregion
        void Start()
        {
            Texture2 = Resources.Load("CameraFilterPack_Sweater") as Texture2D;
            SCShader = Shader.Find("CameraFilterPack/Pixelisation_Sweater");
            if (!SystemInfo.supportsImageEffects)
            {
                enabled = false;
                return;
            }
        }
        void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
        {
            if (SCShader != null)
            {
                TimeX += Time.deltaTime;
                if (TimeX > 100) TimeX = 0;
                material.SetFloat("_TimeX", TimeX);
                material.SetFloat("_Fade", Fade);
                material.SetFloat("_SweaterSize", SweaterSize);
                material.SetFloat("_Intensity", _Intensity);
                material.SetTexture("Texture2", Texture2);
                Graphics.Blit(sourceTexture, destTexture, material);
            }
            else
            {
                Graphics.Blit(sourceTexture, destTexture);
            }
        }
        void Update()
        {
#if UNITY_EDITOR
            if (Application.isPlaying != true)
            {
                SCShader = Shader.Find("CameraFilterPack/Pixelisation_Sweater");
                Texture2 = Resources.Load("CameraFilterPack_Sweater") as Texture2D;
            }
#endif
        }
        void OnDisable()
        {
            if (SCMaterial)
            {
                DestroyImmediate(SCMaterial);
            }
        }
    }