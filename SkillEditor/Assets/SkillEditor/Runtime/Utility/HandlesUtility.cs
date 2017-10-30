using UnityEngine;
using UnityEditor;
using System.Collections;
namespace CySkillEditor
{
    public static class HandlesUtility
    {
        static private Color xAxisColor = new Color(0.858823538f, 0.243137255f, 0.113725491f, 0.93f);
        static private Color yAxisColor = new Color(0.6039216f, 0.9529412f, 0.282352954f, 0.93f);
        static private Color zAxisColor = new Color(0.227450982f, 0.478431374f, 0.972549f, 0.93f);
        static private Color centerColor = new Color(0.8f, 0.8f, 0.8f, 0.93f);
        static private Color TangentColor = new Color(1f, 0f, 0.804f, 0.93f);
        static private Color ControlColor = new Color(1f, 0.4f, 0.10196f, 0.93f);

        static public Vector3 ControlHandle(Vector3 tangent)
        {
            var snapX = EditorPrefs.GetFloat("MoveSnapX");
            var snapY = EditorPrefs.GetFloat("MoveSnapY");
            var snapZ = EditorPrefs.GetFloat("MoveSnapZ");
            var snapMove = new Vector3(snapX, snapY, snapZ);

            float handleSize = GetHandleSize(tangent);
            Color color = Handles.color;
            Handles.color = ControlColor;
            tangent = Handles.FreeMoveHandle(tangent, Quaternion.identity, handleSize * 0.15f, snapMove, new Handles.CapFunction(Handles.RectangleHandleCap));
            
            Handles.color = color;

            return tangent;
        }

        static public Vector3 TangentHandle(Vector3 tangent)
        {
            var snapX = EditorPrefs.GetFloat("MoveSnapX");
            var snapY = EditorPrefs.GetFloat("MoveSnapY");
            var snapZ = EditorPrefs.GetFloat("MoveSnapZ");
            var snapMove = new Vector3(snapX, snapY, snapZ);

            float handleSize = GetHandleSize(tangent);
            Color color = Handles.color;
            Handles.color = TangentColor;
            tangent = Handles.FreeMoveHandle(tangent, Quaternion.identity, handleSize * 0.15f, snapMove, new Handles.CapFunction(Handles.RectangleHandleCap));
            Handles.color = color;

            return tangent;
        }

        static public Vector3 PositionHandle(Vector3 position, Quaternion rotation)
        {
            var snapX = EditorPrefs.GetFloat("MoveSnapX");
            var snapY = EditorPrefs.GetFloat("MoveSnapY");
            var snapZ = EditorPrefs.GetFloat("MoveSnapZ");
            var snapMove = new Vector3(snapX, snapY, snapZ);

            float handleSize = GetHandleSize(position);
            Color color = Handles.color;
            Handles.color = xAxisColor;
            position = Handles.Slider(position, rotation * Vector3.right, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapX);
            Handles.color = yAxisColor;
            position = Handles.Slider(position, rotation * Vector3.up, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapY);
            Handles.color = zAxisColor;
            position = Handles.Slider(position, rotation * Vector3.forward, handleSize, new Handles.CapFunction(Handles.ArrowHandleCap), snapZ);
            Handles.color = centerColor;
            position = Handles.FreeMoveHandle(position, rotation, handleSize * 0.15f, snapMove, new Handles.CapFunction(Handles.ArrowHandleCap));
            Handles.color = color;

            return position;
        }

        static public float GetHandleSize(Vector3 position)
        {
            Camera current = Camera.current;
            position = Handles.matrix.MultiplyPoint(position);
            if (current)
            {
                Transform transform = current.transform;
                Vector3 position2 = transform.position;
                float z = Vector3.Dot(position - position2, transform.TransformDirection(new Vector3(0f, 0f, 1f)));
                Vector3 a = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(0f, 0f, z)));
                Vector3 b = current.WorldToScreenPoint(position2 + transform.TransformDirection(new Vector3(1f, 0f, z)));
                float magnitude = (a - b).magnitude;
                return 40f / Mathf.Max(magnitude, 0.0001f);
            }
            return 20f;
        }
    }
}