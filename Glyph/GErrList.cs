using System;
using System.Collections;
using System.Diagnostics;


using NS_ValCommon;
//using VIDelegate=NS_ValCommon.ValidatorBasic.ValInfoDelegate;

namespace NS_Glyph
{
    public class GErrList : IEnumerable
    {
        // members
        protected ArrayList gerrs;
        protected DIWrapper diw;
        /*
         *        PROPERTIES
         */
        public int Length
        {
            get { return this.gerrs.Count; }
        }
        public DIWrapper DIW
        {
            get { return this.diw; }
            set { this.diw=value; }
        }
        // constructors
        public GErrList()
        {
            this.gerrs=new ArrayList();
            this.diw=null;
        }
        /*
         *        METHODS
         */
        public bool Add(GErr gerrToAdd)
        {
            if (gerrToAdd==null)
            {
                throw new ExceptionGlyph("GErrList","Add","Null argument");
            }
            int numErrBefore=this.gerrs.Count;
            this.gerrs.Add(gerrToAdd);
            int numErrAfter=this.gerrs.Count;
            return (numErrAfter>numErrBefore);    
        }
        public bool Delete(GErr gerrToDelete)
        {
            int numErrBefore=this.gerrs.Count;
            int poz;
            for (poz=0; poz<numErrBefore; poz++)
            {
                if ((object)this.gerrs[poz]==(object)gerrToDelete)
                    break;
            }
            if (poz==numErrBefore)
                return false;
            this.gerrs.RemoveAt(poz);
            int numErrAfter=this.gerrs.Count;
            return (numErrAfter<numErrBefore);
        }
        public void ClearRelease()
        {
            this.gerrs.Clear();
            this.gerrs=null;
        }
        internal void ClearDestroy()
        {
            foreach (GErr gerr in this.gerrs)
            {
                gerr.ClearDestroy();
            }
            this.gerrs.Clear();
            this.gerrs=null;
            this.diw=null;
        }
        public GErr this [int poz]
        {
            get 
            {
                if ((poz<0)||(poz>=this.gerrs.Count))
                    return null;
                return (this.gerrs[poz] as GErr);
            }
        }
        public IEnumerator GetEnumerator() // IEnumerable
        {
            return this.gerrs.GetEnumerator();
        }

        public void DIAFunc_AddToList(ValInfoBasic info)
        {
            if (info==null)
                return;
            GErr gerr;
            gerr=info as GErr;
            if ((gerr==null)&&(this.diw!=null))
            {
                gerr=diw(info);
            }
            this.Add(gerr);
        }

        public void DIAFunc_AddToListUnique(ValInfoBasic infoNew)
        {
            if (infoNew==null)
                return;
            GErr gerrNew=infoNew as GErr;
            if (gerrNew==null)
            {
                if (this.diw!=null)
                {
                    gerrNew=diw(infoNew);
                }
                else
                    return;
            }            
            foreach (GErr gerr in this.gerrs)
            {
                if (gerrNew.IsSame(gerr))
                    return;
            }
            this.Add(gerrNew);
        }
        
        public void ApplyToEach(DIAction dia)
        {
            if (dia!=null)
            {
                foreach (GErr gerr in this.gerrs)
                {
                    dia(gerr);
                }
            }
        }

        public StatusGV.TypeStatusRes StatusResGV(DefsGV.TypeGV typeGV)
        {
            StatusGV.TypeStatusRes statusCur=StatusGV.TypeStatusRes.NoErrors;
            foreach (GErr gerr in this.gerrs)
            {
                if (GErrSign.DICFunc_IsSignedBy(gerr, typeGV))
                {
                    if (gerr.TypeBasic==ValInfoBasic.ValInfoType.Error)
                    {
                        return StatusGV.TypeStatusRes.Errors;
                    }
                    else if (gerr.TypeBasic==ValInfoBasic.ValInfoType.Warning)
                    {
                        statusCur=StatusGV.TypeStatusRes.Warnings;
                    }
                }
            }
            return statusCur;
        }

        // return value: whether the list contains at least one "true" item 
        public bool FilterByCond(DICond dic, GErrList gerrlist)
        {
            if (dic==null)
            {
                throw new ExceptionGlyph("GErrList","FilterByCondition","Null argument");
            }
            bool res=false;
            foreach (GErr gerr in this.gerrs)
            {
                if (dic(gerr))
                {
                    if (gerrlist!=null)
                    {
                        gerrlist.Add(gerr);
                        res=true;
                    }
                }
            }
            return res;
        }

        // Iterates through the error list and returns true iff there's an error that is an Error or an AppError
        public bool TestErrors()
        {
            bool bRet = true;
            foreach (GErr err in this.gerrs)
            {
                if (err.TypeBasic == ValInfoBasic.ValInfoType.Error || err.TypeBasic == ValInfoBasic.ValInfoType.AppError)
                {
                    bRet = false;
                    break;
                }
            }
            return bRet;
        }

    }
/*
    public class GErrStore : GErrList
    {
        public GErrStore()
        {
            this.gerrs=new ArrayList();
        }
        override public bool Add(GErr gerr)
        {
            if (gerr.WasStored) // gerr can belong to at most one storage
                return false;
            if (this.gerrs.Contains(gerr))
                return false;
            gerr.WasStored=true;
            gerr.CntRef++;
            this.gerrs.Add(gerr);
            return true;    
        }
        override public bool Delete(GErr gerr)
        {
            if (!this.gerrs.Contains(gerr))
                return false;
            gerr.CntRef--;
            gerr.WasStored=false;
            gerr.Clear();
            gerr.WasStored=true;
            this.gerrs.Remove(gerr);
            return true;
        }
        override public void Clear()
        {
            foreach (GErr gerr in this.gerrs)
            {
                gerr.CntRef--;
                gerr.WasStored=false;
                gerr.Clear();
                gerr.WasStored=true;
            }
            this.gerrs.Clear();
        }
    }

    public class GErrView : GErrList
    {
        public GErrView()
        {
            this.gerrs=new ArrayList();
        }
        override public bool Add(GErr gerr)
        {
            if (this.gerrs.Contains(gerr))
                return false;
            gerr.CntRef++;
            this.gerrs.Add(gerr);
            return true;    
        }
        override public bool Delete(GErr gerr)
        {
            if (!this.gerrs.Contains(gerr))
                return false;
            gerr.CntRef--;
            gerr.Clear();
            this.gerrs.Remove(gerr);
            return true;
        }
        override public void Clear()
        {
            foreach (GErr gerr in this.gerrs)
            {
                gerr.CntRef--;
                gerr.Clear();
            }
            this.gerrs.Clear();
        }
    }
    */
}


