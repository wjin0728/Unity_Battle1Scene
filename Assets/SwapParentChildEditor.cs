using UnityEngine;
using UnityEditor;
using System;
using System.Reflection;
using UnityEditor.SceneManagement;

public class CopyChildComponentsToParent : EditorWindow
{
    [MenuItem("Tools/Copy Child Components to Parent")]
    public static void CopyComponents()
    {
        GameObject[] selectedObjects = Selection.gameObjects;

        if (selectedObjects.Length == 0)
        {
            EditorUtility.DisplayDialog("오류", "하나 이상의 GameObject를 선택하세요.", "확인");
            return;
        }

        foreach (GameObject parent in selectedObjects)
        {
            if (parent.transform.childCount != 1)
            {
                Debug.LogWarning($"{parent.name}은 자식이 1명이 아닙니다. 무시됩니다.");
                continue;
            }

            GameObject child = parent.transform.GetChild(0).gameObject;

            Component[] childComponents = child.GetComponents<Component>();

            foreach (Component childComp in childComponents)
            {
                if (childComp is Transform) continue;

                Type compType = childComp.GetType();

                // 부모에 동일한 컴포넌트가 이미 있으면 생략
                if (parent.GetComponent(compType) != null)
                {
                    Debug.Log($"{parent.name}에 {compType.Name} 컴포넌트가 이미 있습니다. 생략됨.");
                    continue;
                }

                // 컴포넌트 복사
                Undo.RecordObject(parent, "Copy Component From Child");

                Component copied = parent.AddComponent(compType);
                EditorUtility.CopySerialized(childComp, copied);
                

                Debug.Log($"{compType.Name} 컴포넌트를 {child.name}에서 {parent.name}으로 복사했습니다.");
            }
            Component comp = parent.GetComponent(typeof(LODGroup));
            if (comp != null)
            {
                Undo.DestroyObjectImmediate(comp); // 되돌리기(Undo) 가능
            }
            Undo.DestroyObjectImmediate(child);
        }
    }
}
