// * * * * * * * * * * * * * * * * * * * * * *
//  Description:
//
// * * * * * * * * * * * * * * * * * * * * * *

using System;
using UnityEditor;

namespace RealtimeCSG
{
    public class AssetHelper
    {
        public class AssetEditingContext : IDisposable
        {
            public AssetEditingContext()
            {
                AssetDatabase.StartAssetEditing();
            }

            public void Dispose()
            {
                AssetDatabase.StopAssetEditing();
            }
        }
    }
}
