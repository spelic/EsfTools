namespace EsfParser.Runtime
{
    /// <summary>
    /// Represents an attention identifier (AID) returned from a
    /// CONVERSE operation.  Enter indicates normal acceptance of input,
    /// F1–F10 correspond to function keys and Esc represents an
    /// immediate cancel.  This enumeration mirrors common 3270/5250
    /// keyboards but can be extended if additional keys are needed.
    /// </summary>
    public enum AidKey
    {
        Enter = 0,
        F1,
        F2,
        F3,
        F4,
        F5,
        F6,
        F7,
        F8,
        F9,
        F10,
        Esc
    }
}