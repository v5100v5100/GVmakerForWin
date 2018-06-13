using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// 文件系统接口,通过该接口将GVM中的文件操作映射到系统中的文件操作
    /// 注意:方法中涉及的文件名都是GVM中用到的文件名,不一定与底层实际文件一一对应.其具体解释由实现者提供
    /// </summary>
    public interface FileSystem
    {
        
    /**
     * 得到该文件的InputStream,以读取其内容
     * @return in 当文件存在且canRead返回true时返回指向该文件的InputStream
     * @throws IOException 文件不存在或不可读或发生IO错误
     */
    //public InputStream getInputStream(string fileName) throws IOException;
        public FileStream getInputStream(string fileName);

    /**
     * 得到该文件的OutputStream以向其写入内容<p>
     * 当文件不存在时会创建一个新的文件
     * @return out  返回指向该文件的OutputStream
     * @throws IOException 若文件不可写或发生IO错误
     */
    //public OutputStream getOutputStream(string fileName) throws IOException;
     public FileStream getOutputStream(string fileName);

    /**
     * 删除文件
     * @param fileName 文件名
     * @return true,如果删除成功
     */
    public bool deleteFile(string fileName);

    /**
     * 建立文件夹
     * @param dirName 文件夹名
     * @return true,如果创建成功
     */
    public bool makeDir(string dirName);

        /// <summary>
        /// 得到指定文件/文件夹的相关信息
        /// 注意:这些信息只代表获得该信息时文件的情况,并不随着环境的变化而变化
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <returns>其相关信息</returns>
    public FileSystemInfo getFileInf(string fileName);
    }
}
