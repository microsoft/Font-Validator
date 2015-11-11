using System;
using System.Diagnostics;

using OTFontFile;

namespace OTFontFileVal
{
    /// <summary>
    /// Summary description for val_loca.
    /// </summary>
    public class val_loca : Table_loca, ITableValidate
    {

        public val_loca(OTTag tag, MBOBuffer buf) : base(tag, buf)
        {
        }


        /*
         *        VALIDATION FUNCTIONS
         */

        
        public bool ValidateFormat(Validator validator, 
            OTFont fontOwner)
        {
            int format=this.Format(fontOwner);
            if ((format!=0)&&(format!=1))
            {
                if (validator!=null)
                {
                    validator.Error(E.loca_E_Format, m_tag);
                }
                return false;
            }
            else
            {
                if (validator!=null)
                {
                    validator.Pass(P.loca_P_Format, m_tag);
                }
                return true;
            }
        }

        public bool ValidateNumEntries(Validator validator,
            OTFont fontOwner)
        {
            int numGlyph=this.NumGlyph(fontOwner);
            int numEntry=this.NumEntry(fontOwner);
            if ((numGlyph==Table_loca.ValueInvalid)||
                (numEntry==Table_loca.ValueInvalid)||
                (numEntry!=numGlyph+1))
            {
                if (validator!=null)
                {
                    validator.Error(T.T_NULL,
                        E.loca_E_NumEntries, 
                        m_tag, 
                        "number of entries="+numEntry+" number of glyphs="+numGlyph);
                }
                return false;    
            }
            else
            {
                if (validator!=null)
                {
                    validator.Pass(P.loca_P_NumEntries, m_tag);
                }
                return true;
            }
        }

        private bool ValidateOffsetWithinGlyfRange(Validator validator,
            OTFont fontOwner)
        {
            int cntErr=0;
            int lengthGlyf=this.LengthGlyf(fontOwner);
            if (lengthGlyf==Table_loca.ValueInvalid)
            {
                if (validator!=null)
                {
                    validator.Error(T.T_NULL, E.loca_E_OffsetWithinGlyfRange, 
                        m_tag,
                        "Unable to determine the length of the 'glyf' table");
                }
                return false;
            }
            
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
            {
                if (validator!=null)
                {
                    validator.Error(T.T_NULL, E.loca_E_OffsetWithinGlyfRange, 
                        m_tag,
                        "Unable to determine the number of entries in the 'loca' table");
                }
                return false;
            }
            int offsCur;
            for (int iEntry=0; iEntry<numEntry; iEntry++)
            {
                GetGlyfOffset(iEntry, out offsCur, fontOwner);
                if (iEntry!=numEntry-1)
                {
                    int offsNext;
                    GetGlyfOffset(iEntry+1, out offsNext, fontOwner);
                    int length = offsNext - offsCur;
                    if ((offsCur<0)||(offsCur>=lengthGlyf && length!=0))
                    {
                        validator.Error(T.T_NULL, E.loca_E_OffsetWithinGlyfRange, 
                            m_tag, "offset[" + iEntry + "] = " + (uint)offsCur);
                        cntErr++;
                    }
                }
                else
                {
                    if ((offsCur<0)||(offsCur>lengthGlyf))
                    {
                        validator.Error(T.T_NULL, E.loca_E_OffsetWithinGlyfRange, 
                            m_tag, "offset[" + iEntry + "] = " + (uint)offsCur);
                        cntErr++;
                    }
                }
            }
            if (validator!=null)
            {
                if (cntErr==0)
                {
                    validator.Pass(P.loca_P_OffsetWithinGlyfRange, m_tag);
                }
            }
            return (cntErr==0);
        }

        private bool ValidateOffsetsIncreasing(Validator validator,
            OTFont fontOwner)
        {
            int cntErr=0;
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
            {
                if (validator!=null)
                {
                    validator.Error(T.T_NULL, E.loca_E_OffsetsIncreasing, 
                        m_tag,
                        "Unable to determine the number of entries in the 'loca' table");
                }
                return false;
            }
            
            int offsGlyfCur, offsGlyfNext;            
            for (int iEntry=0; iEntry<numEntry-1; iEntry++)
            {
                if ((!this.GetGlyfOffset(iEntry, out offsGlyfCur, validator, fontOwner))||
                    (!this.GetGlyfOffset(iEntry+1, out offsGlyfNext, validator, fontOwner)))
                {
                    cntErr++;
                    continue;
                }
                if (offsGlyfNext<offsGlyfCur)
                {
                    cntErr++;
                }
            }
            if (validator!=null)
            {
                if (cntErr==0)
                {
                    validator.Pass(P.loca_P_OffsetsIncreasing, m_tag);
                }
                else
                {
                    validator.Error(T.T_NULL, E.loca_E_OffsetsIncreasing, m_tag,
                        "Number of glyphs with the error = "+cntErr);
                }
            }
            return (cntErr==0);
        }

        private bool ValidateGlyfEntryLengthAlignment(Validator validator,
            OTFont fontOwner)
        {
            int cntWarn=0;
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
            {
                if (validator!=null)
                {
                    validator.Warning(T.T_NULL, W.loca_W_GlyfEntryLengthAlignment, 
                        m_tag,
                        "Unable to determine the number of entries in the 'loca' table");
                }
                return false;
            }
            
            int offsGlyfCur, offsGlyfNext;            
            for (int iEntry=0; iEntry<numEntry-1; iEntry++)
            {
                if ((!this.GetGlyfOffset(iEntry, out offsGlyfCur, validator, fontOwner))||
                    (!this.GetGlyfOffset(iEntry+1, out offsGlyfNext, validator, fontOwner)))
                {
                    cntWarn++;
                    continue;
                }
                if (((offsGlyfNext-offsGlyfCur)%4)!=0)
                {
                    cntWarn++;
                }
            }
            if (validator!=null)
            {
                if (cntWarn==0)
                {
                    validator.Pass(P.loca_P_GlyfEntryLengthAlignment, m_tag);
                }
                else
                {
                    validator.Warning(T.T_NULL, W.loca_W_GlyfEntryLengthAlignment, 
                        m_tag,
                        "Number of glyphs with the warning = "+cntWarn);
                }
            }
            return (cntWarn==0);
        }
    
        private bool ValidateGlyfEntryEmpty(Validator validator,
            OTFont fontOwner)
        {
            int cntInfo=0;
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
            {
                if (validator!=null)
                {
                    validator.Warning(T.T_NULL, W._TEST_W_OtherErrorsInTable, 
                        m_tag,
                        "Unable to determine the number of entries in the 'loca' table");
                }
                return false;
            }
            
            int offsGlyfCur, offsGlyfNext;            
            for (int iEntry=0; iEntry<numEntry-1; iEntry++)
            {
                if ((!this.GetGlyfOffset(iEntry, out offsGlyfCur, validator, fontOwner))||
                    (!this.GetGlyfOffset(iEntry+1, out offsGlyfNext, validator, fontOwner)))
                {
                    cntInfo++;
                    continue;
                }
                if (offsGlyfNext==offsGlyfCur)
                {
                    cntInfo++;
                }
            }
            if (validator!=null)
            {
                if (cntInfo==0)
                {
                    validator.Pass(P.loca_P_GlyfEntryEmpty, m_tag);
                }
                else
                {
                    validator.Info(T.T_NULL, I.loca_I_GlyfEntryEmpty, 
                        m_tag,
                        "Number of glyphs that are empty = "+cntInfo);
                }
            }
            return (cntInfo==0);
        }


        private bool ValidateGlyfPartiallyUnreferenced(Validator validator,
            OTFont fontOwner)
        {
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
            {
                validator.Warning(T.T_NULL, W.loca_W_GlyfPartiallyUnreferenced,this.m_tag);
                return false;
            }
            int offsLast;
            if (!this.GetGlyfOffset(numEntry-1,out offsLast,validator,fontOwner))
            {
                validator.Warning(T.T_NULL, W.loca_W_GlyfPartiallyUnreferenced,this.m_tag);
                return false;
            }
            int lengthGlyf=this.LengthGlyf(fontOwner);
            if (lengthGlyf==Table_loca.ValueInvalid)
            {
                validator.Warning(T.T_NULL, W.loca_W_GlyfPartiallyUnreferenced,this.m_tag);
                return false;
            }
            if (lengthGlyf!=offsLast)
            {
                validator.Warning(T.T_NULL, W.loca_W_GlyfPartiallyUnreferenced,this.m_tag);
                return false;
            }
            validator.Pass(P.loca_P_GlyfPartiallyUnreferenced,this.m_tag);
            return true;
        }

        public bool Validate(Validator validator, OTFontVal fontOwner)
        {            
            if (validator==null)
                return false;

            if (validator.PerformTest(T.loca_Format))
            {
                this.ValidateFormat(validator,fontOwner);
            }
            if (validator.PerformTest(T.loca_NumEntries))
            {
                this.ValidateNumEntries(validator,fontOwner);
            }
            if (validator.PerformTest(T.loca_OffsetsIncreasing))
            {
                this.ValidateOffsetsIncreasing(validator,fontOwner);
            }
            if (validator.PerformTest(T.loca_OffsetWithinGlyfRange))
            {
                this.ValidateOffsetWithinGlyfRange(validator,fontOwner);
            }
            if (validator.PerformTest(T.loca_GlyfEntryLengthAlignment))
            {
                this.ValidateGlyfEntryLengthAlignment(validator,fontOwner);
            }
            if (validator.PerformTest(T.loca_GlyfEntryEmpty))
            {
                this.ValidateGlyfEntryEmpty(validator,fontOwner);
            }
            if (validator.PerformTest(T.loca_GlyfPartiallyUnreferenced))
            {
                this.ValidateGlyfPartiallyUnreferenced(validator,fontOwner);
            }
            return true;
        }

        /*
         *        METHODS PRIVATE: GLYF ACCESS
         */

        private bool GetGlyfOffset(int indexGlyph, out int offsGlyf, 
            Validator validator, OTFont fontOwner)
        {
            offsGlyf=Table_loca.ValueInvalid;
            int numEntry=this.NumEntry(fontOwner);
            if (numEntry==Table_loca.ValueInvalid)
                return false;
            if ((indexGlyph<0)||(indexGlyph>=numEntry))
            {
                //Debug.Assert(false, "Table_loca: GetGlyfOffset");
                return false;
            }

            int sizeEntry=this.SizeEntry(fontOwner);
            if (sizeEntry==Table_loca.ValueInvalid)
                return false;
            int pozLoca=sizeEntry*indexGlyph;
            if (pozLoca+sizeEntry>this.GetLength())
            {
                if (validator!=null)
                {
                    validator.Error(E._GEN_E_OffsetExceedsTableLength, "loca");    
                }
                return false;
            }
            if (sizeEntry==2)
            {
                offsGlyf = 2*m_bufTable.GetUshort((uint)pozLoca);
            }
            else
            {
                offsGlyf = (int)(m_bufTable.GetUint((uint)pozLoca));
            }
            return true;
        }

        /*
         *        METHODS PUBLIC: GLYF ACCESS
         */        
        public bool GetValidateEntryGlyf(int indexGlyph, 
            out int offsStart, out int length,
            Validator validator, OTFont fontOwner)
        {
            offsStart=Table_loca.ValueInvalid;
            length=Table_loca.ValueInvalid;

            int offsGlyfCur, offsGlyfNext; 
            if  ((!this.GetGlyfOffset(indexGlyph,out offsGlyfCur,validator, fontOwner))||
                (!this.GetGlyfOffset(indexGlyph+1,out offsGlyfNext,validator, fontOwner)))
            {
                return false; // the error is already reported
            }

            int lengthGlyf=this.LengthGlyf(fontOwner);
            if (lengthGlyf==Table_loca.ValueInvalid)
            {
                return false;
            }

            if ((offsGlyfCur<0)||(offsGlyfCur>=lengthGlyf))
            {
                if (validator!=null)
                {
                    validator.Error(E.loca_E_OffsetWithinGlyfRange,"loca"); 
                }
                return false;
            }
            if ((offsGlyfNext<0)||(offsGlyfNext>=lengthGlyf))
            {
                int numEntry=this.NumEntry(fontOwner);
                if ((indexGlyph!=numEntry-2)||(offsGlyfNext!=lengthGlyf))
                {
                    if (validator!=null)
                    {
                        validator.Error(T.T_NULL, E.loca_E_OffsetWithinGlyfRange,
                            (OTTag)"loca","index glyph="+indexGlyph+1); 
                    }
                }
            } 

            int lengthGlyfCur=offsGlyfNext-offsGlyfCur;
            if (lengthGlyfCur<0)
            {
                if (validator!=null)
                {
                    validator.Error(E.loca_E_OffsetsIncreasing,"loca");
                }
                return false;
            }
            if (lengthGlyfCur==0)
            {
                if (validator!=null)
                {
                    validator.Warning(T.T_NULL, W.loca_W_GlyfEntryEmpty,"loca");
                }
            }
            if (lengthGlyfCur%4!=0)
            {
                if (validator!=null)
                {
                    validator.Warning(T.T_NULL, W.loca_W_GlyfEntryLengthAlignment,"loca");
                }
            }
            offsStart=offsGlyfCur;
            length=lengthGlyfCur;
            return true;
        }
    }
}
        
