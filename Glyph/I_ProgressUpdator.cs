using System;

namespace NS_Glyph
{
    public interface I_ProgressUpdater
    {
        void UpdateProgress(int indGlyph);
        void UpdateProgress(double percents);
        void UpdateStatus(string status);
        void TaskCompleted();
        bool ToStop{ get; }
        void Clear();
    }
}