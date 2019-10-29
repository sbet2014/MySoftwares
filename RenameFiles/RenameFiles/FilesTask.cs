using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks.Dataflow;
using System.Diagnostics;

namespace RenameIamgeFiles
{
    public class FilesTask
    {
        private string sourceDirectory = string.Format("{0}", @"D:\zheng1748@outlook.com\OneDrive\图片\Group10\5\Group10.01");
        private string filePrefix = string.Format("{0}", "2.0");//至少+3

        public void Run()=> ProcessNotSortedIamgeFiles();
        //{
        //    //乱码文件
        //    // ProcessRamdomIamgeFiles();

        //    ProcessNotSortedIamgeFiles();

        //    // ProcessSortedIamgeFiles();
        //}

        public void ProcessNotSortedIamgeFiles() => ProcessNotSortedIamgeFilesAsync()
                       .ConfigureAwait(continueOnCapturedContext: false);

        public void ProcessRamdomIamgeFiles() => ProcessNotSortedIamgeFilesAsync()
                    .ConfigureAwait(continueOnCapturedContext: false);

        public async Task ProcessRamdomIamgeFilesAsync()
        {
            try
            {
                var allNotSortedFiles = (from fi in
                                         (from f in Directory.GetFiles(sourceDirectory)
                                          select new FileInfo(f))
                                          where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                          select fi).ToList();

                await SendBatchAsync(allNotSortedFiles, default);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public async Task ProcessNotSortedIamgeFilesAsync()
        {
            try
            {
                var allNotSortedFiles = (from fi in
                                         (from f in Directory.GetFiles(sourceDirectory)
                                          select new FileInfo(f))
                                          where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                         orderby GetNotSortName(fi) ascending
                                         select fi).ToList();

                await SendBatchAsync(allNotSortedFiles, default(CancellationToken));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public void ProcessSortedIamgeFiles() => ProcessSortedIamgeFilesAsync().
                       ConfigureAwait(continueOnCapturedContext: false);

        public async Task ProcessSortedIamgeFilesAsync()
        {
            try
            {
                var allSortedFiles = (from fi in
                                       (from f in Directory.GetFiles(sourceDirectory)
                                        select new FileInfo(f))
                                      where IamgeFileExtension().Contains(fi.Extension.ToLowerInvariant())
                                      orderby GetSortName(fi) ascending
                                      select fi).ToList();

                await SendBatchAsync(allSortedFiles, default);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        public int GetNotSortGroupName(FileInfo fileInfo)
        {
            try
            {
                var names = fileInfo.Name.Split('.');

                int name = TryParse((names[0]));

                return name;
            }
            catch
            {
                return 0;
            }
        }

        public int GetNotSortName(FileInfo fileInfo)
        {
            try
            {
                int[] fac = new int[] { 100, 10, 1 };
                var names = fileInfo.Name.Split('.');
                int name = 0;
                for (int i = 1; i < names.Count() - 1; i++)
                {
                    name = name + TryParse(names[i]) * fac[i - 1];
                }

                return name;
            }
            catch
            {
                return 0;
            }
        }

        public int GetSortName(FileInfo fileInfo)
        {
            try
            {
               var names = fileInfo.Name.Split('.');

               int name = TryParse((names[1])) + TryParse(names[2]) + TryParse(names[3]);

                return name;
            }
            catch
            {
                return 0;
            }
        }

        private int TryParse(string n)
        {
          //  int result;

            if (int.TryParse(n,  out int result))
            {
                return result;
            }

            return 0;
        }

        public string[] IamgeFileExtension()=>
             new string[] { ".gif", ".jpg", ".jpeg", ".png" };
          
        #region SendBatchAsync Method
        public async Task SendBatchAsync(IEnumerable<FileInfo> fileInfos, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (fileInfos == null)
            {
                throw new ArgumentNullException("fileInfos is not null");
            }

            if (!fileInfos.Any())
            {
                return;
            }

            var headBlock = default(BufferBlock<BlockInfo<FileInfo>>);
            var writeBlocks = default(List<ActionBlock<BlockInfo<FileInfo>>>);

            int messageCount = fileInfos.Count();
            TimeSpan maxDurationlism = TimeSpan.Zero; ;//花去的最长时间

            try
            {
                #region Methods
                int blockCount = Environment.ProcessorCount * 4;//块数
                int minPerBlock = 4;//至少有一块8条
                int maxPerBlock = 100;//至多
                int maxDegreeOfParallelism = Environment.ProcessorCount * 2;//并行数

                //分块
                int[] blockInfos = default(int[]);
                if (messageCount < (blockCount * maxPerBlock))
                {
                    //分块
                    blockInfos = BlockHelper.MacthBlockInfoDown(blockCount, messageCount, minPerBlock);
                }
                else
                {
                    blockInfos = BlockHelper.MacthBlockInfoUp(blockCount, messageCount, maxPerBlock);
                }

                var blockFileInfos = BlockHelper.GetBlockInfos(blockInfos, fileInfos.ToArray()).ToArray();
                #endregion

                #region Methods
                CreateBlockers(maxDegreeOfParallelism, blockInfos);

                foreach (var msg in blockFileInfos)
                {
                    await headBlock.SendAsync(msg)
                       .ConfigureAwait(continueOnCapturedContext: false);
                }

                //Parallel.ForEach(blockMessages, async msg =>
                //{
                //    try
                //    {
                //        await headBlock.SendAsync(msg);
                //    }
                //    catch (Exception ex)
                //    {
                //        _logger.LogError($"exception: {ex.Message} when block send message");
                //    }
                //});

                headBlock.Complete();
                await headBlock.Completion.ContinueWith(_ =>
                {
                    //  _logger.LogInformation($"{1} message was send by {headBlock.GetType()}.");

                    foreach (var wb in writeBlocks)
                    {
                        wb.Complete();
                        wb.Completion.Wait();
                    }
                });

                Console.WriteLine($"ExecutionTime when send message: '{ maxDurationlism}'");
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error when send message: '{ex.Message}'");
            }

            void CreateBlockers(int maxDegreeOfParallelism, int[] blockInfos)
            {
                #region Methods
                headBlock = new BufferBlock<BlockInfo<FileInfo>>();
                writeBlocks = new List<ActionBlock<BlockInfo<FileInfo>>>(blockInfos.Count());

                //限制容量,做均衡负载
                for (int i = 0; i < blockInfos.Count(); i++)
                {
                    var writeBlock = new ActionBlock<BlockInfo<FileInfo>>(async (infos) =>
                    {
                        var sw = Stopwatch.StartNew();

                        try
                        {
                            (bool sucess, Exception mqex) = await SendMessagesCoreAsync(infos, cancellationToken);

                            maxDurationlism = maxDurationlism.Max(sw.Elapsed);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error when send message: '{ex.Message}'");
                        }
                    },
                    new ExecutionDataflowBlockOptions()
                    {
                        BoundedCapacity = blockInfos[i],//限制容量,做均衡负载
                        CancellationToken = cancellationToken
                    });

                    writeBlocks.Add(writeBlock);
                }

                for (int i = 0; i < writeBlocks.Count(); i++)
                {
                    if (writeBlocks[i] is ITargetBlock<BlockInfo<FileInfo>>)
                    {
                        headBlock.LinkTo(writeBlocks[i], (msgs) =>
                        {
                            return msgs.FileInfos?.Count() > 0;
                        });
                    }
                }
                #endregion
            }
        }

        private async Task<(bool sucess, Exception exception)> SendMessagesCoreAsync(BlockInfo<FileInfo> chunkInfo, CancellationToken cancellationToken)
        {
            Exception exception = null;

            var task = Task.Run(() =>
            {
                bool sucess = true;
                try
                {
                    var fileInfos = chunkInfo.FileInfos;
                    for (int i = 0; i < fileInfos.Count(); i++)
                    {
                        int index = chunkInfo.StartPositon + i;
                      //string newFileNamePrefix = $"{filePrefix}.{100 * (chunkInfo.BlockId + 1)}.{ i + 1}";//每块格式为x.100.x
                       string newFileNamePrefix = $"{filePrefix}.{chunkInfo.BlockId + 1}.{ i + 1}";//每块格式为x.100.x

                        string sourceFileName = fileInfos[i].Name;
                        string sourceFileFullName = fileInfos[i].FullName;

                        string extensionName = fileInfos[i].Extension;
                        string newFileName = $"{newFileNamePrefix}{extensionName}";
                        string newFileFullName = $"{sourceDirectory}\\{newFileName}";

                        Console.WriteLine($"{index+1}  {Thread.CurrentThread.ManagedThreadId}  {sourceFileName}=>{newFileName}");

                        if (!File.Exists(newFileFullName))
                        {
                            File.Copy(sourceFileFullName, newFileFullName);
                            File.Delete(sourceFileFullName);
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    sucess = false;
                }
                return sucess;

            }, cancellationToken);

            var result = await task;

            return (result, exception);
        }
        #endregion
    }
}
