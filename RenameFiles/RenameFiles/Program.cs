using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameIamgeFiles
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //BatchRenameFileser batchRenameFileser = new BatchRenameFileser();
            //batchRenameFileser.ChangeIamgeFileNamesToOne();
            //batchRenameFileser.ChangeIamgeFileNamesToTwo();
            // batchRenameFileser.ChangeIamgeFileNames();

            //MoveFileRuner runer = new MoveFileRuner();
            //runer.Run();

            //RenameFilesTask renameFilesTask = new RenameFilesTask();
            //renameFilesTask.Run();

            var filesTask = new FilesTask();
            // filesTask.Run();

            await filesTask.ProcessNotSortedIamgeFilesAsync();

            //await filesTask.ProcessSortedIamgeFilesAsync();

            Console.ReadLine();
        }
    }
}
