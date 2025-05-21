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
            EditorUtility.DisplayDialog("����", "�ϳ� �̻��� GameObject�� �����ϼ���.", "Ȯ��");
            return;
        }

        foreach (GameObject parent in selectedObjects)
        {
            if (parent.transform.childCount != 1)
            {
                Debug.LogWarning($"{parent.name}�� �ڽ��� 1���� �ƴմϴ�. ���õ˴ϴ�.");
                continue;
            }

            GameObject child = parent.transform.GetChild(0).gameObject;

            Component[] childComponents = child.GetComponents<Component>();

            foreach (Component childComp in childComponents)
            {
                if (childComp is Transform) continue;

                Type compType = childComp.GetType();

                // �θ� ������ ������Ʈ�� �̹� ������ ����
                if (parent.GetComponent(compType) != null)
                {
                    Debug.Log($"{parent.name}�� {compType.Name} ������Ʈ�� �̹� �ֽ��ϴ�. ������.");
                    continue;
                }

                // ������Ʈ ����
                Undo.RecordObject(parent, "Copy Component From Child");

                Component copied = parent.AddComponent(compType);
                EditorUtility.CopySerialized(childComp, copied);
                

                Debug.Log($"{compType.Name} ������Ʈ�� {child.name}���� {parent.name}���� �����߽��ϴ�.");
            }
            Component comp = parent.GetComponent(typeof(LODGroup));
            if (comp != null)
            {
                Undo.DestroyObjectImmediate(comp); // �ǵ�����(Undo) ����
            }
            Undo.DestroyObjectImmediate(child);
        }
    }
}
