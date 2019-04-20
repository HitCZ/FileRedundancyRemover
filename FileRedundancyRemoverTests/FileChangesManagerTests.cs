using FileRedundancyRemover.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace FileRedundancyRemoverTests
{
    [TestClass]
    public class FileChangesManagerTests
    {
        #region Fields

        private const string SOURCE_FOLDER_PATH = "SourceFolder";
        private const string TARGET_FOLDER_PATH = "TargetFolder";
        private DirectoryInfo sourceDirectory;
        private DirectoryInfo targetDirectory;
        private List<FileStream> files;
        private FileChangesManager fileManager;

        #endregion Fields

        [TestInitialize]
        public void TestInitialize()
        {
            fileManager = new FileChangesManager();
            sourceDirectory = Directory.CreateDirectory(SOURCE_FOLDER_PATH);
            targetDirectory = Directory.CreateDirectory(TARGET_FOLDER_PATH);

            Directory.CreateDirectory(Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory"));
            Directory.CreateDirectory(Path.Combine(Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory"), "SubDirectory2"));
            Directory.CreateDirectory(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"));
            Directory.CreateDirectory(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory3"));
            Directory.CreateDirectory(Path.Combine(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"), "SubDirectory4"));
            Directory.CreateDirectory(Path.Combine(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"), "SubDirectory2"));
            Directory.CreateDirectory(Path.Combine(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"), "SubDirectory5"));

            files = new List<FileStream>
            {
                File.Create(Path.Combine(SOURCE_FOLDER_PATH, "file1.txt")),
                File.Create(Path.Combine(SOURCE_FOLDER_PATH, "file2.txt")),
                File.Create(Path.Combine(TARGET_FOLDER_PATH, "file1.txt")),
                File.Create(Path.Combine(TARGET_FOLDER_PATH, "file3.txt")),
                File.Create(Path.Combine(Path.Combine(Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory"), "file4.txt"))),
                File.Create(Path.Combine(Path.Combine(Path.Combine(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"), "SubDirectory2")), "file5.txt"))
            };


            CloseStreams();
        }

        [TestMethod]
        public void EqualizeFoldersTest()
        {
            fileManager.EqualizeFoldersAsync(SOURCE_FOLDER_PATH, TARGET_FOLDER_PATH);

            var expectedRootSubFolderCount = 1;
            var actualRootSubFolderCount = targetDirectory.GetDirectories().Length;

            var expectedSubDirsCount = 1;
            var actualSubDirsCount = targetDirectory.GetDirectories()[0].GetDirectories().Length;

            Assert.AreEqual(expectedRootSubFolderCount, actualRootSubFolderCount);
            Assert.IsTrue(targetDirectory.GetFiles().Length == 2);
            Assert.IsTrue(targetDirectory.GetFiles().Any(x => x.Name =="file1.txt") && targetDirectory.GetFiles().Any(x => x.Name == "file2.txt"));
            Assert.AreEqual(expectedSubDirsCount, actualSubDirsCount);
            Assert.IsTrue(targetDirectory.GetDirectories()[0].GetFiles().Length == 1);
            Assert.IsTrue(targetDirectory.GetDirectories()[0].GetFiles().Any(x => x.Name == "file4.txt"));
        }

        [TestMethod]
        public void RemoveRedundantFoldersFromTargetTest()
        {
            fileManager.RemoveRedundantFoldersFromTarget(sourceDirectory.GetDirectories(), targetDirectory.GetDirectories());

            if (!targetDirectory.GetDirectories().Any())
                Assert.Fail();

            var expectedLength = 1;
            var actualLength = targetDirectory.GetDirectories().Length;

            Assert.AreEqual(expectedLength, actualLength);
            Assert.IsTrue(targetDirectory.GetDirectories()[0].Name.Equals("SubDirectory"));
        }

        [TestMethod]
        public void RemoveRedundantFilesFromTargetTest()
        {
            fileManager.RemoveRedundantFilesFromTarget(sourceDirectory.GetFiles(), targetDirectory.GetFiles());

            if (!targetDirectory.GetFiles().Any())
                Assert.Fail();

            var expectedLength = 1;
            var actualLength = targetDirectory.GetFiles().Length;

            Assert.AreEqual(expectedLength, actualLength);
            Assert.IsTrue(targetDirectory.GetFiles()[0].Name.Equals("file1.txt"));
        }

        [TestMethod]
        public void CopyDirectoryTest()
        {
            fileManager.CopyDirectory(SOURCE_FOLDER_PATH, TARGET_FOLDER_PATH);

            if (!targetDirectory.GetDirectories().Any())
                Assert.Fail();

            var expectedSubDirCount = 3;
            var actualSubDirCount = targetDirectory.GetDirectories()[0].GetDirectories().Length;

            var expectedFileCount = 3;
            var actualFileCount = targetDirectory.GetFiles().Length;

            var expectedSubDirFileCount = 1;
            var actualSubDirFileCount = targetDirectory.GetDirectories()[0].GetFiles().Length;
            
            Assert.AreEqual(expectedSubDirCount, actualSubDirCount);
            Assert.AreEqual(expectedFileCount, actualFileCount);
            Assert.AreEqual(expectedSubDirFileCount, actualSubDirFileCount);
        }

        [TestMethod]
        public void RemoveRedundantFoldersAndSubFoldersTest()
        {
            fileManager.RemoveRedundantFoldersAndSubFolders(sourceDirectory, targetDirectory);

            var expectedRootFoldersCount = 1;
            var actualRootFoldersCount = targetDirectory.GetDirectories().Length;

            var expectedSubDirsCount = 1;
            var actualSubDirsCount = targetDirectory.GetDirectories().OrderBy(x => x.Name).ToList()[0]
                .GetDirectories().Length;

            Assert.AreEqual(expectedRootFoldersCount, actualRootFoldersCount);
            Assert.AreEqual(expectedSubDirsCount, actualSubDirsCount);
        }

        [TestMethod]
        public void RemoveRedundantFilesFromFoldersAndSubFoldersTest()
        {
            fileManager.RemoveRedundantFilesFromFoldersAndSubFolders(sourceDirectory, targetDirectory);

            var expectedRootFilesCount = 1;
            var actualRootFilesCount = targetDirectory.GetFiles().Length;

            var expectedSubDirFilesCount = 0;
            var actualSubDirFilesCount 
                = targetDirectory.GetDirectories().OrderBy(x => x.Name).ToList()[0].GetFiles().Length;

            var expectedSubSubDirFilesCount = 0;
            var actualSubSubDirFilesCount =
                targetDirectory.GetDirectories().OrderBy(x => x.Name).ToList()[0].GetDirectories()[0].GetFiles().Length;

            Assert.AreEqual(expectedRootFilesCount, actualRootFilesCount);
            Assert.AreEqual(expectedSubDirFilesCount, actualSubDirFilesCount);
            Assert.AreEqual(expectedSubSubDirFilesCount, actualSubSubDirFilesCount);
        }

        [TestCleanup]
        public void TestCleanup()
        {
            sourceDirectory.Delete(true);
            targetDirectory.Delete(true);
        }

        private void CloseStreams()
        {
            foreach (var fileStream in files)
            {
                fileStream.Close();
            }
        }
    }
}
