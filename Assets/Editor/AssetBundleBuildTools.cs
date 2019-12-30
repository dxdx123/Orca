using System;
using System.IO;
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LitJson;
using System.Text.RegularExpressions;
using UnityEditor.UIElements;
using UnityEngine.Assertions;

using Object = UnityEngine.Object;

public class AssetBundleBuildTools
{
    public const string ROUTE_DATA_PATH = "Assets/Res/Core/AssetBundleRouteData.asset";
    public const string VARIANT_DATA_PATH = "Assets/Res/Core/AssetBundleVariantData.asset";
    
    [MenuItem("Build/AssetBundles")]
    public static void MenuItem_BuildAssetBundles()
    {
        // 生成route、Variant信息
        GenerateRouteConfig();
        GenerateVariantConfig();
        
        var buildOptions = BuildAssetBundleOptions.ChunkBasedCompression
                           | BuildAssetBundleOptions.DisableWriteTypeTree
                           | BuildAssetBundleOptions.StrictMode
                           | BuildAssetBundleOptions.DeterministicAssetBundle
                           | BuildAssetBundleOptions.DisableLoadAssetByFileName
                           | BuildAssetBundleOptions.DisableLoadAssetByFileNameWithExtension;

        var buildTarget = EditorUserBuildSettings.activeBuildTarget;

        BuildAsetBundles(buildOptions, buildTarget);
    }

    private static void GenerateVariantConfig()
    {
        var list = GetVariantList();

        var variantRefList = new List<AssetBundleVariantRef>();
        
        foreach (var tuple in list)
        {
            string abPath = tuple.Item1.ToLower();
            
            int length = tuple.Item2.Count;
            List<string> variantList = new List<string>(length);
            
            for(int i=0; i<length; ++i)
            {
                variantList.Add(tuple.Item2[i].ToLower());
            }
            
            AssetBundleVariantRef variantRef = new AssetBundleVariantRef()
            {
                abPath = abPath,
                variants = variantList,
            };
            
            variantRefList.Add(variantRef);
        }
        
        var asset = ScriptableObject.CreateInstance<AssetBundleVariantData>();
        asset.list = variantRefList;
        AssetDatabase.CreateAsset(asset, VARIANT_DATA_PATH);
        
        AssetDatabase.Refresh(); 
    }

    private static void GenerateRouteConfig()
    {
        var dict = GetAssetBundleDict();

        var list = new List<AssetBundleRef>();

        foreach (var item in dict)
        {
            string path = item.Key;
            if (path != ROUTE_DATA_PATH && path != VARIANT_DATA_PATH)
            {
                string assetBundlePath = item.Value;
                
                list.Add(new AssetBundleRef()
                {
                    path = path.ToLower(),
                    assetBundlePath = assetBundlePath.ToLower(),
                });
            }
            else
            {
                // skip
            }
        }

        var asset = ScriptableObject.CreateInstance<AssetBundleRouteData>();
        asset.list = list;
        AssetDatabase.CreateAsset(asset, ROUTE_DATA_PATH);
        
        AssetDatabase.Refresh();
    }

    public static void BuildAsetBundles(BuildAssetBundleOptions buildOptions, BuildTarget buildTarget)
    {
        new AssetBundleNormalBuild().Build(buildOptions, buildTarget);
    }

    public static Dictionary<string, string> GetAssetBundleDict()
    {
        return new AssetBundleNormalBuild().GetAssetBundleDict();
    }

    public static List<Tuple<string, List <string>>> GetVariantList()
    {
        return new AssetBundleNormalBuild().GetVariantList();
    }

    enum FileBuildType
    {
        Preserve,   // 直接打包
        Dependency, // 依赖打包(有引用才打包)
        Internal,   // 中间资源
    }

    // 文件从哪来
    class AssetBundleDirectory
    {
        // 用于处理目录
        public string directory;
        public string pattern;
        public string _searchOption;

        public SearchOption searchOption
        {
            get
            {
                return EditorUtils.ParseEnum<SearchOption>(_searchOption);
            }
        }

        // 用于追踪到文件
        public string filePattern;
        public string _fileBuildType;

        public FileBuildType fileBuildType
        {
            get
            {
                return EditorUtils.ParseEnum<FileBuildType>(_fileBuildType);
            }
        }

        // 用于确定目的地
        public string sourcePattern;
        public string destPattern;
        
        // 用于确定Variant
        public string variantSourcePattern;
        public string variantDestPattern;
    }

    // 文件去哪里
    class AssetBundleFile
    {
        public string source;

        public FileBuildType buildType;

        public string destination;

        public string variantName;

        public override string ToString()
        {
            return $"source: {source}, destination: {destination}, buildType: {buildType}, variantName: {variantName}";
        }
    }

    // 中间资源状态
    class InternalAsset
    {
        public string source;

        public int refCount;

        public override string ToString()
        {
            return $"source: {source}, refCount: {refCount}";
        }
    }

    internal class AssetBundleNormalBuild
    {
        public const string SEARCH_CONFIG_DIRECTORY = "Assets/JSONS/Editor-AssetBundle_Config.json";

        public void Build(BuildAssetBundleOptions buildOptions, BuildTarget buildTarget)
        {
            var newMergedDict = GetAssetBundlePackageDict();

            // 打包
            BuildAssetBundles(newMergedDict, buildOptions, buildTarget);

            Debug.Log("<color=green>Build AssetBundle Finished</color>");
        }

        public List<Tuple<string, List <string>>> GetVariantList()
        {
            var mergedDict = GetAssetBundlePackageDict();

            var dict = new Dictionary<string, List<string>>();

            foreach (var item in mergedDict)
            {
                if (!string.IsNullOrEmpty(item.Value.variantName))
                {
                    string destAB = item.Value.destination;
                    string variantName = item.Value.variantName;

                    List<string> list;
                    if (dict.TryGetValue(destAB, out list))
                    {
                        if (!list.Contains(variantName))
                        {
                            list.Add(variantName);
                        }
                    }
                    else
                    {
                        list = new List<string>(1);
                        list.Add(variantName);
                        
                        dict.Add(destAB, list);
                    }
                }
                else
                {
                    // skip
                }
            }

            List<Tuple<string, List <string>>> result = new List<Tuple<string, List<string>>>();

            foreach (var item in dict)
            {
                string abPath = item.Key;
                List<string> varList = item.Value;
                
                result.Add(Tuple.Create(abPath, varList));
            }

            return result;
        }

        public Dictionary<string, string> GetAssetBundleDict()
        {
            var mergedDict = GetAssetBundlePackageDict();

            var dict = new Dictionary<string, string>(mergedDict.Count);

            foreach (var item in mergedDict)
            {
                string srcPath = item.Key;

                if (!string.IsNullOrEmpty(item.Value.variantName))
                {
                    string destAB = item.Value.destination + "." + item.Value.variantName;
                    dict.Add(srcPath, destAB);
                }
                else
                {
                    string destAB = item.Value.destination; 
                    dict.Add(srcPath, destAB);
                }
            }

            return dict;
        }
        
        private Dictionary<string, AssetBundleFile> GetAssetBundlePackageDict()
        {
            List<AssetBundleFile> filesList = ProcessSearchFiles();

            // 检查重复
            bool duplicate = CheckDuplication(filesList);
            if (duplicate)
            {
                throw new Exception("!!! 打包配置有冲突，请检查");
            }

            // dependency的需要根据Preserve的依赖对应剔除
            List<AssetBundleFile> abList = TrimUnUsedFileDependency(filesList);
//            foreach(var file in abList)
//            {
//                Debug.LogFormat("!!! true pack path: <color=red>{0}</color>, dest: <color=red>{1}</color>, buildType: <color=red>{2}</color>", file.source, file.destination, file.buildType);
//            }

            // 根据Scene\Preserve的返回中间状态的资源
            List<AssetBundleFile> internalList = GetInternalAssetList(filesList);
//            foreach(var file in internalList)
//            {
//                Debug.LogFormat("!!! internal path: <color=red>{0}</color>, dest: <color=red>{1}</color>, buildType: <color=red>{2}</color>", file.source, file.destination, file.buildType);
//            }


            // 合并
            Dictionary<string, AssetBundleFile> mergedDict = MergeBuildAssetBundles(abList, internalList);
//            foreach (var item in mergedDict)
//            {
//                var file = item.Value;
//                Debug.LogFormat("!!! Merged path: <color=red>{0}</color>, dest: <color=red>{1}</color>, buildType: <color=red>{2}</color>", file.source, file.destination, file.buildType);
//            }

            return mergedDict;
        }

        private void BuildAssetBundles(Dictionary<string, AssetBundleFile> mergedDict, BuildAssetBundleOptions buildOptions, BuildTarget buildTarget)
        {
            string destFolder = string.Format("{0}/{1}", Application.streamingAssetsPath, "AssetBundles");
            if(Directory.Exists(destFolder))
            {
                Directory.Delete(destFolder, true);
            }

            Directory.CreateDirectory(destFolder);
            AssetDatabase.Refresh();

            int length = mergedDict.Count;
            AssetBundleBuild[] buildMap = new AssetBundleBuild[length];

            int index = 0;

            foreach (var item in mergedDict)
            {
                string sourcePath = item.Key;
                string[] resourcesAssets = new string[1] { sourcePath }; // 这里是每个资源指定到AB位置，也许可以反过来一对多指定，区别应该不大
                buildMap[index].assetNames = resourcesAssets;

                string bundleName = item.Value.destination;
                buildMap[index].assetBundleName = bundleName;

                buildMap[index].assetBundleVariant = item.Value.variantName;

                ++index;
            }

            BuildPipeline.BuildAssetBundles(destFolder, buildMap, buildOptions, buildTarget);

            AssetDatabase.Refresh();
        }

        private Dictionary<string, AssetBundleFile> MergeBuildAssetBundlesDict(Dictionary<string, AssetBundleFile> mergedDict, List<AssetBundleFile> sceneDepList)
        {
            var result = new Dictionary<string, AssetBundleFile>(mergedDict);

            for (int i = 0, length = sceneDepList.Count; i < length; ++i)
            {
                var bundleFile = sceneDepList[i];
                string path = bundleFile.source;
                if (!result.ContainsKey(path))
                {
                    result.Add(path, bundleFile);
                }
            }
            
            return result;
        }

        private List<AssetBundleFile> GetAssetBundleFileScenePrefab(string path, HashSet<string> packSet)
        {
            var list = new List<AssetBundleFile>();

            string prefix = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string newDest = Path.Combine(prefix, $"scene_dep_{fileName}");
            
            string[] deps = AssetDatabase.GetDependencies(new string[] {path});

            foreach (var dep in deps)
            {
                if (!packSet.Contains(dep) && dep.StartsWith("Assets/") && IsSupportSerializableType(dep))
                {
                    var bundleFile = new AssetBundleFile()
                    {
                        buildType = FileBuildType.Internal,
                        source = dep,
                        destination = newDest,
                        
                        variantName = null,
                    };
                    
                    list.Add(bundleFile);
                }
            }

            return list;
        }

        private Dictionary<string, AssetBundleFile> MergeBuildAssetBundles(List<AssetBundleFile> abList, List<AssetBundleFile> internalList)
        {
            var dict = new Dictionary<string, AssetBundleFile>();

            foreach(var abFile in abList)
            {
                string path = abFile.source;

                if(!dict.ContainsKey(path))
                {
                    dict.Add(path, abFile);
                }
                else
                {
                    // skip
                }
            }

            foreach(var abFile in internalList)
            {
                string path = abFile.source;

                if(!dict.ContainsKey(path))
                {
                    dict.Add(path, abFile);
                }
                else
                {
                    // skip
                }
            }

            return dict;
        }

        private List<AssetBundleFile> GetInternalAssetList(List<AssetBundleFile> filesList)
        {
            Dictionary<string, InternalAsset> dict = new Dictionary<string, InternalAsset>();

            for (int i = 0, length = filesList.Count; i < length; ++i)
            {
                var abFile = filesList[i];

                if (abFile.buildType == FileBuildType.Preserve)
                {
                    var deps = AssetDatabase.GetDependencies(new string[] { abFile.source });

                    foreach (var dep in deps)
                    {
                        if (dep != abFile.source && dep.StartsWith("Assets/") && IsSupportSerializableType(dep))
                        {
                            InternalAsset asset;
                            if (dict.TryGetValue(dep, out asset))
                            {
                                asset.refCount = asset.refCount + 1;
                            }
                            else
                            {
                                asset = new InternalAsset()
                                {
                                    source = dep,
                                    refCount = 1,
                                };

                                dict.Add(dep, asset);
                            }
                        }
                        else
                        {
                            // continue
                        }
                    }
                }
                else
                {
                    // skip
                }
            }

            HashSet<string> preserveSet = new HashSet<string>();
            for (int i = 0; i < filesList.Count; ++i)
            {
                preserveSet.Add(filesList[i].source);
            }
            
            List<AssetBundleFile> result = new List<AssetBundleFile>();
            
            foreach (var item in dict)
            {
                InternalAsset internalAsset = item.Value;

                if (internalAsset.refCount > 1)
                {
                    string path = item.Key;
                    
                    var build = new AssetBundleFile()
                    {
                        source = path,
                        buildType = FileBuildType.Internal,
                        destination = path + ".assetbundle",
                    };

                    result.Add(build);
                }
            }

            return result;
        }

        private bool IsSupportSerializableType(string path)
        {
            var go = AssetDatabase.LoadAssetAtPath<Object>(path);

            return (go is AnimationClip
                || go is AudioClip
                || go is Shader
                || go is Font
                || go is Material
                || go is Mesh
                || IsPrefab(go)
                || go is Texture);
        }

        private bool IsPrefab(Object go)
        {
            if (go)
            {
                return PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Regular
                    || PrefabUtility.GetPrefabAssetType(go) == PrefabAssetType.Variant;
            }
            else
            {
                return false;
            }
        }

        private bool IsScene(string path)
        {
            if (!string.IsNullOrEmpty(path))
            {
                return path.EndsWith(".unity");
            }
            else
            {
                return false;
            }
        }

        private List<AssetBundleFile> TrimUnUsedFileDependency(List<AssetBundleFile> filesList)
        {
            var preserveDict = new Dictionary<string, AssetBundleFile>();
            var dependencyDict = new Dictionary<string, AssetBundleFile>();

            for(int i=0, length=filesList.Count; i<length; ++i)
            {
                var file = filesList[i];
                string path = file.source;

                var buildType = file.buildType;
                if (buildType == FileBuildType.Preserve)
                {
                    preserveDict.Add(path, file);
                }
                else if(buildType == FileBuildType.Dependency)
                {
                    dependencyDict.Add(path, file);
                }
                else
                {
                    throw new Exception("!!! WTF type: " + buildType);
                }
            }

            var validDepList = GetValidDepList(preserveDict, dependencyDict);

            // 统一返回
            foreach (var item in preserveDict)
            {
                validDepList.Add(item.Value);
            }

            return validDepList;
        }

        private List<AssetBundleFile> GetValidDepList(Dictionary<string, AssetBundleFile> preserveDict, Dictionary<string, AssetBundleFile> dependencyDict)
        {
            var list = new List<AssetBundleFile>();
            
            HashSet<string> allDepSet = new HashSet<string>();

            foreach (var item in preserveDict)
            {
                string path = item.Key;

                string[] deps = AssetDatabase.GetDependencies(path);

                foreach (var dep in deps)
                {
                    if (!allDepSet.Contains(dep))
                    {
                        allDepSet.Add(dep);
                    }
                }
            }

            foreach (var candidatePath in allDepSet)
            {
                if (dependencyDict.ContainsKey(candidatePath))
                {
                    list.Add(dependencyDict[candidatePath]);
                }
            }

            return list;
        }

        private bool CheckDuplication(List<AssetBundleFile> fileList)
        {
            bool dup = false;

            var sets = new HashSet<string>();

            for(int i=0, length=fileList.Count; i<length; ++i)
            {
                var buildFile = fileList[i];
                string path = buildFile.source;

                if(sets.Contains(path))
                {
                    dup = true;
                    Debug.LogErrorFormat("!!! 资源打包有重复，请检查 source: {0}, buildType: {1}", path, buildFile.buildType);
                }
                else
                {
                    sets.Add(path);
                }
            }

            return dup;
        }

        private List<AssetBundleFile> ProcessSearchFiles()
        {
            List<AssetBundleFile> list = new List<AssetBundleFile>();

            string jsonContent = File.ReadAllText(SEARCH_CONFIG_DIRECTORY);
            var jsons = JsonMapper.ToObject(jsonContent);

            for(int i=0, length=jsons.Count; i<length; ++i)
            {
                var json = jsons[i];

                string directory = (string)json["directory"];
                string pattern = (string)json["pattern"];
                string searchOption = (string)json["searchOption"];

                string filePattern = (string)json["filePattern"];
                string fileBuildType = (string)json["fileBuildType"];

                string sourcePattern = (string)json["sourcePattern"];
                string destPattern = (string)json["destPattern"];

                string variantSourcePattern = json["variantSourcePattern"] != null ? (string)json["variantSourcePattern"] : "";
                string variantDestPattern = json["variantDestPattern"] != null ? (string)json["variantDestPattern"] : "";

                var config = new AssetBundleDirectory()
                {
                    directory = directory,
                    pattern = pattern,
                    _searchOption = searchOption,

                    filePattern = filePattern,
                    _fileBuildType = fileBuildType,

                    sourcePattern = sourcePattern,
                    destPattern = destPattern,
                    
                    variantSourcePattern = variantSourcePattern,
                    variantDestPattern = variantDestPattern,
                };

                List<AssetBundleFile> subList = TryCollectDirectoryFile(config);
//                foreach(var item in subList)
//                {
//                    Debug.LogFormat("source: {0} dest: {1} buildType: {2}", item.source, item.destination, item.buildType);
//                }
//                Debug.LogFormat("!!! Processing <color=red>{0}</color> fileCount: <color=red>{1}</color>", config.directory, subList.Count);

                list.AddRange(subList);
            }

            return list;
        }

        private List<AssetBundleFile> TryCollectDirectoryFile(AssetBundleDirectory config)
        {
            var list = new List<AssetBundleFile>();

            var directoryInfo = new DirectoryInfo(config.directory);

            Regex filePattern = new Regex(config.filePattern);

            foreach (var fileInfo in directoryInfo.GetFiles(config.pattern, config.searchOption))
            {
                string fullPath = fileInfo.FullName;

                string formatName = EditorUtils.TrimAssetPath(fullPath);

                if(filePattern.IsMatch(formatName))
                {
                    string destPath = Regex.Replace(formatName, config.sourcePattern, config.destPattern);

                    string variantName = ParseVariantName(formatName, config.variantSourcePattern,
                        config.variantDestPattern);
                    
                    var file = new AssetBundleFile()
                    {
                        source = formatName,
                        buildType = config.fileBuildType,

                        destination = destPath,
                        
                        variantName = variantName,
                    };

                    list.Add(file);
                }
                else
                {
                    // not match, skip
                }
            }

            return list;
        }

        private string ParseVariantName(string path, string variantSourcePattern, string variantDestPattern)
        {
            if (string.IsNullOrEmpty(variantSourcePattern) || string.IsNullOrEmpty(variantDestPattern))
            {
                return null;
            }
            else
            {
                return Regex.Replace(path, variantSourcePattern, variantDestPattern);
            }
        }
    }
}
