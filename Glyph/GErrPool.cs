using System;
using System.Collections;
using System.Diagnostics;
using System.IO;

using NS_ValCommon;
using NS_GMath;


namespace NS_Glyph
{
    /* NOTYPE=-1, INITIALIZER, VALIDATOR, CORRECTOR */
    internal class GErrPool
    {
        /*
         *        MEMBERS: management for external apprication
         */
        private DIActionContainer  dia_OnErrorAdd;
        private DIActionContainer  dia_OnErrorDelete;
        private Hashtable gerrlists;  // storages


        // constructors
        public GErrPool()
        {
            this.dia_OnErrorAdd=new DIActionContainer();
            this.dia_OnErrorDelete=new DIActionContainer();
            this.gerrlists=new Hashtable();
        }

        /*
         *        METODS PRIVATE: accesses
         */

        private GErrList GErrListGet(int indGlyph)
        {
            return (this.gerrlists[indGlyph] as GErrList);

        }

        
        /*
         *            FOR DEBUG ONLY
         */
        internal void PrintGErrs(int indGlyph)
        {
            FileInfo infoFile=new FileInfo("GERRS.txt");
            StreamWriter writer=infoFile.AppendText();
            writer.WriteLine("===> GLYPH "+indGlyph);

            GErrList gerrlist=this.gerrlists[indGlyph] as GErrList;
            
            if (gerrlist!=null)
            {
                writer.WriteLine("NUMBER of ERRORS: "+gerrlist.Length);
            }
            else
            {
                writer.WriteLine("EMPTY LIST");
                writer.Close();
                return;
            }
            foreach (GErr gerr in gerrlist)
            {
                string str=gerr.Write();
                writer.WriteLine("");
                writer.WriteLine("Error: ");
                writer.Write(str);
            }
            writer.Close();

        }




        /*
         *        METHODS: add/remove DIAs
         */

        
        public bool DIAAdd(object obj, string nameDIA, FManager.TypeActionOnErr flag)
        {
            switch (flag)
            {
                case FManager.TypeActionOnErr.onDelete:
                    return (this.dia_OnErrorDelete.AddDeleg(obj, nameDIA));
                case FManager.TypeActionOnErr.onAdd:
                    return (this.dia_OnErrorAdd.AddDeleg(obj, nameDIA));
                default:
                    return false;
            }
        }
        public bool DIAAdd(DIAction dia, FManager.TypeActionOnErr flag)
        {
            switch (flag)
            {
                case FManager.TypeActionOnErr.onDelete:
                    return (this.dia_OnErrorDelete.AddDeleg(dia));
                case FManager.TypeActionOnErr.onAdd:
                    return (this.dia_OnErrorAdd.AddDeleg(dia));
                default:
                    return false;
            }
        }
        public bool DIARemove(DIAction dia, FManager.TypeActionOnErr flag)
        {
            switch (flag)
            {
                case FManager.TypeActionOnErr.onDelete:
                    return (this.dia_OnErrorDelete.RemoveDeleg(dia));
                case FManager.TypeActionOnErr.onAdd:
                    return (this.dia_OnErrorAdd.RemoveDeleg(dia));
                default:
                    return false;
            }
        }
        public void DIARemoveAll(object obj)
        {
            this.dia_OnErrorAdd.RemoveDeleg(obj);
            this.dia_OnErrorDelete.RemoveDeleg(obj);
        }

        /*
         *        METHODS PUBLIC: dumps 
         */
        
        public void GErrListAdd(GErrList gerrlist)
        {
            if (gerrlist==null)
                return;
            gerrlist.ApplyToEach(DIActionBuilder.DIA(this,"DIAFunc_AddToPool"));
        }

        public bool Inform(int indGlyph, DIAction diaToApply)
        {
            bool bRet = true;
            if (indGlyph == GConsts.IND_ALL)
            {
                this.InformAll(diaToApply);
                bRet = true;
            }
            else
            {
                GErrList gerrlist = this.gerrlists[indGlyph] as GErrList;
                if (gerrlist != null)
                {
                    //If some error is of type Error or AppError, then return false
                    bRet &= gerrlist.TestErrors();
                    gerrlist.ApplyToEach(diaToApply);
                }
            }
            return bRet;
        }
        public void InformAll(DIAction diaToApply)
        {
            // get information about ALL GLYPHS
            foreach (DictionaryEntry entry in this.gerrlists)
            {
                GErrList gerrlist=entry.Value as GErrList;
                gerrlist.ApplyToEach(diaToApply);
            }
        }
        /*
         *        METHODS & DIAS: act on single GERR
         */


        internal void DIAFunc_AddToPool(ValInfoBasic info)
        {
            GErr gerr=info as GErr;
            if (gerr==null)
            {
                throw new ExceptionGlyph("GErrPool","DIAFunc_AddToPool",null);
            }
            int indexGlyph=gerr.IndexGlyphOwner;

            if (this.gerrlists[indexGlyph]==null)
            {
                this.gerrlists[indexGlyph]=new GErrList();
            }
            GErrList gerrlist=this.gerrlists[indexGlyph] as GErrList;
            int numGErrBefore=gerrlist.Length;
            ((GErrList)this.gerrlists[indexGlyph]).DIAFunc_AddToListUnique(gerr);
            int numGErrAfter=gerrlist.Length;
            bool isAdded=(numGErrAfter!=numGErrBefore);
            if (isAdded)
            {
                if (this.dia_OnErrorAdd.DIA!=null)
                {
                    this.dia_OnErrorAdd.DIA(gerr);            
                }
            }
        }

        internal void DIAFunc_DeleteFromPool(ValInfoBasic info)
        {
            if (info==null)
                return;
            GErr gerr=info as GErr;
            if (gerr==null)
            {
                throw new ExceptionGlyph("GErrPool","DIAFunc_DeleteFromPool",null);
            }
            if (this.gerrlists[gerr.IndexGlyphOwner]==null)
                return;
            bool isDeleted=((GErrList)(this.gerrlists[gerr.IndexGlyphOwner])).Delete(gerr);
            if (!isDeleted)
            {
                throw new ExceptionGlyph("GErrPool","DIAFunc_DeleteFromPool",null);
            }
            if (this.dia_OnErrorDelete.DIA!=null)
            {
                this.dia_OnErrorDelete.DIA(gerr);
            }
            gerr.ClearDestroy();
        }

        /*
         *        METHODS: clear
         */
    
        internal void ClearByGV(int indGlyph, DefsGV.TypeGV typeGV)
        {
            GErrList gerrlist = this.gerrlists[indGlyph] as GErrList;
            if (gerrlist==null)
                return;
            for (int poz=gerrlist.Length-1; poz>=0; poz--)
            {
                GErr gerr=gerrlist[poz];
                if (GErrSign.DICFunc_IsSignedBy(gerr,typeGV)&&
                    GErrSign.DICFunc_IsSignedBy(gerr,indGlyph))
                {
                    this.DIAFunc_DeleteFromPool(gerr);
                }
            }
        }

        public void ClearReset()
        {
            foreach (DictionaryEntry entry in this.gerrlists)
            {
                GErrList gerrlist=entry.Value as GErrList;
                if (gerrlist!=null)
                {
                    gerrlist.ClearDestroy();
                }
            }
            this.gerrlists.Clear();
        }

        public void ClearDestroy()
        {
            this.ClearReset();
            this.gerrlists=null;
        }
    }
}