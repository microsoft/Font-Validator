using System;
using System.Collections;
using System.Diagnostics;

using NS_IDraw;
using OTFontFile; // OTF2Dot14

using BCurve=NS_GMath.I_BCurveD;

namespace NS_GMath
{
    public class Outline : I_Drawable, I_Transformable
    {
        /*
         *        MEMBERS
         */
        ArrayList conts;
        bool isChangedByRound;

        /*
         *        PROPERTIES
         */
        public bool IsChangedByRound
        {
            get { return this.isChangedByRound; }
        }

        public int NumCont
        {
            get { return this.conts.Count; }
        }
        public int NumKnot 
        {
            get 
            { 
                if (this.NumCont==0)
                    return 0;
                return (this.conts[this.NumCont-1] as Contour).IndKnotEnd+1;
            }
        }


        /*    
         *        CONSTRUCTORS
         */
        public Outline()
        {
            this.conts=new ArrayList();
            this.isChangedByRound=false;
        }
        public Outline(Outline outl):
            this()
        {
            if (outl==null)
            {
                throw new ExceptionGMath("Outline","Outline",null);
            }
            Contour contCopy;
            foreach (Contour cont in this.conts)
            {
                contCopy=new Contour(cont);
                this.conts.Add(contCopy);
            }
            this.isChangedByRound=outl.IsChangedByRound;
        }
        
        /*
         *        METHODS
         */
    
        public Contour ContByIndKnot(int indexKnot)
        {
            if ((indexKnot<0)||(indexKnot>=this.NumKnot))
                return null;
            foreach (Contour cont in this.conts)
            {
                if (cont.PozByInd(indexKnot)!=GConsts.POZ_INVALID)
                    return cont;
            }
            return null;
        }

        public Contour ContByKnot(Knot knot)
        {
            foreach (Contour cont in this.conts)
            {
                if (cont.PozByKnot(knot)!=GConsts.POZ_INVALID)
                {
                    return cont;
                }
            }
            return null;
        }

        public Knot KnotByInd(int indexKnot)
        {
            Contour cont=this.ContByIndKnot(indexKnot);
            if (cont==null)
                return null;
            return cont.KnotByInd(indexKnot);
        }        

        public void ReNumber()
        {
            int indKnot=0;
            foreach (Contour cont in this.conts)
            {
                for (int pozKnot=0; pozKnot<cont.NumKnot; pozKnot++)
                {
                    Knot knot=cont.KnotByPoz(pozKnot);
                    knot.IndexKnot=indKnot;
                    indKnot++;
                }
            }
        }
        public void KnotDelete(Knot knot)
        {
            Contour cont=this.ContByKnot(knot);
            if (cont!=null)
            {
                cont.KnotDelete(knot);
                if (cont.NumKnot==0)
                {
                    this.ContourDelete(cont);
                }
            }
            this.ReNumber();
        }
        
        public void KnotDeleteByInd(int indKnot)
        {
            Contour cont=this.ContByIndKnot(indKnot);
            if (cont!=null)
            {
                cont.KnotDeleteByInd(indKnot);
                if (cont.NumKnot==0)
                {
                    this.ContourDelete(cont);
                }
            }
            this.ReNumber();
        }

        public void ContourAdd(Contour cont)
        {
            // TODO: ReNumber ???
            if (cont!=null)
            {
                this.conts.Add(cont);
            }
        }

        public void ContourDelete(Contour cont)
        {
            // TODO: ReNumber ???
            if (cont!=null)
            {
                if (this.conts.Contains(cont))
                {
                    this.conts.Remove(cont);
                }
            }
        }
        public BCurve CurveByIndKnot(int indKnot)
        {
            Contour cont=this.ContByIndKnot(indKnot);
            if (cont==null)
                return null;
            Knot knot=cont.KnotByInd(indKnot);
            if (knot==null)
                return null;
            return (cont.CurveByKnot(knot));
        }

        public BCurve CurveByKnot(Knot knot)
        {
            Contour cont=this.ContByKnot(knot);
            if (cont==null)
                return null;
            return (cont.CurveByKnot(knot));
        }

        public Contour ContourByPoz(int poz)
        {
            if ((poz<0)||(poz>=this.NumCont))
                return null;
            return (this.conts[poz] as Contour);
        }

        public void ClearReset()
        {
            this.conts.Clear();
            this.isChangedByRound=false;
        }

        public void ClearRelease()
        {
            this.conts.Clear();
            this.conts=null;
            this.isChangedByRound=false;
        }

        public void ClearDestroy()
        {
            foreach (Contour cont in this.conts)
            {
                cont.ClearDestroy();
            }
            this.conts.Clear();
            this.conts=null;
            this.isChangedByRound=false;
        }


        /*
         *        METHODS:    GEOMETRY
         */
        public BoxD BBoxCP        // BBox for the control points
        {
            get
            {
                BoxD bboxCP=new BoxD();
                foreach (Contour cont in this.conts)
                {
                    bboxCP.Unite(cont.BBoxCP);
                }
                return bboxCP;
            }
        }

            public BoxD BBox    // BBox for the outline
        {
            get 
            {
                BoxD bbox=new BoxD();
                foreach (Contour cont in this.conts)
                {
                    bbox.Unite(cont.BBox);
                }
                return bbox;
            }
        }

        public Knot KnotNearest(VecD vecLocation)
        {
            double distMin=MConsts.Infinity;
            Knot knotNearest=null;
            foreach (Contour cont in this.conts)
            {
                Knot knotNearestCur=cont.KnotNearest(vecLocation);
                if (knotNearestCur!=null)
                {
                    double distCur=vecLocation.Dist(knotNearestCur.Val);
                    if (distCur<distMin)
                    {
                        distMin=distCur;
                        knotNearest=knotNearestCur;
                    }
                }
            }
            return knotNearest;
        }

        public bool AreDuplicatedComponents(Component componentA,
            Component componentB,
            out bool areDuplicated)
        {
            areDuplicated=false;
            if ((componentA==null)||(componentB==null))
            {
                throw new ExceptionGMath("Outline","AreDuplicatedComponents",null);
                //return false;
            }
            
            if (componentA==componentB)
                return true;
            for (int pozContA=componentA.PozContStart; 
                pozContA<componentA.PozContStart+componentA.NumCont; 
                pozContA++)
            {
                Contour contA=this.ContourByPoz(pozContA);
                for (int pozContB=componentB.PozContStart; 
                    pozContB<componentB.PozContStart+componentB.NumCont; 
                    pozContB++)
                {
                    Contour contB=this.ContourByPoz(pozContB);
                    bool areDuplicatedCont;
                    contA.AreDuplicated(contB, out areDuplicatedCont);
                    if (!areDuplicatedCont)
                    {
                        return true;
                    }
                }
            }
            areDuplicated=true;
            return true;
        }

        public bool IntersectComponents(Component componentA,
            Component componentB,
            ListInfoInters linters)
        {
            if ((componentA==null)||(componentB==null))
            {
                throw new ExceptionGMath("Outline","IntersectComponents",null);
                //return false;
            }

            if (componentA==componentB)
                return true;
            bool res=true;

            for (int pozContA=componentA.PozContStart; 
                pozContA<componentA.PozContStart+componentA.NumCont; 
                pozContA++)
            {
                Contour contA=this.ContourByPoz(pozContA);
                for (int pozContB=componentB.PozContStart; 
                    pozContB<componentB.PozContStart+componentB.NumCont; 
                    pozContB++)
                {
                    Contour contB=this.ContourByPoz(pozContB);
                    if (!contA.Intersect(contB, linters))
                        res=false;
                }
            }
            return res;
        }

        public bool Intersect(ListInfoInters linters)
        {
            bool res=true;
            foreach (Contour cont in this.conts)
            {
                bool isWrapped;
                if (!cont.IsWrapped(out isWrapped))
                {
                    res=false;
                }
                else
                {
                    if (!isWrapped)
                    {
                        cont.Intersect(linters);
                    }
                }
            }
            for (int pozA=0; pozA<this.NumCont-1; pozA++)
            {
                Contour contA=this.ContourByPoz(pozA);
                for (int pozB=pozA+1; pozB<this.NumCont; pozB++)
                {
                    Contour contB=this.ContourByPoz(pozB);
                    bool isDuplicated;    
                    if (!contA.AreDuplicated(contB, out isDuplicated))
                    {
                        res=false;
                    }
                    else
                    {
                        if (!isDuplicated)
                        {
                            if (!contA.Intersect(contB, linters))
                            {
                                res=false;
                            }
                        }
                    }
                }
            }
            return res;
        }

        public bool OutlineAdd(Outline outl, MatrixD trD)
        {
            if ((outl==null)||(trD==null))
            {
                throw new ExceptionGMath("Outline","OutlineAdd",null);
                //return false;
            }
            for (int pozCont=0; pozCont<outl.NumCont; pozCont++)
            {
                Contour cont=new Contour(outl.ContourByPoz(pozCont));
                cont.Transform(trD);
                this.ContourAdd(cont);
            }
            this.ReNumber();
            return true;
        }

        public bool LoadComponent(Component component, 
            Outline outl, 
            out ArrayList errsLoadComponent)
        {
            isChangedByRound=false;
            errsLoadComponent=null;
            int numKnotBeforeLoad=this.NumKnot;
            int numContBeforeLoad=this.NumCont;
            if (outl==null)
            {
                throw new ExceptionGMath("Outline","LoadComponent",null);
                //return false;
            }
            // validate 
            OTF2Dot14[,] trOTF2Dot14=component.TrOTF2Dot14;

            if (trOTF2Dot14!=null)
            {
                if (Math.Abs((double)trOTF2Dot14[0,0]*(double)trOTF2Dot14[1,1]-
                    (double)trOTF2Dot14[0,1]*(double)trOTF2Dot14[1,0])<MConsts.EPS_COMP)
                {
                    if (errsLoadComponent==null)
                        errsLoadComponent=new ArrayList();
                    errsLoadComponent.Add(Component.TypeErrLoadComponent.IncorrectTransform);
                }
            }
            
            bool isShiftByKnots, isShiftByVec;
            int indKnotAttGlyph=component.IndexKnotAttGlyph;
            int indKnotAttComponent=component.IndexKnotAttComponent;
            isShiftByKnots=((indKnotAttGlyph!=GConsts.IND_UNINITIALIZED)&&
                (indKnotAttComponent!=GConsts.IND_UNINITIALIZED));
            isShiftByVec=(component.Shift!=null);
            if ((isShiftByKnots&&isShiftByVec)||
                ((!isShiftByKnots)&&(!isShiftByVec)))
            {
                if (errsLoadComponent==null)
                    errsLoadComponent=new ArrayList();
                errsLoadComponent.Add(Component.TypeErrLoadComponent.IncorrectShiftSpecification);
            }
        
            if (isShiftByKnots)
            {
                if ((indKnotAttGlyph<0)||(indKnotAttGlyph>=this.NumKnot))
                {
                    if (errsLoadComponent==null)
                        errsLoadComponent=new ArrayList();
                    errsLoadComponent.Add(Component.TypeErrLoadComponent.IncorrectIndexKnotGlyph);
                }
                if ((indKnotAttComponent<0)||(indKnotAttComponent>=outl.NumKnot))
                {
                    if (errsLoadComponent==null)
                        errsLoadComponent=new ArrayList();
                    errsLoadComponent.Add(Component.TypeErrLoadComponent.IncorrectIndexKnotComponent);
                }
            }
            if (errsLoadComponent!=null)
                return true;

            // load
            VecD trShift;
            if (isShiftByKnots)
            {
                Knot knAttGlyph=this.KnotByInd(indKnotAttGlyph);
                Knot knAttComponent=outl.KnotByInd(indKnotAttComponent);
                if ((knAttGlyph==null)||(knAttComponent==null))
                {
                    throw new ExceptionGMath("Outline","LoadComponent",null);
                }
                trShift=knAttGlyph.Val-knAttComponent.Val; // TODO: check !!!
            }
            else
            {
                trShift=component.Shift;
            }
            MatrixD trD=new MatrixD(trOTF2Dot14,trShift);
            component.TrD=trD;
            component.NumKnot=outl.NumKnot;
            component.NumCont=outl.NumCont;
            component.IndKnotStart=numKnotBeforeLoad;
            component.PozContStart=numContBeforeLoad;

            this.OutlineAdd(outl,trD);
            bool isChangedByRoundCur;
            this.FURound(out isChangedByRoundCur);
            if (isChangedByRoundCur)
            {
                this.isChangedByRound=true;
            }
            return true;
        }

        public void FURound(out bool isChanged)
        {
            isChanged=false;
            bool isChangedCur;
            foreach (Contour cont in this.conts)
            {
                cont.FURound(out isChangedCur);
                if (isChangedCur)
                {
                    isChanged=true;
                }
            }
        }

        /*
         *        METHODS:    I_TRANSFORMABLE
         */
        public void Transform(MatrixD m)
        {
            if (this.conts==null)
                return;
            foreach (Contour cont in this.conts)
            {
                cont.Transform(m);
            }
        }

        /*
         *        METHODS:    I_DRAWABLE
         */
        public void Draw(I_Draw i_Draw, DrawParam dp)
        {
            DrawParamContour dpContour=dp as DrawParamContour;
            if (dpContour!=null)
            {
                foreach (Contour cont in this.conts)
                {
                    cont.Draw(i_Draw, dpContour);
                }
            }
        }
    }
}
