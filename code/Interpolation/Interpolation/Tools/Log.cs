using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Interpolation.Tools
{
    class Log
    {
        public void write(string msg)
        {
            msg = DateTime.Now.ToString() + ":" + msg+"\n";
            FileStream fs = null;
            string filename = "InterpolationLog";
            try
            {
                fs = File.OpenWrite(filename);

                fs.Position = fs.Length;

                byte[] data = System.Text.Encoding.Default.GetBytes(msg);

                fs.Write(data, 0, data.Length);
            }
            catch (Exception e)
            {
                System.Console.WriteLine(DateTime.Now.ToString() + ":" + e.Message);
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
