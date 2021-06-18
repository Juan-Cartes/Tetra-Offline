using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;

public class Builder : MonoBehaviour
{
    [MenuItem("Tools/Build Tetra Online for all platforms")]
    public static void BuildAll()
    {
        string folder = EditorUtility.SaveFolderPanel("Please select your SteamCMD ContentBuilder content folder", "", "");
       
        if(BuildForPlatform(BuildTarget.StandaloneLinux64, folder))
        {
            if(BuildForPlatform(BuildTarget.StandaloneOSX, folder))
            {
                if(BuildForPlatform(BuildTarget.StandaloneWindows64, folder))
                {
                    EditorApplication.Beep();
                    Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!!\nTetra Online has been built. Check all folders and logs to verify there was no errors, as this message could not be 100% accurate.\nIf all seems correct, you may run SteamCMD to upload this new build\n!!!!!!!!!!!!!!!!!!!!!!!!");
                 
                }   
            }
        }
    
    }

    public static bool BuildForPlatform(BuildTarget target, string folder)
    {
        string playerPath = string.Empty;

        switch (target)
        {
            case BuildTarget.StandaloneWindows64:
                playerPath = folder + "/Windows/Tetra Online.exe";
                break;
            case BuildTarget.StandaloneLinux64:
                playerPath = folder + "/Linux/Tetra Online.x86_64";
                break;
            case BuildTarget.StandaloneOSX:
                playerPath = folder + "/Macos/Tetra Online.app";
                break;
        }
        
        Debug.Log("Building " + target + " at " + playerPath);


        BuildReport report = BuildPipeline.BuildPlayer(GetScenePaths(), playerPath, target, target == BuildTarget.StandaloneWindows64 ? BuildOptions.ShowBuiltPlayer : BuildOptions.None);
    
        if(report.summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Target " + target + " was built succesfully.");
            return true;
        }
        else
        {
            Debug.LogError("An error occured building " + target + "! See logs!");
            return false;
        }
    
    }
    static string[] GetScenePaths()
    {
        string[] scenes = new string[EditorBuildSettings.scenes.Length];
        for (int i = 0; i < scenes.Length; i++)
        {
            scenes[i] = EditorBuildSettings.scenes[i].path;
        }
        return scenes;
    }


}
