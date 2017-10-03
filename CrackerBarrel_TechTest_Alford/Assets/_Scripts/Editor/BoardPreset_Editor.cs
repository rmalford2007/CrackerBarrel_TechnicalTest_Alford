using UnityEditor;

/// <summary>
/// ScriptableObject editor for BoardPreset. This class hides serialized information from the user based on the style drop down enum. Only shows one style at a time.
/// </summary>
[CustomEditor(typeof(BoardPreset))]
public class BoardPreset_Editor : Editor
{
    SerializedProperty serializedRowByRow;
    SerializedProperty serializedSingleColor;
    SerializedProperty serializedPegCount;
    SerializedProperty serializedStyle;
    public void OnEnable()
    {

        serializedPegCount = serializedObject.FindProperty("baseRowPegCount");
        serializedRowByRow = serializedObject.FindProperty("rowByRowData");
        serializedSingleColor = serializedObject.FindProperty("singleData");
        serializedStyle = serializedObject.FindProperty("selectedStyle");

    }

    public override void OnInspectorGUI()
    {

        //Manually draw this class in the desired order
        EditorGUILayout.PropertyField(serializedPegCount);
        EditorGUILayout.PropertyField(serializedStyle);
        
        //Only display the class variable chosen by the above style drop down
        if ( serializedStyle.enumValueIndex == (int)BoardPreset.ColorStyle.ROW_BY_ROW)
        {
            EditorGUILayout.PropertyField(serializedRowByRow, true);
        }
        else
        {
            EditorGUILayout.PropertyField(serializedSingleColor, true);
        }

        //Apply changes
        serializedObject.ApplyModifiedProperties();
    }


}
    

