using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.IO.Compression;

public class PostBuildZip
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // The directory where the build is located
        string buildDirectory = Path.GetDirectoryName(pathToBuiltProject);

        // Fetch the version from Player Settings
        string version = PlayerSettings.bundleVersion;

        // Increment the build number (for example, by using Android's version code)
        int buildNumber = PlayerSettings.Android.bundleVersionCode + 1;
        PlayerSettings.Android.bundleVersionCode = buildNumber;

        // Save the new version number (optionally include this)
        PlayerSettings.bundleVersion = version;

        // Generate the versioned zip file name with version and build number
        string zipFilePath = Path.Combine(buildDirectory, $"Build_v{version}_b{buildNumber}.zip");

        // Ensure the file doesn't already exist
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        // Zip the entire build folder
        ZipFile.CreateFromDirectory(buildDirectory, zipFilePath);

        Debug.Log($"Build v{version} (Build {buildNumber}) successfully zipped: " + zipFilePath);
    }
}
