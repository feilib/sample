using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ConsoleTest
{
    class Program
    {

        static void Main(string[] args)
        {
            //下面的调用前后可能会有影响，最好每次只开放一个运行

            //实验1：小堆60k  无GC  VS  强制GC
            //Fill(60000, false, true, false);
            //Fill(60000, false, true, true);

            //实验2：大堆90k  无GC  VS  强制GC
            //Fill(90000, false, true, false);
            //Fill(90000, false, true, true);

            //实验3：小堆60k  固定大堆16M 无GC  VS  强制GC
            //Fill(60000, true, false, false);
            //Fill(60000, true, false, true);

            //实验4：大堆90k  固定大堆16M 无GC  VS  强制GC
            //Fill(90000, true, false, false);
            //Fill(90000, true, false, true);

            //实验5：小堆60k  自增大堆16M 无GC  VS  强制GC
            //Fill(60000, true, true, false);
            //Fill(60000, true, true, true);

            //实验6：大堆90k  自增大堆16M 无GC  VS  强制GC
            //Fill(90000, true, true, false);
            //Fill(90000, true, true, true);

            Console.ReadLine();
        }

        static byte[] bigBlock;

        /// <summary>
        /// 内存测试
        /// </summary>
        static void Fill(int blockSize, bool allocateBigBlocks, bool grow, bool alwaysGC)
        {
            DateTime dtenter = DateTime.Now;
            ;

            // 大块尺寸: 默认初始16M
            int largeBlockSize = 1 << 24;

            // 申请小块的数量
            int count = 0;

            try
            {
                // 把小块保存起来，不让回收。
                List<byte[]> smallBlocks = new List<byte[]>();

                for (; ; )
                {
                    // 显示小块的数量
                    if ((count % 1000) == 0)
                    {
                        Console.CursorLeft = 0;
                        Console.Write(new string(' ', 20));
                        Console.CursorLeft = 0;
                        Console.Write("{0}", count);
                        Console.CursorLeft = 0;
                    }

                    // 强制GC
                    if (alwaysGC) GC.Collect();

                    // 申请一个临时的大块
                    if (allocateBigBlocks)
                    {
                        bigBlock = new byte[largeBlockSize];
                    }

                    // 大块递增的话，每循环，大块稍微长大一点
                    if (grow) largeBlockSize++;

                    // 保存小块。。。
                    smallBlocks.Add(new byte[blockSize]);
                    count++;
                }
            }
            catch (OutOfMemoryException e)
            {
                //记录一下时间
                DateTime dtExce = DateTime.Now;

                // 结束了，所有都清零，然后强制GC
                bigBlock = null;
                GC.Collect();


                // 显示溢出是申请小块的数量和条件
                Console.WriteLine("{0}: {1}Mb allocated"
                    , "block size:" + blockSize / 1000 + "k\t"
                      + (allocateBigBlocks ? "WithLarge(16M)" : "OnlySmall")
                      + ",\t"
                      + (alwaysGC ? "force GC" : "auto GC")
                      + ",\t"
                      + (grow ? ",growing" : ",not growing\t")
                      + ",\t"
                    , (count * blockSize) / (1024 * 1024));

                Console.WriteLine("consume second(s):" + (dtExce - dtenter).TotalSeconds);


            }
        }

    }
}