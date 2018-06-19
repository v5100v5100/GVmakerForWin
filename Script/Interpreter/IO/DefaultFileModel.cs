using Script.Interpreter.Ram;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Script.Interpreter.IO
{
    /// <summary>
    /// 文件操作模型的默认实现,该实现通过FileSystem接口得到本地文件的输入与输出流,并在内存中模拟文件的各种操作
    /// </summary>
    public class DefaultFileModel : FileModel
    {
        public const string READ_MODE = "r";
        public const string READ_PLUS_MODE = "r+";
        public const string READ_B_MODE = "rb";
        public const string READ_B_PLUS_MODE = "rb+";
        public const string WRITE_MODE = "w";
        public const string WRITE_PLUS_MODE = "w+";
        public const string WRITE_B_MODE = "wb";
        public const string WRITE_B_PLUS_MODE = "wb+";
        public const string APPEND_MODE = "a";
        public const string APPEND_PLUS_MODE = "a+";
        public const string APPEND_B_MODE = "ab";
        public const string APPEND_B_PLUS_MODE = "ab+";
        private const int SEEK_SET = 0;
        private const int SEEK_CUR = 1;
        private const int SEEK_END = 2;

        //同时访问文件的最大个数
        private readonly static int MAX_FILE_COUNT = 3;
        private FileSystem fileSys;
        private string workDir;
        private FileSystemInfo workDirInf;
        private bool[] canRead;
        private bool[] canWrite;
        private string[] fileNames;
        //是否可用,也就是是否空闲
        private bool[] usable;
        private VirtualFile[] files;
        //用于生成string的byte数组
        private sbyte[] strBuf;

        public DefaultFileModel(FileSystem fileSys)
        {
            this.fileSys = fileSys;
            workDir = "/";
            workDirInf = fileSys.getFileInf(workDir);
            canRead = new bool[MAX_FILE_COUNT];
            canWrite = new bool[MAX_FILE_COUNT];
            usable = new bool[MAX_FILE_COUNT];
            files = new VirtualFile[MAX_FILE_COUNT];
            fileNames = new string[MAX_FILE_COUNT];

            for (int index = 0; index < MAX_FILE_COUNT; index++) 
            {
                usable[index] = true;
                //初始容量:64K
                files[index] = new VirtualFile(0x10000);
            }
            strBuf = new sbyte[400];
        }

        public bool changeDir(Getable source, int addr) 
        {
            int pre = -2;
            int length = 0;
            sbyte b = 0;
            while ((b = source.getByte(addr)) != 0)
            {
                if (b == '/') {
                    if (pre != addr - 1) {
                        strBuf[length++] = b;
                    }
                    pre = addr;
                    addr++;
                }
                else {
                    strBuf[length++] = b;
                    addr++;
                }
            }
            string newDir = null;
            try {
                //newDir = new string(strBuf, 0, length, "gb2312");
                newDir = Util.NewString(strBuf,0,length,Encoding.GetEncoding("gb2312"));
            } catch (Exception uee) {
                newDir = Util.NewString(strBuf,0,length);
            }
            if (newDir == "..")
            {
                if (workDir == "/" )
                {
                    return false;
                }
                int pos = workDir.LastIndexOf('/', workDir.Length - 2) + 1;
                workDir = workDir.Substring(0, pos);
                workDirInf = fileSys.getFileInf(workDir);
                return true;
            }
            else
            {
                if (!newDir.StartsWith("/")) {
                    newDir = workDir + newDir;
                }
                if (!newDir.EndsWith("/")) {
                    newDir += "/";
                }
                if (workDir == newDir) {
                    return true;
                }
                FileSystemInfo inf = fileSys.getFileInf(newDir);
                if (inf.isDirectory()) {
                    workDir = newDir;
                    workDirInf = inf;
                    return true;
                }
                else {
                    return false;
                }
            }
        }

        public bool makeDir(Getable source, int addr)
        {
            string dir = getFileName(source, addr);
            bool result = fileSys.makeDir(dir);
            if (result && isParent(workDir, dir))
            {
                workDirInf = fileSys.getFileInf(workDir);
            }
            return result;
        }

        /// <summary>
        /// 得到当前目录下的文件个数
        /// </summary>
        /// <returns>文件的数量</returns>
        public int getFileNum() 
        {
            return workDirInf.getFileNum();
        }

        /**
         * 得到当前目录下第start个开始的num个文件名,保存到names中
         * @param names 用于保存文件名的string数组
         * @param start 开始文件号
         * @param num   个数
         * @return      实际得到的个数,如出错,返回-1
         */
        public int listFiles(string[] names, int start, int num) 
        {
            return workDirInf.listFiles(names, start, num);
        }

        public int fopen(Getable source, int fileName, int openMode) 
        {
            int num = -1;
            //指示文件指针位置,true开头,false为结尾
            bool pointer = true;
            //是否清除原有文件
            bool clear = false;
            for (int index = 0; index < MAX_FILE_COUNT; index++) {
                if (usable[index]) {
                    num = index;
                    break;
                }
            }
            if (num == -1) {
                return 0;
            }
            string name = getFileName(source, fileName);
            string mode = getstring(source, openMode);
            FileSystemInfo inf = fileSys.getFileInf(name);

            if (READ_MODE == mode || READ_B_MODE == mode) 
            {
                if (!(inf.isFile() && inf.canRead())) {
                    return 0;
                }
                canRead[num] = true;
                canWrite[num] = false;
            }
            else if (READ_PLUS_MODE == mode || READ_B_PLUS_MODE == mode)
            {
                if (!(inf.isFile() && inf.canRead() && inf.canWrite())) {
                    return 0;
                }
                canRead[num] = true;
                canWrite[num] = true;
            }
            else if (WRITE_MODE == mode || WRITE_B_MODE == mode)
            {
                if (inf.isFile() && !inf.canWrite()) {
                    return 0;
                }
                clear = true;
                canRead[num] = false;
                canWrite[num] = true;
            }
            else if (WRITE_PLUS_MODE == mode || WRITE_B_PLUS_MODE == mode )
            {
                if (inf.isFile() && !inf.canWrite()) {
                    return 0;
                }
                clear = true;
                canRead[num] = true;
                canWrite[num] = true;
            }
            else if (APPEND_MODE == mode || APPEND_B_MODE == mode) 
            {
                if (!(inf.isFile() && inf.canWrite())) {
                    return 0;
                }
                canRead[num] = false;
                canWrite[num] = true;
                pointer = false;
            }
            else if (APPEND_PLUS_MODE == mode || APPEND_B_PLUS_MODE == mode)
            {
                if (!(inf.isFile() && inf.canRead() && inf.canWrite())) {
                    return 0;
                }            canRead[num] = true;
                canWrite[num] = true;
                pointer = false;
            }
            else {
                return 0;
            }
            VirtualFile file = files[num];
            if (clear) 
            {
                file.refresh();
            }
            else {
                int length = 0;
                try {
                    //InputStream in = fileSys.getInputStream(name);
                    FileStream inputStream = fileSys.getInputStream(name);
                    //file.readFromStream(in);
                    file.readFromStream(inputStream);
                    length = file.limit();
                    //in.close();
                    inputStream.Close();
                } catch (Exception ex) {
                    return 0;
                }
                file.position(pointer ? 0 : length);
            }
            fileNames[num] = name;
            usable[num] = false;
            return num | 0x80;
        }

        public void fclose(int fp)
        {
            if ((fp & 0x80) == 0)
            {
                return;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT) {
                return;
            }
            if (usable[fp]) {
                return;
            }
            if (canWrite[fp])
            {
                try 
                {
                    //OutputStream out = fileSys.getOutputStream(fileNames[fp]);
                    FileStream outputStream = fileSys.getOutputStream(fileNames[fp]);
                    //files[fp].writeToStream(out);
                    files[fp].writeToStream(outputStream);
                    if (isParent(workDir, fileNames[fp])) {
                        workDirInf = fileSys.getFileInf(workDir);
                    }
                } catch (Exception e)
                {

                }
            }
            usable[fp] = true;
        }

        public int getc(int fp)
        {
            if ((fp & 0x80) == 0)
            {
                return -1;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT)
            {
                return -1;
            }
            if (usable[fp] || !canRead[fp])
            {
                return -1;
            }
            return files[fp].getc();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="c"></param>
        /// <param name="fp"></param>
        /// <returns></returns>
        public int putc(int c, int fp)
        {
            if ((fp & 0x80) == 0) {
                return -1;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT) {
                return -1;
            }
            if (usable[fp] || !canWrite[fp]) {
                return -1;
            }
            return files[fp].putc(c);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dest"></param>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <param name="fp"></param>
        /// <returns></returns>
        public int fread(Setable dest, int addr, int size, int fp)
        {
            if ((fp & 0x80) == 0)
            {
                return 0;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT) 
            {
                return 0;
            }
            if (usable[fp] || !canRead[fp])
            {
                return 0;
            }
            VirtualFile file = files[fp];
            int count = 0, b;
            while (count < size && (b = file.getc()) != -1) 
            {
                dest.setByte(addr++, (sbyte) b);
                count++;
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        /// <param name="size"></param>
        /// <param name="fp"></param>
        /// <returns></returns>
        public int fwrite(Getable source, int addr, int size, int fp)
        {
            if ((fp & 0x80) == 0) {
                return 0;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT) {
                return 0;
            }
            if (usable[fp] || !canWrite[fp]) {
                return 0;
            }
            VirtualFile file = files[fp];
            int count = 0, b;
            while (count < size) {
                b = source.getByte(addr++);
                if (file.putc(b & 0xff) == -1) {
                    break;
                }
                count++;
            }
            return count;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <param name="addr"></param>
        /// <returns></returns>
        public bool deleteFile(Getable source, int addr)
        {
            string file = getFileName(source, addr);
            bool result = fileSys.deleteFile(file);
            //如果当前目录信息被修改,重置之
            if (result && isParent(workDir, file)) {
                workDirInf = fileSys.getFileInf(workDir);
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fp"></param>
        /// <param name="offset"></param>
        /// <param name="base_"></param>
        /// <returns></returns>
        public int fseek(int fp, int offset, int base_) 
        {
            if ((fp & 0x80) == 0) {
                return -1;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT || usable[fp]) {
                return -1;
            }
            VirtualFile file = files[fp];
            int pos = 0;
            switch (base_)
            {
                case SEEK_SET:
                    pos = offset;
                    break;
                case SEEK_CUR:
                    pos = file.position() + offset;
                    break;
                case SEEK_END:
                    pos = file.limit() + offset;
                    break;
                default:
                    return -1;
            }
            return file.position(pos);
        }

        public int ftell(int fp)
        {
            if ((fp & 0x80) == 0)
            {
                return -1;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT || usable[fp]) 
            {
                return -1;
            }
            return files[fp].position();
        }

        public bool feof(int fp) 
        {
            if ((fp & 0x80) == 0) 
            {
                return true;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT || usable[fp]) 
            {
                return true;
            }
            return files[fp].position() == files[fp].limit();
        }

        public void rewind(int fp)
        {
            if ((fp & 0x80) == 0)
            {
                return;
            }
            fp &= 0x7f;
            if (fp >= MAX_FILE_COUNT || usable[fp])
            {
                return;
            }
            files[fp].position(0);
        }

        public void dispose() 
        {
            for (int index = 0; index < MAX_FILE_COUNT; index++)
            {
                fclose(index | 0x80);
            }
        }

        /**
         * 判断dir是否是file的父目录
         */
        private bool isParent(string dir, string file)
        {
            if (file.StartsWith(dir))
            {
                int pos = file.IndexOf('/', dir.Length);
                return pos == -1 || pos == file.Length - 1;
            }
            return false;
        }

        private string getFileName(Getable src, int addr) 
        {
            string name = getstring(src, addr);
            if (!name.StartsWith("/")) {
                name = workDir + name;
            }
            return name;
        }

        private string getstring(Getable src, int addr) 
        {
            int length = 0;
            sbyte b;
            while ((b = src.getByte(addr++)) != 0)
            {
                strBuf[length++] = b;
            }
            //try
            //{
            //    return new string(strBuf, 0, length, "gb2312");
            //}
            //catch (UnsupportedEncodingException uee)
            //{
            //    return new string(strBuf, 0, length);
            //}
            return Util.NewString(strBuf, 0, length, Encoding.GetEncoding("gb2312"));
        }
    }
}
