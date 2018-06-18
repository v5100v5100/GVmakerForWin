using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Script.Interpreter
{
    /// <summary>
    /// GVM的配置参数,用于指定GVM实例的各项参数,immutable
    /// </summary>
    public class GvmConfig
    {

        private int runtimeRamSize_, stringRamSize_, stackSize_, version_;

        /**
         * 默认的JLVM配置
         */
        public GvmConfig() : this(0x6000, 1024, 512, 0x10)
        {
            
        }

        /**
         * 构造函数
         * @param runtimeRamSize 运行内存大小
         * @param stringRamSize 字符堆内存大小
         * @param stackSize     栈大小
         * @param version GVM的版本
         */
        public GvmConfig(int runtimeRamSize, int stringRamSize, int stackSize, int version)
        {
            this.runtimeRamSize_ = runtimeRamSize;
            this.stringRamSize_ = stringRamSize;
            this.stackSize_ = stackSize;
            this.version_ = version;
        }

        public int stringRamSize()
        {
            return stringRamSize_;
        }

        public int stackSize()
        {
            return stackSize_;
        }

        public int runtimeRamSize()
        {
            return runtimeRamSize_;
        }

        /**
         * 得到GVM的版本
         * @return version
         */
        public int version()
        {
            return version_;
        }

        public override String toString()
        {
            return "[Version: 0x" + Convert.ToString(version_, 16) +
                    ",runtimeRamSize: " + runtimeRamSize_ +
                    ",stringRamSize: " + stringRamSize_ +
                    ", stackSize: " + stackSize_ + "]";
        }
    }
}
