using RealtimeCSG.Components;
using RealtimeCSG.Foundation;
using UnityEditor;
using UnityEngine;

namespace RealtimeCSG
{
    internal sealed class HierarchyWindowItemGUI
    {
        internal static void OnHierarchyWindowItemOnGUI(int instanceID, Rect selectionRect)
        {
            if(EditorApplication.isPlayingOrWillChangePlaymode)
            {
                return;
            }

            GameObject o = EditorUtility.EntityIdToObject(instanceID) as GameObject;

            if(selectionRect.Contains(Event.current.mousePosition))
            {
                Transform t = o == null ? null : o.transform;
                SceneDragToolManager.OnHandleDragAndDrop(null, t, selectionRect);
            }

            if(o == null)
            {
                return;
            }

            CsgguiStyleUtility.InitStyles();

            CSGNode node = o.GetComponent<CSGNode>();

            if(node == null || !node.enabled || (node.hideFlags & (HideFlags.HideInHierarchy | HideFlags.HideInInspector)) != 0)
            {
                return;
            }

            CSGOperationType operationType;

            CSGBrush brush = node as CSGBrush;

            if(brush != null)
            {
                operationType = brush.operationType;
                CSGSkin skin = CsgguiStyleUtility.Skin;

                GUI.Label(selectionRect, skin.hierarchyOperations[(int)operationType], CsgguiStyleUtility.RightAlignedLabel);

                if (brush.sortMode != NodeSortMode.Default)
                {
                    Rect sortRect = selectionRect;
                    sortRect.x -= skin.hierarchyOperations[(int)operationType].image.width;

                    GUIContent sortIcon = EditorGUIUtility.IconContent("d_ProfilerTimelineRollUpArrow");

                    if (brush.sortMode == NodeSortMode.Bottom)
                    {
                        sortIcon = EditorGUIUtility.IconContent("d_ProfilerTimelineDigDownArrow");
                    }

                    GUI.Label(sortRect, sortIcon, CsgguiStyleUtility.RightAlignedLabel);
                }

                return;
            }

            CSGOperation operation = node as CSGOperation;

            if(operation != null)
            {
                CSGSkin skin = CsgguiStyleUtility.Skin;
                float iconWidth = 0;

                if(!operation.PassThrough)
                {
                    operationType = operation.operationType;
                    int operationTypeIndex = (int)operationType;

                    if(operationTypeIndex >= 0 && operationTypeIndex < skin.hierarchyOperations.Length)
                    {
                        GUI.Label(selectionRect, skin.hierarchyOperations[operationTypeIndex], CsgguiStyleUtility.RightAlignedLabel);

                        iconWidth = skin.hierarchyOperations[operationTypeIndex].image.width;
                    }
                }
                else
                {
                    GUI.Label(selectionRect, skin.hierarchyPassThrough, CsgguiStyleUtility.RightAlignedLabel);

                    iconWidth = skin.hierarchyPassThrough.image.width;
                }

                if (operation.sortMode != NodeSortMode.Default)
                {
                    Rect sortRect = selectionRect;
                    sortRect.x -= iconWidth;

                    GUIContent sortIcon = EditorGUIUtility.IconContent("d_ProfilerTimelineRollUpArrow");

                    if (operation.sortMode == NodeSortMode.Bottom)
                    {
                        sortIcon = EditorGUIUtility.IconContent("d_ProfilerTimelineDigDownArrow");
                    }

                    GUI.Label(sortRect, sortIcon, CsgguiStyleUtility.RightAlignedLabel);
                }
            }
        }
    }
}
