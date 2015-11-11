using System;
using System.Text;
using System.Diagnostics;

using NS_IDraw;
using fu=System.Int32;

using Curve=NS_GMath.I_CurveD;
using LCurve=NS_GMath.I_LCurveD;

namespace NS_GMath
{
    public class VecD : I_Drawable, I_Transformable, I_BBox
    {
        /*
         *        MEMBERS
         */
        double x;
        double y;

        /*
         *        PROPERTIES
         */
        public double X 
        {
            get { return this.x; }
            set { this.x=value; }
        }
        public double Y
        { 
            get { return this.y; }
            set { this.y=value; }
        }
        public double Norm
        {
            get { return Math.Sqrt(this.x*this.x+this.y*this.y); }
        }
        public fu FUX
        {
            // TODO: check !!!
            get { return (int)Math.Round(this.x); }
        }
        public fu FUY
        {
            // TODO: check !!!
            get { return (int)Math.Round(this.y); }
        }
        public bool IsFU
        {
            get 
            { 
                return ((Math.Abs(this.X-this.FUX)<MConsts.EPS_DEC)&&
                    (Math.Abs(this.Y-this.FUY)<MConsts.EPS_DEC));
            }
        }
        /*
         *        CONSTRUCTORS
         */
        public VecD(double x, double y)
        {
            this.x=x;
            this.y=y;
        }
        public VecD(VecD vec)
        {
            this.x=vec.X;
            this.y=vec.Y;
        }
        /*
         *        OPERATORS
         */
        public static VecD operator+ (VecD vecA, VecD vecB)
        {
            return (new VecD(vecA.X+vecB.X, vecA.Y+vecB.Y));
        }
        public static VecD operator- (VecD vecA, VecD vecB)
        {
            return (new VecD(vecA.X-vecB.X,vecA.Y-vecB.Y));
        }
        public static VecD operator* (double factor, VecD vec)
        {
            return (new VecD(factor*vec.X, factor*vec.Y));
        }
        public static bool operator== (VecD vecA, VecD vecB)
        {
            if (((object)vecA==null)||((object)vecB==null))
                return false;
            return ((vecA-vecB).Norm<MConsts.EPS_DEC);
        }
        public static bool operator!= (VecD vecA, VecD vecB)
        {
            return (!(vecA==vecB));
        }

        /*
         *        METHODS
         */
        public VecD Copy()
        {
            return new VecD(this);
        }
        public void FURound(out bool isChanged)
        {
            double xOld=this.x;
            double yOld=this.y;
            this.x=this.FUX;
            this.y=this.FUY;
            isChanged=((this.x!=xOld)||(this.y!=yOld));
        }
        public void From(VecD vec)
        {
            this.x=vec.X;
            this.y=vec.Y;
        }
        public void From(double x, double y)
        {
            this.x=x;
            this.y=y;
        }
        public double Dot(VecD vec)
        {
            return (this.x*vec.X+this.y*vec.Y);
        }
        public double Cross(VecD vec)
        {
            return (this.x*vec.Y-this.y*vec.X);
        }
        public double Dist(VecD vec)
        {
            return (this-vec).Norm;
        }
        bool Perp(LCurve lrs, out Param param)
        {
            /*
             *        MEANING:    perpendicular to the parametric range
             *                    [-Infinity, Infinity]
             */
            param=null;

            if (lrs.IsDegen)
            {
                throw new ExceptionGMath("VecD","Perp",null);
                //return false;
            }
            VecD start=lrs.Start;
            VecD end=lrs.End;
            double length=(end-start).Norm;
            VecD tang=lrs.DirTang;
    
            param = new Param(((this-start).Dot(tang))/((end-start).Dot(tang)));
            return true;        
        }

        bool Perp(Bez2D bez, out Param[] pars)
        {
            /*
             *        MEANING:    perpendicular to the parametric range 
             *                    [-Infinity, Infinity] for NON-DEGENERATED,
             *                    NON-FLAT bezier
             */
            pars=null;
            Param parM;
            if (bez.IsDegen)
            {
                throw new ExceptionGMath("VecD","Perp",null);
                //return false;
            }
            if (bez.IsSeg(out parM)||bez.IsSelfInters(out parM))
            {
                throw new ExceptionGMath("VecD","Perp",null);
                //return false;
            }
            VecD[] pcf;
            bez.PowerCoeff(out pcf);
            double a = 2*(pcf[2].Dot(pcf[2]));
            double b = 3*(pcf[2].Dot(pcf[1]));
            double c = pcf[1].Dot(pcf[1])+2*((pcf[0]-(this)).Dot(pcf[2]));
            double d = (pcf[0]-(this)).Dot(pcf[1]);
            int numRootReal;
            double[] root;
            Equation.RootsReal(a,b,c,d,out numRootReal,out root);
            pars=new Param[root.Length];
            for (int iRoot=0; iRoot<root.Length; iRoot++)
            {
                pars[iRoot]=new Param(root[iRoot]);
            }
            return true;
        }
        public bool Project(LCurve lcurve, out Param param, out VecD pnt)
        {
            /*
             *        MEANING:    the NEAREST point in the PARAMETRIC RANGE
             *                    of the curve
             * 
             *        ASSUMPTIONS:    the curve may be not reduced
             */
            param=null;
            pnt=null;
            if (lcurve.IsDegen)
            {
                SegD seg=lcurve as SegD;
                if (seg==null)
                {
                    throw new ExceptionGMath("VecD","Project",null);
                    //return false;
                }
                param=new Param(Param.Degen);
                pnt=seg.Middle;
                return true;
            }
            this.Perp(lcurve, out param);
            param.Clip(lcurve.ParamStart,lcurve.ParamEnd);
            pnt=lcurve.Evaluate(param);
            return true;
        }

        public bool ProjectGeneral(Bez2D bez, out Param[] pars, out VecD pnt)
        {
            /*
             *        MEANING:    parameter (or parameters) of the nearest point
             *                    in range [0,1]
             * 
             *        ASSUMPTIONS:    bezier can be non-reduced
             *                        bezier can be self-intersecting
             * 
             */


            pars=null;
            pnt=null;
            Param parM;
            if (!bez.IsSelfInters(out parM))
            {
                Param par;
                if (!this.Project(bez,out par,out  pnt))
                    return false;
                if (par!=null)
                {
                    pars=new Param[1];
                    pars[0]=par;
                }
                return true;
            }

            double valM=parM.Val;
            if (parM<0)
            {
                Bez2D bezRev=new Bez2D(bez);
                bezRev.Reverse();
                if (!this.ProjectGeneral(bezRev,out pars,out pnt))
                    return false;
                if (pars!=null)
                {
                    for (int iPar=0; iPar<pars.Length; iPar++)
                    {
                        pars[iPar].Reverse(1);
                    }
                }
                return true;
            }
            SegD support=bez.SupportFlat();
            Param parSupport;
            if (!this.Project(support,out parSupport, out pnt))
                return false;
            if (!bez.ParamFromSupport(parSupport,out pars))
                return false;
            return true;
        }

        public bool Project(Bez2D bez, out Param par, out VecD pnt)
        {
            /*
             *        MEANING:    the NEAREST point in the RANGE [0,1]
             *        ASSUMPTION:    -    bezier can be non-reduced
             *                    -    bezier is NOT self-intersecting
             */
            par=null;
            pnt=null;
            Param parM;
            if (bez.IsSelfInters(out parM))
            {
                throw new ExceptionGMath("VecD","Project(bez)",null);
                //return false;
            }
            if (bez.IsDegen)
            {
                par=new Param(Param.Degen);
                pnt=bez.Middle;
                return true;
            }
            if (bez.IsSeg(out parM))
            {
                SegD seg=bez.Reduced as SegD;
                this.Project(seg,out par,out pnt);
                bez.ParamFromSeg(par);
                return true;
            }
            // case of non-flat Bezier
            Param[] parsPerp;
            if (!this.Perp(bez, out parsPerp))
                return false;
            double distS=this.Dist(bez.Start);
            double distE=this.Dist(bez.End);
            double distMin=Math.Min(distS,distE);
            double parMin=(distS<=distE)? 0.0: 1.0;
            if (parsPerp!=null)
            {
                for (int iPerp=0; iPerp<parsPerp.Length; iPerp++)
                {
                    if (bez.IsEvaluableStrict(parsPerp[iPerp]))
                    {
                        double distPerp=this.Dist(bez.Evaluate(parsPerp[iPerp]));
                        if (distPerp<distMin)
                        {
                            distMin=distPerp;
                            parMin=parsPerp[iPerp];
                        }
                    }
                }
            }
            par=parMin;
            pnt=bez.Evaluate(parMin);
            return true;
        }


        bool InverseOn(Bez2D bez, out bool isOn, out Param par)
        {
            /*
             *        MEANING:    inverse point which is known to lie on the
             *                    bezier in range [-Infinity, Infinity]
             * 
             *        ASSUMPTIONS:    bez is IRREDUCABLE && NOT S/I 
             * 
             */    
        
            isOn=false;
            par=null;

            Param parM;
            if ((bez.IsDegen)||(bez.IsSeg(out parM))||(bez.IsSelfInters(out parM)))
            {
                throw new ExceptionGMath("VecD","InverseOn(bez)",null);
                //return false;
            }

            VecD[] cfBez;
            bez.PowerCoeff(out cfBez);

            double dev=(cfBez[2].Cross(this-cfBez[0]))*
                (cfBez[2].Cross(this-cfBez[0]))-
                (cfBez[2].Cross(cfBez[1]))*
                ((this-cfBez[0]).Cross(cfBez[1]));

            if (Math.Abs(dev)<MConsts.EPS_DEC)
            {
                isOn=true;
                par=(cfBez[2].Cross(this-cfBez[0]))/(cfBez[2].Cross(cfBez[1]));
            }

            return true;
        }

    
        bool InverseOn(LCurve lrs, out bool isOn, out Param par)
        {
            /*
             *        MEANING:    inverse point which is known to lie on the
             *                    lrs in range [-Infinity, Infinity]
             * 
             *        ASSUMPTIONS:    lrs is non-degenerated 
             * 
             */    
        
            isOn=false;
            par=null;

            if (lrs.IsDegen)
            {
                throw new ExceptionGMath("VecD","InverseOn(lrs)",null);
                //return false;
            }
            this.Inverse(lrs, out par);
            if (this.Dist(lrs.Evaluate(par))<MConsts.EPS_DEC)
            {
                isOn=true;
            }
            else
            {
                par=null;
            }
            return true;
        }

        public bool InverseOn(Curve curve, out bool isOn, out Param par)
        {
            isOn=false;
            par=null;
            if (curve is LCurve)
            {
                return this.InverseOn(curve as LCurve, out isOn, out par);
            }
            if (curve is Bez2D)
            {
                return this.InverseOn(curve as Bez2D, out isOn, out par);
            }
            throw new ExceptionGMath("VecD","InverseOn","NOT IMPLEMENTED");
            //return false;
        }

        public bool Inverse(Bez2D bez, out Param par)
        {
            /*
             *        MEANING:
             *            -    Parameter of the nearest point in range 
             *                [-Infinity, Infinity]
             *        ASSUMPTIONS:
             *            -    Bezier is REDUCED
             *            -    Works for any bezier if the nearest point belongs
             *                to support
             *            -    Works for non S/I bezier if the nearest point lies
             *                without the support  
             */
            par=null;
            
            Param parM;
            if (!bez.IsSelfInters(out parM))
            {    
                Param[] parsPerp;
                if (!this.Perp(bez, out parsPerp))
                    return false;
                double distMin=MConsts.Infinity;
                for (int iPar=0; iPar<parsPerp.Length; iPar++)
                {
                    double distCur=this.Dist(bez.Evaluate(parsPerp[iPar]));
                    if (distCur<distMin)
                    {
                        distMin=distCur;
                        par=parsPerp[iPar];
                    }
                }
                return true;
            }
            // bezier is s/i
            Param[] parsProj;
            VecD pnt;
            if (!this.ProjectGeneral(bez, out parsProj, out pnt))
                return false;
            if (this.Dist(pnt)>MConsts.EPS_DEC)
            {
                throw new ExceptionGMath("VecD","Inverse(bez)",null);
                //return false;
            }
            par=parsProj[0];
            return true;
        }

        public bool Inverse(LCurve lrs, out Param par)
        {
            /*
             *        MEANING:
             *            -    Parameter of the nearest point in range 
             *                [-Infinity, Infinity]
             *        ASSUMPTIONS:
             *            -    LRS is NON-DEGENERSTED
             */
            return (this.Perp(lrs,out par));
        }

        public bool Inverse(Curve curve, out Param par)
        {
            par=null;
            if (curve is LCurve)
            {
                return (this.Inverse(curve as LCurve,out par));
            }
            if (curve is Bez2D)
            {
                return (this.Inverse(curve as Bez2D,out par));
            }
            throw new ExceptionGMath("VecD","Inverse(curve)","NOT IMPLEMENTED");
            //return false;
        }

        /*
         *        METHODS:    I_DRAWABLE
         */
        public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamVec dpVec=dp as DrawParamVec;
            if (dpVec!=null)
            {
                i_Draw.DrawPnt(this.x, this.y, 
                    dpVec.ScrRad, dpVec.StrColor, dpVec.ScrWidth, dpVec.ToFill);
            }
        }
        /*
         *        METHODS : GENERAL
         */        
        public void Clear()
        {
            this.x=0.0;
            this.y=0.0;
        }
        public override bool Equals(object obj)
        {
            VecD vec=obj as VecD;
            if (vec==null)
                return false;
            return ((this.x==vec.X)&&(this.y==vec.Y));
        }
        public override int GetHashCode()
        {
            return (int)(this.x+this.y);
        }

        public override string ToString()
        {
            StringBuilder strBuilder=new StringBuilder();
            strBuilder.Append("VecI=("+this.x+","+this.y+")");
            return strBuilder.ToString();
        }
        /*
         *        METHODS: GEOMETRICAL
         */
        public void Transform(MatrixD m)
        {
            // vector * matrix
            this.x=m[0,0]*this.x+m[1,0]*this.y+m[2,0];
            this.y=m[0,1]*this.x+m[1,1]*this.y+m[2,1];
        }
        
        public VecD Transformed(MatrixD m)
        {
            VecD vec=new VecD(this);
            vec.Transform(m);
            return vec;
        }

        public BoxD BBox 
        { 
            get
            {
                return new BoxD(this.x,this.y,this.x,this.y);
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

