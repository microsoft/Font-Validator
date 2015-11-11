using System;
using NS_IDraw;

namespace NS_GMath
{
    public class DrawParamVec: DrawParam
    {
        // members
        float scrRad;
        bool toFill;
        // properties
        public float ScrRad
        {
            get { return this.scrRad; }
            set { this.scrRad=value; }
        }
        public bool ToFill
        {
            get { return this.toFill; }
            set { this.toFill=value; }
        }
        // constructors
        public DrawParamVec(string strColor, float scrWidth, 
            float scrRad, bool toFill):
            base(strColor, scrWidth)
        {
            this.scrRad=scrRad;
            this.toFill=toFill;
        }
    }

    public class DrawParamCurve : DrawParam
    {
        // members
        bool toDrawEndPoints;
        DrawParamVec dpEndPoints;
        // properties
        public bool ToDrawEndPoints
        {
            get { return this.toDrawEndPoints; }
            set { this.toDrawEndPoints=value; }
        }
        public DrawParamVec DPEndPoints
        {
            get { return this.dpEndPoints; }
            set { this.dpEndPoints=value; }
        }
        // constructors
        public DrawParamCurve(string strColor, float scrWidth,
            bool toDrawEndPoints, DrawParamVec dpEndPoints):
            base(strColor, scrWidth)
        {
            this.toDrawEndPoints=toDrawEndPoints;
            this.dpEndPoints=dpEndPoints;
        }    
        override public void ClearRelease()
        {
            this.dpEndPoints=null;
        }
    }

    public class DrawParamKnot: DrawParam
    {
        // members
        float scrRad;
        bool toShowNumber;
        // properties
        public float ScrRad
        {
            get { return this.scrRad; }
            set { this.scrRad=value; }
        }
        public bool ToShowNumber
        {
            get { return this.toShowNumber; }
            set { this.toShowNumber=value; }
        }
        // constructors
        public DrawParamKnot(string strColor, float scrWidth, 
            float scrRad, bool toShowNumber):
            base(strColor, scrWidth)
        {
            this.scrRad=scrRad;
            this.toShowNumber=toShowNumber;
        }
    }

    public class DrawParamContour : DrawParam
    {
        // members
        DrawParamCurve    dpCurve;
        DrawParamKnot    dpKnot;
        
        // properties
        public DrawParamCurve DPCurve
        {
            get { return this.dpCurve; }
            set { this.dpCurve=value; }
        }
        public DrawParamKnot DPKnot
        {
            get { return this.dpKnot; }
            set { this.dpKnot=value; }
        }
        // constructors
        public DrawParamContour(DrawParamCurve dpCurve, DrawParamKnot dpKnot)
            : base(null,0)
        {
            this.dpCurve=dpCurve;
            this.dpKnot=dpKnot;
        }
        // members
        override public void ClearRelease()
        {
            this.dpCurve=null;
            this.dpKnot=null;
        }
    }
}

