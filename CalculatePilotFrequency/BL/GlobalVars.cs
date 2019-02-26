using System;
using System.Collections.Generic;
using System.Text;

namespace CalculatePilotFrequency
{
    class GlobalVars
    {
        public const Int32 NumBands = 3;
        public static Int32 CurrentTLMSize = 21993;
        public static Int32 CurrentCondensedTLMSize = 269;
        public static Int32 Units_Uvm = 0;
        public static Int32 Units_dbuv = 1;
        public static Int32 Units_dbuvm = 2;
        public static Int32 Units_feet = 0;
        public static Int32 Units_meters = 0;
        public static Int32 Marker1Selected = 0;
        public static Int32 Marker2Selected = 1;
        public static Int32 Marker3Selected = 2;
        public static Int32 VID = 0403;
        public static Int32 PID = 6001;
        public static bool AllowOFDM = true;

        public const float OFDMOffset4k = -18.208f;
        public const float OFDMOffset8k = -15.198f;


        public enum DeviceTypeEnum
        {
            QSDF=0,
            QSHADOWDF=1,
            QCOMPASS=2,
            QS3=3,
            NUMDEVICES=4
        };

        private static Int32 G_TheDay = 0;
        public static Int32 TheDay
        {
            get
            {
                return G_TheDay;
            }
            set
            {
                G_TheDay = value;
            }
        }
        private static Int32 G_TheMonth = 0;
        public static Int32 TheMonth
        {
            get
            {
                return G_TheMonth;
            }
            set
            {
                G_TheMonth = value;
            }
        }
        private static Int32 G_TheYear = 0;
        public static Int32 TheYear
        {
            get
            {
                return G_TheYear;
            }
            set
            {
                G_TheYear = value;
            }
        }

        // connection settings/status
        private static Int32 G_QAMCOMMERROR = 0;
        public static Int32 QAMCOMMERROR
        {
            get
            {
                return G_QAMCOMMERROR;
            }
            set
            {
                G_QAMCOMMERROR = value;
            }
        }
        private static bool G_PortOpened = false;
        public static bool PortOpened
        {
            get
            {
                return G_PortOpened;
            }
            set
            {
                G_PortOpened = value;
            }
        }
        private static string G_PorttoUse = "";
        public static string PorttoUse
        {
            get
            {
                return G_PorttoUse;
            }
            set
            {
                G_PorttoUse = value;
            }
        }
        private static DeviceTypeEnum G_DevicetoUse = DeviceTypeEnum.QSDF;
        public static DeviceTypeEnum DevicetoUse
        {
            get
            {
                return G_DevicetoUse;
            }
            set
            {
                G_DevicetoUse = value;
            }
        }
        private static Int32 G_CurrentConnectionState = 0; //0=disconnect, 1=connected
        public static Int32 CurrentConnectionState
        {
            get
            {
                return G_CurrentConnectionState;
            }
            set
            {
                G_CurrentConnectionState = value;
            }
        }
        private static Int32 G_CurrentDeviceState = 0; //0=no response, 1=response 
        public static Int32 CurrentDeviceState
        {
            get
            {
                return G_CurrentDeviceState;
            }
            set
            {
                G_CurrentDeviceState = value;
            }
        }
        private static UInt32 G_RetrieveDone = 0;
        public static UInt32 RetrieveDone
        {
            get
            {
                return G_RetrieveDone;
            }
            set
            {
                G_RetrieveDone = value;
            }
        }
        private static byte[] G_ReceivedBytes = new byte[CurrentTLMSize];
        public static byte[] ReceivedBytes
        {
            get
            {
                return G_ReceivedBytes;
            }
            set
            {
                G_ReceivedBytes = value;
            }

        }        

        // connected device info
        private static long G_ReturnedSerialNumber = 0;
        public static long ReturnedSerialNumber
        {
            get
            {
                return G_ReturnedSerialNumber;
            }
            set
            {
                G_ReturnedSerialNumber = value;
            }
        }
        private static double G_ReturnedFWVer = 0;
        public static double ReturnedFWVer
        {
            get
            {
                return G_ReturnedFWVer;
            }
            set
            {
                G_ReturnedFWVer = value;
            }

        }

        private static int G_DistanceUnits = 0;
        public static int DistanceUnits
        {
            get
            {
                return G_DistanceUnits;
            }
            set
            {
                G_DistanceUnits = value;
            }
        }
        private static int G_ReturnedDistanceUnits = 0;
        public static int ReturnedDistanceUnits
        {
            get
            {
                return G_ReturnedDistanceUnits;
            }
            set
            {
                G_ReturnedDistanceUnits = value;
            }
        }
        private static int G_UnitsofMeasure = 0;
        public static int UnitsofMeasure
        {
            get
            {
                return G_UnitsofMeasure;
            }
            set
            {
                G_UnitsofMeasure = value;
            }
        }
        private static int G_ReturnedUnitsofMeasure = 0;
        public static int ReturnedUnitsofMeasure
        {
            get
            {
                return G_ReturnedUnitsofMeasure;
            }
            set
            {
                G_ReturnedUnitsofMeasure = value;
            }
        }

        private static bool G_ReturnedToggleMonBand = true;
        public static bool ReturnedToggleMonBand
        {
            get
            {
                return G_ReturnedToggleMonBand;
            }
            set
            {
                G_ReturnedToggleMonBand = value;
            }
        }

        private static UInt32 G_PeakHold = 1;
        public static UInt32 PeakHold
        {
            get
            {
                return G_PeakHold;
            }
            set
            {
                G_PeakHold = value;
            }
        }
        private static UInt32 G_ReturnedPeakHold = 0;
        public static UInt32 ReturnedPeakHold
        {
            get
            {
                return G_ReturnedPeakHold;
            }
            set
            {
                G_ReturnedPeakHold = value;
            }
        }

        private static UInt32[] G_MarkertoUse = new UInt32[NumBands] { 0, 0, 0 };
        public static UInt32[] MarkertoUse
        {
            get
            {
                return G_MarkertoUse;
            }
            set
            {
                G_MarkertoUse = value;
            }
        }
        private static UInt32[] G_ReturnedMarkertoUse = new UInt32[NumBands] { 0, 0, 0 };
        public static UInt32[] ReturnedMarkertoUse
        {
            get
            {
                return G_ReturnedMarkertoUse;
            }
            set
            {
                G_ReturnedMarkertoUse = value;
            }
        }

        private static UInt32 G_Marker1Mod = 1283;
        public static UInt32 Marker1Mod
        {
            get
            {
                return G_Marker1Mod;
            }
            set
            {
                G_Marker1Mod = value;
            }
        }
        private static UInt32 G_ReturnedMarker1Mod = 0;
        public static UInt32 ReturnedMarker1Mod
        {
            get
            {
                return G_ReturnedMarker1Mod;
            }
            set
            {
                G_ReturnedMarker1Mod = value;
            }
        }
        private static UInt32 G_Marker2Mod = 1511;
        public static UInt32 Marker2Mod
        {
            get
            {
                return G_Marker2Mod;
            }
            set
            {
                G_Marker2Mod = value;
            }
        }
        private static UInt32 G_ReturnedMarker2Mod = 0;
        public static UInt32 ReturnedMarker2Mod
        {
            get
            {
                return G_ReturnedMarker2Mod;
            }
            set
            {
                G_ReturnedMarker2Mod = value;
            }
        }
        private static UInt32 G_Marker3Mod = 1663;
        public static UInt32 Marker3Mod
        {
            get
            {
                return G_Marker3Mod;
            }
            set
            {
                G_Marker3Mod = value;
            }
        }
        private static UInt32 G_ReturnedMarker3Mod = 0;
        public static UInt32 ReturnedMarker3Mod
        {
            get
            {
                return G_ReturnedMarker3Mod;
            }
            set
            {
                G_ReturnedMarker3Mod = value;
            }
        }

        private static int G_Sensitivity = 0;
        public static int Sensitivity
        {
            get
            {
                return G_Sensitivity;
            }
            set
            {
                G_Sensitivity = value;
            }
        }
        private static UInt32 G_ReturnedSensitivity = 1;
        public static UInt32 ReturnedSensitivity
        {
            get
            {
                return G_ReturnedSensitivity;
            }
            set
            {
                G_ReturnedSensitivity = value;
            }
        }
        private static int G_DistanceVal = 1;
        public static int DistanceVal
        {
            get
            {
                return G_DistanceVal;
            }
            set
            {
                G_DistanceVal = value;
            }
        }
        private static UInt32 G_ReturnedDistanceVal = 0;
        public static UInt32 ReturnedDistanceVal
        {
            get
            {
                return G_ReturnedDistanceVal;
            }
            set
            {
                G_ReturnedDistanceVal = value;
            }
        }

        private static UInt32 G_MonSquelch = 50;
        public static UInt32 MonSquelch
        {
            get
            {
                return G_MonSquelch;
            }
            set
            {
                G_MonSquelch = value;
            }
        }

        private static UInt32 G_ReturnedMonSquelch = 0;
        public static UInt32 ReturnedMonSquelch
        {
            get
            {
                return G_ReturnedMonSquelch;
            }
            set
            {
                G_ReturnedMonSquelch = value;
            }
        }

        private static UInt32 G_MeasSquelch = 0;
        public static UInt32 MeasSquelch
        {
            get
            {
                return G_MeasSquelch;
            }
            set
            {
                G_MeasSquelch = value;
            }
        }

        private static UInt32 G_ReturnedMeasSquelch = 0;
        public static UInt32 ReturnedMeasSquelch
        {
            get
            {
                return G_ReturnedMeasSquelch;
            }
            set
            {
                G_ReturnedMeasSquelch = value;
            }
        }

        private static UInt32 G_MMMSquelch = 0;
        public static UInt32 MMMSquelch
        {
            get
            {
                return G_MMMSquelch;
            }
            set
            {
                G_MMMSquelch = value;
            }
        }

        private static UInt32 G_ReturnedMMMSquelch = 0;
        public static UInt32 ReturnedMMMSquelch
        {
            get
            {
                return G_ReturnedMMMSquelch;
            }
            set
            {
                G_ReturnedMMMSquelch = value;
            }
        }

        private static UInt32 G_DefaultBand = 1;
        public static UInt32 DefaultBand
        {
            get
            {
                return G_DefaultBand;
            }
            set
            {
                G_DefaultBand = value;
            }
        }
        private static UInt32 G_ReturnedDefaultBand = 0;
        public static UInt32 ReturnedDefaultBand
        {
            get
            {
                return G_ReturnedDefaultBand;
            }
            set
            {
                G_ReturnedDefaultBand = value;
            }
        }

        private static UInt32 G_DefaultMode = 1;
        public static UInt32 DefaultMode
        {
            get
            {
                return G_DefaultMode;
            }
            set
            {
                G_DefaultMode = value;
            }
        }
        private static UInt32 G_ReturnedDefaultMode = 0;
        public static UInt32 ReturnedDefaultMode
        {
            get
            {
                return G_ReturnedDefaultMode;
            }
            set
            {
                G_ReturnedDefaultMode = value;
            }
        }

        private static bool G_MonitorToggleBands = true;
        public static bool MonitorToggleBands
        {
            get
            {
                return G_MonitorToggleBands;
            }
            set
            {
                G_MonitorToggleBands = value;
            }
        }

        private static bool G_MeasureToggleBands = true;
        public static bool MeasureToggleBands
        {
            get
            {
                return G_MeasureToggleBands;
            }
            set
            {
                G_MeasureToggleBands = value;
            }
        }

        private static bool G_ReturnedMeasureToggleBands = true;
        public static bool ReturnedMeasureToggleBands
        {
            get
            {
                return G_ReturnedMeasureToggleBands;
            }
            set
            {
                G_ReturnedMeasureToggleBands = value;
            }
        }

        private static bool[] G_BandEnabled = new bool[NumBands] { true, true, false };
        public static bool[] BandEnabled
        {
            get
            {
                return G_BandEnabled;
            }
            set
            {
                G_BandEnabled = value;
            }
        }

        private static bool[] G_ReturnedBandEnabled = new bool[NumBands] { true, true, false };
        public static bool[] ReturnedBandEnabled
        {
            get
            {
                return G_ReturnedBandEnabled;
            }
            set
            {
                G_ReturnedBandEnabled = value;
            }
        }

        private static bool[] G_M3Enabled = new bool[NumBands] { true, true, true };
        public static bool[] M3Enabled
        {
            get
            {
                return G_M3Enabled;
            }
            set
            {
                G_M3Enabled = value;
            }
        }

        private static bool[] G_ReturnedM3Enabled = new bool[NumBands] { true, true, true };
        public static bool[] ReturnedM3Enabled
        {
            get
            {
                return G_ReturnedM3Enabled;
            }
            set
            {
                G_ReturnedM3Enabled = value;
            }
        }

        private static int G_SquelchIndex = 4;
        public static int SquelchIndex
        {
            get
            {
                return G_SquelchIndex;
            }
            set
            {
                G_SquelchIndex = value;
            }
        }
        private static UInt32 G_ReturnedSquelchIndex = 0;
        public static UInt32 ReturnedSquelchIndex
        {
            get
            {
                return G_ReturnedSquelchIndex;
            }
            set
            {
                G_ReturnedSquelchIndex = value;
            }
        }
        private static int G_VolumeIndex = 0;
        public static int VolumeIndex
        {
            get
            {
                return G_VolumeIndex;
            }
            set
            {
                G_VolumeIndex = value;
            }
        }
        private static UInt32 G_ReturnedVolumeIndex = 0;
        public static UInt32 ReturnedVolumeIndex
        {
            get
            {
                return G_ReturnedVolumeIndex;
            }
            set
            {
                G_ReturnedVolumeIndex = value;
            }
        }
        private static UInt32 G_SquelchSetting = 0; // if not uV/M sent times 10
        public static UInt32 SquelchSetting
        {
            get
            {
                return G_SquelchSetting;
            }
            set
            {
                G_SquelchSetting = value;
            }
        }
        private static UInt32 G_ReturnedSquelchSetting = 0; // if not uV/M sent times 10
        public static UInt32 ReturnedSquelchSetting
        {
            get
            {
                return G_ReturnedSquelchSetting;
            }
            set
            {
                G_ReturnedSquelchSetting = value;
            }
        }
        private static UInt32 G_VolumeSetting = 0;
        public static UInt32 VolumeSetting
        {
            get
            {
                return G_VolumeSetting;
            }
            set
            {
                G_VolumeSetting = value;
            }
        }
        private static UInt32 G_ReturnedVolumeSetting = 0;
        public static UInt32 ReturnedVolumeSetting
        {
            get
            {
                return G_ReturnedVolumeSetting;
            }
            set
            {
                G_ReturnedVolumeSetting = value;
            }
        }

        private static Int32[] G_SquelchSettings = new Int32[7];
        public static Int32[] SquelchSettings
        {
            get
            {
                return G_SquelchSettings;
            }
            set
            {
                G_SquelchSettings = value;
            }

        }
        private static UInt32[] G_ReturnedSquelchSettings = new UInt32[7];
        public static UInt32[] ReturnedSquelchSettings
        {
            get
            {
                return G_ReturnedSquelchSettings;
            }
            set
            {
                G_ReturnedSquelchSettings = value;
            }

        }

        private static double[] G_FreqSetting = new double[NumBands] { 138, 612, 774 };
        public static double[] FreqSetting
        {
            get
            {
                return G_FreqSetting;
            }
            set
            {
                G_FreqSetting = value;
            }

        }
        
        private static double[] G_M3FreqSetting = new double[NumBands] { 138, 612, 776.5 };
        public static double[] M3FreqSetting
        {
            get
            {
                return G_M3FreqSetting;
            }
            set
            {
                G_M3FreqSetting = value;
            }

        }

        private static double[] G_ReturnedFreqSetting = new double[NumBands] { 138, 612, 774 };
        public static double[] ReturnedFreqSetting
        {
            get
            {
                return G_ReturnedFreqSetting;
            }
            set
            {
                G_ReturnedFreqSetting = value;
            }

        }

        private static double[] G_ReturnedM3FreqSetting = new double[NumBands] { 138, 612, 776.5 };
        public static double[] ReturnedM3FreqSetting
        {
            get
            {
                return G_ReturnedM3FreqSetting;
            }
            set
            {
                G_ReturnedM3FreqSetting = value;
            }

        }

        private static float[] G_LevelAdjust = new float[NumBands] { 0, 0, 0 };
        public static float[] LevelAdjust
        {
            get
            {
                return G_LevelAdjust;
            }
            set
            {
                G_LevelAdjust = value;
            }

        }

        private static bool[] G_AnalogEquiv = new bool[NumBands] { false, false, false };
        public static bool[] AnalogEquiv
        {
            get
            {
                return G_AnalogEquiv;
            }
            set
            {
                G_AnalogEquiv = value;
            }
        }

        private static bool[] G_ReturnedAnalogEquiv = new bool[NumBands] { false, false, false };
        public static bool[] ReturnedAnalogEquiv
        {
            get
            {
                return G_ReturnedAnalogEquiv;
            }
            set
            {
                G_ReturnedAnalogEquiv = value;
            }
        }

        private static double[] G_ReturnedCalFreqStart = new double[NumBands] { 136, 610, 770 };
        public static double[] ReturnedCalFreqStart
        {
            get
            {
                return G_ReturnedCalFreqStart;
            }
            set
            {
                G_ReturnedCalFreqStart = value;
            }

        }


        //advanced settings variables for each device
        private static UInt32 G_ReturnedMonitorInterval_10ms = 0;
        public static UInt32 ReturnedMonitorInterval_10ms
        {
            get
            {
                return G_ReturnedMonitorInterval_10ms;
            }
            set
            {
                G_ReturnedMonitorInterval_10ms = value;
            }
        }
        private static UInt32 G_ReturnedBacklightTimeout_10ms = 0;
        public static UInt32 ReturnedBacklightTimeout_10ms
        {
            get
            {
                return G_ReturnedBacklightTimeout_10ms;
            }
            set
            {
                G_ReturnedBacklightTimeout_10ms = value;
            }
        }
        private static UInt32 G_ReturnedMuteTimeout_10ms = 0;
        public static UInt32 ReturnedMuteTimeout_10ms
        {
            get
            {
                return G_ReturnedMuteTimeout_10ms;
            }
            set
            {
                G_ReturnedMuteTimeout_10ms = value;
            }
        }
        private static UInt32 G_ReturnedMonitorTimeout_10ms = 0;
        public static UInt32 ReturnedMonitorTimeout_10ms
        {
            get
            {
                return G_ReturnedMonitorTimeout_10ms;
            }
            set
            {
                G_ReturnedMonitorTimeout_10ms = value;
            }
        }
        private static UInt32 G_ReturnedMeasureTimeout_10ms = 0;
        public static UInt32 ReturnedMeasureTimeout_10ms
        {
            get
            {
                return G_ReturnedMeasureTimeout_10ms;
            }
            set
            {
                G_ReturnedMeasureTimeout_10ms = value;
            }
        }
        private static UInt32 G_ReturnedAdvSetSensitivity = 10;
        public static UInt32 ReturnedAdvSetSensitivity
        {
            get
            {
                return G_ReturnedAdvSetSensitivity;
            }
            set
            {
                G_ReturnedAdvSetSensitivity = value;
            }
        }
        private static UInt32 G_ReturnedAdvSetDistanceUnits = 0;
        public static UInt32 ReturnedAdvSetDistanceUnits
        {
            get
            {
                return G_ReturnedAdvSetDistanceUnits;
            }
            set
            {
                G_ReturnedAdvSetDistanceUnits = value;
            }
        }
        private static UInt32 G_ReturnedAdvSetPeakHold = 0;
        public static UInt32 ReturnedAdvSetPeakHold
        {
            get
            {
                return G_ReturnedAdvSetPeakHold;
            }
            set
            {
                G_ReturnedAdvSetPeakHold = value;
            }
        }
        private static bool G_ReturnedDockPowB1LowGain = false;
        public static bool ReturnedDockPowB1LowGain
        {
            get
            {
                return G_ReturnedDockPowB1LowGain;
            }
            set
            {
                G_ReturnedDockPowB1LowGain = value;
            }
        }
        
        //hard coded factory defaults may need to revisit if these change
        private static UInt32[] G_MonitorIntervalSecs = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 0, 15, 0, 0};
        public static UInt32[] MonitorIntervalSecs
        {
            get
            {
                return G_MonitorIntervalSecs;
            }
            set
            {
                G_MonitorIntervalSecs = value;
            }
        }
        private static UInt32[] G_BacklightTimeoutSecs = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 10, 10, 10, 10 };
        public static UInt32[] BacklightTimeoutSecs
        {
            get
            {
                return G_BacklightTimeoutSecs;
            }
            set
            {
                G_BacklightTimeoutSecs = value;
            }
        }
        private static UInt32[] G_MuteTimeoutSecs = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 300, 300, 300, 300 };
        public static UInt32[] MuteTimeoutSecs
        {
            get
            {
                return G_MuteTimeoutSecs;
            }
            set
            {
                G_MuteTimeoutSecs = value;
            }
        }
        private static UInt32[] G_MonitorTimeoutMins = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 480, 480, 480, 30 };
        public static UInt32[] MonitorTimeoutMins
        {
            get
            {
                return G_MonitorTimeoutMins;
            }
            set
            {
                G_MonitorTimeoutMins = value;
            }
        }
        private static UInt32[] G_MeasureTimeoutMins = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 10, 15, 10, 10 };
        public static UInt32[] MeasureTimeoutMins
        {
            get
            {
                return G_MeasureTimeoutMins;
            }
            set
            {
                G_MeasureTimeoutMins = value;
            }
        }
        private static UInt32[] G_AdvSetSensitivity = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 10, 10, 10, 10 };
        public static UInt32[] AdvSetSensitivity
        {
            get
            {
                return G_AdvSetSensitivity;
            }
            set
            {
                G_AdvSetSensitivity = value;
            }
        }
        private static UInt32[] G_AdvSetDistanceUnits = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 0, 0, 0, 0 };
        public static UInt32[] AdvSetDistanceUnits
        {
            get
            {
                return G_AdvSetDistanceUnits;
            }
            set
            {
                G_AdvSetDistanceUnits = value;
            }
        }
        private static UInt32[] G_AdvSetPeakHold = new UInt32[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES] { 0, 0, 0, 0 };
        public static UInt32[] AdvSetPeakHold
        {
            get
            {
                return G_AdvSetPeakHold;
            }
            set
            {
                G_AdvSetPeakHold = value;
            }
        }
        private static bool G_MMMdualCorr = true;
        public static bool MMMdualCorr
        {
            get
            {
                return G_MMMdualCorr;
            }
            set
            {
                G_MMMdualCorr = value;
            }
        }
        private static bool G_DockPowB1LowGain = false;
        public static bool DockPowB1LowGain
        {
            get
            {
                return G_DockPowB1LowGain;
            }
            set
            {
                G_DockPowB1LowGain = value;
            }
        }
        private static bool G_AllowDockPowB1LowGain = false;
        public static bool AllowDockPowB1LowGain
        {
            get
            {
                return G_AllowDockPowB1LowGain;
            }
            set
            {
                G_AllowDockPowB1LowGain = value;
            }
        }


        private static float[,] G_AntennaGain = new float[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES, NumBands] {{ 0, 0, 0 },                //QSDF external antenna gain
                                                                                                                { 0, 0, 0 },                //QShadow
                                                                                                                { 0, 0, 0 },                //QCompass
                                                                                                                { 0, 0, 0 }};               //QS3
        public static float[,] AntennaGain
        {
            get
            {
                return G_AntennaGain;
            }
            set
            {
                G_AntennaGain = value;
            }

        }


        private static float[,] G_IntAntennaGain = new float[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES, NumBands] {{ 0, 0, 0 },             //QSDF
                                                                                                                   { 0, 0, 0 },             //QShadow
                                                                                                                   { 0, 0, 0 },             //QCompass
                                                                                                                   { 0, 0, 0 }};            //QS3
        public static float[,] IntAntennaGain
        {
            get
            {
                return G_IntAntennaGain;
            }
            set
            {
                G_IntAntennaGain = value;
            }

        }


        private static float[,] G_NFPGain = new float[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES, NumBands] {{ 0, 0, 0 },                    //QSDF
                                                                                                            { 0, 0, 0 },                    //QShadow
                                                                                                            { 0, 0, 0 },                    //QCompass
                                                                                                            { 0, 0, 0 }};                   //QS3
        public static float[,] NFPGain
        {
            get
            {
                return G_NFPGain;
            }
            set
            {
                G_NFPGain = value;
            }

        }


        private static float[,] G_BNCGain = new float[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES, NumBands] {{ 0, 0, 0 },                    //QSDF
                                                                                                            { 0, 0, 0 },                    //QShadow
                                                                                                            { 0, 0, 0 },                    //QCompass
                                                                                                            { 0, 0, 0 }};                   //QS3
        public static float[,] BNCGain
        {
            get
            {
                return G_BNCGain;
            }
            set
            {
                G_BNCGain = value;
            }

        }


        private static float[,] G_YagiGain = new float[(int)GlobalVars.DeviceTypeEnum.NUMDEVICES, NumBands] {{ 0, 0, 0 },                   //QSDF
                                                                                                             { 0, 0, 0 },                   //QShadow
                                                                                                             { 0, 0, 0 },                   //QCompass
                                                                                                             { 0, 0, 0 }};                  //QS3
        public static float[,] YagiGain
        {
            get
            {
                return G_YagiGain;
            }
            set
            {
                G_YagiGain = value;
            }

        }


        private static float[] G_ReturnedAntennaGain = new float[NumBands] { -100.0f, -100.0f, -100.0f };
        public static float[] ReturnedAntennaGain
        {
            get
            {
                return G_ReturnedAntennaGain;
            }
            set
            {
                G_ReturnedAntennaGain = value;
            }
        }
        private static float[] G_ReturnedIntAntennaGain = new float[NumBands] { -100.0f, -100f, -100f };
        public static float[] ReturnedIntAntennaGain
        {
            get
            {
                return G_ReturnedIntAntennaGain;
            }
            set
            {
                G_ReturnedIntAntennaGain = value;
            }
        }
        private static float[] G_ReturnedNFPGain = new float[NumBands] { -100.0f, -100f, -100f };
        public static float[] ReturnedNFPGain
        {
            get
            {
                return G_ReturnedNFPGain;
            }
            set
            {
                G_ReturnedNFPGain = value;
            }
        }
        private static float[] G_ReturnedYagiGain = new float[NumBands] { -100.0f, -100f, -100f };
        public static float[] ReturnedYagiGain
        {
            get
            {
                return G_ReturnedYagiGain;
            }
            set
            {
                G_ReturnedYagiGain = value;
            }
        }
        private static float[] G_ReturnedBNCGain = new float[NumBands] { -100.0f, -100f, -100f };
        public static float[] ReturnedBNCGain
        {
            get
            {
                return G_ReturnedBNCGain;
            }
            set
            {
                G_ReturnedBNCGain = value;
            }
        }

        private static bool G_ReturnedMMMdualCorr = true;
        public static bool ReturnedMMMdualCorr
        {
            get
            {
                return G_ReturnedMMMdualCorr;
            }
            set
            {
                G_ReturnedMMMdualCorr = value;
            }
        }

        private static float[,] G_DefCompassAntCorr = new float[2, NumBands] {{ -11.0f, -6.5f, -3.0f },                //Nagoya Antenna
                                                                               { -3.5f, -6.5f, -3.0f }};               //Smiley 5/8 220 Antenna
        public static float[,] DefCompassAntCorr
        {
            get
            {
                return G_DefCompassAntCorr;
            }
            set
            {
                G_DefCompassAntCorr = value;
            }

        }




        //OFDM settings
        private static bool[] G_OFDMenabled = new bool[NumBands] { false, false, false };
        public static bool[] OFDMenabled
        {
            get
            {
                return G_OFDMenabled;
            }
            set
            {
                G_OFDMenabled = value;
            }
        }
        private static bool[] G_ReturnedOFDMenabled = new bool[NumBands] { false, false, false };
        public static bool[] ReturnedOFDMenabled
        {
            get
            {
                return G_ReturnedOFDMenabled;
            }
            set
            {
                G_ReturnedOFDMenabled = value;
            }
        }
        private static double[,] G_PilotFreqSetting = new double[2,NumBands] {{ 138, 612, 774 },{ 138, 612, 774 }};
        public static double[,] PilotFreqSetting
        {
            get
            {
                return G_PilotFreqSetting;
            }
            set
            {
                G_PilotFreqSetting = value;
            }

        }
        private static float[,] G_PilotLevelAdjust = new float[2, NumBands] { { 0, 0, 0 }, { 0, 0, 0 } };
        public static float[,] PilotLevelAdjust
        {
            get
            {
                return G_PilotLevelAdjust;
            }
            set
            {
                G_PilotLevelAdjust = value;
            }

        }
        private static float[] G_OFDMOffset = new float[NumBands] { OFDMOffset4k, OFDMOffset4k, OFDMOffset4k };
        public static float[] OFDMOffset
        {
            get
            {
                return G_OFDMOffset;
            }
            set
            {
                G_OFDMOffset = value;
            }
        }
        private static double[,] G_ReturnedPilotFreqSetting = new double[2,NumBands] {{138, 612, 774},{ 138, 612, 774 }};
        public static double[,] ReturnedPilotFreqSetting
        {
            get
            {
                return G_ReturnedPilotFreqSetting;
            }
            set
            {
                G_ReturnedPilotFreqSetting = value;
            }

        }
        private static float[,] G_ReturnedPilotLevelAdjust = new float[2,NumBands] {{0,0,0},{0,0,0}};
        public static float[,] ReturnedPilotLevelAdjust
        {
            get
            {
                return G_ReturnedPilotLevelAdjust;
            }
            set
            {
                G_ReturnedPilotLevelAdjust = value;
            }

        }
        private static float[] G_ReturnedOFDMOffset = new float[NumBands] { OFDMOffset4k, OFDMOffset4k, OFDMOffset4k };
        public static float[] ReturnedOFDMOffset
        {
            get
            {
                return G_ReturnedOFDMOffset;
            }
            set
            {
                G_ReturnedOFDMOffset = value;
            }
        }




        public struct freqRange
        {
            public int band;
            public double start;
            public double stop;
            public freqRange(int Band, double Start, double Stop)
            {
                band = Band;
                start = Start;
                stop = Stop;
            }
        };

        private static List<freqRange> G_compassFreqRanges = new List<freqRange> { new freqRange(0, 136.5, 138.5), new freqRange(1, 610.5, 612), new freqRange(2, 760.0, 1200.0) };
        public static List<freqRange> compassFreqRanges
        {
            get
            {
                return G_compassFreqRanges;
            }
            set
            {
                G_compassFreqRanges = value;
            }
        }
        private static List<freqRange> G_compassIntFreqRanges = new List<freqRange> { new freqRange(0, 132.5, 140.0), new freqRange(1, 606.5, 614), new freqRange(2, 824.0, 1200.0) };
        public static List<freqRange> compassIntFreqRanges
        {
            get
            {
                return G_compassIntFreqRanges;
            }
            set
            {
                G_compassIntFreqRanges = value;
            }
        }
        private static List<freqRange> G_compass265FreqRanges = new List<freqRange> { new freqRange(0, 261.0, 269.0), new freqRange(1, 606.5, 614), new freqRange(2, 824.0, 1200.0) };
        public static List<freqRange> compass265FreqRanges
        {
            get
            {
                return G_compass265FreqRanges;
            }
            set
            {
                G_compass265FreqRanges = value;
            }
        }
        private static List<freqRange> G_qs3FreqRanges = new List<freqRange> { new freqRange(0, 136.5, 138.5), new freqRange(1, 610.5, 612), new freqRange(2, 770.0, 860.0), new freqRange(2, 900.0, 1200.0) };
        public static List<freqRange> qs3FreqRanges
        {
            get
            {
                return G_qs3FreqRanges;
            }
            set
            {
                G_qs3FreqRanges = value;
            }
        }
        private static List<freqRange> G_qs3IntFreqRanges = new List<freqRange> { new freqRange(0, 132.5, 140.0), new freqRange(1, 606.5, 614), new freqRange(2, 824.0, 915.0), new freqRange(2, 970.0, 1200.0) };
        public static List<freqRange> qs3IntFreqRanges
        {
            get
            {
                return G_qs3IntFreqRanges;
            }
            set
            {
                G_qs3IntFreqRanges = value;
            }
        }
        private static List<freqRange> G_qs3265FreqRanges = new List<freqRange> { new freqRange(0, 261.0, 269.0), new freqRange(1, 606.5, 614), new freqRange(2, 824.0, 915.0), new freqRange(2, 970.0, 1200.0) };
        public static List<freqRange> qs3265FreqRanges
        {
            get
            {
                return G_qs3265FreqRanges;
            }
            set
            {
                G_qs3265FreqRanges = value;
            }
        }


    }
}
