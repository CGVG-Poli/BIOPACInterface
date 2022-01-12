using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;
using UnityEditor;
#if UNITY_EDITOR
using UnityEditor.Callbacks;
#endif

namespace Utils
{
    public static class Configuration
    {
        private static IDictionary<string, string> _config;

        private static FileInfo _fileInfo =
#if UNITY_EDITOR
            new FileInfo(Path.Combine(Path.Combine(Application.dataPath, "BIOPACInterface/BuildData"), "config.txt"));
#elif !UNITY_EDITOR
            new FileInfo(Path.Combine(Path.Combine(Directory.GetParent(Application.dataPath).FullName, "BuildData"), "config.txt"));
#endif
        static Configuration()
        {
            try
            {
                _config = new Dictionary<string, string>();
                Load(_fileInfo);
            }
            catch (Exception e)
            {
#if UNITY_WSA
            Debug.Log(e);
#else
                Debug.LogException(e);
#endif
            }
        }

        public static T GetEnum<T>(string path, T deflt = default(T))
        {
            if (_config.ContainsKey(path))
                return (T)Enum.Parse(typeof(T), _config[path], true);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static string GetString(string path)
        {
            return GetString(path, string.Empty);
        }

        public static string GetString(string path, string deflt)
        {
            if (_config.ContainsKey(path))
                return _config[path];
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static int GetInt(string path, int deflt = 0)
        {
            if (_config.ContainsKey(path))
                return int.Parse(_config[path], System.Globalization.NumberStyles.Integer,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static float GetFloat(string path, float deflt = 0f)
        {
            if (_config.ContainsKey(path))
                return float.Parse(_config[path], System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture.NumberFormat);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static bool GetBool(string path, bool deflt = false)
        {
            if (_config.ContainsKey(path))
                return bool.Parse(_config[path]);
            else
            {
                Debug.LogWarningFormat("Using default value for property \"{0}\" ({1})", path, deflt);
                return deflt;
            }
        }

        public static void Put(string path, object value)
        {
            _config[path] = value.ToString();
        }

        public static bool Exists(string path)
        {
            return _config.ContainsKey(path);
        }

        public static IEnumerable<string> Keys
        {
            get { return _config.Keys; }
        }

        public static void Load(FileInfo file)
        {
            using (var f = new StreamReader(file.OpenRead()))
            {
                string line;
                while ((line = f.ReadLine()) != null)
                {
                    if (line.Length > 0 && !line.StartsWith("#"))
                    {
                        var split = line.Split('=');
                        _config[split[0].Trim()] = split[1].Trim();
                    }
                }
            }
        }

#if UNITY_EDITOR
        [PostProcessBuild(1)]
        public static void OnPostprocessBuild(BuildTarget target, string pathToBuiltProject)
        {
            string folderToCopy = Path.GetFileName(_fileInfo.DirectoryName);
            string destFolder = Path.Combine(Path.GetDirectoryName(pathToBuiltProject), folderToCopy);
            Debug.Log($"Copying the editor config file @{_fileInfo.DirectoryName} into the build data @{destFolder}");
            DirectoryCopy(_fileInfo.DirectoryName, destFolder, true, new []{".meta"});
        }
#endif
        public static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs, params string[] excludingExtensions)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();

            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles().Where(file => !excludingExtensions.Any(x => file.Name.EndsWith(x, StringComparison.OrdinalIgnoreCase))).ToArray();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                if (File.Exists(tempPath))
                    File.Delete(tempPath);

                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }
    }
}