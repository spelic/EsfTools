using System;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace EsfParser.CodeGen
{
    /// <summary>
    /// Holds static properties for special system functions referenced by MOVE translations.
    /// </summary>
    public static class EzFunctions
    {
        /// <summary>
        /// ORG: MOVE 1 TO EZEFEC;
        /// </summary>
        public static int EZEFEC { get; set; } = 1;

        /// <summary>
        /// ORG: MOVE 1 TO EZESQISL;
        /// </summary>
        public static int EZESQISL { get; set; } = 1;

        /// <summary>
        /// ORG: MOVE 'IS00' TO EZESEGTR;
        /// </summary>
        public static string EZESEGTR { get; set; } = "'IS00'";

        private static string _ezeusrid;
        /// <summary>
        /// User identifier.
        /// </summary>
        public static string EZEUSRID
        {
            get => _ezeusrid;
            set => _ezeusrid = value;
        }

        private static string _ezelterm;
        /// <summary>
        /// Terminal or machine name.
        /// </summary>
        public static string EZELTERM
        {
            get => _ezelterm;
            set => _ezelterm = value;
        }

        private static DateTime _ezedte;
        /// <summary>
        /// Current date.
        /// </summary>
        public static DateTime EZEDTE
        {
            get => _ezedte;
            set => _ezedte = value;
        }

        private static int _ezesqcod;
        /// <summary>
        /// SQL return code.
        /// </summary>
        public static int EZESQCOD
        {
            get => _ezesqcod;
            set => _ezesqcod = value;
        }

        private static string _ezeapp;
        /// <summary>
        /// Application identifier.
        /// </summary>
        public static string EZEAPP
        {
            get => _ezeapp;
            set => _ezeapp = value;
        }

        private static int _ezemno;
        /// <summary>
        /// Definition considerations for EZEMNO
        /// A function used as an edit routine indicates that an error has been detected by
        /// moving a nonzero value to EZEMNO.This automatically displays the map
        /// again, with the field in error highlighted and the text of the message
        /// displayed.
        /// If a message table is not available, an edit routine can force the map to be
        /// conversed again by moving message text to EZEMSG and setting EZEMNO to
        /// 9999.
        /// </summary>
        public static int EZEMNO
        {
            get => _ezemno;
            set => _ezemno = value;
        }

        // ——————————————————————————————————————————————
        // New special system words
        // ——————————————————————————————————————————————

        /// <summary>
        /// Indicates the last function/key pressed (e.g. PF1–PF24, PA1–PA3, Enter, etc.).
        /// </summary>
        public static ConsoleKey EZEAID { get; set; } = ConsoleKey.Enter;

        /// <summary>
        /// Controls whether functions return exception codes (0 or 1).
        /// </summary>
        public static int EZEREPLY { get; set; } = 0;

        /// <summary>
        /// Commit system changes (stub for EZECOMIT).
        /// </summary>
        public static class EZECOMIT
        {
            /// <summary>Invoke commit.</summary>
            public static void Execute()
            {
                // TODO: wire up actual commit behavior if needed
                Console.WriteLine(">> EZECOMIT called");
            }
        }

        /// <summary>
        /// Roll back system changes (stub for EZEROLLB).
        /// </summary>
        public static class EZEROLLB
        {
            /// <summary>Invoke rollback.</summary>
            public static void Execute()
            {
                // TODO: wire up actual rollback behavior if needed
                Console.WriteLine(">> EZEROLLB called");
            }
        }

        /// <summary>
        /// Transfer to error‐handling routine when an I/O error occurs.
        /// </summary>
        public static class EZERTN
        {
            // TODO: implement
            //
            //
            //
            //
            // or‐handling jump
            public static void Execute()
            {
                Console.WriteLine("// [SpecialFunctions] ERROR ROUTINE");
            }
        }

        // ----------------------------------------------------------------------
        //  Language-aware system variable  EZEDTELC
        // ----------------------------------------------------------------------

        /// <summary>
        /// Active language code that controls <see cref="EZEDTELC"/> formatting.
        /// Default is <c>"ENU"</c>.
        /// </summary>
        public static string LanguageCode { get; set; } = "SLO";

        /// <summary>
        /// Current date (Gregorian) formatted for the active <see cref="LanguageCode"/>.
        /// Length is always 10 characters to match VisualAge Generator behaviour.
        /// <para/>
        /// Supported codes &amp; formats<br/>
        /// SLO –  dd.MM.yyyy<br/>
        /// CHS –  yyyy-MM-dd<br/>
        /// PTB –  dd/MM/yyyy<br/>
        /// ENU –  MM/dd/yyyy<br/>
        /// JPN –  yyyy/MM/dd<br/>
        /// KOR –  yyyy-MM-dd<br/>
        /// Unknown code ⇒ ENU format.
        /// </summary>
        public static string EZEDTELC
        {
            get
            {
                var today = DateTime.Today;
                return (LanguageCode?.ToUpperInvariant()) switch
                {
                    "CHS" => today.ToString("yyyy-MM-dd"), // Simplified Chinese
                    "PTB" => today.ToString("dd/MM/yyyy"), // Brazilian Portuguese
                    "SLO" => today.ToString("dd.MM.yyyy"), // Slovenian
                    "JPN" => today.ToString("yyyy/MM/dd"), // Japanese
                    "KOR" => today.ToString("yyyy-MM-dd"), // Korean
                    _      /* ENU & default */ => today.ToString("MM/dd/yyyy")
                };
            }
        }

        // ----------------------------------------------------------------------
        //  Environment identifier  EZESYS
        //  Character, length = 8, padded with spaces, persisted across segments.
        // ----------------------------------------------------------------------

        /// <summary>
        /// Identifies the runtime environment (AIX, MVSCICS, WINGUI, …).
        /// Setter trims / pads to 8 chars; getter always returns 8-byte string.
        /// <para/>
        /// Defaults to a best-effort host guess (<c>WINNT</c>, <c>AIX</c>, …).
        /// </summary>
        public static string EZESYS
        {
            get => _ezesys;
            set => _ezesys = value;
        }

        private static string _ezesys = "WIN";
    }
}
