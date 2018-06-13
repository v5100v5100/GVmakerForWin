using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// 个描述文件信息的接口
    /// </summary>
    public interface FileSystemInfo
    {
        /**
  * 是否为一个文件
  */
        public bool isFile();

        /**
         * 是否为一个文件夹
         * @return 当仅当存在且为文件夹时返回true
         */
        public bool isDirectory();

        /**
         * 该文件夹或文件是否可读
         * @return 当且仅当存在且可读时返回true
         */
        public bool canRead();

        /**
         * 该文件夹或文件是否可写
         * @return 当仅当存在且可写时返回true
         */
        public bool canWrite();

        /**
         * 得到文件夹下文件个数
         * @return 为文件夹时返回其目录下文件个数(含子目录);否则返回-1
         */
        public int getFileNum();

        /**
         * 得到目录下第start个开始的num个文件名,保存到names中
         * @param names 用于保存文件名的string数组
         * @param start 开始文件号
         * @param num   个数
         * @return      实际得到的个数,如出错,返回-1
         */
        public int listFiles(string[] names, int start, int num);
    }
}
