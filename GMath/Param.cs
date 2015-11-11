using System;
using System.Diagnostics;

using BCurve=NS_GMath.I_BCurveD;

namespace NS_GMath
{


    public class Param
    {

        /*
         *        CONSTS & ENUMS
         */
        public const double Infinity=2.0e20;
        public const double Degen=1.0e20;
        public const double Invalid=1.5e20;

        public enum TypeParam 
        {
            Before=-2, 
            Start=-1, 
            Inner=0, 
            End=1, 
            After=2, 

            Invalid=100,
        };
        /*
         *        MEMBERS
         */
        double val;
        
        /*
         *        PROPERTIES
         */
        public double Val
        {
            get { return this.val; }
            set 
            { 
                if (Math.Abs(value)>Param.Infinity)
                {
                    value=Param.Infinity*Math.Sign(value);
                }
                this.val=value;
            }
        }
        public bool IsValid
        {
            get { return (this.val!=Param.Invalid); }
        }
        public bool IsFictive
        {
            get { return ((this.IsDegen)||(this.IsInfinite)||(!this.IsValid)); }
        }
        public bool IsDegen
        {
            get { return (this.val==Param.Degen); }
        }
        public bool IsInfinite
        {
            get { return (Math.Abs(this.val)==Param.Infinity); }
        }
        /*
         *        CONSTRUCTORS
         */
            protected Param()
            {
                this.val=0.5;
            }
            
            public Param(double val)
            {
                this.Val=val;
            }

        /*
         *        METHODS
         */
        virtual public void ClearRelease()
        {
        }
        virtual public Param Copy()
        {
            return new Param(this.val);
        }
        public void Invalidate()
        {
            this.val=Param.Invalid;
        }
        public void Round(params double[] valRound)
        {
            for (int i=0; i<valRound.Length; i++)
            {
                if (Math.Abs(this.Val-valRound[i])<MConsts.EPS_DEC)
                    this.Val=valRound[i];
            }
        }
        public void Reverse(double valRev)
        {
            if (this.val==Param.Degen)
                return;
            if (this.val==Param.Invalid)
                return;
            if (Math.Abs(this.val)==Param.Infinity)
            {
                this.val=-this.val;
                return;
            }
            this.val=valRev-this.val;
            this.Clip(-Param.Infinity,Param.Infinity);
        }
        public void Clip(double start, double end)
        {
            if (start>end)
            {
                throw new ExceptionGMath("Param","Clip",null);
            }
            if (this.val==Param.Degen)
                return;
            if (this.val==Param.Invalid)
                return;
            this.Round(start,end);
            if (this.Val<start)
                this.Val=start;
            if (this.Val>end)
                this.Val=end;
        }
        public void FromReduced(BCurve bcurve)
        {
            // parameter of the (reduced)curve may be invalidated if
            // the curve intersects the self-intersecting Bezier
            if (this.val==Param.Invalid)
                return;
            if (bcurve.IsDegen)
            {
                return;
            }
            if (bcurve is Bez2D)
            {
                Bez2D bez=bcurve as Bez2D;
                Param parM;
                if (bez.IsSeg(out parM))
                {
                    bez.ParamFromSeg(this);
                }
            }
        }
        
        /*
         *        CONVERSIONS
         */
        public static implicit operator double(Param par)
        {
            return par.val;
        }
        public static implicit operator Param(double val)
        {
            return new Param(val);
        }
    }

    public class CParam : Param
    {
        Knot knot;

        /*
         *        CONSTRUCTORS
         */
        public CParam(double val, Knot knot)
        {
            this.Val=val;
            this.knot=knot;
        }
        public CParam(Param par, Knot knot)
        {
            if (par.GetType()!=typeof(Param))
            {
                throw new ExceptionGMath("Param","CParam",null);
            }
            this.Val=par.Val;
            this.knot=knot;
        }

        /*
         *        PROPERTIES
         */
        public Knot Kn
        {
            get { return this.knot; }
        }

        public int IndKnot
        {
            get 
            { 
                if (this.knot==null)
                    return GConsts.IND_UNINITIALIZED;
                return this.knot.IndexKnot; 
            }
        }
            
        /*
         *        METHODS
         */
        override public void ClearRelease()
        {
            this.knot=null;
        }

        override public Param Copy()
        {
            throw new ExceptionGMath("Param","Copy","NOT IMPLEMENTED");
            //return null;
        }
    }
}