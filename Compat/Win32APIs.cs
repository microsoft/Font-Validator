// Copyright (c) Hin-Tak Leung

// All rights reserved.

// MIT License

// Permission is hereby granted, free of charge, to any person obtaining a copy of
// this software and associated documentation files (the ""Software""), to deal in
// the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies
// of the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:

// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.

// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.

using System;
using System.Windows.Forms;

using Compat.Win32APIs;

namespace Win32APIs
{
    public class MultiByte
    {
        public static uint GetCodePageMaxCharSize (uint CodePage)
        {
            // throws 'key not present' for unknown CodePage
            CPInfo cpInfo = new CPInfoTable()[CodePage];

            return cpInfo.MaxCharSize;
        }

        public static bool IsCodePageInstalled (uint CodePage)
        {
            // throws 'key not present' for unknown CodePage
            CPInfo cpInfo = new CPInfoTable()[CodePage];

            return true;
        }

        public static bool IsCodePageLeadByte (uint CodePage, byte c)
        {
            return new CPInfoTable()[CodePage, c];
        }

        public static int MultiByteCharToUnicodeChar (uint CodePage, ushort c)
        {
            return new CPInfoTable()[CodePage, c];
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
