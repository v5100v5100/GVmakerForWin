using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// 文件系统,实现GVM中的各种文件操作功能
    /// </summary>
    public interface FileModel
    {
        /// <summary>
        /// 文件名的最大长度(指转化为gb2312编码后的长度)
        /// </summary>
        public static int FILE_NAME_LENGTH = 18;


        /// <summary>
        /// 改变当前工作目录
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool changeDir(Getable source, int addr);

        /// <summary>
        /// 创建文件夹
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool makeDir(Getable source, int addr);

        /// <summary>
        /// 得到当前目录下的文件个数
        /// </summary>
        /// <returns>文件夹个数</returns>
        public int getFileNum();

        /// <summary>
        /// 得到当前目录下第start个开始的num个文件名,保存到names中
        /// </summary>
        /// <param name="names">用于保存文件名的string数组</param>
        /// <param name="start">开始文件号</param>
        /// <param name="num">个数</param>
        /// <returns>实际得到的个数,如出错,返回-1</returns>
        public int listFiles(string[] names, int start, int num);

        /// <summary>
        /// 打开文件
        /// </summary>
        /// <param name="source">保存数据的源</param>
        /// <param name="fileName">文件名开始地址</param>
        /// <param name="openMode">打开模式开始地址</param>
        /// <returns>文件号,低8位有效</returns>
        public int fopen(Getable source, int fileName, int openMode);

        /// <summary>
        /// 关闭文件
        /// 需要关闭的文件号
        /// </summary>
        /// <param name="fp"></param>
        public void fclose(int fp);

        /// <summary>
        /// 从指定文件读取一个byte
        /// </summary>
        /// <param name="fp">文件号</param>
        /// <returns>读取的字符,低八位有效;若失败返回-1</returns>
        public int getc(int fp);

        /// <summary>
        /// 写入一个字符到指定文件
        /// 
        /// </summary>
        /// <param name="c">要写入的字符,低八位有效</param>
        /// <param name="fp">文件号</param>
        /// <returns>写入的字符,若失败返回-1</returns>
        public int putc(int c, int fp);

        /// <summary>
        /// 读取一段数据
        /// </summary>
        /// <param name="dest">保存数据的Setable</param>
        /// <param name="addr">数据在dest中保存的开始地址</param>
        /// <param name="size">需要读取数据的长度</param>
        /// <param name="fp">文件号</param>
        /// <returns>读取数据的长度,如发生IO错误或遇文件结尾返回0</returns>
        public int fread(Setable dest, int addr, int size, int fp);


        /// <summary>
        /// 写入一段数据
        /// </summary>
        /// <param name="source">需要写入的数据所在的Getable</param>
        /// <param name="addr">数据在source中的开始地址</param>
        /// <param name="size">写入数据的长度</param>
        /// <param name="fp">文件号</param>
        /// <returns>写入数据的长度,如发生IO错误或遇到文件结尾返回0</returns>
        public int fwrite(Getable source, int addr, int size, int fp);

        /// <summary>
        /// 删除文件
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool deleteFile(Getable source, int addr);

        /// <summary>
        /// 文件指针定位
        /// </summary>
        /// <param name="fp">文件号</param>
        /// <param name="offset">编译量</param>
        /// <param name="base_">基点</param>
        /// <returns>定位后的文件指针,若出错返回-1</returns>
        public int fseek(int fp, int offset, int base_);


        /// <summary>
        /// 得到文件指针
        /// </summary>
        /// <param name="fp">文件号</param>
        /// <returns>文件指针</returns>
        public int ftell(int fp);

        /// <summary>
        /// 检查文件是否已结束
        /// </summary>
        /// <param name="fp">文件号</param>
        /// <returns>true,如果已结束;否则false</returns>
        public bool feof(int fp);

        /// <summary>
        /// 文件指针复位
        /// </summary>
        /// <param name="fp">文件号</param>
        public void rewind(int fp);

        /// <summary>
        /// 关闭所有文件,释放占用的资源
        /// </summary>
        public void dispose();
    }
}
