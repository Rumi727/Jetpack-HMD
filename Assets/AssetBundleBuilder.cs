#if UNITY_EDITOR
using UnityEditor;

namespace Rumi.JetpackHMD
{
    public class AssetBundleBuilder
    {
        [MenuItem("Utility/Build AssetBundles")]
        public static void BuildAllAssetBundles()
        {
            BuildPipeline.BuildAssetBundles("Assets/AssetBundles", BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
            AutoDuplicator.Copy(true);
        }
    }
}
#endif