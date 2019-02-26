using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net.Http;
using System.IO;
using System.Reflection;
using System.Configuration;
using System.Web.Script.Serialization;

namespace CalculatePilotFrequency
{
    public partial class OFDMForm : Form
    {
        private List<double> [] pilotFreqs = new List<double>[3];
        private List<double> [] pilotLevels = new List<double>[3];
        private List<OFDM> bandList = new List<OFDM>();
        private List<GlobalVars.freqRange> validFreqRanges;
        private GlobalVars.freqRange preferredB3Range = new GlobalVars.freqRange();

        //private MainForm mainForm = null;

        public string baseUrl;
        const string AppName = "Config";
        const string BAND1 = "BAND1";
        const string BAND2 = "BAND2";
        const string BAND3 = "BAND3";
        const string SaveOFDM = "SaveOFDM";
        const string GetOFDM = "GetOFDM";
        const string DataSavedSuccessfully = "Data Saved successfully";
        const string ProblemOnSave = "Problem while saving data";
        const string UserNotValid = "Credentials are not valid";
        const string RangeNotValidForSubcarrierFrequency = "Range not valid for SubcarrierFrequency. [Valid Range is 54000000 to 1700000000]";
        const string NotValidValueForCyclicPrefixLength = "Not valid value for CyclicPrefixLength. [Valid values 192, 256, 512, 768, 1024]";
        const string NotValidValueForFFTSize = "Not valid value for FFTSize. [valid values either 4096 or 8192]";
        const string NotValidValueForCenterFrequency = "Not valid value for CenterFrequency. [Valid Range 100000000 to 1500000000]";
        const string NotValidValueForMarkerSelection = "Not valid value for MarkerSelection. [Valid Values are 1,2,3]";
        const string NotValidValueForPilot1Frequency = "Not valid value for Pilot1Frequency. [Valid Values Range 50000000 to 1700000000 or 0]";
        const string NotValidValueForPilot2Frequency = "Not valid value for Pilot2Frequency. [Valid Values 50000000- 1700000000 or 0]";
        const string NotValidValueForPilot1RelativeLevelAdjustment = "Not valid value for Pilot1RelativeLevelAdjustment. [Valid Range 0 to 10]";
        const string NotValidValueForPilot2RelativeLevelAdjustment = "Not valid value for Pilot2RelativeLevelAdjustment. [Valid Range 0 to 10]";
        const string FailedToConnect = "Failed to connect the service";
        const string NoRecordFound = "No record found";
        const string EmptyBandData = "Input data for at least one band and press calculate button";
        private ComboBox ddlCycPrefixB1;
        private RadioButton rad4kB3;
        private RadioButton rad8kB1;
        private ListBox lstPilotsB1;
        private TextBox txtSub0FreqB1;
        private TextBox txtPilotsB1;
        private Button btnCalcPilots;
        private Label label1;
        private Label label2;
        private Label label3;
        private Label label4;
        private Label label5;
        const string EmptyCredentials = "Input Credentials";


        public OFDMForm(/*Form callingForm*/)
        {            
            InitializeComponent();
            //mainForm = callingForm as MainForm;
            ddlCycPrefixB1.SelectedIndex = 0;
           // rad4kB3.Checked = true;
           // cmbCycPrefixB2.SelectedIndex = 0;
            rad8kB1.Checked = true;
          //  cmbCycPrefixB3.SelectedIndex = 0;
          //  rad4kB3.Checked = true;
            for (int i = 0; i < GlobalVars.NumBands; i++)
            {
                pilotFreqs[i] = new List<double>();
                pilotLevels[i] = new List<double>();
            }
            baseUrl = ConfigurationManager.AppSettings["BaseAddress"];

            getFreqRanges();

            readFromFile();            
        }

        public void getFreqRanges()
        {
            switch (GlobalVars.DevicetoUse)
            {
                case GlobalVars.DeviceTypeEnum.QCOMPASS:
                    if (GlobalVars.ReturnedCalFreqStart[0] > 200) //265 international
                    {
                        validFreqRanges = GlobalVars.compass265FreqRanges;
                        preferredB3Range.start = validFreqRanges[2].start;
                        preferredB3Range.stop = validFreqRanges[2].stop;
                    }
                    else if (GlobalVars.ReturnedCalFreqStart[0] < 134) //international
                    {
                        validFreqRanges = GlobalVars.compassIntFreqRanges;
                        preferredB3Range.start = validFreqRanges[2].start;
                        preferredB3Range.stop = validFreqRanges[2].stop;
                    }
                    else //standard
                    {
                        validFreqRanges = GlobalVars.compassFreqRanges;
                        preferredB3Range.start = 770.0;
                        preferredB3Range.stop = 777.0;
                    }
                    break;
                case GlobalVars.DeviceTypeEnum.QS3:
                default:
                    if (GlobalVars.ReturnedCalFreqStart[0] > 200) //265 international
                    {
                        validFreqRanges = GlobalVars.qs3265FreqRanges;
                        preferredB3Range.start = validFreqRanges[2].start;
                        preferredB3Range.stop = validFreqRanges[2].stop;
                    }
                    else if (GlobalVars.ReturnedCalFreqStart[0] < 134) //international
                    {
                        validFreqRanges = GlobalVars.qs3IntFreqRanges;
                        preferredB3Range.start = validFreqRanges[2].start;
                        preferredB3Range.stop = validFreqRanges[2].stop;
                    }
                    else //standard
                    {
                        validFreqRanges = GlobalVars.qs3FreqRanges;
                        preferredB3Range.start = 770.0;
                        preferredB3Range.stop = 777.0;
                    }
                    break;
            }
        }

        public void CalculatePilots(UInt32 [] pilots,  UInt32 nFFT, UInt32 nCP, UInt32 f0, int band)
        {
            UInt32 fSamp = 204800000;
            UInt32 fSymU = fSamp / nFFT;
            double fSym = (double)fSamp / (nFFT + nCP);
            double[] fSC = new double[nFFT];
            double[] fTone = new double[nFFT + nCP];
            double dF;
            double PdB;
            double sc, cc;
            double pBest;
            UInt32 iBest = 0;

            pilotFreqs[band].Clear();
            pilotLevels[band].Clear();

            for (UInt32 i = 0; i < nFFT; i++)
            {
                fSC[i] = f0 + (fSymU * i);
            }

            for (UInt32 i = 0; i < (nFFT + nCP); i++)
            {
                fTone[i] = f0 + (fSym * i);
            }

            for (UInt32 j = 0; j < pilots.Length; j++)
            {
                if (pilots[j] != 0)
                {
                    pBest = -1000;
                    for (UInt32 i = 0; i < (nFFT + nCP); i++)
                    {
                        dF = Convert.ToDouble(fTone[i] - fSC[pilots[j]]);
                        sc = sinc(2 * dF / fSym);
                        cc = cosc(2 * dF / fSym);
                        PdB = 10 * Math.Log10((sc * sc) + (cc * cc));
                        if (PdB > pBest)
                        {
                            pBest = PdB;
                            iBest = i;
                        }
                        //Debug.WriteLine(dF.ToString() + " " + PdB.ToString() + " " + sc.ToString() + " " + cc.ToString());
                    }


                    Debug.WriteLine(fTone[iBest] + " " + pBest.ToString() + " " + iBest.ToString());

                    for (int k = 0; k < validFreqRanges.Count; k++)
                    {
                        if ((validFreqRanges[k].band == band) && (fTone[iBest] >= (validFreqRanges[k].start*1e6)) && (fTone[iBest] <= (validFreqRanges[k].stop*1e6)))
                        {
                            
                            pilotFreqs[band].Add(fTone[iBest]);
                            pilotLevels[band].Add(pBest);
                        }
                    }
                }
            }
        }

        private double sinc(double x)
        {
            if (x == 0) return 1;
            else return ((Math.Sin(x * Math.PI)) / (x * Math.PI));
        }

        private double cosc(double x)
        {
            if (x == 0) return 0;
            else return ((Math.Cos(x * Math.PI)-1) / (x * Math.PI));
        }


        private void listPilotsAndSelectBest(int band)
        {
            UInt32 idealRangeStart = Convert.ToUInt32(preferredB3Range.start * 1e6);
            UInt32 idealRangeEnd = Convert.ToUInt32(preferredB3Range.stop * 1e6);

            List<int> candidateIndexes = new List<int>();


            ListBox list;
            switch (band)
            {
                case 0:
                    list = lstPilotsB1;
                    break;
                //case 1:
                //    list = lstPilotsB2;
                //    break;
                //case 2:
                //    list = lstPilotsB3;
                //    break;
                default:
                    list = lstPilotsB1;
                    break;
            }
            list.Items.Clear();            

            for (int i = 0; i < pilotFreqs[band].Count; i++)
            {
                list.Items.Add((pilotFreqs[band][i]/1e6).ToString("F6") + " MHz,   " + pilotLevels[band][i].ToString("F2") + " dB");
                if ((band != 2) || ((pilotFreqs[band][i] >= idealRangeStart) && (pilotFreqs[band][i] <= idealRangeEnd)))
                {
                    candidateIndexes.Add(i);
                }
            }

            //test other possible ranges of band 3???

            if (candidateIndexes.Count >= 2)
            {
                int index;
                int highestIndex = 0;
                double highestLevel = -100;
                int secondHighestIndex = 0;
                double secondHighestLevel = -101;

                for (int i = 0; i < candidateIndexes.Count; i++)
                {
                    index = candidateIndexes[i];
                    if (pilotLevels[band][index] > highestLevel)
                    {
                        secondHighestIndex = highestIndex;
                        secondHighestLevel = highestLevel;
                        highestIndex = index;
                        highestLevel = pilotLevels[band][index];
                    }
                    else if (pilotLevels[band][index] > secondHighestLevel)
                    {
                        secondHighestIndex = index;
                        secondHighestLevel = pilotLevels[band][index];                        
                    }
                }
                
                if (list.Items.Count >= 2)
                {
                    //   list.SelectedIndexChanged -= lstPilots_SelectedIndexChanged;
                    string highest = list.Items[highestIndex].ToString();
                    string Secondhighest = list.Items[secondHighestIndex].ToString();
                    list.Items.Clear();
                    list.Items.Add(highest);
                    list.Items.Add(Secondhighest);
                }
            }


        }


        private bool CheckIfEmptyCredentials()
        {
            if (txtSub0FreqB1.Text == "" || txtPilotsB1.Text == "")
            {
                txtSub0FreqB1.Focus();
                MessageBox.Show(EmptyCredentials, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return true;
            }
            return false;
        }

        private void OnlyNumeric(object sender, KeyPressEventArgs e)
        {
            if (!Char.IsDigit(e.KeyChar) && e.KeyChar != 46 && e.KeyChar != 8 && e.KeyChar != 13)
            {
                e.Handled = true;
            }
        }

        private List<OFDM> FillBandCmtsData()
        {
            var bandList = new List<OFDM>();


            if (!(string.IsNullOrEmpty(txtSub0FreqB1.Text) || string.IsNullOrEmpty(txtPilotsB1.Text)))
            {
                OFDM ofdmBand1Data = new OFDM()
                {
                    BandName = BAND1,
                    //SubcarrierFrequency = (Int32)(Convert.ToDouble(txtSub0FreqB1.Text) * 1e6),
                    SubcarrierFrequency = (Convert.ToDouble(txtSub0FreqB1.Text) * 1000000),
                    CyclicPrefixLength = Convert.ToInt32(ddlCycPrefixB1.Text),
                    FFTSize = rad8kB1.Checked ? (Int32)8192 : (Int32)4096,
                    PilotIndices = txtPilotsB1.Text
                };
                bandList.Add(ofdmBand1Data);
            }

            //if (!(string.IsNullOrEmpty(txtSub0FreqB2.Text) || string.IsNullOrEmpty(txtPilotsB2.Text)))
            //{
            //    OFDM ofdmBand2Data = new OFDM()
            //    {
            //        BandName = BAND2,
            //        SubcarrierFrequency = (Int32)(Convert.ToDouble(txtSub0FreqB2.Text) * 1e6),
            //        CyclicPrefixLength = Convert.ToInt32(cmbCycPrefixB2.Text),
            //        FFTSize = rad8kB2.Checked ? (Int32)8192 : (Int32)4096,
            //        PilotIndices = txtPilotsB2.Text
            //    };
            //    bandList.Add(ofdmBand2Data);
            //}

            //if (!(string.IsNullOrEmpty(txtSub0FreqB3.Text) || string.IsNullOrEmpty(txtPilotsB3.Text)))
            //{
            //    OFDM ofdmBand3Data = new OFDM()
            //    {
            //        BandName = BAND3,
            //        SubcarrierFrequency = (Int32)(Convert.ToDouble(txtSub0FreqB3.Text) * 1e6),
            //        CyclicPrefixLength = Convert.ToInt32(cmbCycPrefixB3.Text),
            //        FFTSize = rad8kB3.Checked ? (Int32)8192 : (Int32)4096,
            //        PilotIndices = txtPilotsB3.Text
            //    };
            //    bandList.Add(ofdmBand3Data);
            //}

            return bandList;

        }


        private void FillBandNonCmtsData()
        {
            int band;
            ListBox list;

            for (int j = 0; j < bandList.Count; j++)
            {
                if (bandList[j].BandName.Contains("1")) band = 0;
                else if (bandList[j].BandName.Contains("2")) band = 1;
                else band = 2;

                switch (band)
                {
                    case 0:
                        list = lstPilotsB1;
                        break;
                    //case 1:
                    //    list = lstPilotsB2;
                    //    break;
                    //case 2:
                    //    list = lstPilotsB3;
                    //    break;
                    default:
                        list = lstPilotsB1;
                        break;
                }

                bandList[j].CenterFrequency = Convert.ToInt32(GlobalVars.FreqSetting[band] * 1000000);
                bandList[j].MarkerSelection = Convert.ToInt32(GlobalVars.MarkertoUse[band] + 1);

                if (list.SelectedIndices.Count != 2)
                {
                    //MessageBox.Show("Exactly two pilots must be selected from list of Calculated Pilot Frequencies"); //FIX - should this prevent data from being sent?
                }
                else
                {
                    int i1 = list.SelectedIndices[0];
                    int i2 = list.SelectedIndices[1];

                    bandList[j].Pilot1Frequency = Convert.ToDecimal(pilotFreqs[band][i1]);
                    bandList[j].Pilot2Frequency = Convert.ToDecimal(pilotFreqs[band][i2]);
                    bandList[j].Pilot1RelativeLevelAdjustment = Convert.ToDecimal(-pilotLevels[band][i1]);
                    bandList[j].Pilot2RelativeLevelAdjustment = Convert.ToDecimal(-pilotLevels[band][i2]);
                    bandList[j].Pilot1TotalLevelAdjustment = 0;
                    bandList[j].Pilot2TotalLevelAdjustment = 0;
                }
            }            
        }


        private void AssignBandDataToTextBoxes(IEnumerable<OFDM> bandData)
        {
            //ClearTextBoxes();
            bandList.Clear();
            foreach (var band in bandData)
            {
                bandList.Add(band);
                if (band.BandName.ToUpper() == BAND1)
                {                    
                    txtSub0FreqB1.Text = (band.SubcarrierFrequency/1e6).ToString();
                    ddlCycPrefixB1.Text = band.CyclicPrefixLength.ToString();
                    if (band.FFTSize == 8192) rad8kB1.Checked = true;
                    else rad4kB3.Checked = true;
                    txtPilotsB1.Text = band.PilotIndices.ToString();

                    pilotFreqs[0].Clear();
                    pilotLevels[0].Clear();
                    if (band.Pilot1Frequency != 0)
                    {
                        pilotFreqs[0].Add(Convert.ToDouble(band.Pilot1Frequency));
                        pilotLevels[0].Add(Convert.ToDouble(-band.Pilot1RelativeLevelAdjustment));
                    }
                    if (band.Pilot2Frequency != 0)
                    {
                        pilotFreqs[0].Add(Convert.ToDouble(band.Pilot2Frequency));
                        pilotLevels[0].Add(Convert.ToDouble(-band.Pilot2RelativeLevelAdjustment));
                    }
                    listPilotsAndSelectBest(0);
                }
                //if (band.BandName.ToUpper() == BAND2)
                //{
                //    txtSub0FreqB2.Text = (band.SubcarrierFrequency/1e6).ToString();
                //    cmbCycPrefixB2.Text = band.CyclicPrefixLength.ToString();
                //    if (band.FFTSize == 8192) rad8kB2.Checked = true;
                //    else rad8kB1.Checked = true;
                //    txtPilotsB2.Text = band.PilotIndices.ToString();

                //    pilotFreqs[1].Clear();
                //    pilotLevels[1].Clear();
                //    if (band.Pilot1Frequency != 0)
                //    {
                //        pilotFreqs[1].Add(Convert.ToDouble(band.Pilot1Frequency));
                //        pilotLevels[1].Add(Convert.ToDouble(-band.Pilot1RelativeLevelAdjustment));
                //    }
                //    if (band.Pilot2Frequency != 0)
                //    {
                //        pilotFreqs[1].Add(Convert.ToDouble(band.Pilot2Frequency));
                //        pilotLevels[1].Add(Convert.ToDouble(-band.Pilot2RelativeLevelAdjustment));
                //    }
                //    listPilotsAndSelectBest(1);
                //}
                //if (band.BandName.ToUpper() == BAND3)
                //{
                //    txtSub0FreqB3.Text = (band.SubcarrierFrequency/1e6).ToString();
                //    cmbCycPrefixB3.Text = band.CyclicPrefixLength.ToString();
                //    if (band.FFTSize == 8192) rad8kB3.Checked = true;
                //    else rad4kB3.Checked = true;
                //    txtPilotsB3.Text = band.PilotIndices.ToString();

                //    pilotFreqs[2].Clear();
                //    pilotLevels[2].Clear();
                //    if (band.Pilot1Frequency != 0)
                //    {
                //        pilotFreqs[2].Add(Convert.ToDouble(band.Pilot1Frequency));
                //        pilotLevels[2].Add(Convert.ToDouble(-band.Pilot1RelativeLevelAdjustment));
                //    }
                //    if (band.Pilot2Frequency != 0)
                //    {
                //        pilotFreqs[2].Add(Convert.ToDouble(band.Pilot2Frequency));
                //        pilotLevels[2].Add(Convert.ToDouble(-band.Pilot2RelativeLevelAdjustment));
                //    }
                //    listPilotsAndSelectBest(2);
                //}
            }            
        }



        private void saveToFile()
        {
            string filename;
            string strAppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            strAppDir = strAppDir + "\\";

            filename = "OfdmParams.txt";

            StreamWriter sw = new StreamWriter(strAppDir + filename);

            GetCMTSResult ofdmData = new GetCMTSResult();
            ofdmData.Data = bandList;

            string inputJson = (new JavaScriptSerializer()).Serialize(ofdmData);
            sw.WriteLine(inputJson);

            sw.Close();
            MessageBox.Show("Parameters saved to " + filename);
        }

        private void readFromFile()
        {
            string filename;
            string strAppDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().GetModules()[0].FullyQualifiedName);
            strAppDir = strAppDir + "\\";

            filename = "OfdmParams.txt";

            if (File.Exists(strAppDir + filename))
            {
                StreamReader sr = new StreamReader(strAppDir + filename);
                string readLine = sr.ReadLine();
                var ofdmResult = (new JavaScriptSerializer()).Deserialize<GetCMTSResult>(readLine);
                AssignBandDataToTextBoxes(ofdmResult.Data);
                sr.Close();
            }
        }

        private void btnGetServer_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckIfEmptyCredentials())
                {
                    return;
                }

                object ofdmCredential = new OFDMCredential()
                {
                    UserName = txtSub0FreqB1.Text,
                    Password = txtPilotsB1.Text
                };
                string inputJson = (new JavaScriptSerializer()).Serialize(ofdmCredential);
                string getOfdmUrl = ConfigurationManager.AppSettings[GetOFDM];
                HttpClient client = new HttpClient();
                HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
                HttpResponseMessage response = client.PostAsync(baseUrl + getOfdmUrl, inputContent).Result;
                if (response.IsSuccessStatusCode)
                {
                    var cmtsResult = (new JavaScriptSerializer()).Deserialize<GetCMTSResult>(response.Content.ReadAsStringAsync().Result);
                    switch (cmtsResult.status)
                    {
                        case CMTSStatus.SuccessOnGet:
                            AssignBandDataToTextBoxes(cmtsResult.Data);
                            break;
                        case CMTSStatus.FailedOnGet:
                            MessageBox.Show(NoRecordFound, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                        case CMTSStatus.CredentialsNotValid:
                            MessageBox.Show(UserNotValid, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                            break;
                    }
                }
                else
                {
                    MessageBox.Show(FailedToConnect, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnSendServer_Click(object sender, EventArgs e)
        {
            try
            {
                if (CheckIfEmptyCredentials())
                {
                    return;
                }

                OFDMCredential ofdmCredential = new OFDMCredential()
                {
                    UserName = txtSub0FreqB1.Text,
                    Password = txtPilotsB1.Text
                };
                FillBandNonCmtsData();
                if (bandList.Count > 0)
                {
                    var ofdmDetails = new OFDMDetails { };
                    ofdmDetails.OFDMCredentials = ofdmCredential;
                    ofdmDetails.OFDMDataList = bandList;
                    string postUrl = ConfigurationManager.AppSettings[SaveOFDM];
                    string inputJson = (new JavaScriptSerializer()).Serialize(ofdmDetails);
                    HttpClient client = new HttpClient();
                    HttpContent inputContent = new StringContent(inputJson, Encoding.UTF8, "application/json");
                    HttpResponseMessage response = client.PostAsync(baseUrl + postUrl, inputContent).Result;
                    if (response.IsSuccessStatusCode)
                    {
                        var cmtsResult = (new JavaScriptSerializer()).Deserialize<SaveCMTSResult>(response.Content.ReadAsStringAsync().Result);
                        switch (cmtsResult.status)
                        {
                            case CMTSStatus.DataSaved:
                                MessageBox.Show(DataSavedSuccessfully, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.CredentialsNotValid:
                                MessageBox.Show(UserNotValid, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.RangeNotValidForSubcarrierFrequency:
                                MessageBox.Show(RangeNotValidForSubcarrierFrequency, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForCyclicPrefixLength:
                                MessageBox.Show(NotValidValueForCyclicPrefixLength, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForFFTSize:
                                MessageBox.Show(NotValidValueForFFTSize, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForCenterFrequency:
                                MessageBox.Show(NotValidValueForCenterFrequency, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForMarkerSelection:
                                MessageBox.Show(NotValidValueForMarkerSelection, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForPilot1Frequency:
                                MessageBox.Show(NotValidValueForPilot1Frequency, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForPilot2Frequency:
                                MessageBox.Show(NotValidValueForPilot2Frequency, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForPilot1RelativeLevelAdjustment:
                                MessageBox.Show(NotValidValueForPilot1RelativeLevelAdjustment, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.NotValidValueForPilot2RelativeLevelAdjustment:
                                MessageBox.Show(NotValidValueForPilot2RelativeLevelAdjustment, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                            case CMTSStatus.ProblemOnSave:
                                MessageBox.Show(ProblemOnSave, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                                break;
                        }
                    }
                }
                else
                {
                    MessageBox.Show(EmptyBandData, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /*
        private void btnCalcPilots_Click(object sender, EventArgs e)
        {
            string[] tokens;
            Char[] delimiters = new Char[] { ':', ',', ' ' };
            Char[] trimchars = new Char[] { ' ', '\r', '\n' };
            UInt32[] pilots;
            string cleanedPilotString = "";

            lstPilotsB1.Items.Clear();
            lstPilotsB2.Items.Clear();
            lstPilotsB3.Items.Clear();
           
            try
            {
                int band;
                bandList = FillBandCmtsData();
                for (int j = 0; j < bandList.Count; j++)
                {
                    if (bandList[j].BandName.Contains("1")) band = 0;
                    else if (bandList[j].BandName.Contains("2")) band = 1;
                    else band = 2;

                    cleanedPilotString = "";
                    tokens = bandList[j].PilotIndices.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);

                    pilots = new UInt32[tokens.Length];
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string tmpstr;
                        UInt32 result;
                        tmpstr = tokens[i].Trim(trimchars);


                        //handle output from certain CMTS with frequency and pilot indexes enclosed in brackets
                        int start = tmpstr.IndexOf('[');
                        if (start > -1)
                        {
                            int end = tmpstr.Substring(start).IndexOf(']');
                            if (end > 1) tmpstr = tmpstr.Substring(start + 1, end - 1);
                        }


                        if (UInt32.TryParse(tmpstr, out result))
                        {
                            pilots[i] = result;
                            cleanedPilotString += (result.ToString() + ":");
                        }
                        else pilots[i] = 0;
                        if (pilots[i] > (bandList[j].FFTSize - 1))
                        {
                            MessageBox.Show("Pilot index must be less than FFT size");
                            return;
                        }
                    }
                    cleanedPilotString = cleanedPilotString.Remove(cleanedPilotString.Length - 1);
                    //FIX - regenerate cleaned up pilot indices string?

                    if ((bandList[j].SubcarrierFrequency > 1.7e9) || (bandList[j].SubcarrierFrequency < 54e6))
                    {
                        MessageBox.Show("Subcarrier 0 Frequency out of range");
                        return;
                    }
                    if (pilots.Length < 2)
                    {
                        MessageBox.Show("Not enough pilots entered");
                        return;
                    }

                    CalculatePilots(pilots, (UInt32)bandList[j].FFTSize, (UInt32)bandList[j].CyclicPrefixLength, (UInt32)bandList[j].SubcarrierFrequency, band);

                    listPilotsAndSelectBest(band);

                    bandList[j].PilotIndices = cleanedPilotString;
                    switch (band)
                    {
                        case 0:
                            txtPilotsB1.Text = bandList[j].PilotIndices;
                            break;
                        case 1:
                            txtPilotsB2.Text = bandList[j].PilotIndices;
                            break;
                        case 2:
                            txtPilotsB3.Text = bandList[j].PilotIndices;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
        */
        private void lstPilots_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*
            //automatically select two adjacent pilots
            int ind = lstPilots.SelectedIndex;
            int num = lstPilots.Items.Count;

            lstPilots.SelectedIndexChanged -= lstPilots_SelectedIndexChanged;
            lstPilots.SelectedIndices.Clear();
            lstPilots.SelectedIndices.Add(ind);

            if (num > 1)
            {
                if (ind == (num-1))
                    lstPilots.SelectedIndices.Add(ind - 1);
                else
                    lstPilots.SelectedIndices.Add(ind+1);
            }
            lstPilots.SelectedIndexChanged += lstPilots_SelectedIndexChanged;
            */
        }

        private void btnOFDMSet_Click(object sender, EventArgs e)
        {
            int band;
            ListBox list;

            FillBandNonCmtsData();
            if (bandList.Count > 0)
            {
                for (int j = 0; j < bandList.Count; j++)
                {
                    if (bandList[j].BandName.Contains("1")) band = 0;
                    else if (bandList[j].BandName.Contains("2")) band = 1;
                    else band = 2;

                    switch (band)
                    {
                        case 0:
                            list = lstPilotsB1;
                            break;
                        //case 1:
                        //    list = lstPilotsB2;
                        //    break;
                        //case 2:
                        //    list = lstPilotsB3;
                        //    break;
                        default:
                            list = lstPilotsB1;
                            break;
                    }

                    if (list.Items.Count >= 2)
                    {
                        if (list.SelectedIndices.Count != 2)
                        {
                            MessageBox.Show("Exactly two pilots must be selected from list of Calculated Pilot Frequencies", "Band " + (band + 1).ToString());
                        }
                        else
                        {
                            int i1 = list.SelectedIndices[0];
                            int i2 = list.SelectedIndices[1];
                            //MainForm.setOFDMvalues((UInt32)band, pilotFreqs[band][i1] / 1e6, pilotFreqs[band][i2] / 1e6, -pilotLevels[band][i1], -pilotLevels[band][i2], (UInt32)bandList[j].FFTSize);
                        }
                    }
                }
                saveToFile();
            }
            else
            {
                MessageBox.Show(EmptyBandData, AppName, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

        }

        private void txtSub0FreqB1_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnlyNumeric(sender, e);
        }

        private void txtSub0FreqB2_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnlyNumeric(sender, e);
        }

        private void txtSub0FreqB3_KeyPress(object sender, KeyPressEventArgs e)
        {
            OnlyNumeric(sender, e);
        }

        private void InitializeComponent()
        {
            this.ddlCycPrefixB1 = new System.Windows.Forms.ComboBox();
            this.rad4kB3 = new System.Windows.Forms.RadioButton();
            this.rad8kB1 = new System.Windows.Forms.RadioButton();
            this.lstPilotsB1 = new System.Windows.Forms.ListBox();
            this.txtSub0FreqB1 = new System.Windows.Forms.TextBox();
            this.txtPilotsB1 = new System.Windows.Forms.TextBox();
            this.btnCalcPilots = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // ddlCycPrefixB1
            // 
            this.ddlCycPrefixB1.FormattingEnabled = true;
            this.ddlCycPrefixB1.Items.AddRange(new object[] {
            "8",
            "12",
            "512",
            "96"});
            this.ddlCycPrefixB1.Location = new System.Drawing.Point(212, 47);
            this.ddlCycPrefixB1.Name = "ddlCycPrefixB1";
            this.ddlCycPrefixB1.Size = new System.Drawing.Size(121, 21);
            this.ddlCycPrefixB1.TabIndex = 0;
            // 
            // rad4kB3
            // 
            this.rad4kB3.AutoSize = true;
            this.rad4kB3.Location = new System.Drawing.Point(212, 83);
            this.rad4kB3.Name = "rad4kB3";
            this.rad4kB3.Size = new System.Drawing.Size(38, 17);
            this.rad4kB3.TabIndex = 1;
            this.rad4kB3.TabStop = true;
            this.rad4kB3.Text = "4K";
            this.rad4kB3.UseVisualStyleBackColor = true;
            // 
            // rad8kB1
            // 
            this.rad8kB1.AutoSize = true;
            this.rad8kB1.Location = new System.Drawing.Point(261, 83);
            this.rad8kB1.Name = "rad8kB1";
            this.rad8kB1.Size = new System.Drawing.Size(38, 17);
            this.rad8kB1.TabIndex = 3;
            this.rad8kB1.TabStop = true;
            this.rad8kB1.Text = "8K";
            this.rad8kB1.UseVisualStyleBackColor = true;
            // 
            // lstPilotsB1
            // 
            this.lstPilotsB1.FormattingEnabled = true;
            this.lstPilotsB1.Location = new System.Drawing.Point(209, 248);
            this.lstPilotsB1.Name = "lstPilotsB1";
            this.lstPilotsB1.Size = new System.Drawing.Size(216, 147);
            this.lstPilotsB1.TabIndex = 6;
            // 
            // txtSub0FreqB1
            // 
            this.txtSub0FreqB1.Location = new System.Drawing.Point(212, 12);
            this.txtSub0FreqB1.Name = "txtSub0FreqB1";
            this.txtSub0FreqB1.Size = new System.Drawing.Size(134, 20);
            this.txtSub0FreqB1.TabIndex = 9;
            this.txtSub0FreqB1.Text = "120";
            // 
            // txtPilotsB1
            // 
            this.txtPilotsB1.Location = new System.Drawing.Point(209, 122);
            this.txtPilotsB1.Multiline = true;
            this.txtPilotsB1.Name = "txtPilotsB1";
            this.txtPilotsB1.Size = new System.Drawing.Size(330, 98);
            this.txtPilotsB1.TabIndex = 10;
            this.txtPilotsB1.Text = "2406:2474:2498:2520:2538:2613:2631:2653:2677:2785:2924:3063:3202:3341:3556:3659:3" +
    "798:3937:4076:4215:4354:4493:4632:4811:4950:5089:5228:5367:5506:5645:5784";
            // 
            // btnCalcPilots
            // 
            this.btnCalcPilots.Location = new System.Drawing.Point(209, 430);
            this.btnCalcPilots.Name = "btnCalcPilots";
            this.btnCalcPilots.Size = new System.Drawing.Size(161, 23);
            this.btnCalcPilots.TabIndex = 20;
            this.btnCalcPilots.Text = "CalcPilots";
            this.btnCalcPilots.UseVisualStyleBackColor = true;
            this.btnCalcPilots.Click += new System.EventHandler(this.btnCalcPilots_Click_1);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(107, 13);
            this.label1.TabIndex = 21;
            this.label1.Text = "Subcarrier Freq(MHz)";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 55);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 13);
            this.label2.TabIndex = 22;
            this.label2.Text = "Cyclic Prefix Length";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(26, 87);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 13);
            this.label3.TabIndex = 23;
            this.label3.Text = "FFT Size";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(26, 122);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(117, 13);
            this.label4.TabIndex = 24;
            this.label4.Text = "Continous Pilot Indexes";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(26, 248);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(177, 13);
            this.label5.TabIndex = 25;
            this.label5.Text = "Calculated Pilot Frequencies/Levels";
            // 
            // OFDMForm
            // 
            this.ClientSize = new System.Drawing.Size(708, 483);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnCalcPilots);
            this.Controls.Add(this.txtPilotsB1);
            this.Controls.Add(this.txtSub0FreqB1);
            this.Controls.Add(this.lstPilotsB1);
            this.Controls.Add(this.rad8kB1);
            this.Controls.Add(this.rad4kB3);
            this.Controls.Add(this.ddlCycPrefixB1);
            this.Name = "OFDMForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btnCalcPilots_Click_1(object sender, EventArgs e)
        {
            string[] tokens;
            Char[] delimiters = new Char[] { ':', ',', ' ' };
            Char[] trimchars = new Char[] { ' ', '\r', '\n' };
            UInt32[] pilots;
            string cleanedPilotString = "";

            lstPilotsB1.Items.Clear();
          //  lstPilotsB2.Items.Clear();
            //lstPilotsB3.Items.Clear();

            try
            {
                int band;
                bandList = FillBandCmtsData();
                for (int j = 0; j < bandList.Count; j++)
                {
                    if (bandList[j].BandName.Contains("1")) band = 0;
                    else if (bandList[j].BandName.Contains("2")) band = 1;
                    else band = 2;

                    cleanedPilotString = "";
                    tokens = bandList[j].PilotIndices.Split(delimiters, System.StringSplitOptions.RemoveEmptyEntries);

                    pilots = new UInt32[tokens.Length];
                    for (int i = 0; i < tokens.Length; i++)
                    {
                        string tmpstr;
                        UInt32 result;
                        tmpstr = tokens[i].Trim(trimchars);


                        //handle output from certain CMTS with frequency and pilot indexes enclosed in brackets
                        int start = tmpstr.IndexOf('[');
                        if (start > -1)
                        {
                            int end = tmpstr.Substring(start).IndexOf(']');
                            if (end > 1) tmpstr = tmpstr.Substring(start + 1, end - 1);
                        }


                        if (UInt32.TryParse(tmpstr, out result))
                        {
                            pilots[i] = result;
                            cleanedPilotString += (result.ToString() + ":");
                        }
                        else pilots[i] = 0;
                        if (pilots[i] > (bandList[j].FFTSize - 1))
                        {
                            MessageBox.Show("Pilot index must be less than FFT size");
                            return;
                        }
                    }
                    cleanedPilotString = cleanedPilotString.Remove(cleanedPilotString.Length - 1);
                    //FIX - regenerate cleaned up pilot indices string?

                    //if ((bandList[j].SubcarrierFrequency > 1.7e9) || (bandList[j].SubcarrierFrequency < 54e6))
                    if ((bandList[j].SubcarrierFrequency > 1700000000) || (bandList[j].SubcarrierFrequency < 54000000))
                    {
                        MessageBox.Show("Subcarrier 0 Frequency out of range");
                        return;
                    }
                    if (pilots.Length < 2)
                    {
                        MessageBox.Show("Not enough pilots entered");
                        return;
                    }

                    CalculatePilots(pilots, (UInt32)bandList[j].FFTSize, (UInt32)bandList[j].CyclicPrefixLength, (UInt32)bandList[j].SubcarrierFrequency, band);

                    listPilotsAndSelectBest(band);

                    bandList[j].PilotIndices = cleanedPilotString;
                    switch (band)
                    {
                        case 0:
                            txtPilotsB1.Text = bandList[j].PilotIndices;
                            break;
                        //case 1:
                        //    txtPilotsB2.Text = bandList[j].PilotIndices;
                        //    break;
                        //case 2:
                        //    txtPilotsB3.Text = bandList[j].PilotIndices;
                        //    break;
                    }

                    if (pilotFreqs[0].Count == 0)
                    {
                        MessageBox.Show("There is no Pilot Frequency/level.");
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, AppName, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
        }
    }
}