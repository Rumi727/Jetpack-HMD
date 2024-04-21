#if UNITY_EDITOR
#nullable enable
using System.IO;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Rumi.JetpackHMD
{
    [InitializeOnLoad]
    public static class AutoDuplicator
    {
        public const string sourceFolderSaveFileName = "AutoDuplicatorSourceFolderSaveFile";
        public const string destFolderSaveFileName = "AutoDuplicatorDestFolderSaveFile";

        public const string sourceFileSaveFileName = "AutoDuplicatorSourceFileSaveFile";
        public const string destFileSaveFileName = "AutoDuplicatorDestFileSaveFile";

        public static string sourceFolder = "";
        public static string destFolder = "";

        public static string sourceFile = "";
        public static string destFile = "";

        static AutoDuplicator()
        {
            if (!File.Exists(sourceFolderSaveFileName) || !File.Exists(destFolderSaveFileName) || !File.Exists(sourceFileSaveFileName) || !File.Exists(destFileSaveFileName))
                ChangePath();
            else
            {
                sourceFolder = File.ReadAllText(sourceFolderSaveFileName);
                destFolder = File.ReadAllText(destFolderSaveFileName);

                sourceFile = File.ReadAllText(sourceFileSaveFileName);
                destFile = File.ReadAllText(destFileSaveFileName);
            }
            
            EditorApplication.focusChanged += (focus) => Copy(false);
        }

        [MenuItem("Utility/Change Auto Copy Path")]
        public static void ChangePath()
        {
            {
                string path = EditorUtility.OpenFolderPanel("Auto Duplicator (Source Folder)", "", "");
                sourceFolder = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);

                if (!string.IsNullOrEmpty(path))
                    File.WriteAllText(sourceFolderSaveFileName, sourceFolder);
            }

            {
                string path = EditorUtility.SaveFolderPanel("Auto Duplicator (Dest Folder)", "", "");
                destFolder = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);

                if (!string.IsNullOrEmpty(path))
                    File.WriteAllText(destFolderSaveFileName, destFolder);
            }

            {
                string path = EditorUtility.OpenFilePanel("Auto Duplicator (Source File)", "", "");
                sourceFile = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);

                if (!string.IsNullOrEmpty(path))
                    File.WriteAllText(sourceFileSaveFileName, sourceFile);
            }

            {
                string path = EditorUtility.SaveFilePanel("Auto Duplicator (Dest File)", "", "", "");
                destFile = Path.GetRelativePath(Directory.GetCurrentDirectory(), path);

                if (!string.IsNullOrEmpty(path))
                    File.WriteAllText(destFileSaveFileName, destFile);
            }
        }

        public static void Copy(bool assetbundleCopy)
        {
            if (Directory.Exists(sourceFolder))
            {
                Directory.Delete(destFolder, true);
                DirectoryCopy(sourceFolder, destFolder);
            }

            if (assetbundleCopy && File.Exists(sourceFile))
                File.Copy(sourceFile, destFile, true);
        }

        public static void DirectoryCopy(string sourceFolder, string destFolder)
        {
            if (!Directory.Exists(destFolder))
                Directory.CreateDirectory(destFolder);

            string[] files = Directory.GetFiles(sourceFolder, "*.cs");
            string[] folders = Directory.GetDirectories(sourceFolder);

            for (int i = 0; i < files.Length; i++)
            {
                string file = files[i];
                string name = Path.GetFileName(file);
                string dest = Path.Combine(destFolder, name);

                File.Copy(file, dest);
            }

            for (int i = 0; i < folders.Length; i++)
            {
                string folder = folders[i];
                string name = Path.GetFileName(folder);
                string dest = Path.Combine(destFolder, name);

                DirectoryCopy(folder, dest);
            }
        }
    }
}
#endif