using System;
using System.Diagnostics;

using OTFontFile;

using NS_ValCommon;
using NS_Glyph;
using NS_GMath;


namespace OTFontFileVal
{
    public class I_IOGlyphsFile : I_IOGlyphs                     
    {
        
        // members
        private OTFont     m_font;
        private val_glyf   m_tableGlyf;
        private val_loca   m_tableLoca;
        private Validator  m_validator;
        private int        m_numGlyph;
        
        // CLOSE the font file ONLY if it was opened during initialization,
        // i.e. only if the font was initialized from the FILE NAME
        private bool        m_toCloseFileOnClear;

        /*
         *        CONSTRUCTORS
         */
        public I_IOGlyphsFile()
        {
            this.m_font=null;
            this.m_tableLoca=null;
            this.m_tableGlyf=null;
            this.m_validator=null;
            this.m_numGlyph=GConsts.IND_UNINITIALIZED;
            this.m_toCloseFileOnClear=false;
        }

        /*
         *        PROPERTIES
         */
        /*
        public OTFile FileFont
        {
            get {return this.font.GetFile();}
        }
        */

        /*
         *        METHODS: I_IOGLYPHS
         */

        public bool Initialize(Object source,
            DIAction dia)
        {
            /*
             *        ASSUMPTION: source is either 
             *                    -    OTFont or 
             *                    -    NameFileFont
             */
            this.m_validator=new Validator();
            this.m_validator.DIA=dia;
            this.m_font=source as OTFont;
            
            if (this.m_font==null)
            {
                string nameFileFont=source as string;
                if (nameFileFont!=null)
                {
                    Validator validatorDummy=new Validator();
                    OTFileVal file = new OTFileVal(validatorDummy);
                    if (!file.open(nameFileFont))
                    {
                        this.m_validator.Error(T.T_NULL, 
                            E.glyf_E_UnableToStartValidation,
                            (OTTag)"glyf",
                            "Unable to open font file "+nameFileFont);
                        this.Clear();
                        return false;
                    }
                    this.m_toCloseFileOnClear=true;
                    try
                    {
                        this.m_font=file.GetFont(0);
                    }
                    catch
                    {
                        this.m_validator.Error(T.T_NULL, 
                            E.glyf_E_UnableToStartValidation,
                            (OTTag)"glyf",
                            "Unable to get font from the file"+nameFileFont);
                        this.Clear();
                        return false;
                    }
                }
            }

            this.m_tableGlyf=(val_glyf)this.m_font.GetTable("glyf");
            if (this.m_tableGlyf==null)
            {
                this.m_validator.Error(T.T_NULL, 
                    E.glyf_E_UnableToStartValidation,
                    (OTTag)"glyf",
                    "Missing table: glyf");
                this.Clear();
                return false;
            }

            this.m_tableLoca=(val_loca)this.m_font.GetTable("loca");
            if (this.m_tableLoca==null)
            {
                this.m_validator.Error(T.T_NULL, 
                    E.glyf_E_UnableToStartValidation,
                    (OTTag)"glyf",
                    "Missing table: loca");
                this.Clear();
                return false;
            }

            if (!this.m_tableLoca.ValidateFormat(m_validator, this.m_font))
            {
                this.m_validator.Error(T.T_NULL, 
                    E.glyf_E_UnableToStartValidation,
                    (OTTag)"glyf",
                    "Table 'loca' has incorrect format");
                this.Clear();
                return false;
            }

            if (!this.m_tableLoca.ValidateNumEntries(null,this.m_font))
            {
                this.m_validator.Error(T.T_NULL, 
                    E.glyf_E_UnableToStartValidation,
                    (OTTag)"glyf",
                    "Table 'loca' has incorrect number of entries");
                this.Clear();
                return false;
            }

            this.m_numGlyph=this.m_tableLoca.NumEntry(this.m_font)-1;
            return true;
        }
        
        public void Clear()
        {
            if (this.m_toCloseFileOnClear)
            {
                if (this.m_font!=null)
                {
                    this.m_font.GetFile().close();
                }
            }
            this.m_font=null;
            this.m_tableLoca=null;
            this.m_tableGlyf=null;
            if (this.m_validator!=null)
            {
                this.m_validator.Clear();
            }
            this.m_validator=null;
        }

        public void ReadTypeGlyph(int indexGlyph, 
            out GConsts.TypeGlyph typeGlyph, 
            DIAction dia)
        {
            typeGlyph=GConsts.TypeGlyph.Undef;
            this.m_validator.DIA=dia;
            
            int offsStart,length;
            if (!this.m_tableLoca.GetValidateEntryGlyf(indexGlyph,
                out offsStart,out length, this.m_validator, this.m_font))
                return;
            if (length==0)
            {
                
                typeGlyph=GConsts.TypeGlyph.Empty;
                return; 
            }
            if ((int)Table_glyf.FieldOffsets.numCont+2>=offsStart+length)
            {
                this.m_validator.Error(
                    E._GEN_E_OffsetExceedsTableLength, 
                    (OTTag)"glyf");
                return;
            }
            int numCont;
            numCont=this.m_tableGlyf.Buffer.GetShort((uint)(offsStart+(int)Table_glyf.FieldOffsets.numCont));
            typeGlyph=(numCont>=0)? GConsts.TypeGlyph.Simple: GConsts.TypeGlyph.Composite;
        }

        public void ReadGGB(int indexGlyph, 
            out BoxD bbox, 
            DIAction dia)
        {
            this.m_validator.DIA=dia;

            bbox=null;
            
            int offsStart, length;
            if (!this.m_tableLoca.GetValidateEntryGlyf(indexGlyph, 
                out offsStart, out length, this.m_validator, this.m_font))
                return;
            if (length==0)
                return;
            short xMin, yMin, xMax, yMax;
            if ((int)Table_glyf.FieldOffsets.nextAfterHeader>=length)
            {
                this.m_validator.Error(
                    E._GEN_E_OffsetExceedsTableLength, 
                    (OTTag)"glyf");
                return;
            }
            
            MBOBuffer buffer=this.m_tableGlyf.Buffer;
            xMin=buffer.GetShort((uint)(offsStart+(int)Table_glyf.FieldOffsets.xMin));
            yMin=buffer.GetShort((uint)(offsStart+(int)Table_glyf.FieldOffsets.yMin));
            xMax=buffer.GetShort((uint)(offsStart+(int)Table_glyf.FieldOffsets.xMax));
            yMax=buffer.GetShort((uint)(offsStart+(int)Table_glyf.FieldOffsets.yMax));
            bbox=new BoxD(xMin,yMin,xMax,yMax);
        }
        
        public void ReadGGO(int indexGlyph, 
            out Outline outl, 
            DIAction dia)
        {
            this.m_validator.DIA=dia;
            outl=null;
            
            GConsts.TypeGlyph typeGlyph;
            this.ReadTypeGlyph(indexGlyph, out typeGlyph, dia);
            if (typeGlyph!=GConsts.TypeGlyph.Simple)
                return;
            int offsStart, length;
            if (!this.m_tableLoca.GetValidateEntryGlyf(indexGlyph,
                out offsStart, out length, this.m_validator, this.m_font))
                return;
            MBOBuffer buffer=this.m_tableGlyf.Buffer;
            int iKnot;
            ushort[] arrIndKnotEnd;
            short[] arrXRel, arrYRel;
            byte[] arrFlag;
            int numCont;
            try 
            {
                numCont=buffer.GetShort((uint)(offsStart+(int)Table_glyf.FieldOffsets.numCont));
                arrIndKnotEnd = new ushort [numCont];
                for (short iCont=0; iCont<numCont; iCont++)
                {
                    arrIndKnotEnd[iCont]=buffer.GetUshort((uint)(offsStart+Table_glyf.FieldOffsets.nextAfterHeader+iCont*2));
                }
                int numKnot=arrIndKnotEnd[numCont-1]+1;
                uint offsInstrLength=(uint)(offsStart+Table_glyf.FieldOffsets.nextAfterHeader+2*numCont);
                ushort lengthInstr=buffer.GetUshort(offsInstrLength);
                uint offsInstr=offsInstrLength+2;
                uint offsFlag=offsInstr+lengthInstr;
                arrFlag = new byte [numKnot];
                iKnot=0;                    // index of flag in array flags
                uint offsCur=offsFlag;        // counter of flag in the file
                while (iKnot<numKnot)
                {
                    byte flag=buffer.GetByte(offsCur++);
                    arrFlag[iKnot++]=flag;
                    bool toRepeat=((flag&(byte)(Table_glyf.MaskFlagKnot.toRepeat))!=0);
                    if (toRepeat)
                    {
                        byte numRepeat=buffer.GetByte(offsCur++);
                        for (byte iRepeat=0; iRepeat<numRepeat; iRepeat++)
                        {
                            arrFlag[iKnot++]=flag;
                        }
                    }
                }
                arrXRel = new short [numKnot];
                arrYRel = new short [numKnot];
                // read data for x-coordinates
                for (iKnot=0; iKnot<numKnot; iKnot++)
                {
                    if ((arrFlag[iKnot]&(byte)(Table_glyf.MaskFlagKnot.isXByte))!=0)
                    {
                        byte xRel=buffer.GetByte(offsCur++);
                        if ((arrFlag[iKnot]&(byte)(Table_glyf.MaskFlagKnot.isXSameOrPozitive))!=0)
                        {
                            arrXRel[iKnot]=xRel;
                        }
                        else
                        {
                            arrXRel[iKnot]=(short)(-xRel);
                        }
                    }
                    else
                    {
                        if ((arrFlag[iKnot]&(byte)(Table_glyf.MaskFlagKnot.isXSameOrPozitive))!=0)
                        {
                            arrXRel[iKnot]=0;
                        }
                        else
                        {
                            arrXRel[iKnot]=buffer.GetShort(offsCur);
                            offsCur+=2;
                        }
                    }
                }
                // read data for y-coordinates
                for (iKnot=0; iKnot<numKnot; iKnot++)
                {
                    if ((arrFlag[iKnot]&(byte)(Table_glyf.MaskFlagKnot.isYByte))!=0)
                    {
                        byte yRel=buffer.GetByte(offsCur++);
                        if ((arrFlag[iKnot]&(byte)(Table_glyf.MaskFlagKnot.isYSameOrPozitive))!=0)
                        {
                            arrYRel[iKnot]=yRel;
                        }
                        else
                        {
                            arrYRel[iKnot]=(short)(-yRel);
                        }
                    }
                    else
                    {
                        if ((arrFlag[iKnot]&(byte)(Table_glyf.MaskFlagKnot.isYSameOrPozitive))!=0)
                        {
                            arrYRel[iKnot]=0;
                        }
                        else
                        {
                            arrYRel[iKnot]=buffer.GetShort(offsCur);
                            offsCur+=2;
                        }
                    }
                }
                if (offsCur-2>=offsStart+length)
                {
                    throw new System.IndexOutOfRangeException();
                }
            }
            catch (System.IndexOutOfRangeException) 
            {
                this.m_validator.Error(
                    E._GEN_E_OffsetExceedsTableLength,
                    (OTTag)"glyf");
                return;
            }

            try
            {
                short xAbs=0;
                short yAbs=0;
                int indKnotStart, indKnotEnd=-1;
                outl=new Outline();
                for (ushort iCont=0; iCont<numCont; iCont++)
                {
                    indKnotStart=indKnotEnd+1;
                    indKnotEnd=arrIndKnotEnd[iCont];
                    Contour cont=null;
                    cont = new Contour();
                    for (iKnot=indKnotStart; iKnot<=indKnotEnd; iKnot++)
                    {
                        xAbs += arrXRel[iKnot];
                        yAbs += arrYRel[iKnot];
                        bool isOn=((arrFlag[iKnot]&((byte)(Table_glyf.MaskFlagKnot.isOnCurve)))!=0);
                        Knot knot = new Knot(iKnot, xAbs, yAbs, isOn);
                        cont.KnotAdd(knot);
                    }
                    outl.ContourAdd(cont);
                }
            }
            catch
            {
                outl.ClearDestroy();
                outl=null;
            }
        }

        public void ReadGGC(int indexGlyph, 
            out Composite comp, 
            DIAction dia)
        {
            this.m_validator.DIA=dia;
            comp=null;
    
            GConsts.TypeGlyph typeGlyph;
            this.ReadTypeGlyph(indexGlyph, out typeGlyph, dia);
            if (typeGlyph!=GConsts.TypeGlyph.Composite)
                return;
            int offsStart, length;
            if (!this.m_tableLoca.GetValidateEntryGlyf(indexGlyph,
                out offsStart, out length, this.m_validator, this.m_font))
                return;

            MBOBuffer buffer=this.m_tableGlyf.Buffer;
            uint offsCur=(uint)(offsStart+Table_glyf.FieldOffsets.nextAfterHeader);
            bool isLast=false;
            try
            {
                Component component;
                while (!isLast)
                {
                    ushort flags=buffer.GetUshort(offsCur);
                    isLast=((flags&(ushort)Table_glyf.MaskFlagComponent.MORE_COMPONENTS)==0);
                    offsCur+=2;

                    ushort indGlyphCur=buffer.GetUshort(offsCur); // TODO: save
                    component=new Component(indGlyphCur);
                
                    // TODO: validate indGlyph is in right boundaries
                    // TODO: add Relations to FManager
                    offsCur+=2;

                    bool weHaveAScale=((flags&(ushort)Table_glyf.MaskFlagComponent.WE_HAVE_A_SCALE)!=0);
                    bool weHaveAnXAndYScale=((flags&(ushort)Table_glyf.MaskFlagComponent.WE_HAVE_AN_X_AND_Y_SCALE)!=0);
                    bool weHaveATwoByTwo=((flags&(ushort)Table_glyf.MaskFlagComponent.WE_HAVE_A_TWO_BY_TWO)!=0);
                    int cnt=0;
                    if (weHaveAScale) cnt++;
                    if (weHaveAnXAndYScale) cnt++;
                    if (weHaveATwoByTwo) cnt++;
                    if (cnt>1)
                    {
                        this.m_validator.Error(T.T_NULL,
                            E.glyf_E_CompositeAmbigousTransform, 
                            (OTTag)"glyf",
                            "Index Component: "+indGlyphCur);
                        return;
                    }
                    if ((flags&(ushort)Table_glyf.MaskFlagComponent.RESERVED)!=0)
                    {
                        this.m_validator.Warning(T.T_NULL,
                            W.glyf_W_CompositeReservedBit,
                            (OTTag)"glyf",
                            "Index Component: "+indGlyphCur);
                    }

                    int arg1, arg2;
                    if ((flags&(ushort)Table_glyf.MaskFlagComponent.ARG_1_AND_2_ARE_WORDS)!=0)
                    {
                        arg1=(int)buffer.GetShort(offsCur);
                        offsCur+=2;
                        arg2=(int)buffer.GetShort(offsCur);
                        offsCur+=2;
                    }
                    else
                    {
                        arg1=(int)buffer.GetSbyte(offsCur);
                        offsCur+=1;
    
                        arg2=(int)buffer.GetSbyte(offsCur);
                        offsCur+=1;
                    }                    
                    // TODO: validate bounding boxes
                    // TODO: check that NOT BOTH shift & knots are initialized, but ONE of them IS initialized
                    // TODO: validate that indKnots (both!) are in the right boundaries
                    // TODO: validate that a single-point contour in any glyph is used as attachment point in at least one other glyph
                    if ((flags&(ushort)Table_glyf.MaskFlagComponent.ARGS_ARE_XY_VALUES)!=0)
                    {
                        component.Shift=new VecD(arg1,arg2);
                    }
                    else
                    {
                        component.IndexKnotAttGlyph=arg1;
                        component.IndexKnotAttComponent=arg2;                
                    }

                    // TODO: check that matrix is non-degenerated (if not null)
                    if (weHaveAScale)
                    {

                        OTF2Dot14[,] m=new OTF2Dot14[2,2];

                        m[0,0]=buffer.GetF2Dot14(offsCur);
                        
                        /*
                        // for debug only - begin
                        if (indGlyphCur==272)
                        {
                            m[0,0]=new OTF2Dot14(30390);
                        }
                        // for debug only - end
                        */
                        
                        offsCur+=2;
                        m[1,1]=m[0,0];
                        component.TrOTF2Dot14=m;
                    }
                    else if (weHaveAnXAndYScale)
                    {
                        OTF2Dot14[,] m=new OTF2Dot14[2,2];
                        m[0,0]=buffer.GetF2Dot14(offsCur);
                        offsCur+=2;
                        m[1,1]=buffer.GetF2Dot14(offsCur);
                        offsCur+=2;
                        component.TrOTF2Dot14=m;
                    }
                    else if (weHaveATwoByTwo)
                    {
                        OTF2Dot14[,] m=new OTF2Dot14[2,2];
                        m[0,0]=buffer.GetF2Dot14(offsCur);
                        offsCur+=2;
                        m[0,1]=buffer.GetF2Dot14(offsCur);
                        offsCur+=2;
                        m[1,0]=buffer.GetF2Dot14(offsCur);
                        offsCur+=2;
                        m[1,1]=buffer.GetF2Dot14(offsCur);
                        offsCur+=2;
                        component.TrOTF2Dot14=m;
                    }
        
                    if ((flags&(ushort)Table_glyf.MaskFlagComponent.WE_HAVE_INSTRUCTIONS)!=0)
                    {
                        ushort numInstr=buffer.GetUshort(offsCur);
                        offsCur+=2;
                        if (offsCur+numInstr>buffer.GetLength())
                        {
                            throw new System.IndexOutOfRangeException();
                        }
                    }
                    if (comp==null)
                        comp=new Composite();
                    comp.AddComponent(component);
                }
            }
            catch (System.IndexOutOfRangeException) 
            {
                this.m_validator.Error(
                    E._GEN_E_OffsetExceedsTableLength,
                    (OTTag)"glyf");
                if (comp != null)
                {
                    comp.ClearDestroy();
                    comp=null;
                }
            }
        }
        
        
        public bool ReadNumGlyph(out int numGlyph, DIAction dia)
        {
            numGlyph=this.m_numGlyph;
            return true;
        }
        

        // TODO: rewrite
        public void ReadFontMetrics(out int descendMin, 
            out int ascendMax,    
            out int lsbMin, 
            out int rsbMax, 
            DIAction dia)
        {
            descendMin=-700;
            ascendMax=2100;
            lsbMin=-1000;
            rsbMax=3500;

            try
            {
                BoxD bboxRes=new BoxD();

                Table_cmap tableCmap = (Table_cmap)this.m_font.GetTable("cmap");
                if (tableCmap==null)
                    return;
                Table_cmap.EncodingTableEntry eteUni = 
                    tableCmap.GetEncodingTableEntry(3,1);
                if (eteUni==null)
                    return;
                Table_cmap.Subtable subtableCmap = tableCmap.GetSubtable(eteUni);
                char[] samples={'A','W','p'};
                for (int iSample=0; iSample<samples.Length; iSample++)
                {                
                    byte[] charbuf = new byte[2];
                    charbuf[0] = (byte)'A';
                    charbuf[1] = 0; 
                    int indGlyph = (int)subtableCmap.MapCharToGlyph(charbuf, 0);
                    Table_glyf.header header=this.m_tableGlyf.GetGlyphHeader((uint)indGlyph,this.m_font);
                    if (header==null)
                        return;
                    BoxD bboxCur=new BoxD(header.xMin, header.yMin,
                        header.xMax, header.yMax);
                    bboxRes.Unite(bboxCur);
                }
                descendMin=(int)bboxRes.VecMin.Y;
                ascendMax=(int)bboxRes.VecMax.Y;
                lsbMin=(int)bboxRes.VecMin.X;
                rsbMax=(int)bboxRes.VecMax.X;
            }
            catch
            {
            }
        }
    }
}

