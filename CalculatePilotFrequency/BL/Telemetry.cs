using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace CalculatePilotFrequency
{
    class Telemetry
    {
        public enum MarkerSelections
        {
            MARKSEL_1 = 0,
            MARKSEL_2 = 1,
            MARKSEL_3 = 2,
            MARKSEL_TEST = 3,
            MARKSEL_ERROR = 4
        }
        public enum LevelUnitsOfMeasure
        {
            LEVEL_UVPM = 0,
            LEVEL_DBUV = 1,
            LEVEL_DBUVPM = 2,
            LEVEL_DB = 3,
            LEVEL_NOUNITS = 4,
            LEVEL_ERROR = 5
        }

        public enum FFTwin
        {
            FFTWIN_NONE = 0,
            FFTWIN_HANN = 1,
            FFTWIN_BLACKMAN_NUTTAL = 2,
            FFTWIN_ERROR = 3
        }

        public enum DistanceUnitsOfMeasure
        {
            DISTANCE_FEET = 0,
            DISTANCE_METERS = 1,
            DISTANCE_NOUNITS = 2,
            DISTANCE_ERROR = 3
        }

        // top-level struct
        public struct TelemetryFrame
        {
            public const int LENGTH = 21993;
            public const UInt32 SYNC = 0x56a5a695;
            private byte[] byteArray;
            public TelemetryBuffer Buffer { get { return new TelemetryBuffer(new ArraySegment<byte>(byteArray, 0, TelemetryBuffer.LENGTH)); } }
            public CalParamsStruct CalParams { get { return new CalParamsStruct(new ArraySegment<byte>(byteArray, 20, CalParamsStruct.LENGTH)); } }
            public CfgParamsStruct CfgParams { get { return new CfgParamsStruct(new ArraySegment<byte>(byteArray, 40, CfgParamsStruct.LENGTH)); } }
            public CommandParamaterStruct CommandParamaters { get { return new CommandParamaterStruct(new ArraySegment<byte>(byteArray, 96, CommandParamaterStruct.LENGTH)); } }
            public UsrParamsRAMStruct Users { get { return new UsrParamsRAMStruct(new ArraySegment<byte>(byteArray, 132, UsrParamsRAMStruct.LENGTH)); } }
            public AnalogStruct Analogs { get { return new AnalogStruct(new ArraySegment<byte>(byteArray, 148, AnalogStruct.LENGTH)); } }
            public normXCorrStatStruct Stats { get { return new normXCorrStatStruct(new ArraySegment<byte>(byteArray, 172, normXCorrStatStruct.LENGTH)); } }
            public PeakNewStruct Peaker { get { return new PeakNewStruct(new ArraySegment<byte>(byteArray, 220, PeakNewStruct.LENGTH)); } }
            public UInt32[] normXCorrBuf { get { UInt32[] tmp = new UInt32[1335]; System.Buffer.BlockCopy(byteArray, 268, tmp, 0, 4 * tmp.Length); return tmp; } }
            public UInt32[] magSqrBuf { get { UInt32[] tmp = new UInt32[4096]; System.Buffer.BlockCopy(byteArray, 5608, tmp, 0, 4 * tmp.Length); return tmp; } }
            public byte cmdRespAccept { get { return byteArray[21992]; } }
            public void Update(byte[] buffer)
            {
                if (byteArray == null || byteArray.Length != buffer.Length)
                    byteArray = new byte[buffer.Length];
                Array.Copy(buffer, 0, byteArray, 0, buffer.Length);
            }
        }


        public struct CondensedTelemetryFrame
        {
            public const int LENGTH = 269;
            public const UInt32 SYNC = 0x56a5a695;
            private byte[] byteArray;
            public TelemetryBuffer Buffer { get { return new TelemetryBuffer(new ArraySegment<byte>(byteArray, 0, TelemetryBuffer.LENGTH)); } }
            public CalParamsStruct CalParams { get { return new CalParamsStruct(new ArraySegment<byte>(byteArray, 20, CalParamsStruct.LENGTH)); } }
            public CfgParamsStruct CfgParams { get { return new CfgParamsStruct(new ArraySegment<byte>(byteArray, 40, CfgParamsStruct.LENGTH)); } }
            public CommandParamaterStruct CommandParamaters { get { return new CommandParamaterStruct(new ArraySegment<byte>(byteArray, 96, CommandParamaterStruct.LENGTH)); } }
            public UsrParamsRAMStruct Users { get { return new UsrParamsRAMStruct(new ArraySegment<byte>(byteArray, 132, UsrParamsRAMStruct.LENGTH)); } }
            public AnalogStruct Analogs { get { return new AnalogStruct(new ArraySegment<byte>(byteArray, 148, AnalogStruct.LENGTH)); } }
            public normXCorrStatStruct Stats { get { return new normXCorrStatStruct(new ArraySegment<byte>(byteArray, 172, normXCorrStatStruct.LENGTH)); } }
            public PeakNewStruct Peaker { get { return new PeakNewStruct(new ArraySegment<byte>(byteArray, 220, PeakNewStruct.LENGTH)); } }
            public byte cmdRespAccept { get { return byteArray[268]; } }
            public void Update(byte[] buffer)
            {
                if (byteArray == null || byteArray.Length != buffer.Length)
                    byteArray = new byte[buffer.Length];
                Array.Copy(buffer, 0, byteArray, 0, buffer.Length);
            }

        }
        // intermediate-level structs
        public struct TelemetryBuffer
        {
            public const int LENGTH = 4 + TelemetryBitfield.LENGTH + I2Sbitfield.LENGTH + 3 * UARTbitfield.LENGTH + 2 * I2Cbitfield.LENGTH;
            private byte[] byteArray;
            public byte[] Sync { get { byte[] tmp = new byte[4]; Array.Copy(byteArray, 0, tmp, 0, 4); return tmp; } }
            public TelemetryBitfield Bitfield { get { return new TelemetryBitfield(new ArraySegment<byte>(byteArray, 4, TelemetryBitfield.LENGTH)); } }
            public I2Sbitfield ADCI2Sflags { get { return new I2Sbitfield(new ArraySegment<byte>(byteArray, 8, I2Sbitfield.LENGTH)); } }
            public UARTbitfield RS232UARTflags { get { return new UARTbitfield(new ArraySegment<byte>(byteArray, 10, UARTbitfield.LENGTH)); } }
            public UARTbitfield USBUARTflags { get { return new UARTbitfield(new ArraySegment<byte>(byteArray, 12, UARTbitfield.LENGTH)); } }
            public UARTbitfield GPSUARTflags { get { return new UARTbitfield(new ArraySegment<byte>(byteArray, 14, UARTbitfield.LENGTH)); } }
            public I2Cbitfield TuneI2Cflags { get { return new I2Cbitfield(new ArraySegment<byte>(byteArray, 16, I2Cbitfield.LENGTH)); } }
            public I2Cbitfield VolI2Cflags { get { return new I2Cbitfield(new ArraySegment<byte>(byteArray, 18, I2Cbitfield.LENGTH)); } }
            public TelemetryBuffer(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }
        public struct CalParamsStruct
        {
            public const int LENGTH = 20;
            private byte[] byteArray;

            public UInt32 SerialNumber { get { return BitConverter.ToUInt32(byteArray, 0); } }
            public Int32 CalFreq { get { return BitConverter.ToInt32(byteArray, 4); } }
            public float CalPwr { get { return BitConverter.ToSingle(byteArray, 8); } }
            public Char[] CalDateStr { get { Char[] tmp = new Char[7]; Array.Copy(byteArray, 12, tmp, 0, tmp.Length); return tmp; } }
            public byte PadByte { get { return byteArray[19]; } }

            public CalParamsStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }
        public struct CfgParamsStruct
        {
            public const int LENGTH = 56;
            private byte[] byteArray;
            public UInt32 CenterFreq { get { return BitConverter.ToUInt32(byteArray, 0); } }
            public UInt32 Squelch1 { get { return BitConverter.ToUInt32(byteArray, 4); } }
            public UInt32 Squelch2 { get { return BitConverter.ToUInt32(byteArray, 8); } }
            public UInt32 Squelch3 { get { return BitConverter.ToUInt32(byteArray, 12); } }
            public UInt32 Squelch4 { get { return BitConverter.ToUInt32(byteArray, 16); } }
            public UInt32 Squelch5 { get { return BitConverter.ToUInt32(byteArray, 20); } }
            public UInt32 Squelch6 { get { return BitConverter.ToUInt32(byteArray, 24); } }
            public UInt32 Squelch7 { get { return BitConverter.ToUInt32(byteArray, 28); } }
            public UInt32 PeakHold { get { return BitConverter.ToUInt32(byteArray, 32); } }
            public UInt32 MarkerFreq1 { get { return BitConverter.ToUInt32(byteArray, 36); } }
            public UInt32 MarkerFreq2 { get { return BitConverter.ToUInt32(byteArray, 40); } }
            public UInt32 MarkerFreq3 { get { return BitConverter.ToUInt32(byteArray, 44); } }
            public UInt32 CurrentMarkerFreq { get { return BitConverter.ToUInt32(byteArray, 48); } }
            public MarkerSelections SelectedMarker { get { return (MarkerSelections)byteArray[52]; } }
            public LevelUnitsOfMeasure LevelUnits { get { return (LevelUnitsOfMeasure)byteArray[53]; } }
            public DistanceUnitsOfMeasure DistanceUnits { get { return (DistanceUnitsOfMeasure)byteArray[54]; } }
            public byte PadByte { get { return byteArray[55]; } }

            public CfgParamsStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }

        public struct CommandParamaterStruct
        {
            public const int LENGTH = 36;//should be 33
            private byte[] byteArray;

            public float AntGainInt { get { return BitConverter.ToSingle(byteArray, 0); } }
            public float AntGainExt { get { return BitConverter.ToSingle(byteArray, 4); } }
            public float CorrThresh { get { return BitConverter.ToSingle(byteArray, 8); } }
            public UInt32 Log2deSpurRatio { get { return BitConverter.ToUInt32(byteArray, 12); } }
            public UInt32 Log2corrSidebandRatio { get { return BitConverter.ToUInt32(byteArray, 16); } }
            public UInt32 CorrMaxOfsDelta { get { return BitConverter.ToUInt32(byteArray, 20); } }
            public UInt32 VAvgDocked { get { return BitConverter.ToUInt32(byteArray, 24); } }
            public UInt32 VAvgUnDocked { get { return BitConverter.ToUInt32(byteArray, 28); } }
            public FFTwin FFTwindow { get { return (FFTwin)byteArray[32]; } }
            public byte PadByte { get { return byteArray[33]; } }
            public byte PadByte2 { get { return byteArray[34]; } }
            public byte PadByte3 { get { return byteArray[35]; } }
            public CommandParamaterStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }

        public struct UsrParamsRAMStruct
        {
            public const int LENGTH = 16;
            private byte[] byteArray;
            public UInt32 VolumeIndex { get { return BitConverter.ToUInt32(byteArray, 0); } }
            public UInt32 DistanceIndex { get { return BitConverter.ToUInt32(byteArray, 4); } }
            public UInt32 Sensitivity { get { return BitConverter.ToUInt32(byteArray, 8); } }
            public UInt32 SquelchIndex { get { return BitConverter.ToUInt32(byteArray, 12); } }
            public UsrParamsRAMStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }



        public struct AnalogStruct
        {
            public const int LENGTH = 24;
            private byte[] byteArray;
            public UInt32 BatVolts { get { return BitConverter.ToUInt32(byteArray, 0); } }
            public float fBatVolts { get { return BitConverter.ToSingle(byteArray, 4); } }
            public UInt32 ExtVolts { get { return BitConverter.ToUInt32(byteArray, 8); } }
            public float fExtVolts { get { return BitConverter.ToSingle(byteArray, 12); } }
            public UInt32 K60Temp { get { return BitConverter.ToUInt32(byteArray, 16); } }
            public float fK60Temp { get { return BitConverter.ToSingle(byteArray, 20); } }

            public AnalogStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }

        public struct normXCorrStatStruct
        {
            public const int LENGTH = 48;
            private byte[] byteArray;
            public UInt64 Sum { get { return BitConverter.ToUInt64(byteArray, 0); } }
            public Int64 NumSumLo { get { return BitConverter.ToInt64(byteArray, 8); } }
            public Int64 NumSumHi { get { return BitConverter.ToInt64(byteArray, 16); } }
            public UInt64 SumSqrs { get { return BitConverter.ToUInt64(byteArray, 24); } }
            public UInt64 Var { get { return BitConverter.ToUInt64(byteArray, 32); } }
            public UInt32 Stdev { get { return BitConverter.ToUInt32(byteArray, 40); } }
            public UInt32 Mean { get { return BitConverter.ToUInt32(byteArray, 44); } }
            public normXCorrStatStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }

        public struct PeakNewStruct
        {
            public const int LENGTH = 48;
            private byte[] byteArray;

            public Int32 MaxXcorrOfs { get { return BitConverter.ToInt32(byteArray, 0); } }
            public float MaxXcorrVal { get { return BitConverter.ToSingle(byteArray, 4); } }
            public UInt32 Sum2fftMagSqr { get { return BitConverter.ToUInt32(byteArray, 8); } }
            public UInt32 LogLevel { get { return BitConverter.ToUInt32(byteArray, 12); } }
            public float dBmV_rx { get { return BitConverter.ToSingle(byteArray, 16); } }
            public float uVm_rx { get { return BitConverter.ToSingle(byteArray, 20); } }
            public float uVm_10ft { get { return BitConverter.ToSingle(byteArray, 24); } }
            public float dBuV_rx { get { return BitConverter.ToSingle(byteArray, 28); } }
            public float dBuV_10ft { get { return BitConverter.ToSingle(byteArray, 32); } }
            public float dBuVm_rx { get { return BitConverter.ToSingle(byteArray, 36); } }
            public float dBuVm_10ft { get { return BitConverter.ToSingle(byteArray, 40); } }
            public byte LeakNotNoise { get { return byteArray[44]; } }
            public byte PadByte { get { return byteArray[45]; } }
            public byte PadByte2 { get { return byteArray[46]; } }
            public byte PadByte3 { get { return byteArray[47]; } }


            public PeakNewStruct(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
            }
        }
        // bottom-level structs
        public struct TelemetryBitfield
        {
            public const int LENGTH = 4;
            private byte[] byteArray;
            private BitArray boolArray;
            public bool ADC_CLIP_L { get { return boolArray[0]; } }
            public bool ADC_CLIP_R { get { return boolArray[1]; } }
            public bool TUNER_LDET { get { return boolArray[2]; } }
            public bool CHG_FLT { get { return boolArray[3]; } }
            public bool CHG_STAT { get { return boolArray[4]; } }
            public bool TRIG_UP { get { return boolArray[5]; } }
            public bool TRIG_DN { get { return boolArray[6]; } }
            public bool EXTPWR_STAT { get { return boolArray[7]; } }
            public bool DOCKED_STAT { get { return boolArray[8]; } }
            public bool GPS_FIX_AVAIL { get { return boolArray[9]; } }
            public bool PBSW_INFO { get { return boolArray[10]; } }
            public bool PBSW_SQLCH { get { return boolArray[11]; } }
            public bool PBSW_SENSE { get { return boolArray[12]; } }
            public bool PBSW_SPARE { get { return boolArray[13]; } }
            public bool PBSW_DIST { get { return boolArray[14]; } }
            public bool RESERVED_L { get { return boolArray[15]; } }
            public bool CMDSTATE_LED { get { return boolArray[16]; } }
            public bool CMDSTATE_ADC_RST { get { return boolArray[17]; } }
            public bool CMDSTATE_VANA { get { return boolArray[18]; } }
            public bool CMDSTATE_VDIG { get { return boolArray[19]; } }
            public bool CMDSTATE_LCD_BL { get { return boolArray[20]; } }
            public bool CMDSTATE_PWRKILL { get { return boolArray[21]; } }
            public bool CMDSTATE_SPKR { get { return boolArray[22]; } }
            public bool CMDSTATE_DOCK_P { get { return boolArray[23]; } }
            public bool CMDSTATE_DOCK_N { get { return boolArray[24]; } }
            public bool CMDSTATE_232 { get { return boolArray[25]; } }
            public bool CMDSTATE_USB_RST { get { return boolArray[26]; } }
            public bool CMDSTATE_GPS_PUSH { get { return boolArray[27]; } }
            public bool CMDSTATE_GPS_XLTR { get { return boolArray[28]; } }
            public bool CMDSTATE_VGPS { get { return boolArray[29]; } }
            public bool RESERVED_H { get { return boolArray[30]; } }
            public bool OVF { get { return boolArray[31]; } }
            public TelemetryBitfield(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
                boolArray = new BitArray(byteArray);
            }
        }
        public struct I2Sbitfield
        {
            public const int LENGTH = 2;
            private byte[] byteArray;
            private BitArray boolArray;
            public bool BUSY_LOCK { get { return boolArray[0]; } }
            public bool RX { get { return boolArray[1]; } }
            public bool OVF { get { return boolArray[2]; } }
            public I2Sbitfield(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
                boolArray = new BitArray(byteArray);
            }
        }
        public struct UARTbitfield
        {
            public const int LENGTH = 2;
            private byte[] byteArray;
            private BitArray boolArray;
            public bool BUSY_LOCK { get { return boolArray[0]; } }
            public bool RX { get { return boolArray[1]; } }
            public bool OVF { get { return boolArray[2]; } }
            public UARTbitfield(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
                boolArray = new BitArray(byteArray);
            }
        }
        public struct I2Cbitfield
        {
            public const int LENGTH = 2;
            private byte[] byteArray;
            private BitArray boolArray;
            public bool BUSY_LOCK { get { return boolArray[0]; } }
            public bool MASTER_READ { get { return boolArray[1]; } }
            public bool NACK { get { return boolArray[2]; } }
            public bool ARBL { get { return boolArray[3]; } }
            public bool FAULT_WRAPPER { get { return boolArray[15]; } }
            public I2Cbitfield(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
                boolArray = new BitArray(byteArray);
            }
        }


        public struct Peakerbitfield
        {
            public const int LENGTH = 1;
            private byte[] byteArray;
            private BitArray boolArray;
            public bool LeakNotNoise { get { return boolArray[0]; } }

            public Peakerbitfield(ArraySegment<byte> buffer)
            {
                byteArray = new byte[buffer.Count];
                Array.Copy(buffer.Array, buffer.Offset, byteArray, 0, buffer.Count);
                boolArray = new BitArray(byteArray);
            }
        }
        
        
    }
}
