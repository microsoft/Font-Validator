using System;

public class StatusGV
{
    /*
     *        ENUMS
     */
    public enum TypeStatusExec
    {
        Aborted,
        UnableToExec,
        NeverExec,
        Completed
    }

    public enum TypeStatusRes
    {
        Undef,
        Warnings,
        Errors,
        NoErrors
    }


    /*
     *        MEMBERS:
     *            isValid -    true if the glyph was not 
     *                        modified since last validation
     *            statusExec    -    status of the latest execution
     *            statusRes    -    shows whether any warnings or errors
     *                            were detected during the latest COMPLETED
     *                            execution;
     *                            UNDEF if the latest execution was NOT
     *                            completed
     */
    bool            isValid;        
    TypeStatusExec    statusExec;
    TypeStatusRes    statusRes;    

    /*
     *        PROPERTIES
     */
    public bool IsValid
    {
        get { return this.isValid; }
        set { this.isValid=value; }
    }

    public TypeStatusExec StatusExec
    {
        get { return this.statusExec; }
        set { this.statusExec=value; }
    }

    public TypeStatusRes StatusRes
    {
        get { return this.statusRes; }
        set 
        {
            this.statusRes=value; // TODO: check...
        }
    }

    /*
     *        CONSTRUCTORS
     */
    public StatusGV()
    {
        this.isValid=false;
        this.statusExec=TypeStatusExec.NeverExec;
        this.statusRes=TypeStatusRes.Undef;
    }

    /*
     *        METHODS
     */


}