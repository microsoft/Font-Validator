using System;
using System.Diagnostics;

using OTFontFile;
using OTFontFile.Rasterizer;
using System.IO;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_hdmx.
    /// </summary>
    public class val_hdmx : Table_hdmx, ITableValidate
    {
        /************************
         * constructors
         */
        
        
        public val_hdmx(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /************************
         * public methods
         */


        public bool Validate(Validator v, OTFontVal fontOwner)
        {
            bool bRet = true;

            ushort numGlyphs = GetNumGlyphs(fontOwner);


            if (v.PerformTest(T.hdmx_Version))
            {
                if (TableVersionNumber == 0)
                {
                    v.Pass(T.hdmx_Version, P.hdmx_P_Version, m_tag);
                }
                else
                {
                    v.Error(T.hdmx_Version, E.hdmx_E_Version, m_tag, TableVersionNumber.ToString());
                    bRet = false;
                }
            }

            bool bNumDeviceRecordsOk = true;
            if (v.PerformTest(T.hdmx_NumDeviceRecords))
            {
                if (NumberDeviceRecords >= 0)
                {
                    v.Pass(T.hdmx_NumDeviceRecords, P.hdmx_P_NumDeviceRecords, m_tag, NumberDeviceRecords.ToString());
                }
                else
                {
                    v.Error(T.hdmx_NumDeviceRecords, E.hdmx_E_NumDeviceRecords_neg, m_tag, NumberDeviceRecords.ToString());
                    bNumDeviceRecordsOk = false;
                    bRet = false;
                }
            }

            bool bSizeOk = true;
            if (v.PerformTest(T.hdmx_SizeofDeviceRecord))
            {

                if ((SizeofDeviceRecord & 3) != 0)
                {
                    v.Error(T.hdmx_SizeofDeviceRecord, E.hdmx_E_SizeofDeviceRecord_alignment, m_tag, SizeofDeviceRecord.ToString());
                    bSizeOk = false;
                    bRet = false;
                }

                uint CalculatedSizeofDeviceRecord = CalculateSizeofDeviceRecord(numGlyphs);

                if (SizeofDeviceRecord != CalculatedSizeofDeviceRecord)
                {
                    string s = "actual = " + SizeofDeviceRecord + ", calc = " + CalculatedSizeofDeviceRecord;
                    v.Error(T.hdmx_SizeofDeviceRecord, E.hdmx_E_SizeofDeviceRecord_numGlyphs, m_tag, s);
                    bSizeOk = false;
                    bRet = false;
                }

                if (bSizeOk)
                {
                    v.Pass(T.hdmx_SizeofDeviceRecord, P.hdmx_P_SizeofDeviceRecord, m_tag, SizeofDeviceRecord.ToString());
                }
            }

            bool bLengthOk = true;
            if (v.PerformTest(T.hdmx_TableLength))
            {
                if (bNumDeviceRecordsOk)
                {
                    uint CalculatedTableLength = 8 + (uint)NumberDeviceRecords * CalculateSizeofDeviceRecord(numGlyphs);
                    if (GetLength() == CalculatedTableLength)
                    {
                        v.Pass(T.hdmx_TableLength, P.hdmx_P_TableLength, m_tag);
                    }
                    else
                    {
                        string s = "actual: " + GetLength() + ", calc: " + CalculatedTableLength;
                        v.Error(T.hdmx_TableLength, E.hdmx_E_TableLength, m_tag, s);
                        bLengthOk = false;
                        bRet = false;
                    }
                }
                else
                {
                    v.Warning(T.hdmx_TableLength, W._TEST_W_OtherErrorsInTable, m_tag, "unable to validate table length");
                }
            }

            if (v.PerformTest(T.hdmx_DeviceRecordPadBytesZero))
            {
                if (bSizeOk && bLengthOk & bNumDeviceRecordsOk)
                {
                    uint unpaddedLength = (uint)numGlyphs + 2;
                    if ((unpaddedLength & 3) == 0)
                    {
                        v.Pass(T.hdmx_DeviceRecordPadBytesZero, P.hdmx_P_DeviceRecordPadBytes_none, m_tag);
                    }
                    else
                    {
                        bool bPadOk = true;
                        if (NumberDeviceRecords > 1)
                        {
                            for (uint i=0; i<NumberDeviceRecords; i++)
                            {
                                DeviceRecord dr = GetDeviceRecord(i, numGlyphs);
                                for (uint j=0; j<dr.GetNumPadBytes(); j++)
                                {
                                    if (dr.GetPadByte(j) != 0)
                                    {
                                        bPadOk = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (bPadOk)
                        {
                            v.Pass(T.hdmx_DeviceRecordPadBytesZero, P.hdmx_P_DeviceRecordPadBytes_zero, m_tag);
                        }
                        else
                        {
                            v.Error(T.hdmx_DeviceRecordPadBytesZero, E.hdmx_E_DeviceRecordPadBytes_nonzero, m_tag);
                            bRet = false;
                        }
                    }
                }
                else
                {
                    v.Warning(T.hdmx_DeviceRecordPadBytesZero, W._TEST_W_OtherErrorsInTable, m_tag, "unable to validate that device record padding bytes are zero");
                }
            }

            if (v.PerformTest(T.hdmx_SortedOrder))
            {
                if (bSizeOk && bLengthOk && bNumDeviceRecordsOk)
                {
                    bool bSortOk = true;
                    if (NumberDeviceRecords > 1)
                    {
                        DeviceRecord drCurr = GetDeviceRecord(0, numGlyphs);
                        DeviceRecord drNext = null;
                        for (uint i=1; i<NumberDeviceRecords; i++)
                        {
                            drNext = GetDeviceRecord(i, numGlyphs);
                            if (drCurr.PixelSize >= drNext.PixelSize)
                            {
                                bSortOk = false;
                                break;
                            }
                            drCurr = drNext;
                        }
                    }
                    if (bSortOk)
                    {
                        v.Pass(T.hdmx_SortedOrder, P.hdmx_P_SortedOrder, m_tag);
                    }
                    else
                    {
                        v.Error(T.hdmx_SortedOrder, E.hdmx_E_SortedOrder, m_tag);
                        bRet = false;
                    }
                }
                else
                {
                    v.Warning(T.hdmx_SortedOrder, W._TEST_W_OtherErrorsInTable, m_tag, "unable to validate that device records are in sorted order");
                }
            }

            if (v.PerformTest(T.hdmx_DuplicateDeviceRecords))
            {
                if (bSizeOk && bLengthOk && bNumDeviceRecordsOk)
                {
                    bool bNoDup = true;
                    if (NumberDeviceRecords > 1)
                    {
                        for (uint i=0; i<NumberDeviceRecords-1; i++)
                        {
                            DeviceRecord dr1 = GetDeviceRecord(i, numGlyphs);
                            for (uint j=i+1; j<NumberDeviceRecords; j++)
                            {
                                DeviceRecord dr2 = GetDeviceRecord(j, numGlyphs);
                                if (dr1.PixelSize == dr2.PixelSize)
                                {
                                    bNoDup = false;
                                    break;
                                }
                            }
                        }
                    }
                    if (bNoDup)
                    {
                        v.Pass(T.hdmx_DuplicateDeviceRecords, P.hdmx_P_DuplicateDeviceRecords, m_tag);
                    }
                    else
                    {
                        v.Error(T.hdmx_DuplicateDeviceRecords, E.hdmx_E_DuplicateDeviceRecords, m_tag);
                        bRet = false;
                    }
                }
                else
                {
                    v.Warning(T.hdmx_DuplicateDeviceRecords, W._TEST_W_OtherErrorsInTable, m_tag, "unable to validate that there are no duplicate device records");
                }
            }

            if (v.PerformTest(T.hdmx_Widths))
            {
                if (bSizeOk && bLengthOk && bNumDeviceRecordsOk)
                {
                    bool bWidthsOk = true;
                    RasterInterf.DevMetricsData dmd = null;
                    try
                    {
                        dmd = fontOwner.GetCalculatedDevMetrics();
                    }
                    catch (Exception e)
                    {
                        v.ApplicationError(T.VDMX_CompareToCalcData, E._Table_E_Exception, m_tag, e.Message);
                        bRet = false;
                    }

                    if (dmd != null)
                    {
                        for (uint i=0; i<NumberDeviceRecords; i++)
                        {
                            DeviceRecord dr = GetDeviceRecord(i, numGlyphs);

                            for (uint iGlyph=0; iGlyph<numGlyphs; iGlyph++)
                            {

                                if (dr.GetWidth(iGlyph) != dmd.hdmxData.Records[i].Widths[iGlyph])
                                {

                                    String sDetails = "rec " + i + ", PixelSize " + dr.PixelSize + ", glyph# " + iGlyph 
                                        + ", width = " + dr.GetWidth(iGlyph) + ", calc = " + dmd.hdmxData.Records[i].Widths[iGlyph];
                                    v.Error(T.hdmx_Widths, E.hdmx_E_Widths, m_tag, sDetails);
                                    bWidthsOk = false;
                                    bRet = false;
                                }
                            }

                        }

                        if (bWidthsOk)
                        {
                            v.Pass(T.hdmx_Widths, P.hdmx_P_Widths, m_tag);
                        }
                    }
                    else
                    {
                        // if user didn't cancel, then check for error message
                        if (!v.CancelFlag)
                        {
                            String sDetails = null;
                            try
                            {
                                sDetails = fontOwner.GetDevMetricsDataError();
                            }
                            catch (Exception e)
                            {
                                v.ApplicationError(T.VDMX_CompareToCalcData, E._Table_E_Exception, m_tag, e.Message);
                            }
                            Debug.Assert(sDetails != null);
                            v.Error(T.hdmx_Widths, E.hdmx_E_Rasterizer, m_tag, sDetails);
                            bRet = false;
                        }                    
                    }

                }
                else
                {
                    v.Warning(T.hdmx_Widths, W._TEST_W_OtherErrorsInTable, m_tag, "unable to validate that the widths are correct");
                }
            }

            return bRet;
        }

        protected uint CalculateSizeofDeviceRecord(ushort numGlyphs)
        {
            uint n = 2 + (uint)numGlyphs;
            if ((n & 3) != 0)
            {
                n += 4 - n % 4;
            }

            return n;
        }

    }
}
