using System;
using System.Diagnostics;

using NS_IDraw;

using Curve=NS_GMath.I_CurveD;
using LCurve=NS_GMath.I_LCurveD;

namespace NS_GMath
{
    public class RayD : LCurve
    {
        /*
         *        ENUMS
         */
        public enum TypeParity
        {
            ParityUndef=-1, 
            ParityEven, 
            ParityOdd
        };

        /*
         *        MEMBERS
         */
        private VecD[] cp;
        /*
             *        CONSTRUCTORS
             */
        private RayD()
        {
            this.cp=new VecD[2];
        }
        public RayD(VecD start, VecD end): this()
        {
            this.cp[0]=new VecD(start);
            this.cp[1]=new VecD(end);
        }
        public RayD(RayD ray) : this()
        {
            this.cp[0]=new VecD(ray.Start);
            this.cp[1]=new VecD(ray.End);
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
                VecD dirTang=(this as LCurve).DirTang;
                if (dirTang==null)
                    return null;
                return new VecD(-dirTang.Y, dirTang.X);
            }
        }
        Curve Curve.Copy()
        {
            return new RayD(this);
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
        public int LComplexity
        {
            get { return 1; }
        }
        public bool IsValid
        {
            get { return (!this.IsDegen); }
        }
        public double CurveLength(Param parS, Param parE)
        {
            if ((!this.IsEvaluableStrict(parS))||(!this.IsEvaluableStrict(parE)))
            {
                throw new ExceptionGMath("RayD","CurveLength",null);
            }
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
                RayD rayRev=new RayD(this);
                rayRev.Reverse();
                return rayRev;
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
            get { return 0; }
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
            par.Clip(-Param.Infinity,Param.Infinity);
            par.Round(0);

            double val=par.Val;
            if (val==Param.Degen)
                return Param.TypeParam.Invalid;    
            if (val==Param.Invalid)
                return Param.TypeParam.Invalid;
            if (val<0) 
            {
                return Param.TypeParam.Before;
            }
            else if (val==0) 
            {
                return Param.TypeParam.Start;
            }
            else
            {
                return Param.TypeParam.Inner;
            }
        }
        public RayD.TypeParity RayParity(RayD ray, bool isStartOnGeom)
        {
            throw new ExceptionGMath("RayD","RayParity","NOT IMPLEMENTED");
            //return RayD.TypeParity.ParityUndef;
        }
        
        public void Transform(MatrixD m)
        {
            throw new ExceptionGMath("RayD","Transform","NOT IMPLEMENTED");
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
                    throw new ExceptionGMath("RayD","BBox",null);
                }
                double xMin, yMin, xMax, yMax;
                if (this.Cp(0).X<this.Cp(1).X)
                {
                    xMin=this.Cp(0).X;
                    xMax=MConsts.Infinity;
                }
                else
                {
                    xMin=-MConsts.Infinity;
                    xMax=this.Cp(0).X;
                }
                if (this.Cp(0).Y<this.Cp(1).Y)
                {
                    yMin=this.Cp(0).Y;
                    yMax=MConsts.Infinity;
                }
                else
                {
                    yMin=-MConsts.Infinity;
                    yMax=this.Cp(0).Y;
                }
                BoxD box=new BoxD(xMin,yMin,xMax,yMax);
                return box;
            }
        }
        public bool IsEvaluableWide(Param par)
        {
            return ((par.IsValid)&&(!par.IsDegen));
        }
        public bool IsEvaluableStrict(Param par)
        {
            if ((par.IsDegen)||(!par.IsValid))
                return false;
            par.Round(this.ParamStart,this.ParamEnd);
            if (par.Val<0)
                return false;
            return true;
        }
        public VecD Evaluate(Param par)
        {
            if (!this.IsEvaluableWide(par))
            {
                throw new ExceptionGMath("RayD","Evaluate",null);
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
                    VecD endToDraw=this.Start+i_draw.DrawWorldInfinity*(this as LCurve).DirTang;
                    i_draw.DrawSeg(this.cp[0].X, this.cp[0].Y,
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
