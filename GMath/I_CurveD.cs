using System;

using NS_IDraw;

namespace NS_GMath
{
    public interface I_Transformable
    {
        void Transform(MatrixD m)    ;
    }
    public interface I_BBox
    {
        BoxD BBox { get; }
    }

    public interface I_CurveD : I_Transformable, I_BBox, I_Drawable
    {
        I_CurveD Copy();

        bool IsSimple { get; }    // non-composite, bounded
        bool IsBounded { get; }    
        bool IsDegen { get; }

        void Reverse();
        I_CurveD Reversed { get; }

        bool IsEvaluableWide(Param par);
        bool IsEvaluableStrict(Param par);

        Param.TypeParam ParamClassify(Param par);

        
        // parametric evaluation
        VecD Evaluate(Param par);
        VecD DirTang(Param par);     
        VecD DirNorm(Param par);

        // non-strict
        double CurveLength(Param parS, Param parE);
        double CurveLength();
        double Curvature(Param par);

        VecD Start { get; }
        VecD End { get; }
        double ParamStart { get; }
        double ParamEnd { get; }
        double ParamReverse { get; }
        
    }
    public interface I_BCurveD : I_CurveD
    {
        bool IsSelfInters(out InfoInters inters);
        int BComplexity { get; }    // number of CP
        VecD Middle { get; }
        void PowerCoeff(out VecD[] pcf);
        I_BCurveD Reduced { get; }
        void Subdivide(Param par, out I_BCurveD curveS, out I_BCurveD curveE);
        I_BCurveD SubCurve(Param parA, Param parB);
    }

    public interface I_LCurveD : I_CurveD // linear curve
    {
        int LComplexity { get; }    // number of unbounded ends
        void PowerCoeff(out VecD[] pcf);
        new VecD DirTang { get; }
        new VecD DirNorm { get; }
    }
    public interface I_CCurveD : I_CurveD
    {
    }


}