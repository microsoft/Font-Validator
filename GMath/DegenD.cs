using System;
using System.Text;
using System.Diagnostics;

using NS_IDraw;
using fu=System.Int32;

using Curve=NS_GMath.I_CurveD;
using BCurve=NS_GMath.I_BCurveD;

namespace NS_GMath
{
    public class DegenD : BCurve, I_Drawable
    {
        /*
         *        MEMBERS
         */
        private VecD cp;

        /*
         *        PROPERTIES
         */
        public VecD Cp
        {
            get { return this.cp; }
            set { this.cp=value; }
        }
        public BCurve Reduced
        {
            get { return this; }
        }
        public VecD Middle
        {
            get { return this.Cp; }
        }
                        
        /*
         *        CONSTRUCTORS
         */
        public DegenD()
        {
            this.cp=new VecD(0,0);
        }
        public DegenD(VecD vec) : this()
        {
            this.cp.From(vec);
        }
        public DegenD(DegenD degen) : this()
        {
            this.cp.From(degen.Cp);
        }
        /*
         *        METHODS: GEOMETRICAL
         */
        public bool IsSelfInters(out InfoInters inters)
        {
            inters=null;
            return false;
        }

        public BCurve Reduce()
        {
            return this;
        }
        public DegenD Copy()
        {
            return new DegenD(this);
        }
        Curve Curve.Copy()
        {
            return (new DegenD(this));
        }

        public void Transform(MatrixD m)
        {
            this.cp.Transform(m);
        }
        public BoxD BBox 
        { 
            get
            {
                return this.cp.BBox;
            }
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
            get { return true; }
        }
        public void Reverse()
        {
        }
        public Curve Reversed
        {
            get { return new DegenD(this); }
        }
        public int BComplexity
        {
            get {return 1;}
        }
        public VecD Evaluate(Param par)
        {
            throw new ExceptionGMath("DegenD","Evaluate","NOT IMPLEMENTED");
            //return null;
        }
        public bool IsEvaluableWide(Param par)
        {
            throw new ExceptionGMath("DegenD","IsEvaluableWide","NOT IMPLEMENTED");
            //return false;    
        }        
        public bool IsEvaluableStrict(Param par)
        {
            throw new ExceptionGMath("DegenD","IsEvaluableStrict","NOT IMPLEMENTED");
            //return false;    
        }        
        public VecD DirTang(Param par)
        {
            return null;
        }
        public VecD DirNorm(Param par)
        {
            return null;
        }
        public double CurveLength(Param parS, Param parE)
        {
            return 0;
        }
        public double CurveLength()
        {
            return 0;
        }
        public double Curvature(Param par)
        {
            return MConsts.Infinity;
        }
        public VecD Start 
        { 
            get { return this.cp; }
        }
        public VecD End 
        { 
            get { return this.cp; }
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
        public Param.TypeParam ParamClassify(Param par)
        {
            throw new ExceptionGMath("DegenD","ParamClassify","NOT IMPLEMENTED");
            //return Param.TypeParam.Inner;
        }
        public RayD.TypeParity RayParity(RayD ray, bool isStartOnGeom)
        {
            throw new ExceptionGMath("DegenD","RayParity","NOT IMPLEMENTED");
            //return RayD.TypeParity.ParityUndef;
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
            curveS=new DegenD(this.cp);
            curveE=new DegenD(this.cp);
        }
        public BCurve SubCurve(Param parA, Param parB)
        {
            if ((!this.IsEvaluableStrict(parA))||(!this.IsEvaluableStrict(parB)))
                return null;
            return new DegenD(this.cp);
        }

        public void PowerCoeff(out VecD[] pcf)
        {
            pcf=new VecD[1];
            pcf[0]=new VecD(this.cp);
        }

        /*
         *        METHODS: I_DRAWABLE
         */
        public void Draw(I_Draw i_draw, DrawParam dp)
        {
            DrawParamCurve dpCurve=dp as DrawParamCurve;
            if (dpCurve==null)
                return;
            if (dpCurve.ToDrawEndPoints)
            {
                this.Cp.Draw(i_draw, dpCurve.DPEndPoints);
            }
        }


        /*
         *        CASTINGS
         */
        /*
        public static implicit operator PointF(VecD vecD)
        {
            PointF pnt=new PointF((float)vecD.X, (float)vecD.Y);
            return pnt;
        }
        */

    }
}

