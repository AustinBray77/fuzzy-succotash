using UnityEngine;
using UnityEditor;
using System.Linq;
/// <summary>
/// Drawer for the RequireInterface attribute.
/// </summary>
[CustomPropertyDrawer(typeof(RequireInterfaceAttribute))]
public class RequireInterfaceDrawer : PropertyDrawer
{
    /// <summary>
    /// Overrides GUI drawing for the attribute.
    /// </summary>
    /// <param name="position">Position.</param>
    /// <param name="property">Property.</param>
    /// <param name="label">Label.</param>
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Check if this is reference type property.
        if (property.propertyType == SerializedPropertyType.ObjectReference)
        {
            // Get attribute parameters.
            System.Type requiredAttribute = (this.attribute as RequireInterfaceAttribute).RequiredType;

            // Begin drawing property field.
            EditorGUI.BeginProperty(position, label, property);

            //Remove the "element" text from the label
            label.text = label.text.Split(' ')[1] + ": ";;

            // Draw property field.
            //Checks if you've given a proper reference, otherwise checks if you've given a gameobject and tries to find the correct component in the gameobject
            Object reference = EditorGUI.ObjectField(position, "", property.objectReferenceValue, requiredAttribute, true);
            
            if (reference is null || reference == property.objectReferenceValue)
            {
                Object obj = EditorGUI.ObjectField(position, "", property.objectReferenceValue, typeof(Object), true);

                if (obj is GameObject g)
                {
                    reference = g.GetComponent(requiredAttribute);
                }
            }

            if(reference is not null)
            {
                property.objectReferenceValue = reference;
            }
            else
            {
                //Error message?
                //Temporarily label inside box?
            }

            //Changes the text inside of the field to be the required type instead of object
            EditorGUI.ObjectField(position, "", property.objectReferenceValue, requiredAttribute, true);

            // Finish drawing property field.
            EditorGUI.EndProperty();
        }
        else
        {
            // If field is not reference, show error message.
            // Save previous color and change GUI to red.
            var previousColor = GUI.color;
            GUI.color = Color.red;
            // Display label with error message.
            EditorGUI.LabelField(position, label, new GUIContent("Invalid Reference"));
            // Revert color change.
            GUI.color = previousColor;
        }
    }
}