using System;
using System.Windows.Forms;

namespace Win32APIs
{
    public class MultiByte
    {
        public static uint GetCodePageMaxCharSize (uint CodePage)
        {
            throw new NotImplementedException("UnImplemented CodePage:" + CodePage + " MaxChar");
        }
        
        public static bool IsCodePageInstalled (uint CodePage)
        {
            //Lie about having every CodePage?
            throw new NotImplementedException("UnImplemented CodePage:" + CodePage);
        }
        
        public static bool IsCodePageLeadByte (uint CodePage, byte c)
        {
            throw new NotImplementedException("UnImplemented Win32API.MultiByte/IsCodePageLeadByte: "
                                              + CodePage + ", byte " + c);
        }
        public static int MultiByteCharToUnicodeChar (uint CodePage, ushort c)
        {
            throw new NotImplementedException("UnImplemented Win32API.MultiByte/MultiByteCharToUnicodeChar: "
                                              + CodePage + ", char " + c);
        }
    }
    public class SH
    {
        public static string BrowseForFolder ()
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.Description =
                "Select the directory to save XML font reports to";

            folderBrowserDialog.ShowNewFolderButton = true;

            // default to user's home, allow navigate to everywhere else
            folderBrowserDialog.SelectedPath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);
            folderBrowserDialog.RootFolder = Environment.SpecialFolder.MyComputer;

            DialogResult result = folderBrowserDialog.ShowDialog();
            if( result == DialogResult.OK )
            {
                return folderBrowserDialog.SelectedPath;
            } else {
                return null; //compat
            }
        }
    }
}
