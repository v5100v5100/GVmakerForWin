using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;


namespace Script.Interpreter
{
    /// <summary>
    /// 
    /// </summary>
    public class DefaultLavApp : LavApp
    {
        //public DefaultLavApp(InputStream in) {
        //    super(getDataByInputStream(in));
        //}

        public DefaultLavApp(FileStream fileStream): base(getDataByInputStream(fileStream)){

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileStream"></param>
        /// <returns></returns>
        private static byte[] getDataByInputStream(FileStream fileStream) {
            //ByteArrayOutputStream bos = new ByteArrayOutputStream();
            MemoryStream bos = new MemoryStream();
            byte[] tmpBuffer = new byte[512];
            try {
                int length;
                //while ((length = fileStream.read(tmpBuffer)) != -1) {
                //    bos.write(tmpBuffer, 0, length);
                //}
                while ((length = fileStream.Read(tmpBuffer, 0, 512)) != -1)
                {
                    bos.Write(tmpBuffer, 0, length);
                }
            } catch (IOException ex) {
                //throw new IllegalArgumentException(ex.getMessage());
            } finally {
                try {
                    fileStream.Close();
                } catch (IOException ex) {
                //do nothing
                }
            }
            //return bos.toByteArray();
            return bos.ToArray();
        }
    }
}
