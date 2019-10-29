using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameIamgeFiles
{
    public class ChunkFileinfo
    {
        //第几块
        public int Index { get; set; }

        //绝对位置
        public int StartPositon { get; set; }

        //块里的数目
        public int Count { get; set; }

        public FileInfo[] FileInfos { get; set; }
    }
}
