using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using Microsoft.VisualBasic.FileIO;

namespace XRayOperatorTool.Model
{
    public class FileReader
    {
        public static readonly string CSV_EXTENSION = "csv";
        public static readonly string JPG_EXTENSION = "jpg";
        private static readonly string DEFAULT_FOLDER_FILE = "default_folder.txt";

        private readonly CommonOpenFileDialog dialog = new CommonOpenFileDialog();
        private string selectedPath = null;
        private string defaultFolderPath = null;

        public string SelectedPath
        {
            get
            {
                return selectedPath;
            }
            set
            {
                if (value != null && value != "")
                {
                    selectedPath = value;
                    int lastSlash = selectedPath.LastIndexOf('/');
                    selectedPath = (lastSlash > -1) ? selectedPath.Substring(0, lastSlash) : selectedPath;
                    DefaultFolderPath = Directory.GetParent(selectedPath).FullName;
                }
            }
        }

        private string DefaultFolderPath
        {
            get
            {
                if (defaultFolderPath == null)
                {
                    try
                    {
                        var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DEFAULT_FOLDER_FILE);
                        var listOfLines = File.ReadLines(file);
                        defaultFolderPath = listOfLines.ToList()[0];
                    }
                    catch (Exception)
                    {
                        defaultFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
                    }
                }
                
                return defaultFolderPath;
            }
            set
            {
                if (value != null)
                {
                    defaultFolderPath = value;
                    var file = Path.Combine(Path.GetDirectoryName(Assembly.GetEntryAssembly().Location), DEFAULT_FOLDER_FILE);
                    File.WriteAllLines(file, new string[] {defaultFolderPath});
                }
            }
        }
        public FileReader()
        {

        }

        public void SelectFolder()
        {
            dialog.InitialDirectory = DefaultFolderPath;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                SelectedPath = dialog.FileName;
            }

        }


        public IEnumerable<string> ReadFiles(string extension)
        {
            if (SelectedPath == null)
            {
                return new List<string>();
            }
            var files = Directory.GetFiles(SelectedPath, "*." + extension, System.IO.SearchOption.AllDirectories);
            return files;
        }

        public void CopyDirectory(string relativePath)
        {
            var currentPath = SelectedPath;
            var currentDirectory = Path.GetFileName(currentPath);
            var parentPath = Directory.GetParent(currentPath);
            var directoryPathArchive = parentPath.CreateSubdirectory(relativePath).FullName;
            directoryPathArchive = Path.Combine(directoryPathArchive, currentDirectory);
            FileSystem.CopyDirectory(currentPath, directoryPathArchive);
            selectedPath = directoryPathArchive;
        }

        public void DeleteDirectory(string absolutePath)
        {
            Directory.Delete(absolutePath, true);
        }

    }
}
