using UnityEditor;
using ZombieEasyTool;

namespace ZombieEasyTool
{
    namespace ZEditor
    {

        [CustomEditor(typeof(UIPrefabManagerSO))]
        public class UIPrefabManagerSOEditorHelper : Editor
        {
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                EditorGUILayout.HelpBox($"Update the file path in {typeof(UIFactory).GetType().Name} when its move somewhere else",MessageType.Info);
            }
        }
    }

}