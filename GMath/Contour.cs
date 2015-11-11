using System;
using System.Text;
using System.Collections;
using System.Diagnostics;
// for debug only - begin
using System.IO;
// for debug only - end


using NS_IDraw;

using Curve=NS_GMath.I_CurveD;
using CCurve=NS_GMath.I_CCurveD;
using BCurve=NS_GMath.I_BCurveD;
using LCurve=NS_GMath.I_LCurveD;


namespace NS_GMath
{
    public class Contour : CCurve, I_Drawable
    {
        /*
         *        MEMBERS
         */
        private ArrayList    knots;
        // for debug only - begin
        //RayD rayTmp;
        // for debug only - end

        static Random rand=new Random();
        
        /*
         *        PROPERTIES
         */
        public int NumKnot 
        {
            get { return this.knots.Count; }
        }
        public int IndKnotStart
        {
            get 
            { 
                Knot knot=this.KnotByPoz(0);
                return (knot==null)? GConsts.IND_UNDEFINED: knot.IndexKnot;
            }
        }
        public int IndKnotEnd
        {
            get 
            { 
                Knot knot=this.KnotByPoz(this.NumKnot-1);
                return (knot==null)? GConsts.IND_UNDEFINED: knot.IndexKnot;
            }
        }
        public bool IsSingleKnot
        {
            get { return (this.NumKnot==1); }
        }
        public bool IsEmpty
        {
            get { return (this.NumKnot==0); }
        }
        
        /*
         *        CONSTRUCTORS
         */

        public Contour()
        {
            this.knots=new ArrayList();
        }

        public Contour(Contour cont):
            this()
        {
            if (cont==null)
            {
                throw new ExceptionGMath("Contour","Contour","Null argument");
            }
            Knot knCopy;
            foreach (Knot kn in cont.knots)
            {
                knCopy=new Knot(kn);
                this.knots.Add(knCopy);
            }
        }
        /*
         *        METHODS
         */
        public int PozPrev(int pozCur)
        {
            if ((pozCur<0)||(pozCur>=this.NumKnot))
                return GConsts.POZ_INVALID;
            if (pozCur==0)
            {
                return (this.NumKnot-1);
            }
            return (pozCur-1);
        }

        public int PozNext(int pozCur)
        {            
            if ((pozCur<0)||(pozCur>=this.NumKnot))
                return GConsts.POZ_INVALID;
            return ((pozCur+1)%(this.NumKnot));
        }
        public int PozByAdvance(int pozCur, int advance)
        {
            if ((pozCur<0)||(pozCur>=this.NumKnot))
                return GConsts.POZ_INVALID;
            return ((pozCur+advance)%(this.NumKnot));
        }
        public int PozByKnot(Knot knot)
        {
            for (int poz=0; poz<this.knots.Count; poz++)
            {
                if ((object)this.knots[poz]==(object)knot)
                {
                    return poz;
                }
            }
            return GConsts.POZ_INVALID;

            //return (this.knots.IndexOf((object)knot));
        }
        public int PozByInd(int indKnot)
        {
            if ((indKnot<this.IndKnotStart)||(indKnot>this.IndKnotEnd))
                return GConsts.POZ_INVALID;
            return (indKnot-this.IndKnotStart);
        }
        public Knot KnotByPoz(int poz)
        {
            if ((poz<0)||(poz>=this.NumKnot))
                return null;
            return (this.knots[poz] as Knot);
        }
        public Knot KnotByInd(int indexKnot)
        {
            return this.KnotByPoz(this.PozByInd(indexKnot));
        }        
        public void KnotAdd(Knot knot)
        {
            // TODO: check !!!
            this.knots.Add(knot);
        }
        
        public void KnotDeleteByPoz(int poz)
        {
            if ((poz>=0)&&(poz<this.NumKnot))
            {
                this.knots.RemoveAt(poz);
            }
        }
        public void KnotDeleteByInd(int indKnot)
        {
            int poz=this.PozByInd(indKnot);
            this.KnotDeleteByPoz(poz);
        }
        public void KnotDelete(Knot knot)
        {
            if (this.knots.Contains(knot))
            {
                this.knots.Remove(knot);
            }
        }
        public void ClearRelease()
        {
            this.knots.Clear();
            this.knots=null;
        }
        public void ClearReset()
        {
            this.knots.Clear();
        }

        public void ClearDestroy()
        {
            foreach (Knot kn in this.knots)
            {
                kn.ClearDestroy();
            }
            this.knots.Clear();
            this.knots=null;
        }

        public BCurve CurveByKnot(Knot knot)
        {
            int poz=this.PozByKnot(knot);
            return (this.CurveByPoz(poz));
        }

        public BCurve CurveByPoz(int poz)
        {
            /*
             *        Vectors in the resulting curve are COPIED from contour
             */
            Knot knA=this.KnotByPoz(poz);
            if (knA==null)
                return null;
            Knot knB=this.KnotByPoz(this.PozNext(poz));

            // knB - on
            if (knB.On)
            {
                if (!knA.On)
                {
                    return null;
                }
                if (knA.On)
                {
                    return (new SegD(knA.Val,knB.Val));
                }
            }
            // knB - off
            Knot knC=this.KnotByPoz(this.PozNext(PozNext(poz)));
            VecD vecStart, vecMid, vecEnd;
            vecMid=knB.Val.Copy();
            if (knA.On)
                vecStart=knA.Val.Copy();
            else
                vecStart=0.5*(knA.Val+knB.Val);
            if (knC.On)
                vecEnd=knC.Val.Copy();
            else
                vecEnd=0.5*(knB.Val+knC.Val);
            
            return new Bez2D(vecStart,vecMid,vecEnd); 
        }

        public bool IsStartByPoz(int poz, out bool isStart)
        {
            /*
             *        Returns FALSE if (poz) is not valid for the contour 
             */
            isStart=false;
            Knot knA=this.KnotByPoz(poz);
            if (knA==null)
            {
                throw new ExceptionGMath("Contour","IsStartByPoz",null);
                //return false;
            }
            Knot knB=this.KnotByPoz(this.PozNext(poz));
            if ((knB.On) && (!knA.On))
            {
                isStart=false;
            }
            else
            {
                isStart=true;
            }
            return true;
        }

        public bool AdvanceByPoz(int poz, out int advance)
        {
            /*
             *        Returnes FALSE if:
             *        -    (poz) is not valid for the contour OR
             *        -    (poz) is not start of a curve
             */
            advance=GConsts.POZ_INVALID;
            bool isStart;
            if (!this.IsStartByPoz(poz,out isStart))
                return false;
            if (!isStart)
                return false;
            Knot knA=this.KnotByPoz(poz);
            if (knA==null)
                return false;
            Knot knB=this.KnotByPoz(this.PozNext(poz));

            // knB - on
            if (knB.On)
            {
                if (knA.On)
                {
                    advance=1;
                    return true;
                }
                else
                {
                    throw new ExceptionGMath("Contour","AdvanceByPoz",null);
                    //return false;
                }
            }
                // knB - off
            else
            {
                Knot knC=this.KnotByPoz(this.PozNext(PozNext(poz)));
                advance=(knC.On)? 2: 1;
                return true;
            }
        }

        public bool PozStartNextByPoz(int pozCur, out int pozStartNext)
        {
            /*
             *        Assumption: pozCur is a START position
             */
            pozStartNext=GConsts.POZ_INVALID;
            int advance;
            if (!this.AdvanceByPoz(pozCur, out advance))
                return false;
            pozStartNext=this.PozByAdvance(pozCur, advance);
            return true;
        }

        public bool PozStartNextNonDegenByPoz(int pozCur, out int pozStartNext)
        {
            /*
             *        Assumption: pozCur is a START position
             */
            pozStartNext=GConsts.POZ_INVALID;

            int pozNext=pozCur, pozWork;
            bool isDegen=true;
            while (isDegen)
            {
                pozWork=pozNext;
                if (!this.PozStartNextByPoz(pozWork,out pozNext))
                    return false;
                isDegen=this.CurveByPoz(pozNext).IsDegen;
            }

            pozStartNext=pozNext;
            return true;
        }
        
        public bool AreConjByPozPoz(int pozA, int pozB, out bool areConj)
        {
            /*
             *        MEANING:
             *            end(A) and start(B) coincide or are 
             *            separated by a sequence of degenerated curves
             *        ASSUMPTIONS:
             *            pozA!=pozB
             *            pozA, pozB are START pozitions
             * 
             */    
            areConj=false;
            if (pozA==pozB)
            {
                throw new ExceptionGMath("Contour","AreConjByPozPoz",null);
                //return false;
            }
            bool isStartA, isStartB;
            if ((!this.IsStartByPoz(pozA,out isStartA))||
                (!this.IsStartByPoz(pozB,out isStartB)))
                return false;
            if ((!isStartA)||(!isStartB))
            {
                throw new ExceptionGMath("Contour","AreConjByPozPoz",null);
                //return false;
            }
            
            int pozNext=pozA, pozWork;
            bool isDegen=true;
            while (isDegen)
            {
                pozWork=pozNext;
                if (!this.PozStartNextByPoz(pozWork,out pozNext))
                    return false;
                isDegen=this.CurveByPoz(pozNext).IsDegen;
                if (pozNext==pozB)
                {
                    areConj=true;
                    return true;
                }
            }
            return true;
        }



        public bool InfoConnectByPozPoz(int pozA, int pozB, 
            out InfoConnect icAB)
        {
            /*    curveA and curveB are connected if
             *    -    they follow one another or
             *    -    they are separated by a sequence of degenerated curves
             * 
             *    ASSUMPTIONS:
             *        pozA!=pozB
             */ 
            icAB=null;
            bool areConj;
            if (!this.AreConjByPozPoz(pozA, pozB, out areConj))
                return false;
            if (!areConj)
                return true;
            // are conjunctive
            int pozNextA;
            if (!this.PozStartNextByPoz(pozA,out pozNextA))
                return false;
            if (pozNextA!=pozB)
            {
                icAB=new InfoConnect(true,false);
            }
            // curveB is next after curveA
            Knot knotNextA=this.KnotByPoz(this.PozNext(pozA));
            Knot knotNextNextA=this.KnotByPoz(this.PozNext(this.PozNext(pozA)));
            bool isTang=((!knotNextA.On)&&(!knotNextNextA.On));
            icAB=new InfoConnect(true,isTang);
            return true;
        }

        /*
                knot0=this;
                knot1=(Knot *)(knot0->next); 
                knot2=(Knot *)(knot1->next);
                on0=knot0->on; on1=knot1->on; on2=knot2->on;
                if ((!on0)&&(on1)) return false;

                if (icToNext) icToNext->From(true,false);
                curve.type=eGeomUndef;
                knotStartNext=knot1;

                if (on1)
            {
                if (on0)
            {
                curve.seg.From(knot0->x,knot0->y,knot1->x,knot1->y,false);
                curve.type=eGeomSeg;
            }
            else
        {
            if (icToNext) icToNext->Connect(false);
        }
        }
        else // 1 - off curve knot
        {
        curve.type=eGeomBez;
        VecD cp0, cp1, cp2;

        if (on0) 
        {
        cp0.From(knot0->x,knot0->y,false); 
        }
        else
        {
        cp0.From(0.5*(knot0->x+knot1->x),0.5*(knot0->y+knot1->y));
        }
        
        cp1.From(knot1->x,knot1->y,false);

        if (on2) 
        {
        knotStartNext=knot2;
        cp2.From(knot2->x,knot2->y,false); 
        }        
        else
        {
        if (icToNext) icToNext->Tangent(true);
        cp2.From(0.5*(knot1->x+knot2->x),0.5*(knot1->y+knot2->y));
        }
        curve.bez.From(cp0,cp1,cp2);
        }

    
        if (getNextNonDegen) 
        {
        CurveD curveAux;
        Knot *knotCurAux, *knotNextAux;
        if (!knotStartNext->GetCurve(curveAux, knotNextAux)) return false;
        if (!curveAux.IsPnt()) return true;
        icToNext->Tangent(false);
        while (curveAux.IsPnt())
        {
        knotCurAux=knotNextAux;
        if (knotCurAux==knotStartNext) return false; // no non-degenerated curves...
        if (!knotCurAux->GetCurve(curveAux, knotNextAux)) return false;
        } 
        knotStartNext=knotCurAux;
        }
        return true;
        }
        */

        public Knot KnotNearest(VecD vecLocation)
        {
            double distMin=MConsts.Infinity;
            Knot knotNearest=null;
            foreach (Knot knot in this.knots)
            {
                double distCur=vecLocation.Dist(knot.Val);
                if (distCur<distMin)
                {
                    distMin=distCur;
                    knotNearest=knot;
                }
            }
            return knotNearest;
        }


        public bool IsWrapped(out bool isWrapped)
        {
            isWrapped=true;
            if (this.NumKnot<=2)
            {
                isWrapped=false;
                return true;
            }
            int pozA=0;
            // pozA -last in the chain of the knots that coincide with Knot(0)
            while (this.KnotByPoz(pozA)==this.KnotByPoz(this.PozNext(pozA)))
            {
                pozA=this.PozNext(pozA);
            }
            // pozB - first knot that coincides with Knot(0)
            int pozB;
            for (pozB=this.PozNext(pozA); pozB<this.NumKnot; pozB++)
            {
                if (this.KnotByPoz(pozB).Val==this.KnotByPoz(0).Val)
                    break;
            }
            if (pozB==this.NumKnot)            
            {
                isWrapped=false;
                return true;
            }
            // pozB - last knot that coincides with Knot(0)
            while (this.KnotByPoz(pozB)==this.KnotByPoz(this.PozNext(pozB)))
            {
                pozA=this.PozNext(pozB);
            }
            int iKnot;
            for (iKnot=0; iKnot<this.NumKnot; iKnot++)
            {
                if (this.KnotByPoz(pozA).Val!=this.KnotByPoz(pozB).Val)
                    break;
                pozA=this.PozNext(pozA);
                pozB=this.PozNext(pozB);
            }
            if (iKnot==this.NumKnot)
            {
                return true;
            }
            isWrapped=false;
            return true;
        }
        
        public bool Intersect(ListInfoInters linters)
        {
            bool res=true;
            for (int pozA=0; pozA<this.NumKnot-1; pozA++)
            {
                BCurve curveA=this.CurveByPoz(pozA);
                if (curveA!=null)
                {
                    Knot knA=this.KnotByPoz(pozA);
                    InfoInters inters;
                    if (curveA.IsSelfInters(out inters))
                    {
                        inters.ParamToCParam(knA,null);
                        linters.Add(inters);
                    }
                    bool isDegenA=curveA.IsDegen;
                    for (int pozB=pozA+1; pozB<this.NumKnot; pozB++)
                    {
                        BCurve curveB=this.CurveByPoz(pozB);
                        if (curveB!=null)
                        {
                            bool isDegenB=curveB.IsDegen;
                            if (isDegenA||isDegenB)
                            {
                                bool areConjAB, areConjBA;
                                if ((!this.AreConjByPozPoz(pozA,pozB,out areConjAB))||
                                    (!this.AreConjByPozPoz(pozB,pozA,out areConjBA)))
                                {
                                    res=false;
                                    continue;
                                }
                                if (areConjAB||areConjBA)
                                    continue;
                            }
                            InfoConnect icAB, icBA; 
                            if ((!this.InfoConnectByPozPoz(pozA,pozB,out icAB))||
                                (!this.InfoConnectByPozPoz(pozB,pozA,out icBA)))
                            {
                                res=false;
                                continue;
                            }
                            int numIntersBefore=linters.Count;
                            Knot knB=this.KnotByPoz(pozB);
                        
                            Inters.IntersectBB(curveA, curveB, icAB, icBA, linters);
                            linters.ParamToCParam(knA,knB,numIntersBefore);
                        }
                    }
                }
            }
            return res;            
        }
        
        public bool Intersect(Contour cont, ListInfoInters linters)
        {
            Contour contA=this;
            Contour contB=cont;
            if (contA==contB)
            {
                return this.Intersect(linters);
            }
            BoxD bboxContA=contA.BBox;
            BoxD bboxContB=contB.BBox;
            if (!bboxContA.HasInters(bboxContB))
                return true;

            for (int pozA=0; pozA<contA.NumKnot; pozA++)
            {
                Knot knA=contA.KnotByPoz(pozA);
                BCurve curveA=contA.CurveByPoz(pozA);
                if (curveA!=null)
                {    
                    BoxD bboxCurveA=curveA.BBox;
                    if (bboxCurveA.HasInters(bboxContB))
                    {
                        for (int pozB=0; pozB<contB.NumKnot; pozB++)
                        {
                            BCurve curveB=contB.CurveByPoz(pozB);
                            if (curveB!=null)
                            {
                                int numIntersBefore=linters.Count;
                                Inters.IntersectBB(curveA, curveB, null, null, linters);
                                Knot knB=contB.KnotByPoz(pozB);
                                linters.ParamToCParam(knA,knB,numIntersBefore);
                            }
                        }
                    }
                }
            }

            contA=null;
            contB=null;
            return true;
        }
        
        /*
         *        METHODS: MISORIENTED
         */

        public bool RayAlmostNormal(out RayD ray, out CParam parStartRay)
        {
            ray=null;
            parStartRay=null;

            bool isCurveOK=false;
            int numTrialMax=100;    // TODO: make constant
            int numTrialCur=0;

            while ((!isCurveOK)&&(numTrialCur<numTrialMax))
            {
                numTrialCur++;
                try
                {
                    int pozKnotStart=rand.Next(this.NumKnot);
                
                    BCurve curveStart=this.CurveByPoz(pozKnotStart);
                    if (curveStart==null)
                    {
                        pozKnotStart=this.PozNext(pozKnotStart);
                        curveStart=this.CurveByPoz(pozKnotStart);
                        if (curveStart==null)
                        {
                            //Debug.Assert(false, "Contour: RayAlmostNormal"); 
                            continue;
                        }
                    }
                    InfoInters inters;
                    if ((!curveStart.IsDegen)&&(curveStart.BBox.Diag>=1-MConsts.EPS_DEC_WEAK)&&
                        (!curveStart.IsSelfInters(out inters)))
                    {
                        double parVal=0.25*rand.Next(1,4);

                        VecD pntStart=curveStart.Evaluate(parVal);
                        VecD dirNorm=curveStart.DirNorm(parVal);
                        if ((pntStart==null)||(dirNorm==null))
                        {
                            //Debug.Assert(false, "Contour: RayAlmostNormal"); 
                            continue;
                        }
                        double angleRot=(rand.NextDouble()-0.5)*(Math.PI/5.0); 
                        double scale=100.0;        // TODO: make constant
                        VecD dirRay=new VecD(scale*(Math.Cos(angleRot)*dirNorm.X-Math.Sin(angleRot)*dirNorm.Y),
                            scale*(Math.Sin(angleRot)*dirNorm.X+Math.Cos(angleRot)*dirNorm.Y));
                        bool isChanged;
                        dirRay.FURound(out isChanged);
                        ray=new RayD(pntStart,pntStart+dirRay);
                        parStartRay=new CParam(parVal,this.KnotByPoz(pozKnotStart));
                        isCurveOK=true;
                    }
                }
                catch (ExceptionGMath)
                {
                }
            }
            return isCurveOK;
        }

        public bool RayParity(RayD ray, CParam parStartRay,
            out MConsts.TypeParity typeParity)
        {
            /*
             *        ASSUMPTIONS 
             *        INPUT:
             *            -    (parStartRay==null) is the ray does not start at 
             *                the contour
             *        RETURN VALUE;
             *            -    (false) in case of real failure; 
             *                (true)+(typeParity==Undef) in unclear cases 
             * 
             */
            typeParity=MConsts.TypeParity.Undef;
            ListInfoInters linters=new ListInfoInters();
            
            bool isStartIntersFound=false;            
            for (int pozKnot=0; pozKnot<this.NumKnot; pozKnot++)
            {
                BCurve curve=this.CurveByPoz(pozKnot);
                if (curve!=null)
                {
                    Knot knot=this.KnotByPoz(pozKnot);
                    int numIntersBefore=linters.Count;
                    if (!Inters.IntersectBL(curve,ray,linters))
                    {
                        linters.ClearDestroy();
                        return false;
                    }
                    int numIntersAfter=linters.Count;
                    if (numIntersAfter!=numIntersBefore)
                    {
                        InfoInters inters;
                        if ((curve.IsDegen)||(curve.IsSelfInters(out inters)))
                        {
                            linters.ClearDestroy();
                            return true;
                        }
                    }
                    bool isRayStartOnCurve=((parStartRay!=null)&&
                        (parStartRay.IndKnot==knot.IndexKnot));
                
                    for (int iInters=numIntersBefore; iInters<numIntersAfter; iInters++)
                    {
                        InfoInters inters=linters[iInters] as InfoInters;
                        if (inters.Dim==InfoInters.TypeDim.Dim1)
                        {
                            linters.ClearDestroy();
                            return true;
                        }
                        IntersD0 intersD0=inters as IntersD0;
                        double parValCurve=intersD0.Ipi.Par(0).Val;
                        double parValRay=intersD0.Ipi.Par(1).Val;
                        if (Math.Abs(parValRay)<MConsts.EPS_DEC)
                        {
                            if ((!isRayStartOnCurve)||(isRayStartOnCurve&&isStartIntersFound))
                            {
                                linters.ClearDestroy();
                                return true;
                            }
                            isStartIntersFound=true;
                        }
                        if ((Math.Abs(parValCurve)<MConsts.EPS_DEC_WEAK)||
                            (Math.Abs(1.0-parValCurve)<MConsts.EPS_DEC_WEAK))
                        {
                            linters.ClearDestroy();
                            return true;
                        }
                        
                        VecD dirTangCurve=curve.DirTang(parValCurve);
                        VecD dirTangRay=(ray as LCurve).DirTang;
                        if ((dirTangCurve==null)||(dirTangRay==null))
                        {
                            linters.ClearDestroy();
                            return true;
                        }
                        if (Math.Abs(dirTangRay.Cross(dirTangCurve))<MConsts.EPS_DEC_WEAK)
                        {
                            linters.ClearDestroy();
                            return true;
                        }
                    }
                    if ((isRayStartOnCurve)&&(!isStartIntersFound))
                    {
                        linters.ClearDestroy();
                        return true;
                    }
                }
            }
            int numIntersAll=(isStartIntersFound)? linters.Count-1: linters.Count;
            typeParity=(numIntersAll%2==0)? MConsts.TypeParity.Even: MConsts.TypeParity.Odd;
            linters.ClearDestroy();
            return true;
        }

        public void InnerOuterBySelfOrientation(out MConsts.TypeInnerOuter typeInnerOuter)
        {
            typeInnerOuter=MConsts.TypeInnerOuter.Undef;

            RayD ray;
            CParam parStartRay;
            MConsts.TypeParity typeParity;
            for (int iTrial=0; iTrial<MConsts.MAX_RAY_INTERS_TRIAL; iTrial++)
            {
                try
                {
                    if (!this.RayAlmostNormal(out ray, out parStartRay)) 
                        continue;
                    // does not count the start of the ray as the intersection
                    if (!this.RayParity(ray, parStartRay, out typeParity)) 
                        continue;
                    if (typeParity==MConsts.TypeParity.Undef) 
                        continue;
                }
                catch (ExceptionGMath)
                {
                    continue;
                }
                typeInnerOuter=(typeParity==MConsts.TypeParity.Even)? 
                    MConsts.TypeInnerOuter.Outer: 
                    MConsts.TypeInnerOuter.Inner;
                return;
            }
        }

        public void InnerOuterByOutline(Outline outl, 
            out MConsts.TypeInnerOuter typeInnerOuter)
        {
            typeInnerOuter=MConsts.TypeInnerOuter.Undef;

            RayD ray;
            CParam parStartRay;
            MConsts.TypeParity typeParityContour, typeParityAll=MConsts.TypeParity.Even; 

            int iTrial;
            for (iTrial=0; iTrial<MConsts.MAX_RAY_INTERS_TRIAL; iTrial++)
            {
                try
                {
                    typeParityAll=MConsts.TypeParity.Even;
                    if (!this.RayAlmostNormal(out ray, out parStartRay))
                    {
                        continue;
                    }
                    int pozCont;
                    for (pozCont=0; pozCont<outl.NumCont; pozCont++)
                    {
                        Contour cont=outl.ContourByPoz(pozCont);
                        if (cont==this)
                            continue;
                        if (!cont.RayParity(ray,null,out typeParityContour))
                            break;
                        if (typeParityContour==MConsts.TypeParity.Undef)
                            break;
                        typeParityAll=(typeParityAll==typeParityContour)? 
                            MConsts.TypeParity.Even: MConsts.TypeParity.Odd;
                    }
                    if (pozCont!=outl.NumCont) 
                        continue;    // parity failed for one of contours
                }
                catch (ExceptionGMath)
                {
                    continue;
                }
                break;                // the result is clear, stop trials
            }
            if (iTrial==MConsts.MAX_RAY_INTERS_TRIAL) 
            {
                return;                // typeInnerOuter is undefined
            }
            typeInnerOuter=(typeParityAll==MConsts.TypeParity.Even)? 
                MConsts.TypeInnerOuter.Outer: MConsts.TypeInnerOuter.Inner;
        }

        public bool AreDuplicated(Contour cont, out bool isDupl)
        {
            isDupl=true;
            if (cont==null)
            {
                throw new ExceptionGMath("Contour","AreDuplicated",null);
                //return false;
            }
            if (this.NumKnot!=cont.NumKnot)
            {
                isDupl=false;
                return true;
            }
            int numKnot=this.NumKnot;
            int pozA=0, pozB;
            for (pozB=0; pozB<numKnot; pozB++)
            {
                if (cont.KnotByPoz(pozB).Val==this.KnotByPoz(0).Val)
                    break;
            }
            if (pozB==numKnot)
            {
                isDupl=false;
                return true;
            }
            int iKnot;
            for (iKnot=0; iKnot<numKnot; iKnot++)
            {
                if (this.KnotByPoz(pozA).Val!=cont.KnotByPoz(pozB).Val)
                    break;
                pozA=this.PozNext(pozA);
                pozB=cont.PozNext(pozB);
            }
            if (iKnot!=numKnot)
            {
                isDupl=false;
                return true;
            }
            return true; // contours are DUPLICATED
        }

        public bool IsMisoriented(Outline outl, out bool isMisoriented)
        {
            isMisoriented=true;
            if (this.NumKnot<=2)
            {
                isMisoriented=false;
                return true;
            }
            MConsts.TypeInnerOuter typeBySelfOrientation, typeByOutline;
            this.InnerOuterBySelfOrientation(out typeBySelfOrientation);
            this.InnerOuterByOutline(outl, out typeByOutline);
            if ((typeBySelfOrientation==MConsts.TypeInnerOuter.Undef)||
                (typeByOutline==MConsts.TypeInnerOuter.Undef))
            {
                //Debug.Assert(false, "Contour: Misoriented");
                return false;
            }
            isMisoriented=(typeBySelfOrientation!=typeByOutline);
            return true;
        }

        public void FURound(out bool isChanged)
        {
            isChanged=false;
            bool isChangedCur;
            foreach (Knot knot in this.knots)
            {
                knot.Val.FURound(out isChangedCur);
                if (isChangedCur)
                {
                    isChanged=true;
                }
            }
        }

        public bool KnotDupl(ListPairInt infosKnotDupl)
        {
            if (infosKnotDupl==null)
            {
                throw new ExceptionGMath("Contour","KnotDupl","Null argument");
                //return false;
            }
            if (this.knots.Count<=1)
                return true;
        
            int numKnot=this.NumKnot;
            int pozKnotFirst=0;
            int shiftFirst=0;


            for (shiftFirst=0; shiftFirst<numKnot; shiftFirst++)
            {
                if (this.KnotByPoz(pozKnotFirst).Val==
                    this.KnotByPoz(this.PozPrev(pozKnotFirst)).Val)
                {
                    pozKnotFirst=this.PozPrev(pozKnotFirst);
                }
                else
                {
                    break;
                }
            }

            // all knots coincide with Kn(0)        
            if (shiftFirst==numKnot)
            {
                infosKnotDupl.Add(this.KnotByPoz(0).IndexKnot,numKnot);
                return true;
            }
            
            
            // not all knots coincide with Kn(0)
            int pozKnotStart=0;
            int pozKnotLast=this.PozPrev(pozKnotFirst);
            
            bool toBreak=false;
            while (!toBreak) // TODO: check
            {
                int pozKnotCur=pozKnotStart;
                int pozKnotNext=this.PozNext(pozKnotCur);
                toBreak=(pozKnotNext==pozKnotLast);

                int multiplicity=1;
                while (this.KnotByPoz(pozKnotCur).Val==
                    this.KnotByPoz(pozKnotNext).Val)
                {
                    pozKnotCur=pozKnotNext;
                    pozKnotNext=this.PozNext(pozKnotNext);
                    toBreak=toBreak||(pozKnotNext==pozKnotLast);
                    multiplicity++;
                }
                if ((pozKnotStart==0)&&((shiftFirst>0)||(multiplicity>1)))
                {
                    infosKnotDupl.Add(this.KnotByPoz(0).IndexKnot,multiplicity+shiftFirst);                        
                }
                else if (multiplicity>1)
                {
                    infosKnotDupl.Add(this.KnotByPoz(pozKnotStart).IndexKnot,multiplicity);
                }
                pozKnotStart=pozKnotNext;
            }
            return true;
        }



        /*
         *        METHODS:    CCURVE
         */

        public void Transform(MatrixD m)
        {
            if (this.knots==null)
                return;
            foreach (Knot knot in this.knots)
                knot.Transform(m);
        }

        public BoxD BBoxCP
        {
            get
            {
                double xMin=MConsts.Infinity;
                double yMin=MConsts.Infinity;
                double xMax=-MConsts.Infinity;
                double yMax=-MConsts.Infinity;
                foreach (Knot knot in this.knots)
                {
                    double x=knot.Val.X;
                    double y=knot.Val.Y;
                    if (x<xMin)
                    {
                        xMin=x;
                    }
                    if (x>xMax)
                    {
                        xMax=x;
                    }
                    if (y<yMin)
                    {
                        yMin=y;
                    }
                    if (y>yMax)
                    {
                        yMax=y;
                    }
                }
                BoxD bboxCP=new BoxD(xMin,yMin,xMax,yMax);
                return bboxCP;
            }
        }

        public BoxD BBox 
        { 
            get
            {
                BoxD bbox=new BoxD();
                for (int poz=0; poz<this.NumKnot; poz++)
                {
                    BCurve bcurve=this.CurveByPoz(poz);
                    if (bcurve!=null)
                    {
                        bbox.Unite(bcurve.BBox);
                    }
                }
                return bbox;
            }
        }
        public Curve Copy()
        {
            throw new ExceptionGMath("Contour","Copy","NOT IMPLEMENTED");
            //return null;
        }

        public bool IsSimple 
        { 
            get { return false;} 
        }
        public bool IsBounded 
        { 
            get    { return true; }
        }
        
        public bool IsDegen 
        { 
            get
            {
                if (this.NumKnot==1)
                    return true;
                int pozKnot;
                for (pozKnot=1; pozKnot<this.NumKnot; pozKnot++)
                {
                    if (this.KnotByPoz(0).Val!=this.KnotByPoz(pozKnot).Val)
                    {
                        break;
                    }
                }
                return (pozKnot==this.NumKnot);
            }
        }

        public void Reverse()
        {
            throw new ExceptionGMath("Contour","Reverse","NOT IMPLEMENTED");
        }
        public Curve Reversed 
        { 
            get
            {
                throw new ExceptionGMath("Contour","Reversed","NOT IMPLEMENTED");
                //return null;
            }
        }

        public bool IsEvaluableWide(Param par)
        {
            throw new ExceptionGMath("Contour","IsEvaluableWide","NOT IMPLEMENTED");
            //return false;
        }
        public bool IsEvaluableStrict(Param par)
        {
            throw new ExceptionGMath("Contour","IsEvaluableStrict","NOT IMPLEMENTED");
            //return false;
        }

        public Param.TypeParam ParamClassify(Param par)
        {
            throw new ExceptionGMath("Contour","ParamClassify","NOT IMPLEMENTED");
            //return Param.TypeParam.Invalid;
        }

        public VecD Evaluate(Param par)
        {
            throw new ExceptionGMath("Contour","Evaluate","NOT IMPLEMENTED");
            //return null;
        }
        public VecD DirTang(Param par)
        {
            throw new ExceptionGMath("Contour","DirTang","NOT IMPLEMENTED");
            //return null;
        }
        public VecD DirNorm(Param par)
        {
            throw new ExceptionGMath("Contour","DirNorm","NOT IMPLEMENTED");
            //return null;
        }
        public double CurveLength(Param parS, Param parE)
        {
            throw new ExceptionGMath("Contour","CurveLength","NOT IMPLEMENTED");
            //return 0;
        }
        public double CurveLength()
        {
            throw new ExceptionGMath("Contour","CurveLength","NOT IMPLEMENTED");
            //return 0;
        }
        public double Curvature(Param par)
        {
            throw new ExceptionGMath("Contour","Curvature","NOT IMPLEMENTED");
            //return 0;
        }
        public VecD Start 
        { 
            get
            {
                throw new ExceptionGMath("Contour","Start","NOT IMPLEMENTED");
                //return null;
            }
        }
        public VecD End 
        { 
            get
            {
                throw new ExceptionGMath("Contour","End","NOT IMPLEMENTED");
                //return null;
            }
        }
        public double ParamStart 
        {
            get
            {
                throw new ExceptionGMath("Contour","ParamStart","NOT IMPLEMENTED");
                //return 0;
            }
        }
        public double ParamEnd 
        { 
            get
            {
                throw new ExceptionGMath("Contour","ParamEnd","NOT IMPLEMENTED");
                //return 0;
            }
        }
        public double ParamReverse 
        { 
            get
            {
                throw new ExceptionGMath("Contour","ParamReverse","NOT IMPLEMENTED");
                //return 0;
            }
        }

        /*
         *        METHODS:    I_DRAWABLE
         */

        public void Draw(I_Draw i_draw, DrawParam dp)
        {
            // for debug only - begin
            /*
            if (this.rayTmp!=null)
            {
                DrawParamCurve dpCurve=new DrawParamCurve("Cyan",1,false,null);
                this.rayTmp.Draw(i_draw, dpCurve);
            }
            */
            // for debug only - end
            DrawParamContour dpContour=dp as DrawParamContour;
            if (dpContour!=null)
            {
                DrawParamKnot dpKnot=dpContour.DPKnot;
                if (dpKnot!=null)
                {
                    foreach (Knot knot in this.knots)
                    {
                        knot.Draw(i_draw,dpKnot);
                    }
                }
                DrawParamCurve dpCurve=dpContour.DPCurve;
                if (dpCurve!=null)
                {
                    for (int poz=0; poz<this.NumKnot; poz++)
                    {
                        BCurve curve=this.CurveByPoz(poz);
                        if (curve!=null)
                        {
                            curve.Draw(i_draw,dpCurve);
                        }
                    }
                }
            }
        }
    }
}