﻿using FileRedundancyRemover.Logic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;

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
            files = new List<FileStream>
            {
                File.Create(Path.Combine(SOURCE_FOLDER_PATH, "file1.txt")),
                File.Create(Path.Combine(SOURCE_FOLDER_PATH, "file2.txt")),
                File.Create(Path.Combine(TARGET_FOLDER_PATH, "file1.txt")),
                File.Create(Path.Combine(TARGET_FOLDER_PATH, "file3.txt"))
            };

            Directory.CreateDirectory(Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory"));
            Directory.CreateDirectory(Path.Combine(Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory"), "SubDirectory2"));

            Directory.CreateDirectory(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"));
            Directory.CreateDirectory(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory3"));
            //File.Create(Path.Combine(TARGET_FOLDER_PATH, "file1.txt"));

            CloseStreams();
        }

        [TestMethod]
        public void RemoveRedundantFoldersFromTargetTest()
        {
            fileManager.RemoveRedundantFoldersFromTarget(sourceDirectory.GetDirectories(), targetDirectory.GetDirectories());

            var expectedLength = 1;
            var actualLength = targetDirectory.GetDirectories().Length;

            Assert.AreEqual(expectedLength, actualLength);
            Assert.IsTrue(targetDirectory.GetDirectories()[0].Name.Equals("SubDirectory"));
        }

        [TestMethod]
        public void RemoveRedundantFilesFromTargetTest()
        {
            fileManager.RemoveRedundantFilesFromTarget(sourceDirectory.GetFiles(), targetDirectory.GetFiles());

            var expectedLength = 1;
            var actualLength = targetDirectory.GetFiles().Length;

            Assert.AreEqual(expectedLength, actualLength);
            Assert.IsTrue(targetDirectory.GetFiles()[0].Name.Equals("file1.txt"));
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
