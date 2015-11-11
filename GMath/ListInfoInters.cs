using System;
using System.Collections;
using System.Diagnostics;

using BCurve=NS_GMath.I_BCurveD;

namespace NS_GMath
{
    public class ListInfoInters    : IEnumerable
    {
        /*
         *        MEMBERS
         */
        int[] indsGlyphComponent;
        ArrayList linters;

        /*
         *        CONSTRUCTORS
         */
        public ListInfoInters()
        {
            this.linters=new ArrayList(); 
            this.indsGlyphComponent=new int[2];
            this.indsGlyphComponent[0]=GConsts.IND_UNINITIALIZED;
            this.indsGlyphComponent[1]=GConsts.IND_UNINITIALIZED;
        }

        /*
         *        PROPERTIES
         */

        public bool ContainsD1
        {
            get 
            {
                foreach (InfoInters inters in this.linters)
                {
                    if (inters.Dim==InfoInters.TypeDim.Dim1)
                        return true;
                }
                return false;
            }
        }
        public bool ContainsBezSI
        {
            get 
            {
                foreach (InfoInters inters in this.linters)
                {
                    if (inters.IsBezSI)
                        return true;
                }
                return false;
            }
        }

        /*
         *        METHODS
         */
        public void SetIndGlyphComponent(int indGlyphA, int indGlyphB)
        {
            this.indsGlyphComponent[0]=indGlyphA;
            this.indsGlyphComponent[1]=indGlyphB;
        }

        public int IndGlyphComponent(int ind)
        {
            if ((ind!=0)&&(ind!=1))
            {
                throw new ExceptionGMath("ListInfoInters","IndGlyphComponent",null);
                //return GConsts.IND_UNDEFINED;
            }
            return this.indsGlyphComponent[ind];
        }

        public void ClearDestroy()
        {
            foreach (InfoInters inters in this.linters)
            {
                inters.ClearRelease();
            }
            this.linters.Clear();
            this.linters=null;
        }
        public InfoInters this [int ind]
        {
            get 
            {
                return (this.linters[ind] as InfoInters);
            }
        }


        public void Add(InfoInters inters)
        {
            this.linters.Add(inters);
        }
        public int Count
        {
            get { return this.linters.Count; }
        }
        public void RemoveAt(int poz)
        {
            this.linters.RemoveAt(poz);
        }
        public void ParamReverse(double valRev, int indCurve, int pozStart)
        {
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                InfoInters inters=linters[poz] as InfoInters;
                inters.ParamReverse(valRev, indCurve);
            }
        }
        public void ParamFromReduced(BCurve bcurve, int indCurve, int pozStart)
        {
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                InfoInters inters=linters[poz] as InfoInters;
                inters.ParamFromReduced(bcurve,indCurve);
            }        
        }
        public void ParamSwap(int pozStart)
        {
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                InfoInters inters=linters[poz] as InfoInters;
                inters.ParamSwap();
            }
        }
        public void ParamToCParam(Knot kn0, Knot kn1, int pozStart)
        {
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                InfoInters inters=linters[poz] as InfoInters;
                inters.ParamToCParam(kn0,kn1);
            }            
        }
        public void ParamInvalidateBezSI(int pozStart)
        {
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                InfoInters inters=linters[poz] as InfoInters;
                inters.ParamInvalidateBezSI();
            }            
        }
        public void CleanEndPointBezSI(VecD pnt, int pozStart)
        {
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                IntersD0 intersD0=linters[poz] as IntersD0;
                if (intersD0!=null)
                {
                    bool toDelete=(intersD0.PntInters==pnt);
                    if (toDelete)
                    {
                        this.linters.RemoveAt(poz);
                        poz--;
                    }
                }
            }
        }
        public void CleanEndPointInters(bool connectAB, bool connectBA,
            int pozStart)
        {
            if ((!connectAB)&&(!connectBA))
                return;
            for (int poz=pozStart; poz<this.linters.Count; poz++)
            {
                IntersD0 intersD0=linters[poz] as IntersD0;
                if (intersD0!=null)
                {
                    if (!intersD0.IncludeBezSI)
                    {
                        intersD0.Ipi.Par(0).Round(0,1);
                        intersD0.Ipi.Par(1).Round(0,1);
                    
                        bool toDelete=((connectAB&&(intersD0.Ipi.Par(0).Val==1)&&(intersD0.Ipi.Par(1).Val==0))||
                            (connectBA&&(intersD0.Ipi.Par(0).Val==0)&&(intersD0.Ipi.Par(1).Val==1)));
                        if (toDelete)
                        {
                            this.linters.RemoveAt(poz);
                            poz--;
                        }
                    }
                }
            }
        }

        
        /*
         *        METHODS:        IENUMERABLE
         */
        public IEnumerator GetEnumerator() 
        {
            return this.linters.GetEnumerator();
        }

    }


}