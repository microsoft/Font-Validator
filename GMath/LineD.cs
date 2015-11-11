using System;
using System.Diagnostics;

using NS_IDraw;

using Curve=NS_GMath.I_CurveD;
using LCurve=NS_GMath.I_LCurveD;

namespace NS_GMath
{
    public class LineD : LCurve
    {
        private VecD[] cp;
        /*
         *        CONSTRUCTORS
         */
        private LineD()
        {
            this.cp=new VecD[2];
        }
        public LineD(VecD start, VecD end): this()
        {
            this.cp[0]=new VecD(start);
            this.cp[1]=new VecD(end);
        }
        public LineD(LCurve lcurve) : this()
        {
            this.cp[0]=new VecD(lcurve.Start);
            this.cp[1]=new VecD(lcurve.End);
        }
        /*
         *        METHODS : GEOMETRY
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
                throw new ExceptionGMath("LineD","DirNorm","NOT IMPLEMENTED");
                //return null;
            }
        }
        public int LComplexity
        {
            get { return 2; }
        }
        Curve Curve.Copy()
        {
            return new LineD(this);
        }
        public VecD Cp(int i)
        {
            if ((i<0)||(i>=1))
                return null;
            return this.cp[i];
        }
        public bool IsSimple 
        {
            get { return true; }
        }
        public bool IsBounded 
        { 
            get { return false; }
        }
        public bool IsDegen    
        { 
            get
            {
                return (this.cp[0]==this.cp[1]);
            }
        }
        public bool IsValid
        {
            get { return (!this.IsDegen); }
        }

        
        public double CurveLength(Param parS, Param parE)
        {
            double length=Math.Abs(parE-parS)*(this.cp[1]-this.cp[0]).Norm;
            if (Math.Abs(length)>=MConsts.Infinity)
            {
                length=MConsts.Infinity*Math.Sign(length);
            }
            return length;
        }
        public double CurveLength()
        {
            return MConsts.Infinity;
        }
        public void Reverse()
        {
            this.cp[1].From(-this.cp[1].X,-this.cp[1].Y);
        }
        public Curve Reversed
        {
            get 
            {
                LineD lineRev=new LineD(this);
                lineRev.Reverse();
                return lineRev;
            }
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
        public double ParamStart 
        {
            get { return -Param.Infinity; }
        }
        public double ParamEnd 
        { 
            get { return Param.Infinity; }
        }
        public double ParamReverse
        {
            get { return 0; }
        }

        public Param.TypeParam ParamClassify(Param par)
        {
            par.Clip(this.ParamStart,this.ParamEnd);
            if ((par.Val==Param.Invalid)||(par.Val==Param.Degen))
                return Param.TypeParam.Invalid;
            return Param.TypeParam.Inner;
        }
        public RayD.TypeParity RayParity(RayD ray, bool isStartOnGeom)
        {
            throw new ExceptionGMath("LineD","RayParity","NOT IMPLEMENTED");
            //return RayD.TypeParity.ParityUndef;
        }        
        public void Transform(MatrixD m)
        {
            throw new ExceptionGMath("LineD","Transform","NOT IMPLEMENTED");        
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
                if (this.IsDegen)
                {
                    throw new ExceptionGMath("LineD","BBox",null);
                }
                BoxD box=new BoxD(-MConsts.Infinity,-MConsts.Infinity,MConsts.Infinity,MConsts.Infinity);
                return box;
            }
        }
        public bool IsEvaluableWide(Param par)
        {
            return ((!par.IsDegen)&&(par.IsValid));
        }
        public bool IsEvaluableStrict(Param par)
        {
            return ((!par.IsDegen)&&(par.IsValid));
        }
        public VecD Evaluate(Param par)
        {
            if (!this.IsEvaluableWide(par))
            {
                throw new ExceptionGMath("LineD","Evaluate",null);
                //return null;
            }
            if (par.IsInfinite)
                return null;
            return (1-par)*this.cp[0]+par*this.cp[1];
        }
        /*
         *        METHODS: I_DRAWABLE
         */
        public void Draw(I_Draw i_draw, DrawParam dp)
        {
            DrawParamCurve dpCurve= dp as DrawParamCurve;
            if (dpCurve!=null)
            {
                if (!this.IsDegen)
                {
                    VecD tang=(this as LCurve).DirTang;
                    VecD startToDraw=this.Start-i_draw.DrawWorldInfinity*tang;
                    VecD endToDraw=this.Start+i_draw.DrawWorldInfinity*tang;
                    i_draw.DrawSeg(startToDraw.X, startToDraw.Y,
                        endToDraw.X, endToDraw.Y,
                        dpCurve.StrColor,dpCurve.ScrWidth);
                }
            }
            if (dpCurve.ToDrawEndPoints)
            {
                this.Cp(0).Draw(i_draw,dpCurve.DPEndPoints);
                this.Cp(1).Draw(i_draw,dpCurve.DPEndPoints);
            }
        }
    }
}
