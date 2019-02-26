using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using System.Reflection;
using System.Globalization;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace CalculatePilotFrequency
{
    class QAMCommander
    {
        [DllImport("kernel32.dll")]
        static extern long Sleep(int dwMilliseconds);

        SerialPort qport = new SerialPort();
        SerialPort gport = new SerialPort();

        private static char[] HexData = { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', 'A', 'B', 'C', 'D', 'E', 'F' };
        byte[] DataBuf = new byte[1];       

        public bool InitializeQAMPort()
        {
            string mystr = "";
            qport.PortName = GlobalVars.PorttoUse;
            qport = new SerialPort(qport.PortName, 2000000);  //115200,230400
            qport.ReadTimeout = 2000;
            qport.WriteTimeout = 2000;
            qport.NewLine = "\r\n";
            qport.DataBits = 8;
            qport.ReadBufferSize = 25000;
            qport.Parity = Parity.None;
            qport.StopBits = StopBits.One;
            qport.Handshake = Handshake.RequestToSend;
            qport.DtrEnable = true;
            try
            {

                qport.Open();                
                return true;
            }
            catch (Exception ex)
            {
                mystr = ex.Message;
                return false;
            }
        }
        public Int32 GetAtoI(byte val)
        {
            Int32 tmp = 0;
            if (val > '9')
                val = Convert.ToByte((val - 'A') + 10);
            else
                val = Convert.ToByte(val - '0');

            tmp = Convert.ToInt32(val);
            return (tmp);
        }

        /** reads a byte in from the serial port  **/
        byte ReadIt()
        {
            byte RealVal;
            if (qport.BytesToRead > 0)
                RealVal = Convert.ToByte(qport.ReadByte());
            else
                RealVal = 0;
            return (RealVal);
        }

        int CheckForPrompt()
        {

            int val;
            int tries = 0;
            while (tries++ < 250)
            {
                val = ReadIt();
                if (val == '>')
                    return (1);
                else if (val == '?')
                {
                    GlobalVars.QAMCOMMERROR = 1;
                    return (-1);
                }


            }
            if (tries >= 250)
            {
                GlobalVars.QAMCOMMERROR = 1;

            }

            return (1);
        }





        public void CloseQAMPort()
        {
            qport.Close();
        }

        public void SendIt(char[] val)
        {
            qport.Write(val, 0, 1);
        }

        private void SetDataBuf(char val)
        {
            char[] DataBuf = new char[1];
            DataBuf[0] = val;
            SendIt(DataBuf);
        }
        private void SetUserValues()
        {
            byte ModeVal = 0x00;
            int done = 0;


            ModeVal |= Convert.ToByte(GlobalVars.UnitsofMeasure << 1);
            ModeVal |= Convert.ToByte(GlobalVars.DistanceUnits & 0x0F);
            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();
                SetDataBuf('B');
                SetDataBuf(HexData[(ModeVal >> 4) & 0x0F]);
                SetDataBuf(HexData[ModeVal & 0x0F]);
                SetDataBuf(HexData[(GlobalVars.PeakHold >> 4) & 0x0F]);
                SetDataBuf(HexData[(GlobalVars.PeakHold & 0x0F)]);
                SetDataBuf('.');
                done = CheckForPrompt();

            }

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();


        }
        private void SendTheMod(int which, uint Fr)
        {
            int done = 0;

            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();
                SetDataBuf('E');

                SetDataBuf(HexData[(which & 0x0F)]);
                SetDataBuf(HexData[(Fr >> 8) & 0x00000F]);
                SetDataBuf(HexData[(Fr >> 4) & 0x00000F]);
                SetDataBuf(HexData[(Fr & 0x00000F)]);

                SetDataBuf('.');
                done = CheckForPrompt();

            }

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();
        }
        private void SetMarker()
        {
            int i = 0;


            for (i = 0; i < 3; i++)
            {
                if (i == 0)
                    SendTheMod(1, GlobalVars.Marker1Mod);
                Sleep(500);
                if (i == 1)
                    SendTheMod(2, GlobalVars.Marker2Mod);
                Sleep(500);
                if (i == 2)
                    SendTheMod(3, GlobalVars.Marker3Mod);
            }

        }
        private void SetFreq(uint freq)
        {
            int done = 0;

            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();
                SetDataBuf('F');
                SetDataBuf(HexData[(freq >> 28) & 0x0000000F]);
                SetDataBuf(HexData[(freq >> 24) & 0x0000000F]);
                SetDataBuf(HexData[(freq >> 20) & 0x0000000F]);
                SetDataBuf(HexData[(freq >> 16) & 0x0000000F]);
                SetDataBuf(HexData[(freq >> 12) & 0x0000000F]);
                SetDataBuf(HexData[(freq >> 8) & 0x0000000F]);
                SetDataBuf(HexData[(freq >> 4) & 0x0000000F]);
                SetDataBuf(HexData[(freq & 0x0000000F)]);

                SetDataBuf('.');
                done = CheckForPrompt();

            }

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();
        }


        private void SetFactoryValues()
        {
            byte FactVal = 0x00;
            byte SquelVol = 0x00;

            int done = 0;
            FactVal = Convert.ToByte((GlobalVars.MarkertoUse[1] + 1) << 6);
            FactVal |= Convert.ToByte((GlobalVars.Sensitivity + 1) << 4);
            FactVal |= Convert.ToByte((GlobalVars.DistanceVal) & 0x0F);
            SquelVol = Convert.ToByte(GlobalVars.SquelchIndex << 4);
            SquelVol |= Convert.ToByte(GlobalVars.VolumeIndex & 0x0F);


            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();
                SetDataBuf('G');
                SetDataBuf(HexData[(FactVal >> 4) & 0x0F]);
                SetDataBuf(HexData[(FactVal & 0x0F)]);
                SetDataBuf(HexData[(SquelVol >> 4) & 0x0F]);
                SetDataBuf(HexData[(SquelVol & 0x0F)]);

                SetDataBuf('.');
                done = CheckForPrompt();

            }
            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();


        }
        private void BreakByte(int modbyte)
        {
            GlobalVars.ReturnedUnitsofMeasure = (modbyte & 0x06) >> 1;
            GlobalVars.ReturnedDistanceUnits = modbyte & 0x01;


        }

        private void DoTelemetryRetrieve()
        {
            int done = 0;
            int i = 0;
            byte[] RetrievedVals = new byte[GlobalVars.CurrentCondensedTLMSize];
            byte[] sync = { 86, 165, 166, 149 };
            char[] tmp = new char[7];
            char[] tmp1 = new char[4];


            Telemetry.CondensedTelemetryFrame tlmStruct = new Telemetry.CondensedTelemetryFrame();


            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();


                SetDataBuf('L');
                SetDataBuf(HexData[1 & 0x0F]);
                SetDataBuf('.');
                Sleep(1500);
                while (qport.BytesToRead == 0)
                {
                }
                while (i < 4)
                {
                    RetrievedVals[i] = ReadIt();
                    if (RetrievedVals[i] == sync[i])
                        i++;
                    else
                        i = 0;
                }



                for (i = 4; i < RetrievedVals.Length; i++)
                {
                    RetrievedVals[i] = ReadIt();

                }
                // done =  CheckForPrompt();
                tlmStruct.Update(RetrievedVals);
                if (tlmStruct.cmdRespAccept == '>')
                    done = 1;
                else
                {
                    GlobalVars.QAMCOMMERROR = 1;
                    done = 1;
                }

            }

            GlobalVars.ReturnedFreqSetting[1] = Convert.ToUInt32(tlmStruct.CfgParams.CenterFreq);
            GlobalVars.ReturnedFreqSetting[0] = 0;
            GlobalVars.ReturnedMarkertoUse[1] = Convert.ToUInt32(tlmStruct.CfgParams.SelectedMarker);
            GlobalVars.ReturnedMarker1Mod = Convert.ToUInt32(tlmStruct.CfgParams.MarkerFreq1);
            GlobalVars.ReturnedMarker2Mod = Convert.ToUInt32(tlmStruct.CfgParams.MarkerFreq2);
            GlobalVars.ReturnedMarker3Mod = Convert.ToUInt32(tlmStruct.CfgParams.MarkerFreq3);
            GlobalVars.ReturnedUnitsofMeasure = Convert.ToInt32(tlmStruct.CfgParams.LevelUnits);
            GlobalVars.ReturnedDistanceUnits = Convert.ToInt32(tlmStruct.CfgParams.DistanceUnits);
            GlobalVars.ReturnedPeakHold = Convert.ToUInt32(tlmStruct.CfgParams.PeakHold);
            GlobalVars.ReturnedSensitivity = tlmStruct.Users.Sensitivity;
            GlobalVars.ReturnedDistanceVal = tlmStruct.Users.DistanceIndex;
            GlobalVars.ReturnedSerialNumber = Convert.ToUInt32(tlmStruct.CalParams.SerialNumber);
            GlobalVars.RetrieveDone = 1;
        }


        private void GetCalDate()
        {
            int done = 0;
            byte WhichDir = 0xF0;
            byte[] RetrievedVals = new byte[6];
            int MonthOne, MonthTwo, DayOne, DayTwo, YearOne, YearTwo;
            int AIndex = 0;
            int i = 0;
            int tries = 0;



            MonthOne = 0;
            MonthTwo = 0;
            DayOne = 0;
            DayTwo = 0;
            YearOne = 0;
            YearTwo = 0;

            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();
                SetDataBuf('R');
                SetDataBuf(HexData[(WhichDir >> 4) & 0x0F]);
                SetDataBuf(HexData[WhichDir & 0x0F]);
                SetDataBuf('.');

                while ((qport.BytesToRead == 0) && (tries++ < 100))
                {
                }
                if (tries >= 100)
                {
                    GlobalVars.QAMCOMMERROR = 1;
                    return;
                }
                for (i = 0; i < 6; i++)
                {

                    RetrievedVals[i] = ReadIt();
                    if ((GetAtoI(RetrievedVals[0]) != 0) && (GetAtoI(RetrievedVals[0]) != 1))
                        i = 0;

                }
                done = CheckForPrompt();
            }
            MonthOne = GetAtoI(RetrievedVals[AIndex++]);
            MonthTwo = GetAtoI(RetrievedVals[AIndex++]);
            GlobalVars.TheMonth = (MonthOne * 10) + MonthTwo;
            DayOne = GetAtoI(RetrievedVals[AIndex++]);
            DayTwo = GetAtoI(RetrievedVals[AIndex++]);
            GlobalVars.TheDay = (DayOne * 10) + DayTwo;
            YearOne = GetAtoI(RetrievedVals[AIndex++]);
            YearTwo = GetAtoI(RetrievedVals[AIndex++]);
            GlobalVars.TheYear = (YearOne * 10) + YearTwo;
            GlobalVars.TheYear += 2000;


            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();
        }

        void SendStopEvents(int stop)
        {
            int done = 0;
            while (done != 1)
            {
                qport.DiscardInBuffer();
                qport.DiscardOutBuffer();
                SetDataBuf('M');
                SetDataBuf(HexData[stop]);
                SetDataBuf(HexData[stop]);

                SetDataBuf('.');
                done = CheckForPrompt();
            }

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();

        }

        private void SetUserValuesDF()
        {
            string cfgstring = "";
            int i;

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();

            switch (GlobalVars.DevicetoUse)
            {
                case GlobalVars.DeviceTypeEnum.QSHADOWDF:
                    cfgstring = "CFG=";
                    cfgstring += GlobalVars.FreqSetting[0].ToString("0.000000") + ":" + GlobalVars.FreqSetting[1].ToString("0.000000") + ":" + Convert.ToString(GlobalVars.MarkertoUse[0] + 1) + ":" + Convert.ToString(GlobalVars.MarkertoUse[1] + 1) + ":";
                    cfgstring += Convert.ToString(GlobalVars.MonSquelch) + ":" + Convert.ToString(GlobalVars.MeasSquelch) + ":";
                    if (GlobalVars.UnitsofMeasure == 0)
                        cfgstring += "UVPM:";
                    /*else if (GlobalVars.UnitsofMeasure == 2)
                        cfgstring += "DBUV:";*/
                    else
                        cfgstring += "DBPM:";

                    cfgstring += GlobalVars.DefaultBand.ToString() + ":";
                    if (GlobalVars.MonitorToggleBands)
                        cfgstring += "Y";
                    else
                        cfgstring += "N";

                    if (GlobalVars.ReturnedFWVer >= 6)
                    {
                        cfgstring += ":" + Convert.ToString(GlobalVars.MMMSquelch) + ":";
                        cfgstring += (GlobalVars.BandEnabled[0] ? "Y" : "N") + ":" + (GlobalVars.M3Enabled[0] ? "Y" : "N") + ":" + (GlobalVars.MMMdualCorr ? "Y" : "N") + ":N" + ":";
                        cfgstring += (GlobalVars.BandEnabled[1] ? "Y" : "N") + ":" + (GlobalVars.M3Enabled[1] ? "Y" : "N") + ":" + (GlobalVars.MMMdualCorr ? "Y" : "N") + ":N";
                    }

                    qport.WriteLine(cfgstring);
                    Sleep(500);

                    //FACT=1500:01:01:04:1000:100:1000:0000000:10:2880000:090000:30000:200:82383700:001FFFFF:N
                    cfgstring = "FACT=" + Convert.ToString(GlobalVars.MonitorIntervalSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";
                    cfgstring += "01:01:04:1000:100:";
                    cfgstring += Convert.ToString(GlobalVars.BacklightTimeoutSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";
                    cfgstring += "0000000:";
                    cfgstring += Convert.ToString(GlobalVars.AdvSetSensitivity[(int)GlobalVars.DevicetoUse]) + ":" + Convert.ToString(GlobalVars.MonitorTimeoutMins[(int)GlobalVars.DevicetoUse] * 60 * 100) + ":";
                    cfgstring += Convert.ToString(GlobalVars.MeasureTimeoutMins[(int)GlobalVars.DevicetoUse] * 60 * 100) + ":" + Convert.ToString(GlobalVars.MuteTimeoutSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";
                    cfgstring += "100:82383700:001FFFFF:N";
                    

                    if (GlobalVars.ReturnedFWVer >= 6)
                    {
                        cfgstring += ":" + (GlobalVars.MeasureToggleBands ? "Y" : "N") + ":Y";
                    }
                    
                    qport.WriteLine(cfgstring);
                    Sleep(250);
                    break;

                case GlobalVars.DeviceTypeEnum.QCOMPASS:
                    cfgstring = "CFG=";
                    for (i = 0; i < 3; i++)
                    {
                        cfgstring += GlobalVars.FreqSetting[i].ToString("0.000000") + ":" + Convert.ToString(GlobalVars.MarkertoUse[i] + 1) + ":" + (GlobalVars.BandEnabled[i] ? "Y" : "N") + ":";
                    }
                    if (GlobalVars.ReturnedFWVer >= 5)
                        cfgstring += GlobalVars.M3FreqSetting[2].ToString("0.000000");
                    else
                        cfgstring = cfgstring.Remove(cfgstring.Length - 1);
                    cfgstring += "\r"; 
                    qport.WriteLine(cfgstring);
                    Sleep(500);

                    //IFSET=0:1:0020:0020:0020:00:0000:1000:10:2880000:0060000:30000:UVPM:F
                    cfgstring = "IFSET=" + Convert.ToString(GlobalVars.DefaultMode) + ":" + Convert.ToString(GlobalVars.DefaultBand) + ":";                    
                    cfgstring += Convert.ToString(GlobalVars.MonSquelch) + ":" + Convert.ToString(GlobalVars.MeasSquelch) + ":" + Convert.ToString(GlobalVars.MMMSquelch) + ":";
                    //cfgstring += "00:0000:1000:10:2880000:0060000:30000:";
                    cfgstring += Convert.ToString(GlobalVars.AdvSetPeakHold[(int)GlobalVars.DevicetoUse]) + ":";
                    cfgstring += Convert.ToString(GlobalVars.MonitorIntervalSecs[(int)GlobalVars.DevicetoUse] * 100) + ":" + Convert.ToString(GlobalVars.BacklightTimeoutSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";
                    cfgstring += Convert.ToString(GlobalVars.AdvSetSensitivity[(int)GlobalVars.DevicetoUse]) + ":" + Convert.ToString(GlobalVars.MonitorTimeoutMins[(int)GlobalVars.DevicetoUse] * 60 * 100) + ":";
                    cfgstring += Convert.ToString(GlobalVars.MeasureTimeoutMins[(int)GlobalVars.DevicetoUse] * 60 * 100) + ":" + Convert.ToString(GlobalVars.MuteTimeoutSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";

                    if (GlobalVars.UnitsofMeasure == 0)
                        cfgstring += "UVPM:";
                    /*else if (GlobalVars.UnitsofMeasure == 2)
                        cfgstring += "DBUV:";*/
                    else
                        cfgstring += "DBPM:";

                    if (GlobalVars.AdvSetDistanceUnits[(int)GlobalVars.DevicetoUse] == 0)
                        cfgstring += "F";
                    else
                        cfgstring += "M";
                    qport.WriteLine(cfgstring);
                    Sleep(250);

                    if ((GlobalVars.AllowOFDM) && (GlobalVars.ReturnedFWVer > 5))
                    {
                        cfgstring = "OFDM=";
                        for (i = 0; i < 3; i++)
                        {
                            cfgstring += GlobalVars.PilotFreqSetting[0, i].ToString("0.000000") + ":" + GlobalVars.PilotFreqSetting[1, i].ToString("0.000000") + ":" + (GlobalVars.OFDMenabled[i] ? "Y" : "N") + ":" + GlobalVars.PilotLevelAdjust[0, i].ToString("0.0") + ":" + GlobalVars.PilotLevelAdjust[1, i].ToString("0.0") + ":" + GlobalVars.OFDMOffset[i].ToString("0.0") + ":";
                        }
                        cfgstring = cfgstring.Remove(cfgstring.Length - 1) + "\r";
                        qport.WriteLine(cfgstring);
                        Sleep(500);
                    }

                    break;

                case GlobalVars.DeviceTypeEnum.QSDF:
                default:
                    cfgstring = "IFSET=";
                    for (i = 0; i < 7; i++)
                    {
                        cfgstring += GlobalVars.SquelchSettings[i].ToString("0000") + ":";
                    }
                    cfgstring += GlobalVars.PeakHold.ToString("00") + ":";

                    if (GlobalVars.ReturnedFWVer >= 2)
                    {
                        //cfgstring += "1000:"; //Backlight10msTicks
                        cfgstring += Convert.ToString(GlobalVars.BacklightTimeoutSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";
                    }

                    if (GlobalVars.UnitsofMeasure == 0)
                        cfgstring += "UVPM:";
                    /*else if (GlobalVars.UnitsofMeasure == 2)
                        cfgstring += "DBUV:";*/
                    else
                        cfgstring += "DBPM:";

                    if (GlobalVars.DistanceUnits == 0)
                        cfgstring += "F";
                    else
                        cfgstring += "M";

                    if (GlobalVars.ReturnedFWVer >= 4)
                    {
                        cfgstring += ":" + (GlobalVars.BandEnabled[0] ? "Y" : "N") + ":" + (GlobalVars.M3Enabled[0] ? "Y" : "N") + ":" + ":N"; //hardcoded "N" is OFDM
                        cfgstring += ":" + (GlobalVars.BandEnabled[1] ? "Y" : "N") + ":" + (GlobalVars.M3Enabled[1] ? "Y" : "N") + ":" + ":N"; //hardcoded "N" is OFDM
                        cfgstring += ":N"; //hardcoded "N" is M3 in dock
                    }

                    if (GlobalVars.ReturnedFWVer >= 7)
                    {
                        cfgstring += ":" + (GlobalVars.DockPowB1LowGain ? "Y" : "N");
                    }
                    qport.WriteLine(cfgstring);
                    Sleep(500);

                    cfgstring = "CFG=";
                    cfgstring += GlobalVars.FreqSetting[0].ToString("0.000000") + ":" + GlobalVars.FreqSetting[1].ToString("0.000000") + ":" + Convert.ToString(GlobalVars.MarkertoUse[0] + 1) + ":" + Convert.ToString(GlobalVars.MarkertoUse[1] + 1) + ":007";
                    qport.WriteLine(cfgstring);
                    Sleep(500);

                    if (GlobalVars.ReturnedFWVer >= 7)
                    {
                        cfgstring = "ATTACH=";
                        cfgstring += "-12.0:+00.0:+06.5:" + GlobalVars.BNCGain[(int)GlobalVars.DeviceTypeEnum.QSDF, 0].ToString("0.0") + ":" + GlobalVars.BNCGain[(int)GlobalVars.DeviceTypeEnum.QSDF,1].ToString("0.0");
                        qport.WriteLine(cfgstring);
                        Sleep(500);
                    }
                    
                    break;

                case GlobalVars.DeviceTypeEnum.QS3:
                    cfgstring = "CFG=";
                    for (i = 0; i < 3; i++)
                    {
                        cfgstring += GlobalVars.FreqSetting[i].ToString("0.000000") + ":" + GlobalVars.M3FreqSetting[i].ToString("0.000000") + ":" + Convert.ToString(GlobalVars.MarkertoUse[i] + 1) + ":" + (GlobalVars.BandEnabled[i] ? "Y" : "N") + ":";
                    }
                    cfgstring = cfgstring.Remove(cfgstring.Length - 1) + "\r";
                    qport.WriteLine(cfgstring);
                    Sleep(500);

                    //IFSET=0:1:0000:0005:0010:0020:0050:0100:1999:00:1000:2880000:UVPM:F
                    cfgstring = "IFSET=" + Convert.ToString(GlobalVars.DefaultMode) + ":" + Convert.ToString(GlobalVars.DefaultBand) + ":";
                    for (i = 0; i < 7; i++)
                    {
                        cfgstring += GlobalVars.SquelchSettings[i].ToString("0000") + ":";
                    }
                    cfgstring += GlobalVars.PeakHold.ToString("00") + ":";

                    cfgstring += Convert.ToString(GlobalVars.BacklightTimeoutSecs[(int)GlobalVars.DevicetoUse] * 100) + ":";
                    cfgstring += Convert.ToString(GlobalVars.MonitorTimeoutMins[(int)GlobalVars.DevicetoUse] * 60 * 100) + ":"; //monitor mode timeout is actually power down timeout

                    if (GlobalVars.UnitsofMeasure == 0)
                        cfgstring += "UVPM:";
                    /*else if (GlobalVars.UnitsofMeasure == 2)
                        cfgstring += "DBUV:";*/
                    else
                        cfgstring += "DBPM:";

                    if (GlobalVars.DistanceUnits == 0)
                        cfgstring += "F";
                    else
                        cfgstring += "M";
                    qport.WriteLine(cfgstring);
                    Sleep(250);

                    if ((GlobalVars.AllowOFDM) && (GlobalVars.ReturnedFWVer > 2))
                    {
                        cfgstring = "OFDM=";
                        for (i = 0; i < 3; i++)
                        {
                            cfgstring += GlobalVars.PilotFreqSetting[0, i].ToString("0.000000") + ":" + GlobalVars.PilotFreqSetting[1, i].ToString("0.000000") + ":" + (GlobalVars.OFDMenabled[i] ? "Y" : "N") + ":" + GlobalVars.PilotLevelAdjust[0, i].ToString("0.0") + ":" + GlobalVars.PilotLevelAdjust[1, i].ToString("0.0") + ":" + GlobalVars.OFDMOffset[i].ToString("0.0") + ":";
                        }
                        cfgstring = cfgstring.Remove(cfgstring.Length - 1) + "\r";
                        qport.WriteLine(cfgstring);
                        Sleep(500);
                    }

                    break;
            }
        }




        /*private void SetOffsets()
        {
            float[] cala;
            string CalAString = "";
            string line = "";
            string[] tokens;
            string value;
            int i;
            int timeout = 0;

            cala = new float[15];

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();


            qport.WriteLine("CALA_0?");
            Sleep(100);

            //CALA_0=0:0:0:0:0:0:0:0:0:0:0:0:0:0:0
            timeout = 20; //2 seconds;
            while ((qport.BytesToRead == 0) && (timeout-- > 0))
            {
                Sleep(100);
            }
            if (timeout == -1)
            {
                MessageBox.Show("No response from unit");
                return;
            }
            Sleep(100);
            try
            {
                line = qport.ReadLine();
            }
            catch
            {
                line = null;
                GlobalVars.QAMCOMMERROR = 1;
                return;
            }

            tokens = line.Split(':');
            if (tokens.Length == 15)
            {
                for (i = 0; i < 15; i++)
                {
                    value = tokens[i].Trim();
                    cala[i] = Convert.ToSingle(value);                    
                }
            }

            if (GlobalVars.AnalogEquiv)
            {
                if (cala[0] > -99 && cala[0] < -40) // -70 +- 30
                {
                    float adj = cala[0] - (-70);
                    for (i = 0; i < 15; i++)
                    {
                        cala[i] += GlobalVars.LevelAdjust - adj;
                    }
                    cala[0] = -70 + GlobalVars.LevelAdjust;  //flag to determine whether using offset or not.
                }
                else
                {
                    for (i = 0; i < 15; i++)
                    {
                        cala[i] += GlobalVars.LevelAdjust;
                    }
                    cala[0] = -70 + GlobalVars.LevelAdjust;  //flag to determine whether using offset or not.
                }
            }
            else
            {
                if (cala[0] > -99 && cala[0] < -40) // -70 +- 30
                {
                    float adj = cala[0] - (-70);
                    //restore original settings
                    for (i = 0; i < 15; i++)
                    {
                        cala[i] -= adj;
                    }
                    cala[0] = 0;  //flag to determine whether using offset or not.
                }
                else
                {
                    //leave original settings alone
                    return;
                }
            }


            for (i = 0; i < 15; i++)
            {
                CalAString += cala[i].ToString("0.00") + ":";
            }
            CalAString = CalAString.TrimEnd(':');
            CalAString = "CALA_0=" + CalAString + "\r";
            qport.WriteLine(CalAString);

            timeout = 20; //2 seconds;
            while ((qport.BytesToRead == 0) && (timeout-- > 0))
            {
                Sleep(100);
            }
            if (timeout == -1)
            {
                MessageBox.Show("No response from unit");
                return;
            }
            Sleep(100);

            line = qport.ReadLine() + qport.NewLine;
            if (line.Contains("OK"))
            {                
                GlobalVars.QAMCOMMERROR = 0;                
            }
            else if (line.Contains("ERROR"))
            {
                MessageBox.Show("Error setting level adjustment");
                GlobalVars.QAMCOMMERROR = 1;                
            }
        }*/


        private void SetOffsets()
        {
            string line = "";
            //string[] tokens;
            //string value;           
            int timeout = 0;
            float[] adj = new float[3] {0,0,0};

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();

            //read currnet ANA values
            /*
            qport.WriteLine("ANA?");
            Sleep(100);

            //ANA=00.0:00.0
            timeout = 20; //2 seconds;
            while ((qport.BytesToRead == 0) && (timeout-- > 0))
            {
                Sleep(100);
            }
            if (timeout == -1)
            {
                MessageBox.Show("No response from unit");
                return;
            }
            Sleep(100);
            try
            {
                line = qport.ReadLine();
            }
            catch
            {
                line = null;
                GlobalVars.QAMCOMMERROR = 1;
                return;
            }

            tokens = line.Split(':');
            if (tokens.Length == 2)
            {
                value = tokens[0].Trim();
                adjLF = Convert.ToSingle(value);
                value = tokens[1].Trim();
                adjHF = Convert.ToSingle(value);
            }
            */

            for (int i = 0;i < 3;i++)
            {
                if (GlobalVars.AnalogEquiv[i])
                {
                    adj[i] = GlobalVars.LevelAdjust[i];
                }
                else
                {
                    adj[i] = 0;
                }
            }


            switch (GlobalVars.DevicetoUse)
            {
                case GlobalVars.DeviceTypeEnum.QSDF:
                    line = "ANA=" + adj[0].ToString("00.0") + ":" + adj[1].ToString("00.0") + "\r";
                    break;
                case GlobalVars.DeviceTypeEnum.QCOMPASS:
                    line = "OFFSET=";
                    for (int i = 0; i< 3;i++)
                    {
                        line += adj[i].ToString("00.0") + ":" + GlobalVars.AntennaGain[(int)GlobalVars.DeviceTypeEnum.QCOMPASS,i].ToString() + ":"; 
                    }
                    line = line.Remove(line.Length-1) + "\r"; 
                    break;
                case GlobalVars.DeviceTypeEnum.QS3:
                    line = "OFFSET=";
                    for (int i = 0; i < 3; i++)
                    {
                        line += adj[i].ToString("00.0") + ":" + GlobalVars.IntAntennaGain[(int)GlobalVars.DeviceTypeEnum.QS3, i].ToString() + ":";
                        line += GlobalVars.AntennaGain[(int)GlobalVars.DeviceTypeEnum.QS3,i].ToString() + ":" + GlobalVars.NFPGain[(int)GlobalVars.DeviceTypeEnum.QS3,i].ToString() + ":";
                        line += GlobalVars.YagiGain[(int)GlobalVars.DeviceTypeEnum.QS3, i].ToString() + ":" + GlobalVars.BNCGain[(int)GlobalVars.DeviceTypeEnum.QS3, i].ToString() + ":";
                    }
                    line = line.Remove(line.Length - 1) + "\r";
                    break;
            }

            qport.WriteLine(line);

            timeout = 20; //2 seconds;
            while ((qport.BytesToRead == 0) && (timeout-- > 0))
            {
                Sleep(100);
            }
            if (timeout == -1)
            {
                MessageBox.Show("No response from unit");
                return;
            }
            Sleep(100);

            line = qport.ReadLine() + qport.NewLine;
            if (line.Contains("OK"))
            {                
                GlobalVars.QAMCOMMERROR = 0;                
            }
            else if (line.Contains("ERROR"))
            {
                MessageBox.Show("Error setting level adjustment");
                GlobalVars.QAMCOMMERROR = 1;                
            }
        }


        private void GetDFParams()
        {
            string line = "";
            string[] tokens;
            string value;
            int i;
            bool done = false;

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();            
            qport.WriteLine("ALLPARAMS?");
            Sleep(100);            

            //CFG=138.0000:612.0000:1:1:007
            //=lfreq:hfreq:lmarker:hmarker:minleak                                     
            while (line != null && !line.StartsWith("CFG="))
            {
                try
                {
                    line = qport.ReadLine();
                }
                catch
                {
                    line = null;
                }
            }

            //CFG=138.0000:612.0000:1:1:0000:0000:UVPM:0:Y (QAM Shadow)
            //LF:HF:LFMarker:HFMarker:MonSquelch:MeasSquelch:levunits:defaultband:monitortoggleband
            //CFG=138.0000:612.0000:1:1:007 (QAM Sniffer)
            if (line == null || !line.StartsWith("CFG="))
            {
                GlobalVars.QAMCOMMERROR = 1;
                return;
            }
            else
            {
                line = line.Remove(0, 4);
                tokens = line.Split(':');
                if (tokens.Length >= 9) //QAM Shadow or QAM Compass or QS3
                {
                    if (line.Contains("UV") || line.Contains("DB"))
                    {
                        //QAM Shadow
                        GlobalVars.DevicetoUse = GlobalVars.DeviceTypeEnum.QSHADOWDF;
                        value = tokens[0].Trim();
                        GlobalVars.ReturnedFreqSetting[0] = Convert.ToDouble(value);
                        value = tokens[1].Trim();
                        GlobalVars.ReturnedFreqSetting[1] = Convert.ToDouble(value);
                        value = tokens[2].Trim();
                        GlobalVars.ReturnedMarkertoUse[0] = Convert.ToUInt32(value) - 1;
                        value = tokens[3].Trim();
                        GlobalVars.ReturnedMarkertoUse[1] = Convert.ToUInt32(value) - 1;
                        value = tokens[4].Trim();
                        GlobalVars.ReturnedMonSquelch = Convert.ToUInt32(value);
                        value = tokens[5].Trim();
                        GlobalVars.ReturnedMeasSquelch = Convert.ToUInt32(value);
                        value = tokens[6].Trim();
                        if (value.Equals("DBPM"))
                            GlobalVars.ReturnedUnitsofMeasure = 1;
                        /*else if (value.Equals("DBUV"))
                            GlobalVars.ReturnedUnitsofMeasure = 2;*/
                        else //uV/m
                            GlobalVars.ReturnedUnitsofMeasure = 0;
                        value = tokens[7].Trim();
                        GlobalVars.ReturnedDefaultBand = Convert.ToUInt32(value);
                        value = tokens[8].Trim();
                        if (value.Equals("N"))
                            GlobalVars.ReturnedToggleMonBand = false;
                        else
                            GlobalVars.ReturnedToggleMonBand = true;

                        if (tokens.Length >= 18)
                        {
                            value = tokens[9].Trim();
                            GlobalVars.ReturnedMMMSquelch = Convert.ToUInt32(value);
                            value = tokens[10].Trim();
                            if (value.Equals("N"))
                                GlobalVars.ReturnedBandEnabled[0] = false;
                            else
                                GlobalVars.ReturnedBandEnabled[0] = true;
                            value = tokens[11].Trim();
                            if (value.Equals("N"))
                                GlobalVars.ReturnedM3Enabled[0] = false;
                            else
                                GlobalVars.ReturnedM3Enabled[0] = true;
                            value = tokens[12].Trim();
                            if (value.Equals("N"))
                                GlobalVars.ReturnedMMMdualCorr = false;
                            else
                                GlobalVars.ReturnedMMMdualCorr = true;

                            
                            value = tokens[14].Trim();
                            if (value.Equals("N"))
                                GlobalVars.ReturnedBandEnabled[1] = false;
                            else
                                GlobalVars.ReturnedBandEnabled[1] = true;
                            value = tokens[15].Trim();
                            if (value.Equals("N"))
                                GlobalVars.ReturnedM3Enabled[1] = false;
                            else
                                GlobalVars.ReturnedM3Enabled[1] = true;
                        }
                    }
                    else if (tokens.Length >= 12)
                    {
                        //QS3
                        GlobalVars.DevicetoUse = GlobalVars.DeviceTypeEnum.QS3;
                        value = tokens[0].Trim();
                        GlobalVars.ReturnedFreqSetting[0] = Convert.ToDouble(value);
                        value = tokens[1].Trim();
                        GlobalVars.ReturnedM3FreqSetting[0] = Convert.ToDouble(value);
                        value = tokens[2].Trim();
                        GlobalVars.ReturnedMarkertoUse[0] = Convert.ToUInt32(value) - 1;
                        value = tokens[3].Trim();
                        GlobalVars.ReturnedBandEnabled[0] = value.Equals("N") ? false : true;

                        value = tokens[4].Trim();
                        GlobalVars.ReturnedFreqSetting[1] = Convert.ToDouble(value);
                        value = tokens[5].Trim();
                        GlobalVars.ReturnedM3FreqSetting[1] = Convert.ToDouble(value);
                        value = tokens[6].Trim();
                        GlobalVars.ReturnedMarkertoUse[1] = Convert.ToUInt32(value) - 1;
                        value = tokens[7].Trim();
                        GlobalVars.ReturnedBandEnabled[1] = value.Equals("N") ? false : true;

                        value = tokens[8].Trim();
                        GlobalVars.ReturnedFreqSetting[2] = Convert.ToDouble(value);
                        value = tokens[9].Trim();
                        GlobalVars.ReturnedM3FreqSetting[2] = Convert.ToDouble(value);
                        value = tokens[10].Trim();
                        GlobalVars.ReturnedMarkertoUse[2] = Convert.ToUInt32(value) - 1;
                        value = tokens[11].Trim();
                        GlobalVars.ReturnedBandEnabled[2] = value.Equals("N") ? false : true;
                    }
                    else
                    {
                        //QAM Compass
                        GlobalVars.DevicetoUse = GlobalVars.DeviceTypeEnum.QCOMPASS;
                        value = tokens[0].Trim();
                        GlobalVars.ReturnedFreqSetting[0] = Convert.ToDouble(value);
                        value = tokens[1].Trim();
                        GlobalVars.ReturnedMarkertoUse[0] = Convert.ToUInt32(value) - 1;
                        value = tokens[2].Trim();
                        GlobalVars.ReturnedBandEnabled[0] = value.Equals("N") ? false : true;
                        value = tokens[3].Trim();
                        GlobalVars.ReturnedFreqSetting[1] = Convert.ToDouble(value);
                        value = tokens[4].Trim();
                        GlobalVars.ReturnedMarkertoUse[1] = Convert.ToUInt32(value) - 1;
                        value = tokens[5].Trim();
                        GlobalVars.ReturnedBandEnabled[1] = value.Equals("N") ? false : true;
                        value = tokens[6].Trim();
                        GlobalVars.ReturnedFreqSetting[2] = Convert.ToDouble(value);
                        value = tokens[7].Trim();
                        GlobalVars.ReturnedMarkertoUse[2] = Convert.ToUInt32(value) - 1;
                        value = tokens[8].Trim();
                        GlobalVars.ReturnedBandEnabled[2] = value.Equals("N") ? false : true;
                        if (tokens.Length >= 10)
                        {
                            value = tokens[9].Trim();
                            GlobalVars.ReturnedM3FreqSetting[2] = Convert.ToDouble(value);
                        }
                    }
                }
                else if (tokens.Length >= 4)
                {
                    //QAM Sniffer DF
                    GlobalVars.DevicetoUse = GlobalVars.DeviceTypeEnum.QSDF;
                    value = tokens[0].Trim();
                    GlobalVars.ReturnedFreqSetting[0] = Convert.ToDouble(value);
                    value = tokens[1].Trim();
                    GlobalVars.ReturnedFreqSetting[1] = Convert.ToDouble(value);
                    value = tokens[2].Trim();
                    GlobalVars.ReturnedMarkertoUse[0] = Convert.ToUInt32(value) - 1;
                    value = tokens[3].Trim();
                    GlobalVars.ReturnedMarkertoUse[1] = Convert.ToUInt32(value) - 1;
                }
            }

            do
            {
                try
                {
                    line = qport.ReadLine();
                }
                catch
                {
                    line = null;
                    break;
                }

                if (line != null)
                {
                    if (line.StartsWith("FACT="))
                    {
                        if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QSHADOWDF)
                        {
                            line = line.Remove(0, 5);
                            tokens = line.Split(':');
                            if (tokens.Length >= 16)
                            {
                                value = tokens[0].Trim();
                                GlobalVars.ReturnedMonitorInterval_10ms = Convert.ToUInt32(value);
                                value = tokens[6].Trim();
                                GlobalVars.ReturnedBacklightTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[8].Trim();
                                GlobalVars.ReturnedAdvSetSensitivity = Convert.ToUInt32(value);
                                value = tokens[9].Trim();
                                GlobalVars.ReturnedMonitorTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[10].Trim();
                                GlobalVars.ReturnedMeasureTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[11].Trim();
                                GlobalVars.ReturnedMuteTimeout_10ms = Convert.ToUInt32(value);
                                if (tokens.Length >= 18)
                                {
                                    value = tokens[16].Trim();
                                    if (value.Equals("N"))
                                        GlobalVars.ReturnedMeasureToggleBands = false;
                                    else
                                        GlobalVars.ReturnedMeasureToggleBands = true;
                                }
                            }
                        }
                    }

                    if (line.StartsWith("IFSET="))
                    {
                        line = line.Remove(0, 6);
                        tokens = line.Split(':');

                        if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QSDF)
                        {
                            //IFSET=0000:0005:0010:0020:0050:0100:1999:00:UVPM:F                            
                            //=sq1:sq2:sq3:sq4:sq5:sq6:sq7:peakhold:unitsofmeasure:distanceunits                                
                            if (tokens.Length == 10)
                            {
                                for (i = 0; i < 7; i++)
                                {
                                    value = tokens[i].Trim();
                                    GlobalVars.ReturnedSquelchSettings[i] = Convert.ToUInt32(value);
                                }
                                value = tokens[7].Trim();
                                GlobalVars.ReturnedPeakHold = Convert.ToUInt32(value);
                                value = tokens[8].Trim();
                                if (value.Equals("DBPM"))
                                    GlobalVars.ReturnedUnitsofMeasure = 1;
                                /*else if (value.Equals("DBUV"))
                                    GlobalVars.ReturnedUnitsofMeasure = 2;*/
                                else //uV/m
                                    GlobalVars.ReturnedUnitsofMeasure = 0;

                                value = tokens[9].Trim();
                                if (value.Equals("M"))
                                    GlobalVars.ReturnedDistanceUnits = 1;
                                else //feet
                                    GlobalVars.ReturnedDistanceUnits = 0;
                            }
                            else if (tokens.Length > 10)
                            {
                                for (i = 0; i < 7; i++)
                                {
                                    value = tokens[i].Trim();
                                    GlobalVars.ReturnedSquelchSettings[i] = Convert.ToUInt32(value);
                                }
                                value = tokens[7].Trim();
                                GlobalVars.ReturnedPeakHold = Convert.ToUInt32(value);
                                value = tokens[8].Trim();
                                GlobalVars.ReturnedBacklightTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[9].Trim();
                                if (value.Equals("DBPM"))
                                    GlobalVars.ReturnedUnitsofMeasure = 1;
                                /*else if (value.Equals("DBUV"))
                                    GlobalVars.ReturnedUnitsofMeasure = 2;*/
                                else //uV/m
                                    GlobalVars.ReturnedUnitsofMeasure = 0;

                                value = tokens[10].Trim();
                                if (value.Equals("M"))
                                    GlobalVars.ReturnedDistanceUnits = 1;
                                else //feet
                                    GlobalVars.ReturnedDistanceUnits = 0;

                                if (tokens.Length >= 18)
                                {
                                    value = tokens[11].Trim();
                                    GlobalVars.ReturnedBandEnabled[0] = value.Equals("N") ? false : true;
                                    value = tokens[12].Trim();
                                    if (value.Equals("Y"))
                                        GlobalVars.ReturnedM3Enabled[0] = true;
                                    else
                                        GlobalVars.ReturnedM3Enabled[0] = false;

                                    value = tokens[14].Trim();
                                    GlobalVars.ReturnedBandEnabled[1] = value.Equals("N") ? false : true;
                                    value = tokens[15].Trim();
                                    if (value.Equals("Y"))
                                        GlobalVars.ReturnedM3Enabled[1] = true;
                                    else
                                        GlobalVars.ReturnedM3Enabled[1] = false;

                                    //if (value.Equals("Y"))
                                    //    GlobalVars.ReturnedM3InDock = true;
                                    //else
                                    //    GlobalVars.ReturnedM3InDock = false;
                                    if (tokens.Length >= 19)
                                    {
                                        value = tokens[18].Trim();
                                        if (value.Equals("Y"))
                                            GlobalVars.ReturnedDockPowB1LowGain = true;
                                        else
                                            GlobalVars.ReturnedDockPowB1LowGain = false;
                                    }
                                }
                            }
                        }
                        else if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QS3)
                        {
                            if (tokens.Length >= 14)
                            {
                                value = tokens[0].Trim();
                                GlobalVars.ReturnedDefaultMode = Convert.ToUInt32(value);
                                value = tokens[1].Trim();
                                GlobalVars.ReturnedDefaultBand = Convert.ToUInt32(value);
                                for (i = 0; i < 7; i++)
                                {
                                    value = tokens[2+i].Trim();
                                    GlobalVars.ReturnedSquelchSettings[i] = Convert.ToUInt32(value);
                                }
                                value = tokens[9].Trim();
                                GlobalVars.ReturnedPeakHold = Convert.ToUInt32(value);
                                value = tokens[10].Trim();
                                GlobalVars.ReturnedBacklightTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[11].Trim();
                                GlobalVars.ReturnedMonitorTimeout_10ms = Convert.ToUInt32(value);

                                value = tokens[12].Trim();
                                if (value.Equals("DBPM"))
                                    GlobalVars.ReturnedUnitsofMeasure = 1;
                                /*else if (value.Equals("DBUV"))
                                    GlobalVars.ReturnedUnitsofMeasure = 2;*/
                                else //uV/m
                                    GlobalVars.ReturnedUnitsofMeasure = 0;

                                value = tokens[13].Trim();
                                if (value.Equals("M"))
                                    GlobalVars.ReturnedDistanceUnits = 1;
                                else //feet
                                    GlobalVars.ReturnedDistanceUnits = 0;
                            }
                        }
                        else if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QCOMPASS)
                        {
                            if (tokens.Length >= 14)
                            {
                                value = tokens[0].Trim();
                                GlobalVars.ReturnedDefaultMode = Convert.ToUInt32(value);
                                value = tokens[1].Trim();
                                GlobalVars.ReturnedDefaultBand = Convert.ToUInt32(value);
                                value = tokens[2].Trim();
                                GlobalVars.ReturnedMonSquelch = Convert.ToUInt32(value);
                                value = tokens[3].Trim();
                                GlobalVars.ReturnedMeasSquelch = Convert.ToUInt32(value);
                                value = tokens[4].Trim();
                                GlobalVars.ReturnedMMMSquelch = Convert.ToUInt32(value);
                                value = tokens[5].Trim();
                                GlobalVars.ReturnedAdvSetPeakHold = Convert.ToUInt32(value);
                                value = tokens[6].Trim();
                                GlobalVars.ReturnedMonitorInterval_10ms = Convert.ToUInt32(value);
                                value = tokens[7].Trim();
                                GlobalVars.ReturnedBacklightTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[8].Trim();
                                GlobalVars.ReturnedAdvSetSensitivity = Convert.ToUInt32(value);
                                value = tokens[9].Trim();
                                GlobalVars.ReturnedMonitorTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[10].Trim();
                                GlobalVars.ReturnedMeasureTimeout_10ms = Convert.ToUInt32(value);
                                value = tokens[11].Trim();
                                GlobalVars.ReturnedMuteTimeout_10ms = Convert.ToUInt32(value);

                                value = tokens[12].Trim();
                                if (value.Equals("DBPM"))
                                    GlobalVars.ReturnedUnitsofMeasure = 1;
                                /*else if (value.Equals("DBUV"))
                                    GlobalVars.ReturnedUnitsofMeasure = 2;*/
                                else //uV/m
                                    GlobalVars.ReturnedUnitsofMeasure = 0;
                                value = tokens[13].Trim();
                                if (value.Equals("M"))
                                    GlobalVars.ReturnedDistanceUnits = 1;
                                else //feet
                                    GlobalVars.ReturnedDistanceUnits = 0;
                            }
                        }
                    }

                    if (line.StartsWith("FW="))
                    {
                        line = line.Remove(0, 3);
                        tokens = line.Split(':');
                        if (tokens.Length >= 1)
                        {
                            value = tokens[0].Trim();
                            GlobalVars.ReturnedFWVer = Convert.ToUInt32(value);
                        }                        
                    }

                    if (line.StartsWith("CAL="))
                    {
                        //CAL=+0.0000:+0.0440:+55.9:+56.7:00006200:10/30/13:+195:+845
                        //=tempgainslopeLF:tempgainslope:caltempLF:caltemp:serial:caldate:LFFreqOffset:HFFreqOffset            
                        line = line.Remove(0, 4);
                        tokens = line.Split(':');
                        if ((GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QCOMPASS) || (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QS3))
                        {
                            if (tokens.Length >= 8)
                            {
                                value = tokens[0].Trim();
                                GlobalVars.ReturnedSerialNumber = Convert.ToInt32(value);
                            }

                        }
                        else
                        {
                            if (tokens.Length >= 8)
                            {
                                value = tokens[4].Trim();
                                GlobalVars.ReturnedSerialNumber = Convert.ToInt32(value);
                            }
                            if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QSHADOWDF) done = true;
                        }
                    }


                    if (line.StartsWith("CALF_"))
                    {
                        //CALF_0=136.000:136.250:136.500:136.750:137.000:137.250:137.500:137.750:138.000:138.250:138.500:138.500:138.500:138.500:138.500
                        UInt32 index = 0;
                        if (line.Length > 5)
                        {
                            string tmpstr = line.Substring(5, 1);
                            index = Convert.ToUInt32(tmpstr);
                        }
                        if (index < GlobalVars.NumBands)
                        {
                            line = line.Remove(0, 7);

                            tokens = line.Split(':');
                            if (tokens.Length >= 15)
                            {
                                //for (int i = 0; i < 15; i++)
                                {
                                    value = tokens[0].Trim();
                                    GlobalVars.ReturnedCalFreqStart[index] = Convert.ToDouble(value);
                                }
                            }
                        }
                    }

                    if (line.StartsWith("ATTACH=") || line == null)
                    {
                        //ATTACH=-12.0:+00.0:+06.5:+01.0:+02.0
                        //=NFPgain[138]:NFPgain[612]:Yagigain:(BNCgain[138]):(BNCgain[612])
                        line = line.Remove(0, 7);
                        tokens = line.Split(':');
                        if (tokens.Length >= 5)
                        {
                            //Single tmp;
                            value = tokens[3].Trim();                            
                            GlobalVars.ReturnedBNCGain[0] = Convert.ToSingle(value);
                            value = tokens[4].Trim();
                            GlobalVars.ReturnedBNCGain[1] = Convert.ToSingle(value);
                        }
                    }


                    if (line.StartsWith("ANA=") || line == null)
                    {
                        //ANA=+00.0:+00.0
                        //=LFOffset:HFOffset
                        line = line.Remove(0, 4);
                        tokens = line.Split(':');
                        if (tokens.Length >= 2)
                        {
                            Single tmp;
                            value = tokens[0].Trim();
                            tmp = Convert.ToSingle(value);
                            if (tmp.CompareTo(-0.1f) < 0 || tmp.CompareTo(0.1f) > 0) GlobalVars.ReturnedAnalogEquiv[0] = true;
                            else GlobalVars.ReturnedAnalogEquiv[0] = false;
                            value = tokens[1].Trim();
                            tmp = Convert.ToSingle(value);
                            if (tmp.CompareTo(-0.1f) < 0 || tmp.CompareTo(0.1f) > 0) GlobalVars.ReturnedAnalogEquiv[1] = true;
                            else GlobalVars.ReturnedAnalogEquiv[1] = false;
                        }
                        if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QSDF) done = true;
                    }

                    if (line.StartsWith("OFFSET=") || line == null)
                    {
                        line = line.Remove(0, 7);
                        tokens = line.Split(':');
                        if (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QS3)
                        {
                            if (tokens.Length >= 18)
                            {
                                Single tmp;
                                int index = 0;
                                for (i = 0; i < GlobalVars.NumBands; i++)
                                {
                                    value = tokens[index++].Trim();
                                    tmp = Convert.ToSingle(value);
                                    if (tmp.CompareTo(-0.1f) < 0 || tmp.CompareTo(0.1f) > 0) GlobalVars.ReturnedAnalogEquiv[i] = true;
                                    else GlobalVars.ReturnedAnalogEquiv[i] = false;

                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedIntAntennaGain[i] = Convert.ToSingle(value);

                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedAntennaGain[i] = Convert.ToSingle(value);

                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedNFPGain[i] = Convert.ToSingle(value);

                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedYagiGain[i] = Convert.ToSingle(value);

                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedBNCGain[i] = Convert.ToSingle(value);
                                }
                            }
                        }
                        else
                        {
                            if (tokens.Length >= 6)
                            {
                                Single tmp;
                                int index = 0;
                                for (i = 0; i < GlobalVars.NumBands; i++)
                                {
                                    value = tokens[index++].Trim();
                                    tmp = Convert.ToSingle(value);
                                    if (tmp.CompareTo(-0.1f) < 0 || tmp.CompareTo(0.1f) > 0) GlobalVars.ReturnedAnalogEquiv[i] = true;
                                    else GlobalVars.ReturnedAnalogEquiv[i] = false;

                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedAntennaGain[i] = Convert.ToSingle(value);
                                }
                            }
                        }
                        //if ((GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QCOMPASS)||(GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QS3)) done = true;
                    }

                    if (line.StartsWith("OFDM=") || line == null)
                    {
                        line = line.Remove(0, 5);
                        tokens = line.Split(':');
                        if ((GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QS3) || (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QCOMPASS))
                        {
                            if (tokens.Length >= 18)
                            {
                                int index = 0;
                                for (i = 0; i < GlobalVars.NumBands; i++)
                                {
                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedPilotFreqSetting[0,i] = Convert.ToDouble(value);
                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedPilotFreqSetting[1,i] = Convert.ToDouble(value);
                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedOFDMenabled[i] = value.Equals("N") ? false : true;
                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedPilotLevelAdjust[0, i] = Convert.ToSingle(value);
                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedPilotLevelAdjust[1, i] = Convert.ToSingle(value);
                                    value = tokens[index++].Trim();
                                    GlobalVars.ReturnedOFDMOffset[i] = Convert.ToSingle(value);                                    
                                }
                            }
                        }
                        if ((GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QCOMPASS) || (GlobalVars.DevicetoUse == GlobalVars.DeviceTypeEnum.QS3)) done = true;
                    }
                }
            }
            while (line != null && !done);

            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();   
        }



        private void DoFreqCal()
        {
            string line = "";
            int timeout;

            GlobalVars.QAMCOMMERROR = 0;
            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();
            qport.WriteLine("AUTOFREQ");
            Sleep(100);

            timeout = 20; //2 seconds;
            while ((qport.BytesToRead == 0) && (timeout-- > 0))
            {
                Sleep(100);
            }
            if (timeout == -1)
            {
                MessageBox.Show("No response from unit");
                return;
            }
            Sleep(100);

            line = qport.ReadLine() + qport.NewLine;

            if (line.Contains("OK"))
            {
                timeout = 300; //30 seconds
                while ((qport.BytesToRead == 0) && (timeout-- > 0))
                {
                    Sleep(100);
                }
                if (timeout == -1)
                {
                    MessageBox.Show("No response from unit");
                    return;
                }
                line = qport.ReadLine() + qport.NewLine;
                if (line.Contains("found"))
                    MessageBox.Show("Frequency calibration successful!");
                else
                    MessageBox.Show("Frequency calibration failed.  Try increasing signal level.");
            }
            else //if (RetStr.Contains("ERROR"))
            {
                GlobalVars.QAMCOMMERROR = 1;
            }
        }

        private void InitializeDefaults()
        {
            string line = "";
            int timeout;

            GlobalVars.QAMCOMMERROR = 0;
            qport.DiscardInBuffer();
            qport.DiscardOutBuffer();
            qport.WriteLine("ALLPARAMS=DEFAULT");
            Sleep(100);

            timeout = 20; //2 seconds;
            while ((qport.BytesToRead == 0) && (timeout-- > 0))
            {
                Sleep(100);
            }
            if (timeout == -1)
            {
                MessageBox.Show("No response from unit");
                return;
            }
            Sleep(100);

            line = qport.ReadLine() + qport.NewLine;

            if (line.Contains("OK"))
            {               
               //MessageBox.Show("Settings initialized to default.");               
            }
            else //if (RetStr.Contains("ERROR"))
            {
                //MessageBox.Show("Failed to initialize defaults.");               
                GlobalVars.QAMCOMMERROR = 1;
            }
        }


        public void ReconCommandCentral(string TheCommand)
        {
            bool IsPortOpen;
            IsPortOpen = InitializeQAMPort();
            if (IsPortOpen == false)
            {
                GlobalVars.PortOpened = false;
                return;
            }
            else
                GlobalVars.PortOpened = true;


            GlobalVars.QAMCOMMERROR = 0;
            switch (TheCommand)
            {
                case "SetInternal":

                    /*
                    SetUserValues();
                    Sleep(500);
                    SetFreq(Convert.ToUInt32(GlobalVars.FreqSetting[1]*1000000));
                    Sleep(500);
                    // SetMarker();
                   // Sleep(500);
                    SetFactoryValues();
                    Sleep(500);
                     */
                    SetUserValuesDF();
                    Sleep(500);
                    GlobalVars.RetrieveDone = 1;
                    break;
                case "SetOffsets":
                    SetOffsets();
                    Sleep(500);
                    break;
                case "Dump":
                    //DoTelemetryRetrieve();
                    GetDFParams();
                    Sleep(500);
                    GlobalVars.RetrieveDone = 1;
                    break;
                case "GetCalDate":
                    GetCalDate();
                    Sleep(500);
                    GlobalVars.RetrieveDone = 1;
                    break;
                case "Stop":
                    SendStopEvents(0);
                    Sleep(500);
                    GlobalVars.RetrieveDone = 1;
                    break;
                case "Start":
                    SendStopEvents(1);
                    Sleep(500);
                    GlobalVars.RetrieveDone = 1;
                    break;
                case "FreqCal":
                    DoFreqCal();
                    Sleep(500);
                    break;
                case "Defaults":
                    InitializeDefaults();
                    Sleep(500);
                    break;

            }
            CloseQAMPort();
        }
    }
}
