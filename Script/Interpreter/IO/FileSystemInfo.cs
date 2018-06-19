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

        /// <summary>
        /// 是否为一个文件
        /// </summary>
        /// <returns></returns>
        public bool isFile();

        /// <summary>
        /// 是否为一个文件夹
        /// </summary>
        /// <returns>当仅当存在且为文件夹时返回true</returns>
        public bool isDirectory();

        /// <summary>
        /// 该文件夹或文件是否可读
        /// 
        /// </summary>
        /// <returns>当且仅当存在且可读时返回true</returns>
        public bool canRead();

        /// <summary>
        /// 该文件夹或文件是否可写
        /// </summary>
        /// <returns>当仅当存在且可写时返回true</returns>
        public bool canWrite();

        /// <summary>
        /// 得到文件夹下文件个数
        /// 
        /// </summary>
        /// <returns>为文件夹时返回其目录下文件个数(含子目录);否则返回-1</returns>
        public int getFileNum();

        /// <summary>
        /// 得到目录下第start个开始的num个文件名,保存到names中
        /// 
        /// </summary>
        /// <param name="names">用于保存文件名的string数组</param>
        /// <param name="start">开始文件号</param>
        /// <param name="num">个数</param>
        /// <returns>实际得到的个数,如出错,返回-1</returns>
        public int listFiles(string[] names, int start, int num);
    }
}
