using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileRedundancyRemover.Logic
{
    public class FileChangesManager
    {
        #region Fields

        private string sourcePath;
        private string targetPath;

        #endregion Fields

        #region Properties



        #endregion Properties

        #region Constructor

        #endregion Constructor

        #region Public Methods

        public void EqualizeFolders(string source, string target)
        {
            var sourceFolder = new DirectoryInfo(source);
            var targetFolder = new DirectoryInfo(target);
            var sourceFolderDirectories = sourceFolder.GetDirectories();
            var targetFolderDirectories = targetFolder.GetDirectories();

            // remove redundant folders from target
            //RemoveFoldersFromTarget(sourceFolderDirectories, targetFolderDirectories);

            // remove folders from subfolders

            // remove redundant files from target
            //RemoveFilesFromTarget(sourceFolder.GetFiles(), targetFolder.GetFiles());

            // copy folders to target
            //CopyDirectory(source, target);

            // check if folder isn't missing any files


            // add missing files from source to target
            AddMissingFilesToTarget(sourceFolder, targetFolder);
        }

        #endregion Public Methods


        #region Private Methods

        private void RemoveFoldersFromTarget(DirectoryInfo[] foldersInSource, DirectoryInfo[] foldersInTarget)
        {
            foreach (var folder in foldersInTarget)
            {
                if (foldersInSource.Contains(folder))
                    continue;
                folder.Delete(true);
            }
        }

        private void RemoveFilesFromTarget(FileInfo[] sourceFiles, FileInfo[] targetFiles)
        {
            var indexesToRemove = targetFiles.Where(x => !sourceFiles.Any(y => y.Name.Equals(x.Name)))
                                  .Select(x => Array.IndexOf(targetFiles, x)).ToList();

            foreach (var index in indexesToRemove)
            {
                targetFiles[index].Delete();
            }
        }

        private void CopyDirectory(string sourceDirName, string destDirName)
        {
            var dirInfo = new DirectoryInfo(sourceDirName);

            if (!dirInfo.Exists)
                throw new DirectoryNotFoundException($"Directory \"{sourceDirName}\" doesn't exist.");

            var directories = dirInfo.GetDirectories();

            if (!Directory.Exists(destDirName))
                Directory.CreateDirectory(destDirName);

            var files = dirInfo.GetFiles();

            foreach (var file in files)
            {
                var tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            foreach (var subDirectory in directories)
            {
                var tempPath = Path.Combine(destDirName, subDirectory.Name);
                CopyDirectory(subDirectory.FullName, tempPath);
            }
        }

        private void AddMissingFilesToTarget(DirectoryInfo sourceFolder, DirectoryInfo targetFolder)
        {
            var sourceFiles = sourceFolder.GetFiles();
            var targetFiles = targetFolder.GetFiles();

            if (!sourceFiles.Any())
                return; 
            
            var indexesToCopy = sourceFiles.Where(x => !targetFiles.Any(y => y.Name.Equals(x.Name)))
                                .Select(x => Array.IndexOf(sourceFiles, x)).ToList();
            
            foreach (var index in indexesToCopy)
            {
                var path = Path.Combine(targetFolder.FullName, sourceFiles[index].Name);
                sourceFiles[index].CopyTo(path);
            }
        }

        #endregion Private Methods
    }
}
