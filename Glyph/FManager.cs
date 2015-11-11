using System;
using System.Collections;
using System.Diagnostics;
using System.IO;
using System.Text;

using NS_ValCommon;
using NS_IDraw;
using NS_GMath;

namespace NS_Glyph
{
    public class FManager
    {
        /*
         *        ENUMS
         */

        public enum TypeActionOnErr
        {
            onAdd,
            onDelete
        }

            
        /*
         *        MEMBERS: management
         */ 
        
        private GStore        gStore;            // Glyphs
        private GErrPool    gerrPool;        // gerrsums
        private GRelations  grel;            
        private ArrayList   indGM;            // indices of modified glyphs
        private ArrayList   pathCompBind;    

        /*
         *        MEMBERS: general font information
         */

        private int            fwNumGlyph;

        /*
         *        MEMBERS: interfaces
         */

        I_IOGlyphs            i_IOGlyphs;         // should be initialized
        I_ProgressUpdater    i_ProgressUpdater;    // can be null

        /*
         *        MEMBERS: flags
         */

        FlagsGV                f_performGV;    // validation flags
        Flags                f_performGC;    // correction flags
        
        
        /*
         *        PROPERTIES PUBLIC
         */ 
        public I_IOGlyphs IIOGlyphs
        {
            get { return this.i_IOGlyphs;}
            set 
            { 
                this.i_IOGlyphs=value;
                if (this.i_IOGlyphs!=null)
                {
                    if (!i_IOGlyphs.ReadNumGlyph(out this.fwNumGlyph,null))
                        this.fwNumGlyph=GConsts.IND_UNDEFINED;
                }
                else
                {
                    this.fwNumGlyph=GConsts.IND_UNINITIALIZED;
                }
            }
        }
        public I_ProgressUpdater IProgressUpdater
        {
            get { return this.i_ProgressUpdater; }
            set { this.i_ProgressUpdater=value; }
        }
        
        /*
         *            FOR DEBUG ONLY
         */
        public void PrintGErrs(int indGlyph)
        {
            this.gerrPool.PrintGErrs(indGlyph);        
        }



        /*
         *            CONSTRUCTORS
         */
        public FManager()
        {
            // storage
            this.gerrPool=new GErrPool();
            this.gStore=new GStore();
            this.grel=new GRelations();

            this.indGM=new ArrayList();
            this.pathCompBind=new ArrayList();
            // font information
            this.fwNumGlyph=GConsts.IND_UNINITIALIZED;
    
            // interfaces
            this.i_IOGlyphs=null;
            
            // flags - which tests shoud be performed
            this.f_performGV=new FlagsGV();
            this.f_performGC=new Flags(typeof(DefsGM.TypeGM));
            this.FlagsSetDefault();

            // glyph as fm holder
            // Glyph glyph=new Glyph(GConsts.IND_UNDEFINED,this);
        }
        


        public FManager(I_IOGlyphs i_IOGlyphs,
            I_ProgressUpdater i_ProgressUpdater, 
            I_TestReader i_TestReader) : 
            this()
        {
            this.i_IOGlyphs=i_IOGlyphs;
            if (this.i_IOGlyphs!=null)
            {
                if (!this.i_IOGlyphs.ReadNumGlyph(out this.fwNumGlyph,null))
                    this.fwNumGlyph=GConsts.IND_UNDEFINED;
            }
                
            this.i_ProgressUpdater=i_ProgressUpdater;
            // TODO: init flags from i_TestReader
        }
                
        /*
         *        PROPERTIES
         */
        public int FNumGlyph
        {
            // TODO: store the number in a local variable
            get 
            {
                return this.fwNumGlyph;
            }
        }

        internal ArrayList PathCompBind
        {
            get { return this.pathCompBind; }
        }


        /*
         *        METHODS: UPDATES
         */
        internal void OnGM(int indGlyph, DefsGM.TypeGM typeGM, bool isCaller)
        {
            Glyph glyph=this.gStore.GGet(indGlyph);
            if ((isCaller)&&(glyph==null))
            {
                throw new ExceptionGlyph("FManager","OnGM","Null argument");
            }
            ArrayList arrCompDep=this.grel.GetCompDep(indGlyph);
            if (arrCompDep!=null)
            {
                foreach (int indCompDep in arrCompDep)
                {
                    this.OnGM(indCompDep, typeGM, false);
                }
            }
            /*
             *        IN ANY CASE
             */
            // clear self-errors on SourceValidator
            if (isCaller && DefsGM.IsSourceValidator(typeGM))
            {
                this.gerrPool.ClearByGV(indGlyph,DefsGV.From(typeGM));
            }

            //    INVALIDATION TABLE:
            //
            //                            Itself              Dependents
            //                       VSrc VBind VPure     VSrc VBind VPure
            //
            //    Validator SOURCE    No     Yes   Yes       No      Yes   Yes
            //
            //    Validator BINDING    No     No       Yes       No   Yes   Yes
            //        
            //    MODIFIER PURE        Yes  Yes   Yes       Yes  Yes   Yes
            //                      |                     |
            //                        BUT: does not clear errors
            //                             VSrc clears its errors by itself

            if (DefsGM.IsSourceValidator(typeGM))
            {
                //    ValidateTypeGlyphSource
                //    ValidateSimpSource
                //    ValidateCompBind
                foreach (DefsGV.TypeGV typeGV in Enum.GetValues(typeof(DefsGV.TypeGV)))
                {
                    if (DefsGV.IsTest(typeGV))
                    {
                        // invalidate & clear errors
                        if (DefsGV.IsSourceValidator(typeGV))
                            continue;
                        glyph.StatusVal(typeGV).IsValid=false;
                        this.gerrPool.ClearByGV(indGlyph,typeGV);    
                    }
                }
            }
            else if (typeGM==DefsGM.TypeGM.ValidateCompBind)
            {
                //    ValidateCompBind
                foreach (DefsGV.TypeGV typeGV in Enum.GetValues(typeof(DefsGV.TypeGV)))
                {
                    if (DefsGV.IsTest(typeGV))
                    {
                        if (DefsGV.IsSourceValidator(typeGV))
                            continue;
                        if (isCaller&&(typeGV==DefsGV.TypeGV.ValidateCompBind))
                            continue;
                        glyph.StatusVal(typeGV).IsValid=false;
                        this.gerrPool.ClearByGV(indGlyph,typeGV);
                    }
                }
            }
            else
            {
                // Pure Modifier
                foreach (DefsGV.TypeGV typeGV in Enum.GetValues(typeof(DefsGV.TypeGV)))
                {
                    if (DefsGV.IsTest(typeGV))
                    {
                        glyph.StatusVal(typeGV).IsValid=false;
                        if (!DefsGV.IsSourceValidator(typeGV))
                        {
                            this.gerrPool.ClearByGV(indGlyph,typeGV);
                        }
                    }
                }
            }
            if (!this.indGM.Contains(indGlyph))
                this.indGM.Add(indGlyph);    
        }

        internal void OnGV(int indGlyph, DefsGV.TypeGV typeGV, GErrList gerrlist)
        {
            Glyph glyph=this.gStore.GGet(indGlyph);
            if (glyph==null)
            {
                throw new ExceptionGlyph("FManager","OnGV",null);
            }
            glyph.StatusVal(typeGV).IsValid=true;
    
            bool areValidAll=true;
            for (int iTest=0; iTest<DefsGV.NumTest; iTest++)
            { 
                if (!glyph.StatusVal((DefsGV.TypeGV)iTest).IsValid)
                {
                    areValidAll=false;
                    break;
                }
            }
            if (areValidAll)
            {
                this.indGM.Remove(indGlyph);
            }
            this.gerrPool.GErrListAdd(gerrlist);
        }

        //internal void OnException()

        
        internal void OnGComposite(int indGlyph)
        {
            Glyph glyph=this.gStore.GGet(indGlyph);
            if (glyph==null)
            {
                throw new ExceptionGlyph("FManager","OnGComposite",null);
            }
            foreach (Component component in glyph.Comp)
            {
                this.grel.AddCompDep(indGlyph, component.IndexGlyphComponent);
            }
        }
        
        
        internal void OnGErrQuery(int indGlyph)
        {
            if ((0<=indGlyph)&&(indGlyph<this.FNumGlyph))
            {
                if (this.indGM.Contains(indGlyph))
                {
                    Glyph glyph=this.gStore.GGet(indGlyph);
                    if (glyph!=null)
                    {
                        glyph.GValidate();
                    }
                }
            }
            if (indGlyph==GConsts.IND_ALL)
            {
                foreach (int ind in this.indGM)
                {
                    this.OnGErrQuery(ind);
                }
            }
        }

        internal void ReportFailure(GErrApplication gerrApp)
        {
            /*
             *        TRY/CATCH FUNCTION
             */
            // TODO: check logic !!!

            try
            {
                this.gerrPool.DIAFunc_AddToPool(gerrApp);
            }
            catch (System.Exception)
            {
            }
        }

        /*
         *            METHODS: PUBLIC
         *            error-reporting interface for external application         
         */
        public bool GErrActionAdd(DIAction dia, TypeActionOnErr flag)
        {
            return (this.gerrPool.DIAAdd(dia, flag));
        }
        public bool GErrActionRemove(DIAction dia, TypeActionOnErr flag)
        {
            return (this.gerrPool.DIARemove(dia, flag));
        }
        public void GErrActionRemoveAll(object obj)
        {
            this.gerrPool.DIARemoveAll(obj);
        }
        public bool GErrSubscribe(object obj, 
            string nameDIAOnErrDispatch, string nameDIAOnErrDelete)
        {
            bool resOnAdd=this.gerrPool.DIAAdd(obj,nameDIAOnErrDispatch,TypeActionOnErr.onAdd);
            bool resOnDelete=this.gerrPool.DIAAdd(obj,nameDIAOnErrDelete,TypeActionOnErr.onDelete);
            return (resOnAdd&&resOnDelete);
        }
        public void GErrUnsubscribe(object obj)
        {
            this.GErrActionRemoveAll(obj);
        }
        
        // get information
        public bool GErrGetInformed(int indGlyph, DIAction diaToApply)
        {
            this.OnGErrQuery(indGlyph);
            return this.gerrPool.Inform(indGlyph, diaToApply);
        }


        /*
         *            METHODS: settings                     
         */
        public FlagsGV FlagsPerformGV
        {
            get { return this.f_performGV; }
        }
        
        private void FlagsSetDefault()
        {
            this.f_performGV.SetAll(true);
            this.f_performGC.SetAll(true);
        }

        /*
         *        METHODS: CLEAR
         */
        public void ClearResetManagementStructs()
        {
            // clear storage
            this.gerrPool.ClearReset();
            this.gStore.ClearReset();
            this.grel.ClearReset();
            // clear common information
            this.fwNumGlyph=GConsts.IND_UNINITIALIZED;
        }
        public void ClearManagementStructs()
        {
            // clear storage
            this.gerrPool.ClearReset();
            this.gStore.ClearReset();
            this.grel.ClearReset();
            this.indGM.Clear();
            this.pathCompBind.Clear();    
        }


        public void ClearDestroy()
        {
            this.gerrPool.ClearDestroy();
            this.gStore.ClearDestroy();
            // clear common information
            this.fwNumGlyph=GConsts.IND_UNINITIALIZED;

            this.gerrPool=null;
            this.gStore=null;

            this.f_performGV.Clear();
            this.f_performGC.Clear();
            this.f_performGV=null;
            this.f_performGC=null;
                
            this.i_IOGlyphs=null;
            this.i_ProgressUpdater=null;
        }

        /*
         *        METHODS: glyph access
         */
        public Glyph GGet(int indGlyph)
        {
            if ((0>indGlyph)||(indGlyph>=this.FNumGlyph))
                return null;
            Glyph glyph=this.gStore.GGet(indGlyph);
            if (glyph==null)
            {
                glyph=new Glyph(indGlyph,this);
                this.gStore.GAdd(glyph);
                glyph.GResetFromSource();    
            }
            return glyph;
        }

        internal ArrayList GGetCompDep(int indGlyph)
        {
            return this.grel.GetCompDep(indGlyph); 
        }
        /*
         *        METHODS: validators
         */
        

        public bool FValidate(int indGlyphMin, int indGlyphMax)
        {
            if (indGlyphMin<0)
                indGlyphMin=0;
            if ((indGlyphMax<0)||(indGlyphMax>=this.FNumGlyph))
                indGlyphMax=this.FNumGlyph-1;
            int indGlyph;
            for (indGlyph=indGlyphMin; indGlyph<=indGlyphMax; indGlyph++)
            {
                Glyph glyph=this.GGet(indGlyph);
                glyph.GValidate();
            
                if (this.i_ProgressUpdater!=null)
                {
                    if (this.i_ProgressUpdater.ToStop)
                    {
                        this.i_ProgressUpdater.UpdateStatus("Validated glyphs: from glyph "+indGlyphMin+" to glyph "+indGlyph);
                        this.i_ProgressUpdater.UpdateProgress(GConsts.IND_UNINITIALIZED);
                        break;
                    }
                    else
                    {
                        this.i_ProgressUpdater.UpdateStatus("Validating Font: from glyph "+indGlyphMin+" to glyph "+indGlyphMax);
                        this.i_ProgressUpdater.UpdateProgress(100.0*(float)(indGlyph-indGlyphMin)/(float)(indGlyphMax-indGlyphMin));            
                        this.i_ProgressUpdater.UpdateProgress(indGlyph);
                    }
                }
            }
            if (indGlyph==indGlyphMax+1)
            {
                if (this.i_ProgressUpdater!=null)
                {
                    this.i_ProgressUpdater.UpdateStatus("Font Validated: from glyph "+indGlyphMin+" to glyph "+indGlyphMax);
                    this.i_ProgressUpdater.UpdateProgress(GConsts.IND_UNINITIALIZED);
                    this.i_ProgressUpdater.TaskCompleted();
                }
            }
            //this.gStore.DumpToFile();
            return true;
        }
    }
}
        
        
        
