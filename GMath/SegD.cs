using System;
using System.Diagnostics;

using NS_IDraw;
using Curve=NS_GMath.I_CurveD;
using BCurve=NS_GMath.I_BCurveD;
using LCurve=NS_GMath.I_LCurveD;


namespace NS_GMath
{
    public class SegD : I_Drawable, BCurve, LCurve
    {
        /*
         *        MEMBERS
         */
        private VecD[] cp;
        /*
         *        CONSTRUCTORS
         */
        private SegD()
        {
            this.cp=new VecD[2];
        }
        public SegD(VecD start, VecD end): this()
        {
            this.cp[0]=new VecD(start);
            this.cp[1]=new VecD(end);
        }
        public SegD(SegD seg) : this()
        {
            this.cp[0]=new VecD(seg.Cp(0));
            this.cp[1]=new VecD(seg.Cp(1));
        }

        /*
         *        PROPERTIES
         */
        VecD LCurve.DirTang
        {
            get 
            {
                if (this.IsDegen)
                    return null;
                return ((1/(this.cp[1]-this.cp[0]).Norm)*(this.cp[1]-this.cp[0]));
            }
        }
        VecD LCurve.DirNorm
        {
            get
            {    
                VecD dirTang=(this as LCurve).DirTang;
                if (dirTang==null)
                    return null;
                return new VecD(-dirTang.Y, dirTang.X);
            }
        }

        /*
         *        METHODS : GEOMETRY
         */
        Curve Curve.Copy()
        {
            return new SegD(this);
        }
        public VecD Cp(int i)
        {
            if ((i<0)||(i>1))
                return null;
            return this.cp[i];
        }
        public bool IsSimple 
        {
            get { return true; }
        }
        public bool IsBounded 
        { 
            get { return true; }
        }
        public bool IsDegen    
        { 
            get
            {
                return (this.cp[0]==this.cp[1]);
            }
        }
        public VecD Middle
        {
            get { return (0.5*(this.cp[0]+this.cp[1])); }
        }
        public BCurve Reduced
        {
            get 
            {
                if (this.IsDegen)
                {
                    return new DegenD(this.Middle);
                }
                return this;
            }
        }
        public double CurveLength(Param parS, Param parE)
        {
            if ((!this.IsEvaluableStrict(parS))||(!this.IsEvaluableStrict(parE)))
            {
                throw new ExceptionGMath("SegD","CurveLength",null);
            }
            return Math.Abs(parE-parS)*(this.cp[1]-this.cp[0]).Norm;
        }
        public double CurveLength()
        {
            return (this.cp[1]-this.cp[0]).Norm;
        }
        public void Reverse()
        {
            VecD tmp=this.cp[0];
            this.cp[0]=this.cp[1];
            this.cp[1]=tmp;
        }
        public Curve Reversed
        {
            get
            {
                SegD segRev=new SegD(this);
                segRev.Reverse();
                return segRev;
            }
        }
        public int BComplexity
        {
            get {return 2;}
        }
        public int LComplexity
        {
            get { return 0;}
        }

        VecD Curve.DirTang(Param par)
        {
            return ((LCurve)this).DirTang;
        }
        VecD Curve.DirNorm(Param par)
        {
            return ((LCurve)this).DirNorm;
        }
        public double Curvature(Param par)
        {
            return 0;
        }
        public VecD Start 
        { 
            get { return this.cp[0]; }
        }
        public VecD End 
        { 
            get { return this.cp[1]; }
        }
        
        public Param.TypeParam ParamClassify(Param par)
        {
            par.Round(this.ParamStart,this.ParamEnd);
            double val=par.Val;
            if ((!par.IsValid)||(par.IsDegen)) 
            {
                return Param.TypeParam.Invalid;
            }
            if (val<0)  
            {
                return Param.TypeParam.Before;
            }
            else if (val==0) 
            {
                return Param.TypeParam.Start;
            }
            else if (val<1)  
            {
                return Param.TypeParam.Inner;
            }
            else if (val==1) 
            {
                return Param.TypeParam.End;
            }
            else
            {
                return Param.TypeParam.After;
            }
        }
        public double ParamStart 
        {
            get { return 0; }
        }
        public double ParamEnd 
        { 
            get { return 1; }
        }
        public double ParamReverse
        {
            get { return 1; }
        }

        public RayD.TypeParity RayParity(RayD ray, bool isStartOnGeom)
        {
            throw new ExceptionGMath("SegD","RayParity","NOT IMPLEMENTED");
            //return RayD.TypeParity.ParityUndef;
        }
        
        public void Transform(MatrixD m)
        {
            this.cp[0].Transform(m);
            this.cp[1].Transform(m);
        }

        public void PowerCoeff(out VecD[] pcf)
        {
            pcf=new VecD[2];
            pcf[1]=this.cp[1]-this.cp[0];
            pcf[0]=this.cp[0];
        }

        public BoxD BBox 
        { 
            get
            {
                double xMin=Math.Min(this.cp[0].X,this.cp[1].X);
                double xMax=Math.Max(this.cp[0].X,this.cp[1].X);
                double yMin=Math.Min(this.cp[0].Y,this.cp[1].Y);
                double yMax=Math.Max(this.cp[0].Y,this.cp[1].Y);
                BoxD box=new BoxD(xMin, yMin, xMax, yMax);
                return box;
            }
        }
        public bool IsSelfInters(out InfoInters inters)
        {
            inters=null;
            return false;
        }

        public void Intersect(I_CurveD curveA, I_CurveD curveB /*, out...*/)
        {
            throw new ExceptionGMath("SegD","Intersect","NOT IMPLEMENTED");
        }
        public bool IsEvaluableWide(Param par)
        {
            if (!par.IsValid)
                return false;
            if (par.IsDegen&&(!this.IsDegen))
                return false;
            return true;
        }
        public bool IsEvaluableStrict(Param par)
        {
            if (!par.IsValid)
                return false;
            if (par.IsDegen)
            {
                return (this.IsDegen);
            }
            par.Round(this.ParamStart,this.ParamEnd);
            return ((0<=par)&&(par<=1));
        }
        public VecD Evaluate(Param par)
        {
            if (this.IsEvaluableWide(par))
            {
                VecD vec=(1-par)*this.cp[0]+par*this.cp[1];
                return vec;
            }
            if ((par.IsDegen)&&(this.IsDegen))
            {
                return 0.5*(this.cp[0]+this.cp[1]);
            }

            return null;
        }
        public void Subdivide(Param par, out BCurve curveS, out BCurve curveE)
        {
            /*
             *        ASSUMPTION: par should be in the range [0,1]
             */
            curveS=null;
            curveE=null;
            if (!this.IsEvaluableStrict(par))
                return;
            VecD pnt=this.Evaluate(par);
            curveS=new SegD(this.cp[0],pnt);
            curveE=new SegD(pnt,this.cp[1]);
        }

        public BCurve SubCurve(Param parA, Param parB)
        {        
            if ((!this.IsEvaluableStrict(parA))||
                (!this.IsEvaluableStrict(parB)))
            {
                return null;
            }
            VecD pntA=this.Evaluate(parA);
            VecD pntB=this.Evaluate(parB);
            /*
            if (Math.Abs(parA-parB)<MConsts.EPS_COMP)
                return new DegenD(0.5*(pntA+pntB));
                */
            return new SegD(pntA,pntB);
        }
        /*
         *        METHODS: I_DRAWABLE
         */
        public void Draw(I_Draw i_draw, DrawParam dp)
        {
            DrawParamCurve dpCurve=dp as DrawParamCurve;
            if (dpCurve==null)
                return;
            i_draw.DrawSeg(this.cp[0].X,this.cp[0].Y,this.cp[1].X,this.cp[1].Y,
                dpCurve.StrColor, dpCurve.ScrWidth);
            if (dpCurve.ToDrawEndPoints)
            {
                DrawParamVec dpEndPoints=dpCurve.DPEndPoints;
                if (dpEndPoints!=null)
                {
                    VecD vec=this.cp[0];
                    vec.Draw(i_draw, dpEndPoints);
                    vec=this.cp[1];
                    vec.Draw(i_draw, dpEndPoints);
                }
            }
        }
    }
}