using System;
using System.Diagnostics;

using OTFontFile;

namespace NS_GMath
{
    public class MatrixD
    {
        /*
         *        MEMBERS
         */
        double[,] tr;



        /*
         *        INDEXER
         */
        public double this [int row, int col]
        {
            get 
            {
                return this.tr[row,col];
            }
            set
            {
                this.tr[row,col]=value;
            }
        }

        /*
         *        CONSTRUCTOR
         */
        public MatrixD()
        {
            this.tr=new double[3,2];
            for (int i=0; i<3; i++)
                for (int j=0; j<2; j++)
                    this.tr[i,j]=0.0;
        }
        public MatrixD(MatrixD matrix)
        {
            this.tr=new double[3,2];
            for (int i=0; i<3; i++)
                for (int j=0; j<2; j++)
                    this.tr[i,j]=matrix[i,j];
        }


        public MatrixD(OTF2Dot14[,] tr,    VecD shift)
            : this()
        {
            if (tr!=null)
            {
                if ((tr.GetLength(0)!=2)||(tr.GetLength(1)!=2))
                {
                    throw new ExceptionGMath("MatrixD","MatrixD",null);
                }
                for (int i=0;i<2;i++)
                    for (int j=0;j<2;j++)
                    {
                        this.tr[i,j]=(double)tr[i,j];
                    }
            }
            else
            {
                this.tr[0,0]=this.tr[1,1]=1.0;
                this.tr[0,1]=this.tr[1,0]=0.0;
            }
            if (shift!=null)
            {
                this.tr[2,0]=shift.X;
                this.tr[2,1]=shift.Y;
            }
        }

        /*
         *        METHODS
         */
        public void From(MatrixD matrix)
        {
            for (int i=0; i<3; i++)
                for (int j=0; j<2; j++)
                    this.tr[i,j]=matrix[i,j];
        }
    }
}