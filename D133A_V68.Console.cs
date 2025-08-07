using System;
using System.Collections.Generic;

namespace D133A_V68_ConsoleApp
{
#region GLOBAL ITEMS
    public static class GlobalItems
    {
        /// <summary>
        /// Števec pozicije kurzorja
        /// </summary>
        public static int CTRCURSO;

        /// <summary>
        /// Števec izpisanih kopij
        /// </summary>
        public static int CTRKOPIJ;

        /// <summary>
        /// Števec linij na stran
        /// </summary>
        public static int CTRLINIJ;

        /// <summary>
        /// Števec za prenos na ekran
        /// </summary>
        public static int CTRSTART;

        /// <summary>
        /// Števec strani
        /// </summary>
        public static int CTRSTRA1;

        /// <summary>
        /// Števec strani
        /// </summary>
        public static int CTRSTRAN;

        /// <summary>
        /// indikator(BRIŠI-DA,BRIŠI-NE)
        /// </summary>
        public static string FLBRISI;

        /// <summary>
        /// indikator(naprej,stop)
        /// </summary>
        public static string FLNAPREJ;

        /// <summary>
        /// indikator OK,NE
        /// </summary>
        public static string FLOK;

        /// <summary>
        /// indikator(DELAM,NEDELAM)
        /// </summary>
        public static string FLOK1;

        /// <summary>
        /// indikator(ZAPIS-DA,ZAPIS-NE)
        /// </summary>
        public static string FLZAPIS;

        /// <summary>
        /// indikator(ZAPIS-DA,ZAPIS-NE)
        /// </summary>
        public static string FLZAPIS1;

        public static string MSG64;

        public static string MSG78;

        public static string MSGCOD;

        public static int MSGN3;

        public static int TE1N10D0;

        public static decimal TE1N10D2;

        public static decimal TE1N10D4;

        public static decimal TE2N10D3;

        public static decimal TE2N10D4;

        public static int TEMN10D0;

        public static decimal TEMN10D2;

        public static decimal TEMN10D4;

        /// <summary>
        /// timest.za primerjavo pred vp.
        /// </summary>
        public static string TIMEST1;

    }
#endregion

#region GLOBAL RECORDS
    public static class GlobalRecords
    {
        public static class CALL_D292
        {
            public static string FIRMA;
            /// <summary>
            /// 'ident izdelka'
            /// </summary>
            public static int IDENT;
            /// <summary>
            /// 'tehnološki postopek'
            /// </summary>
            public static int TP;
            /// <summary>
            /// 'sklop tehnologije'
            /// </summary>
            public static int SKLOP;
            /// <summary>
            /// 'leto DN'
            /// </summary>
            public static int LETODN;
            /// <summary>
            /// 'delovni nalog'
            /// </summary>
            public static int DELNAL;
            public static string VARDELN;
            /// <summary>
            /// 'številka stroja'
            /// </summary>
            public static string STEVSTRO;
            /// <summary>
            /// 'transportna enota'
            /// </summary>
            public static int TRAN_ENOTA;
            /// <summary>
            /// 'oseba, ki dela'
            /// </summary>
            public static string OSEBA;
            /// <summary>
            /// 'če je zapisan atribut'
            /// </summary>
            public static string ZAPIS;
        }

        public static class CALL_D308
        {
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int TRAN_ENOTA;
            public static string SARZA;
            public static decimal KOLICINA;
            public static string ZAPRIIM;
        }

        public static class CALL_MEHANSKE
        {
            public static int ID_SKUPPZ_KEM_NML;
            /// <summary>
            /// 'SIGMABSP'
            /// </summary>
            public static int SIGMABSP;
            /// <summary>
            /// 'SIGMABZG'
            /// </summary>
            public static int SIGMABZG;
            /// <summary>
            /// 'SIG02SP'
            /// </summary>
            public static int SIG02SP;
            /// <summary>
            /// 'SIG02ZG'
            /// </summary>
            public static int SIG02ZG;
            /// <summary>
            /// 'HBSPOD'
            /// </summary>
            public static int HBSPOD;
            /// <summary>
            /// 'HBZGOR'
            /// </summary>
            public static int HBZGOR;
            /// <summary>
            /// 'DELTATIP'
            /// </summary>
            public static string DELTATIP;
            /// <summary>
            /// 'DELVRZG'
            /// </summary>
            public static decimal DELVRZG;
            /// <summary>
            /// 'DELVRSP'
            /// </summary>
            public static decimal DELVRSP;
            public static decimal UMAX;
            public static decimal IEMIN;
            /// <summary>
            /// 'upogib'
            /// </summary>
            public static string UPA;
            /// <summary>
            /// 'upogib'
            /// </summary>
            public static string UPB;
            /// <summary>
            /// 'upogib'
            /// </summary>
            public static string UPC;
        }

        public static class CALL_NOVI_ROK
        {
            public static int LETONA;
            public static int STNAROC;
            public static int POZNARO;
            public static int NOVI_ROK;
            public static string OSEBA;
            public static string FIRMA;
        }

        public static class CALL_ZAHTEVE
        {
            public static int LETONA;
            public static int STNAROC;
            public static int POZNARO;
            public static string KUPCEV_IDENT;
            public static string KOMITENT;
            /// <summary>
            /// 'šifra sklopa'
            /// </summary>
            public static int ID_SKUPPZ_KEM_NML;
            public static string PRENOS;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string DIMENZOR;
            /// <summary>
            /// 'za katero firmo je naročilo'
            /// </summary>
            public static string FIRMA_VH;
            /// <summary>
            /// 'firma_prijava'
            /// </summary>
            public static string FIRMA_PRIJAVA;
        }

        public static class D133R01
        {
            /// <summary>
            /// 'DELNAL'
            /// </summary>
            public static int DELNAL;
            /// <summary>
            /// 'VARDELN'
            /// </summary>
            public static string VARDELN;
            /// <summary>
            /// 'DELNALZ'
            /// </summary>
            public static int DELNALZ;
            /// <summary>
            /// 'VARDELNZ'
            /// </summary>
            public static string VARDELNZ;
            /// <summary>
            /// 'KOLDELNZ'
            /// </summary>
            public static decimal KOLDELNZ;
            /// <summary>
            /// 'DELNALK'
            /// </summary>
            public static int DELNALK;
            /// <summary>
            /// 'VARDELNK'
            /// </summary>
            public static string VARDELNK;
            /// <summary>
            /// 'KOLDELNK'
            /// </summary>
            public static decimal KOLDELNK;
            /// <summary>
            /// 'STNAROC1'
            /// </summary>
            public static int STNAROC1;
            /// <summary>
            /// 'POZNARO1'
            /// </summary>
            public static int POZNARO1;
            /// <summary>
            /// 'STEVPOG1'
            /// </summary>
            public static int STEVPOG1;
            /// <summary>
            /// 'NARKOL1'
            /// </summary>
            public static decimal NARKOL1;
            /// <summary>
            /// 'STNAROC2'
            /// </summary>
            public static int STNAROC2;
            /// <summary>
            /// 'POZNARO2'
            /// </summary>
            public static int POZNARO2;
            /// <summary>
            /// 'STEVPOG2'
            /// </summary>
            public static int STEVPOG2;
            /// <summary>
            /// 'NARKOL2'
            /// </summary>
            public static decimal NARKOL2;
            /// <summary>
            /// 'STNAROC3'
            /// </summary>
            public static int STNAROC3;
            /// <summary>
            /// 'POZNARO3'
            /// </summary>
            public static int POZNARO3;
            /// <summary>
            /// 'STEVPOG3'
            /// </summary>
            public static int STEVPOG3;
            /// <summary>
            /// 'NARKOL3'
            /// </summary>
            public static decimal NARKOL3;
            /// <summary>
            /// 'STPREJEM'
            /// </summary>
            public static string STPREJEM;
            /// <summary>
            /// 'KRNAZ'
            /// </summary>
            public static string KRNAZ;
            /// <summary>
            /// 'DNKOLIC'
            /// </summary>
            public static decimal DNKOLIC;
            /// <summary>
            /// 'DNKOMAD'
            /// </summary>
            public static int DNKOMAD;
            /// <summary>
            /// 'DNPF'
            /// </summary>
            public static decimal DNPF;
            /// <summary>
            /// 'IDENT'
            /// </summary>
            public static int IDENT;
            /// <summary>
            /// 'KLASIFI1'
            /// </summary>
            public static string KLASIFI1;
            /// <summary>
            /// 'KLASIFI2'
            /// </summary>
            public static string KLASIFI2;
            /// <summary>
            /// 'KLASIFI3'
            /// </summary>
            public static string KLASIFI3;
            /// <summary>
            /// 'KLASIFI4'
            /// </summary>
            public static string KLASIFI4;
            /// <summary>
            /// 'KLASIFI5'
            /// </summary>
            public static string KLASIFI5;
            /// <summary>
            /// 'KLASIFI6'
            /// </summary>
            public static string KLASIFI6;
            /// <summary>
            /// 'ZLITINA'
            /// </summary>
            public static string ZLITINA;
            /// <summary>
            /// 'STMASIF'
            /// </summary>
            public static string STMASIF;
            /// <summary>
            /// 'STPOSIF'
            /// </summary>
            public static string STPOSIF;
            /// <summary>
            /// 'NAZIZDEL'
            /// </summary>
            public static string NAZIZDEL;
            /// <summary>
            /// 'DIMENZ40'
            /// </summary>
            public static string DIMENZ40;
            /// <summary>
            /// 'TOLERPL'
            /// </summary>
            public static string TOLERPL;
            /// <summary>
            /// 'TOLERMI'
            /// </summary>
            public static string TOLERMI;
            /// <summary>
            /// 'STANDID'
            /// </summary>
            public static string STANDID;
            /// <summary>
            /// 'SIGMABSP'
            /// </summary>
            public static int SIGMABSP;
            /// <summary>
            /// 'SIGMABZG'
            /// </summary>
            public static int SIGMABZG;
            /// <summary>
            /// 'SIG02SP'
            /// </summary>
            public static int SIG02SP;
            /// <summary>
            /// 'SIG02ZG'
            /// </summary>
            public static int SIG02ZG;
            /// <summary>
            /// 'HBSPOD'
            /// </summary>
            public static int HBSPOD;
            /// <summary>
            /// 'HBZGOR'
            /// </summary>
            public static int HBZGOR;
            /// <summary>
            /// 'DELTATIP'
            /// </summary>
            public static string DELTATIP;
            /// <summary>
            /// 'DELVRZG'
            /// </summary>
            public static decimal DELVRZG;
            /// <summary>
            /// 'DELVRSP'
            /// </summary>
            public static decimal DELVRSP;
            /// <summary>
            /// 'POSZAHT1'
            /// </summary>
            public static string POSZAHT1;
            /// <summary>
            /// 'POSZAHT2'
            /// </summary>
            public static string POSZAHT2;
            /// <summary>
            /// 'IDENTSUR'
            /// </summary>
            public static int IDENTSUR;
            /// <summary>
            /// 'NAZVHOD'
            /// </summary>
            public static string NAZVHOD;
            /// <summary>
            /// 'DIMENZVH'
            /// </summary>
            public static string DIMENZVH;
            /// <summary>
            /// 'STMAVHOD'
            /// </summary>
            public static string STMAVHOD;
            /// <summary>
            /// 'STPOVHOD'
            /// </summary>
            public static string STPOVHOD;
            /// <summary>
            /// 'OPISVIS'
            /// </summary>
            public static string OPISVIS;
            /// <summary>
            /// 'INDIKVI'
            /// </summary>
            public static string INDIKVI;
            /// <summary>
            /// 'POTSURKG'
            /// </summary>
            public static int POTSURKG;
            /// <summary>
            /// 'POTSURKM'
            /// </summary>
            public static decimal POTSURKM;
            /// <summary>
            /// 'REZSURKG'
            /// </summary>
            public static int REZSURKG;
            /// <summary>
            /// 'REZSURKM'
            /// </summary>
            public static int REZSURKM;
            /// <summary>
            /// 'POTREBKG'
            /// </summary>
            public static int POTREBKG;
            /// <summary>
            /// 'POTREBKM'
            /// </summary>
            public static int POTREBKM;
            /// <summary>
            /// 'POTREBTE'
            /// </summary>
            public static int POTREBTE;
            /// <summary>
            /// 'LANSIRKG'
            /// </summary>
            public static int LANSIRKG;
            /// <summary>
            /// 'LANSIRKM'
            /// </summary>
            public static int LANSIRKM;
            /// <summary>
            /// 'RAZLIKG'
            /// </summary>
            public static int RAZLIKG;
            /// <summary>
            /// 'RAZLIKOM'
            /// </summary>
            public static int RAZLIKOM;
            /// <summary>
            /// 'IZRAVNKG'
            /// </summary>
            public static int IZRAVNKG;
            /// <summary>
            /// 'STANJEKG'
            /// </summary>
            public static int STANJEKG;
            public static decimal IZDKOL;
            /// <summary>
            /// 'ODPADKOL'
            /// </summary>
            public static decimal ODPADKOL;
            /// <summary>
            /// 'MOTNJKOL'
            /// </summary>
            public static decimal MOTNJKOL;
            /// <summary>
            /// 'VISEKG'
            /// </summary>
            public static decimal VISEKG;
            /// <summary>
            /// 'PROGENOT'
            /// </summary>
            public static string PROGENOT;
            /// <summary>
            /// 'ZACETNTE'
            /// </summary>
            public static int ZACETNTE;
            /// <summary>
            /// 'KONCNATE'
            /// </summary>
            public static int KONCNATE;
            /// <summary>
            /// 'REFMPP'
            /// </summary>
            public static string REFMPP;
            /// <summary>
            /// 'TERMINER'
            /// </summary>
            public static string TERMINER;
            /// <summary>
            /// 'STATUSDN'
            /// </summary>
            public static string STATUSDN;
            /// <summary>
            /// 'DATSTAT0'
            /// </summary>
            public static string DATSTAT0;
            /// <summary>
            /// 'DATSTAT1'
            /// </summary>
            public static string DATSTAT1;
            /// <summary>
            /// 'DATSTAT2'
            /// </summary>
            public static string DATSTAT2;
            /// <summary>
            /// 'DATSTAT3'
            /// </summary>
            public static string DATSTAT3;
            /// <summary>
            /// 'DATSTAT4'
            /// </summary>
            public static string DATSTAT4;
            /// <summary>
            /// 'DATSTAT5'
            /// </summary>
            public static string DATSTAT5;
            /// <summary>
            /// 'DATSTAT6'
            /// </summary>
            public static string DATSTAT6;
            /// <summary>
            /// 'DATURA'
            /// </summary>
            public static string DATURA;
            /// <summary>
            /// 'SPREMTEH'
            /// </summary>
            public static string SPREMTEH;
            public static int STEVKZ;
            public static int VERKZ;
            public static string KZORG;
            public static int LETODN;
            public static int STIZDAJ;
            public static string STANKEMS;
            public static string STANMEHL;
            public static string STANDTOL;
            public static decimal IZDKOL1;
            public static decimal IZDKOL2;
            public static decimal IZDKOL3;
            public static int ST_TRE;
            public static string ORGEN;
            public static string Q_IND;
            public static decimal UMAX;
            public static decimal IEMIN;
        }

        public static class D133R02
        {
            /// <summary>
            /// 'STNAROC'
            /// </summary>
            public static int STNAROC;
            /// <summary>
            /// 'POZNARO'
            /// </summary>
            public static int POZNARO;
            /// <summary>
            /// 'STPREJEM'
            /// </summary>
            public static string STPREJEM;
            public static int NAMEN;
            public static string POREKLO;
            public static string DIMENZOR;
            public static int LETONA;
            public static int KONCNATE;
            public static string FIRMA;
            public static decimal NAROKOL;
            public static int EM;
            public static decimal NARKOLPR;
            public static decimal TOL_KOL_PL;
            public static decimal TOL_KOL_MI;
            public static int EM_TOL_KOL;
        }

        public static class D133R03
        {
            /// <summary>
            /// 'KOMITENT'
            /// </summary>
            public static string KOMITENT;
            /// <summary>
            /// 'KRNAZ'
            /// </summary>
            public static string KRNAZ;
        }

        public static class D133R04
        {
            /// <summary>
            /// 'DELNAL'
            /// </summary>
            public static int DELNAL;
            /// <summary>
            /// 'VARDELN'
            /// </summary>
            public static string VARDELN;
            public static string STEVSTRO;
            /// <summary>
            /// 'STEVILOP'
            /// </summary>
            public static int STEVILOP;
            public static string NAZSTRKR;
            public static string DATSPRE;
            public static decimal IZDKOL;
            public static int IZDKOM;
            public static string DELVNOS;
            /// <summary>
            /// 'DELIZDA'
            /// </summary>
            public static string DELIZDA;
            /// <summary>
            /// 'PROGENOT'
            /// </summary>
            public static string PROGENOT;
            /// <summary>
            /// 'PFO'
            /// </summary>
            public static decimal PFO;
            /// <summary>
            /// 'PFDOOPER'
            /// </summary>
            public static decimal PFDOOPER;
            /// <summary>
            /// 'DATURA'
            /// </summary>
            public static string DATURA;
        }

        public static class D133R05
        {
            public static string DATUM;
            public static string SIFNAP;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static string NAZIZDEL;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string KLASIFI1;
            public static string KLASIFI2;
            public static string KLASIFI3;
            public static string KLASIFI4;
            public static string KLASIFI5;
            public static string KLASIFI6;
            public static string DIMENZ40;
            public static int KOLICINA;
            public static string OPISMOT;
            public static string KTUGOT;
            public static string KTNAST;
            public static string NAZIVUGO;
            public static string NAZIVNAS;
            public static string PEUGOT;
            public static string PENAST;
            public static int KOLVISEK;
            public static int KOLODPAD;
            public static int POGPRE;
            public static string OSEBA;
            public static string POTRDIL;
            public static string DATURA;
        }

        public static class D133R06
        {
            public static int DELNAL;
            public static string VARDELN;
            public static string STEVSTRO;
            public static int STEVILOP;
            public static string SARZA;
            public static int TRAN_ENOTA;
            public static string DATSPRE;
            public static decimal IZDKOL;
            public static int IZDKOM;
            public static string DELVNOS;
            public static string DELIZDA;
            public static string PROGENOT;
            public static string DATURA;
            public static string KOMENTAR;
            public static string VZOREC;
            public static string STATUSD;
            public static int ZAPSTEV;
            public static string SARZA_DOD;
        }

        public static class D133R07
        {
            /// <summary>
            /// 'DELNAL'
            /// </summary>
            public static int DELNAL;
            /// <summary>
            /// 'VARDELN'
            /// </summary>
            public static string VARDELN;
            /// <summary>
            /// 'DELNALZ'
            /// </summary>
            public static int DELNALZ;
            /// <summary>
            /// 'VARDELNZ'
            /// </summary>
            public static string VARDELNZ;
            /// <summary>
            /// 'KOLDELNZ'
            /// </summary>
            public static decimal KOLDELNZ;
            /// <summary>
            /// 'DELNALK'
            /// </summary>
            public static int DELNALK;
            /// <summary>
            /// 'VARDELNK'
            /// </summary>
            public static string VARDELNK;
            /// <summary>
            /// 'KOLDELNK'
            /// </summary>
            public static decimal KOLDELNK;
            /// <summary>
            /// 'STNAROC1'
            /// </summary>
            public static int STNAROC1;
            /// <summary>
            /// 'POZNARO1'
            /// </summary>
            public static int POZNARO1;
            /// <summary>
            /// 'STEVPOG1'
            /// </summary>
            public static int STEVPOG1;
            /// <summary>
            /// 'NARKOL1'
            /// </summary>
            public static decimal NARKOL1;
            /// <summary>
            /// 'STNAROC2'
            /// </summary>
            public static int STNAROC2;
            /// <summary>
            /// 'POZNARO2'
            /// </summary>
            public static int POZNARO2;
            /// <summary>
            /// 'STEVPOG2'
            /// </summary>
            public static int STEVPOG2;
            /// <summary>
            /// 'NARKOL2'
            /// </summary>
            public static decimal NARKOL2;
            /// <summary>
            /// 'STNAROC3'
            /// </summary>
            public static int STNAROC3;
            /// <summary>
            /// 'POZNARO3'
            /// </summary>
            public static int POZNARO3;
            /// <summary>
            /// 'STEVPOG3'
            /// </summary>
            public static int STEVPOG3;
            /// <summary>
            /// 'NARKOL3'
            /// </summary>
            public static decimal NARKOL3;
            /// <summary>
            /// 'STPREJEM'
            /// </summary>
            public static string STPREJEM;
            /// <summary>
            /// 'KRNAZ'
            /// </summary>
            public static string KRNAZ;
            /// <summary>
            /// 'DNKOLIC'
            /// </summary>
            public static decimal DNKOLIC;
            /// <summary>
            /// 'DNKOMAD'
            /// </summary>
            public static int DNKOMAD;
            /// <summary>
            /// 'DNPF'
            /// </summary>
            public static decimal DNPF;
            /// <summary>
            /// 'IDENT'
            /// </summary>
            public static int IDENT;
            /// <summary>
            /// 'KLASIFI1'
            /// </summary>
            public static string KLASIFI1;
            /// <summary>
            /// 'KLASIFI2'
            /// </summary>
            public static string KLASIFI2;
            /// <summary>
            /// 'KLASIFI3'
            /// </summary>
            public static string KLASIFI3;
            /// <summary>
            /// 'KLASIFI4'
            /// </summary>
            public static string KLASIFI4;
            /// <summary>
            /// 'KLASIFI5'
            /// </summary>
            public static string KLASIFI5;
            /// <summary>
            /// 'KLASIFI6'
            /// </summary>
            public static string KLASIFI6;
            /// <summary>
            /// 'ZLITINA'
            /// </summary>
            public static string ZLITINA;
            /// <summary>
            /// 'STMASIF'
            /// </summary>
            public static string STMASIF;
            /// <summary>
            /// 'STPOSIF'
            /// </summary>
            public static string STPOSIF;
            /// <summary>
            /// 'NAZIZDEL'
            /// </summary>
            public static string NAZIZDEL;
            /// <summary>
            /// 'DIMENZ40'
            /// </summary>
            public static string DIMENZ40;
            /// <summary>
            /// 'TOLERPL'
            /// </summary>
            public static string TOLERPL;
            /// <summary>
            /// 'TOLERMI'
            /// </summary>
            public static string TOLERMI;
            /// <summary>
            /// 'STANDID'
            /// </summary>
            public static string STANDID;
            /// <summary>
            /// 'SIGMABSP'
            /// </summary>
            public static int SIGMABSP;
            /// <summary>
            /// 'SIGMABZG'
            /// </summary>
            public static int SIGMABZG;
            /// <summary>
            /// 'SIG02SP'
            /// </summary>
            public static int SIG02SP;
            /// <summary>
            /// 'SIG02ZG'
            /// </summary>
            public static int SIG02ZG;
            /// <summary>
            /// 'HBSPOD'
            /// </summary>
            public static int HBSPOD;
            /// <summary>
            /// 'HBZGOR'
            /// </summary>
            public static int HBZGOR;
            /// <summary>
            /// 'DELTATIP'
            /// </summary>
            public static string DELTATIP;
            /// <summary>
            /// 'DELVRZG'
            /// </summary>
            public static decimal DELVRZG;
            /// <summary>
            /// 'DELVRSP'
            /// </summary>
            public static decimal DELVRSP;
            /// <summary>
            /// 'POSZAHT1'
            /// </summary>
            public static string POSZAHT1;
            /// <summary>
            /// 'POSZAHT2'
            /// </summary>
            public static string POSZAHT2;
            /// <summary>
            /// 'IDENTSUR'
            /// </summary>
            public static int IDENTSUR;
            /// <summary>
            /// 'NAZVHOD'
            /// </summary>
            public static string NAZVHOD;
            /// <summary>
            /// 'DIMENZVH'
            /// </summary>
            public static string DIMENZVH;
            /// <summary>
            /// 'STMAVHOD'
            /// </summary>
            public static string STMAVHOD;
            /// <summary>
            /// 'STPOVHOD'
            /// </summary>
            public static string STPOVHOD;
            /// <summary>
            /// 'OPISVIS'
            /// </summary>
            public static string OPISVIS;
            /// <summary>
            /// 'INDIKVI'
            /// </summary>
            public static string INDIKVI;
            /// <summary>
            /// 'POTSURKG'
            /// </summary>
            public static int POTSURKG;
            /// <summary>
            /// 'POTSURKM'
            /// </summary>
            public static decimal POTSURKM;
            /// <summary>
            /// 'REZSURKG'
            /// </summary>
            public static int REZSURKG;
            /// <summary>
            /// 'REZSURKM'
            /// </summary>
            public static int REZSURKM;
            /// <summary>
            /// 'POTREBKG'
            /// </summary>
            public static int POTREBKG;
            /// <summary>
            /// 'POTREBKM'
            /// </summary>
            public static int POTREBKM;
            /// <summary>
            /// 'POTREBTE'
            /// </summary>
            public static int POTREBTE;
            /// <summary>
            /// 'LANSIRKG'
            /// </summary>
            public static int LANSIRKG;
            /// <summary>
            /// 'LANSIRKM'
            /// </summary>
            public static int LANSIRKM;
            /// <summary>
            /// 'RAZLIKG'
            /// </summary>
            public static int RAZLIKG;
            /// <summary>
            /// 'RAZLIKOM'
            /// </summary>
            public static int RAZLIKOM;
            /// <summary>
            /// 'IZRAVNKG'
            /// </summary>
            public static int IZRAVNKG;
            /// <summary>
            /// 'STANJEKG'
            /// </summary>
            public static int STANJEKG;
            public static decimal IZDKOL;
            /// <summary>
            /// 'ODPADKOL'
            /// </summary>
            public static decimal ODPADKOL;
            /// <summary>
            /// 'MOTNJKOL'
            /// </summary>
            public static decimal MOTNJKOL;
            /// <summary>
            /// 'VISEKG'
            /// </summary>
            public static decimal VISEKG;
            /// <summary>
            /// 'PROGENOT'
            /// </summary>
            public static string PROGENOT;
            /// <summary>
            /// 'ZACETNTE'
            /// </summary>
            public static int ZACETNTE;
            /// <summary>
            /// 'KONCNATE'
            /// </summary>
            public static int KONCNATE;
            /// <summary>
            /// 'REFMPP'
            /// </summary>
            public static string REFMPP;
            /// <summary>
            /// 'TERMINER'
            /// </summary>
            public static string TERMINER;
            /// <summary>
            /// 'STATUSDN'
            /// </summary>
            public static string STATUSDN;
            /// <summary>
            /// 'DATSTAT0'
            /// </summary>
            public static string DATSTAT0;
            /// <summary>
            /// 'DATSTAT1'
            /// </summary>
            public static string DATSTAT1;
            /// <summary>
            /// 'DATSTAT2'
            /// </summary>
            public static string DATSTAT2;
            /// <summary>
            /// 'DATSTAT3'
            /// </summary>
            public static string DATSTAT3;
            /// <summary>
            /// 'DATSTAT4'
            /// </summary>
            public static string DATSTAT4;
            /// <summary>
            /// 'DATSTAT5'
            /// </summary>
            public static string DATSTAT5;
            /// <summary>
            /// 'DATSTAT6'
            /// </summary>
            public static string DATSTAT6;
            /// <summary>
            /// 'DATURA'
            /// </summary>
            public static string DATURA;
            /// <summary>
            /// 'SPREMTEH'
            /// </summary>
            public static string SPREMTEH;
            public static int STEVKZ;
            public static int VERKZ;
            public static string KZORG;
            public static int LETODN;
            public static int STIZDAJ;
            public static string STANKEMS;
            public static string STANMEHL;
            public static string STANDTOL;
            public static decimal IZDKOL1;
            public static decimal IZDKOL2;
            public static decimal IZDKOL3;
            public static int ST_TRE;
            public static string ORGEN;
            public static string Q_IND;
            public static decimal UMAX;
            public static decimal IEMIN;
        }

        public static class D133R10
        {
            public static string DATUM;
            public static string SIFNAP;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static string NAZIZDEL;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string KLASIFI1;
            public static string KLASIFI2;
            public static string KLASIFI3;
            public static string KLASIFI4;
            public static string KLASIFI5;
            public static string KLASIFI6;
            public static string DIMENZ40;
            public static int KOLICINA;
            public static string OPISMOT;
            public static string KTUGOT;
            public static string KTNAST;
            public static string NAZIVUGO;
            public static string NAZIVNAS;
            public static string PEUGOT;
            public static string PENAST;
            public static int KOLVISEK;
            public static int KOLODPAD;
            public static int POGPRE;
            public static string OSEBA;
            public static string POTRDIL;
            public static string DATURA;
        }

        public static class D133R101
        {
            public static string SIFNAP;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static string NAZIZDEL;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string KLASIFI1;
            public static string KLASIFI2;
            public static string KLASIFI3;
            public static string KLASIFI4;
            public static string KLASIFI5;
            public static string KLASIFI6;
            public static string DIMENZ40;
            public static int KOLICINA;
            public static string OPISMOT;
            public static string KTUGOT;
            public static string KTNAST;
            public static string NAZIVUGO;
            public static string NAZIVNAS;
            public static string PEUGOT;
            public static string PENAST;
            public static int KOLVISEK;
            public static int KOLODPAD;
            public static string PLANER;
            public static string POTRDIL;
            public static string DATUMPR;
            public static int POGPRE;
        }

        public static class D133R11
        {
            public static int SERIJA;
            public static int LETO;
            public static int OBDSARZA;
            public static int ZAPSTSERIJA;
            public static int ZAPSTPREIZ;
            public static string STSARZE;
            public static int DELNAL;
            public static string PROGENOT;
            public static string FIRMA;
            public static string OBLIKA;
            public static string STATUS;
            public static string VPISAL;
            public static string DATURA;
            public static string USTREZA;
            public static decimal NATTRD;
            public static decimal DOGNAPT;
            public static decimal RAZTEZ;
            public static decimal ZOZANJE;
            public static decimal ERICHSEN;
            public static decimal USESENJE;
            public static string UPOGPRE;
            public static decimal TRDOTA;
            public static int IDENT;
            public static string TEHNOLOG;
            public static string DAT_RESITVE;
            public static int LETODN;
        }

        public static class D133R12
        {
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static int STEVILOP;
            public static int STEVKZ;
            public static int VERKZ;
            public static string PE;
            public static int VERPE;
            public static int STPOSZAHT;
            public static int VERPOSZAHT;
            public static string DATURA;
            public static string STEVSTRO;
        }

        public static class D133R13
        {
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static int STEVILOP;
            public static int STEVKZ;
            public static int VERKZ;
            public static string PE;
            public static int VERPE;
            public static int STPOSZAHT;
            public static int VERPOSZAHT;
            public static int ZAPSTPREIZ;
            public static int TRAN_ENOTA;
            public static int SERIJA;
            public static string STEVSTRO;
            public static string SIFMOT;
            public static decimal VREDNOST;
            public static string USTREZA;
            public static string OSEBA;
            public static string DATURA;
            public static int LETO;
        }

        public static class D133R14
        {
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static int STEVILOP;
            public static int STEVKZ;
            public static int VERKZ;
            public static string PE;
            public static int VERPE;
            public static int STPOSZAHT;
            public static int VERPOSZAHT;
            public static int ZAPSTPREIZ;
            public static int TRAN_ENOTA;
            public static int SERIJA;
            public static string STEVSTRO;
            public static string SIFMOT;
            public static decimal VREDNOST;
            public static string USTREZA;
            public static string OSEBA;
            public static string DATURA;
            public static int LETO;
            public static int STEVKZ1;
            public static string PE1;
            public static int STPOSZAHT1;
            public static int VERPOSZAHT1;
            public static string FREKVENCA;
        }

        public static class D133R15
        {
            public static int ZAPST_PCP;
            public static string FIRMA;
            public static int IDENT;
            public static string ORGEN;
            public static int NAMEN;
            public static string POREKLO;
            public static string SARZA;
            public static int STEVKZ;
            public static string UZ;
            public static decimal KOLICINA;
            public static int KOMADI;
            public static string DATURA;
            public static string VNESEL;
        }

        public static class D133R16
        {
            public static int ID_SLEPMOTNJA_SKL;
            public static string DATUM;
            public static string SIFNAP;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static string PROGENOT;
            public static string SARZA;
            public static int TRAN_ENOTA;
            public static int KOL_SKL;
            public static string KOMENTAR;
            public static string DATUM_ODVZEMA;
            public static int ZAPSTEV;
            public static string SARZA_DOD;
            public static string DOBAVITELJ;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string DIMENZ40;
        }

        public static class D133R17
        {
            public static int ZAPST_SEZNAMIZD;
            public static int ZAPST_PCP;
            public static string FIRMA;
            public static int IDENT;
            public static string ORGEN;
            public static int NAMEN;
            public static string POREKLO;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static decimal KOLICINA;
            public static int KOMADI;
            public static string DATUM_ZAHTEVE;
            public static string DATUM_REALIZAC;
            public static string STATUS;
            public static string VILICARIST;
            public static string DATURA;
            public static string VNESEL;
            public static int STIZDAJ;
        }

        public static class D133R18
        {
            public static string SARZA;
            public static int IDSARZA;
        }

        public static class D133R19
        {
            public static string FIRMA;
            public static int IDENT;
            public static string ORGEN;
            public static int NAMEN;
            public static string POREKLO;
            public static int ZAPSTEV;
            public static int TRAN_ENOTA;
            public static string SARZA;
            public static decimal KOLICINA;
            public static int DELNAL;
            public static string DATURA;
            public static string VNESEL;
            public static string LOK_OSN;
            public static int STIZDAJ;
            public static string OPOMBA;
            public static string OPIS;
            public static string SARZA_DOD;
            public static string SIFMOT;
        }

        public static class D133R20
        {
            public static int ZAP;
            public static string L1;
            public static int L2;
            public static int L3;
            public static int L4;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int ZAPSTEV;
            public static int TRAN_ENOTA;
            public static string OZNAKA;
            public static int IDENT;
            public static string SARZA;
            public static decimal KOLICINA;
            public static string REZERV;
            public static string OPIS;
            public static string REZERVIRAL;
            public static string DATVNOSA;
            public static string VNESEL;
        }

        public static class D133R21
        {
            public static string DATUM;
            public static string SIFNAP;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static string NAZIZDEL;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string KLASIFI1;
            public static string KLASIFI2;
            public static string KLASIFI3;
            public static string KLASIFI4;
            public static string KLASIFI5;
            public static string KLASIFI6;
            public static string DIMENZ40;
            public static int KOLICINA;
            public static string OPISMOT;
            public static string KTUGOT;
            public static string KTNAST;
            public static string NAZIVUGO;
            public static string NAZIVNAS;
            public static string PEUGOT;
            public static string PENAST;
            public static int KOLVISEK;
            public static int KOLODPAD;
            public static int POGPRE;
            public static string OSEBA;
            public static string POTRDIL;
            public static string DATURA;
        }

        public static class D133R22
        {
            public static int ID_SLEPMOTNJA_RES;
            public static string DATUM;
            public static string SIFNAP;
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static string UKREP;
            public static string VPISAL_UKR;
            public static string DATUM_UKR;
            public static string RESITEV;
            public static string VPISAL_RES;
            public static string DATUM_RES;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
        }

        public static class D133R23
        {
            public static int LETONA;
            public static int STNAROC;
            public static int POZNARO;
            public static int ID_SKUPPZ_KEM_NML;
            public static string FIRMA;
        }

        public static class D133R24
        {
            public static int ID_NAROCILO_NOVI_ROK;
            public static int LETONA;
            public static int STNAROC;
            public static int POZNARO;
            public static string NOVI_ROK;
            public static int NOVA_TE;
            public static string KOMENTAR;
            public static string OSEBA_VNOSA;
            public static string DATUM_VNOSA;
            public static string KOMENTAR1;
            public static string OSEBA_VNOSA1;
            public static string DATUM_VNOSA1;
            public static string STATUS;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
        }

        public static class D133R26
        {
            public static int STEVKZ;
            public static int VERKZ;
            public static string PE;
            public static int VERPE;
            public static int STPOSZAHT;
            public static int VERPOSZAHT;
            public static int STEVKZ1;
            public static string AKCIJA;
            public static string JEZIK;
            public static string IZPIS;
            public static string FREKVENCA;
            public static int SIFZAH;
        }

        public static class D133R30
        {
            public static int ID_SKUP_DELNALOG_TRE_ATES;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int STEVILOP;
            public static string STEVSTRO;
            public static string SARZA;
            public static int TRAN_ENOTA;
            public static string DATUM_ZAPISA;
            public static int IND_STARANJA;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
        }

        public static class D133R37
        {
            public static int ID_SKUPTT_IZD_P_DIM_TIP;
            public static int IDENT;
            public static int ID_SKUSDIMENZIJA_TIP;
            public static string VREDNOST;
            public static decimal TOLERANCA_MIN;
            public static decimal TOLERANCA_MAX;
            public static decimal OVALNOST_MIN;
            public static decimal OVALNOST_MAX;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
        }

        public static class D133R38
        {
            public static int ID_MHN_VZOREC;
            public static int SERIJA;
            public static int LETO;
            public static int OBDSARZA;
            public static int ZAPSTSERIJA;
            public static int ZAPSTPREIZ;
            public static string STSARZE;
            public static int DELNAL;
            public static string PROGENOT;
            public static string FIRMA;
            public static string OBLIKA;
            public static string STATUS;
            public static string VPISAL;
            public static string DATURA;
            public static string USTREZA;
            public static decimal NATTRD;
            public static decimal DOGNAPT;
            public static decimal RAZTEZ;
            public static decimal ZOZANJE;
            public static decimal ERICHSEN;
            public static decimal USESENJE;
            public static string UPOGPRE;
            public static decimal TRDOTA;
            public static int IDENT;
            public static string TEHNOLOG;
            public static string DAT_RESITVE;
            public static int LETODN;
            public static string PONOVNI;
            public static string VZOREC_VZET;
            public static string USTREZA_PREJ;
            public static string DATPRIJ;
            public static int ID_MHN_VPIS_REZULTATA;
            public static int ID_STATUS;
            public static int ID_VRSTA_ODLOCITVE;
            public static int ID_VRSTA_STANJA;
            public static int ID_MHNS_KOMENTAR_VZORCA;
            public static decimal ELEKTROPREVODNOST;
            public static string UPOGPRE180;
        }

        public static class D133R39
        {
            public static int ID_SKUP_NOSILNI_DELNAL;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int LETODN_NOSILNI;
            public static int DELNAL_NOSILNI;
            public static string VARDELN_NOSILNI;
            public static string DATUM_VNOSA;
            public static string OPOMBA;
            public static string STATUS_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string FIRMA;
        }

        public static class D133R40
        {
            public static int ID_MHN_VZOREC;
            public static int SERIJA;
            public static int LETO;
            public static int OBDSARZA;
            public static int ZAPSTSERIJA;
            public static int ZAPSTPREIZ;
            public static string STSARZE;
            public static int DELNAL;
            public static string PROGENOT;
            public static string FIRMA;
            public static string OBLIKA;
            public static string STATUS;
            public static string VPISAL;
            public static string DATURA;
            public static string USTREZA;
            public static decimal NATTRD;
            public static decimal DOGNAPT;
            public static decimal RAZTEZ;
            public static decimal ZOZANJE;
            public static decimal ERICHSEN;
            public static decimal USESENJE;
            public static string UPOGPRE;
            public static decimal TRDOTA;
            public static int IDENT;
            public static string TEHNOLOG;
            public static string DAT_RESITVE;
            public static int LETODN;
            public static string PONOVNI;
            public static string VZOREC_VZET;
            public static string USTREZA_PREJ;
            public static string DATPRIJ;
            public static int ID_MHN_VPIS_REZULTATA;
            public static int ID_STATUS;
            public static int ID_VRSTA_ODLOCITVE;
            public static int ID_VRSTA_STANJA;
            public static int ID_MHNS_KOMENTAR_VZORCA;
            public static decimal ELEKTROPREVODNOST;
            public static string UPOGPRE180;
        }

        public static class D133R42
        {
            public static int SIFRAEM;
            public static string KRNAZEM;
            public static string POLNAZEM;
            public static string DATURA;
        }

        public static class D133R43
        {
            public static int IDENT;
            public static int STTP;
            public static string DIMENZ40;
            public static string NAZIZDEL;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string KLASIFI1;
            public static string KLASIFI2;
            public static string KLASIFI3;
            public static string KLASIFI4;
            public static string KLASIFI5;
            public static string KLASIFI6;
            public static string TOLERPL;
            public static string TOLERMI;
            public static decimal METKG;
            public static decimal KVMETKG;
            public static decimal KOMADKG;
        }

        public static class D133R53
        {
            public static int ID_SKUP_NOSILNI_DELNAL;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int LETODN_NOSILNI;
            public static int DELNAL_NOSILNI;
            public static string VARDELN_NOSILNI;
            public static string DATUM_VNOSA;
            public static string OPOMBA;
            public static string STATUS_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string FIRMA;
        }

        public static class D133R54
        {
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int STIZDAJ;
            public static int DELNALZ;
            public static string VARDELNZ;
            public static decimal KOLDELNZ;
            public static int DELNALK;
            public static string VARDELNK;
            public static decimal KOLDELNK;
            public static int STNAROC1;
            public static int POZNARO1;
            public static int STEVPOG1;
            public static decimal NARKOL1;
            public static int STNAROC2;
            public static int POZNARO2;
            public static int STEVPOG2;
            public static decimal NARKOL2;
            public static int STNAROC3;
            public static int POZNARO3;
            public static int STEVPOG3;
            public static decimal NARKOL3;
            public static string STPREJEM;
            public static string KRNAZ;
            public static decimal DNKOLIC;
            public static int DNKOMAD;
            public static decimal DNPF;
            public static int IDENT;
            public static string KLASIFI1;
            public static string KLASIFI2;
            public static string KLASIFI3;
            public static string KLASIFI4;
            public static string KLASIFI5;
            public static string KLASIFI6;
            public static string ZLITINA;
            public static string STMASIF;
            public static string STPOSIF;
            public static string NAZIZDEL;
            public static string DIMENZ40;
            public static string TOLERPL;
            public static string TOLERMI;
            public static string STANDID;
            public static string STANKEMS;
            public static string STANMEHL;
            public static string STANDTOL;
            public static int SIGMABSP;
            public static int SIGMABZG;
            public static int SIG02SP;
            public static int SIG02ZG;
            public static int HBSPOD;
            public static int HBZGOR;
            public static string DELTATIP;
            public static decimal DELVRZG;
            public static decimal DELVRSP;
            public static string POSZAHT1;
            public static string POSZAHT2;
            public static int IDENTSUR;
            public static string NAZVHOD;
            public static string DIMENZVH;
            public static string STMAVHOD;
            public static string STPOVHOD;
            public static string OPISVIS;
            public static string INDIKVI;
            public static int POTSURKG;
            public static decimal POTSURKM;
            public static int REZSURKG;
            public static int REZSURKM;
            public static int POTREBKG;
            public static int POTREBKM;
            public static int POTREBTE;
            public static int LANSIRKG;
            public static int LANSIRKM;
            public static int RAZLIKG;
            public static int RAZLIKOM;
            public static int IZRAVNKG;
            public static int STANJEKG;
            public static decimal IZDKOL;
            public static decimal IZDKOL1;
            public static decimal IZDKOL2;
            public static decimal IZDKOL3;
            public static decimal ODPADKOL;
            public static decimal MOTNJKOL;
            public static decimal VISEKG;
            public static string PROGENOT;
            public static int ZACETNTE;
            public static int KONCNATE;
            public static string REFMPP;
            public static string TERMINER;
            public static string STATUSDN;
            public static string SPREMTEH;
            public static string DATSTAT0;
            public static string DATSTAT1;
            public static string DATSTAT2;
            public static string DATSTAT3;
            public static string DATSTAT4;
            public static string DATSTAT5;
            public static string DATSTAT6;
            public static string DATURA;
            public static int ST_TRE;
            public static int STEVKZ;
            public static int VERKZ;
            public static string KZORG;
            public static string ORGEN;
            public static string Q_IND;
            public static decimal UMAX;
            public static decimal IEMIN;
        }

        public static class D133R55
        {
            public static int ID_TT_SKLADSUR_OST;
            public static string FIRMA;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static decimal KOLICINA;
            public static int KOMADI;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static int ID_TT_SKLADSUR_OST1;
            public static string ORGEN;
            public static int IDENT1;
            public static int NAMEN;
            public static string POREKLO;
            public static string SARZA;
            public static int IDENT;
            public static string DIMENZ40;
            public static string NAZIZDEL;
            public static string ZLITINA;
            public static int OZNAKA;
        }

        public static class D133R56
        {
            public static int DELNAL;
            public static string VARDELN;
            public static string STEVSTRO;
            public static int STEVILOP;
            public static string SARZA;
            public static string TRAN_ENOTA;
            public static string DATSPRE;
            public static decimal IZDKOL;
            public static int IZDKOM;
            public static string DELVNOS;
            public static string DELIZDA;
            public static string PROGENOT;
            public static string DATURA;
            public static string KOMENTAR;
            public static string VZOREC;
            public static string STATUSD;
            public static int ZAPSTEV;
            public static string SARZA_DOD;
            public static int ID_FOLP_DOSJE_TRE;
        }

        public static class D133R57
        {
            public static int ID_SKUPPZ_KEM_NML;
            public static int SKLOP_PZ_KEM_NML;
            public static int ID_SKUPKOM_IDENT_PZ;
            public static int ID_SKUPKOM_IDENT_KEM;
            public static int ID_SKUPKOM_IDENT_NML;
            public static string KOMITENT;
            public static int IDENT;
            public static int STEVKZ;
            public static int VERKZ;
            public static string OPOMBA;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static int MIN_SKLOP_PZ_KEM_NML;
        }

        public static class D133R58
        {
            public static int ID_VALP_TEHN_DN_STROJ_ATRIBUT;
            public static string FIRMA;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static int TRAN_ENOTA;
            public static int ID_VALP_TEHN_STROJI_P_ATRIBUT;
            public static int ID_VALS_TEHN_PROFIL;
            public static string VREDNOST;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string PREVZETO;
            public static string DAT_PREVZEMA;
            public static string MIN_VRED;
            public static string MAX_VRED;
        }

        public static class D133R59
        {
            public static int DELNAL;
            public static string VARDELN;
            public static int IDENT;
            public static int STEVILOP;
            public static string STEVSTRO;
            public static string TEHNAVO1;
            public static string TEHNAVO2;
            public static string TEHNAVO3;
            public static decimal STEVIDEL;
            public static decimal TPZ;
            public static decimal T1;
            public static decimal T2;
            public static decimal PFO;
            public static string KVALODPD;
            public static decimal PFDOOPER;
            public static string DATURA;
        }

        public static class D133R60
        {
            public static int ID;
            public static int LETODN;
            public static int DELNAL;
            public static string VARDELN;
            public static string PROGENOT;
            public static int IDENT;
            public static int IDENTSUR;
            public static int TRAN_ENOTA;
            public static string SARZA;
            public static string SARZA_DOD;
            public static int ZAPST_PAKET;
            public static int ID_VRSTA_NAPAKE_NCR;
            public static string SIFMOT;
            public static int ID_VZROK_NAPAKE_NCR;
            public static int ID_UKREP_NAPAKE_NCR;
            public static string OSEBA_NCR;
            public static string DATUM_NCR;
            public static string OPOMBA;
            public static int ID_SLIK_NAPAK;
            public static int ID_TVEGANJE_NAPAKE_NCR;
            public static string NCR_ODOBRITEV;
            public static string NCR_OSEBA_ODOBRITVE;
            public static string NCR_DATUM_ODOBRITVE;
            public static int ID_NAVODILO_NAPAKE_NCR;
            public static string DATUM_PRIJAVE;
            public static int TRAN_ENOTA_NOVA;
            public static string SARZA_NOVA;
            public static decimal KOLICINA;
            public static string ZVAR_STATUS;
            public static string ZRACNOST_STATUS;
            public static string DEBELINA_STATUS;
            public static decimal DEBELINA;
            public static string SIRINA_STATUS;
            public static decimal SIRINA;
            public static string ZUNPREM_STATUS;
            public static decimal ZUNPREM;
            public static string NOTPREM_STATUS;
            public static decimal NOTPREM;
            public static int ID_SLIK_SUROVINE;
            public static string USTREZA_PTP;
            public static string OPOMBA_PTP;
            public static string OSEBA_PTP;
            public static string DATUM_PTP;
            public static string STANJE_ZADNJE_SPREMEMBE;
            public static string DATURA_ZADNJE_SPREMEMBE;
            public static string OSEBA_ZADNJE_SPREMEMBE;
            public static string NCR_ODOBRITEV_OPOMBA;
        }

        public static class D133W01
        {
            /// <summary>
            /// 'ime programa 30x'
            /// </summary>
            public static string APPL;
            /// <summary>
            /// 'priimek in ime uporabnika'
            /// </summary>
            public static string ZAPRIIM;
            /// <summary>
            /// 'stevec programov'
            /// </summary>
            public static int STEVAPPL;
            /// <summary>
            /// 'stevilka funkcije'
            /// </summary>
            public static int FUNKCIJA;
            /// <summary>
            /// 'PRINTER'
            /// </summary>
            public static string PRINTER;
            /// <summary>
            /// 'podjetje'
            /// </summary>
            public static string FIRMA;
            /// <summary>
            /// 'uporabnisko geslo'
            /// </summary>
            public static string ZASIFRA;
            /// <summary>
            /// 'naziv podjetja'
            /// </summary>
            public static string NAZIVF1;
            /// <summary>
            /// 'zaporedna stevilka osebe'
            /// </summary>
            public static int IDZAPST;
            /// <summary>
            /// 'govorno podrocje'
            /// </summary>
            public static string PODRGOV;
            /// <summary>
            /// 'skupina uporabnika'
            /// </summary>
            public static string AVTORIZ;
            /// <summary>
            /// 'rezerva 1'
            /// </summary>
            public static string R1;
            /// <summary>
            /// 'rezerva 2'
            /// </summary>
            public static int R2;
            /// <summary>
            /// 'DELNAL'
            /// </summary>
            public static int DELNAL;
            /// <summary>
            /// 'VARDELN'
            /// </summary>
            public static string VARDELN;
        }

        public static class D133W02
        {
            public static int STEVKZ;
            public static int VERKZ;
            /// <summary>
            /// 'Imp. št. KZ-ja'
            /// </summary>
            public static string KZORG;
            /// <summary>
            /// 'prejemnik na KZ'
            /// </summary>
            public static string PREJEMNIK;
            /// <summary>
            /// 'iz katerega programa prihaja'
            /// </summary>
            public static string APLIK;
        }

        public static class D133W03
        {
            public static string UPORABNIK;
            /// <summary>
            /// 'Delovni nalog'
            /// </summary>
            public static int DELNAL;
            /// <summary>
            /// 'Varianta del. nal.'
            /// </summary>
            public static string VARDELN;
            /// <summary>
            /// 'številka operacije'
            /// </summary>
            public static int STEVILOP;
            /// <summary>
            /// 'iz katerega programa prihaja'
            /// </summary>
            public static string APLIK;
        }

        public static class D133W04
        {
            public static int STEVILOP;
            public static string STEVSTRO;
            public static string NAZSTRKR;
            public static string NASEL;
            public static string NASEL5;
            public static string NASEL6;
            public static int STEVEC;
            public static int STEVEC5;
            public static string KONEC;
            public static int INDIK;
            public static decimal IZDKOL;
            public static decimal TOL1_PL;
            public static decimal TOL1_MI;
            public static decimal TOL2_PL;
            public static decimal TOL2_MI;
            public static decimal TOL3_PL;
            public static decimal TOL3_MI;
            /// <summary>
            /// 'iz tt_naropozi'
            /// </summary>
            public static decimal TOL_KOL_PL;
            /// <summary>
            /// 'iz tt_naropozi'
            /// </summary>
            public static decimal TOL_KOL_MI;
            /// <summary>
            /// 'iz tt_naropozi'
            /// </summary>
            public static int EM_TOL_KOL;
            /// <summary>
            /// 'količina na naročilih'
            /// </summary>
            public static decimal KOL_NAR1;
            /// <summary>
            /// 'količina na naročilih'
            /// </summary>
            public static decimal KOL_TEMP;
            /// <summary>
            /// 'količina na naročilih'
            /// </summary>
            public static decimal KOL_TEMP_IZD;
            /// <summary>
            /// 'količina na naročilih'
            /// </summary>
            public static decimal KOL_NAR2;
            /// <summary>
            /// 'količina na naročilih'
            /// </summary>
            public static decimal KOL_NAR3;
            public static int MAX_NOSILNI;
            public static int TEMP_NAR;
            /// <summary>
            /// 'indikator staranja'
            /// </summary>
            public static int IND_STARANJA;
            public static string STIMA;
            public static int O;
            public static string TRE_SEST;
            public static string TEKST1;
            public static string TEKST2;
            // public static string _*; // VIRTUAL FILLER
            public static string SARZA18;
            public static string SARZA12;
            public static string SARZA1;
            public static string SARZA5;
            /// <summary>
            /// 'generirana sarža'
            /// </summary>
            public static int IDSARZA;
            /// <summary>
            /// 'ali je v testu'
            /// </summary>
            public static string SISTEM;
            public static int SUMA_VZORCEV;
            public static int SUMA_REZULTATOV;
            public static int ID_JEZIK;
            public static string FIRMA_VH;
            /// <summary>
            /// 'da ali ne'
            /// </summary>
            public static string MAGNA;
        }

        public static class D133W05
        {
            /// <summary>
            /// 'sestavljena sarža'
            /// </summary>
            public static string SARZA_SES;
            public static string SAR1;
            public static string SAR2;
            /// <summary>
            /// 'polje za saržo na M2'
            /// </summary>
            public static string KZ_SUR3;
            public static string POLJE_SAR;
            /// <summary>
            /// 'sarža'
            /// </summary>
            public static string SARZA_M2;
            public static string P1;
            public static int STEV_SARZA;
            public static int STEVEC15;
            public static string POLNI_SARZA;
            public static string SARZA_ID;
            public static int SARZA_NUM;
        }

        public static class TE99W01
        {
            public static string CTRSTRAN;
            public static string CTRSTRA1;
            public static string CTRLINIJ;
            public static string CTRSTART;
            public static string CTRCURSO;
            public static string CTRKOPIJ;
            public static string FLOK;
            public static string FLOK1;
            public static string FLNAPREJ;
            public static string FLZAPIS;
            public static string FLZAPIS1;
            public static string FLBRISI;
            public static string TEMN10D0;
            public static string TE1N10D0;
            public static string TEMN10D4;
            public static string TEMN10D2;
            public static string TE1N10D4;
            public static string TE1N10D2;
            public static string TE2N10D4;
            public static string TE2N10D3;
            // public static string _*; // VIRTUAL FILLER
            public static string MSG78;
            public static string MSG64;
            public static string MSGCOD;
            public static string MSGN3;
            // public static string _*; // VIRTUAL FILLER
            public static string TIMEST1;
        }

    }
#endregion

#region MAPS
    /// <summary>
    /// A constant (read-only) field on the screen.
    /// </summary>
    public class CfieldTag
    {
        public int Row { get; set; }
        public int Column { get; set; }
        public string Type { get; set; } = "";
        public int Bytes { get; set; }
        public string Value { get; set; } = "";
        public override string ToString() =>
            $"CFIELD [{Row},{Column}] {Type}/{Bytes}B: '{Value}'";
    }
    /// <summary>
    /// A variable (read-write) field on the screen, with runtime MDT & intensity.
    /// </summary>
    public class VfieldTag
    {
        public event Action<VfieldTag>? OnCursor;
        public int Row { get; set; }
        public int Column { get; set; }
        public string Name { get; set; } = "";
        public string Type { get; set; } = "";
        public int Bytes { get; set; }
        public string Value { get; set; } = "";
        public bool IsModified { get; private set; }
        public string Intensity { get; private set; } = "NORMAL";
        public void SetModified() => IsModified = true;
        public void ClearModified() => IsModified = false;
        public void SetDark() => Intensity = "DARK";
        public void SetBright() => Intensity = "BRIGHT";
        public void SetNormal() => Intensity = "NORMAL";
        public void SetCursor() => OnCursor?.Invoke(this);
        public override string ToString() =>
            $"VFIELD [{Row},{Column}] {Name}/{Type}/{Bytes}B: '{Value}' (MDT={IsModified}, Intensity={Intensity})";
    }
public static class GlobalMaps
{
    public static class D133H01
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        static D133H01()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133H01");
                WriteWrapped(8, 0, "");
                WriteWrapped(23, 0, "Pomoč za delo s programom D133A");
                WriteWrapped(55, 0, "");
                WriteWrapped(59, 0, "");
                WriteWrapped(0, 2, "S tem programom lahko gledamo podatke v zvezi z delovnim nalogom");
                WriteWrapped(66, 2, "");
                WriteWrapped(1, 3, "in sicer:");
                WriteWrapped(11, 3, "");
                WriteWrapped(2, 4, "- na tej mapi generalne podatke o delovnem nalogu in z PF 4 tipko: - detalj 2");
                WriteWrapped(0, 5, "");
                WriteWrapped(76, 5, "");
                WriteWrapped(7, 6, "- detalj 2, kjer lahko vidimo preostale podatke DN in siv+cer stanje");
                WriteWrapped(76, 6, "");
                WriteWrapped(9, 7, "na operacijah - do 7 operacij lahko prikaže, kjer imamo možnosti:");
                WriteWrapped(75, 7, "");
                WriteWrapped(79, 7, "");
                WriteWrapped(61, 8, "");
                WriteWrapped(62, 8, "");
                WriteWrapped(10, 9, "- F4 - prikaz transp.enot za izbrano operacijo-kurzor na operaciji");
                WriteWrapped(78, 9, "");
                WriteWrapped(17, 10, "(prikaz 22 TRE, z F8 dobimo naslednjih 22)");
                WriteWrapped(60, 10, "");
                WriteWrapped(77, 10, "");
                WriteWrapped(37, 11, "");
                WriteWrapped(10, 12, "- F5 - prikaz do 20 motenj tega delovnega naloga");
                WriteWrapped(59, 12, "");
                WriteWrapped(17, 13, "(sortirano po datumu prijave padajoče - zadnja prijavljena");
                WriteWrapped(76, 13, "");
                WriteWrapped(17, 14, "je prva prikazana)");
                WriteWrapped(36, 14, "");
                WriteWrapped(1, 23, "Pritisni karkoli za povratek");
                WriteWrapped(30, 23, "");
            
            
                Console.SetCursorPosition(0, 0);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
    public static class D133M01
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
            new VfieldTag { Row = 1, Column = 21, Name = "LB1", Type = "CHA", Bytes = 30, Value = "" },
            new VfieldTag { Row = 1, Column = 64, Name = "REFMPP", Type = "CHA", Bytes = 15, Value = "" },
            new VfieldTag { Row = 2, Column = 1, Name = "DATUM", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 2, Column = 12, Name = "MOTNJA", Type = "CHA", Bytes = 6, Value = "" },
            new VfieldTag { Row = 2, Column = 19, Name = "UKREP", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 2, Column = 21, Name = "NCR", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 2, Column = 64, Name = "STATUSDN", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 2, Column = 75, Name = "SPREMTEH", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 3, Column = 1, Name = "LB2", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 3, Column = 12, Name = "DELNAL", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 3, Column = 20, Name = "VARDELN", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 3, Column = 27, Name = "DELNALK", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 3, Column = 38, Name = "DELNALZ", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 3, Column = 44, Name = "VARDELNZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 3, Column = 52, Name = "PROGENOT", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 3, Column = 65, Name = "DNPF", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 4, Column = 1, Name = "LB3", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 4, Column = 12, Name = "ZACETNTE", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 4, Column = 27, Name = "KONCNATE", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 4, Column = 44, Name = "DNKOLIC", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 4, Column = 56, Name = "DNKOMAD", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 5, Column = 1, Name = "LB4", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 5, Column = 12, Name = "STNAROC1", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 5, Column = 20, Name = "POZNARO1", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 5, Column = 25, Name = "STEVPOG1", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 5, Column = 28, Name = "KRNAZ", Type = "CHA", Bytes = 29, Value = "" },
            new VfieldTag { Row = 5, Column = 58, Name = "NARKOL1", Type = "NUM", Bytes = 11, Value = "" },
            new VfieldTag { Row = 5, Column = 73, Name = "NOVI_ROK1", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 6, Column = 1, Name = "LB5", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 6, Column = 12, Name = "STNAROC2", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 6, Column = 20, Name = "POZNARO2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 6, Column = 25, Name = "STEVPOG2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 6, Column = 28, Name = "KRNAZ2", Type = "CHA", Bytes = 29, Value = "" },
            new VfieldTag { Row = 6, Column = 58, Name = "NARKOL2", Type = "NUM", Bytes = 11, Value = "" },
            new VfieldTag { Row = 6, Column = 73, Name = "NOVI_ROK2", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 7, Column = 1, Name = "LB6", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 7, Column = 12, Name = "STNAROC3", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 7, Column = 20, Name = "POZNARO3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 7, Column = 25, Name = "STEVPOG3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 7, Column = 28, Name = "KRNAZ3", Type = "CHA", Bytes = 29, Value = "" },
            new VfieldTag { Row = 7, Column = 58, Name = "NARKOL3", Type = "NUM", Bytes = 11, Value = "" },
            new VfieldTag { Row = 7, Column = 73, Name = "NOVI_ROK3", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 8, Column = 1, Name = "LB24", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 8, Column = 14, Name = "DELNAL_NOSILNI", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 8, Column = 20, Name = "OPOMBA", Type = "CHA", Bytes = 59, Value = "" },
            new VfieldTag { Row = 9, Column = 10, Name = "IDENT", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 9, Column = 34, Name = "KLASIFI1", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 37, Name = "KLASIFI2", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 40, Name = "KLASIFI3", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 43, Name = "KLASIFI4", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 46, Name = "KLASIFI5", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 49, Name = "KLASIFI6", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 74, Name = "ZLITINA", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 10, Column = 10, Name = "NAZIZDEL", Type = "CHA", Bytes = 20, Value = "" },
            new VfieldTag { Row = 10, Column = 46, Name = "KZORG", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 10, Column = 52, Name = "VERKZ", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 10, Column = 55, Name = "STEVKZ", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 10, Column = 74, Name = "STMASIF", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 11, Column = 10, Name = "DIMENZ40", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 11, Column = 74, Name = "STPOSIF", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 12, Column = 10, Name = "TOLERPL", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 12, Column = 51, Name = "LAB_DOL", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 12, Column = 56, Name = "DOLZINA_KOL", Type = "CHA", Bytes = 6, Value = "" },
            new VfieldTag { Row = 12, Column = 63, Name = "LAB_DOL1", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 12, Column = 67, Name = "TOL_DOL_PL", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 12, Column = 72, Name = "LAB_DOL2", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 12, Column = 75, Name = "TOL_DOL_MI", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 13, Column = 10, Name = "TOLERMI", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 13, Column = 69, Name = "NAMEN", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 78, Name = "ATRIBUT", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 1, Name = "LB7", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 14, Column = 19, Name = "DIMENZOR", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 15, Column = 1, Name = "LB8", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 15, Column = 19, Name = "POSZAHT1", Type = "CHA", Bytes = 60, Value = "" },
            new VfieldTag { Row = 16, Column = 10, Name = "POREKLO", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 16, Column = 19, Name = "POSZAHT2", Type = "CHA", Bytes = 60, Value = "" },
            new VfieldTag { Row = 17, Column = 68, Name = "OPISVIS", Type = "CHA", Bytes = 11, Value = "" },
            new VfieldTag { Row = 18, Column = 1, Name = "LB18", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 18, Column = 10, Name = "INDIKVI", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 12, Name = "IDENTSUR", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 18, Column = 19, Name = "DIMENZVH", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 18, Column = 60, Name = "NAZVHOD", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 19, Column = 19, Name = "STMAVHOD", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 27, Name = "STPOVHOD", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 33, Name = "KZ_SUR3", Type = "CHA", Bytes = 46, Value = "" },
            new VfieldTag { Row = 20, Column = 1, Name = "LB9", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 20, Column = 18, Name = "POTSURKG", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 20, Column = 27, Name = "LB12", Type = "CHA", Bytes = 15, Value = "" },
            new VfieldTag { Row = 20, Column = 43, Name = "IZDKOL", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 20, Column = 52, Name = "LB15", Type = "CHA", Bytes = 14, Value = "" },
            new VfieldTag { Row = 20, Column = 67, Name = "MOTNJKOL", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 21, Column = 1, Name = "LB10", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 21, Column = 18, Name = "LANSIRKG", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 21, Column = 27, Name = "LB13", Type = "CHA", Bytes = 15, Value = "" },
            new VfieldTag { Row = 21, Column = 43, Name = "ODPADKOL", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 21, Column = 52, Name = "LB16", Type = "CHA", Bytes = 14, Value = "" },
            new VfieldTag { Row = 21, Column = 67, Name = "ODPADMOT", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 22, Column = 1, Name = "LB11", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 22, Column = 18, Name = "RAZLIKG", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 22, Column = 27, Name = "LB14", Type = "CHA", Bytes = 15, Value = "" },
            new VfieldTag { Row = 22, Column = 43, Name = "STANJEKG", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 22, Column = 52, Name = "LB17", Type = "CHA", Bytes = 14, Value = "" },
            new VfieldTag { Row = 22, Column = 67, Name = "VISEKG", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 23, Column = 1, Name = "EZEMSG", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 24, Column = 1, Name = "LB19", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 24, Column = 66, Name = "LB20", Type = "CHA", Bytes = 13, Value = "" },
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        /// <summary>Variable field 'LB1'</summary>
        public static VfieldTag LB1Tag => _vfieldsByName["LB1"][0];

        public static string LB1
        {
            get => LB1Tag.Value;
            set => LB1Tag.Value = value;
        }

        /// <summary>Variable field 'REFMPP'</summary>
        public static VfieldTag REFMPPTag => _vfieldsByName["REFMPP"][0];

        public static string REFMPP
        {
            get => REFMPPTag.Value;
            set => REFMPPTag.Value = value;
        }

        /// <summary>Variable field 'DATUM'</summary>
        public static VfieldTag DATUMTag => _vfieldsByName["DATUM"][0];

        public static string DATUM
        {
            get => DATUMTag.Value;
            set => DATUMTag.Value = value;
        }

        /// <summary>Variable field 'MOTNJA'</summary>
        public static VfieldTag MOTNJATag => _vfieldsByName["MOTNJA"][0];

        public static string MOTNJA
        {
            get => MOTNJATag.Value;
            set => MOTNJATag.Value = value;
        }

        /// <summary>Variable field 'UKREP'</summary>
        public static VfieldTag UKREPTag => _vfieldsByName["UKREP"][0];

        public static string UKREP
        {
            get => UKREPTag.Value;
            set => UKREPTag.Value = value;
        }

        /// <summary>Variable field 'NCR'</summary>
        public static VfieldTag NCRTag => _vfieldsByName["NCR"][0];

        public static string NCR
        {
            get => NCRTag.Value;
            set => NCRTag.Value = value;
        }

        /// <summary>Variable field 'STATUSDN'</summary>
        public static VfieldTag STATUSDNTag => _vfieldsByName["STATUSDN"][0];

        public static string STATUSDN
        {
            get => STATUSDNTag.Value;
            set => STATUSDNTag.Value = value;
        }

        /// <summary>Variable field 'SPREMTEH'</summary>
        public static VfieldTag SPREMTEHTag => _vfieldsByName["SPREMTEH"][0];

        public static string SPREMTEH
        {
            get => SPREMTEHTag.Value;
            set => SPREMTEHTag.Value = value;
        }

        /// <summary>Variable field 'LB2'</summary>
        public static VfieldTag LB2Tag => _vfieldsByName["LB2"][0];

        public static string LB2
        {
            get => LB2Tag.Value;
            set => LB2Tag.Value = value;
        }

        /// <summary>Variable field 'DELNAL'</summary>
        public static VfieldTag DELNALTag => _vfieldsByName["DELNAL"][0];

        public static string DELNAL
        {
            get => DELNALTag.Value;
            set => DELNALTag.Value = value;
        }

        /// <summary>Variable field 'VARDELN'</summary>
        public static VfieldTag VARDELNTag => _vfieldsByName["VARDELN"][0];

        public static string VARDELN
        {
            get => VARDELNTag.Value;
            set => VARDELNTag.Value = value;
        }

        /// <summary>Variable field 'DELNALK'</summary>
        public static VfieldTag DELNALKTag => _vfieldsByName["DELNALK"][0];

        public static string DELNALK
        {
            get => DELNALKTag.Value;
            set => DELNALKTag.Value = value;
        }

        /// <summary>Variable field 'DELNALZ'</summary>
        public static VfieldTag DELNALZTag => _vfieldsByName["DELNALZ"][0];

        public static string DELNALZ
        {
            get => DELNALZTag.Value;
            set => DELNALZTag.Value = value;
        }

        /// <summary>Variable field 'VARDELNZ'</summary>
        public static VfieldTag VARDELNZTag => _vfieldsByName["VARDELNZ"][0];

        public static string VARDELNZ
        {
            get => VARDELNZTag.Value;
            set => VARDELNZTag.Value = value;
        }

        /// <summary>Variable field 'PROGENOT'</summary>
        public static VfieldTag PROGENOTTag => _vfieldsByName["PROGENOT"][0];

        public static string PROGENOT
        {
            get => PROGENOTTag.Value;
            set => PROGENOTTag.Value = value;
        }

        /// <summary>Variable field 'DNPF'</summary>
        public static VfieldTag DNPFTag => _vfieldsByName["DNPF"][0];

        public static string DNPF
        {
            get => DNPFTag.Value;
            set => DNPFTag.Value = value;
        }

        /// <summary>Variable field 'LB3'</summary>
        public static VfieldTag LB3Tag => _vfieldsByName["LB3"][0];

        public static string LB3
        {
            get => LB3Tag.Value;
            set => LB3Tag.Value = value;
        }

        /// <summary>Variable field 'ZACETNTE'</summary>
        public static VfieldTag ZACETNTETag => _vfieldsByName["ZACETNTE"][0];

        public static string ZACETNTE
        {
            get => ZACETNTETag.Value;
            set => ZACETNTETag.Value = value;
        }

        /// <summary>Variable field 'KONCNATE'</summary>
        public static VfieldTag KONCNATETag => _vfieldsByName["KONCNATE"][0];

        public static string KONCNATE
        {
            get => KONCNATETag.Value;
            set => KONCNATETag.Value = value;
        }

        /// <summary>Variable field 'DNKOLIC'</summary>
        public static VfieldTag DNKOLICTag => _vfieldsByName["DNKOLIC"][0];

        public static string DNKOLIC
        {
            get => DNKOLICTag.Value;
            set => DNKOLICTag.Value = value;
        }

        /// <summary>Variable field 'DNKOMAD'</summary>
        public static VfieldTag DNKOMADTag => _vfieldsByName["DNKOMAD"][0];

        public static string DNKOMAD
        {
            get => DNKOMADTag.Value;
            set => DNKOMADTag.Value = value;
        }

        /// <summary>Variable field 'LB4'</summary>
        public static VfieldTag LB4Tag => _vfieldsByName["LB4"][0];

        public static string LB4
        {
            get => LB4Tag.Value;
            set => LB4Tag.Value = value;
        }

        /// <summary>Variable field 'STNAROC1'</summary>
        public static VfieldTag STNAROC1Tag => _vfieldsByName["STNAROC1"][0];

        public static string STNAROC1
        {
            get => STNAROC1Tag.Value;
            set => STNAROC1Tag.Value = value;
        }

        /// <summary>Variable field 'POZNARO1'</summary>
        public static VfieldTag POZNARO1Tag => _vfieldsByName["POZNARO1"][0];

        public static string POZNARO1
        {
            get => POZNARO1Tag.Value;
            set => POZNARO1Tag.Value = value;
        }

        /// <summary>Variable field 'STEVPOG1'</summary>
        public static VfieldTag STEVPOG1Tag => _vfieldsByName["STEVPOG1"][0];

        public static string STEVPOG1
        {
            get => STEVPOG1Tag.Value;
            set => STEVPOG1Tag.Value = value;
        }

        /// <summary>Variable field 'KRNAZ'</summary>
        public static VfieldTag KRNAZTag => _vfieldsByName["KRNAZ"][0];

        public static string KRNAZ
        {
            get => KRNAZTag.Value;
            set => KRNAZTag.Value = value;
        }

        /// <summary>Variable field 'NARKOL1'</summary>
        public static VfieldTag NARKOL1Tag => _vfieldsByName["NARKOL1"][0];

        public static string NARKOL1
        {
            get => NARKOL1Tag.Value;
            set => NARKOL1Tag.Value = value;
        }

        /// <summary>Variable field 'NOVI_ROK1'</summary>
        public static VfieldTag NOVI_ROK1Tag => _vfieldsByName["NOVI_ROK1"][0];

        public static string NOVI_ROK1
        {
            get => NOVI_ROK1Tag.Value;
            set => NOVI_ROK1Tag.Value = value;
        }

        /// <summary>Variable field 'LB5'</summary>
        public static VfieldTag LB5Tag => _vfieldsByName["LB5"][0];

        public static string LB5
        {
            get => LB5Tag.Value;
            set => LB5Tag.Value = value;
        }

        /// <summary>Variable field 'STNAROC2'</summary>
        public static VfieldTag STNAROC2Tag => _vfieldsByName["STNAROC2"][0];

        public static string STNAROC2
        {
            get => STNAROC2Tag.Value;
            set => STNAROC2Tag.Value = value;
        }

        /// <summary>Variable field 'POZNARO2'</summary>
        public static VfieldTag POZNARO2Tag => _vfieldsByName["POZNARO2"][0];

        public static string POZNARO2
        {
            get => POZNARO2Tag.Value;
            set => POZNARO2Tag.Value = value;
        }

        /// <summary>Variable field 'STEVPOG2'</summary>
        public static VfieldTag STEVPOG2Tag => _vfieldsByName["STEVPOG2"][0];

        public static string STEVPOG2
        {
            get => STEVPOG2Tag.Value;
            set => STEVPOG2Tag.Value = value;
        }

        /// <summary>Variable field 'KRNAZ2'</summary>
        public static VfieldTag KRNAZ2Tag => _vfieldsByName["KRNAZ2"][0];

        public static string KRNAZ2
        {
            get => KRNAZ2Tag.Value;
            set => KRNAZ2Tag.Value = value;
        }

        /// <summary>Variable field 'NARKOL2'</summary>
        public static VfieldTag NARKOL2Tag => _vfieldsByName["NARKOL2"][0];

        public static string NARKOL2
        {
            get => NARKOL2Tag.Value;
            set => NARKOL2Tag.Value = value;
        }

        /// <summary>Variable field 'NOVI_ROK2'</summary>
        public static VfieldTag NOVI_ROK2Tag => _vfieldsByName["NOVI_ROK2"][0];

        public static string NOVI_ROK2
        {
            get => NOVI_ROK2Tag.Value;
            set => NOVI_ROK2Tag.Value = value;
        }

        /// <summary>Variable field 'LB6'</summary>
        public static VfieldTag LB6Tag => _vfieldsByName["LB6"][0];

        public static string LB6
        {
            get => LB6Tag.Value;
            set => LB6Tag.Value = value;
        }

        /// <summary>Variable field 'STNAROC3'</summary>
        public static VfieldTag STNAROC3Tag => _vfieldsByName["STNAROC3"][0];

        public static string STNAROC3
        {
            get => STNAROC3Tag.Value;
            set => STNAROC3Tag.Value = value;
        }

        /// <summary>Variable field 'POZNARO3'</summary>
        public static VfieldTag POZNARO3Tag => _vfieldsByName["POZNARO3"][0];

        public static string POZNARO3
        {
            get => POZNARO3Tag.Value;
            set => POZNARO3Tag.Value = value;
        }

        /// <summary>Variable field 'STEVPOG3'</summary>
        public static VfieldTag STEVPOG3Tag => _vfieldsByName["STEVPOG3"][0];

        public static string STEVPOG3
        {
            get => STEVPOG3Tag.Value;
            set => STEVPOG3Tag.Value = value;
        }

        /// <summary>Variable field 'KRNAZ3'</summary>
        public static VfieldTag KRNAZ3Tag => _vfieldsByName["KRNAZ3"][0];

        public static string KRNAZ3
        {
            get => KRNAZ3Tag.Value;
            set => KRNAZ3Tag.Value = value;
        }

        /// <summary>Variable field 'NARKOL3'</summary>
        public static VfieldTag NARKOL3Tag => _vfieldsByName["NARKOL3"][0];

        public static string NARKOL3
        {
            get => NARKOL3Tag.Value;
            set => NARKOL3Tag.Value = value;
        }

        /// <summary>Variable field 'NOVI_ROK3'</summary>
        public static VfieldTag NOVI_ROK3Tag => _vfieldsByName["NOVI_ROK3"][0];

        public static string NOVI_ROK3
        {
            get => NOVI_ROK3Tag.Value;
            set => NOVI_ROK3Tag.Value = value;
        }

        /// <summary>Variable field 'LB24'</summary>
        public static VfieldTag LB24Tag => _vfieldsByName["LB24"][0];

        public static string LB24
        {
            get => LB24Tag.Value;
            set => LB24Tag.Value = value;
        }

        /// <summary>Variable field 'DELNAL_NOSILNI'</summary>
        public static VfieldTag DELNAL_NOSILNITag => _vfieldsByName["DELNAL_NOSILNI"][0];

        public static string DELNAL_NOSILNI
        {
            get => DELNAL_NOSILNITag.Value;
            set => DELNAL_NOSILNITag.Value = value;
        }

        /// <summary>Variable field 'OPOMBA'</summary>
        public static VfieldTag OPOMBATag => _vfieldsByName["OPOMBA"][0];

        public static string OPOMBA
        {
            get => OPOMBATag.Value;
            set => OPOMBATag.Value = value;
        }

        /// <summary>Variable field 'IDENT'</summary>
        public static VfieldTag IDENTTag => _vfieldsByName["IDENT"][0];

        public static string IDENT
        {
            get => IDENTTag.Value;
            set => IDENTTag.Value = value;
        }

        /// <summary>Variable field 'KLASIFI1'</summary>
        public static VfieldTag KLASIFI1Tag => _vfieldsByName["KLASIFI1"][0];

        public static string KLASIFI1
        {
            get => KLASIFI1Tag.Value;
            set => KLASIFI1Tag.Value = value;
        }

        /// <summary>Variable field 'KLASIFI2'</summary>
        public static VfieldTag KLASIFI2Tag => _vfieldsByName["KLASIFI2"][0];

        public static string KLASIFI2
        {
            get => KLASIFI2Tag.Value;
            set => KLASIFI2Tag.Value = value;
        }

        /// <summary>Variable field 'KLASIFI3'</summary>
        public static VfieldTag KLASIFI3Tag => _vfieldsByName["KLASIFI3"][0];

        public static string KLASIFI3
        {
            get => KLASIFI3Tag.Value;
            set => KLASIFI3Tag.Value = value;
        }

        /// <summary>Variable field 'KLASIFI4'</summary>
        public static VfieldTag KLASIFI4Tag => _vfieldsByName["KLASIFI4"][0];

        public static string KLASIFI4
        {
            get => KLASIFI4Tag.Value;
            set => KLASIFI4Tag.Value = value;
        }

        /// <summary>Variable field 'KLASIFI5'</summary>
        public static VfieldTag KLASIFI5Tag => _vfieldsByName["KLASIFI5"][0];

        public static string KLASIFI5
        {
            get => KLASIFI5Tag.Value;
            set => KLASIFI5Tag.Value = value;
        }

        /// <summary>Variable field 'KLASIFI6'</summary>
        public static VfieldTag KLASIFI6Tag => _vfieldsByName["KLASIFI6"][0];

        public static string KLASIFI6
        {
            get => KLASIFI6Tag.Value;
            set => KLASIFI6Tag.Value = value;
        }

        /// <summary>Variable field 'ZLITINA'</summary>
        public static VfieldTag ZLITINATag => _vfieldsByName["ZLITINA"][0];

        public static string ZLITINA
        {
            get => ZLITINATag.Value;
            set => ZLITINATag.Value = value;
        }

        /// <summary>Variable field 'NAZIZDEL'</summary>
        public static VfieldTag NAZIZDELTag => _vfieldsByName["NAZIZDEL"][0];

        public static string NAZIZDEL
        {
            get => NAZIZDELTag.Value;
            set => NAZIZDELTag.Value = value;
        }

        /// <summary>Variable field 'KZORG'</summary>
        public static VfieldTag KZORGTag => _vfieldsByName["KZORG"][0];

        public static string KZORG
        {
            get => KZORGTag.Value;
            set => KZORGTag.Value = value;
        }

        /// <summary>Variable field 'VERKZ'</summary>
        public static VfieldTag VERKZTag => _vfieldsByName["VERKZ"][0];

        public static string VERKZ
        {
            get => VERKZTag.Value;
            set => VERKZTag.Value = value;
        }

        /// <summary>Variable field 'STEVKZ'</summary>
        public static VfieldTag STEVKZTag => _vfieldsByName["STEVKZ"][0];

        public static string STEVKZ
        {
            get => STEVKZTag.Value;
            set => STEVKZTag.Value = value;
        }

        /// <summary>Variable field 'STMASIF'</summary>
        public static VfieldTag STMASIFTag => _vfieldsByName["STMASIF"][0];

        public static string STMASIF
        {
            get => STMASIFTag.Value;
            set => STMASIFTag.Value = value;
        }

        /// <summary>Variable field 'DIMENZ40'</summary>
        public static VfieldTag DIMENZ40Tag => _vfieldsByName["DIMENZ40"][0];

        public static string DIMENZ40
        {
            get => DIMENZ40Tag.Value;
            set => DIMENZ40Tag.Value = value;
        }

        /// <summary>Variable field 'STPOSIF'</summary>
        public static VfieldTag STPOSIFTag => _vfieldsByName["STPOSIF"][0];

        public static string STPOSIF
        {
            get => STPOSIFTag.Value;
            set => STPOSIFTag.Value = value;
        }

        /// <summary>Variable field 'TOLERPL'</summary>
        public static VfieldTag TOLERPLTag => _vfieldsByName["TOLERPL"][0];

        public static string TOLERPL
        {
            get => TOLERPLTag.Value;
            set => TOLERPLTag.Value = value;
        }

        /// <summary>Variable field 'LAB_DOL'</summary>
        public static VfieldTag LAB_DOLTag => _vfieldsByName["LAB_DOL"][0];

        public static string LAB_DOL
        {
            get => LAB_DOLTag.Value;
            set => LAB_DOLTag.Value = value;
        }

        /// <summary>Variable field 'DOLZINA_KOL'</summary>
        public static VfieldTag DOLZINA_KOLTag => _vfieldsByName["DOLZINA_KOL"][0];

        public static string DOLZINA_KOL
        {
            get => DOLZINA_KOLTag.Value;
            set => DOLZINA_KOLTag.Value = value;
        }

        /// <summary>Variable field 'LAB_DOL1'</summary>
        public static VfieldTag LAB_DOL1Tag => _vfieldsByName["LAB_DOL1"][0];

        public static string LAB_DOL1
        {
            get => LAB_DOL1Tag.Value;
            set => LAB_DOL1Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_DOL_PL'</summary>
        public static VfieldTag TOL_DOL_PLTag => _vfieldsByName["TOL_DOL_PL"][0];

        public static string TOL_DOL_PL
        {
            get => TOL_DOL_PLTag.Value;
            set => TOL_DOL_PLTag.Value = value;
        }

        /// <summary>Variable field 'LAB_DOL2'</summary>
        public static VfieldTag LAB_DOL2Tag => _vfieldsByName["LAB_DOL2"][0];

        public static string LAB_DOL2
        {
            get => LAB_DOL2Tag.Value;
            set => LAB_DOL2Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_DOL_MI'</summary>
        public static VfieldTag TOL_DOL_MITag => _vfieldsByName["TOL_DOL_MI"][0];

        public static string TOL_DOL_MI
        {
            get => TOL_DOL_MITag.Value;
            set => TOL_DOL_MITag.Value = value;
        }

        /// <summary>Variable field 'TOLERMI'</summary>
        public static VfieldTag TOLERMITag => _vfieldsByName["TOLERMI"][0];

        public static string TOLERMI
        {
            get => TOLERMITag.Value;
            set => TOLERMITag.Value = value;
        }

        /// <summary>Variable field 'NAMEN'</summary>
        public static VfieldTag NAMENTag => _vfieldsByName["NAMEN"][0];

        public static string NAMEN
        {
            get => NAMENTag.Value;
            set => NAMENTag.Value = value;
        }

        /// <summary>Variable field 'ATRIBUT'</summary>
        public static VfieldTag ATRIBUTTag => _vfieldsByName["ATRIBUT"][0];

        public static string ATRIBUT
        {
            get => ATRIBUTTag.Value;
            set => ATRIBUTTag.Value = value;
        }

        /// <summary>Variable field 'LB7'</summary>
        public static VfieldTag LB7Tag => _vfieldsByName["LB7"][0];

        public static string LB7
        {
            get => LB7Tag.Value;
            set => LB7Tag.Value = value;
        }

        /// <summary>Variable field 'DIMENZOR'</summary>
        public static VfieldTag DIMENZORTag => _vfieldsByName["DIMENZOR"][0];

        public static string DIMENZOR
        {
            get => DIMENZORTag.Value;
            set => DIMENZORTag.Value = value;
        }

        /// <summary>Variable field 'LB8'</summary>
        public static VfieldTag LB8Tag => _vfieldsByName["LB8"][0];

        public static string LB8
        {
            get => LB8Tag.Value;
            set => LB8Tag.Value = value;
        }

        /// <summary>Variable field 'POSZAHT1'</summary>
        public static VfieldTag POSZAHT1Tag => _vfieldsByName["POSZAHT1"][0];

        public static string POSZAHT1
        {
            get => POSZAHT1Tag.Value;
            set => POSZAHT1Tag.Value = value;
        }

        /// <summary>Variable field 'POREKLO'</summary>
        public static VfieldTag POREKLOTag => _vfieldsByName["POREKLO"][0];

        public static string POREKLO
        {
            get => POREKLOTag.Value;
            set => POREKLOTag.Value = value;
        }

        /// <summary>Variable field 'POSZAHT2'</summary>
        public static VfieldTag POSZAHT2Tag => _vfieldsByName["POSZAHT2"][0];

        public static string POSZAHT2
        {
            get => POSZAHT2Tag.Value;
            set => POSZAHT2Tag.Value = value;
        }

        /// <summary>Variable field 'OPISVIS'</summary>
        public static VfieldTag OPISVISTag => _vfieldsByName["OPISVIS"][0];

        public static string OPISVIS
        {
            get => OPISVISTag.Value;
            set => OPISVISTag.Value = value;
        }

        /// <summary>Variable field 'LB18'</summary>
        public static VfieldTag LB18Tag => _vfieldsByName["LB18"][0];

        public static string LB18
        {
            get => LB18Tag.Value;
            set => LB18Tag.Value = value;
        }

        /// <summary>Variable field 'INDIKVI'</summary>
        public static VfieldTag INDIKVITag => _vfieldsByName["INDIKVI"][0];

        public static string INDIKVI
        {
            get => INDIKVITag.Value;
            set => INDIKVITag.Value = value;
        }

        /// <summary>Variable field 'IDENTSUR'</summary>
        public static VfieldTag IDENTSURTag => _vfieldsByName["IDENTSUR"][0];

        public static string IDENTSUR
        {
            get => IDENTSURTag.Value;
            set => IDENTSURTag.Value = value;
        }

        /// <summary>Variable field 'DIMENZVH'</summary>
        public static VfieldTag DIMENZVHTag => _vfieldsByName["DIMENZVH"][0];

        public static string DIMENZVH
        {
            get => DIMENZVHTag.Value;
            set => DIMENZVHTag.Value = value;
        }

        /// <summary>Variable field 'NAZVHOD'</summary>
        public static VfieldTag NAZVHODTag => _vfieldsByName["NAZVHOD"][0];

        public static string NAZVHOD
        {
            get => NAZVHODTag.Value;
            set => NAZVHODTag.Value = value;
        }

        /// <summary>Variable field 'STMAVHOD'</summary>
        public static VfieldTag STMAVHODTag => _vfieldsByName["STMAVHOD"][0];

        public static string STMAVHOD
        {
            get => STMAVHODTag.Value;
            set => STMAVHODTag.Value = value;
        }

        /// <summary>Variable field 'STPOVHOD'</summary>
        public static VfieldTag STPOVHODTag => _vfieldsByName["STPOVHOD"][0];

        public static string STPOVHOD
        {
            get => STPOVHODTag.Value;
            set => STPOVHODTag.Value = value;
        }

        /// <summary>Variable field 'KZ_SUR3'</summary>
        public static VfieldTag KZ_SUR3Tag => _vfieldsByName["KZ_SUR3"][0];

        public static string KZ_SUR3
        {
            get => KZ_SUR3Tag.Value;
            set => KZ_SUR3Tag.Value = value;
        }

        /// <summary>Variable field 'LB9'</summary>
        public static VfieldTag LB9Tag => _vfieldsByName["LB9"][0];

        public static string LB9
        {
            get => LB9Tag.Value;
            set => LB9Tag.Value = value;
        }

        /// <summary>Variable field 'POTSURKG'</summary>
        public static VfieldTag POTSURKGTag => _vfieldsByName["POTSURKG"][0];

        public static string POTSURKG
        {
            get => POTSURKGTag.Value;
            set => POTSURKGTag.Value = value;
        }

        /// <summary>Variable field 'LB12'</summary>
        public static VfieldTag LB12Tag => _vfieldsByName["LB12"][0];

        public static string LB12
        {
            get => LB12Tag.Value;
            set => LB12Tag.Value = value;
        }

        /// <summary>Variable field 'IZDKOL'</summary>
        public static VfieldTag IZDKOLTag => _vfieldsByName["IZDKOL"][0];

        public static string IZDKOL
        {
            get => IZDKOLTag.Value;
            set => IZDKOLTag.Value = value;
        }

        /// <summary>Variable field 'LB15'</summary>
        public static VfieldTag LB15Tag => _vfieldsByName["LB15"][0];

        public static string LB15
        {
            get => LB15Tag.Value;
            set => LB15Tag.Value = value;
        }

        /// <summary>Variable field 'MOTNJKOL'</summary>
        public static VfieldTag MOTNJKOLTag => _vfieldsByName["MOTNJKOL"][0];

        public static string MOTNJKOL
        {
            get => MOTNJKOLTag.Value;
            set => MOTNJKOLTag.Value = value;
        }

        /// <summary>Variable field 'LB10'</summary>
        public static VfieldTag LB10Tag => _vfieldsByName["LB10"][0];

        public static string LB10
        {
            get => LB10Tag.Value;
            set => LB10Tag.Value = value;
        }

        /// <summary>Variable field 'LANSIRKG'</summary>
        public static VfieldTag LANSIRKGTag => _vfieldsByName["LANSIRKG"][0];

        public static string LANSIRKG
        {
            get => LANSIRKGTag.Value;
            set => LANSIRKGTag.Value = value;
        }

        /// <summary>Variable field 'LB13'</summary>
        public static VfieldTag LB13Tag => _vfieldsByName["LB13"][0];

        public static string LB13
        {
            get => LB13Tag.Value;
            set => LB13Tag.Value = value;
        }

        /// <summary>Variable field 'ODPADKOL'</summary>
        public static VfieldTag ODPADKOLTag => _vfieldsByName["ODPADKOL"][0];

        public static string ODPADKOL
        {
            get => ODPADKOLTag.Value;
            set => ODPADKOLTag.Value = value;
        }

        /// <summary>Variable field 'LB16'</summary>
        public static VfieldTag LB16Tag => _vfieldsByName["LB16"][0];

        public static string LB16
        {
            get => LB16Tag.Value;
            set => LB16Tag.Value = value;
        }

        /// <summary>Variable field 'ODPADMOT'</summary>
        public static VfieldTag ODPADMOTTag => _vfieldsByName["ODPADMOT"][0];

        public static string ODPADMOT
        {
            get => ODPADMOTTag.Value;
            set => ODPADMOTTag.Value = value;
        }

        /// <summary>Variable field 'LB11'</summary>
        public static VfieldTag LB11Tag => _vfieldsByName["LB11"][0];

        public static string LB11
        {
            get => LB11Tag.Value;
            set => LB11Tag.Value = value;
        }

        /// <summary>Variable field 'RAZLIKG'</summary>
        public static VfieldTag RAZLIKGTag => _vfieldsByName["RAZLIKG"][0];

        public static string RAZLIKG
        {
            get => RAZLIKGTag.Value;
            set => RAZLIKGTag.Value = value;
        }

        /// <summary>Variable field 'LB14'</summary>
        public static VfieldTag LB14Tag => _vfieldsByName["LB14"][0];

        public static string LB14
        {
            get => LB14Tag.Value;
            set => LB14Tag.Value = value;
        }

        /// <summary>Variable field 'STANJEKG'</summary>
        public static VfieldTag STANJEKGTag => _vfieldsByName["STANJEKG"][0];

        public static string STANJEKG
        {
            get => STANJEKGTag.Value;
            set => STANJEKGTag.Value = value;
        }

        /// <summary>Variable field 'LB17'</summary>
        public static VfieldTag LB17Tag => _vfieldsByName["LB17"][0];

        public static string LB17
        {
            get => LB17Tag.Value;
            set => LB17Tag.Value = value;
        }

        /// <summary>Variable field 'VISEKG'</summary>
        public static VfieldTag VISEKGTag => _vfieldsByName["VISEKG"][0];

        public static string VISEKG
        {
            get => VISEKGTag.Value;
            set => VISEKGTag.Value = value;
        }

        /// <summary>Variable field 'EZEMSG'</summary>
        public static VfieldTag EZEMSGTag => _vfieldsByName["EZEMSG"][0];

        public static string EZEMSG
        {
            get => EZEMSGTag.Value;
            set => EZEMSGTag.Value = value;
        }

        /// <summary>Variable field 'LB19'</summary>
        public static VfieldTag LB19Tag => _vfieldsByName["LB19"][0];

        public static string LB19
        {
            get => LB19Tag.Value;
            set => LB19Tag.Value = value;
        }

        /// <summary>Variable field 'LB20'</summary>
        public static VfieldTag LB20Tag => _vfieldsByName["LB20"][0];

        public static string LB20
        {
            get => LB20Tag.Value;
            set => LB20Tag.Value = value;
        }

        static D133M01()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133M01-V68");
                WriteWrapped(13, 0, "");
                WriteWrapped(51, 0, "");
                WriteWrapped(53, 0, "  Planer:");
                WriteWrapped(79, 0, "");
                WriteWrapped(24, 1, "");
                WriteWrapped(38, 1, "");
                WriteWrapped(53, 1, "  St.dok:");
                WriteWrapped(65, 1, "");
                WriteWrapped(66, 1, "");
                WriteWrapped(67, 1, "Tehn.:");
                WriteWrapped(76, 1, "");
                WriteWrapped(79, 1, "");
                WriteWrapped(17, 2, "/");
                WriteWrapped(21, 2, "DNk:");
                WriteWrapped(32, 2, "DNz:");
                WriteWrapped(45, 2, "");
                WriteWrapped(47, 2, "PP:");
                WriteWrapped(55, 2, "");
                WriteWrapped(57, 2, "PF-DN:");
                WriteWrapped(70, 2, "");
                WriteWrapped(75, 2, "");
                WriteWrapped(79, 2, "");
                WriteWrapped(18, 3, "Kon.TE:");
                WriteWrapped(33, 3, "Količina:");
                WriteWrapped(52, 3, "kg");
                WriteWrapped(62, 3, "kom");
                WriteWrapped(66, 3, "");
                WriteWrapped(72, 3, "Novi R");
                WriteWrapped(79, 3, "");
                WriteWrapped(17, 4, "/");
                WriteWrapped(22, 4, "/");
                WriteWrapped(69, 4, "kg");
                WriteWrapped(79, 4, "");
                WriteWrapped(17, 5, "/");
                WriteWrapped(22, 5, "/");
                WriteWrapped(69, 5, "kg");
                WriteWrapped(79, 5, "");
                WriteWrapped(17, 6, "/");
                WriteWrapped(22, 6, "/");
                WriteWrapped(69, 6, "kg");
                WriteWrapped(79, 6, "");
                WriteWrapped(79, 7, "");
                WriteWrapped(0, 8, "Ident :");
                WriteWrapped(16, 8, "");
                WriteWrapped(18, 8, "Klasifikacija:");
                WriteWrapped(51, 8, "");
                WriteWrapped(59, 8, " Zlitina:");
                WriteWrapped(78, 8, "");
                WriteWrapped(0, 9, "Naziv :");
                WriteWrapped(30, 9, "");
                WriteWrapped(32, 9, "");
                WriteWrapped(42, 9, "KZ");
                WriteWrapped(49, 9, "/");
                WriteWrapped(58, 9, "  Stanje mat.:");
                WriteWrapped(79, 9, "");
                WriteWrapped(0, 10, "Dimenz:");
                WriteWrapped(50, 10, "");
                WriteWrapped(52, 10, "");
                WriteWrapped(60, 10, "Stanje pov.:");
                WriteWrapped(79, 10, "");
                WriteWrapped(0, 11, "Tol(+):");
                WriteWrapped(79, 11, "");
                WriteWrapped(0, 12, "Tol(-):");
                WriteWrapped(50, 12, "");
                WriteWrapped(52, 12, "");
                WriteWrapped(60, 12, "");
                WriteWrapped(61, 12, "Namen:");
                WriteWrapped(71, 12, "");
                WriteWrapped(73, 12, "");
                WriteWrapped(79, 12, "");
                WriteWrapped(59, 13, "");
                WriteWrapped(60, 13, "<--dim. od kupca");
                WriteWrapped(77, 13, "");
                WriteWrapped(79, 13, "");
                WriteWrapped(79, 14, "");
                WriteWrapped(0, 15, "Poreklo:");
                WriteWrapped(13, 15, "");
                WriteWrapped(79, 15, "");
                WriteWrapped(0, 16, "============================================================");
                WriteWrapped(79, 16, "");
                WriteWrapped(79, 17, "");
                WriteWrapped(0, 18, "Stanje mat./pov.:");
                WriteWrapped(24, 18, "/");
                WriteWrapped(79, 18, "");
                WriteWrapped(75, 19, "kg");
                WriteWrapped(78, 19, "");
                WriteWrapped(75, 20, "kg");
                WriteWrapped(78, 20, "");
                WriteWrapped(75, 21, "kg");
                WriteWrapped(78, 21, "");
                WriteWrapped(79, 22, "");
                WriteWrapped(79, 23, "");
            
                WriteWrapped(20, 0, "");
                WriteWrapped(63, 0, "");
                WriteWrapped(0, 1, "");
                WriteWrapped(11, 1, "");
                WriteWrapped(18, 1, "");
                WriteWrapped(20, 1, "");
                WriteWrapped(63, 1, "");
                WriteWrapped(74, 1, "");
                WriteWrapped(0, 2, "");
                WriteWrapped(11, 2, "");
                WriteWrapped(19, 2, "");
                WriteWrapped(26, 2, "");
                WriteWrapped(37, 2, "");
                WriteWrapped(43, 2, "");
                WriteWrapped(51, 2, "");
                WriteWrapped(64, 2, "");
                WriteWrapped(0, 3, "");
                WriteWrapped(11, 3, "");
                WriteWrapped(26, 3, "");
                WriteWrapped(43, 3, "");
                WriteWrapped(55, 3, "");
                WriteWrapped(0, 4, "");
                WriteWrapped(11, 4, "");
                WriteWrapped(19, 4, "");
                WriteWrapped(24, 4, "");
                WriteWrapped(27, 4, "");
                WriteWrapped(57, 4, "");
                WriteWrapped(72, 4, "");
                WriteWrapped(0, 5, "");
                WriteWrapped(11, 5, "");
                WriteWrapped(19, 5, "");
                WriteWrapped(24, 5, "");
                WriteWrapped(27, 5, "");
                WriteWrapped(57, 5, "");
                WriteWrapped(72, 5, "");
                WriteWrapped(0, 6, "");
                WriteWrapped(11, 6, "");
                WriteWrapped(19, 6, "");
                WriteWrapped(24, 6, "");
                WriteWrapped(27, 6, "");
                WriteWrapped(57, 6, "");
                WriteWrapped(72, 6, "");
                WriteWrapped(0, 7, "");
                WriteWrapped(13, 7, "");
                WriteWrapped(19, 7, "");
                WriteWrapped(9, 8, "");
                WriteWrapped(33, 8, "");
                WriteWrapped(36, 8, "");
                WriteWrapped(39, 8, "");
                WriteWrapped(42, 8, "");
                WriteWrapped(45, 8, "");
                WriteWrapped(48, 8, "");
                WriteWrapped(73, 8, "");
                WriteWrapped(9, 9, "");
                WriteWrapped(45, 9, "");
                WriteWrapped(51, 9, "");
                WriteWrapped(54, 9, "");
                WriteWrapped(73, 9, "");
                WriteWrapped(9, 10, "");
                WriteWrapped(73, 10, "");
                WriteWrapped(9, 11, "");
                WriteWrapped(50, 11, "");
                WriteWrapped(55, 11, "");
                WriteWrapped(62, 11, "");
                WriteWrapped(66, 11, "");
                WriteWrapped(71, 11, "");
                WriteWrapped(74, 11, "");
                WriteWrapped(9, 12, "");
                WriteWrapped(68, 12, "");
                WriteWrapped(77, 12, "");
                WriteWrapped(0, 13, "");
                WriteWrapped(18, 13, "");
                WriteWrapped(0, 14, "");
                WriteWrapped(18, 14, "");
                WriteWrapped(9, 15, "");
                WriteWrapped(18, 15, "");
                WriteWrapped(67, 16, "");
                WriteWrapped(0, 17, "");
                WriteWrapped(9, 17, "");
                WriteWrapped(11, 17, "");
                WriteWrapped(18, 17, "");
                WriteWrapped(59, 17, "");
                WriteWrapped(18, 18, "");
                WriteWrapped(26, 18, "");
                WriteWrapped(32, 18, "");
                WriteWrapped(0, 19, "");
                WriteWrapped(17, 19, "");
                WriteWrapped(26, 19, "");
                WriteWrapped(42, 19, "");
                WriteWrapped(51, 19, "");
                WriteWrapped(66, 19, "");
                WriteWrapped(0, 20, "");
                WriteWrapped(17, 20, "");
                WriteWrapped(26, 20, "");
                WriteWrapped(42, 20, "");
                WriteWrapped(51, 20, "");
                WriteWrapped(66, 20, "");
                WriteWrapped(0, 21, "");
                WriteWrapped(17, 21, "");
                WriteWrapped(26, 21, "");
                WriteWrapped(42, 21, "");
                WriteWrapped(51, 21, "");
                WriteWrapped(66, 21, "");
                WriteWrapped(0, 22, "");
                WriteWrapped(0, 23, "");
                WriteWrapped(65, 23, "");
            
                Console.SetCursorPosition(20, 0);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
    public static class D133M02
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
            new VfieldTag { Row = 1, Column = 28, Name = "LB1", Type = "CHA", Bytes = 30, Value = "" },
            new VfieldTag { Row = 2, Column = 1, Name = "LB2", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 2, Column = 7, Name = "LETODN", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 2, Column = 13, Name = "DELNAL", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 2, Column = 20, Name = "VARDELN", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 2, Column = 33, Name = "DELNALK", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 2, Column = 39, Name = "VARDELNK", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 2, Column = 46, Name = "DELNALZ", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 2, Column = 52, Name = "VARDELNZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 2, Column = 64, Name = "POREKLO", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 2, Column = 75, Name = "NAMEN", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 3, Column = 1, Name = "LB3", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 3, Column = 7, Name = "ZACETNTE", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 3, Column = 21, Name = "KONCNATE", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 3, Column = 42, Name = "DNKOLIC", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 3, Column = 54, Name = "DNKOMAD", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 4, Column = 1, Name = "LB9", Type = "CHA", Bytes = 14, Value = "" },
            new VfieldTag { Row = 4, Column = 16, Name = "IDENT", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 4, Column = 32, Name = "KZORG", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 4, Column = 38, Name = "VERKZ", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 4, Column = 41, Name = "STEVKZ", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 4, Column = 74, Name = "ZLITINA", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 5, Column = 1, Name = "LB10", Type = "CHA", Bytes = 14, Value = "" },
            new VfieldTag { Row = 5, Column = 16, Name = "NAZIZDEL", Type = "CHA", Bytes = 20, Value = "" },
            new VfieldTag { Row = 5, Column = 74, Name = "STMASIF", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 6, Column = 1, Name = "LB11", Type = "CHA", Bytes = 14, Value = "" },
            new VfieldTag { Row = 6, Column = 16, Name = "DIMENZ40", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 6, Column = 74, Name = "STPOSIF", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 7, Column = 1, Name = "LB4", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 8, Column = 5, Name = "SIGMABSP", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 8, Column = 11, Name = "SIGMABZG", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 8, Column = 25, Name = "SIG02SP", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 8, Column = 31, Name = "SIG02ZG", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 8, Column = 43, Name = "HBSPOD", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 8, Column = 49, Name = "HBZGOR", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 8, Column = 57, Name = "DELTATIP", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 8, Column = 64, Name = "DELVRSP", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 8, Column = 73, Name = "DELVRZG", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 9, Column = 7, Name = "UMAX", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 9, Column = 25, Name = "IEMIN", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 9, Column = 47, Name = "UPA", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 9, Column = 51, Name = "UPB", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 9, Column = 55, Name = "UPC", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 9, Column = 78, Name = "ATRIBUT", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 10, Column = 1, Name = "LB5", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 11, Column = 5, Name = "DATSTAT0", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 11, Column = 20, Name = "DATSTAT1", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 11, Column = 35, Name = "DATSTAT2", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 11, Column = 50, Name = "DATSTAT3", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 11, Column = 65, Name = "DATSTAT4", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 12, Column = 5, Name = "DATSTAT5", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 12, Column = 20, Name = "DATSTAT6", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 12, Column = 35, Name = "LB7", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 12, Column = 53, Name = "DATURA", Type = "CHA", Bytes = 26, Value = "" },
            new VfieldTag { Row = 13, Column = 1, Name = "LB6", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 14, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 14, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 14, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 14, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 14, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 14, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 15, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 15, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 15, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 15, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 15, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 15, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 16, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 16, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 16, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 16, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 16, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 16, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 16, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 17, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 17, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 17, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 17, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 17, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 17, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 17, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 18, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 18, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 18, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 18, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 18, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 18, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 19, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 19, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 19, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 19, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 19, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 19, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 20, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 20, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 20, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 20, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 20, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 20, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 20, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 21, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 21, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 21, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 21, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 21, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 21, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 21, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 22, Column = 2, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 22, Column = 5, Name = "ZNAK-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 22, Column = 7, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 22, Column = 12, Name = "NAZSTRKR", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 22, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 22, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 22, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 22, Column = 68, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 23, Column = 1, Name = "EZEMSG", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 24, Column = 1, Name = "LB8", Type = "CHA", Bytes = 78, Value = "" },
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        /// <summary>Variable field 'LB1'</summary>
        public static VfieldTag LB1Tag => _vfieldsByName["LB1"][0];

        public static string LB1
        {
            get => LB1Tag.Value;
            set => LB1Tag.Value = value;
        }

        /// <summary>Variable field 'LB2'</summary>
        public static VfieldTag LB2Tag => _vfieldsByName["LB2"][0];

        public static string LB2
        {
            get => LB2Tag.Value;
            set => LB2Tag.Value = value;
        }

        /// <summary>Variable field 'LETODN'</summary>
        public static VfieldTag LETODNTag => _vfieldsByName["LETODN"][0];

        public static string LETODN
        {
            get => LETODNTag.Value;
            set => LETODNTag.Value = value;
        }

        /// <summary>Variable field 'DELNAL'</summary>
        public static VfieldTag DELNALTag => _vfieldsByName["DELNAL"][0];

        public static string DELNAL
        {
            get => DELNALTag.Value;
            set => DELNALTag.Value = value;
        }

        /// <summary>Variable field 'VARDELN'</summary>
        public static VfieldTag VARDELNTag => _vfieldsByName["VARDELN"][0];

        public static string VARDELN
        {
            get => VARDELNTag.Value;
            set => VARDELNTag.Value = value;
        }

        /// <summary>Variable field 'DELNALK'</summary>
        public static VfieldTag DELNALKTag => _vfieldsByName["DELNALK"][0];

        public static string DELNALK
        {
            get => DELNALKTag.Value;
            set => DELNALKTag.Value = value;
        }

        /// <summary>Variable field 'VARDELNK'</summary>
        public static VfieldTag VARDELNKTag => _vfieldsByName["VARDELNK"][0];

        public static string VARDELNK
        {
            get => VARDELNKTag.Value;
            set => VARDELNKTag.Value = value;
        }

        /// <summary>Variable field 'DELNALZ'</summary>
        public static VfieldTag DELNALZTag => _vfieldsByName["DELNALZ"][0];

        public static string DELNALZ
        {
            get => DELNALZTag.Value;
            set => DELNALZTag.Value = value;
        }

        /// <summary>Variable field 'VARDELNZ'</summary>
        public static VfieldTag VARDELNZTag => _vfieldsByName["VARDELNZ"][0];

        public static string VARDELNZ
        {
            get => VARDELNZTag.Value;
            set => VARDELNZTag.Value = value;
        }

        /// <summary>Variable field 'POREKLO'</summary>
        public static VfieldTag POREKLOTag => _vfieldsByName["POREKLO"][0];

        public static string POREKLO
        {
            get => POREKLOTag.Value;
            set => POREKLOTag.Value = value;
        }

        /// <summary>Variable field 'NAMEN'</summary>
        public static VfieldTag NAMENTag => _vfieldsByName["NAMEN"][0];

        public static string NAMEN
        {
            get => NAMENTag.Value;
            set => NAMENTag.Value = value;
        }

        /// <summary>Variable field 'LB3'</summary>
        public static VfieldTag LB3Tag => _vfieldsByName["LB3"][0];

        public static string LB3
        {
            get => LB3Tag.Value;
            set => LB3Tag.Value = value;
        }

        /// <summary>Variable field 'ZACETNTE'</summary>
        public static VfieldTag ZACETNTETag => _vfieldsByName["ZACETNTE"][0];

        public static string ZACETNTE
        {
            get => ZACETNTETag.Value;
            set => ZACETNTETag.Value = value;
        }

        /// <summary>Variable field 'KONCNATE'</summary>
        public static VfieldTag KONCNATETag => _vfieldsByName["KONCNATE"][0];

        public static string KONCNATE
        {
            get => KONCNATETag.Value;
            set => KONCNATETag.Value = value;
        }

        /// <summary>Variable field 'DNKOLIC'</summary>
        public static VfieldTag DNKOLICTag => _vfieldsByName["DNKOLIC"][0];

        public static string DNKOLIC
        {
            get => DNKOLICTag.Value;
            set => DNKOLICTag.Value = value;
        }

        /// <summary>Variable field 'DNKOMAD'</summary>
        public static VfieldTag DNKOMADTag => _vfieldsByName["DNKOMAD"][0];

        public static string DNKOMAD
        {
            get => DNKOMADTag.Value;
            set => DNKOMADTag.Value = value;
        }

        /// <summary>Variable field 'LB9'</summary>
        public static VfieldTag LB9Tag => _vfieldsByName["LB9"][0];

        public static string LB9
        {
            get => LB9Tag.Value;
            set => LB9Tag.Value = value;
        }

        /// <summary>Variable field 'IDENT'</summary>
        public static VfieldTag IDENTTag => _vfieldsByName["IDENT"][0];

        public static string IDENT
        {
            get => IDENTTag.Value;
            set => IDENTTag.Value = value;
        }

        /// <summary>Variable field 'KZORG'</summary>
        public static VfieldTag KZORGTag => _vfieldsByName["KZORG"][0];

        public static string KZORG
        {
            get => KZORGTag.Value;
            set => KZORGTag.Value = value;
        }

        /// <summary>Variable field 'VERKZ'</summary>
        public static VfieldTag VERKZTag => _vfieldsByName["VERKZ"][0];

        public static string VERKZ
        {
            get => VERKZTag.Value;
            set => VERKZTag.Value = value;
        }

        /// <summary>Variable field 'STEVKZ'</summary>
        public static VfieldTag STEVKZTag => _vfieldsByName["STEVKZ"][0];

        public static string STEVKZ
        {
            get => STEVKZTag.Value;
            set => STEVKZTag.Value = value;
        }

        /// <summary>Variable field 'ZLITINA'</summary>
        public static VfieldTag ZLITINATag => _vfieldsByName["ZLITINA"][0];

        public static string ZLITINA
        {
            get => ZLITINATag.Value;
            set => ZLITINATag.Value = value;
        }

        /// <summary>Variable field 'LB10'</summary>
        public static VfieldTag LB10Tag => _vfieldsByName["LB10"][0];

        public static string LB10
        {
            get => LB10Tag.Value;
            set => LB10Tag.Value = value;
        }

        /// <summary>Variable field 'NAZIZDEL'</summary>
        public static VfieldTag NAZIZDELTag => _vfieldsByName["NAZIZDEL"][0];

        public static string NAZIZDEL
        {
            get => NAZIZDELTag.Value;
            set => NAZIZDELTag.Value = value;
        }

        /// <summary>Variable field 'STMASIF'</summary>
        public static VfieldTag STMASIFTag => _vfieldsByName["STMASIF"][0];

        public static string STMASIF
        {
            get => STMASIFTag.Value;
            set => STMASIFTag.Value = value;
        }

        /// <summary>Variable field 'LB11'</summary>
        public static VfieldTag LB11Tag => _vfieldsByName["LB11"][0];

        public static string LB11
        {
            get => LB11Tag.Value;
            set => LB11Tag.Value = value;
        }

        /// <summary>Variable field 'DIMENZ40'</summary>
        public static VfieldTag DIMENZ40Tag => _vfieldsByName["DIMENZ40"][0];

        public static string DIMENZ40
        {
            get => DIMENZ40Tag.Value;
            set => DIMENZ40Tag.Value = value;
        }

        /// <summary>Variable field 'STPOSIF'</summary>
        public static VfieldTag STPOSIFTag => _vfieldsByName["STPOSIF"][0];

        public static string STPOSIF
        {
            get => STPOSIFTag.Value;
            set => STPOSIFTag.Value = value;
        }

        /// <summary>Variable field 'LB4'</summary>
        public static VfieldTag LB4Tag => _vfieldsByName["LB4"][0];

        public static string LB4
        {
            get => LB4Tag.Value;
            set => LB4Tag.Value = value;
        }

        /// <summary>Variable field 'SIGMABSP'</summary>
        public static VfieldTag SIGMABSPTag => _vfieldsByName["SIGMABSP"][0];

        public static string SIGMABSP
        {
            get => SIGMABSPTag.Value;
            set => SIGMABSPTag.Value = value;
        }

        /// <summary>Variable field 'SIGMABZG'</summary>
        public static VfieldTag SIGMABZGTag => _vfieldsByName["SIGMABZG"][0];

        public static string SIGMABZG
        {
            get => SIGMABZGTag.Value;
            set => SIGMABZGTag.Value = value;
        }

        /// <summary>Variable field 'SIG02SP'</summary>
        public static VfieldTag SIG02SPTag => _vfieldsByName["SIG02SP"][0];

        public static string SIG02SP
        {
            get => SIG02SPTag.Value;
            set => SIG02SPTag.Value = value;
        }

        /// <summary>Variable field 'SIG02ZG'</summary>
        public static VfieldTag SIG02ZGTag => _vfieldsByName["SIG02ZG"][0];

        public static string SIG02ZG
        {
            get => SIG02ZGTag.Value;
            set => SIG02ZGTag.Value = value;
        }

        /// <summary>Variable field 'HBSPOD'</summary>
        public static VfieldTag HBSPODTag => _vfieldsByName["HBSPOD"][0];

        public static string HBSPOD
        {
            get => HBSPODTag.Value;
            set => HBSPODTag.Value = value;
        }

        /// <summary>Variable field 'HBZGOR'</summary>
        public static VfieldTag HBZGORTag => _vfieldsByName["HBZGOR"][0];

        public static string HBZGOR
        {
            get => HBZGORTag.Value;
            set => HBZGORTag.Value = value;
        }

        /// <summary>Variable field 'DELTATIP'</summary>
        public static VfieldTag DELTATIPTag => _vfieldsByName["DELTATIP"][0];

        public static string DELTATIP
        {
            get => DELTATIPTag.Value;
            set => DELTATIPTag.Value = value;
        }

        /// <summary>Variable field 'DELVRSP'</summary>
        public static VfieldTag DELVRSPTag => _vfieldsByName["DELVRSP"][0];

        public static string DELVRSP
        {
            get => DELVRSPTag.Value;
            set => DELVRSPTag.Value = value;
        }

        /// <summary>Variable field 'DELVRZG'</summary>
        public static VfieldTag DELVRZGTag => _vfieldsByName["DELVRZG"][0];

        public static string DELVRZG
        {
            get => DELVRZGTag.Value;
            set => DELVRZGTag.Value = value;
        }

        /// <summary>Variable field 'UMAX'</summary>
        public static VfieldTag UMAXTag => _vfieldsByName["UMAX"][0];

        public static string UMAX
        {
            get => UMAXTag.Value;
            set => UMAXTag.Value = value;
        }

        /// <summary>Variable field 'IEMIN'</summary>
        public static VfieldTag IEMINTag => _vfieldsByName["IEMIN"][0];

        public static string IEMIN
        {
            get => IEMINTag.Value;
            set => IEMINTag.Value = value;
        }

        /// <summary>Variable field 'UPA'</summary>
        public static VfieldTag UPATag => _vfieldsByName["UPA"][0];

        public static string UPA
        {
            get => UPATag.Value;
            set => UPATag.Value = value;
        }

        /// <summary>Variable field 'UPB'</summary>
        public static VfieldTag UPBTag => _vfieldsByName["UPB"][0];

        public static string UPB
        {
            get => UPBTag.Value;
            set => UPBTag.Value = value;
        }

        /// <summary>Variable field 'UPC'</summary>
        public static VfieldTag UPCTag => _vfieldsByName["UPC"][0];

        public static string UPC
        {
            get => UPCTag.Value;
            set => UPCTag.Value = value;
        }

        /// <summary>Variable field 'ATRIBUT'</summary>
        public static VfieldTag ATRIBUTTag => _vfieldsByName["ATRIBUT"][0];

        public static string ATRIBUT
        {
            get => ATRIBUTTag.Value;
            set => ATRIBUTTag.Value = value;
        }

        /// <summary>Variable field 'LB5'</summary>
        public static VfieldTag LB5Tag => _vfieldsByName["LB5"][0];

        public static string LB5
        {
            get => LB5Tag.Value;
            set => LB5Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT0'</summary>
        public static VfieldTag DATSTAT0Tag => _vfieldsByName["DATSTAT0"][0];

        public static string DATSTAT0
        {
            get => DATSTAT0Tag.Value;
            set => DATSTAT0Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT1'</summary>
        public static VfieldTag DATSTAT1Tag => _vfieldsByName["DATSTAT1"][0];

        public static string DATSTAT1
        {
            get => DATSTAT1Tag.Value;
            set => DATSTAT1Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT2'</summary>
        public static VfieldTag DATSTAT2Tag => _vfieldsByName["DATSTAT2"][0];

        public static string DATSTAT2
        {
            get => DATSTAT2Tag.Value;
            set => DATSTAT2Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT3'</summary>
        public static VfieldTag DATSTAT3Tag => _vfieldsByName["DATSTAT3"][0];

        public static string DATSTAT3
        {
            get => DATSTAT3Tag.Value;
            set => DATSTAT3Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT4'</summary>
        public static VfieldTag DATSTAT4Tag => _vfieldsByName["DATSTAT4"][0];

        public static string DATSTAT4
        {
            get => DATSTAT4Tag.Value;
            set => DATSTAT4Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT5'</summary>
        public static VfieldTag DATSTAT5Tag => _vfieldsByName["DATSTAT5"][0];

        public static string DATSTAT5
        {
            get => DATSTAT5Tag.Value;
            set => DATSTAT5Tag.Value = value;
        }

        /// <summary>Variable field 'DATSTAT6'</summary>
        public static VfieldTag DATSTAT6Tag => _vfieldsByName["DATSTAT6"][0];

        public static string DATSTAT6
        {
            get => DATSTAT6Tag.Value;
            set => DATSTAT6Tag.Value = value;
        }

        /// <summary>Variable field 'LB7'</summary>
        public static VfieldTag LB7Tag => _vfieldsByName["LB7"][0];

        public static string LB7
        {
            get => LB7Tag.Value;
            set => LB7Tag.Value = value;
        }

        /// <summary>Variable field 'DATURA'</summary>
        public static VfieldTag DATURATag => _vfieldsByName["DATURA"][0];

        public static string DATURA
        {
            get => DATURATag.Value;
            set => DATURATag.Value = value;
        }

        /// <summary>Variable field 'LB6'</summary>
        public static VfieldTag LB6Tag => _vfieldsByName["LB6"][0];

        public static string LB6
        {
            get => LB6Tag.Value;
            set => LB6Tag.Value = value;
        }

        /// <summary>Variable field 'STEVILOP' (multiple)</summary>
        public static string[] STEVILOP
        {
            get
            {
                return _vfieldsByName["STEVILOP"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'ZNAK-KZ' (multiple)</summary>
        public static string[] ZNAK_KZ
        {
            get
            {
                return _vfieldsByName["ZNAK-KZ"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'STEVSTRO' (multiple)</summary>
        public static string[] STEVSTRO
        {
            get
            {
                return _vfieldsByName["STEVSTRO"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'NAZSTRKR' (multiple)</summary>
        public static string[] NAZSTRKR
        {
            get
            {
                return _vfieldsByName["NAZSTRKR"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'IZDKOL' (multiple)</summary>
        public static string[] IZDKOL
        {
            get
            {
                return _vfieldsByName["IZDKOL"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'IZDKOM' (multiple)</summary>
        public static string[] IZDKOM
        {
            get
            {
                return _vfieldsByName["IZDKOM"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'DELVNOS' (multiple)</summary>
        public static string[] DELVNOS
        {
            get
            {
                return _vfieldsByName["DELVNOS"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'DATSPRE' (multiple)</summary>
        public static string[] DATSPRE
        {
            get
            {
                return _vfieldsByName["DATSPRE"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'EZEMSG'</summary>
        public static VfieldTag EZEMSGTag => _vfieldsByName["EZEMSG"][0];

        public static string EZEMSG
        {
            get => EZEMSGTag.Value;
            set => EZEMSGTag.Value = value;
        }

        /// <summary>Variable field 'LB8'</summary>
        public static VfieldTag LB8Tag => _vfieldsByName["LB8"][0];

        public static string LB8
        {
            get => LB8Tag.Value;
            set => LB8Tag.Value = value;
        }

        static D133M02()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133M02");
                WriteWrapped(8, 0, "");
                WriteWrapped(58, 0, "");
                WriteWrapped(11, 1, "");
                WriteWrapped(18, 1, "");
                WriteWrapped(21, 1, "");
                WriteWrapped(27, 1, "DNk:");
                WriteWrapped(40, 1, "DNz:");
                WriteWrapped(53, 1, "");
                WriteWrapped(54, 1, "Poreklo:");
                WriteWrapped(67, 1, "Namen:");
                WriteWrapped(77, 1, "");
                WriteWrapped(78, 1, "");
                WriteWrapped(13, 2, "");
                WriteWrapped(14, 2, "KoTE:");
                WriteWrapped(27, 2, "");
                WriteWrapped(29, 2, "  Količina:");
                WriteWrapped(50, 2, "kg");
                WriteWrapped(60, 2, "kom");
                WriteWrapped(65, 2, "");
                WriteWrapped(22, 3, "");
                WriteWrapped(25, 3, "");
                WriteWrapped(27, 3, "KZ:");
                WriteWrapped(35, 3, "/");
                WriteWrapped(44, 3, "");
                WriteWrapped(58, 3, "");
                WriteWrapped(59, 3, " Zlitina    :");
                WriteWrapped(78, 3, "");
                WriteWrapped(36, 4, "");
                WriteWrapped(39, 4, "");
                WriteWrapped(58, 4, "");
                WriteWrapped(59, 4, " Stanje mat.:");
                WriteWrapped(79, 4, "");
                WriteWrapped(56, 5, "");
                WriteWrapped(59, 5, " Stanje pov.:");
                WriteWrapped(79, 5, "");
                WriteWrapped(79, 6, "");
                WriteWrapped(0, 7, "Rm:");
                WriteWrapped(8, 7, "-");
                WriteWrapped(14, 7, "   Rp 02:");
                WriteWrapped(28, 7, "-");
                WriteWrapped(34, 7, "    HB:");
                WriteWrapped(46, 7, "-");
                WriteWrapped(52, 7, " A:");
                WriteWrapped(61, 7, ":");
                WriteWrapped(70, 7, "-");
                WriteWrapped(79, 7, "");
                WriteWrapped(0, 8, "Umax:");
                WriteWrapped(12, 8, "");
                WriteWrapped(17, 8, "IEmin:");
                WriteWrapped(30, 8, "");
                WriteWrapped(38, 8, "Upogib:");
                WriteWrapped(58, 8, "");
                WriteWrapped(67, 8, "");
                WriteWrapped(79, 8, "");
                WriteWrapped(79, 9, "");
                WriteWrapped(1, 10, "0:");
                WriteWrapped(15, 10, "");
                WriteWrapped(16, 10, "1:");
                WriteWrapped(30, 10, "");
                WriteWrapped(31, 10, "2:");
                WriteWrapped(45, 10, "");
                WriteWrapped(46, 10, "3:");
                WriteWrapped(60, 10, "");
                WriteWrapped(61, 10, "4:");
                WriteWrapped(75, 10, "");
                WriteWrapped(79, 10, "");
                WriteWrapped(1, 11, "5:");
                WriteWrapped(15, 11, "");
                WriteWrapped(16, 11, "6:");
                WriteWrapped(30, 11, "");
                WriteWrapped(79, 11, "");
                WriteWrapped(79, 12, "");
                WriteWrapped(66, 13, "");
                WriteWrapped(78, 13, "");
                WriteWrapped(79, 13, "");
                WriteWrapped(66, 14, "");
                WriteWrapped(78, 14, "");
                WriteWrapped(79, 14, "");
                WriteWrapped(66, 15, "");
                WriteWrapped(78, 15, "");
                WriteWrapped(79, 15, "");
                WriteWrapped(66, 16, "");
                WriteWrapped(78, 16, "");
                WriteWrapped(79, 16, "");
                WriteWrapped(66, 17, "");
                WriteWrapped(78, 17, "");
                WriteWrapped(79, 17, "");
                WriteWrapped(66, 18, "");
                WriteWrapped(78, 18, "");
                WriteWrapped(66, 19, "");
                WriteWrapped(78, 19, "");
                WriteWrapped(66, 20, "");
                WriteWrapped(78, 20, "");
                WriteWrapped(66, 21, "");
                WriteWrapped(78, 21, "");
                WriteWrapped(79, 22, "");
                WriteWrapped(79, 23, "");
            
                WriteWrapped(27, 0, "");
                WriteWrapped(0, 1, "");
                WriteWrapped(6, 1, "");
                WriteWrapped(12, 1, "");
                WriteWrapped(19, 1, "");
                WriteWrapped(32, 1, "");
                WriteWrapped(38, 1, "");
                WriteWrapped(45, 1, "");
                WriteWrapped(51, 1, "");
                WriteWrapped(63, 1, "");
                WriteWrapped(74, 1, "");
                WriteWrapped(0, 2, "");
                WriteWrapped(6, 2, "");
                WriteWrapped(20, 2, "");
                WriteWrapped(41, 2, "");
                WriteWrapped(53, 2, "");
                WriteWrapped(0, 3, "");
                WriteWrapped(15, 3, "");
                WriteWrapped(31, 3, "");
                WriteWrapped(37, 3, "");
                WriteWrapped(40, 3, "");
                WriteWrapped(73, 3, "");
                WriteWrapped(0, 4, "");
                WriteWrapped(15, 4, "");
                WriteWrapped(73, 4, "");
                WriteWrapped(0, 5, "");
                WriteWrapped(15, 5, "");
                WriteWrapped(73, 5, "");
                WriteWrapped(0, 6, "");
                WriteWrapped(4, 7, "");
                WriteWrapped(10, 7, "");
                WriteWrapped(24, 7, "");
                WriteWrapped(30, 7, "");
                WriteWrapped(42, 7, "");
                WriteWrapped(48, 7, "");
                WriteWrapped(56, 7, "");
                WriteWrapped(63, 7, "");
                WriteWrapped(72, 7, "");
                WriteWrapped(6, 8, "");
                WriteWrapped(24, 8, "");
                WriteWrapped(46, 8, "");
                WriteWrapped(50, 8, "");
                WriteWrapped(54, 8, "");
                WriteWrapped(77, 8, "");
                WriteWrapped(0, 9, "");
                WriteWrapped(4, 10, "");
                WriteWrapped(19, 10, "");
                WriteWrapped(34, 10, "");
                WriteWrapped(49, 10, "");
                WriteWrapped(64, 10, "");
                WriteWrapped(4, 11, "");
                WriteWrapped(19, 11, "");
                WriteWrapped(34, 11, "");
                WriteWrapped(52, 11, "");
                WriteWrapped(0, 12, "");
                WriteWrapped(1, 13, "");
                WriteWrapped(4, 13, "");
                WriteWrapped(6, 13, "");
                WriteWrapped(11, 13, "");
                WriteWrapped(31, 13, "");
                WriteWrapped(41, 13, "");
                WriteWrapped(47, 13, "");
                WriteWrapped(67, 13, "");
                WriteWrapped(1, 14, "");
                WriteWrapped(4, 14, "");
                WriteWrapped(6, 14, "");
                WriteWrapped(11, 14, "");
                WriteWrapped(31, 14, "");
                WriteWrapped(41, 14, "");
                WriteWrapped(47, 14, "");
                WriteWrapped(67, 14, "");
                WriteWrapped(1, 15, "");
                WriteWrapped(4, 15, "");
                WriteWrapped(6, 15, "");
                WriteWrapped(11, 15, "");
                WriteWrapped(31, 15, "");
                WriteWrapped(41, 15, "");
                WriteWrapped(47, 15, "");
                WriteWrapped(67, 15, "");
                WriteWrapped(1, 16, "");
                WriteWrapped(4, 16, "");
                WriteWrapped(6, 16, "");
                WriteWrapped(11, 16, "");
                WriteWrapped(31, 16, "");
                WriteWrapped(41, 16, "");
                WriteWrapped(47, 16, "");
                WriteWrapped(67, 16, "");
                WriteWrapped(1, 17, "");
                WriteWrapped(4, 17, "");
                WriteWrapped(6, 17, "");
                WriteWrapped(11, 17, "");
                WriteWrapped(31, 17, "");
                WriteWrapped(41, 17, "");
                WriteWrapped(47, 17, "");
                WriteWrapped(67, 17, "");
                WriteWrapped(1, 18, "");
                WriteWrapped(4, 18, "");
                WriteWrapped(6, 18, "");
                WriteWrapped(11, 18, "");
                WriteWrapped(31, 18, "");
                WriteWrapped(41, 18, "");
                WriteWrapped(47, 18, "");
                WriteWrapped(67, 18, "");
                WriteWrapped(1, 19, "");
                WriteWrapped(4, 19, "");
                WriteWrapped(6, 19, "");
                WriteWrapped(11, 19, "");
                WriteWrapped(31, 19, "");
                WriteWrapped(41, 19, "");
                WriteWrapped(47, 19, "");
                WriteWrapped(67, 19, "");
                WriteWrapped(1, 20, "");
                WriteWrapped(4, 20, "");
                WriteWrapped(6, 20, "");
                WriteWrapped(11, 20, "");
                WriteWrapped(31, 20, "");
                WriteWrapped(41, 20, "");
                WriteWrapped(47, 20, "");
                WriteWrapped(67, 20, "");
                WriteWrapped(1, 21, "");
                WriteWrapped(4, 21, "");
                WriteWrapped(6, 21, "");
                WriteWrapped(11, 21, "");
                WriteWrapped(31, 21, "");
                WriteWrapped(41, 21, "");
                WriteWrapped(47, 21, "");
                WriteWrapped(67, 21, "");
                WriteWrapped(0, 22, "");
                WriteWrapped(0, 23, "");
            
                Console.SetCursorPosition(27, 0);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
    public static class D133M03
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
            new VfieldTag { Row = 4, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 4, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 4, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 5, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 5, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 5, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 6, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 6, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 6, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 7, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 7, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 7, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 8, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 8, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 8, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 9, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 9, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 10, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 10, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 10, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 11, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 11, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 11, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 12, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 12, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 12, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 13, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 13, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 14, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 14, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 15, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 15, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 16, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 16, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 17, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 17, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 18, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 18, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 19, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 19, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 20, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 20, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 21, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 21, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 22, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 22, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 22, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 23, Column = 1, Name = "SIFNAP", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 23, Column = 4, Name = "OPISMOT", Type = "CHA", Bytes = 64, Value = "" },
            new VfieldTag { Row = 23, Column = 69, Name = "DATUMPR", Type = "CHA", Bytes = 10, Value = "" },
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        /// <summary>Variable field 'SIFNAP' (multiple)</summary>
        public static string[] SIFNAP
        {
            get
            {
                return _vfieldsByName["SIFNAP"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'OPISMOT' (multiple)</summary>
        public static string[] OPISMOT
        {
            get
            {
                return _vfieldsByName["OPISMOT"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'DATUMPR' (multiple)</summary>
        public static string[] DATUMPR
        {
            get
            {
                return _vfieldsByName["DATUMPR"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        static D133M03()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133M03");
                WriteWrapped(8, 0, "");
                WriteWrapped(18, 0, "Prikaz motenj tega DN po zaporedju prijave");
                WriteWrapped(61, 0, "");
                WriteWrapped(68, 0, "");
                WriteWrapped(0, 2, "Šifra");
                WriteWrapped(6, 2, "Opis motnje");
                WriteWrapped(18, 2, "");
                WriteWrapped(68, 2, "Datum pr.");
                WriteWrapped(78, 2, "");
                WriteWrapped(79, 3, "");
                WriteWrapped(79, 4, "");
                WriteWrapped(79, 5, "");
                WriteWrapped(79, 6, "");
                WriteWrapped(79, 7, "");
                WriteWrapped(79, 8, "");
                WriteWrapped(79, 9, "");
                WriteWrapped(79, 10, "");
                WriteWrapped(79, 11, "");
                WriteWrapped(79, 12, "");
                WriteWrapped(79, 13, "");
                WriteWrapped(79, 14, "");
                WriteWrapped(79, 15, "");
                WriteWrapped(79, 16, "");
                WriteWrapped(79, 17, "");
                WriteWrapped(79, 18, "");
                WriteWrapped(79, 19, "");
                WriteWrapped(79, 20, "");
                WriteWrapped(79, 21, "");
                WriteWrapped(79, 22, "");
                WriteWrapped(0, 23, "pritisni katerokoli tipko za povratek");
                WriteWrapped(38, 23, "");
            
                WriteWrapped(0, 3, "");
                WriteWrapped(3, 3, "");
                WriteWrapped(68, 3, "");
                WriteWrapped(0, 4, "");
                WriteWrapped(3, 4, "");
                WriteWrapped(68, 4, "");
                WriteWrapped(0, 5, "");
                WriteWrapped(3, 5, "");
                WriteWrapped(68, 5, "");
                WriteWrapped(0, 6, "");
                WriteWrapped(3, 6, "");
                WriteWrapped(68, 6, "");
                WriteWrapped(0, 7, "");
                WriteWrapped(3, 7, "");
                WriteWrapped(68, 7, "");
                WriteWrapped(0, 8, "");
                WriteWrapped(3, 8, "");
                WriteWrapped(68, 8, "");
                WriteWrapped(0, 9, "");
                WriteWrapped(3, 9, "");
                WriteWrapped(68, 9, "");
                WriteWrapped(0, 10, "");
                WriteWrapped(3, 10, "");
                WriteWrapped(68, 10, "");
                WriteWrapped(0, 11, "");
                WriteWrapped(3, 11, "");
                WriteWrapped(68, 11, "");
                WriteWrapped(0, 12, "");
                WriteWrapped(3, 12, "");
                WriteWrapped(68, 12, "");
                WriteWrapped(0, 13, "");
                WriteWrapped(3, 13, "");
                WriteWrapped(68, 13, "");
                WriteWrapped(0, 14, "");
                WriteWrapped(3, 14, "");
                WriteWrapped(68, 14, "");
                WriteWrapped(0, 15, "");
                WriteWrapped(3, 15, "");
                WriteWrapped(68, 15, "");
                WriteWrapped(0, 16, "");
                WriteWrapped(3, 16, "");
                WriteWrapped(68, 16, "");
                WriteWrapped(0, 17, "");
                WriteWrapped(3, 17, "");
                WriteWrapped(68, 17, "");
                WriteWrapped(0, 18, "");
                WriteWrapped(3, 18, "");
                WriteWrapped(68, 18, "");
                WriteWrapped(0, 19, "");
                WriteWrapped(3, 19, "");
                WriteWrapped(68, 19, "");
                WriteWrapped(0, 20, "");
                WriteWrapped(3, 20, "");
                WriteWrapped(68, 20, "");
                WriteWrapped(0, 21, "");
                WriteWrapped(3, 21, "");
                WriteWrapped(68, 21, "");
                WriteWrapped(0, 22, "");
                WriteWrapped(3, 22, "");
                WriteWrapped(68, 22, "");
            
                Console.SetCursorPosition(0, 3);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
    public static class D133M04
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
            new VfieldTag { Row = 1, Column = 21, Name = "LB1", Type = "CHA", Bytes = 47, Value = "" },
            new VfieldTag { Row = 4, Column = 1, Name = "LB2", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 4, Column = 19, Name = "DELNAL", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 4, Column = 25, Name = "VARDELN", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 4, Column = 34, Name = "DELNALK", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 4, Column = 40, Name = "VARDELNK", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 4, Column = 47, Name = "DELNALZ", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 4, Column = 53, Name = "VARDELNZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 4, Column = 61, Name = "KZORG", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 4, Column = 67, Name = "VERKZ", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 4, Column = 70, Name = "STEVKZ", Type = "NUM", Bytes = 3, Value = "" },
            new VfieldTag { Row = 5, Column = 1, Name = "LB3", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 5, Column = 19, Name = "DNKOLIC", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 5, Column = 31, Name = "DNKOMAD", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 5, Column = 43, Name = "LB7", Type = "CHA", Bytes = 11, Value = "" },
            new VfieldTag { Row = 5, Column = 55, Name = "ZACETNTE", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 5, Column = 62, Name = "LB8", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 5, Column = 73, Name = "KONCNATE", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 6, Column = 1, Name = "LB4", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 6, Column = 19, Name = "IDENT", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 6, Column = 37, Name = "POREKLO", Type = "CHA", Bytes = 3, Value = "" },
            new VfieldTag { Row = 6, Column = 48, Name = "NAMEN", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 6, Column = 74, Name = "ZLITINA", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 7, Column = 1, Name = "LB5", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 7, Column = 19, Name = "NAZIZDEL", Type = "CHA", Bytes = 20, Value = "" },
            new VfieldTag { Row = 7, Column = 74, Name = "STMASIF", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 8, Column = 1, Name = "LB6", Type = "CHA", Bytes = 17, Value = "" },
            new VfieldTag { Row = 8, Column = 19, Name = "DIMENZ40", Type = "CHA", Bytes = 40, Value = "" },
            new VfieldTag { Row = 8, Column = 74, Name = "STPOSIF", Type = "CHA", Bytes = 5, Value = "" },
            new VfieldTag { Row = 9, Column = 1, Name = "LB9", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 10, Column = 3, Name = "STEVILOP", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 10, Column = 6, Name = "STEVSTRO", Type = "CHA", Bytes = 4, Value = "" },
            new VfieldTag { Row = 10, Column = 11, Name = "NAZSTRKR", Type = "CHA", Bytes = 20, Value = "" },
            new VfieldTag { Row = 10, Column = 32, Name = "IZDKOL", Type = "NUM", Bytes = 9, Value = "" },
            new VfieldTag { Row = 10, Column = 42, Name = "IZDKOM", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 10, Column = 48, Name = "DELVNOS", Type = "CHA", Bytes = 20, Value = "" },
            new VfieldTag { Row = 10, Column = 69, Name = "DATSPRE", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 11, Column = 1, Name = "LB10", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 13, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 13, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 13, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 13, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 13, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 13, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 13, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 13, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 13, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 13, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 13, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 14, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 14, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 14, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 14, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 14, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 14, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 14, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 15, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 15, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 15, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 15, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 15, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 15, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 16, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 16, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 16, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 16, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 16, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 16, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 16, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 16, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 16, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 16, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 16, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 17, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 17, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 17, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 17, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 17, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 17, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 17, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 17, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 17, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 17, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 17, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 18, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 18, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 18, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 18, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 18, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 18, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 18, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 19, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 19, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 19, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 19, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 19, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 19, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 19, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 19, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 19, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 20, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 20, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 20, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 20, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 20, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 20, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 20, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 20, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 20, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 20, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 20, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 21, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 21, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 21, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 21, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 21, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 21, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 21, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 21, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 21, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 21, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 21, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 22, Column = 1, Name = "TRAN_ENOTA", Type = "NUM", Bytes = 8, Value = "" },
            new VfieldTag { Row = 22, Column = 10, Name = "SARZA", Type = "CHA", Bytes = 18, Value = "" },
            new VfieldTag { Row = 22, Column = 29, Name = "IDSARZA", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 22, Column = 36, Name = "IZDKOL-TRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 22, Column = 42, Name = "IZDKOMTRE", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 22, Column = 48, Name = "VZOREC", Type = "CHA", Bytes = 2, Value = "" },
            new VfieldTag { Row = 22, Column = 51, Name = "USTREZA", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 22, Column = 53, Name = "USTREZA-KZ", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 22, Column = 55, Name = "KOM", Type = "CHA", Bytes = 12, Value = "" },
            new VfieldTag { Row = 22, Column = 68, Name = "STATSUD", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 22, Column = 70, Name = "L1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 22, Column = 72, Name = "L2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 22, Column = 75, Name = "L3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 22, Column = 78, Name = "L4", Type = "NUM", Bytes = 1, Value = "" },
            new VfieldTag { Row = 23, Column = 1, Name = "EZEMSG", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 24, Column = 1, Name = "LB11", Type = "CHA", Bytes = 78, Value = "" },
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        /// <summary>Variable field 'LB1'</summary>
        public static VfieldTag LB1Tag => _vfieldsByName["LB1"][0];

        public static string LB1
        {
            get => LB1Tag.Value;
            set => LB1Tag.Value = value;
        }

        /// <summary>Variable field 'LB2'</summary>
        public static VfieldTag LB2Tag => _vfieldsByName["LB2"][0];

        public static string LB2
        {
            get => LB2Tag.Value;
            set => LB2Tag.Value = value;
        }

        /// <summary>Variable field 'DELNAL'</summary>
        public static VfieldTag DELNALTag => _vfieldsByName["DELNAL"][0];

        public static string DELNAL
        {
            get => DELNALTag.Value;
            set => DELNALTag.Value = value;
        }

        /// <summary>Variable field 'VARDELN'</summary>
        public static VfieldTag VARDELNTag => _vfieldsByName["VARDELN"][0];

        public static string VARDELN
        {
            get => VARDELNTag.Value;
            set => VARDELNTag.Value = value;
        }

        /// <summary>Variable field 'DELNALK'</summary>
        public static VfieldTag DELNALKTag => _vfieldsByName["DELNALK"][0];

        public static string DELNALK
        {
            get => DELNALKTag.Value;
            set => DELNALKTag.Value = value;
        }

        /// <summary>Variable field 'VARDELNK'</summary>
        public static VfieldTag VARDELNKTag => _vfieldsByName["VARDELNK"][0];

        public static string VARDELNK
        {
            get => VARDELNKTag.Value;
            set => VARDELNKTag.Value = value;
        }

        /// <summary>Variable field 'DELNALZ'</summary>
        public static VfieldTag DELNALZTag => _vfieldsByName["DELNALZ"][0];

        public static string DELNALZ
        {
            get => DELNALZTag.Value;
            set => DELNALZTag.Value = value;
        }

        /// <summary>Variable field 'VARDELNZ'</summary>
        public static VfieldTag VARDELNZTag => _vfieldsByName["VARDELNZ"][0];

        public static string VARDELNZ
        {
            get => VARDELNZTag.Value;
            set => VARDELNZTag.Value = value;
        }

        /// <summary>Variable field 'KZORG'</summary>
        public static VfieldTag KZORGTag => _vfieldsByName["KZORG"][0];

        public static string KZORG
        {
            get => KZORGTag.Value;
            set => KZORGTag.Value = value;
        }

        /// <summary>Variable field 'VERKZ'</summary>
        public static VfieldTag VERKZTag => _vfieldsByName["VERKZ"][0];

        public static string VERKZ
        {
            get => VERKZTag.Value;
            set => VERKZTag.Value = value;
        }

        /// <summary>Variable field 'STEVKZ'</summary>
        public static VfieldTag STEVKZTag => _vfieldsByName["STEVKZ"][0];

        public static string STEVKZ
        {
            get => STEVKZTag.Value;
            set => STEVKZTag.Value = value;
        }

        /// <summary>Variable field 'LB3'</summary>
        public static VfieldTag LB3Tag => _vfieldsByName["LB3"][0];

        public static string LB3
        {
            get => LB3Tag.Value;
            set => LB3Tag.Value = value;
        }

        /// <summary>Variable field 'DNKOLIC'</summary>
        public static VfieldTag DNKOLICTag => _vfieldsByName["DNKOLIC"][0];

        public static string DNKOLIC
        {
            get => DNKOLICTag.Value;
            set => DNKOLICTag.Value = value;
        }

        /// <summary>Variable field 'DNKOMAD'</summary>
        public static VfieldTag DNKOMADTag => _vfieldsByName["DNKOMAD"][0];

        public static string DNKOMAD
        {
            get => DNKOMADTag.Value;
            set => DNKOMADTag.Value = value;
        }

        /// <summary>Variable field 'LB7'</summary>
        public static VfieldTag LB7Tag => _vfieldsByName["LB7"][0];

        public static string LB7
        {
            get => LB7Tag.Value;
            set => LB7Tag.Value = value;
        }

        /// <summary>Variable field 'ZACETNTE'</summary>
        public static VfieldTag ZACETNTETag => _vfieldsByName["ZACETNTE"][0];

        public static string ZACETNTE
        {
            get => ZACETNTETag.Value;
            set => ZACETNTETag.Value = value;
        }

        /// <summary>Variable field 'LB8'</summary>
        public static VfieldTag LB8Tag => _vfieldsByName["LB8"][0];

        public static string LB8
        {
            get => LB8Tag.Value;
            set => LB8Tag.Value = value;
        }

        /// <summary>Variable field 'KONCNATE'</summary>
        public static VfieldTag KONCNATETag => _vfieldsByName["KONCNATE"][0];

        public static string KONCNATE
        {
            get => KONCNATETag.Value;
            set => KONCNATETag.Value = value;
        }

        /// <summary>Variable field 'LB4'</summary>
        public static VfieldTag LB4Tag => _vfieldsByName["LB4"][0];

        public static string LB4
        {
            get => LB4Tag.Value;
            set => LB4Tag.Value = value;
        }

        /// <summary>Variable field 'IDENT'</summary>
        public static VfieldTag IDENTTag => _vfieldsByName["IDENT"][0];

        public static string IDENT
        {
            get => IDENTTag.Value;
            set => IDENTTag.Value = value;
        }

        /// <summary>Variable field 'POREKLO'</summary>
        public static VfieldTag POREKLOTag => _vfieldsByName["POREKLO"][0];

        public static string POREKLO
        {
            get => POREKLOTag.Value;
            set => POREKLOTag.Value = value;
        }

        /// <summary>Variable field 'NAMEN'</summary>
        public static VfieldTag NAMENTag => _vfieldsByName["NAMEN"][0];

        public static string NAMEN
        {
            get => NAMENTag.Value;
            set => NAMENTag.Value = value;
        }

        /// <summary>Variable field 'ZLITINA'</summary>
        public static VfieldTag ZLITINATag => _vfieldsByName["ZLITINA"][0];

        public static string ZLITINA
        {
            get => ZLITINATag.Value;
            set => ZLITINATag.Value = value;
        }

        /// <summary>Variable field 'LB5'</summary>
        public static VfieldTag LB5Tag => _vfieldsByName["LB5"][0];

        public static string LB5
        {
            get => LB5Tag.Value;
            set => LB5Tag.Value = value;
        }

        /// <summary>Variable field 'NAZIZDEL'</summary>
        public static VfieldTag NAZIZDELTag => _vfieldsByName["NAZIZDEL"][0];

        public static string NAZIZDEL
        {
            get => NAZIZDELTag.Value;
            set => NAZIZDELTag.Value = value;
        }

        /// <summary>Variable field 'STMASIF'</summary>
        public static VfieldTag STMASIFTag => _vfieldsByName["STMASIF"][0];

        public static string STMASIF
        {
            get => STMASIFTag.Value;
            set => STMASIFTag.Value = value;
        }

        /// <summary>Variable field 'LB6'</summary>
        public static VfieldTag LB6Tag => _vfieldsByName["LB6"][0];

        public static string LB6
        {
            get => LB6Tag.Value;
            set => LB6Tag.Value = value;
        }

        /// <summary>Variable field 'DIMENZ40'</summary>
        public static VfieldTag DIMENZ40Tag => _vfieldsByName["DIMENZ40"][0];

        public static string DIMENZ40
        {
            get => DIMENZ40Tag.Value;
            set => DIMENZ40Tag.Value = value;
        }

        /// <summary>Variable field 'STPOSIF'</summary>
        public static VfieldTag STPOSIFTag => _vfieldsByName["STPOSIF"][0];

        public static string STPOSIF
        {
            get => STPOSIFTag.Value;
            set => STPOSIFTag.Value = value;
        }

        /// <summary>Variable field 'LB9'</summary>
        public static VfieldTag LB9Tag => _vfieldsByName["LB9"][0];

        public static string LB9
        {
            get => LB9Tag.Value;
            set => LB9Tag.Value = value;
        }

        /// <summary>Variable field 'STEVILOP'</summary>
        public static VfieldTag STEVILOPTag => _vfieldsByName["STEVILOP"][0];

        public static string STEVILOP
        {
            get => STEVILOPTag.Value;
            set => STEVILOPTag.Value = value;
        }

        /// <summary>Variable field 'STEVSTRO'</summary>
        public static VfieldTag STEVSTROTag => _vfieldsByName["STEVSTRO"][0];

        public static string STEVSTRO
        {
            get => STEVSTROTag.Value;
            set => STEVSTROTag.Value = value;
        }

        /// <summary>Variable field 'NAZSTRKR'</summary>
        public static VfieldTag NAZSTRKRTag => _vfieldsByName["NAZSTRKR"][0];

        public static string NAZSTRKR
        {
            get => NAZSTRKRTag.Value;
            set => NAZSTRKRTag.Value = value;
        }

        /// <summary>Variable field 'IZDKOL'</summary>
        public static VfieldTag IZDKOLTag => _vfieldsByName["IZDKOL"][0];

        public static string IZDKOL
        {
            get => IZDKOLTag.Value;
            set => IZDKOLTag.Value = value;
        }

        /// <summary>Variable field 'IZDKOM'</summary>
        public static VfieldTag IZDKOMTag => _vfieldsByName["IZDKOM"][0];

        public static string IZDKOM
        {
            get => IZDKOMTag.Value;
            set => IZDKOMTag.Value = value;
        }

        /// <summary>Variable field 'DELVNOS'</summary>
        public static VfieldTag DELVNOSTag => _vfieldsByName["DELVNOS"][0];

        public static string DELVNOS
        {
            get => DELVNOSTag.Value;
            set => DELVNOSTag.Value = value;
        }

        /// <summary>Variable field 'DATSPRE'</summary>
        public static VfieldTag DATSPRETag => _vfieldsByName["DATSPRE"][0];

        public static string DATSPRE
        {
            get => DATSPRETag.Value;
            set => DATSPRETag.Value = value;
        }

        /// <summary>Variable field 'LB10'</summary>
        public static VfieldTag LB10Tag => _vfieldsByName["LB10"][0];

        public static string LB10
        {
            get => LB10Tag.Value;
            set => LB10Tag.Value = value;
        }

        /// <summary>Variable field 'TRAN_ENOTA' (multiple)</summary>
        public static string[] TRAN_ENOTA
        {
            get
            {
                return _vfieldsByName["TRAN_ENOTA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'SARZA' (multiple)</summary>
        public static string[] SARZA
        {
            get
            {
                return _vfieldsByName["SARZA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'IDSARZA' (multiple)</summary>
        public static string[] IDSARZA
        {
            get
            {
                return _vfieldsByName["IDSARZA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'IZDKOL-TRE' (multiple)</summary>
        public static string[] IZDKOL_TRE
        {
            get
            {
                return _vfieldsByName["IZDKOL-TRE"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'IZDKOMTRE' (multiple)</summary>
        public static string[] IZDKOMTRE
        {
            get
            {
                return _vfieldsByName["IZDKOMTRE"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'VZOREC' (multiple)</summary>
        public static string[] VZOREC
        {
            get
            {
                return _vfieldsByName["VZOREC"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'USTREZA' (multiple)</summary>
        public static string[] USTREZA
        {
            get
            {
                return _vfieldsByName["USTREZA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'USTREZA-KZ' (multiple)</summary>
        public static string[] USTREZA_KZ
        {
            get
            {
                return _vfieldsByName["USTREZA-KZ"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'KOM' (multiple)</summary>
        public static string[] KOM
        {
            get
            {
                return _vfieldsByName["KOM"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'STATSUD' (multiple)</summary>
        public static string[] STATSUD
        {
            get
            {
                return _vfieldsByName["STATSUD"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'L1' (multiple)</summary>
        public static string[] L1
        {
            get
            {
                return _vfieldsByName["L1"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'L2' (multiple)</summary>
        public static string[] L2
        {
            get
            {
                return _vfieldsByName["L2"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'L3' (multiple)</summary>
        public static string[] L3
        {
            get
            {
                return _vfieldsByName["L3"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'L4' (multiple)</summary>
        public static string[] L4
        {
            get
            {
                return _vfieldsByName["L4"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'EZEMSG'</summary>
        public static VfieldTag EZEMSGTag => _vfieldsByName["EZEMSG"][0];

        public static string EZEMSG
        {
            get => EZEMSGTag.Value;
            set => EZEMSGTag.Value = value;
        }

        /// <summary>Variable field 'LB11'</summary>
        public static VfieldTag LB11Tag => _vfieldsByName["LB11"][0];

        public static string LB11
        {
            get => LB11Tag.Value;
            set => LB11Tag.Value = value;
        }

        static D133M04()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133M04");
                WriteWrapped(8, 0, "");
                WriteWrapped(68, 0, "");
                WriteWrapped(26, 1, "");
                WriteWrapped(69, 1, "");
                WriteWrapped(4, 2, "");
                WriteWrapped(14, 2, "");
                WriteWrapped(44, 2, "");
                WriteWrapped(26, 3, "");
                WriteWrapped(28, 3, "DNk:");
                WriteWrapped(41, 3, "DNz:");
                WriteWrapped(54, 3, "");
                WriteWrapped(56, 3, "KZ:");
                WriteWrapped(64, 3, "/");
                WriteWrapped(73, 3, "");
                WriteWrapped(27, 4, "kg");
                WriteWrapped(37, 4, "kom");
                WriteWrapped(79, 4, "");
                WriteWrapped(25, 5, "");
                WriteWrapped(27, 5, "Poreklo:");
                WriteWrapped(40, 5, "Namen:");
                WriteWrapped(50, 5, "");
                WriteWrapped(59, 5, "");
                WriteWrapped(60, 5, "Zlitina    :");
                WriteWrapped(78, 5, "");
                WriteWrapped(39, 6, "");
                WriteWrapped(59, 6, "");
                WriteWrapped(60, 6, "Stanje mat.:");
                WriteWrapped(79, 6, "");
                WriteWrapped(59, 7, " Stanje pov.:");
                WriteWrapped(79, 7, "");
                WriteWrapped(79, 8, "");
                WriteWrapped(79, 9, "");
                WriteWrapped(79, 10, "");
                WriteWrapped(0, 11, "TRE      Izv. sarža        Gen.S.  Kol   Kom VZ U Ukz Komentar    S");
                WriteWrapped(69, 11, "Regal-lok");
                WriteWrapped(79, 11, "");
                WriteWrapped(79, 12, "");
                WriteWrapped(79, 13, "");
                WriteWrapped(79, 14, "");
                WriteWrapped(79, 15, "");
                WriteWrapped(79, 16, "");
                WriteWrapped(79, 17, "");
                WriteWrapped(79, 18, "");
                WriteWrapped(79, 19, "");
                WriteWrapped(79, 20, "");
                WriteWrapped(79, 21, "");
                WriteWrapped(79, 22, "");
                WriteWrapped(79, 23, "");
            
                WriteWrapped(20, 0, "");
                WriteWrapped(0, 3, "");
                WriteWrapped(18, 3, "");
                WriteWrapped(24, 3, "");
                WriteWrapped(33, 3, "");
                WriteWrapped(39, 3, "");
                WriteWrapped(46, 3, "");
                WriteWrapped(52, 3, "");
                WriteWrapped(60, 3, "");
                WriteWrapped(66, 3, "");
                WriteWrapped(69, 3, "");
                WriteWrapped(0, 4, "");
                WriteWrapped(18, 4, "");
                WriteWrapped(30, 4, "");
                WriteWrapped(42, 4, "");
                WriteWrapped(54, 4, "");
                WriteWrapped(61, 4, "");
                WriteWrapped(72, 4, "");
                WriteWrapped(0, 5, "");
                WriteWrapped(18, 5, "");
                WriteWrapped(36, 5, "");
                WriteWrapped(47, 5, "");
                WriteWrapped(73, 5, "");
                WriteWrapped(0, 6, "");
                WriteWrapped(18, 6, "");
                WriteWrapped(73, 6, "");
                WriteWrapped(0, 7, "");
                WriteWrapped(18, 7, "");
                WriteWrapped(73, 7, "");
                WriteWrapped(0, 8, "");
                WriteWrapped(2, 9, "");
                WriteWrapped(5, 9, "");
                WriteWrapped(10, 9, "");
                WriteWrapped(31, 9, "");
                WriteWrapped(41, 9, "");
                WriteWrapped(47, 9, "");
                WriteWrapped(68, 9, "");
                WriteWrapped(0, 10, "");
                WriteWrapped(0, 12, "");
                WriteWrapped(9, 12, "");
                WriteWrapped(28, 12, "");
                WriteWrapped(35, 12, "");
                WriteWrapped(41, 12, "");
                WriteWrapped(47, 12, "");
                WriteWrapped(50, 12, "");
                WriteWrapped(52, 12, "");
                WriteWrapped(54, 12, "");
                WriteWrapped(67, 12, "");
                WriteWrapped(69, 12, "");
                WriteWrapped(71, 12, "");
                WriteWrapped(74, 12, "");
                WriteWrapped(77, 12, "");
                WriteWrapped(0, 13, "");
                WriteWrapped(9, 13, "");
                WriteWrapped(28, 13, "");
                WriteWrapped(35, 13, "");
                WriteWrapped(41, 13, "");
                WriteWrapped(47, 13, "");
                WriteWrapped(50, 13, "");
                WriteWrapped(52, 13, "");
                WriteWrapped(54, 13, "");
                WriteWrapped(67, 13, "");
                WriteWrapped(69, 13, "");
                WriteWrapped(71, 13, "");
                WriteWrapped(74, 13, "");
                WriteWrapped(77, 13, "");
                WriteWrapped(0, 14, "");
                WriteWrapped(9, 14, "");
                WriteWrapped(28, 14, "");
                WriteWrapped(35, 14, "");
                WriteWrapped(41, 14, "");
                WriteWrapped(47, 14, "");
                WriteWrapped(50, 14, "");
                WriteWrapped(52, 14, "");
                WriteWrapped(54, 14, "");
                WriteWrapped(67, 14, "");
                WriteWrapped(69, 14, "");
                WriteWrapped(71, 14, "");
                WriteWrapped(74, 14, "");
                WriteWrapped(77, 14, "");
                WriteWrapped(0, 15, "");
                WriteWrapped(9, 15, "");
                WriteWrapped(28, 15, "");
                WriteWrapped(35, 15, "");
                WriteWrapped(41, 15, "");
                WriteWrapped(47, 15, "");
                WriteWrapped(50, 15, "");
                WriteWrapped(52, 15, "");
                WriteWrapped(54, 15, "");
                WriteWrapped(67, 15, "");
                WriteWrapped(69, 15, "");
                WriteWrapped(71, 15, "");
                WriteWrapped(74, 15, "");
                WriteWrapped(77, 15, "");
                WriteWrapped(0, 16, "");
                WriteWrapped(9, 16, "");
                WriteWrapped(28, 16, "");
                WriteWrapped(35, 16, "");
                WriteWrapped(41, 16, "");
                WriteWrapped(47, 16, "");
                WriteWrapped(50, 16, "");
                WriteWrapped(52, 16, "");
                WriteWrapped(54, 16, "");
                WriteWrapped(67, 16, "");
                WriteWrapped(69, 16, "");
                WriteWrapped(71, 16, "");
                WriteWrapped(74, 16, "");
                WriteWrapped(77, 16, "");
                WriteWrapped(0, 17, "");
                WriteWrapped(9, 17, "");
                WriteWrapped(28, 17, "");
                WriteWrapped(35, 17, "");
                WriteWrapped(41, 17, "");
                WriteWrapped(47, 17, "");
                WriteWrapped(50, 17, "");
                WriteWrapped(52, 17, "");
                WriteWrapped(54, 17, "");
                WriteWrapped(67, 17, "");
                WriteWrapped(69, 17, "");
                WriteWrapped(71, 17, "");
                WriteWrapped(74, 17, "");
                WriteWrapped(77, 17, "");
                WriteWrapped(0, 18, "");
                WriteWrapped(9, 18, "");
                WriteWrapped(28, 18, "");
                WriteWrapped(35, 18, "");
                WriteWrapped(41, 18, "");
                WriteWrapped(47, 18, "");
                WriteWrapped(50, 18, "");
                WriteWrapped(52, 18, "");
                WriteWrapped(54, 18, "");
                WriteWrapped(67, 18, "");
                WriteWrapped(69, 18, "");
                WriteWrapped(71, 18, "");
                WriteWrapped(74, 18, "");
                WriteWrapped(77, 18, "");
                WriteWrapped(0, 19, "");
                WriteWrapped(9, 19, "");
                WriteWrapped(28, 19, "");
                WriteWrapped(35, 19, "");
                WriteWrapped(41, 19, "");
                WriteWrapped(47, 19, "");
                WriteWrapped(50, 19, "");
                WriteWrapped(52, 19, "");
                WriteWrapped(54, 19, "");
                WriteWrapped(67, 19, "");
                WriteWrapped(69, 19, "");
                WriteWrapped(71, 19, "");
                WriteWrapped(74, 19, "");
                WriteWrapped(77, 19, "");
                WriteWrapped(0, 20, "");
                WriteWrapped(9, 20, "");
                WriteWrapped(28, 20, "");
                WriteWrapped(35, 20, "");
                WriteWrapped(41, 20, "");
                WriteWrapped(47, 20, "");
                WriteWrapped(50, 20, "");
                WriteWrapped(52, 20, "");
                WriteWrapped(54, 20, "");
                WriteWrapped(67, 20, "");
                WriteWrapped(69, 20, "");
                WriteWrapped(71, 20, "");
                WriteWrapped(74, 20, "");
                WriteWrapped(77, 20, "");
                WriteWrapped(0, 21, "");
                WriteWrapped(9, 21, "");
                WriteWrapped(28, 21, "");
                WriteWrapped(35, 21, "");
                WriteWrapped(41, 21, "");
                WriteWrapped(47, 21, "");
                WriteWrapped(50, 21, "");
                WriteWrapped(52, 21, "");
                WriteWrapped(54, 21, "");
                WriteWrapped(67, 21, "");
                WriteWrapped(69, 21, "");
                WriteWrapped(71, 21, "");
                WriteWrapped(74, 21, "");
                WriteWrapped(77, 21, "");
                WriteWrapped(0, 22, "");
                WriteWrapped(0, 23, "");
            
                Console.SetCursorPosition(20, 0);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
    public static class D133M05
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
            new VfieldTag { Row = 1, Column = 24, Name = "LB1", Type = "CHA", Bytes = 34, Value = "" },
            new VfieldTag { Row = 5, Column = 4, Name = "LB2", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 5, Column = 21, Name = "LETONA1", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 5, Column = 26, Name = "STNAROC1", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 5, Column = 33, Name = "POZNARO1", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 6, Column = 4, Name = "LB3", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 6, Column = 21, Name = "NARKOLPR1", Type = "NUM", Bytes = 12, Value = "" },
            new VfieldTag { Row = 6, Column = 34, Name = "LB4", Type = "CHA", Bytes = 11, Value = "" },
            new VfieldTag { Row = 6, Column = 46, Name = "IZDKOL1", Type = "NUM", Bytes = 12, Value = "" },
            new VfieldTag { Row = 6, Column = 59, Name = "KOLICINA_TOL1", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 7, Column = 4, Name = "LB5", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 7, Column = 25, Name = "TOL_KOL_PL1", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 7, Column = 35, Name = "TOL_KOL_MI1", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 7, Column = 40, Name = "KRNAZEM1", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 9, Column = 4, Name = "LB2_2", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 9, Column = 21, Name = "LETONA2", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 9, Column = 26, Name = "STNAROC2", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 9, Column = 33, Name = "POZNARO2", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 10, Column = 4, Name = "LB3_2", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 10, Column = 21, Name = "NARKOLPR2", Type = "NUM", Bytes = 12, Value = "" },
            new VfieldTag { Row = 10, Column = 34, Name = "LB4_2", Type = "CHA", Bytes = 11, Value = "" },
            new VfieldTag { Row = 10, Column = 46, Name = "IZDKOL2", Type = "NUM", Bytes = 12, Value = "" },
            new VfieldTag { Row = 10, Column = 59, Name = "KOLICINA_TOL2", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 11, Column = 4, Name = "LB5_2", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 11, Column = 25, Name = "TOL_KOL_PL2", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 11, Column = 35, Name = "TOL_KOL_MI2", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 11, Column = 40, Name = "KRNAZEM2", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 13, Column = 4, Name = "LB2_3", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 13, Column = 21, Name = "LETONA3", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 13, Column = 26, Name = "STNAROC3", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 13, Column = 33, Name = "POZNARO3", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 4, Name = "LB3_3", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 14, Column = 21, Name = "NARKOLPR3", Type = "NUM", Bytes = 12, Value = "" },
            new VfieldTag { Row = 14, Column = 34, Name = "LB4_3", Type = "CHA", Bytes = 11, Value = "" },
            new VfieldTag { Row = 14, Column = 46, Name = "IZDKOL3", Type = "NUM", Bytes = 12, Value = "" },
            new VfieldTag { Row = 14, Column = 59, Name = "KOLICINA_TOL3", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 15, Column = 4, Name = "LB5_3", Type = "CHA", Bytes = 16, Value = "" },
            new VfieldTag { Row = 15, Column = 25, Name = "TOL_KOL_PL3", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 15, Column = 35, Name = "TOL_KOL_MI3", Type = "NUM", Bytes = 4, Value = "" },
            new VfieldTag { Row = 15, Column = 40, Name = "KRNAZEM3", Type = "CHA", Bytes = 10, Value = "" },
            new VfieldTag { Row = 23, Column = 1, Name = "EZEMSG", Type = "CHA", Bytes = 78, Value = "" },
            new VfieldTag { Row = 24, Column = 1, Name = "LB16", Type = "CHA", Bytes = 78, Value = "" },
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        /// <summary>Variable field 'LB1'</summary>
        public static VfieldTag LB1Tag => _vfieldsByName["LB1"][0];

        public static string LB1
        {
            get => LB1Tag.Value;
            set => LB1Tag.Value = value;
        }

        /// <summary>Variable field 'LB2'</summary>
        public static VfieldTag LB2Tag => _vfieldsByName["LB2"][0];

        public static string LB2
        {
            get => LB2Tag.Value;
            set => LB2Tag.Value = value;
        }

        /// <summary>Variable field 'LETONA1'</summary>
        public static VfieldTag LETONA1Tag => _vfieldsByName["LETONA1"][0];

        public static string LETONA1
        {
            get => LETONA1Tag.Value;
            set => LETONA1Tag.Value = value;
        }

        /// <summary>Variable field 'STNAROC1'</summary>
        public static VfieldTag STNAROC1Tag => _vfieldsByName["STNAROC1"][0];

        public static string STNAROC1
        {
            get => STNAROC1Tag.Value;
            set => STNAROC1Tag.Value = value;
        }

        /// <summary>Variable field 'POZNARO1'</summary>
        public static VfieldTag POZNARO1Tag => _vfieldsByName["POZNARO1"][0];

        public static string POZNARO1
        {
            get => POZNARO1Tag.Value;
            set => POZNARO1Tag.Value = value;
        }

        /// <summary>Variable field 'LB3'</summary>
        public static VfieldTag LB3Tag => _vfieldsByName["LB3"][0];

        public static string LB3
        {
            get => LB3Tag.Value;
            set => LB3Tag.Value = value;
        }

        /// <summary>Variable field 'NARKOLPR1'</summary>
        public static VfieldTag NARKOLPR1Tag => _vfieldsByName["NARKOLPR1"][0];

        public static string NARKOLPR1
        {
            get => NARKOLPR1Tag.Value;
            set => NARKOLPR1Tag.Value = value;
        }

        /// <summary>Variable field 'LB4'</summary>
        public static VfieldTag LB4Tag => _vfieldsByName["LB4"][0];

        public static string LB4
        {
            get => LB4Tag.Value;
            set => LB4Tag.Value = value;
        }

        /// <summary>Variable field 'IZDKOL1'</summary>
        public static VfieldTag IZDKOL1Tag => _vfieldsByName["IZDKOL1"][0];

        public static string IZDKOL1
        {
            get => IZDKOL1Tag.Value;
            set => IZDKOL1Tag.Value = value;
        }

        /// <summary>Variable field 'KOLICINA_TOL1'</summary>
        public static VfieldTag KOLICINA_TOL1Tag => _vfieldsByName["KOLICINA_TOL1"][0];

        public static string KOLICINA_TOL1
        {
            get => KOLICINA_TOL1Tag.Value;
            set => KOLICINA_TOL1Tag.Value = value;
        }

        /// <summary>Variable field 'LB5'</summary>
        public static VfieldTag LB5Tag => _vfieldsByName["LB5"][0];

        public static string LB5
        {
            get => LB5Tag.Value;
            set => LB5Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_KOL_PL1'</summary>
        public static VfieldTag TOL_KOL_PL1Tag => _vfieldsByName["TOL_KOL_PL1"][0];

        public static string TOL_KOL_PL1
        {
            get => TOL_KOL_PL1Tag.Value;
            set => TOL_KOL_PL1Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_KOL_MI1'</summary>
        public static VfieldTag TOL_KOL_MI1Tag => _vfieldsByName["TOL_KOL_MI1"][0];

        public static string TOL_KOL_MI1
        {
            get => TOL_KOL_MI1Tag.Value;
            set => TOL_KOL_MI1Tag.Value = value;
        }

        /// <summary>Variable field 'KRNAZEM1'</summary>
        public static VfieldTag KRNAZEM1Tag => _vfieldsByName["KRNAZEM1"][0];

        public static string KRNAZEM1
        {
            get => KRNAZEM1Tag.Value;
            set => KRNAZEM1Tag.Value = value;
        }

        /// <summary>Variable field 'LB2_2'</summary>
        public static VfieldTag LB2_2Tag => _vfieldsByName["LB2_2"][0];

        public static string LB2_2
        {
            get => LB2_2Tag.Value;
            set => LB2_2Tag.Value = value;
        }

        /// <summary>Variable field 'LETONA2'</summary>
        public static VfieldTag LETONA2Tag => _vfieldsByName["LETONA2"][0];

        public static string LETONA2
        {
            get => LETONA2Tag.Value;
            set => LETONA2Tag.Value = value;
        }

        /// <summary>Variable field 'STNAROC2'</summary>
        public static VfieldTag STNAROC2Tag => _vfieldsByName["STNAROC2"][0];

        public static string STNAROC2
        {
            get => STNAROC2Tag.Value;
            set => STNAROC2Tag.Value = value;
        }

        /// <summary>Variable field 'POZNARO2'</summary>
        public static VfieldTag POZNARO2Tag => _vfieldsByName["POZNARO2"][0];

        public static string POZNARO2
        {
            get => POZNARO2Tag.Value;
            set => POZNARO2Tag.Value = value;
        }

        /// <summary>Variable field 'LB3_2'</summary>
        public static VfieldTag LB3_2Tag => _vfieldsByName["LB3_2"][0];

        public static string LB3_2
        {
            get => LB3_2Tag.Value;
            set => LB3_2Tag.Value = value;
        }

        /// <summary>Variable field 'NARKOLPR2'</summary>
        public static VfieldTag NARKOLPR2Tag => _vfieldsByName["NARKOLPR2"][0];

        public static string NARKOLPR2
        {
            get => NARKOLPR2Tag.Value;
            set => NARKOLPR2Tag.Value = value;
        }

        /// <summary>Variable field 'LB4_2'</summary>
        public static VfieldTag LB4_2Tag => _vfieldsByName["LB4_2"][0];

        public static string LB4_2
        {
            get => LB4_2Tag.Value;
            set => LB4_2Tag.Value = value;
        }

        /// <summary>Variable field 'IZDKOL2'</summary>
        public static VfieldTag IZDKOL2Tag => _vfieldsByName["IZDKOL2"][0];

        public static string IZDKOL2
        {
            get => IZDKOL2Tag.Value;
            set => IZDKOL2Tag.Value = value;
        }

        /// <summary>Variable field 'KOLICINA_TOL2'</summary>
        public static VfieldTag KOLICINA_TOL2Tag => _vfieldsByName["KOLICINA_TOL2"][0];

        public static string KOLICINA_TOL2
        {
            get => KOLICINA_TOL2Tag.Value;
            set => KOLICINA_TOL2Tag.Value = value;
        }

        /// <summary>Variable field 'LB5_2'</summary>
        public static VfieldTag LB5_2Tag => _vfieldsByName["LB5_2"][0];

        public static string LB5_2
        {
            get => LB5_2Tag.Value;
            set => LB5_2Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_KOL_PL2'</summary>
        public static VfieldTag TOL_KOL_PL2Tag => _vfieldsByName["TOL_KOL_PL2"][0];

        public static string TOL_KOL_PL2
        {
            get => TOL_KOL_PL2Tag.Value;
            set => TOL_KOL_PL2Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_KOL_MI2'</summary>
        public static VfieldTag TOL_KOL_MI2Tag => _vfieldsByName["TOL_KOL_MI2"][0];

        public static string TOL_KOL_MI2
        {
            get => TOL_KOL_MI2Tag.Value;
            set => TOL_KOL_MI2Tag.Value = value;
        }

        /// <summary>Variable field 'KRNAZEM2'</summary>
        public static VfieldTag KRNAZEM2Tag => _vfieldsByName["KRNAZEM2"][0];

        public static string KRNAZEM2
        {
            get => KRNAZEM2Tag.Value;
            set => KRNAZEM2Tag.Value = value;
        }

        /// <summary>Variable field 'LB2_3'</summary>
        public static VfieldTag LB2_3Tag => _vfieldsByName["LB2_3"][0];

        public static string LB2_3
        {
            get => LB2_3Tag.Value;
            set => LB2_3Tag.Value = value;
        }

        /// <summary>Variable field 'LETONA3'</summary>
        public static VfieldTag LETONA3Tag => _vfieldsByName["LETONA3"][0];

        public static string LETONA3
        {
            get => LETONA3Tag.Value;
            set => LETONA3Tag.Value = value;
        }

        /// <summary>Variable field 'STNAROC3'</summary>
        public static VfieldTag STNAROC3Tag => _vfieldsByName["STNAROC3"][0];

        public static string STNAROC3
        {
            get => STNAROC3Tag.Value;
            set => STNAROC3Tag.Value = value;
        }

        /// <summary>Variable field 'POZNARO3'</summary>
        public static VfieldTag POZNARO3Tag => _vfieldsByName["POZNARO3"][0];

        public static string POZNARO3
        {
            get => POZNARO3Tag.Value;
            set => POZNARO3Tag.Value = value;
        }

        /// <summary>Variable field 'LB3_3'</summary>
        public static VfieldTag LB3_3Tag => _vfieldsByName["LB3_3"][0];

        public static string LB3_3
        {
            get => LB3_3Tag.Value;
            set => LB3_3Tag.Value = value;
        }

        /// <summary>Variable field 'NARKOLPR3'</summary>
        public static VfieldTag NARKOLPR3Tag => _vfieldsByName["NARKOLPR3"][0];

        public static string NARKOLPR3
        {
            get => NARKOLPR3Tag.Value;
            set => NARKOLPR3Tag.Value = value;
        }

        /// <summary>Variable field 'LB4_3'</summary>
        public static VfieldTag LB4_3Tag => _vfieldsByName["LB4_3"][0];

        public static string LB4_3
        {
            get => LB4_3Tag.Value;
            set => LB4_3Tag.Value = value;
        }

        /// <summary>Variable field 'IZDKOL3'</summary>
        public static VfieldTag IZDKOL3Tag => _vfieldsByName["IZDKOL3"][0];

        public static string IZDKOL3
        {
            get => IZDKOL3Tag.Value;
            set => IZDKOL3Tag.Value = value;
        }

        /// <summary>Variable field 'KOLICINA_TOL3'</summary>
        public static VfieldTag KOLICINA_TOL3Tag => _vfieldsByName["KOLICINA_TOL3"][0];

        public static string KOLICINA_TOL3
        {
            get => KOLICINA_TOL3Tag.Value;
            set => KOLICINA_TOL3Tag.Value = value;
        }

        /// <summary>Variable field 'LB5_3'</summary>
        public static VfieldTag LB5_3Tag => _vfieldsByName["LB5_3"][0];

        public static string LB5_3
        {
            get => LB5_3Tag.Value;
            set => LB5_3Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_KOL_PL3'</summary>
        public static VfieldTag TOL_KOL_PL3Tag => _vfieldsByName["TOL_KOL_PL3"][0];

        public static string TOL_KOL_PL3
        {
            get => TOL_KOL_PL3Tag.Value;
            set => TOL_KOL_PL3Tag.Value = value;
        }

        /// <summary>Variable field 'TOL_KOL_MI3'</summary>
        public static VfieldTag TOL_KOL_MI3Tag => _vfieldsByName["TOL_KOL_MI3"][0];

        public static string TOL_KOL_MI3
        {
            get => TOL_KOL_MI3Tag.Value;
            set => TOL_KOL_MI3Tag.Value = value;
        }

        /// <summary>Variable field 'KRNAZEM3'</summary>
        public static VfieldTag KRNAZEM3Tag => _vfieldsByName["KRNAZEM3"][0];

        public static string KRNAZEM3
        {
            get => KRNAZEM3Tag.Value;
            set => KRNAZEM3Tag.Value = value;
        }

        /// <summary>Variable field 'EZEMSG'</summary>
        public static VfieldTag EZEMSGTag => _vfieldsByName["EZEMSG"][0];

        public static string EZEMSG
        {
            get => EZEMSGTag.Value;
            set => EZEMSGTag.Value = value;
        }

        /// <summary>Variable field 'LB16'</summary>
        public static VfieldTag LB16Tag => _vfieldsByName["LB16"][0];

        public static string LB16
        {
            get => LB16Tag.Value;
            set => LB16Tag.Value = value;
        }

        static D133M05()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133M05");
                WriteWrapped(12, 0, "");
                WriteWrapped(58, 0, "");
                WriteWrapped(20, 3, "");
                WriteWrapped(0, 4, "1:");
                WriteWrapped(35, 4, "");
                WriteWrapped(60, 5, "");
                WriteWrapped(20, 6, "(+)");
                WriteWrapped(29, 6, "(-):");
                WriteWrapped(50, 6, "");
                WriteWrapped(20, 7, "");
                WriteWrapped(0, 8, "2:");
                WriteWrapped(35, 8, "");
                WriteWrapped(60, 9, "");
                WriteWrapped(20, 10, "(+)");
                WriteWrapped(29, 10, "(-):");
                WriteWrapped(50, 10, "");
                WriteWrapped(21, 11, "");
                WriteWrapped(0, 12, "3:");
                WriteWrapped(35, 12, "");
                WriteWrapped(60, 13, "");
                WriteWrapped(20, 14, "(+)");
                WriteWrapped(29, 14, "(-):");
                WriteWrapped(50, 14, "");
                WriteWrapped(79, 22, "");
                WriteWrapped(79, 23, "");
            
                WriteWrapped(23, 0, "");
                WriteWrapped(3, 4, "");
                WriteWrapped(20, 4, "");
                WriteWrapped(25, 4, "");
                WriteWrapped(32, 4, "");
                WriteWrapped(3, 5, "");
                WriteWrapped(20, 5, "");
                WriteWrapped(33, 5, "");
                WriteWrapped(45, 5, "");
                WriteWrapped(58, 5, "");
                WriteWrapped(3, 6, "");
                WriteWrapped(24, 6, "");
                WriteWrapped(34, 6, "");
                WriteWrapped(39, 6, "");
                WriteWrapped(3, 8, "");
                WriteWrapped(20, 8, "");
                WriteWrapped(25, 8, "");
                WriteWrapped(32, 8, "");
                WriteWrapped(3, 9, "");
                WriteWrapped(20, 9, "");
                WriteWrapped(33, 9, "");
                WriteWrapped(45, 9, "");
                WriteWrapped(58, 9, "");
                WriteWrapped(3, 10, "");
                WriteWrapped(24, 10, "");
                WriteWrapped(34, 10, "");
                WriteWrapped(39, 10, "");
                WriteWrapped(3, 12, "");
                WriteWrapped(20, 12, "");
                WriteWrapped(25, 12, "");
                WriteWrapped(32, 12, "");
                WriteWrapped(3, 13, "");
                WriteWrapped(20, 13, "");
                WriteWrapped(33, 13, "");
                WriteWrapped(45, 13, "");
                WriteWrapped(58, 13, "");
                WriteWrapped(3, 14, "");
                WriteWrapped(24, 14, "");
                WriteWrapped(34, 14, "");
                WriteWrapped(39, 14, "");
                WriteWrapped(0, 22, "");
                WriteWrapped(0, 23, "");
            
                Console.SetCursorPosition(23, 0);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
    public static class D133M13
    {
        /// <summary>All variable fields on this map</summary>
        public static readonly IReadOnlyList<VfieldTag> Vfields = new List<VfieldTag>
        {
            new VfieldTag { Row = 3, Column = 16, Name = "DELNAL", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 3, Column = 24, Name = "VARDELN", Type = "CHA", Bytes = 1, Value = "" },
            new VfieldTag { Row = 3, Column = 33, Name = "IDENT", Type = "NUM", Bytes = 6, Value = "" },
            new VfieldTag { Row = 3, Column = 41, Name = "DIMENZ40_DN", Type = "CHA", Bytes = 37, Value = "" },
            new VfieldTag { Row = 7, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 7, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 7, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 7, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 7, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 7, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 7, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 7, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 8, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 8, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 8, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 8, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 8, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 8, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 8, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 8, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 9, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 9, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 9, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 9, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 9, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 9, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 9, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 10, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 10, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 10, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 10, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 10, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 10, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 10, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 10, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 11, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 11, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 11, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 11, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 11, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 11, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 11, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 11, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 12, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 12, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 12, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 12, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 12, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 12, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 12, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 12, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 13, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 13, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 13, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 13, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 13, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 13, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 13, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 14, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 14, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 14, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 14, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 14, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 14, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 14, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 15, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 15, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 15, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 15, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 15, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 15, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 15, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 16, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 16, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 16, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 16, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 16, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 16, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 16, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 17, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 17, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 17, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 17, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 17, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 17, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 17, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 18, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 18, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 18, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 18, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 18, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 18, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 18, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 19, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 19, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 19, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 19, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 19, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 19, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 20, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 20, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 20, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 20, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 20, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 20, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 20, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 21, Column = 3, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 6, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 21, Column = 15, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 21, Column = 21, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 21, Column = 41, Name = "OZNAKA", Type = "NUM", Bytes = 2, Value = "" },
            new VfieldTag { Row = 21, Column = 44, Name = "SARZA", Type = "CHA", Bytes = 8, Value = "" },
            new VfieldTag { Row = 21, Column = 53, Name = "KOLICINA", Type = "NUM", Bytes = 5, Value = "" },
            new VfieldTag { Row = 21, Column = 59, Name = "DIMENZ40", Type = "CHA", Bytes = 19, Value = "" },
            new VfieldTag { Row = 23, Column = 1, Name = "EZEMSG", Type = "CHA", Bytes = 78, Value = "" },
        };

        // group variable fields by Name
        private static readonly Dictionary<string, IReadOnlyList<VfieldTag>> _vfieldsByName =
            Vfields
             .GroupBy(v => v.Name, StringComparer.OrdinalIgnoreCase)
             .ToDictionary(g => g.Key, g => (IReadOnlyList<VfieldTag>)g.ToList());

        /// <summary>Variable field 'DELNAL'</summary>
        public static VfieldTag DELNALTag => _vfieldsByName["DELNAL"][0];

        public static string DELNAL
        {
            get => DELNALTag.Value;
            set => DELNALTag.Value = value;
        }

        /// <summary>Variable field 'VARDELN'</summary>
        public static VfieldTag VARDELNTag => _vfieldsByName["VARDELN"][0];

        public static string VARDELN
        {
            get => VARDELNTag.Value;
            set => VARDELNTag.Value = value;
        }

        /// <summary>Variable field 'IDENT'</summary>
        public static VfieldTag IDENTTag => _vfieldsByName["IDENT"][0];

        public static string IDENT
        {
            get => IDENTTag.Value;
            set => IDENTTag.Value = value;
        }

        /// <summary>Variable field 'DIMENZ40_DN'</summary>
        public static VfieldTag DIMENZ40_DNTag => _vfieldsByName["DIMENZ40_DN"][0];

        public static string DIMENZ40_DN
        {
            get => DIMENZ40_DNTag.Value;
            set => DIMENZ40_DNTag.Value = value;
        }

        /// <summary>Variable field 'OZNAKA' (multiple)</summary>
        public static string[] OZNAKA
        {
            get
            {
                return _vfieldsByName["OZNAKA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'SARZA' (multiple)</summary>
        public static string[] SARZA
        {
            get
            {
                return _vfieldsByName["SARZA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'KOLICINA' (multiple)</summary>
        public static string[] KOLICINA
        {
            get
            {
                return _vfieldsByName["KOLICINA"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'DIMENZ40' (multiple)</summary>
        public static string[] DIMENZ40
        {
            get
            {
                return _vfieldsByName["DIMENZ40"].Select(t=>t.Value).ToArray();
            }
            set
            {
                var list = _vfieldsByName["{name}"];
                for(int i=0;i<list.Count && i<value.Length;i++)
                    list[i].Value = value[i];
            }
        }

        /// <summary>Variable field 'EZEMSG'</summary>
        public static VfieldTag EZEMSGTag => _vfieldsByName["EZEMSG"][0];

        public static string EZEMSG
        {
            get => EZEMSGTag.Value;
            set => EZEMSGTag.Value = value;
        }

        static D133M13()
        {
            foreach(var tag in Vfields)
                tag.OnCursor += t =>
                {
                    CursorRow = t.Row;
                    CursorColumn = t.Column;
                    Console.SetCursorPosition(CursorColumn - 1, CursorRow - 1);
                };
        }
        public static int CursorRow { get; private set; }
        public static int CursorColumn { get; private set; }

        public static void Render()
        {
                Console.Clear();
            
                void WriteWrapped(int col, int row, string text)
                {
                    int c = col, r = row;
                    while (!string.IsNullOrEmpty(text) && r < 24)
                    {
                        int space = 80 - c;
                        if (space <= 1) { c = 0; r++; continue; }
                        int take = text.Length <= space ? text.Length : space;
                        var part = text.Substring(0, take);
                        Console.SetCursorPosition(c, r);
                        Console.Write(part);
                        text = text.Substring(take);
                        c = 0; r++;
                    }
                }
            
                WriteWrapped(0, 0, "D133M13");
                WriteWrapped(8, 0, "");
                WriteWrapped(22, 0, "*** Seznam lansirane surovine na DN ***");
                WriteWrapped(62, 0, "");
                WriteWrapped(53, 1, "");
                WriteWrapped(0, 2, "Delovni nalog:");
                WriteWrapped(21, 2, "/");
                WriteWrapped(25, 2, "Ident:");
                WriteWrapped(39, 2, "");
                WriteWrapped(78, 2, "");
                WriteWrapped(1, 4, "Zš  Sarža     Kol  Dimenzija          Zš  Sarža     Kol Dimenzija");
                WriteWrapped(77, 4, "");
                WriteWrapped(1, 5, "---------------------------------------------------------------------------");
                WriteWrapped(78, 5, "");
                WriteWrapped(78, 6, "");
                WriteWrapped(78, 7, "");
                WriteWrapped(78, 8, "");
                WriteWrapped(78, 9, "");
                WriteWrapped(78, 10, "");
                WriteWrapped(78, 11, "");
                WriteWrapped(78, 12, "");
                WriteWrapped(78, 13, "");
                WriteWrapped(78, 14, "");
                WriteWrapped(78, 15, "");
                WriteWrapped(78, 16, "");
                WriteWrapped(78, 17, "");
                WriteWrapped(78, 18, "");
                WriteWrapped(78, 19, "");
                WriteWrapped(78, 20, "");
                WriteWrapped(79, 22, "");
                WriteWrapped(0, 23, "F3-nazaj");
                WriteWrapped(9, 23, "");
            
                WriteWrapped(15, 2, "");
                WriteWrapped(23, 2, "");
                WriteWrapped(32, 2, "");
                WriteWrapped(40, 2, "");
                WriteWrapped(2, 6, "");
                WriteWrapped(5, 6, "");
                WriteWrapped(14, 6, "");
                WriteWrapped(20, 6, "");
                WriteWrapped(40, 6, "");
                WriteWrapped(43, 6, "");
                WriteWrapped(52, 6, "");
                WriteWrapped(58, 6, "");
                WriteWrapped(2, 7, "");
                WriteWrapped(5, 7, "");
                WriteWrapped(14, 7, "");
                WriteWrapped(20, 7, "");
                WriteWrapped(40, 7, "");
                WriteWrapped(43, 7, "");
                WriteWrapped(52, 7, "");
                WriteWrapped(58, 7, "");
                WriteWrapped(2, 8, "");
                WriteWrapped(5, 8, "");
                WriteWrapped(14, 8, "");
                WriteWrapped(20, 8, "");
                WriteWrapped(40, 8, "");
                WriteWrapped(43, 8, "");
                WriteWrapped(52, 8, "");
                WriteWrapped(58, 8, "");
                WriteWrapped(2, 9, "");
                WriteWrapped(5, 9, "");
                WriteWrapped(14, 9, "");
                WriteWrapped(20, 9, "");
                WriteWrapped(40, 9, "");
                WriteWrapped(43, 9, "");
                WriteWrapped(52, 9, "");
                WriteWrapped(58, 9, "");
                WriteWrapped(2, 10, "");
                WriteWrapped(5, 10, "");
                WriteWrapped(14, 10, "");
                WriteWrapped(20, 10, "");
                WriteWrapped(40, 10, "");
                WriteWrapped(43, 10, "");
                WriteWrapped(52, 10, "");
                WriteWrapped(58, 10, "");
                WriteWrapped(2, 11, "");
                WriteWrapped(5, 11, "");
                WriteWrapped(14, 11, "");
                WriteWrapped(20, 11, "");
                WriteWrapped(40, 11, "");
                WriteWrapped(43, 11, "");
                WriteWrapped(52, 11, "");
                WriteWrapped(58, 11, "");
                WriteWrapped(2, 12, "");
                WriteWrapped(5, 12, "");
                WriteWrapped(14, 12, "");
                WriteWrapped(20, 12, "");
                WriteWrapped(40, 12, "");
                WriteWrapped(43, 12, "");
                WriteWrapped(52, 12, "");
                WriteWrapped(58, 12, "");
                WriteWrapped(2, 13, "");
                WriteWrapped(5, 13, "");
                WriteWrapped(14, 13, "");
                WriteWrapped(20, 13, "");
                WriteWrapped(40, 13, "");
                WriteWrapped(43, 13, "");
                WriteWrapped(52, 13, "");
                WriteWrapped(58, 13, "");
                WriteWrapped(2, 14, "");
                WriteWrapped(5, 14, "");
                WriteWrapped(14, 14, "");
                WriteWrapped(20, 14, "");
                WriteWrapped(40, 14, "");
                WriteWrapped(43, 14, "");
                WriteWrapped(52, 14, "");
                WriteWrapped(58, 14, "");
                WriteWrapped(2, 15, "");
                WriteWrapped(5, 15, "");
                WriteWrapped(14, 15, "");
                WriteWrapped(20, 15, "");
                WriteWrapped(40, 15, "");
                WriteWrapped(43, 15, "");
                WriteWrapped(52, 15, "");
                WriteWrapped(58, 15, "");
                WriteWrapped(2, 16, "");
                WriteWrapped(5, 16, "");
                WriteWrapped(14, 16, "");
                WriteWrapped(20, 16, "");
                WriteWrapped(40, 16, "");
                WriteWrapped(43, 16, "");
                WriteWrapped(52, 16, "");
                WriteWrapped(58, 16, "");
                WriteWrapped(2, 17, "");
                WriteWrapped(5, 17, "");
                WriteWrapped(14, 17, "");
                WriteWrapped(20, 17, "");
                WriteWrapped(40, 17, "");
                WriteWrapped(43, 17, "");
                WriteWrapped(52, 17, "");
                WriteWrapped(58, 17, "");
                WriteWrapped(2, 18, "");
                WriteWrapped(5, 18, "");
                WriteWrapped(14, 18, "");
                WriteWrapped(20, 18, "");
                WriteWrapped(40, 18, "");
                WriteWrapped(43, 18, "");
                WriteWrapped(52, 18, "");
                WriteWrapped(58, 18, "");
                WriteWrapped(2, 19, "");
                WriteWrapped(5, 19, "");
                WriteWrapped(14, 19, "");
                WriteWrapped(20, 19, "");
                WriteWrapped(40, 19, "");
                WriteWrapped(43, 19, "");
                WriteWrapped(52, 19, "");
                WriteWrapped(58, 19, "");
                WriteWrapped(2, 20, "");
                WriteWrapped(5, 20, "");
                WriteWrapped(14, 20, "");
                WriteWrapped(20, 20, "");
                WriteWrapped(40, 20, "");
                WriteWrapped(43, 20, "");
                WriteWrapped(52, 20, "");
                WriteWrapped(58, 20, "");
                WriteWrapped(0, 22, "");
            
                Console.SetCursorPosition(15, 2);
            
        }

        public static void SetClear()
        {
            foreach(var tag in Vfields)
            {
                tag.Value = string.Empty;
                tag.ClearModified();
                tag.SetNormal();
            }
            Render();
        }

        public static void CopyFrom(object record)
        {
            var props = record.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach(var tag in Vfields)
            {
                var p = props.FirstOrDefault(x => string.Equals(x.Name, tag.Name, StringComparison.OrdinalIgnoreCase));
                if(p==null) continue;
                tag.Value = p.GetValue(record)?.ToString() ?? string.Empty;
            }
            Render();
        }
    }
}
#endregion

    public static class Program
    {
        public static void Main()
        {
            // TODO: Write application logic here using the generated Global* classes.
            Console.WriteLine("ESF program initialized.");
        }
    }
}
