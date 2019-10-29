using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RenameIamgeFiles
{
    public class BlockHelper
    {
        public static int[] GetBlockInfo(int blockCount, int totalCount)
        {
            int[] chunks = new int[blockCount];//分成x块
            int chunkSize = totalCount / blockCount;//每一块的数量
            int chunkMore = totalCount % blockCount;//余下的数量

            if (chunkSize == 0)//小于x
            {
                for (int i = 0; i < chunkMore; i++)
                {
                    chunks[i] = 1;
                }
                chunkSize = 1;
            }
            else
            {
                for (int i = 0; i < blockCount; i++)
                {
                    chunks[i] = chunkSize;
                }
                if (chunkMore > 0)
                {
                    //chunks[blockCount - 1] = chunks[blockCount - 1] + chunkMore;
                    for (int i = 0; i < blockCount && chunkMore > 0; i++)
                    {
                        chunks[i] = chunks[i] + 1;
                        chunkMore--;
                    }
                }
            }

            return chunks;
        }

        public static int[] MacthBlockInfoDown(int blockCount, int totalCount, int minnumPerBlock)
        {
            int fac = blockCount;
            var blockInfos = GetBlockInfo(fac, totalCount);

            bool isLittle = blockInfos.Any(b => b <= 0) || !blockInfos.Any(b => b >= minnumPerBlock);//至少有一个

            while (isLittle && fac > 1)
            {
                fac = fac - 1;
                blockInfos = GetBlockInfo(fac, totalCount);
                isLittle = blockInfos.Any(b => b <= 0) || !blockInfos.Any(b => b >= minnumPerBlock);
                if (isLittle && fac == 2)
                {
                    if (blockInfos.Count() == 2 && blockInfos.Sum(b => b) > minnumPerBlock)
                        isLittle = false;
                }
            };

            return blockInfos;
        }

        public static int[] MacthBlockInfoUp(int blockCount, int totalCount, int maxnumPerBlock)
        {
            int fac = blockCount;
            var blockInfos = GetBlockInfo(fac, totalCount);

            bool isLarge = blockInfos.Any(b => b > maxnumPerBlock);

            while (isLarge)
            {
                fac = fac + 1;
                blockInfos = GetBlockInfo(fac, totalCount);
                isLarge = blockInfos.Any(b => b > maxnumPerBlock);
            };

            return blockInfos;
        }

        public static IEnumerable<T[]> GetMessageByBlockInfo<T>(int[] blockInfos, T[] messages)
        {
            var count = blockInfos.Sum(b => b);
            var blockMessages = new List<T[]>(count);

            int startPositon = 0;
            for (int i = 0; i < blockInfos.Count(); i++)
            {
                var msgScope = new T[blockInfos[i]];
                for (int j = 0; j < blockInfos[i]; j++)
                {
                    int positon = startPositon + j;
                    var msg = messages[positon];
                    msgScope[j] = msg;
                }
                startPositon += blockInfos[i];//重置起始位置
                blockMessages.Add(msgScope);
            }

            return blockMessages;
        }

        public static IEnumerable<BlockInfo<T>> GetBlockInfos<T>(int[] blockInfos, T[] messages)
        {
            var count = blockInfos.Sum(b => b);
            var blockMessages = new List<BlockInfo<T>>(count);

            int startPositon = 0;
            for (int i = 0; i < blockInfos.Count(); i++)
            {
                BlockInfo<T> bloInfo = new BlockInfo<T>() { BlockId = i, StartPositon = startPositon };

                var msgScope = new T[blockInfos[i]];
                for (int j = 0; j < blockInfos[i]; j++)
                {
                    int positon = startPositon + j;
                    var msg = messages[positon];
                    msgScope[j] = msg;
                }
                startPositon += blockInfos[i];//重置起始位置
                bloInfo.FileInfos = msgScope;
                blockMessages.Add(bloInfo);
            }

            return blockMessages;
        }
    }
}

