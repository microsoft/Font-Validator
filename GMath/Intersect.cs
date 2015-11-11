using System;
using System.Collections;
using System.Diagnostics;
using System.Reflection;

using TypeDim=NS_GMath.InfoInters.TypeDim;
using Curve=NS_GMath.I_CurveD;
using BCurve=NS_GMath.I_BCurveD;
using LCurve=NS_GMath.I_LCurveD;


namespace NS_GMath
{
    public class Inters
    {

        /*
         *        INTERSECTIONS: (BCurve,BCurve)
         *            - ORDER is IMPORTANT 
         *                order:    if infoConnect are not nulls, => 
         *                        it is supposed that curveA(1)=curveB(0)
         *
         *        NOTES:        -    InfoConnect are supposed to be CORRECT
         *                    -    Curves are maximally reduced in AuxIntersectBB
         *                    -    Self/intersecting Bezier is NOT a reduction
         *                        from bezier
         */
         
        
        public static bool AuxIntersectBB(DegenD degenA, DegenD degenB, 
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(degen,degen)","Null argument");
            }
            bool connectAB = ((icAB!=null)&&(icAB.IsConnect));
            bool connectBA = ((icBA!=null)&&(icBA.IsConnect));
            bool connect = connectAB||connectBA;
            if (connect)  // connect,=> no "real" intersections
            {
                return true;
            }
            if (degenA.Cp==degenB.Cp) // coincide but are not known to be connected
            {
                IntersD0 inters=new IntersD0(Param.Degen,Param.Degen,
                    0.5*(degenA.Cp+degenB.Cp),false);
                linters.Add(inters);
            }
            return true;
        }
        
        public static bool AuxIntersectBB(DegenD degen, SegD seg,
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(degen,seg)","Null argument");
            }
        
            // no reduction
            bool connectAB = ((icAB!=null)&&(icAB.IsConnect));
            if (connectAB)
            {
                return true;
            }
            Param param;
            VecD pnt;
            degen.Cp.Project(seg,out param, out pnt);
            if (degen.Cp.Dist(pnt)<MConsts.EPS_DEC)
            {
                IntersD0 inters=new IntersD0(Param.Degen,param,pnt,false);
                linters.Add(inters);
            }
            return true;
        }

        public static bool AuxIntersectBB(DegenD degen, Bez2D bez,
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(degen,bez)","Null argument");
            }
            
            // no reduction !!
            
            bool connectAB = ((icAB!=null)&&(icAB.IsConnect));
            bool connectBA = ((icBA!=null)&&(icBA.IsConnect));
            bool connect=connectAB||connectBA;
            if (connect)
            {
                return true;
            }

            // bbox check
            if (!bez.BBox.Contains(degen.Cp))
                return true;

            Param[] pars;
            VecD pnt;
            if (!degen.Cp.ProjectGeneral(bez,out pars, out pnt))
                return false;
            if (degen.Cp.Dist(pnt)<MConsts.EPS_DEC)
            {
                Param parM;
                bool isSelfInters=bez.IsSelfInters(out parM);
                for (int iPar=0; iPar<pars.Length; iPar++)
                {
                    IntersD0 inters=new IntersD0(Param.Degen,pars[iPar],pnt,isSelfInters);
                    linters.Add(inters);
                }
            }
            return true;
        }
    
        public static bool AuxIntersectBB(SegD segA, SegD segB,
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            bool connectAB = ((icAB!=null)&&(icAB.IsConnect));
            bool connectBA = ((icBA!=null)&&(icBA.IsConnect));            
            bool connect=connectAB||connectBA;
            

            InfoInters inters;
            if (!Inters.IntersectLL(segA,segB,out inters))
                return false;
            if (inters!=null)
            {
                if ((!connect)||
                    ((connect)&&(inters.Dim==InfoInters.TypeDim.Dim1)))
                {
                    linters.Add(inters);
                }
            }
            return true;
        }

        public static bool AuxIntersectBB(SegD seg, Bez2D bez,
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            // both seg & bez are irreducable !!!
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(seg,bez)","Null argument");
            }
            
            bool connectAB = ((icAB!=null)&&(icAB.IsConnect));
            bool connectBA = ((icBA!=null)&&(icBA.IsConnect));
            if ((connectBA)&&(!connectAB)) 
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(seg,bez)",null);
                //return false;
            }
            bool connect=connectAB||connectBA;

            if (!connect)
            {
                int numIntersBefore=linters.Count;
                if (!Inters.AuxIntersectBL(bez,seg,linters))
                    return false;
                linters.ParamSwap(numIntersBefore);
                return true;
            }

            // bez and seg are connected, => connectAB=true
            Param parM;
            if (bez.IsSelfInters(out parM))
            {
                if (connectBA) // both ends are connected
                {
                    // parM!=Infinity - otherwise the seg is degenerated
                    double valM=parM.Val;
                    IntersD1 inters;
                    if (valM>1)
                    {
                        inters=new IntersD1(0,1,
                            1,1/(2*valM-1),seg,true);
                    }
                    else
                    {
                        inters=new IntersD1(0,1,
                            1,(2*valM)/(2*valM-1),seg,true);
                    }
                    linters.Add(inters);
                    return true;
                }
                if (icAB.IsTangent)
                {
                    return true; // no additional intersections
                }
                else
                {
                    SegD segSupp=bez.SupportFlat();
                    InfoInters inters;
                    if (!Inters.IntersectLL(seg,segSupp,out inters))
                        return false;
                    if (inters==null)
                        return true;
                    inters.ParamInvalidateBezSI();
                    int numIntersBefore=linters.Count;
                    linters.Add(inters);
                    /*
                     *    CLEAN END-POINT if the Bezier does not return to it
                     */
                    bool coversBezStart=bez.CoversEndPoint(true);
                    if (!coversBezStart)
                    {
                        linters.CleanEndPointBezSI(bez.Start,numIntersBefore);
                    }
                    return true;
                }
            }
            
            //    bezier is NOT self-intersecting
            if (connectBA)    
                return true;    // no additional intersections
            if (icAB.IsTangent)
                return true;    // no additional intersections
            
            //    seg & bez are connected and not-tangent,=>
            //    at most one additional point of intersection
            VecD[] cfSeg, cfBez; 
            seg.PowerCoeff(out cfSeg); 
            bez.PowerCoeff(out cfBez);
            VecD tang=(seg as LCurve).DirTang;
            VecD norm=(seg as LCurve).DirNorm;

            // connected but not-tangent: one
            double[] rootsBez;
            int numRootBez;
            Equation.RootsReal(cfBez[2].Dot(norm),cfBez[1].Dot(norm),
                    out numRootBez, out rootsBez); 
            if (numRootBez==Equation.NumRootInfinite)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(seg,bez)",null);
                //return false;
            }
            if (rootsBez==null)
                return true;
            Param parBez=rootsBez[0];
            if (bez.IsEvaluableStrict(parBez))
            {
                double valBez=parBez.Val;
                Param parSeg=1+valBez*(cfBez[2].Dot(tang)*valBez+cfBez[1].Dot(tang))/cfSeg[1].Dot(tang);
                if (seg.IsEvaluableStrict(parSeg)) // ??? && (parSeg!=1)
                {
                    IntersD0 inters=new IntersD0(parSeg,parBez,
                        0.5*(seg.Evaluate(parSeg)+bez.Evaluate(parBez)),false);
                    linters.Add(inters);
                }
            }
            return true;
        }


        public static bool AuxIntersectBB(Bez2D bezA, Bez2D bezB,
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            // bezA and bezB are irreducable !!!

            bool connectAB = ((icAB!=null)&&(icAB.IsConnect));
            bool connectBA = ((icBA!=null)&&(icBA.IsConnect));
            if ((connectBA)&&(!connectAB))
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
            }
            bool connect = connectAB||connectBA;

            Param parM;
            bool isSelfIntersA=bezA.IsSelfInters(out parM);
            bool isSelfIntersB=bezB.IsSelfInters(out parM);
            
            if (isSelfIntersA||isSelfIntersB)
            {
                BCurve curveA=bezA;
                if (isSelfIntersA) 
                    curveA=bezA.SupportFlat();
                BCurve curveB=bezB;
                if (isSelfIntersB)
                    curveB=bezB.SupportFlat();
                int numIntersBefore=linters.Count;
                Inters.IntersectBB(curveA,curveB,null,null,linters);
                /*
                 *    CLEAN END-POINT if the curve does not return to it
                 */
                if ((connectAB)&&(!connectBA))
                {
                    bool coversA1=false;
                    bool coversB0=false;
                    if (isSelfIntersA)
                    {
                        coversA1=bezA.CoversEndPoint(false);
                    }
                    if (isSelfIntersB)
                    {
                        coversB0=bezB.CoversEndPoint(true);
                    }
                    if ((!coversA1)&&(!coversB0))
                    {
                        linters.CleanEndPointBezSI(bezA.End,numIntersBefore);
                    }
                }
                linters.ParamInvalidateBezSI(numIntersBefore);
                return true;
            }

            // test for 1-dimensional intersection of supports
            bool isB0OnA, isB2OnA;
            Param paramAInvB0, paramAInvB2;
            if (!bezB.Cp(0).InverseOn(bezA,out isB0OnA,out paramAInvB0)) 
                return false;
            if (!bezB.Cp(2).InverseOn(bezA,out isB2OnA,out paramAInvB2)) 
                return false;
            if ((isB0OnA)&&(isB2OnA))
            {
                bool areCoincide=true;
                Param par;
                for (int i=1;i<=3;i++)
                {
                    //    evaluate bezB at paramaters 1/4, 1/2, 3/4 and check
                    //    whether the points lie on bezA [-Infinity,Infinity]
                    VecD pnt=bezB.Evaluate(0.25*i);
                    if (!pnt.InverseOn(bezA,out areCoincide,out par)) 
                        return false;
                    if (!areCoincide) 
                        break;
                }
                if (areCoincide)
                {
                    Param.TypeParam typeB0 = bezA.ParamClassify(paramAInvB0);
                    Param.TypeParam typeB2 = bezA.ParamClassify(paramAInvB2);
                    int mult = (int)typeB0*(int)typeB2;
                    
                    if (mult==4)
                    {
                        return true; // no intersections
                    }
                    else if (mult==1)
                    {
                        // bezB is degenerated
                        throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                        //return false;
                    }
                    else if (mult==2) 
                    {
                        // 0-dimentional connection at the end point
                        if ((typeB0==Param.TypeParam.Start)&&
                            (typeB2==Param.TypeParam.Before)) 
                        {
                            if (connect) 
                            {
                                throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                                //return false;    
                            }
                            IntersD0 inters=new IntersD0(0,0,bezB.Start,false);
                            linters.Add(inters);
                            return true;
                        }
                        if ((typeB0==Param.TypeParam.Before)&&
                            (typeB2==Param.TypeParam.Start)) 
                        {
                            if (connect)
                            {
                                throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                                //return false;    
                            }
                            IntersD0 inters=new IntersD0(1,0,bezB.End,false);
                            linters.Add(inters);
                            return true;
                        }
                        if ((typeB0==Param.TypeParam.End)&&
                            (typeB2==Param.TypeParam.After))
                        {
                            if (!connect)
                            {
                                IntersD0 inters=new IntersD0(0,1,bezB.Start,false);
                                linters.Add(inters);
                                return true;
                            }
                            return true;
                        }
                        if ((typeB0==Param.TypeParam.After)&&
                            (typeB2==Param.TypeParam.End)) 
                        {
                            if (connect)
                            {
                                throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                                //return false;    
                            }
                            IntersD0 inters=new IntersD0(1,1,bezB.End,false);
                            linters.Add(inters);
                            return true;
                        }
                    }
                    else if (mult<=0)
                    {
                        InfoInters inters;
                        Inters.RefineIntersBBD1(bezA,bezB,out inters);
                        linters.Add(inters);
                        return true;
                    }
                    throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                    //return false;                        
                }
            }

            /*
             *        INTERSECTION IS 0-DIMENTIONAL AT MOST
             */ 
            VecD[] cfA, cfB;
            bezA.PowerCoeff(out cfA);
            bezB.PowerCoeff(out cfB);
        
            Param parA, parB;
            int numRootB;
            double[] rootsB;
            double kappa=cfA[2].Cross(cfA[1]);

            // bezA and bezB are non-degenerated and consequent
            if (connectAB)
            {
                if (bezA.End!=bezB.Start) 
                {
                    throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                    //return false;                        
                }                    

                if (connectBA)     
                {
                    // both ends are connected
                    if (bezA.Start!=bezB.End) 
                    {
                        throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                        //return false;                        
                    }                    
                        
                    if (icAB.IsTangent||icBA.IsTangent) 
                    {
                        // tangent connection - no additional intersections
                        return true;
                    }

                    double crossA2B2=cfA[2].Cross(cfB[2]);
                    double[] cfEqn=    { kappa*(kappa+2*crossA2B2+cfA[1].Cross(cfB[2])),
                                      -crossA2B2*(2*kappa+crossA2B2),
                                      crossA2B2*crossA2B2};
                    Equation.RootsReal(cfEqn[2],cfEqn[1],cfEqn[0],
                        out numRootB, out rootsB);
                    if (numRootB==Equation.NumRootInfinite)
                    {
                        throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                        //return false;                        
                    }
                    if (rootsB!=null)
                    {
                        for (int iRoot=0; iRoot<numRootB; iRoot++)
                        {
                            parB=rootsB[iRoot];
                            if (bezB.IsEvaluableStrict(parB))
                            {
                                parA=1.0+
                                    parB.Val*(cfA[2].Cross(cfB[2])*parB.Val+
                                    cfA[2].Cross(cfB[1]))/kappa;
                                if (bezA.IsEvaluableStrict(parA) /*&& (parA!=1.)*/)
                                {
                                    IntersD0 inters=new IntersD0(parA,parB,
                                        0.5*(bezA.Evaluate(parA)+bezB.Evaluate(parB)),
                                        false);
                                    linters.Add(inters);
                                }
                            }
                        }
                    }
                    
                    return true;
                }

                // consequent Bezier with one connection
                if (icAB.IsTangent)  
                {
                    // tangent connection - at most 2 additional intersections
                    double[] cfEqn={kappa*(kappa-cfB[2].Cross(cfB[1])),
                                       2*cfA[2].Cross(cfB[2])*kappa,
                                       cfA[2].Cross(cfB[2])*cfA[2].Cross(cfB[2])};
                    Equation.RootsReal(cfEqn[2],cfEqn[1],cfEqn[0],
                        out numRootB, out rootsB);
                    if (numRootB==Equation.NumRootInfinite)
                    {
                        throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                        //return false;                        
                    }
                    if (rootsB!=null)
                    {
                        for (int iRoot=0; iRoot<numRootB; iRoot++)
                        {
                            parB=rootsB[iRoot];
                            if (bezB.IsEvaluableStrict(parB))
                            {
                                parA=1+
                                    parB.Val*(cfA[2].Cross(cfB[2])*parB.Val+
                                    cfA[2].Cross(cfB[1]))/kappa;
                                if (bezA.IsEvaluableStrict(parA)/*&&(parA!=1)*/)
                                {
                                    IntersD0 inters=new IntersD0(parA,parB,
                                        0.5*(bezA.Evaluate(parA)+bezB.Evaluate(parB)),
                                        false);
                                    linters.Add(inters);
                                }
                            }
                        }
                    }
                    return true;
                }
                else 
                {
                    // non-tangent connection - at most 3 additional intersections
                    double[] cfEqn={kappa*(2*cfA[2].Cross(cfB[1])+cfA[1].Cross(cfB[1])),
                                       cfA[2].Cross(cfB[1])*cfA[2].Cross(cfB[1])+
                                       kappa*(2*cfA[2].Cross(cfB[2])+cfA[1].Cross(cfB[2])),
                                       2*cfA[2].Cross(cfB[2])*cfA[2].Cross(cfB[1]),
                                       cfA[2].Cross(cfB[2])*cfA[2].Cross(cfB[2])};
                    Equation.RootsReal(cfEqn[3],cfEqn[2],cfEqn[1],cfEqn[0],
                        out numRootB, out rootsB);
                    if (numRootB==Equation.NumRootInfinite)
                    {
                        throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                        //return false;                        
                    }
                    if (rootsB!=null)
                    {
                        for (int iRoot=0; iRoot<numRootB; iRoot++)
                        {
                            parB=rootsB[iRoot];
                            if (bezB.IsEvaluableStrict(parB))
                            {
                                parA=1+
                                    parB.Val*(cfA[2].Cross(cfB[2])*parB+
                                    cfA[2].Cross(cfB[1]))/kappa;
                                if (bezA.IsEvaluableStrict(parA)/*&&(parA!=1)*/)
                                {
                                    IntersD0 inters=new IntersD0(parA,parB,
                                        0.5*(bezA.Evaluate(parA)+bezB.Evaluate(parB)),
                                        false);
                                    linters.Add(inters);
                                }
                            }
                        }
                    }
                    return true;
                }
            }

            // bezA and bezB are non-degenerated, non-consequent curves
            bool isSwappedAB=false;
            if (Math.Abs(cfA[2].Cross(cfA[1]))<Math.Abs(cfB[2].Cross(cfB[1])))
            {
                kappa = cfB[2].Cross(cfB[1]); 
                isSwappedAB = true;
                VecD tmp;
                for (int i=0; i<3; i++)
                {
                    tmp=cfA[i]; cfA[i]=cfB[i]; cfB[i]=tmp;
                }
            }
            double[] e={cfA[2].Cross(cfB[0]-cfA[0]),
                           cfA[2].Cross(cfB[1]),
                           cfA[2].Cross(cfB[2])};
            double[] f={(cfB[0]-cfA[0]).Cross(cfA[1]),
                           cfB[1].Cross(cfA[1]),
                           cfB[2].Cross(cfA[1])};
            Equation.RootsReal(e[2]*e[2], 
                2*e[2]*e[1], 
                e[1]*e[1]+2*e[2]*e[0]-kappa*f[2],
                2*e[1]*e[0]-kappa*f[1], 
                e[0]*e[0]-kappa*f[0],
                out numRootB, out rootsB);

            if (numRootB==Equation.NumRootInfinite)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBB(bez,bez)",null);
                //return false;                        
            }
            if (rootsB!=null)
            {
                for (int iRoot=0; iRoot<numRootB; iRoot++)
                {
                    parB=rootsB[iRoot];
                    parA=Equation.Evaluate(parB.Val, e[2], e[1], e[0])/kappa;        
                    if (isSwappedAB) 
                    {
                        Param parTmp; 
                        parTmp=parA; 
                        parA=parB; 
                        parB=parTmp;
                    }
                    if (bezA.IsEvaluableStrict(parA)&&bezB.IsEvaluableStrict(parB))
                    {
                        IntersD0 inters=new IntersD0(parA,parB,
                            0.5*(bezA.Evaluate(parA)+bezB.Evaluate(parB)),
                            false);
                        linters.Add(inters);
                    }
                }
            }
            return true;
        }

        public static bool IntersectBB(BCurve curveA, BCurve curveB, 
            InfoConnect icAB, InfoConnect icBA, ListInfoInters linters)
        {
            BoxD bboxA=curveA.BBox;
            BoxD bboxB=curveB.BBox;
            if (!bboxA.HasInters(bboxB))
                return true;

            int numIntersBefore=linters.Count;

            bool connectAB=(icAB!=null)&&(icAB.IsConnect);
            bool connectBA=(icBA!=null)&&(icBA.IsConnect);
            bool toReverseByConnection=(connectBA)&&(!connectAB);

            if (toReverseByConnection)
            {
                if (!Inters.IntersectBB(curveB,curveA,icBA,icAB,linters))
                    return false;
                linters.ParamSwap(numIntersBefore);
                return false;
            }
        
            BCurve redA = curveA.Reduced;
            BCurve redB = curveB.Reduced;
            bool toReverseByComplexity=(redA.BComplexity>redB.BComplexity);

            object[] pars={redA,redB,icAB,icBA,linters};
            if (toReverseByComplexity)
            {
                // TODO: check !!!
                // TODO: what happens with connection info ???
                pars[0]=redB.Reversed;
                pars[1]=redA.Reversed;
            }
            
            Type[] types={pars[0].GetType(),pars[1].GetType(),
                             typeof(InfoConnect),typeof(InfoConnect),typeof(ListInfoInters)};
            MethodInfo infoMethod=typeof(Inters).GetMethod("AuxIntersectBB",types);
            bool res;
            try
            {
                res=(bool)infoMethod.Invoke(null,pars);
            }
            catch(System.Reflection.TargetInvocationException TIException)
            {
                throw TIException.InnerException;
            }
            
            if (toReverseByComplexity)
            {
                linters.ParamReverse(1,0,numIntersBefore); 
                linters.ParamReverse(1,1,numIntersBefore);
                linters.ParamSwap(numIntersBefore);
            }
            if ((object)redA!=(object)curveA)
            {
                linters.ParamFromReduced(curveA,0,numIntersBefore);
            }
            if ((object)redB!=(object)curveB)
            {
                linters.ParamFromReduced(curveB,1,numIntersBefore);
            }

            // clean-up end-point intersections
            linters.CleanEndPointInters(connectAB,connectBA,numIntersBefore);    
            return res;


        }

        /*
         *        INTERSECTIONS: (BCurve,LCurve)
         *            - lcurve is required non-degenerated
         *
         *        NOTES:        -    Curves are maximally reduced in AuxIntersectBL
         *                    -    Self/intersecting Bezier is NOT a reduction
         *                        from bezier
         */
    
        public static bool AuxIntersectBL(DegenD degen, LCurve lrs, ListInfoInters linters)
        {
            //    IMPORTANT (TODO): 
            //    such intersection is NOT VALID for computation of ray's parity
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBL(degen,lrs)","Null argument");
            }
            if (lrs.IsDegen)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBL(degen,lrs)",null);
            }
            if (lrs is SegD)
            {
                return Inters.AuxIntersectBB(degen, lrs as SegD, null, null, linters);
            }

            LineD line=new LineD(lrs);
            Param param;
            VecD  pnt;
            if (!degen.Cp.Project(line,out param,out pnt))
                return false;
            if (degen.Cp.Dist(pnt)<MConsts.EPS_DEC) 
            {
                if (lrs.IsEvaluableStrict(param))
                {
                    IntersD0 inters=new IntersD0(Param.Degen, param, pnt, false);
                    linters.Add(inters);
                }
            }
            return true;
        }

        public static bool AuxIntersectBL(SegD seg, LCurve lrs, ListInfoInters linters)
        {    
            // segment is irreducable !!!
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBL(seg,lrs)","Null argument");
            }
            if (lrs.IsDegen)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBL(seg,lrs)",null);
            }

            InfoInters inters;
            if (!Inters.IntersectLL(seg, lrs, out inters))
                return false;
            if (inters!=null)
                linters.Add(inters);
            return true;
        }

        public static bool AuxIntersectBL(Bez2D bez, LCurve lrs, ListInfoInters linters)
        {
            // bezier is irreducable !!!
            if (linters==null)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBL(bez,lrs)","Null argument");
            }
            if (lrs.IsDegen)
            {
                throw new ExceptionGMath("Intersect","AuxIntersectBL(bez,lrs)",null);
            }
        
            Param parM;
            if (bez.IsSelfInters(out parM))
            {
                if (parM.Val<0)
                {
                    Bez2D bezRev=bez.Reversed as Bez2D;
                    int numIntersBefore=linters.Count;
                    if (!Inters.AuxIntersectBL(bezRev,lrs,linters))
                        return false;
                    linters.ParamReverse(1,0,numIntersBefore);
                    return true;
                }
                SegD support=bez.SupportFlat();
                InfoInters intersSup;
                if (!Inters.IntersectLL(support, lrs, out intersSup))
                    return false;
                if (intersSup==null)
                    return true;
                /*
                 *  convert parameters from support to Bezier
                 */
                // invalidate in case of D1 intersection
                if (intersSup.Dim==InfoInters.TypeDim.Dim1)
                {
                    (intersSup as IntersD1).ParamInvalidateBezSI();
                    linters.Add(intersSup);
                    return true;
                }

                // write as 1 or 2 intersections with different parameters
                // in case of D0 intersections
                InfoInters[] intersBez;
                if (!bez.IntersFromSupport(intersSup,0,out intersBez))
                    return false;
                for (int iIntersBez=0; iIntersBez<intersBez.Length; iIntersBez++)
                {
                    linters.Add(intersBez[iIntersBez]);
                }
                return true;
            }

            // bezier is NOT self/intersecting
            VecD[] cfLrs, cfBez; 
            lrs.PowerCoeff(out cfLrs);
            bez.PowerCoeff(out cfBez);
            VecD norm=lrs.DirNorm;
            VecD tang=lrs.DirTang;
                
            double[] roots;
            int numRootBez;
            Equation.RootsReal(cfBez[2].Dot(norm), 
                cfBez[1].Dot(norm),(cfBez[0]-cfLrs[0]).Dot(norm),
                out numRootBez, out roots);
            if (numRootBez==Equation.NumRootInfinite)
            {
                // bezier is irreducable,=> only D0 intersections are possible
                throw new ExceptionGMath("Intersect","AuxIntersectBL(bez,lrs)",null);
                //return false;
            }
            for (int iRoot=0; iRoot<numRootBez; iRoot++)
            {
                Param parBez=roots[iRoot];
                if (bez.IsEvaluableStrict(parBez))
                {
                    Param parLrs=Equation.Evaluate(parBez.Val,
                        cfBez[2].Dot(tang), cfBez[1].Dot(tang),
                        (cfBez[0]-cfLrs[0]).Dot(tang))/(cfLrs[1].Dot(tang));
                    if (lrs.IsEvaluableStrict(parLrs))
                    {
                        IntersD0 inters=new IntersD0(parBez,parLrs,
                            0.5*(lrs.Evaluate(parLrs.Val)+bez.Evaluate(parBez.Val)),
                            false);
                        linters.Add(inters);
                    }
                }
            }
            return true;
        }

        public static bool IntersectBL(BCurve bcurve, LCurve lrs, ListInfoInters linters)
        {
            if (lrs.IsDegen)
            {
                throw new ExceptionGMath("Intersect","IntersectBL",null);
                //return false;
            }

            BCurve bred = bcurve.Reduced;
            int numIntersBefore=linters.Count;

            object[] pars={bred,lrs,linters};
            Type[] types={pars[0].GetType(),typeof(LCurve),typeof(ListInfoInters)};
            MethodInfo infoMethod=typeof(Inters).GetMethod("AuxIntersectBL",types);
            bool res;
            try
            {
                res=(bool)infoMethod.Invoke(null,pars);
            }
            catch(System.Reflection.TargetInvocationException TIException)
            {
                throw TIException.InnerException;
            }

            if ((object)bred!=(object)bcurve)
            {
                linters.ParamFromReduced(bcurve,0,numIntersBefore);
            }
            return res;
        }




        /*
         *        INTERSECT: (LCurve, LCurve)
         *                - both curves are supposed to be NON-DEGENERATED
         */
        
        public static bool IntersectLL(LCurve lrsA, LCurve lrsB,
            out InfoInters inters)
        {
            inters=null;
            if ((lrsA.IsDegen)||(lrsB.IsDegen))
            {
                throw new ExceptionGMath("Intersect","IntersectLL(lrs,lrs)",null);
            }

            VecD a0=lrsA.Start;
            VecD a1=lrsA.End;
            VecD b0=lrsB.Start;
            VecD b1=lrsB.End;
            VecD dirA=lrsA.DirTang;
            VecD dirB=lrsB.DirTang;
            double det = dirA.Cross(dirB);

            // lrsA and lrsB are not parallel
            if (Math.Abs(det)>MConsts.EPS_DEC) 
            {
                double lenA = (a1-a0).Norm;
                double lenB = (b1-b0).Norm;
                VecD diff = b0-a0;
                Param parA = (diff.Cross(dirB))/(det*lenA);
                Param parB = (diff.Cross(dirA))/(det*lenB);
                if (lrsA.IsEvaluableStrict(parA)&&lrsB.IsEvaluableStrict(parB))
                {
                    VecD pnt = 0.5*(lrsA.Evaluate(parA)+lrsB.Evaluate(parB));
                    inters=new IntersD0(parA,parB,pnt,false);
                }
                return true;
            }

            // lrsA and lrsB are parallel
            LineD lineB=new LineD(lrsB);
            Param paramBInvA0, paramBInvA1;
            VecD pntProjA0, pntProjA1;
            a0.Project(lineB, out paramBInvA0, out pntProjA0);
            a1.Project(lineB, out paramBInvA1, out pntProjA1);
            double distA0=a0.Dist(pntProjA0);
            double distA1=a1.Dist(pntProjA1);

            if ((distA0<MConsts.EPS_DEC)||(distA1<MConsts.EPS_DEC))
            {
                // lrsA and lrsB are colinear
                Param.TypeParam typeA0=lrsB.ParamClassify(paramBInvA0);
                Param.TypeParam typeA1=lrsB.ParamClassify(paramBInvA1);
                int mult=(int)typeA0*(int)typeA1;

                if (mult==4)
                {
                    return true;
                }
                else if (mult==1) 
                {
                    throw new ExceptionGMath("Intersect","IntersectLL(lrs,lrs)",null); // lrsA is degenerated
                    //return false;
                }
                else if (mult==2)
                {
                    if ((typeA0==Param.TypeParam.Start)&&
                        (typeA1==Param.TypeParam.Before))
                    {
                        inters=new IntersD0(0,0,a0,false);
                    }
                    if ((typeA0==Param.TypeParam.Before)&&
                        (typeA1==Param.TypeParam.Start)) 
                    {
                        inters=new IntersD0(1,0,a1,false);
                    }
                    if ((typeA0==Param.TypeParam.End)&&
                        (typeA1==Param.TypeParam.After)) 
                    {
                        inters=new IntersD0(0,1,a0,false);
                    }
                    if ((typeA0==Param.TypeParam.After)&&
                        (typeA1==Param.TypeParam.End))
                    {
                        inters=new IntersD0(1,1,a1,false);
                    }
                    return true;
                }
                else if (mult<=0)
                {
                    return (Inters.RefineIntersLLD1(lrsA,lrsB,out inters));
                }
            }
            
            return true;
        }

            /*
             *        REFINE INTERSECTION D1
             */
                

        public static bool RefineIntersBBD1(BCurve curveA, BCurve curveB,
            out InfoInters inters)
        {
                /*
                 *        ASSUMPTIONS:
                 *            -    curveA & curveB are MAXIMALLY REDUCED
                 *            -    intersection is KNOWN to have dimension D1
                 *            -    does not work in case of SI beziers
                 *        =>    OR: both curveA & curveB are SEGMENTS
                 *            OR:    both curveA & curveB are BEZIER
                 */        
            inters=null;
            InfoInters selfinters;
            if (curveA.IsSelfInters(out selfinters)||curveB.IsSelfInters(out selfinters))
            {
                throw new ExceptionGMath("Intersect","RefineIntersBBD1",null);
                //return false;
            }
            VecD a0=curveA.Start;
            VecD a1=curveA.End;

            Param paramBInvA0, paramBInvA1;
            bool isOn;
            if (!a0.InverseOn(curveB, out isOn, out paramBInvA0)||(!isOn))
                return false;
            if (!a1.InverseOn(curveB, out isOn, out paramBInvA1)||(!isOn))
                return false;
            paramBInvA0.Round(0,1);
            paramBInvA1.Round(0,1);
            bool areCoDirected=(paramBInvA1>=paramBInvA0);
            if (!areCoDirected)
            {
                BCurve revB=curveB.Reversed as BCurve;
                if (!Inters.RefineIntersBBD1(curveA,revB,out inters))
                    return false;
                if (inters!=null)
                {
                    inters.ParamReverse(1,1);
                }
                return true;
            }
            VecD b0=curveB.Start;
            VecD b1=curveB.End;
            Param paramAInvB0, paramAInvB1;
            if (!b0.InverseOn(curveA, out isOn, out paramAInvB0)||(!isOn))
                return false;
            if (!b1.InverseOn(curveA, out isOn, out paramAInvB1)||(!isOn))
                return false;
            paramAInvB0.Round(0,1);
            paramAInvB1.Round(0,1);

            Param paramInA=null, paramInB=null, paramOutA=null, paramOutB=null;
            VecD pntIn=null, pntOut=null;
            if (paramBInvA0<=0)    // before or start
            {
                paramInA=paramAInvB0;
                paramInB=0;
                pntIn=b0;
            }
            else if (paramBInvA0<1)    // inner
            {
                paramInA=0;
                paramInB=paramBInvA0;
                pntIn=a0;
            }
            
            if ((paramBInvA1>=0)&&(paramBInvA1<=1)) // inner or end
            {
                paramOutA=1;
                paramOutB=paramBInvA1;
                pntOut=a1;
            }
            else if (paramBInvA1>1) // after
            {
                paramOutA=paramAInvB1;
                paramOutB=1;
                pntOut=b1;
            }
            if     ((pntIn==null)||(pntOut==null))
            {
                throw new ExceptionGMath("Intersect","RefineIntersBBD1",null);
                //return false;
            }

            Curve curveInters=curveA.SubCurve(paramInA,paramOutA);
            inters=new IntersD1(paramInA,paramInB,
                paramOutA,paramOutB,curveInters,false);
            return true;
        }

        public static bool RefineIntersLLD1(LCurve lrsA, LCurve lrsB,
            out InfoInters inters)
        {
            inters=null;
            if ((lrsA is SegD) && (lrsB is SegD))
            {
                BCurve curveA=lrsA as BCurve;
                BCurve curveB=lrsB as BCurve;
                return Inters.RefineIntersBBD1(curveA, curveB, out inters);
            }
            if (lrsA.LComplexity>lrsB.LComplexity)
            {
                bool res=Inters.RefineIntersLLD1(lrsB,lrsA,out inters);
                if (inters!=null)
                {
                    inters.ParamSwap();
                }
                return res;
            }
            VecD a0=lrsA.Start;
            VecD a1=lrsA.End;
            VecD b0=lrsB.Start;
            VecD b1=lrsB.End;

            Param paramBInvA0, paramBInvA1;
            bool isOn;
            if ((!a0.InverseOn(lrsB, out isOn, out paramBInvA0))||(!isOn))
                return false;
            if ((!a1.InverseOn(lrsB, out isOn, out paramBInvA1))||(!isOn))
                return false;
            Param paramAInvB0, paramAInvB1;
            if ((!b0.InverseOn(lrsA, out isOn, out paramAInvB0))||(!isOn))
                return false;
            if ((!b1.InverseOn(lrsA, out isOn, out paramAInvB1))||(!isOn))
                return false;

            bool areCoDirected=(paramBInvA1.Val>=paramBInvA0.Val);
            if (!areCoDirected)
            {
                if (lrsA is LineD)
                {
                    if (lrsB is LineD)
                    {
                        paramAInvB0=(areCoDirected)? -Param.Infinity: Param.Infinity;
                        paramAInvB1=(areCoDirected)? Param.Infinity: -Param.Infinity;
                        
                        inters=new IntersD1(paramAInvB0,-Param.Infinity, 
                            paramAInvB1,Param.Infinity,lrsB,false);
                        return true;
                    }
                    if (lrsB is RayD)
                    {
                        paramAInvB1=(areCoDirected)? Param.Infinity: -Param.Infinity;
                        inters=new IntersD1(paramAInvB0,0,
                            paramAInvB1,Param.Infinity,lrsB,false);
                        return true;
                    }
                    if (lrsB is SegD)
                    {
                        inters=new IntersD1(paramAInvB0,0,
                            paramAInvB1,1,lrsB,false);
                        return true;
                    }
                }
                if (lrsA is RayD)
                {
                    if (lrsB is RayD)
                    {
                        if (areCoDirected)
                        {
                            if (paramAInvB0>0)
                            {
                                inters=new IntersD1(paramAInvB0,0,
                                    Param.Infinity,Param.Infinity,lrsB,false);
                                return true;
                            }
                            else 
                            {
                                inters=new IntersD1(0,paramBInvA0,
                                    Param.Infinity,Param.Infinity,lrsA,false);
                                return true;
                            }
                        }
                        else
                        {
                            if (paramAInvB0>0)
                            {
                                inters=new IntersD1(0,paramBInvA0,
                                    paramAInvB0,0,new SegD(a0,b0),false);
                                return true;
                            }
                        }
                    }
                    if (lrsB is SegD)
                    {
                        // intersection is known to have dimension D1 !!!
                        if ((paramBInvA0>=1)||(paramBInvA0<=0))
                        {
                            inters=new IntersD1(paramAInvB0,0,
                                paramAInvB1,1,new SegD(b0,b1),false);
                            return true;
                        }
                        if ((0<paramBInvA0)&&(paramBInvA1<1))
                        {
                            if (areCoDirected)
                            {
                                inters=new IntersD1(0,paramBInvA0,
                                    paramAInvB1,1,new SegD(a0,b1),false);
                                return true;
                            }
                            else
                            {
                                inters=new IntersD1(0,paramBInvA0,
                                    paramAInvB0,0,new SegD(a0,b0),false);
                                return true;
                            }
                        }
                    }
                }
            }
            throw new ExceptionGMath("Intersect","RefineIntersLLD1",null);
            //return false;
        }        
    }
}
             

