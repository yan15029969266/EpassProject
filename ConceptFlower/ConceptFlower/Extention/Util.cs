using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConceptFlower.Extention
{
    public class Util
    {
        public static void Base64SaveFlie(string strbase64,string filePath)
        {

            try
            {
                if (!Directory.Exists(Path.GetDirectoryName(filePath)))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                }
                if(File.Exists(filePath))
                {
                    File.Delete(filePath);
                }
                byte[] arr = Convert.FromBase64String(strbase64);
                MemoryStream ms = new MemoryStream(arr);
                //string file = @"d:\test\test.jep";
                //将下载下来的文件放在当前目录下，保存为e.wav；当然如果是图片，可以保存为a.jpg
                FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate);
                ms.WriteTo(fs);
                ms.Close();
                fs.Close();
                ms = null;
                fs = null;
                //ms.Close();
            }
            catch (Exception ex)
            {
            }
        }
    }
}
