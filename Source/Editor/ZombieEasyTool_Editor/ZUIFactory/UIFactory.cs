using UnityEditor;
using UnityEngine;

namespace ZombieEasyTool
{
    public static class UIFactory
    {
        private const int UI_MENU_PRIORITY = -50;
        private static readonly string PREFAB_MANAGER_PATH = "Assets/ZombieEasyToolResources/ZUIPrefabManager.asset";

        [MenuItem("Zombie/ZombieUI/TextDisplayer",priority = UI_MENU_PRIORITY)]
        public static void CreateTextDisplayer()
        {
            InstantiateUIPrefab(prefabManager => prefabManager.Displayer.TextDisplayer1);
        }

        [MenuItem("Zombie/ZombieUI/Canvas",priority = UI_MENU_PRIORITY)]
        public static void CreateCanvas()
        {
            InstantiateUIPrefab(prefabManager => prefabManager.Canvas);
        }

        [MenuItem("Zombie/ZombieUI/TextDisplayer",true)]
        private static bool SelectionHasCanvasValidate() =>
            Selection.activeGameObject != null && Selection.activeGameObject.GetComponentInParent<Canvas>() != null;

        private static UIPrefabManagerSO LoadUIPrefabManagerSO() => AssetDatabase.LoadAssetAtPath<UIPrefabManagerSO>(PREFAB_MANAGER_PATH);

        private static void InstantiateUIPrefab(System.Func<UIPrefabManagerSO,GameObject> itemSelector)
        {
            var UIPrefabManager = LoadUIPrefabManagerSO();

            if(UIPrefabManager == null)
            {
                Debug.LogError($"Cannot find prefab manager in {PREFAB_MANAGER_PATH}");
                return;
            }

            var Item = itemSelector(UIPrefabManager);

            if(Item == null)
            {
                Debug.LogError($"UI you want to create in prefab manager is null");
                return;
            }

            var UIInstance = PrefabUtility.InstantiatePrefab(Item, Selection.activeTransform);

            Undo.RegisterCreatedObjectUndo(UIInstance,$"Create {UIInstance.name}");
            Selection.activeObject = UIInstance;
        }
    }
}