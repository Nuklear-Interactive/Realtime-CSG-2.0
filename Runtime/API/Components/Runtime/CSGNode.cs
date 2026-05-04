using System;
using UnityEngine;
using InternalRealtimeCSG;
using UnityEngine.Serialization;

namespace RealtimeCSG.Components
{
#if UNITY_EDITOR
    [Serializable]
    public enum PrefabSourceAlignment : byte
    {
        AlignedFront
      , AlignedBack
      , AlignedLeft
      , AlignedRight
      , AlignedTop
      , AlignedBottom
    }

    public enum PrefabDestinationAlignment : byte
    {
        AlignToSurface
      , AlignSurfaceUp
      , Default
    }

    public enum NodeSortMode
    {
        Default,
        Top,
        Bottom
    }
#endif

    /// <summary>Parent class of <see cref="CSGBrush"/>/<see cref="CSGOperation"/>/<see cref="CSGModel"/></summary>
#if UNITY_EDITOR
    [DisallowMultipleComponent]
#endif
    public abstract class CSGNode : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] public PrefabInstantiateBehaviour prefabBehaviour = PrefabInstantiateBehaviour.Reference;
        [SerializeField] public PrefabSourceAlignment prefabSourceAlignment = PrefabSourceAlignment.AlignedTop;
        [SerializeField] public PrefabDestinationAlignment prefabDestinationAlignment = PrefabDestinationAlignment.AlignToSurface;
        [SerializeField] public NodeSortMode sortMode = NodeSortMode.Default;

        public const int INVALID_NODE_ID = 0;

        public void Sort()
        {
            if (sortMode == NodeSortMode.Top)
            {
                if (transform.GetSiblingIndex() > 0)
                {
                    Transform objectAbove = transform.parent.GetChild(transform.GetSiblingIndex() - 1);

                    CSGNode aboveNode = objectAbove.GetComponent<CSGNode>();

                    if (aboveNode == null)
                    {
                        transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                    }
                    else
                    {
                        if (aboveNode.sortMode != NodeSortMode.Top)
                        {
                            transform.SetSiblingIndex(transform.GetSiblingIndex() - 1);
                        }
                    }
                }
            }
            else if (sortMode == NodeSortMode.Bottom)
            {
                if (transform.GetSiblingIndex() < transform.parent.childCount - 1)
                {
                    Transform objectBelow = transform.parent.GetChild(transform.GetSiblingIndex() + 1);

                    CSGNode belowNode = objectBelow.GetComponent<CSGNode>();

                    if (belowNode == null)
                    {
                        transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                    }
                    else
                    {
                        if (belowNode.sortMode != NodeSortMode.Bottom)
                        {
                            transform.SetSiblingIndex(transform.GetSiblingIndex() + 1);
                        }
                    }
                }
            }

        }
#endif
    }
}
