using System;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(BlockData))]
public class BlockDataEditor : Editor
{

    private bool upgradesFoldout = true;


    private SerializedProperty blockNameProp;
    private SerializedProperty spriteProp;
    private SerializedProperty powerValuesProp;
    private SerializedProperty attackRangeProp;
    private SerializedProperty upgradesProp;


    private void OnEnable()
    {
        blockNameProp = serializedObject.FindProperty("BlockName");
        spriteProp = serializedObject.FindProperty("Sprite");
        powerValuesProp = serializedObject.FindProperty("PowerValues");
        attackRangeProp = serializedObject.FindProperty("attackRange");
        upgradesProp = serializedObject.FindProperty("DefaultUpgrades");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        // This is for handling the information that Unity could already serialise
        EditorGUILayout.PropertyField(blockNameProp);
        EditorGUILayout.PropertyField(spriteProp);
        EditorGUILayout.PropertyField(powerValuesProp);
        EditorGUILayout.PropertyField(attackRangeProp);

        EditorGUILayout.Space();

        // This is for the upgrades which Unity could not Serialise by default

        //EditorGUILayout.LabelField("Default Upgrades", EditorStyles.boldLabel);

        upgradesFoldout = EditorGUILayout.Foldout(upgradesFoldout, "Default Upgrades", true, EditorStyles.foldoutHeader);

        if (upgradesFoldout)
        {

            for (int i = 0; i < upgradesProp.arraySize; i++)
            {
                SerializedProperty element = upgradesProp.GetArrayElementAtIndex(i);

                EditorGUILayout.BeginVertical("box");

                EditorGUILayout.BeginHorizontal();

                if (element.managedReferenceValue != null)
                {
                    string upgradeName =
                        GetUpgradeDisplayName(
                            (BlockUpgrade)element.managedReferenceValue);

                    EditorGUILayout.LabelField(
                        upgradeName,
                        EditorStyles.boldLabel);
                }
                else
                {
                    EditorGUILayout.LabelField(
                        "Empty Upgrade",
                        EditorStyles.boldLabel);
                }

                if (GUILayout.Button("X", GUILayout.Width(25)))
                {
                    Undo.RecordObject(target, "Remove Block Upgrade");

                    upgradesProp.DeleteArrayElementAtIndex(i);
                    break;
                }

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.PropertyField(element, GUIContent.none, true);


                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.Space();

            // This handles the "Add Upgrade button"
            if (GUILayout.Button("+ Add Upgrade"))
            {
                GenericMenu menu = new GenericMenu();

                foreach (Type type in TypeCache.GetTypesDerivedFrom<BlockUpgrade>())
                {
                    if (type.IsAbstract)
                        continue;

                    Type selectedType = type;

                    menu.AddItem(
                        new GUIContent(selectedType.Name),
                        false,
                        () => AddUpgrade(selectedType)
                    );
                }

                menu.ShowAsContext();
            }

        }

        serializedObject.ApplyModifiedProperties();

    }


    // This is the method that actually adds the upgrade once clicked in the inspector
    private void AddUpgrade(Type upgradeType)
    {

        Undo.RecordObject(target, "Add Block Upgrade");

        serializedObject.Update();

        int index = upgradesProp.arraySize;

        upgradesProp.arraySize++;

        SerializedProperty element =
            upgradesProp.GetArrayElementAtIndex(index);

        //This is the part that actually creates teh upgrade
        element.managedReferenceValue =
            Activator.CreateInstance(upgradeType);

        serializedObject.ApplyModifiedProperties();
    }


    // This method is soley to make upgrade names display in a nicer way in the incpector (with spaces in between words)
    private string GetUpgradeDisplayName(BlockUpgrade upgrade)
    {
        string name = upgrade.GetType().Name;

        if (name.EndsWith("Upgrade"))
        {
            name = name.Substring(
                0,
                name.Length - "Upgrade".Length);
        }

        //This part finds capital Letters
        System.Text.RegularExpressions.Regex regex =
            new System.Text.RegularExpressions.Regex(
                "(?<!^)([A-Z])");

        name = regex.Replace(name, " $1");

        return name ; // This originally contained "+ Upgrade" but i think it is better without
    }
}
