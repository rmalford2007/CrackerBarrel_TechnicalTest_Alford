using UnityEditor;
using UnityEditor.EventSystems;

/// <summary>
/// Editor class for TextGrow_MouseHover. This was needed to show sizeCurve and idlePulseCurve in the inspector, as the base class was EventTrigger which didn't support showing them.
/// </summary>
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
        //Display missing animation curves
        EditorGUILayout.PropertyField(sizeCurveProperty);
        EditorGUILayout.PropertyField(idleCurveProperty);
        serializedObject.ApplyModifiedProperties();
        base.OnInspectorGUI();
    }
}

