using System;
using System.IO;
using System.Text;

using NS_ValCommon;

namespace NS_Glyph
{
    public class GErrFile 
    {
        // members
        private FileInfo infoFile;

        // constructors
        private GErrFile()
        {
        }

        public GErrFile(string nameFile)
        {
            this.infoFile=new FileInfo(nameFile);
            if (this.infoFile.Exists)
            {
                this.infoFile.Delete();
            }
            this.infoFile=new FileInfo(nameFile);
            StreamWriter writer=this.infoFile.AppendText();
            writer.WriteLine("");
            writer.Close();
        }

        // methods
        public void DIAFunc_WriteToFile(ValInfoBasic info)
        {
            GErr gerr = info as GErr;
            // TODO: wrap

            if (gerr!=null)
            {
                string str=gerr.Write();
                StreamWriter writer=this.infoFile.AppendText();
                writer.WriteLine(str);
                writer.Close();
            }
        }
    }
}
