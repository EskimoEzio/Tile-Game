using UnityEditor;
using UnityEngine;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

public class UpgradeScriptCreator : EditorWindow
{
    private string upgradeName = "NewUpgrade";
    private string folderPath;

    public static void Open(string folderPath)
    {
        UpgradeScriptCreator window =
            GetWindow<UpgradeScriptCreator>(
                true,
                "Create Upgrade Script"
            );

        window.folderPath = folderPath;
        window.upgradeName = "NewUpgrade";

        window.minSize = new Vector2(350, 100);
        window.maxSize = new Vector2(350, 100);

        window.ShowUtility();
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField(
            "Upgrade Name",
            EditorStyles.boldLabel
        );

        GUI.SetNextControlName("UpgradeNameField");

        upgradeName = EditorGUILayout.TextField(
            upgradeName
        );

        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal();

        if (GUILayout.Button("Create"))
        {
            CreateScript();
        }

        if (GUILayout.Button("Cancel"))
        {
            Close();
        }

        EditorGUILayout.EndHorizontal();

        if (Event.current.type == EventType.KeyDown &&
            Event.current.keyCode == KeyCode.Return)
        {
            CreateScript();
        }

        if (Event.current.type == EventType.KeyDown &&
            Event.current.keyCode == KeyCode.Escape)
        {
            Close();
        }
    }

    private void CreateScript()
    {
        if (string.IsNullOrWhiteSpace(upgradeName))
        {
            EditorUtility.DisplayDialog(
                "Invalid Name",
                "Please enter an upgrade name.",
                "OK"
            );

            return;
        }

        // Check existing upgrades for duplicate IDs
        Dictionary<int, List<Type>> upgradesByID =
            GetUpgradeIDs();

        List<string> duplicateIDs = upgradesByID
            .Where(pair => pair.Value.Count > 1)
            .Select(pair =>
                $"ID {pair.Key}: " +
                string.Join(", ", pair.Value.Select(type => type.Name))
            )
            .ToList();

        if (duplicateIDs.Count > 0)
        {
            EditorUtility.DisplayDialog(
                "Duplicate Upgrade IDs",
                "Duplicate UpgradeIDs were found:\n\n" +
                string.Join("\n", duplicateIDs) +
                "\n\nPlease fix these before creating a new upgrade.",
                "OK"
            );

            return;
        }

        // Find the next available ID
        int nextUpgradeID = GetNextUpgradeID(upgradesByID);

        string fileName = upgradeName + ".cs";

        string filePath = AssetDatabase.GenerateUniqueAssetPath(
            Path.Combine(folderPath, fileName)
        );

        string className =
            Path.GetFileNameWithoutExtension(filePath);

        string scriptContents =
$@"using System;
using UnityEngine;

[Serializable]
public class {className} : BlockUpgrade
{{
    public override int UpgradeID {{ get; }} = {nextUpgradeID};

    [SerializeField] private int exampleValue = 0;
}}
";

        File.WriteAllText(filePath, scriptContents);

        AssetDatabase.Refresh();

        Selection.activeObject =
            AssetDatabase.LoadAssetAtPath<MonoScript>(filePath);

        Close();

        Debug.Log(
            $"Created upgrade script: {filePath} " +
            $"with UpgradeID {nextUpgradeID}"
        );
    }

    private Dictionary<int, List<Type>> GetUpgradeIDs()
    {
        Dictionary<int, List<Type>> upgradesByID = new Dictionary<int, List<Type>>();

        var upgradeTypes = TypeCache.GetTypesDerivedFrom<BlockUpgrade>();

        foreach (Type upgradeType in upgradeTypes)
        {
            if (upgradeType.IsAbstract)
                continue;

            try
            {
                BlockUpgrade upgrade =
                    (BlockUpgrade)Activator.CreateInstance(upgradeType);

                int upgradeID = upgrade.UpgradeID;

                if (!upgradesByID.ContainsKey(upgradeID))
                {
                    upgradesByID.Add(
                        upgradeID,
                        new List<Type>()
                    );
                }

                upgradesByID[upgradeID].Add(upgradeType);
            }
            catch (Exception exception)
            {
                Debug.LogWarning(
                    $"Could not read UpgradeID from " +
                    $"{upgradeType.Name}: {exception.Message}"
                );
            }
        }

        return upgradesByID;
    }

    private int GetNextUpgradeID(Dictionary<int, List<Type>> upgradesByID)
    {
        int nextID = 1;

        while (upgradesByID.ContainsKey(nextID))
        {
            nextID++;
        }

        return nextID;
    }

    [MenuItem("Assets/Create/Tile Game/Upgrade Script")]
    public static void CreateUpgradeScript()
    {
        string folderPath = "Assets";

        if (Selection.activeObject != null)
        {
            string selectedPath =
                AssetDatabase.GetAssetPath(
                    Selection.activeObject
                );

            if (AssetDatabase.IsValidFolder(selectedPath))
            {
                folderPath = selectedPath;
            }
            else
            {
                folderPath =
                    Path.GetDirectoryName(selectedPath)
                        .Replace("\\", "/");
            }
        }

        Open(folderPath);
    }
}