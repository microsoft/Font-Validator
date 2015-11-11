using System;
using System.Diagnostics;
using System.Reflection;
using System.Collections;
using System.Text;

using NS_GMath;
using NS_ValCommon;
using NS_IDraw;

using ArrayList = System.Collections.ArrayList;


namespace NS_Glyph
{
    public class Glyph : I_Drawable
    {        
        /*
         *        MEMBERS:    content
         */ 
        private int index;
        private GConsts.TypeGlyph typeGlyph;
            

        BoxD        bbox;
        Outline        outl;
        Composite    comp;
        
        /*
         *        MEMBERS:    management 
         */
        private FManager        fm;
        
        private StatusGV[]        statusGV;

    
        /*
         *        PROPERTIES
         */
        internal BoxD BBox
        {
            get { return this.bbox; }
            set 
            { 
                if (this.bbox!=null)
                {
                    this.bbox.Clear();
                    this.bbox=null;
                }
                this.bbox=value; 
            }
        }
        internal Outline Outl
        {
            get { return this.outl; }
            set 
            { 
                if (this.outl!=null)
                {
                    this.outl.ClearDestroy();
                    this.outl=null;
                }
                this.outl=value; 
            }
        }
        internal Composite Comp
        {
            get { return this.comp; }
            set 
            { 
                if (this.comp!=null)
                {
                    this.comp.ClearDestroy();
                    this.comp=null;
                }
                this.comp=value; 
            }
        }
        public bool IsDiffFromSource
        {
            get
            {
                switch (this.typeGlyph)
                {
                    case GConsts.TypeGlyph.Uninitialized:
                        return true;
                    case GConsts.TypeGlyph.Undef:
                        return false;
                    case GConsts.TypeGlyph.Empty:
                        return ((this.outl!=null)||(this.comp!=null));
                    case GConsts.TypeGlyph.Simple:
                        return (!this.statusGV[(int)DefsGV.TypeGV.ValidateSimpSource].IsValid);
                    case GConsts.TypeGlyph.Composite:
                        return (!this.statusGV[(int)DefsGV.TypeGV.ValidateCompSource].IsValid);
                }
                throw new ExceptionGlyph("Glyph","IsDiffFromSource",null);
                //return true;
            }
        }

        /*
        internal Flags IsFailedGV
        {
            get { return this.f_isFailedGV; }
        }
        internal Flags IsValidGV
        {
            get { return this.f_isValidGV; }
        }
        */
        public GConsts.TypeGlyph TypeGlyph
        {
            get { return this.typeGlyph; }
        }
        public int IndexGlyph
        {
            get {return this.index;}
        }
        internal ArrayList CompDep
        {
            get { return this.fm.GGetCompDep(this.index); }
        }

        public string StringInfoGeneral
        {
            get 
            { 
                StringBuilder sb=new StringBuilder();
                sb.Append("Glyph index: "+this.index);
                sb.Append("   type:"+this.typeGlyph);
                if ((this.typeGlyph==GConsts.TypeGlyph.Composite)&&(this.comp!=null))
                {
                    sb.Append("   components: ");
                    foreach (Component component in this.comp)
                    {
                        sb.Append(component.IndexGlyphComponent);
                        sb.Append(" ");
                    }
                }
                if (this.typeGlyph==GConsts.TypeGlyph.Simple)
                {
                    if (this.bbox==null)
                        sb.Append("\r\nFailed to load Bounding Box");
                    if (this.outl==null)
                        sb.Append("\r\nFailed to load Outline");
                }
                if (this.typeGlyph==GConsts.TypeGlyph.Composite)
                {
                    if (this.bbox==null)
                        sb.Append("\r\nFailed to load Bounding Box");
                    if (this.comp==null)
                        sb.Append("\r\nFailed to load Composite Data");
                }
                if (!this.IsDiffFromSource)
                {
                    sb.Append("\r\nUnmodified (with respect to source)");
                }
                else
                {
                    sb.Append("\r\nMODIFIED (with respect to source)");
                }
                return sb.ToString();
            }
        }
        /*
         *        CONSTRUCTORS
         */
        private Glyph()
        {
        }

        public Glyph(int indexGlyph, FManager fm)
        {
            this.fm=fm;

            this.index=indexGlyph;
            this.typeGlyph=GConsts.TypeGlyph.Uninitialized;
            this.bbox=null;
            this.outl=null;
            this.comp=null;

            this.statusGV=new StatusGV[DefsGV.NumTest];
            for (int iTest=0; iTest<DefsGV.NumTest; iTest++)
            {
                this.statusGV[iTest]=new StatusGV();
            }
        }

        /*
         *        METHODS : ACCESS
         */

        internal StatusGV StatusVal(DefsGV.TypeGV typeGV)
        {
            return this.statusGV[(int)typeGV]; 
        }


        /*
             *        METHODS : CALLS
             */

        private bool CanExecDepend(DefsGV.TypeGV typePreRequired)
        {

            if ((this.typeGlyph!=GConsts.TypeGlyph.Simple)&&
                (this.typeGlyph!=GConsts.TypeGlyph.Composite))
            {
                throw new ExceptionGlyph("Glyph","CanExecDepend",null);
            }
            if ((this.typeGlyph==GConsts.TypeGlyph.Composite)&&
                (DefsGV.IsPureSimp(typePreRequired)))
                return true;

            if (this.statusGV[(int)typePreRequired].StatusExec!=
                StatusGV.TypeStatusExec.Completed)
                return false;

            switch (typePreRequired)
            {
                case DefsGV.TypeGV.ValidateTypeGlyphSource:
                    return (this.typeGlyph!=GConsts.TypeGlyph.Undef);
                case DefsGV.TypeGV.ValidateSimpSource:
                    return ((this.bbox!=null)&&(this.outl!=null));
                case DefsGV.TypeGV.ValidateCompSource:
                    return ((this.bbox!=null)&&(this.comp!=null));
                case DefsGV.TypeGV.ValidateCompBind:
                    if (!this.statusGV[(int)typePreRequired].IsValid)
                        return false;
                    return ((this.bbox!=null)&&(this.comp!=null)&&(this.outl!=null));
                default:
                    if (!this.statusGV[(int)typePreRequired].IsValid)
                        return false;
                    return true;
            }
        }

        private bool CanBeExec(DefsGV.TypeGV typeGV)
        {
            DefsGV.TypeGV[] typesPreRequired=DefsGV.GetTestsPreRequired(typeGV);
            if (typesPreRequired!=null)
            {
                foreach (DefsGV.TypeGV typePreRequired in typesPreRequired)
                {
                    if (!this.CanExecDepend(typePreRequired))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        

        private void CallGV(DefsGV.TypeGV typeGV)
        {    
            /*
             *        TRY/CATCH function
             */

            /*
             *        MEANING:    UNCONDITIONAL call of action
             */

            GErrList gerrlist=null;
            try
            {
                this.statusGV[(int)typeGV].StatusExec=StatusGV.TypeStatusExec.Aborted;
                this.statusGV[(int)typeGV].StatusRes=StatusGV.TypeStatusRes.Undef;
                gerrlist=new GErrList();
            
                if (DefsGV.IsModifier(typeGV))
                {
                    this.fm.OnGM(this.index,DefsGM.From(typeGV),true);        
                }

                // check that pre-required tests are performed
                if (!this.CanBeExec(typeGV))
                {
                    this.statusGV[(int)typeGV].StatusExec=
                        StatusGV.TypeStatusExec.UnableToExec;
                }

                // call Validate.. function
                System.Exception excValidation=null;
                if (this.statusGV[(int)typeGV].StatusExec!=
                    StatusGV.TypeStatusExec.UnableToExec)
                {
                    try
                    {
                        string strTypeCV=Enum.GetName(typeof(DefsGV.TypeGV),typeGV);
                        MethodInfo infoMethod=this.GetType().GetMethod(strTypeCV, 
                            System.Reflection.BindingFlags.Instance|
                            System.Reflection.BindingFlags.NonPublic);
                        if (infoMethod!=null)
                        {
                            object[] pars={gerrlist};
                
                            // statusGV can be set to:
                            // Completed || UnableToExec || Aborted
                            try
                            {
                                this.statusGV[(int)typeGV].StatusExec=(StatusGV.TypeStatusExec)
                                    infoMethod.Invoke(this, pars);
                            }
                            catch(System.Reflection.TargetInvocationException TIException)
                            {
                                throw TIException.InnerException;
                            }

                            GErrSign.Sign(gerrlist, typeGV, this.index);
                        }
                        if (this.statusGV[(int)typeGV].StatusExec==
                            StatusGV.TypeStatusExec.Completed)
                        {
                            this.statusGV[(int)typeGV].StatusRes=
                                gerrlist.StatusResGV(typeGV);
                        }
                    }
                    catch (System.Exception exc)
                    {
                        excValidation=exc;
                        this.statusGV[(int)typeGV].StatusExec=
                            StatusGV.TypeStatusExec.Aborted;
                    }
                }

                if (this.statusGV[(int)typeGV].StatusExec!=
                    StatusGV.TypeStatusExec.Completed)
                {
                    this.CleanUpOnNonCompletedGV(typeGV);
                    GErrValidation gerrVal=new GErrValidation(this.index,
                        this.typeGlyph,
                        typeGV,
                        this.statusGV[(int)typeGV].StatusExec,
                        excValidation);
                    gerrlist.Add(gerrVal);
                }

                GErrSign.Sign(gerrlist,typeGV,this.index);
                this.fm.OnGV(this.index, typeGV, gerrlist);
                gerrlist.ClearRelease();
            }

            catch (Exception excManagement) // exception thrown by the fm management structures 
            {
                this.statusGV[(int)typeGV].StatusExec=
                    StatusGV.TypeStatusExec.Aborted;
                this.CleanUpOnNonCompletedGV(typeGV);    // T/C function
                GErrValidation gerrVal=new GErrValidation(this.index,
                    this.typeGlyph,
                    typeGV,
                    StatusGV.TypeStatusExec.Aborted,
                    excManagement);
                this.fm.ReportFailure(gerrVal);            // T/C function
                if (gerrlist!=null)
                {
                    try { gerrlist.ClearRelease(); }
                    catch (Exception) {}
                }
            }                        
        }

        private void CleanUpOnNonCompletedGV(DefsGV.TypeGV typeGV)
        {
            /*
             *        TRY/CATCH function
             */
            switch (typeGV)
            {
                case DefsGV.TypeGV.ValidateTypeGlyphSource:
                    this.typeGlyph=GConsts.TypeGlyph.Undef;
                    break;
                case DefsGV.TypeGV.ValidateSimpSource:
                    if (this.bbox!=null)
                        this.bbox=null;
                    if (this.outl!=null)
                    {
                        try 
                        {
                            this.outl.ClearDestroy();
                        }
                        catch(System.Exception)
                        {
                        }
                        this.outl=null;
                    }
                    break;
                case DefsGV.TypeGV.ValidateCompSource:
                    if (this.bbox!=null)
                        this.bbox=null;
                    if (this.comp!=null)
                    {
                        try 
                        {
                            this.comp.ClearDestroy();
                        }
                        catch(System.Exception)
                        {
                        }
                        this.comp=null;
                    }
                    break;
                case DefsGV.TypeGV.ValidateCompBind:
                    if (this.outl!=null)
                    {
                        try 
                        {
                            this.outl.ClearDestroy();
                        }
                        catch(System.Exception)
                        {
                        }
                        this.outl=null;
                    }
                    break;
            }
            
            /*
            private int index;
            private GConsts.TypeGlyph typeGlyph;
            BoxD        bbox;
            Outline        outl;
            Composite    comp;
            private FManager        fm;        
            private StatusGV[]        statusGV;
            */



        }

        private void CallGM(DefsGM.TypeGM typeGM, params object[] pars)
        {
            /*
             *        MEANING: UNCONDITIONAL call of action
             */
            this.fm.OnGM(this.index,typeGM,true);

            string strTypeGM=Enum.GetName(typeof(DefsGM.TypeGM),typeGM);
            MethodInfo infoMethod=typeof(Glyph).GetMethod(strTypeGM);
            try
            {
                infoMethod.Invoke(this,pars);
            }
            catch(System.Reflection.TargetInvocationException TIException)
            {
                throw TIException.InnerException;
            }
        }

        override public int GetHashCode()
        {
            return this.IndexGlyph;
        }
        override public bool Equals(object obj)
        {
            Glyph glyph = obj as Glyph;
            if ((object)glyph==null)
                return false;
            return (this.IndexGlyph==glyph.IndexGlyph);
        }

        public void ClearDestroy()
        {
            if (this.bbox!=null)
            {
                this.bbox.Clear();
                this.bbox=null;
            }
            if (this.comp!=null)
            {
                this.comp.ClearDestroy();
                this.comp=null;
            }
            if (this.outl!=null)
            {
                this.outl.ClearDestroy();
                this.outl=null;
            }
            if (this.statusGV!=null)
            {
                this.statusGV=null;
            }
            this.index=GConsts.IND_UNINITIALIZED;
            this.fm=null;

        }

        public void ClearRelease()
        {
            if (this.bbox!=null)
            {
                this.bbox.Clear();
                this.bbox=null;
            }
            if (this.comp!=null)
            {
                this.comp.ClearRelease();
                this.comp=null;
            }
            if (this.outl!=null)
            {
                this.outl.ClearRelease();
                this.outl=null;
            }
            this.index=GConsts.IND_UNINITIALIZED;
            this.fm=null;
        }
        
        internal Glyph CreateCopy()
        {
            // TODO: write !!!
            return null;
        }


        /*
         *        METHODS    :    VALIDATORS SOURCE
         */
        internal StatusGV.TypeStatusExec ValidateTypeGlyphSource(GErrList gerrlist)
        {
            DIAction dia=DIActionBuilder.DIA(gerrlist,"DIAFunc_AddToListUnique");
            gerrlist.DIW=new DIWrapperSource(this.index,
                GConsts.TypeGlyph.Undef,
                GScope.TypeGScope._GG_);

            I_IOGlyphs i_IOGlyphs=this.fm.IIOGlyphs;
            if (i_IOGlyphs==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateTypeGlyphSource",null);
                return StatusGV.TypeStatusExec.Aborted;
            }

            int numErrBefore=gerrlist.Length;
            i_IOGlyphs.ReadTypeGlyph(this.index,out this.typeGlyph, dia);
            int numErrAfter=gerrlist.Length;
            int iErr;
            for (iErr=numErrBefore; iErr<numErrAfter; iErr++)
            {
                gerrlist[iErr].TypeGlyph=this.typeGlyph;    
            }
            gerrlist.DIW=null;
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateSimpSource(GErrList gerrlist)
        {
            if (this.bbox!=null)
            {
                this.bbox.Clear();
                this.bbox=null;
            }
            if (this.outl!=null)
            {
                this.outl.ClearDestroy();
                this.outl=null;
            }    

            if (this.typeGlyph==GConsts.TypeGlyph.Empty)
            {
                return StatusGV.TypeStatusExec.Completed;
            }
            if (this.typeGlyph!=GConsts.TypeGlyph.Simple)            
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpSource",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            DIAction dia=DIActionBuilder.DIA(gerrlist,"DIAFunc_AddToListUnique");
            gerrlist.DIW=new DIWrapperSource(this.index,this.typeGlyph,GScope.TypeGScope._GGB_);

            I_IOGlyphs i_IOGlyphs=this.fm.IIOGlyphs;
            if (i_IOGlyphs==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpSource",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            i_IOGlyphs.ReadGGB(this.index, out this.bbox, dia);
            i_IOGlyphs.ReadGGO(this.index, out this.outl, dia);
            gerrlist.DIW=null;
            
            return StatusGV.TypeStatusExec.Completed;
        }
        
        internal StatusGV.TypeStatusExec ValidateCompSource(GErrList gerrlist)
        {
            if (this.bbox!=null)
            {
                this.bbox.Clear();
                this.bbox=null;
            }
            if (this.comp!=null)
            {
                this.comp.ClearDestroy();
                this.comp=null;
            }
            if (this.outl!=null)
            {
                this.outl.ClearDestroy();
                this.outl=null;
            }    
            
            if (this.typeGlyph==GConsts.TypeGlyph.Empty)
            {
                return StatusGV.TypeStatusExec.Completed;
            }
            if (this.typeGlyph!=GConsts.TypeGlyph.Composite)            
            {
                //throw new ExceptionGlyph("Glyph","ValidateCompSource",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            DIAction dia=DIActionBuilder.DIA(gerrlist,"DIAFunc_AddToListUnique");
            gerrlist.DIW=new DIWrapperSource(this.index,this.typeGlyph,GScope.TypeGScope._GGB_);

            I_IOGlyphs i_IOGlyphs=this.fm.IIOGlyphs;
            if (i_IOGlyphs==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateCompSource",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            i_IOGlyphs.ReadGGB(this.index, out this.bbox, dia);
            i_IOGlyphs.ReadGGC(this.index, out this.comp, dia);
            if (this.comp!=null)
            {
                this.fm.OnGComposite(this.index);
            }


            gerrlist.DIW=null;
            return StatusGV.TypeStatusExec.Completed;        
        }


        /*
         *        METHODS    :    VALIDATORS CONTENT
         *
         */

        internal StatusGV.TypeStatusExec ValidateSimpBBox(GErrList gerrlist)
        {
            if ((this.bbox==null)||(this.outl==null))
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpBBox",null);
                return StatusGV.TypeStatusExec.Aborted;    
            }
            BoxD bboxCP;        // bbox for the control points
            bboxCP=this.outl.BBoxCP;
            if (!bbox.Equals(bboxCP))
            {
                GErrBBox gerrBbox=new GErrBBox(this.IndexGlyph,
                    this.typeGlyph,
                    this.bbox,bboxCP);
                gerrlist.Add(gerrBbox);
            }
            BoxD bboxOutl;        // bbox for Outline
            bboxOutl=this.outl.BBox;
            //bboxOutline.SetEnlargeFU();
            if (!bboxOutl.Equals(bboxCP))
            {
                // warning
                GErrExtremeNotOnCurve gerrExtreme=new 
                    GErrExtremeNotOnCurve(this.IndexGlyph,
                    this.typeGlyph,bboxCP,bboxOutl);
                gerrlist.Add(gerrExtreme);

            }
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateSimpKnotDupl(GErrList gerrlist)
        {
            ListPairInt infosKnotDupl=new ListPairInt();
            for (int pozCont=0; pozCont<this.outl.NumCont; pozCont++)
            {
                Contour cont=this.outl.ContourByPoz(pozCont);
                cont.KnotDupl(infosKnotDupl);
            }
            if (infosKnotDupl.Count!=0)
            {
                GErr gerr=new GErrKnotDupl(this.index, infosKnotDupl);
                gerrlist.Add(gerr);
            }
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateSimpContDegen(GErrList gerrlist)
        {
            ArrayList listIndsKnotStart=new ArrayList();
            for (int pozCont=0; pozCont<this.outl.NumCont; pozCont++)
            {
                Contour cont=this.outl.ContourByPoz(pozCont);
                if (cont.IsDegen)
                {
                    listIndsKnotStart.Add(cont.IndKnotStart);
                }
            }
            int numCont=listIndsKnotStart.Count;
            if (numCont!=0)
            {
                int[] indsKnotStart=new int[numCont];
                for (int iCont=0; iCont<numCont; iCont++)
                {
                    indsKnotStart[iCont]=(int)listIndsKnotStart[iCont];
                }
                GErr gerr=new GErrContDegen(this.index, indsKnotStart);
                gerrlist.Add(gerr);
            }
            return StatusGV.TypeStatusExec.Completed;
        }
        
        internal StatusGV.TypeStatusExec ValidateSimpContWrap(GErrList gerrlist)
        {
            /*
             *        ASSUMPTION: determines only "fully" wrapped contours
             */
            if (this.outl==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpContWrap",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
        
            ArrayList pozsContWrap=new ArrayList();
            for (int pozCont=0; pozCont<this.outl.NumCont; pozCont++)
            {
                Contour cont=this.outl.ContourByPoz(pozCont);
                bool isWrapped;
                if (!cont.IsWrapped(out isWrapped))
                {
                    //throw new ExceptionGlyph("Glyph","ValidateSimpContWrap",null);
                    return StatusGV.TypeStatusExec.Aborted;
                }
                if (isWrapped)
                {
                    pozsContWrap.Add(pozCont);
                }
            }
            int numCont=pozsContWrap.Count;
            if (numCont!=0)
            {
                int[] indsKnotStart=new int[numCont];
                for (int iCont=0; iCont<numCont; iCont++)
                {
                    int pozCont=(int)pozsContWrap[iCont];
                    indsKnotStart[iCont]=this.Outl.ContourByPoz(pozCont).IndKnotStart;
                }
                GErr gerr=new GErrContWrap(this.index, indsKnotStart);
                gerrlist.Add(gerr);
                //this.isOrientDefined=false;
            }
            /*
            else
            {
                //this.isOrientDefined=true;
            }
            */
            pozsContWrap.Clear();
            return StatusGV.TypeStatusExec.Completed;
        }
    

        internal StatusGV.TypeStatusExec ValidateSimpContDupl(GErrList gerrlist)
        {
            if (this.outl==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpContDupl",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            ListPairInt pairsIndKnotStart=new ListPairInt();
            for (int pozContA=0; pozContA<this.outl.NumCont-1; pozContA++)
            {
                Contour contA=this.outl.ContourByPoz(pozContA);
                for (int pozContB=pozContA+1; pozContB<this.outl.NumCont; pozContB++)
                {
                    Contour contB=this.outl.ContourByPoz(pozContB);
                    bool isDupl;
                    if (!contA.AreDuplicated(contB, out isDupl))
                    {
                        //throw new ExceptionGlyph("Glyph","ValidateSimpContDupl",null);
                        return StatusGV.TypeStatusExec.Aborted;
                    }
                    if (isDupl)
                    {
                        pairsIndKnotStart.Add(this.outl.ContourByPoz(pozContA).IndKnotStart,
                            this.outl.ContourByPoz(pozContB).IndKnotStart);
                    }
                }
            }
            if (pairsIndKnotStart.Count!=0)
            {
                GErr gerr=new GErrContDupl(this.index, pairsIndKnotStart);
                gerrlist.Add(gerr);
                //this.isOrientDefined=false;
            }
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateSimpContInters(GErrList gerrlist)
        {
            /*
             *        MEANING:    finds ALL intersections: 
             *                    including D0, D1 and S/I bezier
             */
            ListInfoInters linters = new ListInfoInters();
            if (this.outl==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpContInters",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            if (!this.outl.Intersect(linters))
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpContInters",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            if (linters.Count>0)
            {
                GErr gerr=new GErrContInters(this.index, linters);
                gerrlist.Add(gerr);
                //this.isOrientDefined=false;
            }
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateSimpContMisor(GErrList gerrlist)
        {
            if ((this.statusGV[(int)DefsGV.TypeGV.ValidateSimpContDupl].StatusRes!=
                StatusGV.TypeStatusRes.NoErrors)||
                (this.statusGV[(int)DefsGV.TypeGV.ValidateSimpContWrap].StatusRes!=
                StatusGV.TypeStatusRes.NoErrors)||
                (this.statusGV[(int)DefsGV.TypeGV.ValidateSimpContInters].StatusRes!=
                StatusGV.TypeStatusRes.NoErrors))
            {
                return StatusGV.TypeStatusExec.UnableToExec;
            }
            if (this.outl==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateSimpContMisor",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            ArrayList pozsContMisor=new ArrayList();
            for (int pozCont=0; pozCont<this.outl.NumCont; pozCont++)
            {
                Contour cont=this.outl.ContourByPoz(pozCont);
                bool isMisoriented;
                if (!cont.IsMisoriented(this.outl, out isMisoriented))
                {
                    throw new ExceptionGlyph("Glyph","ValidateSimpContMisor",null);
                    //return StatusGV.TypeStatusExec.Aborted;
                }
                if (isMisoriented)
                {
                    pozsContMisor.Add(pozCont);
                }
            }

            int numCont=pozsContMisor.Count;
            if (numCont!=0)
            {
                int[] indsKnotStart=new int[numCont];
                for (int iCont=0; iCont<numCont; iCont++)
                {
                    int pozCont=(int)pozsContMisor[iCont];
                    indsKnotStart[iCont]=this.Outl.ContourByPoz(pozCont).IndKnotStart;
                }
                GErr gerr=new GErrContMisor(this.index, indsKnotStart);
                gerrlist.Add(gerr);
            }
            pozsContMisor.Clear();
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateCompBind(GErrList gerrlist)
        {
            /*
             *        ASSUMPTION: -    the glyph is composite and Comp is already
             *                        read
             *                    -    clears outline that is already
             *                        built in case of failure to load one
             *                        of components
             */
            if (this.outl!=null)
            {
                this.outl.ClearDestroy();
                this.outl=null;
            }

            GErr gerr;
            bool isCircular=this.fm.PathCompBind.Contains(this.index);
            if (isCircular)
            {
                int numGlyph=this.fm.PathCompBind.Count;
                int[] pathCompBind=new int[numGlyph+1];
                for (int iGlyph=0; iGlyph<numGlyph; iGlyph++)
                {
                    pathCompBind[iGlyph]=(int)this.fm.PathCompBind[iGlyph];
                }
                pathCompBind[numGlyph]=this.index;
                gerr=new GErrComponentCircularDependency(this.index,
                    this.index, pathCompBind);
                gerrlist.Add(gerr);
                this.fm.PathCompBind.Clear();
                return StatusGV.TypeStatusExec.Completed;
            }
            if (this.outl!=null)
            {
                this.outl.ClearReset();
                this.outl=null;
            }
            if (this.comp==null)
            {
                //throw new ExceptionGlyph("Glyph","ValidateCompBind",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
    
            this.fm.PathCompBind.Add(this.index);

            //throw new ArgumentOutOfRangeException();    


            if (this.comp.NumComponent==0)
            {
                this.outl=new Outline();
                gerr=new GErrComponentEmpty(this.index,GConsts.IND_UNDEFINED);
                gerrlist.Add(gerr);
                return StatusGV.TypeStatusExec.Completed;
            }

            foreach (Component component in this.comp)
            {
                int indGlyphComponent=component.IndexGlyphComponent;
                if ((indGlyphComponent<0)||(indGlyphComponent>=this.fm.FNumGlyph))
                {
                    gerr=new GErrComponentIndexGlyph(this.index,indGlyphComponent);
                    gerrlist.Add(gerr);
                    this.fm.PathCompBind.Clear();
                    if (this.outl!=null)
                    {
                        this.outl.ClearDestroy();
                        this.outl=null;
                    }
                    return StatusGV.TypeStatusExec.Completed;
                }
                Glyph glyphComponent=this.fm.GGet(indGlyphComponent);
                if (glyphComponent==null)
                {
                    gerr=new GErrComponentLoadFailure(this.index,indGlyphComponent);
                    gerrlist.Add(gerr);
                    this.fm.PathCompBind.Clear();
                    if (this.outl!=null)
                    {
                        this.outl.ClearDestroy();
                        this.outl=null;
                    }
                    return StatusGV.TypeStatusExec.Completed;
                }
                if (glyphComponent.typeGlyph==GConsts.TypeGlyph.Empty)
                {
                    gerr=new GErrComponentEmpty(this.index,indGlyphComponent);
                    gerrlist.Add(gerr);
                    continue;
                }
                else
                {
                    if (glyphComponent.Outl==null)
                    {
                        gerr=new GErrComponentLoadFailure(this.index,indGlyphComponent);
                        gerrlist.Add(gerr);
                        this.fm.PathCompBind.Clear();
                        if (this.outl!=null)
                        {
                            this.outl.ClearDestroy();
                            this.outl=null;
                        }
                        return StatusGV.TypeStatusExec.Completed;
                    }
                }

                if (this.outl==null)
                {
                    this.outl=new Outline();
                }
                ArrayList errsLoadComponent;

                if (!this.outl.LoadComponent(component, 
                    glyphComponent.outl, 
                    out errsLoadComponent))
                {
                    this.fm.PathCompBind.Clear();
                    if (this.outl!=null)
                    {
                        this.outl.ClearDestroy();
                        this.outl=null;
                    }
                    //throw new ExceptionGlyph("Glyph","ValidateCompBind",null);
                    return StatusGV.TypeStatusExec.Aborted;
                }
                if (errsLoadComponent!=null)
                {
                    foreach (Component.TypeErrLoadComponent errLoadComponent in errsLoadComponent)
                    {    
                        gerr=null;
                        switch (errLoadComponent)
                        {
                            case Component.TypeErrLoadComponent.IncorrectShiftSpecification:
                                gerr=new GErrComponentIncorrectShift(this.index,
                                    component.IndexGlyphComponent);
                                break;
                            case Component.TypeErrLoadComponent.IncorrectIndexKnotGlyph:
                                gerr=new GErrComponentIndexKnot(this.index,
                                    component.IndexGlyphComponent,
                                    component.IndexKnotAttGlyph,
                                    true);
                                break;
                            case Component.TypeErrLoadComponent.IncorrectIndexKnotComponent:
                                gerr=new GErrComponentIndexKnot(this.index,
                                    component.IndexGlyphComponent,
                                    component.IndexKnotAttComponent,
                                    false);
                                break;
                            case Component.TypeErrLoadComponent.IncorrectTransform:
                                gerr=new GErrComponentIncorrectTransform(this.index,
                                    component.IndexGlyphComponent,
                                    component.TrOTF2Dot14);
                                break;
                        }
                        gerrlist.Add(gerr);
                    }
                    if (this.outl!=null)
                    {
                        this.outl.ClearDestroy();
                        this.outl=null;
                    }
                    this.fm.PathCompBind.Clear();
                    return StatusGV.TypeStatusExec.Completed;
                }
            }
            this.fm.PathCompBind.RemoveAt(this.fm.PathCompBind.Count-1);
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateCompBBox(GErrList gerrlist)
        {
            if ((this.bbox==null)||(this.outl==null))
            {
                //throw new ExceptionGlyph("Glyph","ValidateCompBBox",null);
                return StatusGV.TypeStatusExec.Aborted;    
            }
            BoxD bboxCP;        // bbox for the control points
            bboxCP=this.outl.BBoxCP;
            if (!this.bbox.Equals(bboxCP))
            {
                double deviation=this.bbox.DeviationMax(bboxCP);
                bool toSkipError=((this.outl.IsChangedByRound)&&(Math.Abs(deviation)<=1.0));
                if (!toSkipError)
                {    
                    GErrBBox gerrBbox=new GErrBBox(this.IndexGlyph,
                        this.typeGlyph,
                        this.bbox,bboxCP);
                    gerrlist.Add(gerrBbox);
                }
            }

            BoxD bboxOutl;        // bbox for Outline
            bboxOutl=this.outl.BBox;
            //bboxOutline.SetEnlargeFU();
            if (!bboxOutl.Equals(bboxCP))
            {
                // warning
                GErrExtremeNotOnCurve gerrExtreme=new 
                    GErrExtremeNotOnCurve(this.IndexGlyph,
                    this.typeGlyph,bboxCP,bboxOutl);
                gerrlist.Add(gerrExtreme);

            }
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateCompComponentDupl(GErrList gerrlist)
        {
            ListPairInt pairsIndComponentDupl=new ListPairInt();
        
            if ((this.comp==null)||(this.outl==null))
            {
                //throw new ExceptionGlyph("Glyph","ValidateCompComponentDupl",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            int numComponent=this.comp.NumComponent;
            for (int pozComponentA=0; pozComponentA<numComponent-1; pozComponentA++)
            {
                Component componentA=this.comp.ComponentByPoz(pozComponentA);
                for (int pozComponentB=pozComponentA+1; pozComponentB<numComponent; pozComponentB++)
                {
                    ListInfoInters linters=new ListInfoInters();
                    Component componentB=this.comp.ComponentByPoz(pozComponentB);
                    bool areDuplicated;
                    this.outl.AreDuplicatedComponents(componentA,componentB,out areDuplicated);
                    if (areDuplicated)
                    {
                        pairsIndComponentDupl.Add(componentA.IndexGlyphComponent,
                            componentB.IndexGlyphComponent);
                    }
                }
            }
            if (pairsIndComponentDupl.Count!=0)
            {
                GErr gerr=new GErrComponentDupl(this.index, pairsIndComponentDupl);
                gerrlist.Add(gerr);
            }
            return StatusGV.TypeStatusExec.Completed;
        }

        internal StatusGV.TypeStatusExec ValidateCompComponentInters(GErrList gerrlist)
        {
            ArrayList arrLinters=new ArrayList();
            if ((this.comp==null)||(this.outl==null))
            {
                //throw new ExceptionGlyph("Glyph","ValidateCompComponentInters",null);
                return StatusGV.TypeStatusExec.Aborted;
            }
            int numComponent=this.comp.NumComponent;
            for (int pozComponentA=0; pozComponentA<numComponent-1; pozComponentA++)
            {
                Component componentA=this.comp.ComponentByPoz(pozComponentA);
                for (int pozComponentB=pozComponentA+1; pozComponentB<numComponent; pozComponentB++)
                {
                    ListInfoInters linters=new ListInfoInters();
                    Component componentB=this.comp.ComponentByPoz(pozComponentB);
                    bool areDuplicated;
                    this.outl.AreDuplicatedComponents(componentA,componentB,out areDuplicated);
                    if (areDuplicated)
                        continue;
                    this.outl.IntersectComponents(componentA, componentB, linters);
            
                    if (linters.Count!=0)
                    {
                        linters.SetIndGlyphComponent(componentA.IndexGlyphComponent,
                            componentB.IndexGlyphComponent);
                        arrLinters.Add(linters);
                    }
                }
            }
            if (arrLinters.Count!=0)
            {
                GErr gerr=new GErrComponentInters(this.index, arrLinters);
                gerrlist.Add(gerr);
            }
            return StatusGV.TypeStatusExec.Completed;
        }



        /*
         *        VALIDATION GROUPS & Auxiliary Functions
         */

        public void GValidate()
        {
            bool performGV;
            bool needsActionGV;
            needsActionGV=this.NeedsActionForValidate(DefsGV.TypeGV.ValidateTypeGlyphSource);
            if (needsActionGV)
            {
                this.CallGV(DefsGV.TypeGV.ValidateTypeGlyphSource);
            }
            if (this.typeGlyph==GConsts.TypeGlyph.Undef)
            {
                foreach (StatusGV stGV in this.statusGV)
                {
                    stGV.StatusExec=StatusGV.TypeStatusExec.UnableToExec;
                }
                return;
            }
            if (this.typeGlyph==GConsts.TypeGlyph.Empty)
                return;
            if ((this.TypeGlyph==GConsts.TypeGlyph.Simple)||
                (this.TypeGlyph==GConsts.TypeGlyph.Composite))
            {
                DefsGV.TypeGV[] orderTest=null;
                if (this.typeGlyph==GConsts.TypeGlyph.Simple)
                {
                    orderTest=DefsGV.orderTestSimp;
                }
                if (this.typeGlyph==GConsts.TypeGlyph.Composite)
                {
                    orderTest=DefsGV.orderTestComp;
                }
                FlagsGV flagsGV=this.fm.FlagsPerformGV;
                foreach (DefsGV.TypeGV typeGV in orderTest)
                {
                    performGV=flagsGV[typeGV];
                    needsActionGV=this.NeedsActionForValidate(typeGV);
                    if ((!performGV)||(!needsActionGV))
                        continue;
                    this.CallGV(typeGV);
                }
            }
        }

        public void GResetFromSource()
        {
            /*
             *        MEANING:    -    RESETS glyph from original source
             *                    -    BINDS composite glyphs
             */
            if (this.NeedsActionForReset(DefsGV.TypeGV.ValidateTypeGlyphSource))
            {
                this.CallGV(DefsGV.TypeGV.ValidateTypeGlyphSource);
            }
            switch (this.typeGlyph)
            {
                case GConsts.TypeGlyph.Simple:
                    if (this.NeedsActionForReset(DefsGV.TypeGV.ValidateSimpSource))
                    {
                        this.CallGV(DefsGV.TypeGV.ValidateSimpSource);
                    }
                    break;
                case GConsts.TypeGlyph.Composite:
                    if (this.NeedsActionForReset(DefsGV.TypeGV.ValidateCompSource))
                    {
                        this.CallGV(DefsGV.TypeGV.ValidateCompSource);
                    }
                    if (this.NeedsActionForReset(DefsGV.TypeGV.ValidateCompBind))
                    {
                        this.CallGV(DefsGV.TypeGV.ValidateCompBind);
                    }
                    break;
            }
        }

        
        private bool NeedsActionForValidate(DefsGV.TypeGV typeGV)
        {
            if (DefsGV.IsSourceValidator(typeGV))
            {
                return (this.statusGV[(int)typeGV].StatusExec==
                    StatusGV.TypeStatusExec.NeverExec);
            }
            return (!this.statusGV[(int)typeGV].IsValid);
        }

        private bool NeedsActionForReset(DefsGV.TypeGV typeGV)
        {
            if (DefsGV.IsPureValidator(typeGV))
                return false;
            if ((typeGV==DefsGV.TypeGV.ValidateSimpSource)&&
                (this.typeGlyph==GConsts.TypeGlyph.Composite))
                return false;
            if ((typeGV==DefsGV.TypeGV.ValidateCompSource)&&
                (this.typeGlyph==GConsts.TypeGlyph.Simple))
                return false;
            int ind=(int)typeGV;
            return (!this.statusGV[(int)typeGV].IsValid);
        }




        /*
         *        METHODS:    MODIFY:    GM
         */
        
        public bool ModifyMoveKnot(int indKnot, VecD vecLocNew)
        {
            this.fm.OnGM(this.index,DefsGM.TypeGM.ModifyMoveKnot,true);

            Knot knot=this.outl.KnotByInd(indKnot);
            if (knot!=null)
            {
                knot.Val.From(vecLocNew);
                bool isChanged;
                knot.Val.FURound(out isChanged);
            }
            return true;
        }

        public bool ModifyDeleteKnot(int indKnot)
        {
            this.fm.OnGM(this.index,DefsGM.TypeGM.ModifyDeleteKnot,true);
            this.outl.KnotDeleteByInd(indKnot);
            return true;
        }

        /*
         *        METHODS:    INFO    (all information is copied 
         *                            prior passing to a user)
         */

        public Knot InfoKnotByLocation(VecD vecLocation)
        {
            Knot knotNearestCopy=null;
            if (this.outl!=null)
            {
                Knot knotNearest=this.outl.KnotNearest(vecLocation);
                if (knotNearest!=null)
                {
                    knotNearestCopy=new Knot(knotNearest);
                }
            }
            return knotNearestCopy;
        }

        public Knot InfoKnotByInd(int indKnot)
        {
            Knot knot=this.Outl.KnotByInd(indKnot);
            if (knot==null)
                return null;
            return new Knot(knot);
        }
        /*
         *        METHODS:    I_DRAWABLE
         */
        public void Draw(I_Draw i_draw, DrawParam dp)
        {

            if (this.bbox!=null)
            {
                DrawParam dpBBox=new DrawParam("Yellow",0.5F);
                this.bbox.Draw(i_draw, dpBBox);
            }

            if (this.outl!=null)
            {
                DrawParamKnot dpKnot=new DrawParamKnot("Blue",0.5F,1.5F,true);
                DrawParamVec dpEndPoints=new DrawParamVec("Orange",0.5F,0.7F,true);
                
                string colorCurve;
                if (this.typeGlyph==GConsts.TypeGlyph.Composite)
                {
                    colorCurve="Blue";
                }
                else
                {
                    bool isOrientDefined=
                        (this.statusGV[(int)DefsGV.TypeGV.ValidateSimpContMisor].IsValid)&&
                        (this.statusGV[(int)DefsGV.TypeGV.ValidateSimpContMisor].StatusExec==
                        StatusGV.TypeStatusExec.Completed);
                    colorCurve=isOrientDefined? "Blue": "Green";
                }
                DrawParamCurve dpCurve=new DrawParamCurve(colorCurve,1F,true,dpEndPoints);
                DrawParamContour dpContour=new DrawParamContour(dpCurve,dpKnot);
            
                this.outl.Draw(i_draw, dpContour);

                BoxD bboxComputed=this.outl.BBox;
                bboxComputed.SetEnlargeFU();
                DrawParam dpBBoxComputed=new DrawParam("Yellow",0.5F);
                bboxComputed.Draw(i_draw,dpBBoxComputed);
            }
        }
    }
}