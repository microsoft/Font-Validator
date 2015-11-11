using System;
using System.Diagnostics;
using System.Collections;

namespace NS_GMath
{
    public class Equation
    {
        public const int NumRootInfinite = -1;

        /*
         *        EVALUATE
         */
        public static double Evaluate(double par, params double[] cf)
        {
            // cf[0]*par^(n-1)+cf[1]*par^(n-2)+...+cf[n-2]*par+cf[n-1]
            if ((cf==null)||(cf.Length==0))
            {
                throw new ExceptionGMath("Equation","Evaluate",null);
            }
            int n=cf.Length;
            double res=0.0;
            for (int i=0; i<n; i++)
            {
                res=res*par+cf[i];
            }
            return res;
        }

        /*
         *        COMPLEX ROOTS
         */
        // deg0
        public static void RootsAll(double a, 
            out int numRoot, out int numRootReal, out CNum[] root)
        {
            numRoot=(Math.Abs(a)>MConsts.EPS_DEC)? 0: Equation.NumRootInfinite;
            numRootReal=numRoot;
            root=null;
        }

        // deg1
        public static void RootsAll(double a, double b,
            out int numRoot, out int numRootReal, out CNum[] root)
        {
            if (Math.Abs(a)<MConsts.EPS_DEC)
            {
                Equation.RootsAll(b,out numRoot,out numRootReal,out root);
            }
            else
            {
                numRoot=1;
                numRootReal=numRoot;
                root=new CNum[1];
                root[0]=new CNum(-b/a,0);
            }
        }

        // deg2
        public static void RootsAll(double a, double b, double c,
            out int numRoot, out int numRootReal, out CNum[] root)
        {
            // multiply root is referred as different roots;
            // in case of multiply roots output: 
            //        numRoot=2; root[0]=root[1]; 
            //        root[0].Multiplicity=root[1].Multiplicity=2 
            // TODO: check LOGIC above !!!

            if (Math.Abs(a)<MConsts.EPS_DEC)
            {
                Equation.RootsAll(b,c,out numRoot, out numRootReal, out root);
            }
            else
            {
                numRoot = 2;
                root=new CNum[2];

                double D = b*b - 4.0*a*c;
                if (D < 0) // TODO: check D<=-MConsts.EPS_DEC  
                {
                    numRootReal = 0;
                    double sqrtD = Math.Sqrt(Math.Abs(D));
                    root[0]=new CNum(-b/(2.0*a),-sqrtD/(2.0*a));
                    root[1]=new CNum(-b/(2.0*a), sqrtD/(2.0*a));            
                } 
                else
                {
                    numRootReal = 2;
                    if (D==0)
                    {
                        root[0]=new CNum(-b/(2.0*a), 0.0, 2);    // TODO: CHECK MULTIPLICITY !!!
                        root[1]=new CNum(-b/(2.0*a), 0.0, 2);
                    }
                    else
                    {
                        double sqrtD = Math.Sqrt(D);
                        root[0]=new CNum((-b-sqrtD)/(2.0*a), 0);
                        root[1]=new CNum((-b+sqrtD)/(2.0*a), 0);
                    }
                }
            }
        }

        // deg3
        public static void RootsAll(double a, double b, double c, double d,
            out int numRoot, out int numRootReal, out CNum[] root)
        {
            //    output:    
            //    case of 3 real roots => 
            //            the roots are sorted in increasing order
            //    case of 1 real root  => real, complex-, complex+
            //    
            //    multiply real roots referred as different roots

            /*
             *  Catch EXACT TANGENTS when they are not obvious
             *    from the control points specification
             */
            
            double rootToAdd=-1.0;                  // asign an invalid parametric value
            int numRootRed=-1, numRootRealRed=-1; // roots after reduction
            CNum[] rootRed=null;
            if (Equation.Evaluate(0.0,a,b,c,d)==0.0)
            {
                rootToAdd=0.0;
                Equation.RootsAll(a,b,c,
                    out numRootRed, out numRootRealRed, out rootRed);
            }
            if (Equation.Evaluate(1.0,a,b,c,d)==0.0)
            {
                rootToAdd=1.0;
                Equation.RootsAll(a,a+b,a+b+c,
                    out numRootRed, out numRootRealRed, out rootRed);
            }
            if (rootToAdd!=-1.0)
            {
                root=new CNum[numRootRed+1];
                numRoot=numRootRed+1;
                numRootReal=numRootRealRed+1;
                //
                //  two complex roots of the reduced (quadratic) equation
                //
                if ((numRootRed-numRootRealRed)==2)
                {
                    root[0]=new CNum(rootToAdd,0.0);
                    root[1]=new CNum(rootRed[0]);
                    root[2]=new CNum(rootRed[1]);
                    return;
                }
                //
                //  all roots of the reduced equation are real
                //
                CNum rootNew=new CNum(rootToAdd,0.0);
                // adjust multiplicities
                for (int iRootRed=0; iRootRed<numRootRed; iRootRed++)
                {
                    if (rootToAdd==rootRed[iRootRed].Re)
                    {
                        rootNew.Multiplicity+=1;
                        rootRed[iRootRed].Multiplicity+=1;
                    }
                }
                if (numRootRed==0)
                {
                    root[0]=new CNum(rootNew);
                    return;
                }
                if (numRootRed==1)
                {
                    if (rootToAdd<=rootRed[0].Re)
                    {
                        root[0]=new CNum(rootNew);
                        root[1]=new CNum(rootRed[0]);
                    }
                    else
                    {
                        root[0]=new CNum(rootRed[0]);
                        root[1]=new CNum(rootNew);
                    }
                }
                if (numRootRed==2)
                {
                    if (rootToAdd<=rootRed[0].Re)
                    {
                        root[0]=new CNum(rootNew);
                        root[1]=new CNum(rootRed[0]);
                        root[2]=new CNum(rootRed[1]);
                    }
                    else if (rootToAdd<=rootRed[1].Re)
                    {
                        root[0]=new CNum(rootRed[0]);
                        root[1]=new CNum(rootNew);
                        root[2]=new CNum(rootRed[1]);
                    }
                    else
                    {
                        root[0]=new CNum(rootRed[0]);
                        root[1]=new CNum(rootRed[1]);
                        root[2]=new CNum(rootNew);
                    }
                }
                return; // end - catch EXACT TANGENTS
            }
            
            if (Math.Abs(a)<MConsts.EPS_DEC) 
            {
                Equation.RootsAll(b,c,d,out numRoot, out numRootReal, out root);
            }
            else  
            {
                numRoot = 3;
                root=new CNum[3];

                double bb, cc, dd, pp, qq, D;
                bb = b/a; 
                cc = c/a; 
                dd = d/a;
                pp = -(bb*bb/3.0)+cc; 
                qq = (2.0*bb*bb*bb/27.0)-(bb*cc/3.0)+dd;

                // solve quadratic equation (1,qq,-pp^3/27)
                D = qq*qq+(4.0*pp*pp*pp/27.0);
                D = (Math.Abs(D)<MConsts.EPS_DEC*MConsts.EPS_DEC) ? 0.0 : D; //tolerancing for double roots

                if (D>0) //D>0, => 1 real & 2 complex roots
                {
                    numRootReal = 1;
                    double zp = 0.5*(-qq+Math.Sqrt(D));   // two real roots of the quadratic equation
                    double zm = 0.5*(-qq-Math.Sqrt(D));
                    
                    double zpRoot3 = Math.Sign(zp)*Math.Pow(Math.Abs(zp),(double)(1.0/3.0));
                    double zmRoot3 = Math.Sign(zm)*Math.Pow(Math.Abs(zm),(double)(1.0/3.0));
                    double sumRoots = zpRoot3+zmRoot3;
                    double difRoots = zpRoot3-zmRoot3;
                    root[0]=new CNum(-bb/3.0+sumRoots,0);
                    root[1]=new CNum(-bb/3.0-0.5*sumRoots, -0.5*Math.Sqrt(3.0)*difRoots);
                    root[2]=new CNum(-bb/3.0-0.5*sumRoots,  0.5*Math.Sqrt(3.0)*difRoots);
                }
                else    //D<=0, => 3 real roots
                {
                    numRootReal = 3;
                    ArrayList arr=new ArrayList();  // roots of the depressed equation, root = y-bb/3
                    int i;
                    if (D<0)
                    {
                        CNum z=new CNum(-0.5*qq, 0.5*Math.Sqrt(-D)); // root of the quartatic equation
                        if (z.IsZero)
                        {
                            throw new ExceptionGMath("Equation","RootsAll","Degree 3");
                        }
                        double zRadius=z.Radius;
                        double zAngle=z.Angle;
                        double zRadiusRoot3 = Math.Pow(zRadius,1.0/3.0);
                        for (i=0; i<=2; i++)
                        {
                            arr.Add(2.0*zRadiusRoot3*Math.Cos((zAngle+2.0*i*Math.PI)/3.0));
                        }
                        arr.Sort();            // TODO: check
                        for (i=0; i<=2; i++)
                        {
                            root[i]=new CNum(-bb/3.0+(double)arr[i],0);
                        }
                    }
                    if (D==0) // case of the multiply roots
                    {
                        double z = -0.5*qq;   // double real root of the quadratic equation
                        double zRoot3 = Math.Sign(z)*Math.Pow(Math.Abs(z),1.0/3.0);
                        arr.Add(2.0*zRoot3);
                        arr.Add(-zRoot3);
                        arr.Add(-zRoot3);
                        if (qq>0)
                        {
                            root[0]=new CNum(-bb/3.0+(double)arr[0],0,1);
                            root[1]=new CNum(-bb/3.0+(double)arr[1],0,2);
                            root[2]=new CNum(root[1]);
                        }
                        else if (qq==0)
                        {
                            root[0]=new CNum(-bb/3.0,0,3);
                            root[1]=new CNum(root[0]);
                            root[2]=new CNum(root[0]);
                        }
                        else
                        {
                            root[0]=new CNum(-bb/3.0+(double)arr[1],0,2);
                            root[1]=new CNum(root[0]);
                            root[2]=new CNum(-bb/3.0+(double)arr[0],0,2);
                        }
                    }
                }
            }    
        }

        /*
         *        REAL ROOTS
         */
        private static void RootsAllToReal(int numRootReal, CNum[] croot, out double[] root)
        {
            root=null;
            if (numRootReal>0)
            {
                root=new double[numRootReal];
                for (int i=0; i<numRootReal; i++)
                {
                    root[i]=croot[i].Re;
                }
            }            
        }

        // deg0
        public static void RootsReal(double a, 
            out int numRootReal, out double[] root)
        {
            CNum[] croot;
            int numRoot;
            Equation.RootsAll(a,out numRoot,out numRootReal,out croot);    
            Equation.RootsAllToReal(numRootReal,croot,out root);
        }

        // deg1
        public static void RootsReal(double a, double b,
            out int numRootReal, out double[] root)
        {
            CNum[] croot;
            int numRoot;
            Equation.RootsAll(a,b,out numRoot,out numRootReal,out croot);    
            Equation.RootsAllToReal(numRootReal,croot,out root);
        }

        // deg2
        public static void RootsReal(double a, double b, double c,
            out int numRootReal, out double[] root)
        {
            CNum[] croot;
            int numRoot;
            Equation.RootsAll(a,b,c,out numRoot,out numRootReal,out croot);    
            Equation.RootsAllToReal(numRootReal,croot,out root);
        }

        // deg3
        public static void RootsReal(double a, double b, double c, double d,
            out int numRootReal, out double[] root)
        {
            CNum[] croot;
            int numRoot;
            Equation.RootsAll(a,b,c,d,out numRoot,out numRootReal,out croot);    
            Equation.RootsAllToReal(numRootReal,croot,out root);
        }

        // deg 4
        public static void RootsReal(double a, double b, double c, double d, double e,
            out int numRootReal, out double[] root)
        {
            /*
             *  Catch EXACT TANGENTS when they are not obvious
             *    from the control points specification
             */ 
            
            double rootToAdd=-1.0; // asign an invalid parametric value
            int numRootRealRed=-1; // roots after reduction
            double[] rootRed=null;
            if (Equation.Evaluate(0.0,a,b,c,d,e)==0.0)
            {
                rootToAdd=0.0;
                Equation.RootsReal(a,b,c,d,
                    out numRootRealRed, out rootRed);
            }
            if (Equation.Evaluate(1.0,a,b,c,d,e)==0.0)
            {
                rootToAdd=1.0;
                Equation.RootsReal(a,a+b,a+b+c,a+b+c+d,
                    out numRootRealRed, out rootRed);
            }
            if (rootToAdd!=-1.0)
            {
                root=new double[numRootRealRed+1];
                numRootReal=numRootRealRed+1;
                int indToInsert=0;
                for (indToInsert=0; indToInsert<numRootRealRed; indToInsert++)
                {
                    if (rootToAdd<=rootRed[indToInsert])
                        break;
                }
                for (int iRoot=0; iRoot<indToInsert; iRoot++)
                {
                    root[iRoot]=rootRed[iRoot];
                }
                root[indToInsert]=rootToAdd;
                for (int iRoot=indToInsert; iRoot<numRootRealRed; iRoot++)
                {
                    root[iRoot+1]=rootRed[iRoot];
                }
                return; // end - catch EXACT TANGENT
            }
            

            if (Math.Abs(a)<MConsts.EPS_DEC) 
            {
                Equation.RootsReal(b,c,d,e,out numRootReal,out root);
            }
            else      
            {
                numRootReal=0;
                root=null;
                double bb, cc, dd, ee, pp, qq, rr;
                bb = b/a; cc = c/a; dd = d/a; ee = e/a;
                // substitution: x = z - bb/4
                
                pp=Equation.Evaluate(bb/4.0,-6.0, 0, cc);
                qq=Equation.Evaluate(bb/4.0, 8.0, 0, -2.0*cc, dd);
                rr=Equation.Evaluate(bb/4.0,-3.0, 0, cc, -dd, ee);
        
                // solve C(ubic) R(esolvent) equation (1, 2pp, pp^2-4rr, -q^2)
                int numRealRootCR, numRootCR; 
                CNum[] rootCR;
            
                // the cubic equation is non-degenerated,=> numRootCR = 3
                Equation.RootsAll(1.0, 2.0*pp, pp*pp-4.0*rr, -qq*qq, 
                    out numRootCR, out numRealRootCR, out rootCR);
                if (numRootCR==Equation.NumRootInfinite)
                {
                    throw new ExceptionGMath("Equation","RootsReal","Degree 4");
                }

                if (numRealRootCR == 3) // 3 real (not necessarily different) roots of CR
                {
                    if (rootCR[0].Re>=0)   // 3 real roots of CR; all roots are positive
                    {
                        numRootReal = 4;
                        double sqrt0 = Math.Sqrt(rootCR[0].Re); 
                        double sqrt1 = Math.Sqrt(rootCR[1].Re);
                        double sqrt2 = (Math.Sign(qq)!=0) ? 
                            Math.Sign(qq)*Math.Sqrt(rootCR[2].Re) : 
                            Math.Sqrt(rootCR[2].Re);
                
                        ArrayList arr=new ArrayList();
                        arr.Add(-bb/4.0+0.5*( sqrt0+sqrt1-sqrt2));
                        arr.Add(-bb/4.0+0.5*(-sqrt0+sqrt1+sqrt2));
                        arr.Add(-bb/4.0+0.5*( sqrt0-sqrt1+sqrt2));
                        arr.Add(-bb/4.0+0.5*(-sqrt0-sqrt1-sqrt2));
                        arr.Sort();
                    
                        root=new double[4];
                        for (int i=0;i<4;i++)
                        {
                            root[i]=(double)arr[i];
                        }
                    }

                    if ((rootCR[1].Re<0)&&(rootCR[2].Re>=0)) // 3 real roots of CR; 1 root is positive
                    {
                        if (Math.Abs(rootCR[0].Re-rootCR[1].Re)<MConsts.EPS_DEC)
                        {
                            numRootReal = 2; // double-root
                            root=new double[2];
                            root[0]=root[1]=-bb/4.0-0.5*Math.Sqrt(rootCR[2].Re)*Math.Sign(qq);
                        }
                    }
                }            
                if ((numRealRootCR==1) && (rootCR[0].Re>=0)) // 1 real root of CR; the root is positive
                {
                    numRootReal = 2;
                    double sqrt0 = Math.Sign(qq)*Math.Sqrt(rootCR[0].Re);
                    double sqrt1 = rootCR[1].PrimeRoot(2).Re;
                    root=new double[2];
                    root[0] = -bb/4.0-0.5*sqrt0-sqrt1;
                    root[1] = -bb/4.0-0.5*sqrt0+sqrt1;
                }
            }
        }
    }
}
