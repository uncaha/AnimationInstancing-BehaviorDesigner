using UnityEditor;
using UnityEngine;
using UnityEditor.Animations;

namespace AniPlayable
{
    public class ExportAnimatiorTool : EditorWindow
    {
        private AnimatorController exportObject;
        private string exportPath = "";
        private string selectPath = "";

        static ExportAnimatiorTool sWindow;
        [MenuItem("Tools/AnimatorTool/AnimatorToAsset")]
        static void CreateWindow()
        {
            if (sWindow == null)
            {
                //Rect rect = new Rect(0, 0, 300, 300);
                //sWindow = GetWindowWithRect<ExportAnimatiorTool>(rect, true, "ExportAnimatiorTool");
                //Selection.activeObject = null;
                sWindow = GetWindow(typeof(ExportAnimatiorTool)) as ExportAnimatiorTool;

            }
            sWindow.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("AnimatorController", EditorStyles.boldLabel);
            exportObject = EditorGUILayout.ObjectField("", exportObject, typeof(AnimatorController), true) as AnimatorController;

            DrawPath();

            if (GUILayout.Button("Export"))
            {
                if (exportObject)
                {
                    if (!string.IsNullOrEmpty(exportPath))
                    {
                        try
                        {
                            PlayableAnimatorUtil.GetInstance().ExportToAsset(exportPath, exportObject);
                        }
                        catch (System.Exception err)
                        {
                            Debug.LogError(err);
                        }
                        
                        //this.Close();
                    }
                }
                else
                {
                    Debug.LogError("必须选择要导出的AnimatorController.");
                }
            }
        }

        void DrawPath()
        {
            GUILayout.Label("ExportPath", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.TextField("", selectPath, EditorStyles.textField);
            if (GUILayout.Button("...", GUILayout.Width(25)))
            {

                string tpath = EditorUtility.OpenFolderPanel("Path select", selectPath, "");

                if (!string.IsNullOrEmpty(tpath))
                {
                    if (!tpath.Contains(Application.dataPath))
                    {
                        Debug.LogError("必须选择Assets下的目录。");
                    }
                    else
                    {
                        exportPath = tpath.Replace(Application.dataPath, "Assets");
                        selectPath = tpath;
                    }

                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnInspectorUpdate()
        {
            this.Repaint();
        }

        private void OnFocus()
        {
            
        }

        private void OnLostFocus()
        {
            
        }
    }
}
