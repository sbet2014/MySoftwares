using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace RenameIamgeFiles
{
    public class FileDto
    {
        public int SortName { get; set; }
        public string ExtensionName { get; set; }
        public string FullName { get; set; }
       
    }
   
    public class BatchRenameFileser
    {
        private string sourceDirectory = string.Format("{0}", @"C:\zheng1748\OneDrive\图片\temp04");
        private string filePrefix = string.Format("{0}", "1.0");

        public void CreateNewIamgeFileNames()
        {
            //string[] allFiles = Directory.GetFiles(sourceDirectory);
            //FileInfo[] fileInfos = new FileInfo[allFiles.Count()];

            var allFiles = (from f in Directory.GetFiles(sourceDirectory)
                             select new FileInfo(f)).
                            Where( f => (f.Extension.ToLowerInvariant() == ".gif")|| 
                                          (f.Extension.ToLowerInvariant() == ".jpg" ) || 
                                          (f.Extension.ToLowerInvariant() == ".jpeg") || 
                                          (f.Extension.ToLowerInvariant() == ".png" )).ToList();

            //for (int i = 0; i < allFiles.Count(); i++)
            //{
            //    fileInfos[i] = new FileInfo(allFiles[i]);
            //}
            int[] chunks = new int[9];
            int chunkSize = allFiles.Count() / 9;
            for (int i = 0; i < 9; i++)
            {
                chunks[i] = chunkSize;
            }
            if (allFiles.Count() % 9 > 0)
            {
                chunks[8] = chunks[8] + allFiles.Count() % 9;
            }
            try
            {
                Console.WriteLine("开始");
                //int startIndex = 0;
                //for (int i = 0; i < allFiles.Count(); i++)
                //{
                //    Console.WriteLine(string.Format("{0}", allFiles[i]));
                //    string sourceFileName = fileInfos[i].FullName;
                //    string newFileName = string.Format("{0}{1}", startIndex + i, Path.GetExtension(allFiles[i]));
                //    string destFileName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);
                //    File.Copy(sourceFileName, destFileName);
                //    File.Delete(sourceFileName);
                //}

                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < chunks[i]; j++)
                    {
                        Console.WriteLine(string.Format("{0}", i * chunkSize + j));
                     
                        int index = i * chunkSize + j;
                        string newFileNamePrefix = string.Format("{0}.{1}.{2}", filePrefix, 100 * (i + 1), j + 1);

                        string sourceFileFullName = allFiles[index].FullName;

                        string extensionName = allFiles[index].Extension;
                        string newFileName = string.Format("{0}{1}", newFileNamePrefix, extensionName);
                        string newFileFullName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);

                        if (!File.Exists(newFileFullName))
                        {
                            File.Copy(sourceFileFullName, newFileFullName);
                            File.Delete(sourceFileFullName);
                        }
                    }
                }

                Console.Write("结束");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ChangeIamgeFileNamesToTwo()
        {
            string[] allFiles = Directory.GetFiles(sourceDirectory);
            FileInfo[] fileInfos = new FileInfo[allFiles.Count()];
            FileDto[] fileDtos = new FileDto[allFiles.Count()];
            for (int i = 0; i < allFiles.Count(); i++)
            {
                fileInfos[i] = new FileInfo(allFiles[i]);
                var names = fileInfos[i].Name.Split('.');
                int name = int.Parse(names[0]);
                string extensionName = fileInfos[i].Extension;

               // string extensionName = names[4];
                //int name = int.Parse(names[2])+ int.Parse(names[3]);

                fileDtos[i] = new FileDto { SortName = name, ExtensionName = extensionName, FullName = fileInfos[i].FullName };
            }
            List<FileDto> fileList = new List<FileDto>(fileDtos);
            var sortedFiles = (from f in fileList
                                 orderby f.SortName
                                 select f).ToArray();
            //var iamgeFiles = allFiles.Where(f => Path.GetExtension(f) == ".gif" || Path.GetExtension(f) == ".GIF" || Path.GetExtension(f) == ".jpg" || Path.GetExtension(f) == ".JPG" ||  Path.GetExtension(f) == ".jpeg" || Path.GetExtension(f) == ".png" || Path.GetExtension(f) == ".PNG").ToArray();
            int[] chunks = new int[9];
            int chunkSize = allFiles.Count() / 9;
            for (int i = 0; i < 9; i++)
            {
                chunks[i] = chunkSize;
            }
            if (allFiles.Count() % 9 > 0)
            {
                chunks[8] = chunks[8] + allFiles.Count() % 9;
            }
            try
            {
                Console.WriteLine("开始");
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < chunks[i]; j++)
                    {
                        Console.WriteLine(string.Format("{0}", i * chunkSize + j));
                        //string sourceFileName = allFiles[i * chunkSize + j];
                        //string newFileName = string.Format("{0}.{1}.{2}{3}", filePrefix, 100 * (i + 1) + j, j + 1, Path.GetExtension(allFiles[i * chunkSize + j]));
                        //string destFileName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);
                        int index = i * chunkSize + j;
                        string indeInChunk = string.Format("{0}.{1}.{2}", filePrefix, 100 * (i + 1), j + 1);

                        string sourceFileName = sortedFiles[index].FullName;
                        string newFileName = string.Format("{0}{1}", indeInChunk, sortedFiles[index].ExtensionName);
                        string destFileName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);
                        File.Copy(sourceFileName, destFileName);
                        File.Delete(sourceFileName);
                    }
                }

                Console.Write("结束");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ChangeIamgeFileNames()
        {
            string[] allFiles = Directory.GetFiles(sourceDirectory);
            FileInfo[] fileInfos = new FileInfo[allFiles.Count()];
            FileDto[] fileDtos = new FileDto[allFiles.Count()];
            for (int i = 0; i < allFiles.Count(); i++)
            {
                fileInfos[i] = new FileInfo(allFiles[i]);
                var names = fileInfos[i].Name.Split('.');
                //int name = int.Parse(names[0]);
                //string extensionName = fileInfos[i].Extension;

                string extensionName = fileInfos[i].Extension;
                int name = int.Parse(names[2]) + int.Parse(names[3]);

                fileDtos[i] = new FileDto { SortName = name, ExtensionName = extensionName, FullName = fileInfos[i].FullName };
            }
            List<FileDto> fileList = new List<FileDto>(fileDtos);
            var sortedFiles = (from f in fileList
                               orderby f.SortName
                               select f).ToArray();
            //var iamgeFiles = allFiles.Where(f => Path.GetExtension(f) == ".gif" || Path.GetExtension(f) == ".GIF" || Path.GetExtension(f) == ".jpg" || Path.GetExtension(f) == ".JPG" ||  Path.GetExtension(f) == ".jpeg" || Path.GetExtension(f) == ".png" || Path.GetExtension(f) == ".PNG").ToArray();
            int[] chunks = new int[9];
            int chunkSize = allFiles.Count() / 9;
            for (int i = 0; i < 9; i++)
            {
                chunks[i] = chunkSize;
            }
            if (allFiles.Count() % 9 > 0)
            {
                chunks[8] = chunks[8] + allFiles.Count() % 9;
            }
            try
            {
                Console.WriteLine("开始");
                for (int i = 0; i < 9; i++)
                {
                    for (int j = 0; j < chunks[i]; j++)
                    {
                        Console.WriteLine(string.Format("{0}", i * chunkSize + j));
                        //string sourceFileName = allFiles[i * chunkSize + j];
                        //string newFileName = string.Format("{0}.{1}.{2}{3}", filePrefix, 100 * (i + 1) + j, j + 1, Path.GetExtension(allFiles[i * chunkSize + j]));
                        //string destFileName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);
                        int index = i * chunkSize + j;
                        string indeInChunk = string.Format("{0}.{1}.{2}", filePrefix, 100 * (i + 1), j + 1);

                        string sourceFileName = sortedFiles[index].FullName;
                        string newFileName = string.Format("{0}{1}", indeInChunk, sortedFiles[index].ExtensionName);
                        string destFileName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);
                        File.Copy(sourceFileName, destFileName);
                        File.Delete(sourceFileName);
                    }
                }

                Console.Write("结束");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
