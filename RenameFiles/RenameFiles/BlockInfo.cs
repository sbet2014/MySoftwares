using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameIamgeFiles
{
    public class BlockInfo<T>
    {
        public BlockInfo()
        {

        }

        //第几块
        public int BlockId { get; set; }

        //绝对位置
        public int StartPositon { get; set; }

        //块里的数目
        public int Count => FileInfos.Count();

        public T[] FileInfos { get; set; } = new T[0];
    }
}
