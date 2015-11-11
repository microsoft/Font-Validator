using System;


namespace NS_IDraw
{
    public class DrawParam
    {
        /*
         *        MEMBERS
         */
        string strColor;
        float scrWidth;
        /*
         *        PROPERTIES
         */
        public string StrColor
        {
            get { return this.strColor; }
            set { this.strColor=value; }
        }
        public float ScrWidth
        {
            get { return this.scrWidth; }
            set { this.scrWidth=value; }
        }
        /*
         *        CONSTRUCTORS
         */
        public DrawParam(string strColor, float scrWidth)
        {
            this.strColor=strColor;
            this.scrWidth=scrWidth;
        }
        /*
         *        METHODS
         */
        virtual public void ClearRelease()
        {
            this.strColor=null;
        }
    }

    public interface I_Draw
    {
        bool IsColorDefined(string nameColor);
        double DrawWorldInfinity{ get; }


        // draw primitives
        void DrawPnt(double worldX, double worldY, float scrRad, 
            string nameCol, float scrWidth, bool toFill);
        void DrawSeg(double worldX0, double worldY0, 
            double worldX1, double worldY1, 
            string nameCol, float scrWidth);
        void DrawBez2(double worldX0, double worldY0, 
            double worldX1, double worldY1, 
            double worldX2, double worldY2,
            string nameCol, float scrWidth);
        void DrawString(string str, double worldX, double worldY,
            string nameCol, float scrSize, float scrShiftX, float scrShiftY);


        // managenets
        void RegisterOnPaint(I_Drawable drawable, DrawParam drawparam);
        void UnRegistryOnPaint(I_Drawable drawable);
        void UnRegistryOnPaintAll();
        void Clear();
    }
}