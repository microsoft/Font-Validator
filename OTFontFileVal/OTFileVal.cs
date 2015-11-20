using System;
using System.Diagnostics;
using System.IO;

using Microsoft.Win32.SafeHandles;

using OTFontFile.Rasterizer;

using NS_ValCommon;

using OTFontFile;

namespace OTFontFileVal
{
    public class OTFileVal : OTFile
    {
        /***************
         * constructors
         */


        public OTFileVal(Validator v)
        {
            if (v == null)
            {
                throw new ArgumentNullException();
            }

            m_TableManager = new TableManagerVal(this);
            m_Validator = v;
        }

        /*****************
        * private methods
        */

        private void SetValInfo()
        {

            //m_Validator.Info("_GEN_I_OpeningFile", null, sFilename);

            switch (m_FontFileType)
            {
                case FontFileType.INVALID:
                    m_Validator.Error(E._GEN_E_InvalidFontFile, null);
                    break;

                case FontFileType.SINGLE:
                    m_Validator.Info(I._GEN_I_Single, null);
                    break;

                case FontFileType.COLLECTION:
                    //m_Validator.Info(I._GEN_I_Collection, null);
                    m_Validator.Info(T.T_NULL, I._GEN_I_Collection, null, "TTC Header Version (major number): " + (m_ttch.version >> 16) + ".0");
                    break;

                default:
                    m_Validator.Error(E._GEN_E_InvalidFontFile, null);
                    break;
            }

        }

        /*****************
        * protected methods
        */

        /*****************
         * public methods
         */

        //meant to avoid code repetition (is called from OTFontVal too)
        public bool ValidateTable(OTTable table, Validator v, DirectoryEntry de, OTFontVal fontOwner)
        {
            String tname = GetTableManager().GetUnaliasedTableName(de.tag);
            bool bRet = true;

            // verify the checksum value from the directory entry matches the checksum for the table
            if (!(tname == "DSIG" && IsCollection()))
            {
                uint calcChecksum = 0;
                if (table != null)
                {
                    calcChecksum = table.CalcChecksum();
                }

                if (de.checkSum != calcChecksum)
                {
                    string s = "table '" + de.tag + "', calc: 0x" + calcChecksum.ToString("x8") + ", font: 0x" + de.checkSum.ToString("x8");
                    v.Error(T.T_NULL, E._DE_E_ChecksumError, de.tag, s);
                    bRet = false;
                }
            }

            // verify that table has pad bytes set to zero
            if (table != null)
            {
                uint nBytes = GetNumPadBytesAfterTable(table);

                bool bPadBytesZero = true;

                if (nBytes != 0)
                {
                    long PadFilePos = table.GetBuffer().GetFilePos() + table.GetBuffer().GetLength();
                    byte[] padbuf = ReadBytes(PadFilePos, nBytes);
                    for (int iByte = 0; iByte < padbuf.Length; iByte++)
                    {
                        if (padbuf[iByte] != 0)
                        {
                            bPadBytesZero = false;
                            break;
                        }
                    }

                }

                if (bPadBytesZero == false)
                {
                    v.Warning(T.T_NULL, W._DE_W_PadBytesNotZero, de.tag, "after " + de.tag + " table");
                }
            }

            // ask the table object to validate its data
            if (!(tname == "DSIG" && IsCollection())) v.OnTableValidationEvent(de, true);
            if (table != null)
            {
                if (v.TestTable(de.tag)) // don't test deselected tables
                {
                    try
                    {
                        ITableValidate valtable = (ITableValidate)table;
                        bRet &= valtable.Validate(v, fontOwner);
                    }
                    catch (InvalidCastException e)
                    {
                        v.ApplicationError(T.T_NULL, E._Table_E_Exception, table.m_tag, e.ToString());
                        bRet = false;
                    }
                }
                else
                {
                    v.Info(I._Table_I_NotSelected, de.tag);
                }
            }
            else
            {
                if (de.length == 0)
                {
                    // check if it's a known OT table type since zero length private tables seem allowable
                    if (TableManager.IsKnownOTTableType(de.tag))
                    {
                        v.Error(T.T_NULL, E._Table_E_Invalid, de.tag, "The directory entry length is zero");
                        bRet = false;
                    }
                }
                else if (de.offset == 0)
                {
                    v.Error(T.T_NULL, E._Table_E_Invalid, de.tag, "The directory entry offset is zero");
                    bRet = false;
                }
                else if (de.offset > GetFileLength())
                {
                    v.Error(T.T_NULL, E._Table_E_Invalid, de.tag, "The table offset points past end of file");
                    bRet = false;
                }
                else if (de.offset + de.length > GetFileLength())
                {
                    v.Error(T.T_NULL, E._Table_E_Invalid, de.tag, "The table extends past end of file");
                    bRet = false;
                }
                else
                {
                    v.Error(E._Table_E_Invalid, de.tag);
                    bRet = false;
                }
            }
            if (!(tname == "DSIG" && IsCollection())) v.OnTableValidationEvent(de, false);

            return bRet;

        }


        public void InitValidation(DIAction vid)
        {
            m_Validator.SetValInfoDelegate(vid);
        }

        public new bool open(string sFilename)
        {
            bool bRet = base.open(sFilename);

            SetValInfo();

            return bRet;
        }

        public new bool open(SafeFileHandle handle)
        {
            bool bRet = base.open(handle);

            SetValInfo();

            return bRet;
        }

        public Validator GetValidator()
        {
            return m_Validator;
        }

        public bool Validate()
        {
            long StartTicks = DateTime.Now.Ticks;
			bool bRet = true;

            m_Validator.OnFileValidationEvent(this, true);

            for (uint i=0; i<m_nFonts; i++)
            {
                // check to see if user canceled validation
                if (m_Validator.CancelFlag)
                {
                	bRet = false;
                    break;
                }

                m_Validator.OnFontValidationEvent(i, true);

                OTFontVal f = GetFont(i);

                if (f != null)
                {
                    m_Validator.OnFontParsedEvent(f);

                    if(!f.Validate()) bRet = false;
                }

                m_Validator.OnFontValidationEvent(i, false);
            }

            // build the elapsed time string
            int nSeconds = (int)((DateTime.Now.Ticks-StartTicks)/(double)10000000);
            int nHours = nSeconds / 3600;
            nSeconds = nSeconds - nHours*3600;
            int nMins = nSeconds / 60;
            nSeconds = nSeconds - nMins*60;
            string sTime = nHours.ToString() + ":" + nMins.ToString("d2") + ":" + nSeconds.ToString("d2");

            m_Validator.Info(T.T_NULL, I._GEN_I_TotalValTime, null, sTime);

            //Let's try to validate the DSIG table in a TTC, if it exists
            if (IsCollection())
            {
                if (m_ttch.version >= 0x00010000 && GetTableManager().GetUnaliasedTableName(m_ttch.DsigTag) == "DSIG")
                {
                    MBOBuffer buf = this.ReadPaddedBuffer(m_ttch.DsigOffset, m_ttch.DsigLength);
                    OTTable table = GetTableManager().CreateTableObject(m_ttch.DsigTag, buf);
                    DirectoryEntry de = new DirectoryEntry();
                    de.tag = m_ttch.DsigTag;
                    de.offset = m_ttch.DsigOffset;
                    de.length = m_ttch.DsigLength;

                    OTFontVal fontOwner = new OTFontVal(this);
                    bRet &= ValidateTable(table, m_Validator, de, fontOwner);
                }
                else
                {
                    m_Validator.Warning(T.T_NULL, W._FONT_W_MissingRecommendedTable, null, "DSIG");
                }
            }

            m_Validator.OnFileValidationEvent(this, false);

            if (m_Validator.CancelFlag)
            {
                m_Validator.ApplicationError(T.T_NULL, E._GEN_E_UserCanceled, null, "");
				bRet = false;
            }

			return bRet;

        }

        public void CancelValidation()
        {
            // tell the rasterizer to stop
            RasterInterf l_Rasterizer = RasterInterf.getInstance();
            l_Rasterizer.CancelCalcDevMetrics();
            l_Rasterizer.CancelRastTest();
        }

        
        public new TableManagerVal GetTableManager()
        {
            return (TableManagerVal) m_TableManager;
        }
        

        public new OTFontVal GetFont(uint i)
        {
            OTFontVal f = null;

            Debug.Assert(i < m_nFonts);
            Debug.Assert(m_FontFileType != FontFileType.INVALID);
            if (i > 0)
            {
                Debug.Assert(m_FontFileType == FontFileType.COLLECTION);
            }

            if (i < m_nFonts)
            {
                if (m_FontFileType == FontFileType.SINGLE)
                {
                    f = OTFontVal.ReadFont(this, 0, 0);
                }
                else if (m_FontFileType == FontFileType.COLLECTION)
                {
                    Debug.Assert(m_ttch != null);
                    if (m_ttch != null)
                    {
                        uint nDirOffset = (uint)m_ttch.DirectoryOffsets[(int)i];

                        f = OTFontVal.ReadFont(this, i, nDirOffset);
                    }
                }
            }

            return f;
        }

        public    RasterInterf GetRasterizer()
        {
            return RasterInterf.getInstance();
        }


        public new MBOBuffer ReadPaddedBuffer(uint filepos, uint length)
        {
            // allocate a buffer to hold the table
            MBOBuffer buf = new MBOBuffer(filepos, length);

            // read the table
            m_fs.Seek(filepos, SeekOrigin.Begin);
            int nBytes = m_fs.Read(buf.GetBuffer(), 0, (int)length);
            if (nBytes != length)
            {
                // check for premature EOF
                if (m_fs.Position == m_fs.Length)
                {
                    m_Validator.Error(E._GEN_E_EOFError, null);
                }
                else
                {
                    m_Validator.Error(E._GEN_E_ReadError, null);
                }

                buf = null;
            }

            return buf;
        }

        /**************
         * member data
         */


        Validator m_Validator;
    }

}
