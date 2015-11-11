using System;
using System.Diagnostics;


using NS_ValCommon; // Seems unnecessary
using OTFontFile;   // OTF2Dot14

namespace NS_GMath
{
    public class Component
    {
        /*
         *        ENUM
         */

        public enum TypeErrLoadComponent
        {
            IncorrectShiftSpecification,
            IncorrectIndexKnotGlyph,
            IncorrectIndexKnotComponent,
            IncorrectTransform
        }


        /*
         *        MEMBERS
         */
        int indGlyphComponent;
        int indKnotAttGlyph;
        int indKnotAttComponent;
        VecD shift;
        OTF2Dot14[,] trOTF2Dot14;

        // filled in during composite binding
        MatrixD trD;
        int indKnotStart;
        int numKnot;
        int pozContStart;
        int numCont;
        /*
         *        PROPERTIES
         */
        public int NumCont
        {
            get { return this.numCont; }
            set { this.numCont=value; }
        }
        public int NumKnot
        {
            get { return this.numKnot; }
            set { this.numKnot=value; }
        }
        public int IndKnotStart
        {
            get { return this.indKnotStart; }
            set { this.indKnotStart=value; }
        }
        public int PozContStart
        {
            get { return this.pozContStart; }
            set { this.pozContStart=value; }
        }

        public int IndexGlyphComponent
        {
            get { return this.indGlyphComponent; }
            set { this.indGlyphComponent=value; }
        }
        public int IndexKnotAttGlyph
        {
            get { return this.indKnotAttGlyph; }
            set { this.indKnotAttGlyph=value; }
        }
        public int IndexKnotAttComponent
        {
            get { return this.indKnotAttComponent; }
            set { this.indKnotAttComponent=value; }
        }
        public VecD Shift
        {
            get 
            { 
                if ((object)this.shift == null)
                {
                    return null;
                }
                else
                {
                    return new VecD(this.shift); 
                }
            }
            set 
            { 
                if ((object)value==null)
                {
                    this.shift=null;
                    return;
                }
                if ((object)this.shift==null)
                {
                    this.shift=new VecD(value);
                }
                else
                {
                    this.shift.From(value);
                }
            }
        }
        public OTF2Dot14[,] TrOTF2Dot14
        {
            get 
            {
                if (this.trOTF2Dot14==null)
                    return null;
                OTF2Dot14[,] tr=new OTF2Dot14[2,2];
                for (int i=0; i<2; i++)
                    for (int j=0; j<2; j++)
                        tr[i,j]=this.trOTF2Dot14[i,j];
                return tr;
            }
            set 
            { 
                if (value==null)
                {
                    this.trOTF2Dot14=null;
                    return;
                }
                if ((value.GetLength(0)!=2)||(value.GetLength(1)!=2))
                {
                    throw new ExceptionGMath("Component","TrOTF2Dot14","");
                }
                if (this.trOTF2Dot14==null)
                {
                    this.trOTF2Dot14=new OTF2Dot14[2,2];
                }
                for (int i=0; i<2; i++)
                    for (int j=0; j<2; j++)
                        this.trOTF2Dot14[i,j]=value[i,j];
            }        
        }
        public MatrixD TrD
        {
            get 
            {
                return this.trD;
            }
            set 
            {
                if (value==null)
                {
                    this.trD=value;
                    return;
                }
                if (this.trD==null)
                    this.trD=new MatrixD(value);
                else
                    this.trD.From(value);
                
            }
        }

        /*
             *        CONSTRUCTORS
             */
        private Component()
        {
        }
        public Component(int indexGlyphComponent)
        {
            this.indGlyphComponent=indexGlyphComponent;
            this.indKnotAttGlyph=GConsts.IND_UNINITIALIZED;
            this.indKnotAttComponent=GConsts.IND_UNINITIALIZED;
            this.shift=null;
            this.trOTF2Dot14=null;
            this.trD=null;
            this.numKnot=GConsts.IND_UNINITIALIZED;
            this.numCont=GConsts.IND_UNINITIALIZED;
            this.indKnotStart=GConsts.IND_UNINITIALIZED;
            this.pozContStart=GConsts.IND_UNINITIALIZED;
        }
        public Component(Component component)
        {
            this.indGlyphComponent=component.IndexGlyphComponent;
            this.indKnotAttGlyph=component.IndexKnotAttGlyph;
            this.indKnotAttComponent=component.IndexKnotAttComponent;
            this.shift=component.Shift;
            this.trOTF2Dot14=component.TrOTF2Dot14;
            this.trD=component.TrD;
            this.numKnot=component.NumKnot;
            this.numCont=component.NumCont;
            this.indKnotStart=component.IndKnotStart;
            this.pozContStart=component.PozContStart;
        }

            /*
             *        METHODS
             */
        bool ContainsKnot(int indKnot)
        {
            return ((this.indKnotStart<=indKnot)&&
                (indKnot<this.indKnotStart+this.numKnot));
        }

        internal void ClearRelease()
        {
            this.shift=null;
            this.trOTF2Dot14=null;
            this.trD=null;
        }

        internal void ClearReset()
        {
            this.ClearRelease();
            this.indGlyphComponent=GConsts.IND_UNINITIALIZED;
            this.indKnotAttGlyph=GConsts.IND_UNINITIALIZED;
            this.indKnotAttComponent=GConsts.IND_UNINITIALIZED;
        }
    }
}
