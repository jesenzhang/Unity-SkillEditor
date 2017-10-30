using UnityEngine;
using UnityEditor;

namespace CySkillEditor
{
    public class PreviewExampleInspector : Editor
    {

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override GUIContent GetPreviewTitle()
        {
            return new GUIContent("预览");
        }

        public override void OnPreviewSettings()
        {
            GUILayout.Label("文本", "preLabel");
            GUILayout.Button("按钮", "preButton");
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            InitPreview();

            // 上下左右的旋转
            m_PreviewDir = Drag2D(m_PreviewDir, r);
            if (UnityEngine.Event.current.type != EventType.Repaint)
            {
                return;
            }

            m_PreviewUtility.BeginPreview(r, background);
            Camera camera = m_PreviewUtility.m_Camera;

            float num = Mathf.Max(m_PreviewBounds.extents.magnitude, 0.0001f);
            float num2 = num * 3.8f;
            Quaternion quaternion = Quaternion.Euler(-m_PreviewDir.y, -m_PreviewDir.x, 0f);
            Vector3 position = m_PreviewBounds.center - quaternion * (Vector3.forward * num2);
            camera.transform.position = position;
            camera.transform.rotation = quaternion;
            camera.nearClipPlane = num2 - num * 1.1f;
            camera.farClipPlane = num2 + num * 1.1f;

            SetEnabledRecursive(m_PreviewInstance, true);
            camera.Render();
            SetEnabledRecursive(m_PreviewInstance, false);
            m_PreviewUtility.EndAndDrawPreview(r);
        }

        private PreviewRenderUtility m_PreviewUtility;
        private GameObject m_PreviewInstance;
        public GameObject model;

        // 预览对象的包围盒
        private Bounds m_PreviewBounds;
        // 预览的方向
        private Vector2 m_PreviewDir = new Vector2(120f, -20f);

        // 预览摄像机的绘制层 Camera.PreviewCullingLayer
        // 为了防止引擎更改，可以通过反射获取，这里直接写值
        private const int kPreviewCullingLayer = 31;

        private void InitPreview()
        {
            if (m_PreviewUtility == null)
            {
                // 参数true代表绘制场景内的游戏对象
                m_PreviewUtility = new PreviewRenderUtility(true);
                // 设置摄像机的一些参数
                m_PreviewUtility.m_CameraFieldOfView = 30f;
                // 设置摄像机
                m_PreviewUtility.m_Camera.cullingMask = 1 << kPreviewCullingLayer;


            }  // 创建预览的游戏对象
            CreatePreviewInstances();
        }

        private void DestroyPreview()
        {
            if (m_PreviewUtility != null)
            {
                // 务必要进行清理，才不会残留生成的摄像机对象等
                m_PreviewUtility.Cleanup();
                m_PreviewUtility = null;
            }
        }

        private void CreatePreviewInstances()
        {
            DestroyPreviewInstances();
            // 实例化对象
            m_PreviewInstance = Instantiate(model, Vector3.zero, Quaternion.identity) as GameObject;
            // 递归设置隐藏标志和层
            InitInstantiatedPreviewRecursive(m_PreviewInstance);
            // 关闭对象渲染
            SetEnabledRecursive(m_PreviewInstance, false);

            m_PreviewBounds = new Bounds(m_PreviewInstance.transform.position, Vector3.zero);
            GetRenderableBoundsRecurse(ref m_PreviewBounds, m_PreviewInstance);
        }

        private void DestroyPreviewInstances()
        {
            if (m_PreviewInstance != null)
            {
                DestroyImmediate(m_PreviewInstance);
            }
            m_PreviewInstance = null;
        }

        void OnDestroy()
        {
            DestroyPreviewInstances();
            DestroyPreview();
        }

        private static void InitInstantiatedPreviewRecursive(GameObject go)
        {
            go.hideFlags = HideFlags.HideAndDontSave;
            go.layer = kPreviewCullingLayer;
            foreach (Transform transform in go.transform)
            {
                InitInstantiatedPreviewRecursive(transform.gameObject);
            }
        }

        public static void SetEnabledRecursive(GameObject go, bool enabled)
        {
            Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
            for (int i = 0; i < componentsInChildren.Length; i++)
            {
                Renderer renderer = componentsInChildren[i];
                renderer.enabled = enabled;
            }
        }

        public static void GetRenderableBoundsRecurse(ref Bounds bounds, GameObject go)
        {
            MeshRenderer meshRenderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter meshFilter = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            if (meshRenderer && meshFilter && meshFilter.sharedMesh)
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = meshRenderer.bounds;
                }
                else
                {
                    // 扩展包围盒，以让包围盒能够包含另一个包围盒
                    bounds.Encapsulate(meshRenderer.bounds);
                }
            }
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            if (skinnedMeshRenderer && skinnedMeshRenderer.sharedMesh)
            {
                if (bounds.extents == Vector3.zero)
                {
                    bounds = skinnedMeshRenderer.bounds;
                }
                else
                {
                    bounds.Encapsulate(skinnedMeshRenderer.bounds);
                }
            }
            foreach (Transform transform in go.transform)
            {
                GetRenderableBoundsRecurse(ref bounds, transform.gameObject);
            }
        }

        public static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            UnityEngine.Event current = UnityEngine.Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && position.width > 50f)
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        // 让鼠标可以拖动到屏幕外后，从另一边出来
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        // 按住 Shift 键后，可以加快旋转
                        scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return scrollPosition;
        }
    }
}