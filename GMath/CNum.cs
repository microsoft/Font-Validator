using System;
using System.Diagnostics;

namespace NS_GMath
{
    public class CNum 
    { 
        /*
         *        MEMBERS
         */
        double re;
        double im;
        int multiplicity;

        /*
         *        PROPERTIES
         */
        public double Re 
        {
            get { return this.re; }
            set { this.re=value; }
        }
        public double Im
        {
            get { return this.im; }
            set { this.im=value; }
        }
        public int Multiplicity
        {
            get { return this.multiplicity; }
            set { this.multiplicity=value; }
        }

        public double Radius
        {
            get {return Math.Sqrt(this.re*this.re+this.im*this.im);}
        }

        public bool IsZero
        {
            get { return (this.Radius<MConsts.EPS_DEC); }
        }

        public bool IsPureRe
        {
            get { return (Math.Abs(this.im)<MConsts.EPS_DEC); }
        }

        public bool IsPureIm
        {
            get { return (Math.Abs(this.re)<MConsts.EPS_DEC); }
        }

        public double Angle
        {
            // angle is measured in radians
            get 
            {
                if (this.IsZero)
                    return 0.0;
                return (Math.Atan2(this.im, this.re));
            }
        }

        /*
         *        METHODS
         */        
        public CNum PrimeRoot(int order)
        {
            if (Math.Abs(order)<=MConsts.EPS_DEC)
            {
                throw new ExceptionGMath("CNum","PrimeRoot","");
            }
            return this.Pow(1.0/order);
        }

        public CNum Pow(double power)
        {
            if (this.IsZero)
                return new CNum(0,0);
            CNum res=new CNum();
            res.FromRadiusAngle(Math.Pow(this.Radius,power),this.Angle*power);
            return res;
        }

        /*
         *        CONSTRUCTORS
         */
        public CNum() : this(0,0,1)
        {
        }

        public CNum(double re, double im): this(re,im,1)
        {
        }

        public CNum(CNum z): this(z.Re,z.Im,z.Multiplicity)
        {
        }

        public CNum(double re, double im, int multiplicity)
        {
            this.re=re;
            this.im=im;
            this.multiplicity=multiplicity;
        }
        
        /*
         *        INITIALIZERS
         */
        public void From(CNum z)
        {
            this.re=z.Re;
            this.im=z.Im;
            this.multiplicity=z.Multiplicity;
        }

        public void FromRadiusAngle(double rad, double angle, int mult) 
        {
            // angle is measured in radians
            this.re=rad*Math.Cos(angle); 
            this.im=rad*Math.Sin(angle);
            //JJF Clearly a bug this.multiplicity=multiplicity;
            this.multiplicity = mult;
        }

        public void FromRadiusAngle(double rad, double angle) 
        {
            this.FromRadiusAngle(rad,angle,1);
        }

        public void FromReIm(double re, double im, int multiplicity) 
        {
            this.re=re;
            this.im=im;
            this.multiplicity=multiplicity;
        }        
        public void FromReIm(double re, double im)
        {
            this.FromReIm(re,im,1);
        }
    }
}
