using System;
using System.Collections;
using System.Text;
using System.Diagnostics;

using OTFontFile; // OTF2Dot14
using NS_ValCommon;

using NS_IDraw;
using NS_GMath;

using Curve=NS_GMath.I_CurveD;
using BCurve=NS_GMath.I_BCurveD;

namespace NS_Glyph
{
    public class GErr : ValInfoBasic, I_Drawable
    {        
        /*
         *        ENUMS
         */
        public enum TypeGErrSeverity
        {  
            Undef=-1, Low, Middle, High, Fatal // TODO: try to put VAL_UNDEF into Undef
        }
        
        /*
         *        MEMBERS
         */
        protected int indexGlyphOwner=GConsts.IND_UNINITIALIZED;
        protected GConsts.TypeGlyph typeGlyph;
        protected string gscope;        // glyph scope
        protected TypeGErrSeverity severity; 

        protected string signCreator=null;  // creator of the error 

        /*
         *        PROPERTIES 
         */
        public TypeGErrSeverity Severity 
        {
            get { return this.severity; }
            set { this.severity=value; }
        }

        public GConsts.TypeGlyph TypeGlyph
        {
            get { return this.typeGlyph; }
            set { this.typeGlyph=value; }
        }

        public string GetScope
        {
            get { return this.gscope; }
        }

        public int IndexGlyphOwner
        {
            get { return this.indexGlyphOwner; }
        }
        internal string SignCreator
        {
            get { return this.signCreator; }
            set { this.signCreator=value; }
        }

        public string StrTypeName
        {
            get 
            { 
                return "("+this.TypeBasic.ToString()[0]+"): "+
                    this.ValueName+" ";
            }
        }

        public string StrTypeNameSign
        {
            get
            {
                return this.StrTypeName+this.SignCreator+" ";
            }
        }

        override public string ValueUser
        {
            get 
            { 
                return ("Glyph index "+this.indexGlyphOwner);
            }
            set { this.m_StringValueUser=value; }
        }

        /*
         *        METHODS
         */
        protected void SetScope(params GScope.TypeGScope[] scopes)
        {
            this.gscope=GScope.StrGScope(scopes);
        }

        override public bool IsSame(object obj)                        
        {
            return false;
        }

        
        public bool ClearDestroy() // return value indicatetes whether the error was cleared
        {    
            this.ClearFunc();
            this.indexGlyphOwner=GConsts.IND_DESTROYED;
            base.Name = "GERR_UNINITIALIZED";
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            return true;
        }

        protected virtual void ClearFunc()
        {

        }

        public virtual string WriteUserFriendly()
        {
            StringBuilder sb=new StringBuilder("");
            sb.Append(this.IndexGlyphOwner+": ");
            if (this.ValueName!=null)
            {
                sb.Append(this.ValueName);
            }
            return sb.ToString();
        }

        override public string ToString()
        {
            string strIndex=this.IndexGlyphOwner.ToString();
            strIndex=strIndex.PadLeft(7);
            return strIndex;
        }
        
        
        public virtual string WriteSpecific()
        {
            return "";
        }

        public string Write()
        {
            StringBuilder sb=new StringBuilder("\r\n");
            
            sb.Append("Validation Info Type : ");
            sb.Append(Enum.GetName(typeof(ValInfoBasic.ValInfoType),this.TypeBasic));
            sb.Append("\r\n");

            sb.Append("Index Glyph : ");
            sb.Append(this.IndexGlyphOwner);
            sb.Append("\r\n");
    
            sb.Append("Type Glyph : ");
            sb.Append(this.typeGlyph);
            sb.Append("\r\n");

            sb.Append("Error Type : ");
            sb.Append(this.GetType());
            sb.Append("\r\n");
            
            sb.Append("Error Scopes : ");
            sb.Append(this.GetScope);
            sb.Append("\r\n");

            sb.Append("Error Severity :");
            sb.Append(Enum.GetName(typeof(GErr.TypeGErrSeverity),this.Severity));
            sb.Append("\r\n");

            if (this.Name!=null)
            {
                sb.Append("Name : ");
                sb.Append(this.Name);
                sb.Append("\r\n");
            }

            if (this.ValueName!=null)
            {
                sb.Append("Value Name : ");
                sb.Append(this.ValueName);
                sb.Append("\r\n");
            }

            if (this.ValueUser!=null)
            {
                sb.Append("Details : ");
                sb.Append(this.ValueUser);
                sb.Append("\r\n");
            }

            if ((this.m_StringValueUser!=null)&&(this.m_StringValueUser!=""))
            {
                sb.Append("Value User : ");
                sb.Append(this.m_StringValueUser);
                sb.Append("\r\n");
            }

            /*
            if (this.ValueUser!=null)
            {
                sb.Append("Value User : ");
                sb.Append(this.ValueUser);
                sb.Append("\r\n");
            }
            */
            
            if (this.TagPrincipal!=null)
            {
                sb.Append("Tag Principal : ");
                sb.Append((string)(this.TagPrincipal));
                sb.Append("\r\n");
            }
            /*
            if (this.TagRelated!=null)
            {
                sb.Append("TagRelated : ");
                sb.Append((string)(this.TagRelated));
                sb.Append("\r\n");
            }
            */
            sb.Append("Signature of Creator : ");
            sb.Append(this.signCreator);
            sb.Append("\r\n");

            sb.Append(this.WriteSpecific());
            sb.Append("\r\n");

            return sb.ToString();
        }

        public virtual bool Read(string str)
        {
            return true;
        }

        /*
         *        METHODS:    I_DRAWABLE
         */
        public virtual void Draw(I_Draw i_draw, DrawParam dp)
        {
            throw new ExceptionGlyph("GErr","Draw","Pure virtual function");
        }


        //===================== search utilities ===========
        // PARAMETERS:
        //        indexGlyph: IND_ANY      - any index, including IND_INVALID & IND_ALL
        //                    otherwise - exact match
        //        scope:        SEARCH_ANY_               - any scope, including UNDEF_
        //                    simple scope or UNDEF_ - exact match
        //                    complex scope           - indicates whether gerrsum 
        //                                             contains specified error in
        //                                             at least ONE of SUBSCOPES

        internal class DICWrap_HasErrorType
        {
            int indexGlyph;
            GScope.TypeGScope scope;
            System.Type typeGErr;

            public DICWrap_HasErrorType(int indexGlyph, GScope.TypeGScope scope, System.Type typeGErr)
            {
                this.indexGlyph=indexGlyph;
                this.scope=scope;
                this.typeGErr=typeGErr;
            }

            public bool DICFunc(GErr gerr)
            {
                return gerr.HasErrorType(this.indexGlyph,this.scope,this.typeGErr);
            }

            DICond DIC
            {
                get 
                {
                    return (Delegate.CreateDelegate(typeof(DICond),this,"DICFunc")) as DICond;
                }
            }
        }
            
        public virtual bool HasErrorType(int indexGlyph,   
            GScope.TypeGScope scope,   
            System.Type typeGErr)
        {    
            if ((indexGlyph!=GConsts.IND_ANY)&&(indexGlyph!=this.IndexGlyphOwner))
                return false;
            if (!GScope.GScInterfer(scope, this.GetScope))
                return false;
            // traverse GErr hierarchy (if any)
            Type type=this.GetType();
            do
            {
                if (type==typeGErr)
                    return true;
                type=type.BaseType;
            } while (type!=typeof(GErr));
            return false;
        }
        
        public bool HasSeverity(int indexGlyph,   
            GScope.TypeGScope scope,   
            GErr.TypeGErrSeverity severity)
        {    
            if ((indexGlyph!=GConsts.IND_ANY)&&(indexGlyph!=this.IndexGlyphOwner))
                return false;
            if (!GScope.GScInterfer(scope, this.GetScope))
                return false;
            return (severity==this.Severity);
        }
        /*
                static public bool DICFunc_HasTypeExact(ValInfoBasic info, Type typeGErr)
                {
                    GErr gerr=info as GErr;
                    if (gerr==null)
                        return false;
                    return (gerr is typeGErr);
                }
        */
        static public bool DICFunc_HasSeverity(ValInfoBasic info, GErr.TypeGErrSeverity severity)
        {
            GErr gerr=info as GErr;
            if (gerr==null)
                return false;   // has NO ANY severity
            return (gerr.severity==severity);
        }
    }    

    public class DIWrapperSource
    {
        int indGlyphOwner;
        GConsts.TypeGlyph typeGlyph;
        GScope.TypeGScope scope;
    
        public DIWrapperSource(int indGlyphOwner, GConsts.TypeGlyph typeGlyph,
            GScope.TypeGScope scope)
        {
            this.indGlyphOwner=indGlyphOwner;
            this.typeGlyph=typeGlyph;
            this.scope=scope;
        }

        public GErr DIWFunc(ValInfoBasic info)
        {
            return new GErrSource(info,this.indGlyphOwner,this.typeGlyph,this.scope);
        }

        public static implicit operator DIWrapper(DIWrapperSource diwSource)
        {
            return (Delegate.CreateDelegate(typeof(DIWrapper),diwSource,"DIWFunc") as DIWrapper);
        }
    }

    /*==================================================================
     * 
     * 
     *        DERIVED CLASSES
     * 
     * 
     ==================================================================*/
    
    /*===================================================================
     * 
     *        GErrApplication
     * 
     *==================================================================*/


    abstract public class GErrApplication : GErr
    {
        /*
         *        MEANING:    the error is reported in case of an
         *                    UNHANDELED EXCEPTION
         *                    (unspecified failure of the application)
         * 
         */

        protected string strException;

        protected GErrApplication()
        {
        }

        public GErrApplication(int indGlyph,
            GConsts.TypeGlyph typeGlyph,
            Exception exception)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.AppError;
            base.Name = "GERR_APPLICATION";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";

            // GErr
            base.indexGlyphOwner =indGlyph;
            base.severity = GErr.TypeGErrSeverity.High;
            base.SetScope(GScope.TypeGScope._UNDEF_);
            base.typeGlyph=typeGlyph;
            
            // this
            this.strException=this.ExceptionToString(exception);
            GErrSign.Sign(this,DefsGV.TypeGV.Invalid,this.indexGlyphOwner);
        }

        virtual protected string ExceptionToString(Exception exception)
        {
            try
            {
                if (exception==null)
                {
                    return "";
                }
                else if (exception.InnerException!=null) 
                {        
                    string str="Exception: "+exception.InnerException.Message;
                    if (exception.InnerException.StackTrace!=null)
                        str+=exception.InnerException.StackTrace;
                    return str;
                }
                else
                {
                    string str="Exception: "+exception.Message;
                    if (exception.StackTrace!=null)
                        str+=exception.StackTrace;
                    return str;
                }
            }
            catch (Exception)
            {
                return "Unable to write the exception";
            }
        }

        protected override void ClearFunc()
        {
            this.strException=null;
        }

        public override string WriteSpecific()
        {
            return this.strException;
        }

        public override void Draw(I_Draw i_draw, DrawParam dp)
        {
        }
    }

    public class GErrValidation : GErrApplication
    {
        // members 
        private DefsGV.TypeGV typeGV;
        private StatusGV.TypeStatusExec statusExec;

        // properties
        internal DefsGV.TypeGV TypeGV
        {
            get { return this.typeGV; }
        }

        internal StatusGV.TypeStatusExec StatusExec
        {
            get { return this.statusExec; }
        }


        // constructors
        private GErrValidation() 
        {
        }

        public GErrValidation(int indGlyph, 
            GConsts.TypeGlyph typeGlyph,
            DefsGV.TypeGV typeGV,
            StatusGV.TypeStatusExec statusExec,
            Exception exception) :
            base(indGlyph,typeGlyph,exception)
        {
            this.typeGV=typeGV;
            this.statusExec=statusExec;
            switch (statusExec)
            {
                case StatusGV.TypeStatusExec.Aborted:
                    base.m_Type=ValInfoBasic.ValInfoType.AppError;
                    base.Name="GERR_VAL_ABORTED";
                    base.Severity=GErr.TypeGErrSeverity.High;
                    break;
                case StatusGV.TypeStatusExec.UnableToExec:
                    base.m_Type=ValInfoBasic.ValInfoType.AppError;
                    base.Name="GERR_VAL_UNABLE_TO_PERFORM_TEST";
                    base.Severity=GErr.TypeGErrSeverity.Low;
                    break;
                default:        // should not occur
                    break;
            }
            GErrSign.Sign(this,this.typeGV,this.indexGlyphOwner);
        }

        // methods
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Validation: "+this.typeGV+"  \r\n");
            sb.Append("Status Exec: "+this.statusExec+" \r\n");
            sb.Append(base.WriteSpecific());
            return sb.ToString();
        }

        override public string ValueUser
        {
            get 
            { 
                StringBuilder sb=new StringBuilder(base.ValueUser);
                sb.Append(" Test: "+this.typeGV+" ");
                sb.Append(base.WriteSpecific());
                return sb.ToString();
            }
        }
    }

    /*==================================================================
     * 
     *        GErrSource
     * 
     *=================================================================*/

    public class GErrSource : GErr
    {
        /*
         *        CONSTRUCTOR
         */
        public GErrSource(ValInfoBasic info,
            int indGlyphOwner, GConsts.TypeGlyph typeGlyph,
            GScope.TypeGScope scope)
        {
            // ValInfoBasic
            base.TypeBasic = info.TypeBasic;
            base.Name = info.Name;
            base.ValueUser = info.ValueUser;
            base.NameFileErrs = info.NameFileErrs;
            base.NameAsmFileErrs = info.NameAsmFileErrs;
            base.TagPrincipal = info.TagPrincipal;


            // GErr
            base.indexGlyphOwner=indGlyphOwner;
            base.typeGlyph=typeGlyph;
            if (info.TypeBasic==ValInfoBasic.ValInfoType.Error)
            {
                base.severity=GErr.TypeGErrSeverity.Fatal;
            }
            else if (info.TypeBasic==ValInfoBasic.ValInfoType.Warning)
            {
                base.severity=GErr.TypeGErrSeverity.Low;
            }
            else
            {
                base.severity=GErr.TypeGErrSeverity.Undef;
            }
            base.SetScope(scope);
        }        
        /*
         *        METHODS:    I_DRAWABLE
         */
        override public void Draw(I_Draw i_draw, DrawParam dp)
        {
        }
        /*
         *        METHODS:    IsSame
         */
        override public bool IsSame(object obj)                        
        {
            ValInfoBasic info=new ValInfoBasic(this);
            return info.IsSame(obj);
        }


    }

    /*================================================================
     *        GERR:    Bbox
     *==============================================================*/

    public class GErrBBox : GErr
    {
        // is reported when the bounding box given in GLYF table 
        // differs from the bounding box computed from the
        // CONTROL POINTS of the outline

        // members 
        private BoxD bboxCP;
        private BoxD bboxInit;
    

        private GErrBBox() 
        {
        }
        public GErrBBox(int indGlyph, GConsts.TypeGlyph typeGlyph,
            BoxD bboxInit, BoxD bboxCP) 
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.Name = "GERR_BBOX";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";

            // GErr
            base.indexGlyphOwner =indGlyph;
            base.severity = GErr.TypeGErrSeverity.Middle;
            base.SetScope(GScope.TypeGScope._GGB_,GScope.TypeGScope._GGO_);
            base.typeGlyph=typeGlyph;
            
            // this
            this.bboxInit=new BoxD(bboxInit);
            this.bboxCP=new BoxD(bboxCP);
        }

        protected override void ClearFunc()
        {
            this.bboxInit=null;
            this.bboxCP=null;
        }

        override public string ValueUser
        {
            get 
            { 
                int devMax=(int)Math.Round(this.bboxInit.DeviationMax(this.bboxCP));
                StringBuilder sb=new StringBuilder(base.ValueUser);
                string strTypeGlyph=this.typeGlyph.ToString().PadRight(10,' ');
                sb.Append(", "+strTypeGlyph);
                sb.Append(" Maximal deviation="+devMax+"(FU)");
                return sb.ToString();
            }
        }


        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("BBox Init :");
            sb.Append(" xmin="+this.bboxInit.VecMin.X);
            sb.Append(" ymin="+this.bboxInit.VecMin.Y);
            sb.Append(" xmax="+this.bboxInit.VecMax.X);
            sb.Append(" ymax="+this.bboxInit.VecMax.Y);
            sb.Append(" \r\n");
            sb.Append("BBox for the Control Points :");
            sb.Append(" xmin="+this.bboxCP.VecMin.X);
            sb.Append(" ymin="+this.bboxCP.VecMin.Y);
            sb.Append(" xmax="+this.bboxCP.VecMax.X);
            sb.Append(" ymax="+this.bboxCP.VecMax.Y);
            sb.Append(" \r\n");
            return sb.ToString();
        }
        /*
         *        METHODS:    I_DRAWABLE
         */
        override public void Draw(I_Draw i_draw, DrawParam dp)
        {
            if (((object)this.bboxInit==null)||((object)this.bboxCP==null))
                return;
            DrawParam dpBoxInit=new DrawParam("Red",1);
            this.bboxInit.Draw(i_draw,dpBoxInit);
            DrawParam dpBoxCP=new DrawParam("Cyan",1);
            this.bboxCP.Draw(i_draw,dpBoxCP);
        }

    }

    /*================================================================
     *        GERR:    Extreme
     *==============================================================*/

    public class GErrExtremeNotOnCurve : GErr
    {
        // members 
        private BoxD bboxCP;
        private BoxD bboxOutl;
    

        private GErrExtremeNotOnCurve() 
        {
        }
        public GErrExtremeNotOnCurve(int indGlyph, 
            GConsts.TypeGlyph typeGlyph,
            BoxD bboxCP, BoxD bboxOutl) 
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Warning;
            base.Name = "GERR_EXTEREME_NOT_ON_CURVE";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";

            // GErr
            base.indexGlyphOwner =indGlyph;
            base.severity = GErr.TypeGErrSeverity.Middle;
            base.SetScope(GScope.TypeGScope._GGB_,GScope.TypeGScope._GGO_);
            base.typeGlyph=typeGlyph;
            
            // this
            this.bboxCP=new BoxD(bboxCP);
            this.bboxOutl=new BoxD(bboxOutl);
        }

        protected override void ClearFunc()
        {
            this.bboxCP=null;
            this.bboxOutl=null;
        }

        /*
        override public string ValueUser
        {
            get 
            { 
            }
        }
        */

        public override string WriteSpecific()
        {            
            StringBuilder sb=new StringBuilder();

            double xMinCP=this.bboxCP.VecMin.X;
            double yMinCP=this.bboxCP.VecMin.Y;
            double xMaxCP=this.bboxCP.VecMax.X;
            double yMaxCP=this.bboxCP.VecMax.Y;
            double xMinOutl=this.bboxOutl.VecMin.X;
            double yMinOutl=this.bboxOutl.VecMin.Y;
            double xMaxOutl=this.bboxOutl.VecMax.X;
            double yMaxOutl=this.bboxOutl.VecMax.Y;
            
            if (xMinCP<xMinOutl)
            {
                sb.Append(" Left:   by control points="+xMinCP+" by outline="+xMinOutl);
                sb.Append(" \r\n");
            }
            if (yMinCP<yMinOutl)
            {
                sb.Append(" Bottom: by control points="+yMinCP+" by outline="+yMinOutl);
                sb.Append(" \r\n");
            }
            if (xMaxCP>xMaxOutl)
            {
                sb.Append(" Right:  by control points="+xMaxCP+" by outline="+xMaxOutl);
                sb.Append(" \r\n");
            }
            if (yMaxCP>yMaxOutl)
            {
                sb.Append(" Top:    by control points="+yMaxCP+" by outline="+yMaxOutl);
                sb.Append(" \r\n");
            }

            return sb.ToString();
        }
        /*
         *        METHODS:    I_DRAWABLE
         */
        override public void Draw(I_Draw i_draw, DrawParam dp)
        {
            if (((object)this.bboxCP==null)||((object)this.bboxOutl==null))
                return;
            double xMinCP=this.bboxCP.VecMin.X;
            double yMinCP=this.bboxCP.VecMin.Y;
            double xMaxCP=this.bboxCP.VecMax.X;
            double yMaxCP=this.bboxCP.VecMax.Y;
            double xMinOutl=this.bboxOutl.VecMin.X;
            double yMinOutl=this.bboxOutl.VecMin.Y;
            double xMaxOutl=this.bboxOutl.VecMax.X;
            double yMaxOutl=this.bboxOutl.VecMax.Y;

            SegD seg;
            DrawParamCurve dpCurve;
            if (xMinCP<xMinOutl)
            {
                seg=new SegD(new VecD(xMinOutl,yMinCP),
                    new VecD(xMinOutl,yMaxCP));
                dpCurve=new DrawParamCurve("Orange",1.5F,false,null);
                seg.Draw(i_draw,dpCurve);
            }
            if (yMinCP<yMinOutl)
            {
                seg=new SegD(new VecD(xMinCP,yMinOutl),
                    new VecD(xMaxCP,yMinOutl));
                dpCurve=new DrawParamCurve("Orange",1.5F,false,null);
                seg.Draw(i_draw,dpCurve);
            }
            if (xMaxCP>xMaxOutl)
            {
                seg=new SegD(new VecD(xMaxOutl,yMinCP),
                    new VecD(xMaxOutl,yMaxCP));
                dpCurve=new DrawParamCurve("Orange",1.5F,false,null);
                seg.Draw(i_draw,dpCurve);
            }
            if (yMaxCP>yMaxOutl)
            {
                seg=new SegD(new VecD(xMinCP,yMaxOutl),
                    new VecD(xMaxCP,yMaxOutl));
                dpCurve=new DrawParamCurve("Orange",1.5F,false,null);
                seg.Draw(i_draw,dpCurve);
            }
        }
    }

    /*================================================================
     *        GERR:    ContMisor
     *==============================================================*/

    public class GErrContMisor : GErr
    {
        // members
        private int[] indsKnotStart;

        // properties
        public int NumContMisor
        {
            get { return this.indsKnotStart.Length; }
        }
        // constructors
        private GErrContMisor() 
        {
        }
        public GErrContMisor(int indGlyph, int[] indsKnotStart) 
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.Name = "GERR_CONT_MISOR";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
    
            // GErr
            base.indexGlyphOwner = indGlyph;
            base.typeGlyph=GConsts.TypeGlyph.Simple;
            base.severity = GErr.TypeGErrSeverity.High;
            base.SetScope(GScope.TypeGScope._GGO_);
            
            // this
            int numCont=indsKnotStart.Length;
            this.indsKnotStart=new int[numCont];
            for (int iCont=0; iCont<numCont; iCont++)
            {
                this.indsKnotStart[iCont]=indsKnotStart[iCont];
            }
        }
        /*
         *        METHODS
         */
        public int IndKnotStart(int poz)
        {
            if ((poz<0)||(poz>=this.NumContMisor))
                return GConsts.IND_UNDEFINED;
            return ((int)this.indsKnotStart[poz]);
        }

        protected override void ClearFunc()
        {
            this.indsKnotStart=null;
        }

        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Indices Knot Start : ");
            for (int poz=0; poz<this.NumContMisor; poz++)
            {
                sb.Append(this.indsKnotStart[poz]+" ");
            }
            sb.Append(" \r\n");
            
            return sb.ToString();
        }
        /*
         *        METHODS:    I_DRAWABLE
         */
        override public void Draw(I_Draw i_draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            foreach (int indKnotStart in this.indsKnotStart)
            {
                Contour cont=dpGErr.G.Outl.ContByIndKnot(indKnotStart);
                DrawParamCurve dpCurve=new DrawParamCurve("Magenta",1.5F,false,null);
                DrawParamContour dpCont=new DrawParamContour(dpCurve,null);
                cont.Draw(i_draw, dpCont);
            }
        }
    }


    /*================================================================
     *        GERR:    ContWrap
     *==============================================================*/

    public class GErrContWrap : GErr
    {
        // members
        private int[] indsKnotStart;
        // properties

        // constructors
        private GErrContWrap() 
        {
        }
        public GErrContWrap(int indGlyphOwner, int[] indsKnotStart)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.Name = "GERR_CONT_WRAP";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
    
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Simple;
            base.severity = GErr.TypeGErrSeverity.High;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            int numCont=indsKnotStart.Length;
            this.indsKnotStart=new int[numCont];
            for (int iCont=0; iCont<numCont; iCont++)
            {
                this.indsKnotStart[iCont]=indsKnotStart[iCont];
            }
        }

        protected override void ClearFunc()
        {
            this.indsKnotStart=null;
        }
        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            int numCont=this.indsKnotStart.Length;
            for (int iCont=0; iCont<numCont; iCont++)
            {
                Contour cont=dpGErr.G.Outl.ContByIndKnot(indsKnotStart[iCont]);
                DrawParamCurve dpCurve=new DrawParamCurve("Pink",2.0F,false,null);
                DrawParamContour dpCont=new DrawParamContour(dpCurve,null);
                cont.Draw(i_Draw, dpCont);
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            int numCont=this.indsKnotStart.Length;
            sb.Append("Number of wrapped contours: "+numCont+" \r\n");
            sb.Append("Starting knots of contours: ");
            for (int iCont=0; iCont<numCont; iCont++)
            {
                sb.Append(this.indsKnotStart[iCont]+" ");
            }
            sb.Append(" \r\n");
            return sb.ToString();
        }

    }

    /*================================================================
     *        GERR:    ContWrap
     *==============================================================*/

    public class GErrContDegen : GErr
    {
        // members
        private int[] indsKnotStart;
        // properties

        // constructors
        private GErrContDegen() 
        {
        }
        public GErrContDegen(int indGlyphOwner, int[] indsKnotStart)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Warning;
            base.Name = "GERR_CONT_DEGEN";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
    
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Simple;
            base.severity = GErr.TypeGErrSeverity.Low;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            int numCont=indsKnotStart.Length;
            this.indsKnotStart=new int[numCont];
            for (int iCont=0; iCont<numCont; iCont++)
            {
                this.indsKnotStart[iCont]=indsKnotStart[iCont];
            }
        }

        protected override void ClearFunc()
        {
            this.indsKnotStart=null;
        }
        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            int numCont=this.indsKnotStart.Length;
            for (int iCont=0; iCont<numCont; iCont++)
            {
                Contour cont=dpGErr.G.Outl.ContByIndKnot(indsKnotStart[iCont]);
                Knot knot=cont.KnotByPoz(0);
                i_Draw.DrawPnt(knot.Val.X,knot.Val.Y,4.5F,"Green",1F,false);
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            int numCont=this.indsKnotStart.Length;
            sb.Append("Number of degenerated contours: "+numCont+" \r\n");
            sb.Append("Starting knots of contours: ");
            for (int iCont=0; iCont<numCont; iCont++)
            {
                sb.Append(this.indsKnotStart[iCont]+" ");
            }
            sb.Append(" \r\n");
            return sb.ToString();
        }

    }

    /*================================================================
     *        GERR:    ContDupl
     *==============================================================*/

    public class GErrContDupl : GErr
    {
        // members
        private ListPairInt pairsIndKnotStart;
        // properties

        // constructors
        private GErrContDupl() 
        {
        }
        public GErrContDupl(int indGlyphOwner, ListPairInt pairsIndKnotStart)
        {
            if (pairsIndKnotStart==null)
            {
                throw new ExceptionGlyph("GErrContDupl","GErrContDupl","Null argument");
            }
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.Name = "GERR_CONT_DUPL";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
    
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Simple;
            base.severity = GErr.TypeGErrSeverity.High;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            this.pairsIndKnotStart=pairsIndKnotStart;
        }

        protected override void ClearFunc()
        {
            this.pairsIndKnotStart.Clear();
            this.pairsIndKnotStart=null;
        }

        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            foreach (PairInt pair in this.pairsIndKnotStart)
            {
                Contour cont=dpGErr.G.Outl.ContByIndKnot(pair[0]);
                DrawParamCurve dpCurve=new DrawParamCurve("Orange",2.0F,false,null);
                DrawParamContour dpCont=new DrawParamContour(dpCurve,null);
                cont.Draw(i_Draw, dpCont);
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Number of duplicated contours: "+this.pairsIndKnotStart.Count+" \r\n");
            sb.Append("Starting knots of contours: ");
            foreach (PairInt pair in this.pairsIndKnotStart)
            {
                sb.Append("("+pair[0]+","+pair[1]+") ");
            }
            sb.Append(" \r\n");
            return sb.ToString();
        }

    }

    /*================================================================
     *        GERR:    ContInters
     *==============================================================*/

    public class GErrContInters : GErr
    {
        /*
         *        IMPORTANT:    DO NOT write property that gains access to
         *                    knot (in parameter description) in elements of
         *                    linters
         */

        // members
        private ListInfoInters linters;
        // properties

        // constructors
        private GErrContInters() 
        {
        }
        public GErrContInters(int indGlyphOwner, ListInfoInters linters)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.Name = "GERR_CONT_INTERS";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
    
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Simple;
            base.severity = GErr.TypeGErrSeverity.High;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            this.linters = linters;
        }
        // methods
        override public string ToString()
        {
            string strIndex=this.IndexGlyphOwner.ToString();
            string strRes;
            if (this.linters==null)
            {
                throw new ExceptionGlyph("GErrContInters","ToString",null);
            }    
            if (this.linters.ContainsD1)
            {
                strRes="D1 "+strIndex.PadLeft(6);
            }
            else if (this.linters.ContainsBezSI)
            {
                strRes="Bez "+strIndex.PadLeft(5);
            }
            else
            {    
                strRes=base.ToString();
            }
            
            return strRes;
        }

        protected override void ClearFunc()
        {
            this.linters.ClearDestroy();
            this.linters=null;
        }

        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            foreach (InfoInters inters in this.linters)
            {
                switch(inters.GetType().ToString())
                {
                    case "NS_GMath.IntersD0":
                        IntersD0 intersD0=inters as IntersD0;
                        double x=intersD0.PntInters.X;
                        double y=intersD0.PntInters.Y;
                        i_Draw.DrawPnt(x,y,1.5F,"Red",1,true);
                        break;
                    case "NS_GMath.IntersD1":
                        IntersD1 intersD1=inters as IntersD1;
                        Curve curveInters=intersD1.CurveInters;
                        DrawParamCurve dpCurve;
                        if (!intersD1.IsBezSI)
                        {
                            dpCurve=new DrawParamCurve("Red",1.5F,false,null);
                        }
                        else
                        {
                            dpCurve=new DrawParamCurve("DarkMagenta",5.0F,false,null);
                        }
                        bool toDrawCurves=false;//(curveInters.BBox.Diag<3.0);
                        try
                        {
                            curveInters.Draw(i_Draw, dpCurve);
                        }
                        catch (System.Exception)
                        {    
                            toDrawCurves=true;
                        }
                        
                        if (toDrawCurves)
                        {
                            CParam cparamInA=intersD1.IpiIn.Par(0) as CParam;
                            BCurve curve=dpGErr.G.Outl.CurveByIndKnot(cparamInA.IndKnot);
                            dpCurve=new DrawParamCurve("Red",1.0F,false,null);
                            curve.Draw(i_Draw, dpCurve);

                            CParam cparamInB=intersD1.IpiIn.Par(1) as CParam;
                            if (cparamInB!=null)
                            {
                                curve=dpGErr.G.Outl.CurveByIndKnot(cparamInA.IndKnot);
                                curve.Draw(i_Draw, dpCurve);
                            }
                        }
                        break;
                    default:
                        break;
                }
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            int numInters=this.linters.Count;
            sb.Append("Number of intersections: "+numInters+" \r\n");
            sb.Append("Intersections: \r\n");
            foreach (InfoInters inters in this.linters)
            {

                if (inters.Dim==InfoInters.TypeDim.Dim0)
                {
                    IntersD0 intersD0=inters as IntersD0;
                    int indKnot0=(intersD0.Ipi.Par(0) as CParam).IndKnot;
                    int indKnot1=(intersD0.Ipi.Par(1) as CParam).IndKnot;
                    sb.Append("D0,knots: ("+indKnot0+","+indKnot1+") \r\n");
                }
                if (inters.Dim==InfoInters.TypeDim.Dim1)
                {
                    IntersD1 intersD1=inters as IntersD1;
                    if (intersD1.IsBezSI)
                    {
                        int indKnot=(intersD1.IpiIn.Par(0) as CParam).IndKnot;
                        sb.Append("BezSI,knot: "+indKnot+ "\r\n");
                    }
                    else
                    {
                        int indKnot0=(intersD1.IpiIn.Par(0) as CParam).IndKnot;
                        int indKnot1=(intersD1.IpiIn.Par(1) as CParam).IndKnot;
                        sb.Append("D1,knots: ("+indKnot0+","+indKnot1+") \r\n");
                    }
                }
            }
            return sb.ToString();
        }
    }


    public class GErrKnotDupl : GErr
    {
        // members
        private ListPairInt infosKnotDupl;
        // properties

        // constructors
        private GErrKnotDupl() 
        {
        }
        public GErrKnotDupl(int indGlyphOwner, ListPairInt infosKnotDupl)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Warning;
            base.Name = "GERR_KNOT_DUPL";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
        
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Simple;
            base.severity = GErr.TypeGErrSeverity.Middle;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            this.infosKnotDupl = infosKnotDupl;
        }
        protected override void ClearFunc()
        {
            this.infosKnotDupl.Clear();
            this.infosKnotDupl=null;
        }

        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            foreach (PairInt infoKnotDupl in this.infosKnotDupl)
            {
                VecD vec=dpGErr.G.Outl.KnotByInd(infoKnotDupl[0]).Val;
                i_Draw.DrawPnt(vec.X,vec.Y,3.0F,"Green",0.5F,false);
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Number of duplicated knots: "+this.infosKnotDupl.Count+"\r\n");
            foreach (PairInt infoKnotDupl in this.infosKnotDupl)
            {
                sb.Append("   Knot: "+infoKnotDupl[0]+"  multiplicity: "+infoKnotDupl[1]+" \r\n");
            }
            return sb.ToString();
        }
    }



    /*================================================================
     *        GERR:    COMPOSITE BINDING
     *==============================================================*/
    public class GErrComponent : GErr    // gerr composite binding
    {
        // members
        protected int indexGlyphComponent;

        // properties
        public int IndexGlyphComponent
        {
            get { return this.indexGlyphComponent; }
        }
        override public string ValueUser
        {
            get 
            { 
                return ("Glyph index "+this.indexGlyphOwner+
                    ", Component index "+this.indexGlyphComponent);
            }
            set { this.m_StringValueUser=value; }
        }

        // constructors
        protected GErrComponent() 
        {
        }

        protected GErrComponent(int indGlyphOwner,
            int indGlyphComponent)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.ValueUser = null;
            
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
        
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.severity = GErr.TypeGErrSeverity.Fatal;
            base.SetScope(GScope.TypeGScope._GGC_,GScope.TypeGScope._GGO_);
            base.typeGlyph=GConsts.TypeGlyph.Composite;

            // this
            this.indexGlyphComponent=indGlyphComponent;
        }
        
        // methods
        protected override void ClearFunc()
        {
            this.indexGlyphComponent=GConsts.IND_DESTROYED;
        }

        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
        }

        override public string WriteSpecific()
        {
            return ("Index Component: "+this.indexGlyphComponent+" \r\n");
        }

    }

    public class GErrComponentIndexGlyph : GErrComponent
    {        
        // constructors
        private GErrComponentIndexGlyph() 
        {
        }
        public GErrComponentIndexGlyph(int indGlyphOwner, int indGlyphComponent)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_INDEX_GLYPH";
            base.TypeBasic = ValInfoType.Warning;
            
            // GErr
            base.severity = GErr.TypeGErrSeverity.Low;    
        }
    }        

    public class GErrComponentLoadFailure : GErrComponent
    {
        // constructors
        private GErrComponentLoadFailure() 
        {
        }
        public GErrComponentLoadFailure(int indGlyphOwner, int indGlyphComponent)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_LOAD_FAILURE";
        }
    }

    public class GErrComponentIncorrectShift : GErrComponent
    {
        // constructors
        private GErrComponentIncorrectShift() 
        {
        }
        public GErrComponentIncorrectShift(int indGlyphOwner, int indGlyphComponent)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_INCORRECT_SHIFT";
        }
    }


    public class GErrComponentIncorrectTransform : GErrComponent
    {
        // members
        OTF2Dot14[,] tr;

        // constructors
        private GErrComponentIncorrectTransform() 
        {
        }
        public GErrComponentIncorrectTransform(int indGlyphOwner, int indGlyphComponent,
            OTF2Dot14[,] tr)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_INCORRECT_TRANSFORM";
            // this
            if ((tr.GetLength(0)!=2)||(tr.GetLength(1)!=2))
            {
                throw new ExceptionGlyph("GErrComponentIncorrectTransform","GErrComponentIncorrectTransform",null);
            }
            this.tr=new OTF2Dot14[2,2];
            for (int i=0; i<2; i++)
                for (int j=0; j<2; j++)
                    this.tr[i,j]=tr[i,j];
        }
        override public string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Transform: \r\n");
            for (int i=0; i<2; i++)
                for (int j=0; j<2; j++)
                {
                    sb.Append("("+i+","+j+") "+(double)this.tr[i,j]+" \r\n");
                }
            return sb.ToString();
        }

    }


    public class GErrComponentCircularDependency : GErrComponent
    {
        // members
        int[] pathCompBind;

        // constructors
        private GErrComponentCircularDependency() 
        {
        }
        public GErrComponentCircularDependency(int indGlyphOwner, 
            int indGlyphComponent,
            params int[] pathCompBind)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_CIRCULAR_DEPENDENCY";
            // this
            if (pathCompBind==null)
            {
                throw new ExceptionGlyph("GErrComponentCircularDependency","GErrComponentCircularDependency","Null argument");
            }
            int numGlyph=pathCompBind.Length;
            this.pathCompBind=new int[numGlyph];
            for (int iGlyph=0; iGlyph<numGlyph; iGlyph++)
            {
                this.pathCompBind[iGlyph]=pathCompBind[iGlyph];
            }
        }
        override public string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Glyphs sequence in composite binding: \r\n");
            for (int iGlyph=0; iGlyph<this.pathCompBind.Length; iGlyph++)
            {
                sb.Append(iGlyph+", ");
            }
            sb.Append(" \r\n");
            return sb.ToString();
        }

    }


    public class GErrComponentEmpty : GErrComponent
    {
        // constructors
        private GErrComponentEmpty() 
        {
        }
        public GErrComponentEmpty(int indGlyphOwner, int indGlyphComponent)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_EMPTY";
        }
    }

    public class GErrComponentIndexKnot : GErrComponent
    {
        // members
        int indexKnot;
        bool isKnotGlyph;

        // properties
        int IndexKnot
        {
            get { return this.indexKnot; }
        }
        bool IsKnotGlyph
        {
            get { return this.isKnotGlyph; }
        }

        // constructors
        private GErrComponentIndexKnot() 
        {
        }
        public GErrComponentIndexKnot(int indGlyphOwner, 
            int indGlyphComponent,
            int indKnot,
            bool isKnotGlyph)
            : base(indGlyphOwner, indGlyphComponent)
        {
            // ValInfoBasic
            base.Name = "GERR_COMPONENT_INDEX_KNOT";
            
            // this
            this.indexKnot=indKnot;
            this.isKnotGlyph=isKnotGlyph;
        }
        override public string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Glyph Index of Component"+this.indexGlyphComponent+"\r\n");
            if (this.isKnotGlyph)
            {
                sb.Append("Index of knot in Glyph: "+this.indexKnot+" \r\n");
            }
            else 
            {
                sb.Append("Index of knot in Component: "+this.indexKnot+" \r\n");
            }
            return sb.ToString();
        }

    }


    /*================================================================
     *        GERR:    ComponentInters
     *==============================================================*/

    public class GErrComponentInters : GErr
    {
        // members
        private ArrayList arrLinters;
        // properties

        // constructors
        private GErrComponentInters() 
        {
        }
        public GErrComponentInters(int indGlyphOwner, ArrayList arrLinters)
        {
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Warning;
            base.Name = "GERR_COMPONENT_INTERS";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";
        
            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Composite;
            base.severity = GErr.TypeGErrSeverity.Low;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            this.arrLinters = arrLinters;
        }
        protected override void ClearFunc()
        {
            for (int iLinters=0; iLinters<this.arrLinters.Count; iLinters++)
            {
                ListInfoInters linters=this.arrLinters[iLinters] as ListInfoInters;
                linters.ClearDestroy();
            }
            this.arrLinters.Clear();
            this.arrLinters=null;
        }

        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            foreach (ListInfoInters linters in this.arrLinters)
            {
                foreach (InfoInters inters in linters)
                {
                    switch(inters.GetType().ToString())
                    {
                        case "NS_GMath.IntersD0":
                            IntersD0 intersD0=inters as IntersD0;
                            double x=intersD0.PntInters.X;
                            double y=intersD0.PntInters.Y;
                            i_Draw.DrawPnt(x,y,2.0F,"Red",1,true);
                            break;
                        case "NS_GMath.IntersD1":
                            IntersD1 intersD1=inters as IntersD1;
                            Curve curveInters=intersD1.CurveInters;
                            DrawParamCurve dpCurve=new DrawParamCurve("Red",2.0F,false,null);
                            bool toDrawCurves=(curveInters.BBox.Diag<3.0);
                            try
                            {
                                curveInters.Draw(i_Draw, dpCurve);
                            }
                            catch(System.Exception)
                            {    
                                toDrawCurves=true;
                            }
                        
                            if (toDrawCurves)
                            {
                                CParam cparamInA=intersD1.IpiIn.Par(0) as CParam;
                                BCurve curve=dpGErr.G.Outl.CurveByIndKnot(cparamInA.IndKnot);
                                dpCurve=new DrawParamCurve("Red",2.0F,false,null);
                                curve.Draw(i_Draw, dpCurve);

                                CParam cparamInB=intersD1.IpiIn.Par(1) as CParam;
                                if (cparamInB!=null)
                                {
                                    curve=dpGErr.G.Outl.CurveByIndKnot(cparamInA.IndKnot);
                                    curve.Draw(i_Draw, dpCurve);
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Number of pairs of intersecting components: "+this.arrLinters.Count+" \r\n");
            foreach (ListInfoInters linters in this.arrLinters)
            {
                sb.Append("Indices of intersecting components: ("+
                    linters.IndGlyphComponent(0)+","+
                    linters.IndGlyphComponent(1)+") \r\n");
                sb.Append("Intersections: \r\n");
                foreach (InfoInters inters in linters)
                {
                    if (inters.Dim==InfoInters.TypeDim.Dim0)
                    {
                        IntersD0 intersD0=inters as IntersD0;
                        int indKnot0=(intersD0.Ipi.Par(0) as CParam).IndKnot;
                        int indKnot1=(intersD0.Ipi.Par(1) as CParam).IndKnot;
                        sb.Append("D0,knots: ("+indKnot0+","+indKnot1+") \r\n");
                    }
                    if (inters.Dim==InfoInters.TypeDim.Dim1)
                    {
                        IntersD1 intersD1=inters as IntersD1;
                        if (intersD1.IsBezSI)
                        {
                            int indKnot=(intersD1.IpiIn.Par(0) as CParam).IndKnot;
                            sb.Append("BezSI,knot: "+indKnot+" \r\n");
                        }
                        else
                        {
                            int indKnot0=(intersD1.IpiIn.Par(0) as CParam).IndKnot;
                            int indKnot1=(intersD1.IpiIn.Par(1) as CParam).IndKnot;
                            sb.Append("D1,knots: ("+indKnot0+","+indKnot1+") \r\n");
                        }
                    }
                }
            }
            return sb.ToString();
        }
    }

    /*================================================================
     *        GERR:    ComponentDupl
     *==============================================================*/

    public class GErrComponentDupl : GErr
    {
        // members
        private ListPairInt pairsIndGlyphComponent;
        // properties

        // constructors
        private GErrComponentDupl() 
        {
        }
        public GErrComponentDupl(int indGlyphOwner, ListPairInt pairsIndGlyphComponent)
        {
            if (pairsIndGlyphComponent==null)
            {
                throw new ExceptionGlyph("GErrComponentDupl","GErrComponentDupl","Null argument");
            }
            // ValInfoBasic
            base.TypeBasic = ValInfoType.Error;
            base.Name = "GERR_COMPONENT_DUPL";
            base.ValueUser = null;
            base.NameFileErrs = GErrConsts.FILE_RES_GERR_STRINGS;
            base.NameAsmFileErrs = GErrConsts.ASM_RES_GERR_STRINGS;
            base.TagPrincipal = "glyf";

            // GErr
            base.indexGlyphOwner = indGlyphOwner;
            base.typeGlyph = GConsts.TypeGlyph.Composite;
            base.severity = GErr.TypeGErrSeverity.High;
            base.SetScope(GScope.TypeGScope._GGO_);

            // this
            this.pairsIndGlyphComponent=pairsIndGlyphComponent;
        }

        protected override void ClearFunc()
        {
            this.pairsIndGlyphComponent.Clear();
            this.pairsIndGlyphComponent=null;
        }

        override public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamGErr dpGErr=dp as DrawParamGErr;
            if (dpGErr==null)
                return;
            foreach (PairInt pair in this.pairsIndGlyphComponent)
            {
                int indGlyph=pair[0];
                Component component=dpGErr.G.Comp.ComponentByIndGlyph(indGlyph);
                for (int pozCont=component.PozContStart; 
                    pozCont<component.PozContStart+component.NumCont;
                    pozCont++)
                {
                    Contour cont=dpGErr.G.Outl.ContourByPoz(pozCont);
                    DrawParamCurve dpCurve=new DrawParamCurve("Red",2.0F,false,null);
                    DrawParamContour dpCont=new DrawParamContour(dpCurve,null);
                    cont.Draw(i_Draw, dpCont);
                }
            }
        }
        public override string WriteSpecific()
        {
            StringBuilder sb=new StringBuilder();
            sb.Append("Number of pairs of duplicated components: "+this.pairsIndGlyphComponent.Count+" \r\n");
            sb.Append("Indices of duplicated components: ");
            foreach (PairInt pair in this.pairsIndGlyphComponent)
            {
                sb.Append("("+pair[0]+","+pair[1]+") ");
            }
            sb.Append(" \r\n");
            return sb.ToString();
        }

    }
}

