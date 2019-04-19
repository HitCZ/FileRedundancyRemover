using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace FileRedundancyRemoverTests
{
    [TestClass]
    public class FIleChangesManagerTests
    {
        #region Fields

        private const string SOURCE_FOLDER_PATH = "SourceFolder";
        private const string TARGET_FOLDER_PATH = "TargetFolder";
        private DirectoryInfo sourceDirectory;
        private DirectoryInfo targetDirectory;

        #endregion Fields

        [TestInitialize]
        public void TestInitialize()
        {
            sourceDirectory = Directory.CreateDirectory(SOURCE_FOLDER_PATH);
            targetDirectory = Directory.CreateDirectory(TARGET_FOLDER_PATH);

            File.Create(Path.Combine(SOURCE_FOLDER_PATH, "file1.txt"));
            File.Create(Path.Combine(SOURCE_FOLDER_PATH, "file2.txt"));

            File.Create(Path.Combine(TARGET_FOLDER_PATH, "file1.txt"));
            File.Create(Path.Combine(TARGET_FOLDER_PATH, "file3.txt"));

            Directory.CreateDirectory(Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory"));
            var subDirPath = Path.Combine(SOURCE_FOLDER_PATH, "SubDirectory");
            Directory.CreateDirectory(Path.Combine(subDirPath, "SubDirectory2"));


            Directory.CreateDirectory(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory"));
            Directory.CreateDirectory(Path.Combine(TARGET_FOLDER_PATH, "SubDirectory3"));
            //File.Create(Path.Combine(TARGET_FOLDER_PATH, "file1.txt"));
        }

        [TestMethod]
        public void RemoveFoldersFromTargetTest()
        {
            //var manager = new FileChangesManager();
        }

        [TestCleanup]
        public void TestCleanup()
        {
            sourceDirectory.Delete(true);
            targetDirectory.Delete(true);
        }
    }
}