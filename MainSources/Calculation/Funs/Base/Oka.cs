using System.Runtime.InteropServices;

namespace Calculation
{
    //Работа с библиотекой Water Steam Pro
    public static class Oka
    {
        //Регистрация
        [DllImport("okawsp6.dll", EntryPoint = "wspLOCALREGISTRATIONEXA")]
        public static extern double Register(string name, int data);

        public static void Register()
        {
            Register("ZAOECUTE", 0x4EE24F71);
        }

        [DllImport("okawsp6.dll", EntryPoint = "wspSURFTENT")]
        public static extern double wspSURFTENT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspVPT")]
        public static extern double wspVPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspUPT")]
        public static extern double wspUPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspSPT")]
        public static extern double wspSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspHPT")]
        public static extern double wspHPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPPT")]
        public static extern double wspCPPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVPT")]
        public static extern double wspCVPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspWPT")]
        public static extern double wspWPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONPT")]
        public static extern double wspJOULETHOMPSONPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDPT")]
        public static extern double wspTHERMCONDPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISPT")]
        public static extern double wspDYNVISPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEPT")]
        public static extern double wspPRANDTLEPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISPT")]
        public static extern double wspKINVISPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKPT")]
        public static extern double wspKPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspVPTX")]
        public static extern double wspVPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspUPTX")]
        public static extern double wspUPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspSPTX")]
        public static extern double wspSPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspHPTX")]
        public static extern double wspHPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPPTX")]
        public static extern double wspCPPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVPTX")]
        public static extern double wspCVPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspWPTX")]
        public static extern double wspWPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONPTX")]
        public static extern double wspJOULETHOMPSONPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDPTX")]
        public static extern double wspTHERMCONDPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISPTX")]
        public static extern double wspDYNVISPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEPTX")]
        public static extern double wspPRANDTLEPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISPTX")]
        public static extern double wspKINVISPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspKPTX")]
        public static extern double wspKPTX(double P, double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspTPH")]
        public static extern double wspTPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspTPS")]
        public static extern double wspTPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspVPH")]
        public static extern double wspVPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspUPH")]
        public static extern double wspUPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspSPH")]
        public static extern double wspSPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPPH")]
        public static extern double wspCPPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVPH")]
        public static extern double wspCVPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspWPH")]
        public static extern double wspWPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONPH")]
        public static extern double wspJOULETHOMPSONPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDPH")]
        public static extern double wspTHERMCONDPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISPH")]
        public static extern double wspDYNVISPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEPH")]
        public static extern double wspPRANDTLEPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISPH")]
        public static extern double wspKINVISPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspKPH")]
        public static extern double wspKPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspVPS")]
        public static extern double wspVPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspUPS")]
        public static extern double wspUPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspHPS")]
        public static extern double wspHPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPPS")]
        public static extern double wspCPPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVPS")]
        public static extern double wspCVPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspWPS")]
        public static extern double wspWPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONPS")]
        public static extern double wspJOULETHOMPSONPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDPS")]
        public static extern double wspTHERMCONDPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISPS")]
        public static extern double wspDYNVISPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEPS")]
        public static extern double wspPRANDTLEPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISPS")]
        public static extern double wspKINVISPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspKPS")]
        public static extern double wspKPS(double P, double S);


        [DllImport("okawsp6.dll", EntryPoint = "wspTEXPANSIONPTPEFF")]
        public static extern double wspTEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspVEXPANSIONPTPEFF")]
        public static extern double wspVEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspUEXPANSIONPTPEFF")]
        public static extern double wspUEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspHEXPANSIONPTPEFF")]
        public static extern double wspHEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspSEXPANSIONPTPEFF")]
        public static extern double wspSEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPEXPANSIONPTPEFF")]
        public static extern double wspCPEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVEXPANSIONPTPEFF")]
        public static extern double wspCVEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspWEXPANSIONPTPEFF")]
        public static extern double wspWEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONEXPANSIONPTPEFF")]
        public static extern double wspJOULETHOMPSONEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDEXPANSIONPTPEFF")]
        public static extern double wspTHERMCONDEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISEXPANSIONPTPEFF")]
        public static extern double wspDYNVISEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEEXPANSIONPTPEFF")]
        public static extern double wspPRANDTLEEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISEXPANSIONPTPEFF")]
        public static extern double wspKINVISEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspKEXPANSIONPTPEFF")]
        public static extern double wspKEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);


        [DllImport("okawsp6.dll", EntryPoint = "wspTEXPANSIONPTXPEFF")]
        public static extern double wspTEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspVEXPANSIONPTXPEFF")]
        public static extern double wspVEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspUEXPANSIONPTXPEFF")]
        public static extern double wspUEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspHEXPANSIONPTXPEFF")]
        public static extern double wspHEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspSEXPANSIONPTXPEFF")]
        public static extern double wspSEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPEXPANSIONPTXPEFF")]
        public static extern double wspCPEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVEXPANSIONPTXPEFF")]
        public static extern double wspCVEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspWEXPANSIONPTXPEFF")]
        public static extern double wspWEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONEXPANSIONPTXPEFF")]
        public static extern double wspJOULETHOMPSONEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDEXPANSIONPTXPEFF")]
        public static extern double wspTHERMCONDEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISEXPANSIONPTXPEFF")]
        public static extern double wspDYNVISEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEEXPANSIONPTXPEFF")]
        public static extern double wspPRANDTLEEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISEXPANSIONPTXPEFF")]
        public static extern double wspKINVISEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspKEXPANSIONPTXPEFF")]
        public static extern double wspKEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspXEXPANSIONPTPEFF")]
        public static extern double wspXEXPANSIONPTPEFF(double P0, double T0, double P1, double Eff);

        [DllImport("okawsp6.dll", EntryPoint = "wspXEXPANSIONPTXPEFF")]
        public static extern double wspXEXPANSIONPTXPEFF(double P0, double T0, double X0, double P1, double Eff);


        [DllImport("okawsp6.dll", EntryPoint = "wspTHS")]
        public static extern double wspTHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspPHS")]
        public static extern double wspPHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspVHS")]
        public static extern double wspVHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspUHS")]
        public static extern double wspUHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPHS")]
        public static extern double wspCPHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVHS")]
        public static extern double wspCVHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspWHS")]
        public static extern double wspWHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONHS")]
        public static extern double wspJOULETHOMPSONHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDHS")]
        public static extern double wspTHERMCONDHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISHS")]
        public static extern double wspDYNVISHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEHS")]
        public static extern double wspPRANDTLEHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISHS")]
        public static extern double wspKINVISHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspKHS")]
        public static extern double wspKHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspXHS")]
        public static extern double wspXHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspXPH")]
        public static extern double wspXPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspXPS")]
        public static extern double wspXPS(double P, double S);

        //Функции свойств на линии насыщения.

        [DllImport("okawsp6.dll", EntryPoint = "wspPST")]
        public static extern double wspPST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspDPDTST")]
        public static extern double wspDPDTST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspTSP")]
        public static extern double wspTSP(double P);

        [DllImport("okawsp6.dll", EntryPoint = "wspVSST")]
        public static extern double wspVSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspVSWT")]
        public static extern double wspVSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspROUGHRSWT")]
        public static extern double wspROUGHRSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspUSST")]
        public static extern double wspUSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspUSWT")]
        public static extern double wspUSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspSSST")]
        public static extern double wspSSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspSSWT")]
        public static extern double wspSSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspHSST")]
        public static extern double wspHSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspHSWT")]
        public static extern double wspHSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPSST")]
        public static extern double wspCPSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPSWT")]
        public static extern double wspCPSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVSST")]
        public static extern double wspCVSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVSWT")]
        public static extern double wspCVSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVDPSST")]
        public static extern double wspCVDPSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVDPSWT")]
        public static extern double wspCVDPSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspWSST")]
        public static extern double wspWSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspWSWT")]
        public static extern double wspWSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONSST")]
        public static extern double wspJOULETHOMPSONSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONSWT")]
        public static extern double wspJOULETHOMPSONSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDSST")]
        public static extern double wspTHERMCONDSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDSWT")]
        public static extern double wspTHERMCONDSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISSST")]
        public static extern double wspDYNVISSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISSWT")]
        public static extern double wspDYNVISSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLESST")]
        public static extern double wspPRANDTLESST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLESWT")]
        public static extern double wspPRANDTLESWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISSST")]
        public static extern double wspKINVISSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISSWT")]
        public static extern double wspKINVISSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKSST")]
        public static extern double wspKSST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKSWT")]
        public static extern double wspKSWT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspRST")]
        public static extern double wspRST(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspROUGHHSSS")]
        public static extern double wspROUGHHSSS(double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspROUGHHSWS")]
        public static extern double wspROUGHHSWS(double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspTSHS")]
        public static extern double wspTSHS(double H, double S);


        //Функции свойств в двухфазной области: вода-пар.
        [DllImport("okawsp6.dll", EntryPoint = "wspVSTX")]
        public static extern double wspVSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspUSTX")]
        public static extern double wspUSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspSSTX")]
        public static extern double wspSSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspHSTX")]
        public static extern double wspHSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPSTX")]
        public static extern double wspCPSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVSTX")]
        public static extern double wspCVSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspWSTX")]
        public static extern double wspWSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONSTX")]
        public static extern double wspJOULETHOMPSONSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDSTX")]
        public static extern double wspTHERMCONDSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISSSTX")]
        public static extern double wspDYNVISSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLESTX")]
        public static extern double wspPRANDTLESTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISSTX")]
        public static extern double wspKINVISSTX(double T, double X);

        [DllImport("okawsp6.dll", EntryPoint = "wspKSTX")]
        public static extern double wspKSTX(double T, double X);


        //Функции свойств на линиях сублимации и плавления.
        [DllImport("okawsp6.dll", EntryPoint = "wspPSUBT")]
        public static extern double wspPSUBT(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspPMELTIT")]
        public static extern double wspPMELTIT(double T);

        //Функции для расчета свойств в метастабильной области (переохлажденный пар)
        [DllImport("okawsp6.dll", EntryPoint = "wspVMSPT")]
        public static extern double wspVMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspUMSPT")]
        public static extern double wspUMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspSMSPT")]
        public static extern double wspSMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspHMSPT")]
        public static extern double wspHMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCPMSPT")]
        public static extern double wspCPMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCVMSPT")]
        public static extern double wspCVMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspWMSPT")]
        public static extern double wspWMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSONMSPT")]
        public static extern double wspJOULETHOMPSONMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDMSPT")]
        public static extern double wspTHERMCONDMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISMSPT")]
        public static extern double wspDYNVISMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspPRANDTLEMSPT")]
        public static extern double wspPRANDTLEMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKINVISMSPT")]
        public static extern double wspKINVISMSPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspKMSPT")]
        public static extern double wspKMSPT(double P, double T);


        //Категория: Source
        [DllImport("okawsp6.dll", EntryPoint = "wspP23T")]
        public static extern double wspP23T(double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspT23P")]
        public static extern double wspT23P(double P);

        [DllImport("okawsp6.dll", EntryPoint = "wspWATERSTATEAREA")]
        public static extern int wspWATERSTATEAREA(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspWATERSTATEAREA2")]
        public static extern int wspWATERSTATEAREA2(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspTHERMCONDRT")]
        public static extern double wspTHERMCONDRT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspDYNVISRT")]
        public static extern double wspDYNVISRT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspV1PT")]
        public static extern double wspV1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspU1PT")]
        public static extern double wspU1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspS1PT")]
        public static extern double wspS1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspH1PT")]
        public static extern double wspH1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCP1PT")]
        public static extern double wspCP1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCV1PT")]
        public static extern double wspCV1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspW1PT")]
        public static extern double wspW1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSON1PT")]
        public static extern double wspJOULETHOMPSON1PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspV2PT")]
        public static extern double wspV2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspU2PT")]
        public static extern double wspU2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspS2PT")]
        public static extern double wspS2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspH2PT")]
        public static extern double wspH2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCP2PT")]
        public static extern double wspCP2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCV2PT")]
        public static extern double wspCV2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspW2PT")]
        public static extern double wspW2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSON2PT")]
        public static extern double wspJOULETHOMPSON2PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspV3RT")]
        public static extern double wspV3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspU3RT")]
        public static extern double wspU3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspS3RT")]
        public static extern double wspS3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspH3RT")]
        public static extern double wspH3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCP3RT")]
        public static extern double wspCP3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCV3RT")]
        public static extern double wspCV3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspW3RT")]
        public static extern double wspW3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSON3RT")]
        public static extern double wspJOULETHOMPSON3RT(double R, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspV3PT")]
        public static extern double wspV3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspU3PT")]
        public static extern double wspU3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspS3PT")]
        public static extern double wspS3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspH3PT")]
        public static extern double wspH3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCP3PT")]
        public static extern double wspCP3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCV3PT")]
        public static extern double wspCV3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspW3PT")]
        public static extern double wspW3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSON3PT")]
        public static extern double wspJOULETHOMPSON3PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspV5PT")]
        public static extern double wspV5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspU5PT")]
        public static extern double wspU5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspS5PT")]
        public static extern double wspS5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspH5PT")]
        public static extern double wspH5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCP5PT")]
        public static extern double wspCP5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspCV5PT")]
        public static extern double wspCV5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspW5PT")]
        public static extern double wspW5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspJOULETHOMPSON5PT")]
        public static extern double wspJOULETHOMPSON5PT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspT1PH")]
        public static extern double wspT1PH(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT1PS")]
        public static extern double wspT1PS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT1HS")]
        public static extern double wspT1HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspP1HS")]
        public static extern double wspP1HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2APH")]
        public static extern double wspT2APH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2APS")]
        public static extern double wspT2APS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2BPH")]
        public static extern double wspT2BPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2BPS")]
        public static extern double wspT2BPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2CPH")]
        public static extern double wspT2CPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2CPS")]
        public static extern double wspT2CPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2PH")]
        public static extern double wspT2PH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2PS")]
        public static extern double wspT2PS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT2HS")]
        public static extern double wspT2HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspP2HS")]
        public static extern double wspP2HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT3PH")]
        public static extern double wspT3PH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT3PS")]
        public static extern double wspT3PS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT3HS")]
        public static extern double wspT3HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspP3HS")]
        public static extern double wspP3HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspV3HS")]
        public static extern double wspV3HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspV3PH")]
        public static extern double wspV3PH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspV3PS")]
        public static extern double wspV3PS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT3RH")]
        public static extern double wspT3RH(double R, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT5PH")]
        public static extern double wspT5PH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspT5PS")]
        public static extern double wspT5PS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspP2B2CH")]
        public static extern double wspP2B2CH(double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspH2B2CP")]
        public static extern double wspH2B2CP(double P);

        [DllImport("okawsp6.dll", EntryPoint = "wspWATERSTATEAREAPH")]
        public static extern double wspWATERSTATEAREAPH(double P, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspWATERSTATEAREAPS")]
        public static extern double wspWATERSTATEAREAPS(double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspPHASESTATEPT")]
        public static extern int wspPHASESTATEPT(double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspWATERSTATEAREAHS")]
        public static extern double wspWATERSTATEAREAHS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspTB23HS")]
        public static extern double wspTB23HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspPB23HS")]
        public static extern double wspPB23HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspHB13S")]
        public static extern double wspHB13S(double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspT5HS")]
        public static extern double wspT5HS(double H, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspP5HS")]
        public static extern double wspP5HS(double H, double S);


        //Свойства газов
        [DllImport("okawsp6.dll", EntryPoint = "wspgCPIDT")]
        public static extern double wspgCPIDT(int Id, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgCPGST")]
        public static extern double wspgCPGST(string GS, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgHIDT")]
        public static extern double wspgHIDT(int Id, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgHGST")]
        public static extern double wspgHGST(string GS, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgSIDPT")]
        public static extern double wspgSIDPT(int Id, double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgSGSPT")]
        public static extern double wspgSGSPT(string GS, double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgCVIDT")]
        public static extern double wspgCVIDT(int Id, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgCVGST")]
        public static extern double wspgCVGST(string GS, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgUIDT")]
        public static extern double wspgUIDT(int Id, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgUGST")]
        public static extern double wspgUGST(string GS, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgMMID")]
        public static extern double wspgMMID(int Id);

        [DllImport("okawsp6.dll", EntryPoint = "wspgMMGS")]
        public static extern double wspgMMGS(string GS);

        [DllImport("okawsp6.dll", EntryPoint = "wspgGCID")]
        public static extern double wspgGCID(int Id);

        [DllImport("okawsp6.dll", EntryPoint = "wspgGCGS")]
        public static extern double wspgGCGS(string GS);

        [DllImport("okawsp6.dll", EntryPoint = "wspgVIDPT")]
        public static extern double wspgVIDPT(int Id, double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgVGSPT")]
        public static extern double wspgVGSPT(string GS, double P, double T);

        [DllImport("okawsp6.dll", EntryPoint = "wspgTIDH")]
        public static extern double wspgTIDH(int Id, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspgTGSH")]
        public static extern double wspgTGSH(string GS, double H);

        [DllImport("okawsp6.dll", EntryPoint = "wspgTIDPS")]
        public static extern double wspgTIDPS(int Id, double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspgTGSPS")]
        public static extern double wspgTGSPS(string GS, double P, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspgPIDTS")]
        public static extern double wspgPIDTS(int Id, double T, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspgPGSTS")]
        public static extern double wspgPGSTS(string GS, double T, double S);

        [DllImport("okawsp6.dll", EntryPoint = "wspgVFGSGS")]
        public static extern double wspgVFGSGS(string GS, string GS2);

        [DllImport("okawsp6.dll", EntryPoint = "wspgMFGSGS")]
        public static extern double wspgMFGSGS(string GS, string GS2);

        [DllImport("okawsp6.dll", EntryPoint = "wspgVFIDID")]
        public static extern double wspgVFIDID(int Id, int Id2);

        [DllImport("okawsp6.dll", EntryPoint = "wspgMFIDID")]
        public static extern double wspgMFIDID(int Id, int Id2);

        [DllImport("okawsp6.dll", EntryPoint = "wspSETMAXITERATION")]
        public static extern int wspSETMAXITERATION(int maxiteration);

        [DllImport("okawsp6.dll", EntryPoint = "wspGETMAXITERATION")]
        public static extern int wspGETMAXITERATION();

        [DllImport("okawsp6.dll", EntryPoint = "wspGETLASTERROR")]
        public static extern int wspGETLASTERROR();

        [DllImport("okawsp6.dll", EntryPoint = "wspgSETCALCDISSMODE")]
        public static extern int wspgSETCALCDISSMODE(int errorcode);

        [DllImport("okawsp6.dll", EntryPoint = "wspgGETCALCDISSMODE")]
        public static extern int wspgGETCALCDISSMODE();

        [DllImport("okawsp6.dll", EntryPoint = "wspSETTOLERANCE")]
        public static extern int wspSETTOLERANCE(double tolerance);

        [DllImport("okawsp6.dll", EntryPoint = "wspGETTOLERANCE")]
        public static extern int wspGETTOLERANCE();
    }
}