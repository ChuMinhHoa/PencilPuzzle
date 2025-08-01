using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

[InitializeOnLoad]
public class MouseObjectEditorSpawner : EditorWindow
{
    [Title("Pencil Spawner")]
    [SerializeField] GameObject pencilPrefab;
    [SerializeField] GameObject hiddenPrefab;
    [SerializeField] GameObject underPrefab;
    
    [FormerlySerializedAs("Sharpener")]
    [Title("Sharpener Spawner")]
    [SerializeField] GameObject sharpenerPrefab;

    static GameObject spawnedObject;
    static bool isDragging = false;
    static int currentPrefabIndex = 0; // 0: pencil, 1: hidden, 2: under

    static MouseObjectEditorSpawner windowInstance;

    static MouseObjectEditorSpawner()
    {
        SceneView.duringSceneGui += OnSceneGUI;
    }

    [MenuItem("Tools/Prefab Spawner")]
    public static void ShowWindow()
    {
        windowInstance = GetWindow<MouseObjectEditorSpawner>("Prefab Spawner");
    }

    void OnEnable()
    {
        windowInstance = this;
    }

    void OnGUI()
    {
        pencilPrefab = (GameObject)EditorGUILayout.ObjectField("Pencil Prefab", pencilPrefab, typeof(GameObject), false);
        hiddenPrefab = (GameObject)EditorGUILayout.ObjectField("Hidden Prefab", hiddenPrefab, typeof(GameObject), false);
        underPrefab = (GameObject)EditorGUILayout.ObjectField("Under Prefab", underPrefab, typeof(GameObject), false);
        sharpenerPrefab = (GameObject)EditorGUILayout.ObjectField("Sharpener Prefab", sharpenerPrefab, typeof(GameObject), false);
        EditorGUILayout.HelpBox("Press 'O' to spawn Pencil. Right-click to cycle: Hidden, Under.", MessageType.Info);
    }

    static void OnSceneGUI(SceneView sceneView)
    {
        if (windowInstance == null) return;
        Event e = Event.current;

        // Use windowInstance.pencilPrefab, etc.
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.O)
        {
            if (windowInstance.pencilPrefab != null)
            {
                Vector3 worldPos = GetMouseWorldPosition(sceneView);
                spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(windowInstance.pencilPrefab);
                Undo.RegisterCreatedObjectUndo(spawnedObject, "Spawn Pencil");
                spawnedObject.transform.position = worldPos;
                isDragging = true;
                currentPrefabIndex = 0;
            }
            e.Use();
        }

        if (spawnedObject != null && e.type == EventType.MouseDown && e.button == 1)
        {
            GameObject[] prefabs = { windowInstance.pencilPrefab, windowInstance.hiddenPrefab, windowInstance.underPrefab, windowInstance.sharpenerPrefab };
            currentPrefabIndex = (currentPrefabIndex + 1) % prefabs.Length;
            GameObject nextPrefab = prefabs[currentPrefabIndex];
            if (nextPrefab != null)
            {
                Vector3 pos = spawnedObject.transform.position;
                Undo.DestroyObjectImmediate(spawnedObject);
                spawnedObject = (GameObject)PrefabUtility.InstantiatePrefab(nextPrefab);
                Undo.RegisterCreatedObjectUndo(spawnedObject, "Switch Prefab");
                spawnedObject.transform.position = pos;
                isDragging = true;
            }
            e.Use();
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Escape)
        {
            if (spawnedObject != null)
            {
                Undo.DestroyObjectImmediate(spawnedObject);
                spawnedObject = null;
                isDragging = false;
            }
            e.Use();
        }

        if (spawnedObject != null && isDragging)
        {
            if (e.type == EventType.MouseMove || e.type == EventType.MouseDrag)
            {
                Vector3 worldPos = GetMouseWorldPosition(sceneView);
                Undo.RecordObject(spawnedObject.transform, "Move Object");
                spawnedObject.transform.position = worldPos;
                sceneView.Repaint();
            }
            if (e.type == EventType.MouseUp && e.button == 0)
            {
                isDragging = false;
                spawnedObject = null;
                e.Use();
            }
        }
        if (spawnedObject != null && e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
        {
            Undo.RecordObject(spawnedObject.transform, "Rotate Object");
            spawnedObject.transform.Rotate(0, 90, 0, Space.World);
            e.Use();
        }

        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Z)
        {
            spawnedObject = Selection.activeGameObject;
            isDragging = true;
        }
    }

    static Vector3 GetMouseWorldPosition(SceneView sceneView)
    {
        Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
        Plane plane = new Plane(Vector3.up, Vector3.zero);
        plane.Raycast(ray, out float enter);
        Vector3 pos = ray.GetPoint(enter);

        pos.x = Mathf.Round(pos.x * 2f) / 2f;
        pos.y = Mathf.Round(pos.y * 2f) / 2f;
        pos.z = Mathf.Round(pos.z * 2f) / 2f;

        return pos;
    }
}