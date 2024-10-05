using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
using System.IO.Compression;
using System;

public class PostBuildZip
{
    [PostProcessBuild]
    public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
    {
        // The directory where the built project resides
        string buildDirectory = Path.GetDirectoryName(pathToBuiltProject);

        // Fetch the version from Player Settings
        string version = PlayerSettings.bundleVersion;

        // Get the current timestamp and format it for filenames
        string buildNumber = DateTime.Now.ToString("yyyyMMdd");

        // Generate the versioned zip file name with version and build number
        string zipFilePath = Path.Combine(buildDirectory, $"Build_v{version}_build{buildNumber}.zip");

        // Ensure the zip file doesn't already exist
        if (File.Exists(zipFilePath))
        {
            File.Delete(zipFilePath);
        }

        // Zip all files and directories except for zip files
        ZipDirectoryExcludingZips(buildDirectory, zipFilePath);

        Debug.Log($"Build v{version} (Build {buildNumber}) successfully zipped: " + zipFilePath);
    }

    // Method to zip the directory while excluding .zip files
    private static void ZipDirectoryExcludingZips(string sourceDir, string zipFilePath)
    {
        using (FileStream zipToOpen = new FileStream(zipFilePath, FileMode.Create))
        {
            using (ZipArchive archive = new ZipArchive(zipToOpen, ZipArchiveMode.Create))
            {
                // Recursively add files and directories to the zip archive
                AddFilesToZip(archive, sourceDir, "");
            }
        }
    }

    // Helper method to add files and directories to the zip archive
    private static void AddFilesToZip(ZipArchive archive, string sourceDir, string baseDir)
    {
        foreach (string file in Directory.GetFiles(sourceDir))
        {
            // Exclude any .zip files
            if (!file.EndsWith(".zip"))
            {
                // Get the relative path for adding into the zip
                string entryName = Path.Combine(baseDir, Path.GetFileName(file));
                archive.CreateEntryFromFile(file, entryName);
            }
        }

        foreach (string directory in Directory.GetDirectories(sourceDir))
        {
            // Recursively add subdirectories
            string subDir = Path.Combine(baseDir, Path.GetFileName(directory));
            AddFilesToZip(archive, directory, subDir);
        }
    }
}
