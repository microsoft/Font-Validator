using System;

namespace NS_Glyph
{
    public class DefsGM
    {
        public enum TypeGM
        {
            Invalid=-1,

            // validators
            ValidateTypeGlyphSource,
            ValidateSimpSource,
            ValidateCompSource,
            ValidateCompBind,

            // correctors
            CorrectSimpMisorCont,

            // pure modifiers
            ModifyMoveKnot,
            ModifyDeleteKnot
        }
         
        public static bool IsSourceValidator(TypeGM typeGM)
        {
            return ((typeGM==TypeGM.ValidateSimpSource)||
                (typeGM==TypeGM.ValidateCompSource)||
                (typeGM==TypeGM.ValidateTypeGlyphSource));
        }
        public static bool IsValidator(TypeGM typeGM)
        {
            return (Enum.GetName(typeof(TypeGM),typeGM).StartsWith("Validate"));
        }
        public static bool IsCorrector(TypeGM typeGM)
        {
            return (Enum.GetName(typeof(TypeGM),typeGM).StartsWith("Correct"));
        }
        public static TypeGM From(DefsGV.TypeGV typeGV)
        {
            string strTypeGV=Enum.GetName(typeof(DefsGV.TypeGV),typeGV);
            object obj;
            try
            {
                obj=Enum.Parse(typeof(DefsGM.TypeGM),strTypeGV);
            }
            catch
            {
                return DefsGM.TypeGM.Invalid;
            }
            return (DefsGM.TypeGM)obj;
        }
    }
}