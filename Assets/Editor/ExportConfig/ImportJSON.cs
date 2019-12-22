using System.IO;
using UnityEditor;
using UnityEngine;

public class ImportJSON
{
    [MenuItem("Import/JSONS %&j", false, 100)]
    public static void MenuItem_ImportJSON()
    {
        // string oldDirectory = Directory.GetCurrentDirectory();
        // Directory.SetCurrentDirectory("xlsx2json");
        //
        // string command = "./export.sh";
        // EditorUtils.ExecuteCommandSync(command);
        //
        // Directory.SetCurrentDirectory(oldDirectory);
    }
}
