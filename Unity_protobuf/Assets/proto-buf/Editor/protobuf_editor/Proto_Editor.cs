using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using BnBCustomEditor.Utils.Proto;
using System.IO;

public class Proto_Editor : EditorWindow {
    private SettingData m_settingData;
    private const string DATABASE_PATH = @"Assets/proto-buf/Editor/Database/ProtoSettingDatabase.asset";

    private const string DATABASE_CONFIG_PATH = @"/proto-buf/Editor/Database/ProtoSetting.json";

    private ProtoSettingDatabase m_protoSetting;
    private Vector2 _scrollPos;

    [MenuItem("Tools/ProtoBuf")]
    public static void Init() {
        Proto_Editor window = EditorWindow.GetWindow<Proto_Editor>();
        window.minSize = new Vector2(800, 400);
        window.Show();
        // m_settingData = new SettingData();
    }

    void OnEnable() {
        if (m_settingData == null) LoadDatabase();
    }

    void OnGUI() {
        //EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DisplayUIInfo();
        //EditorGUILayout.EndHorizontal();
    }

    private string GetConfigPath() {
        return Application.dataPath + DATABASE_CONFIG_PATH;
    }

    void LoadDatabase() {
        string configPath = GetConfigPath();
        if (File.Exists(configPath)) {
            string json = File.ReadAllText(configPath);
            Debug.Log(json);
            if (string.IsNullOrEmpty(json)) {
                CreateDatabase();
            } else {
                m_settingData = JsonUtility.FromJson<SettingData>(json);
            }
        } else {
            CreateDatabase();
        }
    }

    void CreateDatabase() {
        // m_protoSetting = ScriptableObject.CreateInstance<ProtoSettingDatabase>();
        // AssetDatabase.CreateAsset(m_protoSetting, DATABASE_PATH);
        // AssetDatabase.SaveAssets();
        // AssetDatabase.Refresh();
        m_settingData = new SettingData();
        string json = JsonUtility.ToJson(m_settingData);
        File.WriteAllText(GetConfigPath(), json);
    }

    private void CustomSetDirty() {
        string json = JsonUtility.ToJson(m_settingData);
        File.WriteAllText(GetConfigPath(), json);
    }

    private void DisplayUIInfo() {
        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Set Tool ", GUILayout.Width(120))) {
            string path = EditorUtility.OpenFilePanel("Select Tool ProtoGen.exe in ProtoBuf", "", "");
            Debug.Log(path);
            m_settingData.ToolPath = path.Replace("/", "\\");
            Debug.Log(m_settingData.ToolPath);
            CustomSetDirty();
        }
        m_settingData.ToolPath = EditorGUILayout.TextField(new GUIContent(""), m_settingData.ToolPath);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Set Output Dir", GUILayout.Width(120))) {
            string path = EditorUtility.OpenFolderPanel("Select Output Dir for C# files", "", "");

            m_settingData.OutputPath = path.Replace("/", "\\");
            CustomSetDirty();
        }

        m_settingData.OutputPath = EditorGUILayout.TextField(new GUIContent(""), m_settingData.OutputPath);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));

        if (GUILayout.Button("Proto Files Dir", GUILayout.Width(120))) {
            string path = EditorUtility.OpenFolderPanel("Select Proto Files Dir", "", "");

            m_settingData.ProtoFilesPath = path.Replace("/", "\\");
            CustomSetDirty();
        }

        m_settingData.ProtoFilesPath = EditorGUILayout.TextField(new GUIContent(""), m_settingData.ProtoFilesPath);

        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        //newSprite.Id = EditorGUILayout.TextField(new GUIContent("Name : "), newSprite.Id);
        //newSprite.SpritePath = EditorGUILayout.TextField(new GUIContent("Sprite Path : "), newSprite.SpritePath);
        //newSprite.Type = (SpriteCatagory)EditorGUI.EnumPopup(GUILayoutUtility.GetRect(0.0f, 20.0f, GUILayout.ExpandWidth(true)), "Catagory:", newSprite.Type);

        EditorGUILayout.BeginVertical(); //GUILayout.ExpandWidth(true)
        EditorGUILayout.LabelField("Protobuf File Names");
        for (int i = 0; i < m_settingData.ProtoFiles.Count; i++) {
            var fileName = m_settingData.ProtoFiles[i];
            fileName = EditorGUILayout.TextField(new GUIContent((i + 1).ToString() + " : "), fileName);
        }
        //if (GUILayout.Button("Remove Last Name", GUILayout.Width(120)))
        //{

        //}
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginHorizontal(GUILayout.ExpandWidth(true));
        DropPrefabArea(m_settingData);
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.Space();
        EditorGUILayout.Space();

        if (GUILayout.Button("Generate C# source code from Proto code", GUILayout.ExpandWidth(true))) {
            //  m_settingData.ToolPath + " " +
            m_settingData.Command = "--proto_path=" + m_settingData.ProtoFilesPath;
            for (int i = 0; i < m_settingData.ProtoFiles.Count; i++) {
                m_settingData.Command += " " + m_settingData.ProtoFiles[i];
            }
            m_settingData.Command += " " + "-output_directory=" + m_settingData.OutputPath;

            // Debug.Log(m_settingData.Command);

            // var cmd = System.Diagnostics.Process.Start("cmd.exe", m_settingData.Command);
            System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
            startInfo.WorkingDirectory = @"d:\";
            startInfo.FileName = m_settingData.ToolPath;
            startInfo.Arguments = m_settingData.Command;
            System.Diagnostics.Process.Start(startInfo);
        }

        if (GUILayout.Button("Refresh AssetDatabase", GUILayout.ExpandWidth(true))) {
            // AssetDatabase.Refresh();
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        if (GUILayout.Button("Build Event Map", GUILayout.ExpandWidth(true))) {
            OutPutEventMap();
        }
    }

    private const string EventMapFile = "ProtoEventName.cs";

    private Dictionary<string, int> m_eventMap = new Dictionary<string, int>();
    private Dictionary<string, string> m_eventDescMap = new Dictionary<string, string>();
    private string pattern = @"//(?<caseName>\w+)\[(?<id>\d{6})\]\s*message\s+(?<typeName>\w+)";

    private void OutPutEventMap() {
        m_eventMap.Clear();
        m_eventDescMap.Clear();

        var protoPath = m_settingData.ProtoFilesPath;

        for (int i = 0; i < m_settingData.ProtoFiles.Count; i++) {
            var protoFile = m_settingData.ProtoFiles[i];
            var protoFilePath = protoPath + "/" + protoFile;
            string txt = File.ReadAllText(protoFilePath);
            MatchCollection mathes = Regex.Matches(txt, pattern, RegexOptions.Singleline);

            foreach (Match mathe in mathes) {
                if (mathe.Success) {
                    var caseName = mathe.Groups["caseName"].Value;
                    var id = mathe.Groups["id"].Value;
                    var typeName = mathe.Groups["typeName"].Value;
                    m_eventMap.Add(typeName, int.Parse(id));
                    m_eventDescMap.Add(typeName, caseName);
                    Debug.Log("caseName: " + caseName + " id: " + id + " typeName: " + typeName);
                }
            }
        }
        GenerateProtoEventName();
    }

    //生成ProtoEventName.cs
    private void GenerateProtoEventName() {
        string path = m_settingData.OutputPath + "/" + EventMapFile;
        using (StreamWriter sw = new StreamWriter(path)) {
            sw.WriteLine("//-----------Auto Generate ByTools-----------");
            //日期加时间
            sw.WriteLine("//-----------Time:" + DateTime.Now.ToString("s") + "-----------");
            sw.WriteLine("//-----------Don't change-----------");
            sw.WriteLine("public enum ProtoEventName");
            sw.WriteLine("{");
            foreach (var item in m_eventMap) {
                string desc = m_eventDescMap[item.Key];
                sw.WriteLine($"\t/// <summary>");
                sw.WriteLine($"\t/// {desc}");
                sw.WriteLine($"\t/// </summary>");
                sw.WriteLine($"\t{item.Key} = {item.Value},");
            }
            sw.WriteLine("}");
            sw.WriteLine("//-----------End-----------");
        }
    }

    public void DropPrefabArea(SettingData data) {
        Event evt = Event.current;
        GUI.color = Color.green;
        Rect drop_area = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUI.Box(drop_area, "Drag Proto files Here one by one or all together");
        GUI.color = Color.white;
        switch (evt.type) {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!drop_area.Contains(evt.mousePosition))
                    return;

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform) {
                    DragAndDrop.AcceptDrag();
                    m_settingData.ProtoFiles = new System.Collections.Generic.List<string>();
                    foreach (string dragged_object in DragAndDrop.paths) {
                        int offset = dragged_object.IndexOf("Resources/");
                        string processedString = (dragged_object.Split('.')[0]).Remove(0, offset + "Resources/".Length);
                        //  data.SpritePath = processedString;
                        var list = dragged_object.Split('/');
                        //Debug.Log(list[list.Length - 1]);
                        m_settingData.ProtoFiles.Add(list[list.Length - 1]);
                    }
                    CustomSetDirty();
                }
                break;
        }
    }
}