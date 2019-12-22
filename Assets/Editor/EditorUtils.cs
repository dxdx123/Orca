using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public static class EditorUtils
{
    public static string TrimAssetPath(string fullPath)
    {
        string path = fullPath.Replace("\\", "/");

        int index = path.IndexOf("Assets/", StringComparison.Ordinal);
        Assert.IsTrue(index >= 0);

        return path.Substring(index);
    }

    public static T ParseEnum<T>(string value)
    {
        return (T) Enum.Parse(typeof(T), value, true);
    }
    
    public static void ExecuteCommandSync(string command)
    {
        try
        {
            // create the ProcessStartInfo using "cmd" as the program to be run,
            // and "/c " as the parameters.
            // Incidentally, /c tells cmd that we want it to execute the command that follows,
            // and then exit.
            System.Diagnostics.ProcessStartInfo procStartInfo = new System.Diagnostics.ProcessStartInfo();

#if UNITY_EDITOR_OSX
            procStartInfo.FileName = "/bin/bash";
            procStartInfo.Arguments = "-c " + command;
#else
            procStartInfo.FileName = "cmd.exe";
            procStartInfo.Arguments = "/c " + command;
#endif

            // The following commands are needed to redirect the standard output.
            // This means that it will be redirected to the Process.StandardOutput StreamReader.
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            // Do not create the black window.
            procStartInfo.CreateNoWindow = true;
            // Now we create a process, assign its ProcessStartInfo and start it
            System.Diagnostics.Process proc = new System.Diagnostics.Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            // Get the output into a string
            string result = proc.StandardOutput.ReadToEnd();
            // Display the command output.
            Debug.Log(result);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
