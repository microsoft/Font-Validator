using System;
using System.Text;

using NS_IDraw;

namespace NS_GMath
{
    public class BoxD : I_Drawable
    {
        /*
         *        MEMBERS
         */
        private VecD vecMin;
        private VecD vecMax;

        /*
         *        PROPERTIES
         */
        public VecD VecMin
        {
            get { return this.vecMin;}
            set { this.vecMin = value;}
        }
        public VecD VecMax
        {
            get { return this.vecMax;}
            set { this.vecMax = value;}
        }
        public bool IsEmpty
        {
            get 
            {
                return ((this.vecMin.X>this.vecMax.X)||
                    (this.vecMin.Y>this.vecMax.Y));
            }
        }
        public double Diag
        {
            get 
            {
                if (this.IsEmpty)
                    return -MConsts.Infinity;
                else
                    return (this.vecMax.Dist(this.vecMin));
            }

        }

        /*
         *        CONSTRUCTORS
         */
        public BoxD():
            this(MConsts.Infinity, MConsts.Infinity, -MConsts.Infinity, -MConsts.Infinity)
        {
        }
        public BoxD(BoxD box): this(box.VecMin, box.VecMax)
        {    
        }
        public BoxD(VecD vecMin, VecD vecMax)
        {
            this.vecMin=new VecD(vecMin);
            this.vecMax=new VecD(vecMax);
        }
        public BoxD(double xMin, double yMin, double xMax, double yMax)
        {
            this.vecMin=new VecD(xMin,yMin);
            this.vecMax=new VecD(xMax,yMax);
        }

        /*
         *        METHODS
         */
        override public bool Equals(object obj)
        {
            BoxD box=obj as BoxD;
            if ((object)box==null)
                return false;
            return ((this.VecMax==box.VecMax)&&(this.VecMin==box.VecMin));
        }

        public double DeviationMax(BoxD box)
        {
            if ((this.IsEmpty)||(box.IsEmpty))
                return -MConsts.Infinity;
            double[] devs=new double[4];
            devs[0]=this.vecMin.X-box.VecMin.X;
            devs[1]=this.vecMin.Y-box.VecMin.Y;
            devs[2]=this.vecMax.X-box.VecMax.X;
            devs[3]=this.vecMax.Y-box.VecMax.Y;
            double devMax=0.0;
            for (int iDev=0; iDev<4; iDev++)
            {
                double devCur=Math.Abs(devs[iDev]);
                if (devCur>devMax)
                {
                    devMax=devCur;
                }
            }
            return devMax;
        }

        public bool Contains(VecD vec)
        {
            // true: if vec is inside or on boundary
            return ((this.VecMin.X<=vec.X)&&(vec.X<=this.VecMax.X)&&
                    (this.VecMin.Y<=vec.Y)&&(vec.Y<=this.VecMax.Y));
        }
        public void Unite(BoxD box)
        {
            this.vecMin.From(Math.Min(this.vecMin.X,box.VecMin.X),
                Math.Min(this.vecMin.Y,box.VecMin.Y));
            this.vecMax.From(Math.Max(this.vecMax.X,box.VecMax.X),
                Math.Max(this.vecMax.Y,box.VecMax.Y));
        }
        
        public bool HasInters(BoxD box)
        {
            if (box==null)
                return false;
            if ((this.IsEmpty)||(box.IsEmpty))
                return false;
            bool notInters=(box.VecMin.X>this.vecMax.X)||
                (box.VecMax.X<this.vecMin.X)||
                (box.VecMin.Y>this.vecMax.Y)||
                (box.VecMax.Y<this.vecMin.Y);
            return (!notInters);
        }

        public void SetFU()
        {
            bool isChanged;
            this.vecMin.FURound(out isChanged);
            this.vecMax.FURound(out isChanged);
        }
        
        public void SetEnlargeFU()
        {
            this.vecMin.X=Math.Floor(this.vecMin.X);
            this.vecMin.Y=Math.Floor(this.vecMin.Y);
            this.vecMax.X=Math.Ceiling(this.vecMax.X);
            this.vecMax.Y=Math.Ceiling(this.vecMax.Y);
        }

        public void Clear()
        {
            this.vecMin.Clear();
            this.vecMax.Clear();
        }
        public override int GetHashCode()
        {
            return (this.VecMin.GetHashCode()+this.VecMax.GetHashCode());
        }
        public override string ToString()
        {
            StringBuilder strBuilder=new StringBuilder();
            strBuilder.Append("BoxI - Min:"+this.vecMin.ToString()+" Max:"+this.vecMax.ToString());
            return strBuilder.ToString();
        }
        /*
         *        METHODS:    I_DRAWABLE
         */
        
        public void Draw(I_Draw i_draw, DrawParam dp)
        {
            if (this.IsEmpty)
                return;
            
            if (dp!=null)
            {
                i_draw.DrawSeg(this.vecMin.X,this.vecMin.Y,this.vecMax.X,this.vecMin.Y,dp.StrColor,dp.ScrWidth);
                i_draw.DrawSeg(this.vecMax.X,this.vecMin.Y,this.vecMax.X,this.vecMax.Y,dp.StrColor,dp.ScrWidth);
                i_draw.DrawSeg(this.vecMax.X,this.vecMax.Y,this.vecMin.X,this.vecMax.Y,dp.StrColor,dp.ScrWidth);            
                i_draw.DrawSeg(this.vecMin.X,this.vecMax.Y,this.vecMin.X,this.vecMin.Y,dp.StrColor,dp.ScrWidth);
            }
        }
    }
}