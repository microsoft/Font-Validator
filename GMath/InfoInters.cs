using System;
using System.Diagnostics;

using Curve=NS_GMath.I_CurveD;
using BCurve=NS_GMath.I_BCurveD;


namespace NS_GMath
{
    public class InfoParamInters
    {
        /*
         *        MEMBERS
         */
        Param[] param;
        /*
         *        CONSTRUCTORS
         */
        private InfoParamInters()
        {
            this.param=new Param[2];
        }
        public InfoParamInters(Param parA, Param parB): this()
        {
            if (parA==null)
            {
                this.param[0]=null;
            }
            else
            {
                this.param[0]=parA.Copy();
            }
            if (parB==null)
            {
                this.param[1]=null;
            }
            else
            {
                this.param[1]=parB.Copy();
            }
        }

        public InfoParamInters(InfoParamInters ipi): 
            this(ipi.Par(0), ipi.Par(1))
        {
        }
        

        /*
         *        PROPERTIES 
         */
        public Param Par(int indCurve)
        {
            return this.param[indCurve]; 
        }
        /*
         *        METHODS
         */
        public void ClearRelease()
        {
            if (this.param[0]!=null)
            {
                this.param[0].ClearRelease();
                this.param[0]=null;
            }
            if (this.param[1]!=null)
            {
                this.param[1].ClearRelease();
                this.param[1]=null;
            }
        }
        public void ParamReverse(double valReverse, int indCurve)
        {
            if (this.param[indCurve]!=null)
            {
                this.param[indCurve].Reverse(valReverse);
            }
        }
        public void ParamFromReduced(BCurve bcurve, int indCurve)
        {
            if (this.param[indCurve]!=null)
            {
                this.param[indCurve].FromReduced(bcurve);    
            }
        }
        public void ParamSwap()
        {
            Param parTmp=this.param[0];
            this.param[0]=this.param[1];
            this.param[1]=parTmp;
            parTmp=null;
        }
        public void ParamToCParam(Knot kn0, Knot kn1)
        {
            this.param[0]=new CParam(this.param[0], kn0);
            if (this.param[1]!=null)
            {
                this.param[1]=new CParam(this.param[1], kn1);
            }
        }
        public void ParamInvalidate()
        {
            this.param[0].Val=Param.Invalid;
            this.param[1].Val=Param.Invalid;
        }
    }



    public abstract class InfoInters 
    {
        /*
         *        MEMBERS
         */
        protected bool includeBezSI;
        /*
         *        ENUMS
         */
        public enum TypeDim
        {
            Dim0=0, Dim1=1
        }
        /*
         *        PROPERTIES
         */
        virtual public TypeDim Dim
        {
            get
            {
                throw new ExceptionGMath("InfoInters","Dim","Pure virtual function");
                //return TypeDim.Dim0;
            }
        }
        public bool IncludeBezSI
        {
            get { return this.includeBezSI; }
            set { this.includeBezSI=true; }
        }
        virtual public bool IsBezSI
        {
            get 
            {
                throw new ExceptionGMath("InfoInters","IsBezSI","Pure virtual function");
                //return false;
            }
        }

        /*
         *        METHODS
         */
        virtual public void ParamReverse(double valRev, int indCurve)
        {
            throw new ExceptionGMath("InfoInters","ParamReverse","Pure virtual function");
        }
        virtual public void ParamFromReduced(BCurve bcurve, int indCurve)
        {
            throw new ExceptionGMath("InfoInters","ParamFromReduced","Pure virtual function");
        }
        virtual public void ParamSwap()
        {
            throw new ExceptionGMath("InfoInters","ParamSwap","Pure virtual function");
        }
        virtual public void ParamToCParam(Knot kn0, Knot kn1)
        {
            throw new ExceptionGMath("InfoInters","ParamToCParam","Pure virtual function");
        }
        virtual public void ParamInvalidateBezSI()
        {
            throw new ExceptionGMath("InfoInters","ParamInvalidateBezSI","Pure virtual function");
        }
        virtual public void ClearRelease()
        {
            throw new ExceptionGMath("InfoInters","ClearRelease","Pure virtual function");
        }
    }

    public class IntersD0 : InfoInters
    {
        /*
         *        MEMBERS
         */
        InfoParamInters ipi;
        VecD pntInters;

        /*
         *        PROPERTIES
         */
        public InfoParamInters Ipi
        {
            get { return this.ipi; }
        }
        public VecD PntInters
        {
            get { return this.pntInters; }
        }
        override public TypeDim Dim
        {
            get { return TypeDim.Dim0; }
        }
        override public bool IsBezSI
        {
            get { return false; }
        }
        /*
         *        CONSTRUCTORS
         */
        public IntersD0(Param parA, Param parB, VecD pnt, bool includeBezSI)
        {
            // COPY
            this.ipi=new InfoParamInters(parA,parB);
            this.pntInters=new VecD(pnt);
            this.includeBezSI=includeBezSI;
        }
        public IntersD0(IntersD0 inters): 
            this(inters.Ipi.Par(0), inters.Ipi.Par(1), inters.PntInters,
            inters.IncludeBezSI)
        {
        }
        /*
         *        METHODS
         */
        override public void ClearRelease()
        {
            this.ipi.ClearRelease();
            this.ipi=null;
            this.pntInters=null;
        }
        override public void ParamReverse(double valRev, int indCurve)
        {
            this.ipi.ParamReverse(valRev, indCurve);
        }
        override public void ParamSwap()
        {
            this.ipi.ParamSwap();
        }
        override public void ParamFromReduced(BCurve bcurve, int indCurve)
        {
            this.ipi.ParamFromReduced(bcurve,indCurve);
        }
        override public void ParamToCParam(Knot kn0, Knot kn1)
        {
            this.ipi.ParamToCParam(kn0,kn1);
        }
        override public void ParamInvalidateBezSI()
        {
            this.ipi.ParamInvalidate();
            this.includeBezSI=true;
        }
    }

    public class IntersD1 : InfoInters
    {
        /*
         *        MEMBERS
         */
        InfoParamInters[] ipis;
        Curve            curveInters;

        /*
         *        PROPERTIES
         */
        public InfoParamInters IpiIn
        {
            get { return this.ipis[0]; }
        }
        public InfoParamInters IpiOut
        {
            get { return this.ipis[1]; }
        }
        override public TypeDim Dim
        {
            get { return TypeDim.Dim1; }
        }
        public Curve CurveInters
        {
            get { return this.curveInters; }
        }
        override public bool IsBezSI
        {
            get { return ((this.ipis[0].Par(0)!=null)&&(this.ipis[1].Par(1)==null)); }
        }

        /*
         *        CONSTRUCTORS
         */
        public IntersD1(Param parInA, Param parInB, 
            Param parOutA, Param parOutB, 
            Curve curveInters, bool includeBezSI)
        {
            this.ipis=new InfoParamInters[2];
            this.ipis[0]=new InfoParamInters(parInA, parInB);
            this.ipis[1]=new InfoParamInters(parOutA, parOutB);
            this.curveInters=(curveInters==null)? null: curveInters.Copy();
            this.includeBezSI=includeBezSI;
        }
        /*
         *        METHODS
         */
        override public void ParamReverse(double valRev, int indCurve)
        {
            this.ipis[0].ParamReverse(valRev,indCurve);
            this.ipis[1].ParamReverse(valRev,indCurve);
        }
        override public void ParamSwap()
        {
            this.ipis[0].ParamSwap();
            this.ipis[1].ParamSwap();
            this.curveInters.Reverse();
        }
        override public void ParamFromReduced(BCurve bcurve, int indCurve)
        {
            this.ipis[0].ParamFromReduced(bcurve,indCurve);
            this.ipis[1].ParamFromReduced(bcurve,indCurve);
        }
        override public void ParamToCParam(Knot kn0, Knot kn1)
        {
            this.ipis[0].ParamToCParam(kn0,kn1);
            this.ipis[1].ParamToCParam(kn0,kn1);
        }
        override public void ParamInvalidateBezSI()
        {
            this.ipis[0].ParamInvalidate();
            this.ipis[1].ParamInvalidate();
            this.includeBezSI=true;
        }
        override public void ClearRelease()
        {
            this.ipis[0].ClearRelease();
            this.ipis[0]=null;
            this.ipis[1].ClearRelease();
            this.ipis[1]=null;
            this.ipis=null;
            // TODO: try curveInters.ClearRelease
            this.curveInters=null;
        }
    }
}
