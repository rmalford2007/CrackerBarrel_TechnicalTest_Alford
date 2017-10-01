using UnityEditor;
using UnityEditor.EventSystems;

[CustomEditor(typeof(TextGrow_MouseHover))]
public class TextGrow_Editor : EventTriggerEditor
{
    SerializedProperty sizeCurveProperty;
    SerializedProperty idleCurveProperty;
    protected override void OnEnable()
    {
        base.OnEnable();

        sizeCurveProperty = serializedObject.FindProperty("sizeCurve");
        idleCurveProperty = serializedObject.FindProperty("idlePulseCurve");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        EditorGUILayout.PropertyField(sizeCurveProperty);
        EditorGUILayout.PropertyField(idleCurveProperty);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}

