using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks.Dataflow;

namespace RenameIamgeFiles
{
    public class RenameFilesTask
    {
        private string sourceDirectory = string.Format("{0}", @"D:\zheng1748@outlook.com\OneDrive\图片\Group10\Group10.35");
        private string filePrefix = string.Format("{0}", "12.0");

        public void Run()
        {
            ProcessNotSortedIamgeFiles();

           // ProcessSortedIamgeFiles();
        }

        public void ProcessNotSortedIamgeFiles()
        {
            //var allNotSortedFiles = GetNotSortedFileInfos();
            //var allSortedFiles = GetSortedFileInfos();

            //int[] chunks = SetChunks(allNotSortedFiles.Count());

            try
            {
                // var chunkInfos = GetNotSortedChunkFileInfos();

                // CreateNewFileName(chunks, allNotSortedFiles);

                //  CreateNewFileNameByChunk(chunkInfos);

                //CreateNewFileNameByTask(chunkInfos);

                var getNotSortedChunkFileInfos = new TransformBlock<string, ChunkFileinfo[]>(dir =>
                {
                    var chunkInfos = GetNotSortedChunkFileInfos(dir);

                    return chunkInfos;
                });

                var createNewFileNames = new ActionBlock<ChunkFileinfo[]>(notSortedChunkInfos =>
                {
                    CreateNewFileNameByTask(notSortedChunkInfos);
                });

                getNotSortedChunkFileInfos.LinkTo(createNewFileNames);

                getNotSortedChunkFileInfos.SendAsync(sourceDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ProcessSortedIamgeFiles()
        {
            try
            {
                var getSortedChunkFileInfos = new TransformBlock<string, ChunkFileinfo[]>(dir =>
                {
                    var chunkInfos = GetSortedChunkFileInfos(dir);

                    return chunkInfos;
                });

                var createNewFileNames = new ActionBlock<ChunkFileinfo[]>(sortedChunkInfos =>
                {
                    CreateNewFileNameByTask(sortedChunkInfos);
                });

                getSortedChunkFileInfos.LinkTo(createNewFileNames);

                getSortedChunkFileInfos.SendAsync(sourceDirectory);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public FileInfo[] GetNotSortedFileInfos()
        {
            var allNotSortedFiles = from fi in
                                    (from f in Directory.GetFiles(sourceDirectory)
                                     select new FileInfo(f))
                                    where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                    select fi;

            var chunks = GetChunkFileinfos(allNotSortedFiles.ToArray());

            return allNotSortedFiles.ToArray();
        }

        public ChunkFileinfo[] GetNotSortedChunkFileInfos(string directory)
        {
            var allNotSortedFiles = from fi in
                                    (from f in Directory.GetFiles(directory)
                                     select new FileInfo(f))
                                    where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                    select fi;

            var chunks = GetChunkFileinfos(allNotSortedFiles.ToArray());

            return chunks;
        }

        public ChunkFileinfo[] GetSortedChunkFileInfos(string directory)
        {
            var allSortedFiles = from fi in
                                (from f in Directory.GetFiles(directory)
                                 select new FileInfo(f))
                                 where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                 orderby GetSortName(fi)
                                 select fi;

            var chunks = GetChunkFileinfos(allSortedFiles.ToArray());

            return chunks;
        }

        public ChunkFileinfo[] GetChunkFileinfos(FileInfo[] fileInfos)
        {
            ChunkFileinfo[] chunkFileinfos = new ChunkFileinfo[9];

            int[] chunks = new int[9];//分成9块
            int chunkSize = fileInfos.Count() / 9;//每一块的数量
            int chunkMore = fileInfos.Count() % 9;//余下的数量

            if (chunkSize == 0)//小于9
            {
                for (int i = 0; i < chunkMore; i++)
                {
                    chunks[i] = 1;
                }
                chunkSize = 1;
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    chunks[i] = chunkSize;
                }
                if (chunkMore > 0)
                {
                    chunks[8] = chunks[8] + chunkMore;
                }
            }

            for (int i = 0; i < 9; i++)
            {
                chunkFileinfos[i] = new ChunkFileinfo() { Index = i,StartPositon= i * chunkSize,Count = chunks[i], FileInfos = new FileInfo[chunks[i]] };
               // chunkFileinfos[i].StartPositon = i * chunkSize;
               // chunkFileinfos[i].Count = chunks[i];
                for (int j = 0; j < chunks[i]; j++)
                {
                    int positon = chunkFileinfos[i].StartPositon + j;
                    chunkFileinfos[i].FileInfos[j] = fileInfos[positon];
                }
            }

            return chunkFileinfos;
        }

        public int[] SetChunks(int filesCount)
        {
            int[] chunks = new int[9];
            int chunkSize = filesCount / 9;
            int chunkMore = filesCount % 9;

            if (chunkSize <= 1)
            {
                int index = Math.Min(9, chunkMore);
                for (int i = 0; i < index; i++)
                {
                    chunks[i] = 1;
                }
            }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    chunks[i] = chunkSize;
                    if (i == 9 && chunkMore > 0)
                    {
                        chunks[8] = chunks[8] + chunkMore;
                    }
                }
            }

            return chunks;
        }

        public FileInfo[] GetSortedFileInfos()
        {
            var allSortedFiles = from fi in
                                    (from f in Directory.GetFiles(sourceDirectory)
                                     select new FileInfo(f))
                                 where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                 orderby GetSortName(fi)
                                 select fi;

            var chunks = GetChunkFileinfos(allSortedFiles.ToArray());

            return allSortedFiles.ToArray();
        }

        public int GetSortName(FileInfo fileInfo)
        {
            try
            {
                var names = fileInfo.Name.Split('.');

                int name = TryParse((names[2])) + TryParse(names[3]) + TryParse(names[4]);

                return name;
            }
            catch
            {
                return 0;
            }
        }

        private int TryParse(string n)
        {
            int result;

            if (int.TryParse(n, out result))
            {
                return result;
            }

            return 0;
        }

        public string[] IamgeFileExtension()
        {
            var names = new string[] { ".gif", ".jpg", ".jpeg", ".png" };
            return names;
        }

        public void CreateNewFileName(int[] chunks, FileInfo[] allFiles)
        {
            Console.WriteLine("开始");

            for (int i = 0; i < 9; i++)
            {
                for (int j = 0; j < chunks[i]; j++)
                {
                    int index = i * chunks[i] + j;
                    string newFileNamePrefix = string.Format("{0}.{1}.{2}", filePrefix, 100 * (i + 1), j + 1);

                    string sourceFileName = allFiles[index].Name;
                    string sourceFileFullName = allFiles[index].FullName;

                    string extensionName = allFiles[index].Extension;
                    string newFileName = string.Format("{0}{1}", newFileNamePrefix, extensionName);
                    string newFileFullName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);

                    Console.WriteLine($"{index}:{sourceFileName}=>{newFileName}");

                    if (!File.Exists(newFileFullName))
                    {
                        File.Copy(sourceFileFullName, newFileFullName);
                        File.Delete(sourceFileFullName);
                    }
                }
            }

            Console.Write("结束");
        }

        public void CreateNewFileNameByChunk(ChunkFileinfo[] chunks)
        {
            Console.WriteLine("开始");

            for (int i = 0; i < chunks.Count(); i++)
            {
                int chunkSize = chunks[i].FileInfos.Count();
                for (int j = 0; j < chunkSize; j++)
                {
                    int index = i * chunkSize + j;
                    string newFileNamePrefix = string.Format("{0}.{1}.{2}", filePrefix, 100 * (i + 1), j + 1);

                    string sourceFileName = chunks[i].FileInfos[j].Name;
                    string sourceFileFullName = chunks[i].FileInfos[j].FullName;

                    string extensionName = chunks[i].FileInfos[j].Extension;
                    string newFileName = string.Format("{0}{1}", newFileNamePrefix, extensionName);
                    string newFileFullName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);

                    Console.WriteLine($"{index}:      {sourceFileName}=>{newFileName}");

                    if (!File.Exists(newFileFullName))
                    {
                        File.Copy(sourceFileFullName, newFileFullName);
                        File.Delete(sourceFileFullName);
                    }
                }
            }

            Console.Write("结束");
        }

        public async void CreateNewFileNameByTask(ChunkFileinfo[] chunks)
        {
            Console.WriteLine("开始");

            var tasks = from c in chunks
                        select Task.Factory.StartNew((state) =>
                        {
                            try
                            {
                                var chunk = state as ChunkFileinfo;
                                for (int j = 0; j < chunk.Count; j++)
                                {
                                    int index = chunk.StartPositon + j;
                                   // string newFileNamePrefix = string.Format("{0}.{1}.{2}", filePrefix, 100 * (chunk.Index + 1), j + 1);
                                    string newFileNamePrefix = $"{filePrefix}.{100 * (chunk.Index + 1)}.{ j + 1}";//每块格式为x.100.x

                                    string sourceFileName = chunk.FileInfos[j].Name;
                                    string sourceFileFullName = chunk.FileInfos[j].FullName;

                                    string extensionName = chunk.FileInfos[j].Extension;
                                    //string newFileName = string.Format("{0}{1}", newFileNamePrefix, extensionName);
                                    //string newFileFullName = string.Format(@"{0}\{1}", sourceDirectory, newFileName);
                                    string newFileName = $"{newFileNamePrefix}{extensionName}";
                                    string newFileFullName = $"{sourceDirectory}\\{newFileName}";

                                    Console.WriteLine($"{index}  {Thread.CurrentThread.ManagedThreadId}  {sourceFileName}=>{newFileName}");

                                    if (!File.Exists(newFileFullName))
                                    {
                                        File.Copy(sourceFileFullName, newFileFullName);
                                        File.Delete(sourceFileFullName);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex.Message);
                            }

                        }, c);

            await Task.WhenAll(tasks);

            Console.Write("结束");
        }
    }
}
