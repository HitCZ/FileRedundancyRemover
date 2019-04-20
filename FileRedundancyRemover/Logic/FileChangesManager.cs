using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileRedundancyRemover.Properties;

namespace FileRedundancyRemover.Logic
{
    public class FileChangesManager
    {
        public Action<ProgressState> ProgressChangedAction { get; set; }

        #region Public Methods

        public async Task EqualizeFoldersAsync(string source, string target)
        {
            var sourceFolder = new DirectoryInfo(source);
            var targetFolder = new DirectoryInfo(target);

            InvokeProgressChanged(0, Strings.MSG_RemovingRedundantFolders);
            await Task.Run(() => RemoveRedundantFoldersAndSubFolders(sourceFolder, targetFolder));

            InvokeProgressChanged(33, Strings.MSG_RemovingRedundantFiles);
            await Task.Run(() => RemoveRedundantFilesFromFoldersAndSubFolders(sourceFolder, targetFolder));

            InvokeProgressChanged(66, Strings.MSG_CopyingFiles);
            await Task.Run(() => CopyDirectory(sourceFolder.FullName, targetFolder.FullName));

            InvokeProgressChanged(100, Strings.MSG_ProcessComplete);
        }

        public void RemoveRedundantFilesFromFoldersAndSubFolders(DirectoryInfo sourceFolder, DirectoryInfo targetFolder)
        {
            var sourceFiles = sourceFolder.GetFiles();
            var targetFiles = targetFolder.GetFiles();
            var sourceSubDirs = sourceFolder.GetDirectories();
            var targetSubDirs = targetFolder.GetDirectories();

            RemoveRedundantFilesFromTarget(sourceFiles, targetFiles);

            foreach (var subDir in sourceSubDirs)
            {
                var targetSubDir = targetSubDirs.FirstOrDefault(x => x.Name.Equals(subDir.Name));

                if (targetSubDir is null)
                    continue;

                RemoveRedundantFilesFromTarget(subDir.GetFiles(), targetSubDir.GetFiles());
                RemoveRedundantFilesFromFoldersAndSubFolders(subDir, targetSubDir);
            }
        }

        public void RemoveRedundantFoldersAndSubFolders(DirectoryInfo sourceFolder, DirectoryInfo targetFolder)
        {
            var sourceSubDirs = sourceFolder.GetDirectories();
            var targetSubDirs = targetFolder.GetDirectories();

            // remove unwanted folders in current folder
            RemoveRedundantFoldersFromTarget(sourceSubDirs, targetSubDirs);

            foreach (var folder in sourceSubDirs)
            {
                var sourceSubFolders = folder.GetDirectories();
                var targetSubFolders = targetSubDirs.First(x => x.Name == folder.Name).GetDirectories();

                RemoveRedundantFoldersFromTarget(folder.GetDirectories(), targetSubFolders);

                // recursively remove all unwanted folders in every subfolder
                foreach (var sourceSubFolder in sourceSubFolders)
                {
                    var tmp = targetSubFolders.FirstOrDefault(x => x.Name == sourceSubFolder.Name);

                    if (tmp is null)
                        continue;

                    RemoveRedundantFoldersAndSubFolders(sourceSubFolder, tmp);
                }
            }
        }

        public void RemoveRedundantFoldersFromTarget(DirectoryInfo[] foldersInSource, DirectoryInfo[] foldersInTarget)
        {
            foreach (var folder in foldersInTarget)
            {
                if (foldersInSource.Any(x => x.Name.Equals(folder.Name)))
                    continue;
                folder.Delete(true);
            }
        }

        public void RemoveRedundantFilesFromTarget(FileInfo[] sourceFiles, FileInfo[] targetFiles)
        {
            var indexesToRemove = targetFiles.Where(x => !sourceFiles.Any(y => y.Name.Equals(x.Name)))
                                  .Select(x => Array.IndexOf(targetFiles, x)).ToList();

            foreach (var index in indexesToRemove)
            {
                targetFiles[index].Delete();
            }
        }

        public void CopyDirectory(string sourceDirName, string destDirName)
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
                try
                {
                    file.CopyTo(tempPath, false);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                    Console.WriteLine(ex.StackTrace);
                }
            }

            foreach (var subDirectory in directories)
            {
                var tempPath = Path.Combine(destDirName, subDirectory.Name);
                CopyDirectory(subDirectory.FullName, tempPath);
            }
        }

        #endregion Public Methods

        #region Private Methods

        private void InvokeProgressChanged(double progress, string text)
        {
            var progressState = new ProgressState
            {
                Progress = progress,
                ProgressText = text
            };
            ProgressChangedAction.Invoke(progressState);
        }

        #endregion Private Methods
    }
}
