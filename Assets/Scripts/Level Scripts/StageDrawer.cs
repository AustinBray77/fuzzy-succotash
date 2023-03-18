using UnityEditor;
using UnityEngine;

// IngredientDrawer
[CustomPropertyDrawer(typeof(LevelProgresser.Stage))]
public class IngredientDrawer : PropertyDrawer
{
    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        //Is there a better way to find the index in the array, other than getting it from the previous label
        string index = label.text.Split(' ')[1];
        label.text = "Stage: " + index; 

        // Draw label
        EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Don't make child fields be indented
        var indent = EditorGUI.indentLevel;
        EditorGUI.indentLevel = 0;

        // Calculate rects (split space evenly into two rects)
        float gap = 20;
        
        //float width = (position.width * 0.91f) + -100;
        float offset = 75; //offset accounts for the stage text at the beginning:
        float width = (position.width * 0.5f) - gap/2 - offset/2;

        var rect1 = new Rect(position.x + position.width - width - gap - width, position.y, width, position.height);
        var rect2 = new Rect(position.x + position.width - width, position.y, width, position.height);

        // Draw fields - you can pass GUIContent.none to each so they are drawn without labels
        EditorGUI.PropertyField(rect1, property.FindPropertyRelative("_activations"));
        EditorGUI.PropertyField(rect2, property.FindPropertyRelative("_deactivations"));

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        //float height1 = 20 + (property.FindPropertyRelative("_activations").isExpanded ? 40 : 0) + property.FindPropertyRelative("_activations").arraySize * 20;
        //float height2 = 20 + (property.FindPropertyRelative("_deactivations").isExpanded ? 40 : 0) + property.FindPropertyRelative("_deactivations").arraySize * 20;

        float height1 = 20;
        if(property.FindPropertyRelative("_activations").isExpanded)
        {
            if (property.FindPropertyRelative("_activations").arraySize == 0)
            {
                height1 += 50;
            }
            else
            {
                height1 += 30 + (property.FindPropertyRelative("_activations").arraySize * 20);
            }
        }

        float height2 = 20;
        if (property.FindPropertyRelative("_deactivations").isExpanded)
        {
            if (property.FindPropertyRelative("_deactivations").arraySize == 0)
            {
                height2 += 50;
            }
            else
            {
                height2 += 30 + (property.FindPropertyRelative("_deactivations").arraySize * 20);
            }
        }

        return Mathf.Max(height1, height2);
    }
}