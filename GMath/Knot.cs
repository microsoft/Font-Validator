using System;

using NS_IDraw;



namespace NS_GMath
{
    public class Knot : I_Drawable, I_Transformable
    {
        /*
         *        MEMBERS
         */
        private int        indexKnot;    // if belongs to outline
        private VecD    val;
        private bool    on;

        /*
         *        CONSTRUCTORS
         */
        public Knot()
        {
            this.indexKnot=GConsts.IND_UNINITIALIZED;
            this.on=true;
            this.val=new VecD(0,0);
        }
        public Knot(int indKnot, VecD val, bool on)
        {
            this.indexKnot=indKnot;
            this.on=on;
            this.val=new VecD(val.X,val.Y);
        }
        public Knot(int indKnot, double x, double y, bool on)
        {
            this.indexKnot=indKnot;
            this.val=new VecD(x,y);
            this.on=on;
        }
        public Knot(Knot knot):
            this(knot.IndexKnot,knot.Val,knot.On)
        {
        }
        /*
         *        PROPERTIES
         */
        public int IndexKnot
        {
            get {return this.indexKnot;}
            set {this.indexKnot=value;}
        }
        public VecD Val
        {
            get {return this.val;} 
            set 
            {
                this.val.X=value.X;
                this.val.Y=value.Y;
            }
        }
        public bool On
        {
            get {return this.on;}
            set {this.on=value;}
        }

        /*
         *        METHODS
         */
        public void ClearDestroy()
        {
            this.indexKnot=GConsts.IND_DESTROYED;
            this.val.Clear();
            this.val=null;
        }
        public void ClearReset()
        {
            this.indexKnot=GConsts.IND_UNINITIALIZED;
            this.val.Clear();
            this.on=true;
        }

        /*
         *        METHODS: I_TRANSFORMABLE
         */
        public void Transform(MatrixD m)
        {
            this.val.Transform(m);
        }

        /*
         *        METHODS: I_DRAWABLE
         */
        public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamKnot dpKnot=dp as DrawParamKnot;
            if (dpKnot!=null)
            {
                i_Draw.DrawPnt(this.Val.X,this.Val.Y,
                    dpKnot.ScrRad, dpKnot.StrColor, dpKnot.ScrWidth, this.on);
                if (dpKnot.ToShowNumber)
                {
                    i_Draw.DrawString(this.indexKnot.ToString(),this.Val.X,this.Val.Y,
                        "Blue", 5, 1, -7);
                }
            
            }

        }
    }
}
