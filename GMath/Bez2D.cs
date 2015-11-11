using System;
using NS_IDraw;

using System.Diagnostics;
using Curve=NS_GMath.I_CurveD;
using BCurve=NS_GMath.I_BCurveD;
using LCurve=NS_GMath.I_LCurveD;


namespace NS_GMath
{

    public class Bez2D : BCurve
    {
        /*
         *        MEMBERS
         */
        private VecD[] cp;

        /*
         *        CONSTRUCTORS
         */
        private Bez2D()
        {
            this.cp=new VecD[3];
        }
        public Bez2D(VecD start, VecD mid, VecD end)
            : this()
        {
            this.cp[0]=new VecD(start);
            this.cp[1]=new VecD(mid);
            this.cp[2]=new VecD(end);
        }
        public Bez2D(Bez2D bez) 
            : this()
        {
            this.cp[0]=new VecD(bez.Cp(0));
            this.cp[1]=new VecD(bez.Cp(1));
            this.cp[2]=new VecD(bez.Cp(2));
        }
        /*
         *        METHODS
         */
        public bool IsSeg(out Param parM)
        {    
            /*
             *        MEANING: NON_DEGENERATED, PARAMETRIC segment
             *                 (implies, that the Bezier is not 
             *                 self-intersecting)
             */
            parM=null;
            if (this.IsDegen)
                return false;
            if (this.cp[0]==this.cp[2])
                return false; // S/I bezier
            if (this.cp[0]==this.cp[1])
            {
                parM=new Param(0);
                return true;
            }
            if (this.cp[1]==this.cp[2])
            {
                parM=new Param(1);
                return true;
            }
            SegD seg=new SegD(this.cp[0], this.cp[2]);
            LineD line=new LineD(seg);
            VecD pnt;
            Param par;
            this.cp[1].Project(line,out par,out pnt);
            if (this.cp[1].Dist(pnt)>MConsts.EPS_DEC) 
                return false;
            if (!seg.IsEvaluableStrict(par))
                return false;
            parM=par;
            if (Math.Abs(parM.Val-0.5)<MConsts.EPS_DEC)
            {
                parM.Val=0.5;
            }
            return true;
        }

        public bool IsSelfInters(out InfoInters inters)
        {
            /*
             *        RETURN VALUE: true if Self-Intersecting; false otherwise
             */
            inters=null;
            Param parM;
            if (!IsSelfInters(out parM))
                return false;
            if (parM.Val==Param.Infinity)
            {
                inters=new IntersD1(0,null,1,null,this,true);
            }
            else if (parM>1)
            {
                Param paramStart=1.0/(2.0*parM.Val-1.0);
                BCurve curveInters=this.SubCurve(paramStart,1);
                if (curveInters==null)
                {
                    throw new ExceptionGMath("Bez2D","IsSelfInters","");
                }
                inters=new IntersD1(paramStart,null,1.0,null,curveInters,true);
            }
            else if (parM<0)
            {
                Param paramEnd=(-2.0*parM.Val)/(1.0-2.0*parM.Val);
                BCurve curveInters=this.SubCurve(0,paramEnd);
                if (curveInters==null)
                {
                    throw new ExceptionGMath("Bez2D","IsSelfInters","");
                }
                inters=new IntersD1(0.0,null,paramEnd,null,curveInters,true);
            }
            return true;
        }

        public bool CoversEndPoint(bool isStart)
        {
            /*
             *        ASSUMPTION:
             *            The Bezier is KNOWN to be SELF-INTERSECTING
             *        MEANING:
             *            Determines whether a SELF-INTERSECTING Bezier covers
             *            its end-point more than once
             */
            if (this.cp[0]==this.cp[2])
                return true;
            if (isStart)
            {
                VecD vec01=this.cp[1]-this.cp[0];
                VecD vec02=this.cp[2]-this.cp[0];
                return (vec01.Dot(vec02)<0);
            }
            else
            {
                VecD vec21=this.cp[1]-this.cp[2];
                VecD vec20=this.cp[0]-this.cp[2];
                return (vec21.Dot(vec20)<0);
            }
        }

        public bool IsSelfInters(out Param parM)
        {    
            /*
             *        RETURN VALUE: true if Self-Intersecting; false otherwise
             */
            parM=null;
            if (this.IsDegen)
                return false;
            Param parSeg;
            if (this.IsSeg(out parSeg))
                return false;
            SegD seg=new SegD(this.cp[0], this.cp[2]);
            if (seg.IsDegen)
            {
                parM=Param.Infinity;
                return true;
            }
            LineD line=new LineD(seg);
            VecD pnt;
            this.cp[1].Project(line,out parM,out pnt);
            if (this.cp[1].Dist(pnt)>MConsts.EPS_DEC) 
                return false;
            return true;
        }        
        Curve Curve.Copy()
        {
            return new Bez2D(this);
        }
        public VecD Cp(int i)
        {
            if ((i<0)||(i>2))
                return null;
            return this.cp[i];
        }
        public bool IsSimple 
        {
            get { return true; }
        }
        public bool IsBounded 
        { 
            get { return true; }
        }
        public bool IsDegen    
        { 
            get
            {
                return ((this.cp[0]==this.cp[1])&&(this.cp[1]==this.cp[2]));
            }
        }

        public BCurve Reduced
        {
            /*
             *        Returns:
             *            SegD    if the bezier can be REPARAMETERIZED as a 
             *                    NON-DEGENERATED segment (it implies that the 
             *                    bezier has no self-intersections and is not a
             *                    point)
             *            DegenD    if the bezier is degenerated: all three 
             *                    control points lie at the same location
             */
            get 
            {
                if (this.IsDegen)
                {
                    return new DegenD(this.Middle);
                }
                Param parM;
                if (this.IsSeg(out parM))
                {
                    return new SegD(this.cp[0], this.cp[2]);
                }            
                return this;
            }
        }

        
/*
 
// meaning: 
//    can be REPARAMETERIZED as a NON-DEGENERATED segment between S(tart) & E(nd)
//    => has no self-intersections and is not a point 
bool Bez2D::IsSeg(double &parM)
{
}

// meaning: 
//    control points A,B,C are collinear 
// examples:        A=B=C                       true
//                  A=C != B                    true
//                  B is outside of [AC]        true
bool Bez2D::IsFlat()
{
    if (IsPnt()) return true;
    LineD line(mCp[0],mCp[2]);
    if (line.IsDegen()) return true;
    InfoPntPar ippM;
    double distM;
    mCp[1].Dist(line, distM, ippM);
    return (distM<epsD);
}

// meaning:
//      has 1D self-intersections 
bool Bez2D::IsSelfInters()
{
    double parM;
    return (IsFlat() && (!IsSeg(parM)) && (!IsPnt()));
}

double Bez2D::ParFromSeg(double parM, double parSeg, Boolean &isSamePnt)
{
    isSamePnt = true;
    
    if (fabs(parM-0.5)<epsD) return parSeg;

    double D = parM*parM+(1.-2.*parM)*parSeg;
    D = (fabs(D)<epsD) ? 0.0 : D;
    if (D<0) 
    {
        D = 0.;
        isSamePnt = false;
    }
    double parBez;
    if (parM<0.3)
    {
        parBez = (-parM + sqrt(D))/(1.-2.*parM); //unstable for parM -> 0.5
    }
    else if ((0.3<parM)&&(parM<0.5))
    {
        parBez = parSeg/(parM+sqrt(D));
    }
    else if ((0.5<parM)&&(parM<0.7))
    {
        parBez = parSeg/(parM-sqrt(D));
    }
    else if (parM>0.7)
    {
        parBez = (-parM - sqrt(D))/(1.-2.*parM);
    }
    return parBez;
}

*/


        public double CurveLength(Param parS, Param parE)
        {
            throw new ExceptionGMath("Bez2D","CurveLength","NOT IMPLEMENTED");
            //return 0;
        }
        public double CurveLength()
        {
            throw new ExceptionGMath("Bez2D","CurveLength","NOT IMPLEMENTED");
            //return 0;
        }
        public void Reverse()
        {
            VecD tmp=this.cp[0]; 
            this.cp[0]=this.cp[2]; 
            this.cp[2]=tmp;
        }
        public bool IsEvaluableWide(Param par)
        {
            if (!par.IsValid)
                return false;
            if (Math.Abs(par.Val)==Param.Infinity)
                return false;
            if ((par.Val==Param.Degen)&&(!this.IsDegen))
                return false;
            return true;
        }
        public bool IsEvaluableStrict(Param par)
        {
            par.Round(this.ParamStart,this.ParamEnd);
            return ((0<=par)&&(par<=1));
        }
        public VecD Evaluate(Param par)
        {
            if (!this.IsEvaluableWide(par))
            {
                throw new ExceptionGMath("VecD","Evaluate",null);
            }
            VecD vec=(1-par)*(1-par)*this.cp[0]+
                2*par*(1-par)*this.cp[1]+
                par*par*this.cp[2];
            return vec;
        }

        public VecD DirTang(Param par)
        {
            if (this.IsDegen)
                return null;
            Param parM;
            if (this.IsSeg(out parM))
            {
                SegD seg=new SegD(this.cp[0],this.cp[2]);
                return ((seg as LCurve).DirTang);
            }
            if (this.IsSelfInters(out parM))
            {
                SegD support=this.SupportFlat();
                return ((support as LCurve).DirTang);
            }

            VecD tan=2.0*(1.0-par.Val)*(this.cp[1]-this.cp[0])+
                2.0*par.Val*(this.cp[2]-this.cp[1]);
            if (tan.Norm<MConsts.EPS_COMP)
                return null;
            return ((1.0/tan.Norm)*tan);
        }

        public VecD DirNorm(Param par)
        {
            VecD tan=this.DirTang(par);
            if ((object)tan==null)
                return null;
            return (new VecD(-tan.Y,tan.X));
        }



        public double Curvature(Param par)
        {
            throw new ExceptionGMath("Bez2D","Curvature","NOT IMPLEMENTED");
            //return 0;
        }
        public VecD Start 
        { 
            get { return this.cp[0]; }
        }
        public VecD End 
        { 
            get { return this.cp[2]; }
        }
        public double ParamStart 
        {
            get { return 0; }
        }
        public double ParamEnd 
        { 
            get { return 1; }
        }
        public double ParamReverse
        {
            get { return 1; }
        }

        public VecD Middle
        {
            get { return this.Evaluate(0.5); }
        }
        public SegD SupportFlat()
        {
            Param parM;
            if ((this.IsDegen)||(this.IsSeg(out parM)))
            {
                return new SegD(this.Start,this.End);
            }
            if (this.IsSelfInters(out parM))
            {
                double valM=parM.Val;
                if (valM==Param.Infinity)
                {
                    return new SegD(this.Start,0.5*(this.Start+this.Cp(1)));
                }
                if (valM>1)
                {
                    return new SegD(this.Start, this.Start+((valM*valM)/(2*valM-1))*(this.End-this.Start));
                }
                if (parM.Val<0)
                {
                    Bez2D bezRev=this.Reversed as Bez2D;
                    SegD supportRev= bezRev.SupportFlat();
                    return supportRev.Reversed as SegD;
                }
            }
            return null;
        }
        public Param.TypeParam ParamClassify(Param par)
        {
            par.Round(this.ParamStart,this.ParamEnd);
            double val=par.Val;
            if ((!par.IsValid)||(par.IsDegen)) 
            {
                return Param.TypeParam.Invalid;
            }
            if (val<0)  
            {
                return Param.TypeParam.Before;
            }
            else if (val==0) 
            {
                return Param.TypeParam.Start;
            }
            else if (val<1)  
            {
                return Param.TypeParam.Inner;
            }
            else if (val==1) 
            {
                return Param.TypeParam.End;
            }
            else
            {
                return Param.TypeParam.After;
            }
        }
        public RayD.TypeParity RayParity(RayD ray, bool isStartOnGeom)
        {
            throw new ExceptionGMath("Bez2D","RayParity", "NOT IMPLEMENTED");
            //return RayD.TypeParity.ParityUndef;
        }
        
        public void Transform(MatrixD m)
        {
            this.cp[0].Transform(m);
            this.cp[1].Transform(m);
            this.cp[2].Transform(m);
        }

        public void PowerCoeff(out VecD[] pcf)
        {
            pcf=new VecD[3];
            pcf[2]=this.cp[0]-2*this.cp[1]+this.cp[2];
            pcf[1]=2*(this.cp[1]-this.cp[0]);
            pcf[0]=this.cp[0];
        }

        public BoxD BBox 
        { 
            get
            {
                Param parM;
                if (this.IsSelfInters(out parM))
                {
                    SegD support=this.SupportFlat();
                    return support.BBox;
                }

                BCurve bcurve=this.Reduced;
                if (!(bcurve is Bez2D))
                {
                    return bcurve.BBox;
                }

                VecD[] pcf;
                this.PowerCoeff(out pcf);
                double parExtrX=0, parExtrY=0;
                if (Math.Abs(pcf[2].X)>MConsts.EPS_DEC)
                {
                    parExtrX=-pcf[1].X/(2*pcf[2].X);
                }
                if (Math.Abs(pcf[2].Y)>MConsts.EPS_DEC)
                {
                    parExtrY=-pcf[1].Y/(2*pcf[2].Y);
                }
                double extrX=this.cp[0].X;
                double extrY=this.cp[0].Y;
                if ((0<=parExtrX)&&(parExtrX<=1))
                {
                    extrX=this.Evaluate(parExtrX).X;
                }
                if ((0<=parExtrY)&&(parExtrY<=1))
                {
                    extrY=this.Evaluate(parExtrY).Y;
                }
                double xMin = Math.Min(Math.Min(this.cp[0].X,this.cp[2].X),extrX);
                double yMin = Math.Min(Math.Min(this.cp[0].Y,this.cp[2].Y),extrY);
                double xMax = Math.Max(Math.Max(this.cp[0].X,this.cp[2].X),extrX);
                double yMax = Math.Max(Math.Max(this.cp[0].Y,this.cp[2].Y),extrY);
                BoxD box=new BoxD(xMin,yMin,xMax,yMax);
                return box;
            }
        }

        public bool ParamFromSupport(Param parSupport, out Param[] parsBez)
        {
            parsBez=null;
            parSupport.Round(0,1);
            if ((parSupport.Val<0)||(parSupport.Val>1))
            {
                throw new ExceptionGMath("Bez2D","ParamFromSupport",null);
                //return false;
            }

            Param parM;
            if (!this.IsSelfInters(out parM))
            {
                parsBez=new Param[1];
                parsBez[0]=parSupport.Copy();
                this.ParamFromSeg(parsBez[0]);
            }

            // self-intersecting Bezier
            double valM=parM.Val;
            if (valM==Param.Infinity)
            {
                double tau=0.5*parSupport.Val; // parameter with respect to [Cp(0),Cp(1)]
                parsBez=new Param[2];
                parsBez[0]=new Param(0.5*(1-Math.Sqrt(1-2*tau)));
                parsBez[1]=new Param(0.5*(1+Math.Sqrt(1-2*tau)));
            }
            else if (valM>1)
            {
                double tau=((valM*valM)/(1-2*valM))*parSupport.Val;
                double D=valM*valM+tau*(1-2*valM);
                if (Math.Abs(D)<MConsts.EPS_DEC)
                {
                    D=0;
                }
                if (D<0)
                {
                    throw new ExceptionGMath("BezD","ParamFromSegGeneral",null);
                    //return false;
                }
                if (tau<1)        
                {
                    parsBez=new Param[1];
                    parsBez[0]=new Param((valM-Math.Sqrt(D))/(1-2*valM));
                }
                else // 1<=tau<=(valM*valM)/(1-2*valM)
                {
                    parsBez=new Param[2];
                    parsBez[0]=new Param((valM-Math.Sqrt(D))/(1-2*valM));
                    parsBez[1]=new Param((valM+Math.Sqrt(D))/(1-2*valM));
                }
                return true;
            }
            else
            {
                // valM<0
                Bez2D bezRev=this.Reversed as Bez2D;
                if (!bezRev.ParamFromSupport(1-parSupport.Val, out parsBez))
                    return false;
                if (parsBez==null)
                    return true;
                for (int iParBez=0; iParBez<parsBez.Length; iParBez++)
                {
                    parsBez[iParBez].Reverse(1);
                }
                return true;
            }
            throw new ExceptionGMath("Bez2D","ParamFromSupport","NOT IMPLEMENTED");
            //return false;
        }
        
        public bool IntersFromSupport(InfoInters intersSup, int indBez, 
            out InfoInters[] intersBez)
        {
            /*
             *        ASSUMPTION: works for D0 intersections ONLY!!!
             */
            intersBez=null;
            if (intersSup==null)
                return true;
            if (intersSup.Dim==InfoInters.TypeDim.Dim1)
            {
                throw new ExceptionGMath("Bez2D","IntersFromSupport","NOT IMPLEMENTED");
                //return false;
            }
            InfoParamInters ipiSup=(intersSup as IntersD0).Ipi;
            Param[] parsBez;
            if (!this.ParamFromSupport(ipiSup.Par(indBez), out parsBez))
                return false;
            intersBez=new IntersD0[parsBez.Length];
            for (int iPar=0; iPar<parsBez.Length; iPar++)
            {
                if (indBez==0)
                {
                    intersBez[iPar]=new IntersD0(parsBez[iPar],ipiSup.Par(1),
                        (intersSup as IntersD0).PntInters, true);
                }
                else
                {
                    intersBez[iPar]=new IntersD0(ipiSup.Par(0),parsBez[iPar],
                        (intersSup as IntersD0).PntInters, true);
                }
            }
            return true;
        }

        public int BComplexity
        {
            get {return 3;}
        }
        public Curve Reversed
        {
            get 
            {
                Bez2D bez=new Bez2D(this);
                bez.Reverse();
                return bez;
            }
        }
        public bool ParamFromSeg(Param par)
        {
            double valSeg=par.Val;

            if ((valSeg<0)||(valSeg>1))
            {
                throw new ExceptionGMath("Bez2D","ParamFromSeg","");
                //return false;
            }
            Param parM;
            if (!this.IsSeg(out parM))
            {
                throw new ExceptionGMath("Bez2D","ParamFromSeg","");
                //return false;
            }
            double valM=parM.Val;

            if (Math.Abs(valM-0.5)<MConsts.EPS_DEC) 
                return true;

            double D = valM*valM+(1-2*valM)*valSeg;
            if (Math.Abs(D)<MConsts.EPS_DEC)
            {
                D=0;
            }
            if (D<0)
            {
                throw new ExceptionGMath("Bez2D","ParamFromSeg","");
                //return false;
            }
            double valBez=0;
            if ((0.3<valM)&&(valM<0.7)) // unstable for valM --> 0.5
            {
                valBez = valSeg/(valM+Math.Sqrt(D));
            }
            else 
            {
                valBez = (-valM+Math.Sqrt(D))/(1-2*valM);
            }
            par.Val=valBez;
            par.Round(0,1);
            if (!((par.Val>=0.0)&&(par.Val<=1.0)))
            {
                throw new ExceptionGMath("Bez2D","ParamFromSeg","");
                //return false;
            }
            return true;
        }


        public void Intersect(I_CurveD curveA, I_CurveD curveB /*, out...*/)
        {
            throw new ExceptionGMath("Bez2D","Intersect","NOT IMPLEMENTED");
        }

        public void Subdivide(Param par, out BCurve curveS, out BCurve curveE)
        {
            /*
             *        ASSUMPTION: par should be in the range [0,1]
             */
            curveS=null;
            curveE=null;
            if (!this.IsEvaluableStrict(par))
                return;
            VecD pnt01=(1-par.Val)*this.cp[0]+par.Val*this.cp[1];
            VecD pnt12=(1-par.Val)*this.cp[1]+par.Val*this.cp[2];
            VecD pnt = (1-par.Val)*pnt01+par.Val*pnt12;
            curveS=new Bez2D(this.cp[0],pnt01,pnt);
            curveE=new Bez2D(pnt,pnt12,this.cp[2]);
        }

        
        public BCurve SubCurve(Param parA, Param parB)
        {
            if ((!this.IsEvaluableStrict(parA))||(!this.IsEvaluableStrict(parB)))
                return null;

            Bez2D bezWork=new Bez2D(this);
            double valA=parA.Val;
            double valB=parB.Val;
            if (valA>valB)
            {
                bezWork.Reverse();
                valA=1-valA;
                valB=1-valB;
            }
            /*
            if ((valS==0)&&(valE==1))
                return new Bez2D(bezWork);
            CurveD curveS, curveE;
            if (valS==0)
            {
                bezWork.Subdivide(valB,out curveS, out curveE);
                return curveS;
            }
            if (valE==1)
            {
                bezWork.Subdivide(valA,out curveS, out curveE);
                return curveE;
            }
            */
            VecD a01,a12,b01,b12;
            a01=(1-valA)*bezWork.Cp(0)+valA*bezWork.Cp(1);
            a12=(1-valA)*bezWork.Cp(1)+valA*bezWork.Cp(2);
            b01=(1-valB)*bezWork.Cp(0)+valB*bezWork.Cp(1);
            b12=(1-valB)*bezWork.Cp(1)+valB*bezWork.Cp(2);

            VecD start, mid, end;
            start=(1-valA)*a01+valA*a12;
            end=(1-valB)*b01+valB*b12;
            mid=(1-valB)*a01+valB*a12; 
            // mid=(1-valA)*(1-valB)*bezWork.Cp(0)+valA*valB*bezWork.Cp(2)+
            //        ((1-valB)*valA+(1-valA)*valB))*bezWork.Cp(1);

            return new Bez2D(start,mid,end);
        }

        /*
         *        I_DRAWABLE
         */
        public void Draw(I_Draw i_draw, DrawParam dp)
        {
            DrawParamCurve dpCurve=dp as DrawParamCurve;
            if (dpCurve==null)
                return;
            i_draw.DrawBez2(this.cp[0].X,this.cp[0].Y,
                this.cp[1].X,this.cp[1].Y,
                this.cp[2].X,this.cp[2].Y,
                dpCurve.StrColor, dpCurve.ScrWidth);
            if (dpCurve.ToDrawEndPoints)
            {
                this.cp[0].Draw(i_draw, dpCurve.DPEndPoints);
                this.cp[2].Draw(i_draw, dpCurve.DPEndPoints);
            }
        }
    }
}