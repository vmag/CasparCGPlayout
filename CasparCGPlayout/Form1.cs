using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Net;
using System.Reflection;
using System.Security.Principal;
using System.Threading;
using System.Timers;
using System.Windows.Forms;
using Bespoke.Common.Osc;
using CasparCGPlayout.Database;
using CasparCGPlayout.ItemClasses;
using CasparCGPlayout.Properties;
using CasparCGPlayout.Utils;
using MySql.Data.MySqlClient;
using Svt.Caspar;
using Svt.Network;
using log4net.Config;
using log4net;
using System.IO;

namespace CasparCGPlayout
{
    public partial class Form1 : Form
    {
        //Main Caspar Device


        //Layers
        private const int NowNextCgLayer = 50;
        private const int VideoLayer = 1;
        private const int AnnouncerLayer = 2;
        private static readonly String VideoLayerString = VideoLayer.ToString(CultureInfo.InvariantCulture);
        private static readonly ILog Logger = LogManager.GetLogger(typeof (Form1));
        private readonly Int32 _channelId;

        //OSC Events
        private readonly String _oscFilefps = "/channel/1/stage/layer/" + VideoLayerString + "/file/frame_rate";
        private readonly String _oscFilename = "/channel/1/stage/layer/" + VideoLayerString + "/filename";
        private readonly String _oscState = "/channel/1/stage/layer/" + VideoLayerString + "/state";

        //Look & Feel
        private readonly Color _playingGreen = Color.LightGreen;
        private readonly Color _stoppedGrey = Color.DarkGray;
        private readonly CasparDevice _casparDevice = new CasparDevice();
        private readonly CurrentPlayingItem _cpi = new CurrentPlayingItem();
        private string _timeLeft;
        private string _timeLength;
        private string _timePast;
        private MySqlConnection _connection;
        private OscServer _oscServer;

        public Form1()
        {
            //Setup Logger*
#pragma warning disable 612,618
            DOMConfigurator.Configure();
#pragma warning restore 612,618

            InitializeComponent();

            //Wrap this to catch configuration errors

            try
            {
                casparServerIp = ConfigurationManager.AppSettings["ServerIP"];
                casparServerOscPort = Int32.Parse(ConfigurationManager.AppSettings["OSCPort"]);
                casparServerAmcpPort = Int32.Parse(ConfigurationManager.AppSettings["AMCPPort"]);
                casparDatabaseServerHostname = ConfigurationManager.AppSettings["MySQLHostname"];
                casparDatabaseServerUsername = ConfigurationManager.AppSettings["MySQLUser"];
                casparDatabaseServerPassword = ConfigurationManager.AppSettings["MySQLPass"];
                casparDatabaseServerDatabase = ConfigurationManager.AppSettings["MySQLDatabase"];
                casparDatabaseServerConnectiontimeout = ConfigurationManager.AppSettings["SQLConnectonTimeout"];
                segueNotifier = Int32.Parse(ConfigurationManager.AppSettings["SegueNotifier"]);
                videoMixTime = Int32.Parse(ConfigurationManager.AppSettings["VideoMixTime"]);
                _channelId = Int32.Parse(ConfigurationManager.AppSettings["Channel"]);
            }
            catch (ArgumentOutOfRangeException e)
            {
                Logger.Info(e.Message);
            }
            _cpi.index = 0;
        }

        private bool channelDogVisible { get; set; }

        //AMCP/OSC/MySQL Connection Bits
        public string casparServerIp { get; set; }
        public int casparServerOscPort { get; set; }
        public int casparServerAmcpPort { get; set; }
        public string casparDatabaseServerHostname { get; set; }
        public string casparDatabaseServerUsername { get; set; }
        public string casparDatabaseServerPassword { get; set; }
        public string casparDatabaseServerDatabase { get; set; }
        public string casparDatabaseServerConnectiontimeout { get; set; }
        public bool boolHold { get; set; }

        //OSC Vars
        public string casparAudchannels { get; set; }
        public string casparAudformat { get; set; }
        public string casparAudcodec { get; set; }
        public string casparVidwidth { get; set; }
        public string casparVidheight { get; set; }
        public string casparVidfield { get; set; }
        public string casparVidcodec { get; set; }
        public string casparPlayingName { get; set; }
        public string casparAudSampleRate { get; set; }
        public string casparFilename { get; set; }
        public string casparSpeed { get; set; }

        //Misc
        public Int32 segueNotifier { get; set; }
        public Int32 videoMixTime { get; set; }

        public bool boolLastSeconds { get; set; }

        private void form1Load(object sender, EventArgs e)
        {
            changeControlStatus(false);
            SetupLogging();
            connectDatabase();
            setupCasparCGDevice();
            ConnectOSCListener();
            populateWallDB();
            CheckUserPriveledges();
            SetupNTP();
        }

        private void SetupLogging()
        {
        }


        private void SetupNTP()
        {
            tmrNTPUpdate.Interval = Int32.Parse(ConfigurationManager.AppSettings["NTPPollInterval"]);
            tmrNTPUpdate.Enabled = true;
            DoTimeSync();
        }


        private void CheckUserPriveledges()
        {
            var pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);
            if (!hasAdministrativeRight)
                MessageBox.Show(
                    Resources.Form1_CheckUserPriveledges_It_is_recommended_to_run_this_Application_with_Elevated_Rights_to_ensure_system_time_accuracy);
        }

        private void DoTimeSync()
        {
            var ntp = new NTPClient(ConfigurationManager.AppSettings["NTPServer"]);
            ntp.connect(true);
            //if (ntp.IsResponseValid()) { listBox1.Items.Add("Successfully Sync'd time"); } else { listBox1.Items.Add("+++ Issue syncing Time +++"); }
        }

        public void lblUpMessage(String info)
        {
            if (lblCountUp.InvokeRequired)
            {
                LblUpMessageDel method = lblUpMessage;
                lblCountUp.Invoke(method, new object[] {info});
                return;
            }

            lblCountUp.Text = info;
        }

        public void lblDownMessage(String info)
        {
            if (lblCountDown.InvokeRequired)
            {
                LblDownMessageDel method = lblDownMessage;
                lblCountDown.Invoke(method, new object[] {info});
                return;
            }

            lblCountDown.Text = info;
        }

        public void lblLengthMessage(String info)
        {
            if (lblLength.InvokeRequired)
            {
                LblLengthMessageDel method = lblLengthMessage;
                lblLength.Invoke(method, new object[] {info});
                return;
            }

            lblLength.Text = info;
        }

        private void handleOSCMessage(OscMessage oscMessage)
        {
            Logger.Info(oscMessage.Address);

            if (oscMessage.Address == _oscFilefps)
            {
                _cpi.framerate = Convert.ToInt32(Double.Parse(oscMessage.Data[0].ToString()));
                // logger.Info(cpi.Framerate);
                
            }
            else if (oscMessage.Address == _oscState)
            {
                CheckPlayingStatus(oscMessage);
            }
            else if (oscMessage.Address == _oscFilename)
            {
                casparPlayingName = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/frame")
            {
                //logger.Info(oscMessage.Data[0].ToString());
                if (_cpi != null && _cpi.framerate != 0)
                updateDisplay(oscMessage);
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/path")
            {
                casparFilename = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/audio/sample-rate")
            {
                casparAudSampleRate = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/audio/channels")
            {
                casparAudchannels = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/audio/format")
            {
                casparAudformat = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/audio/codec")
            {
                casparAudcodec = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/video/width")
            {
                casparVidwidth = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/video/height")
            {
                casparVidheight = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/video/field")
            {
                casparVidfield = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/" + VideoLayerString + "/file/video/codec")
            {
                casparVidcodec = oscMessage.Data[0].ToString();
            }
            else if (oscMessage.Address == "/channel/1/mixer/audio/pFS")
            {
                double dbfs = double.Parse(oscMessage.Data[0].ToString());

               // Invoke(new MethodInvoker(delegate { progressBar1.Value = (int)Math.Round(dbfs * 10000); }));


                //Console.WriteLine(.ToString());
            }
            else if (oscMessage.Address == "/channel/1/stage/layer/1/speed")
            {
                casparSpeed = oscMessage.Data[0].ToString();
            }
        }

        

        private void updateDisplay(OscMessage oscMessage)
        {
            Int32 frame = Convert.ToInt32(double.Parse(oscMessage.Data[0].ToString()));
            
            _cpi.position = frame - _cpi.inFrames;
            _cpi.length = Convert.ToInt32(double.Parse(oscMessage.Data[1].ToString()));


            _timePast = TimeUtils.frameToHHMMSSFF(_cpi.position, _cpi.framerate);
            _timeLeft = TimeUtils.frameToHHMMSSFF(((_cpi.length - _cpi.inFrames) - _cpi.outFrames) - _cpi.position, _cpi.framerate);
            _timeLength = TimeUtils.frameToHHMMSSFF(((_cpi.length - _cpi.inFrames) - _cpi.outFrames), _cpi.framerate);

            Logger.Debug("Frame: " + _cpi.position + " of " + _cpi.length + " (" + _timePast + " / " + _timeLeft + ")\n");

            checkItemPlayingLastFrames((_cpi.length - _cpi.inFrames) - _cpi.outFrames);
            //CheckItemDuration();

            try
            {
                Invoke(new MethodInvoker(delegate
                                             {
                                                 lblCountDown.Text = _timeLeft;
                                                 lblCountUp.Text = _timePast;
                                                 lblLength.Text = _timeLength;
                                             }));
            }
            catch (InvalidOperationException ex)
            {
                Logger.Info(ex.Message);
            }
        }

        private void CheckPlayingStatus(OscMessage oscMessage)
        {
            switch ((String) oscMessage.Data[0])
            {
                case "playing":
                    //FindCurrentPlayingVideoandSetAsCurrentItem(caspar_playing_name);
                    break;
                case "stopped":
                    break;
            }
        }


        private void checkItemPlayingLastFrames(Double p)
        {
            Double i = p - (_cpi.framerate*segueNotifier);

            Logger.Debug("Position: " + _cpi.position + " - Looking for " + i.ToString(CultureInfo.InvariantCulture));
            Logger.Debug("tmrLastSeconds: " + tmrLastSeconds.Enabled);
            //If current position is 5 seconds behind the outframes (p) then tmrLastSeconds = true;
            if ((_cpi.position >= i) && (_cpi.position <= _cpi.length))
            {
                Logger.Debug("Found @: " + _cpi.position);

                if (tmrLastSeconds.Enabled == false)
                {
                    Invoke(new MethodInvoker(delegate
                                                 {
                                                     Logger.Info("In Last Seconds, Standby...");
                                                     tmrLastSeconds.Enabled = true;
                                                     tmrLastSeconds.Start();
                                                 }));

                    Logger.Debug("just enabled tmrLastSeconds");
                }
            }

            /*//CATCH EOF - This is buggy as hell, FFMPEG producer doesn't report frame length properly
            if (cpi.Position >= cpi.Length-5)
            {
                StopLastSecondsandResetColor();
            }
            */

            //If current position is at or greater than outframes (p) then go next
            if (tmrLastSeconds.Enabled)
            {
                if (_cpi.position == p)
                {
                    Logger.Info("******* GO NEXT *******");
                    _cpi.index++;
                    GoNext();
                }
            }
        }

        private void GoNext()
        {
            var item = (ListBoxItem) ListRunningOrder.Items[_cpi.lastPlayedIndex];

            if (!boolHold)
            {
                Invoke(new MethodInvoker(delegate
                                             {
                                                 stopLastSecondsandResetColor();

                                                 item.isPlaying = false;
                                                 item = (ListBoxItem) ListRunningOrder.Items[_cpi.index];

                                                 if (_cpi.index == ListRunningOrder.Items.Count)
                                                 {
                                                     _cpi.index = 0;
                                                 } //Loop
                                                 ListRunningOrder.SetSelected(_cpi.index, true);

                                                 _cpi.lastPlayedIndex = _cpi.index;
                                                 _cpi.inFrames = item.inFrames;
                                                 _cpi.outFrames = item.outFrames;

                                                 switch (item.type)
                                                 {
                                                     case TypeEnum.Video:
                                                         buildPlayVideo(ListRunningOrder.SelectedIndex);
                                                         break;
                                                     case TypeEnum.CG:
                                                         buildAddCg(ListRunningOrder.SelectedIndex);
                                                         break;
                                                 }

                                                 ListRunningOrder.Invalidate();
                                             }));
            }
            else
            {
                switch (item.type)
                {
                    case TypeEnum.Video:
                        stopLastSecondsandResetColor();
                        _casparDevice.Channels[_channelId].Stop(VideoLayer);
                        break;
                }
            }
        }


        private void stopLastSecondsandResetColor()
        {
            Logger.Debug("StopLastSecondsandResetColor");
            tmrLastSeconds.Enabled = false;
            boolLastSeconds = false;
            lblCountDown.BackColor = SystemColors.ControlDark;
            lblRemainDesc.BackColor = SystemColors.ControlDark;
            panel3.BackColor = SystemColors.ControlDark;
            _casparDevice.Channels[_channelId].Stop();
        }

        private void buildAddCg(int selectedIndex)
        {
            var item = (ListBoxCGItem) ListRunningOrder.Items[_cpi.index];

#pragma warning disable 612,618
            var cg = new CasparCGItem("BoltonNS", NowNextCgLayer, false);
#pragma warning restore 612,618

            var cgxml = new List<CGDataPair>{new CGDataPair("f0", "Olly Murs"), new CGDataPair("f1", "Dance with me tonight")};
            cg.Data.AddRange(cgxml);

            //CasparItem c_item = new CasparItem(VIDEO_LAYER, "AMB", new Transition(TransitionType.MIX, 300));
            //casparDevice.Channels[CHANNEL_ID].Load(c_item);
            //casparDevice.Channels[CHANNEL_ID].Play(c_item.VideoLayer);


            var cItem2 = new CasparItem(AnnouncerLayer, "test", new Transition(TransitionType.MIX, 50));
            _casparDevice.Channels[_channelId].Load(cItem2);
            _casparDevice.Channels[_channelId].Play(cItem2.VideoLayer);


#pragma warning disable 612,618
            _casparDevice.Channels[_channelId].CG.Add(cg, false);
#pragma warning restore 612,618
            _casparDevice.Channels[_channelId].CG.Play(NowNextCgLayer);

            _cpi.framerate = 25;
            _cpi.position = 0;
            _cpi.length = item.lengthOfClip.Seconds*25;
            tmrCGTiming.Enabled = true;

            var myTimer = new System.Timers.Timer();
            myTimer.Elapsed += cgTimerfinish;
            myTimer.Interval = item.lengthOfClip.TotalMilliseconds;
            myTimer.Start();

            item.isPlaying = true;
            tmrClockStarts.Enabled = false;
        }

        private void buildPlayVideo(int currentPlayingItemIndex)
        {
            var item = (ListBoxVideoItem) ListRunningOrder.Items[_cpi.index];
            //CasparItem c_item = new CasparItem(VIDEO_LAYER, item.ClipID, new Transition(TransitionType.MIX, iVideoMixTime), item.InFrames);

            var cItem = new CasparItem(VideoLayer, item.clipID, new Transition(TransitionType.CUT, 0), item.inFrames);

            Logger.Info("Built CasparItem: " + cItem);

            _casparDevice.Channels[_channelId].LoadBG(cItem);

            Logger.Info("Playing CasparItem: " + cItem);

            _casparDevice.Channels[_channelId].Play(cItem.VideoLayer);
            item.isPlaying = true;
            tmrClockStarts.Enabled = false;
        }

        private void checkItemDuration()
        {
            TmrNoItemDuration.Enabled = _cpi.length == 0;
        }

        private void tmrNtpUpdateTick(object sender, EventArgs e)
        {
            DoTimeSync();
        }

        private void ListBox1DoubleClick(object sender, EventArgs e)
        {
            string[] all = Assembly.GetEntryAssembly().GetManifestResourceNames();

            foreach (string one in all)
            {
                listBox1.Items.Add(one);
            }
        }

        #region GUI

        private void form1FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_casparDevice.IsConnected)
            {
                if (CheckAvailableChannels(_casparDevice, _channelId))
                    _casparDevice.Channels[_channelId].Stop();
            }
        }

        private void TslAboutClick(object sender, EventArgs e)
        {
            ShowAbout();
        }

        private void BtnHoldClick(object sender, EventArgs e)
        {
            if (btnHold.BackColor == Color.Silver)
            {
                btnHold.Text = "HOLDING ITEM";
                btnHold.BackColor = Color.Red;
                boolHold = true;
            }
            else
            {
                btnHold.Text = "Hold";
                btnHold.BackColor = Color.Silver;
                boolHold = false;
            }
        }

        private void TsLabelStopallClick(object sender, EventArgs e)
        {
            stopLastSecondsandResetColor();
            _casparDevice.Channels[_channelId].Clear();
        }

        private void ShowAbout()
        {
            var ab = new AboutBox
                         {
                             AppTitle = "CasparCGPlayout",
                             AppDescription =
                                 "A vague attempt at a client for using CasparCG to playout a TV station, complete with Graphics & Programmes",
                             AppVersion = "1.0.0",
                             AppCopyright = "(c) Andy Mace <andy.mace@mediauk.net> 2012",
                             AppMoreInfo = "",
                             AppDetailsButton = true
                         };

            ab.ShowDialog(this);
        }

        #endregion

        #region Connects

        private void setupCasparCGDevice()
        {
            _casparDevice.Settings.Hostname = casparServerIp;
            _casparDevice.Settings.Port = casparServerAmcpPort;
            _casparDevice.Settings.AutoConnect = false;
            _casparDevice.Connected += casparAmcpConnected;
            _casparDevice.Disconnected += caspar_AMCP_Disconnected;
            _casparDevice.FailedConnect += casparDevice_AMCPFailed_Connect;
            _casparDevice.Connect();
        }

        private Boolean CheckAvailableChannels(CasparDevice casparDevice, int channelId)
        {
            if (casparDevice.Channels.Count >= channelId + 1)
                return true;
            else
                return false;
        }

        private void ConnectOSCListener()
        {
            if (_casparDevice.IsConnected)
            {
                Logger.Info("OSC Client Connect");
                _oscServer = new OscServer(TransportType.Tcp, IPAddress.Parse(casparServerIp), casparServerOscPort);

                _oscServer.MessageReceived += oscServerMessageReceived;
                //osc_server.Connected += new OscServerConnectedHandler(osc_server_Connected);
                //osc_server.Disconnected += new OscServerDisconnectedHandler(osc_server_Disconnected);
                //Need to build in Connected/Disconnected/FailedConnect Event Handlers
                _oscServer.FilterRegisteredMethods = false;
                _oscServer.Start();
            }
        }

        private void connectDatabase()
        {
            String connectionString = DatabaseConnectionUtils.CreateConnStr(casparDatabaseServerHostname,
                                                                            casparDatabaseServerDatabase,
                                                                            casparDatabaseServerUsername,
                                                                            casparDatabaseServerPassword,
                                                                            casparDatabaseServerConnectiontimeout);
            _connection = new MySqlConnection(connectionString);
            _connection.StateChange += connectionStateChange;

            try
            {
                _connection.Open();
            }
            catch (MySqlException e)
            {
                if (
                    Resources.Form1_connectDatabase_FATAL__Can_t_connect_to_MySQL_Server__Exiting != null && MessageBox.Show(text: Resources.Form1_connectDatabase_FATAL__Can_t_connect_to_MySQL_Server__Exiting, caption: Resources.Form1_connectDatabase_CasparCG_Playout, buttons: MessageBoxButtons.OK) == DialogResult.OK)
                {
                    Application.Exit();
                }
            }
        }

       
        private void populateWallDB()
        {
            const string command = "SELECT * FROM items_media, media, playlist WHERE media.item_id = items_media.id AND playlist.item_id = media.id";
            new MySqlDataAdapter(command, _connection);
            using (var cmd = new MySqlCommand(command, _connection))
            {
                try
                {
                    MySqlDataReader rdr = cmd.ExecuteReader();


                    while (rdr.Read())
                    {
                        switch ((TypeEnum)Enum.Parse(typeof (TypeEnum), rdr.GetString("type")))
                        {
                            case TypeEnum.Video:
                                var videoitem = new ListBoxVideoItem(1, "00:00:00", rdr.GetString("item_id"),
                                                                     rdr.GetString("display1"), rdr.GetString("display2"),
                                                                     rdr.GetTimeSpan("timespan"),
                                                                     (TypeEnum) Enum.Parse(typeof (TypeEnum), rdr.GetString("type")),
                                                                     (WhatNextEnum) Enum.Parse(typeof (WhatNextEnum), rdr.GetString("whatnext")),
                                                                     rdr.GetInt32("inframes"), rdr.GetInt32("outframes"), BytestoImage((byte[])rdr["thumbnail"]));
                                ListRunningOrder.Items.Add(videoitem);
                                break;
                            case TypeEnum.CG:
                                var cgitem = new ListBoxCGItem(1, "00:00:00", rdr.GetString("item_id"),
                                                               rdr.GetString("display1"), rdr.GetString("display2"),
                                                               rdr.GetTimeSpan("timespan"),
                                                               (TypeEnum)
                                                               Enum.Parse(typeof (TypeEnum), rdr.GetString("type")),
                                                               (WhatNextEnum)
                                                               Enum.Parse(typeof (WhatNextEnum), rdr.GetString("whatnext")),
                                                               rdr.GetInt32("inframes"), rdr.GetInt32("outframes"), BytestoImage((byte[])rdr["thumbnail"]));
                                ListRunningOrder.Items.Add(cgitem);
                                break;
                        }
                    }
                }

                catch (Exception)
                {
                    Logger.Info("Can't create playlist");
                }
            }

            if (ListRunningOrder.Items.Count != 0)
            {
                ListRunningOrder.SelectedIndex = 0;
                _cpi.index = 0;
            }
        }

        private Image BytestoImage(byte[] p)
        {
            MemoryStream ms = new MemoryStream(p);
            return Image.FromStream(ms);
        }

        #endregion

        #region Event Handlers

        private void connectionStateChange(object sender, StateChangeEventArgs e)
        {
            Logger.Info("Mysql ConnInfoonState: " + e.CurrentState);
            if (e.CurrentState == ConnectionState.Open)
            {
                toolstriplabelDBConnected.BackColor = Color.Green;
                toolstriplabelDBConnected.Text = Resources.Form1_ConnectionStateChange_DB_Connected;
            }

            if (e.CurrentState == ConnectionState.Closed)
            {
                toolstriplabelDBConnected.BackColor = Color.Red;
                toolstriplabelDBConnected.Text = Resources.Form1_ConnectionStateChange_DB_Disconnected;
            }
        }

        private void oscServerDisconnected(object sender)
        {
            Logger.Info("DisconnectInfo");
        }

        private void oscServerConnected(object sender)
        {
            Logger.Info(Resources.Form1_tmrConnectionCheck_Tick_Connected);
        }


        private void oscServerMessageReceived(object sender, OscMessageReceivedEventArgs e)
        {
            handleOSCMessage(e.Message);
        }

        private void casparAmcpConnected(object sender, NetworkEventArgs e)
        {
            if (e == null) throw new ArgumentNullException("e");
            /* if (!CheckAvailableChannels(casparDevice, CHANNEL_ID))
            {
                if (MessageBox.Show("CasparCG Server does not have requested Channel: " + CHANNEL_ID + "\nPlease reconfigure!", "Error", MessageBoxButtons.OK, MessageBoxIcon.Exclamation) == DialogResult.OK)
                {
                    Application.Exit();
                }
                else { Application.Exit(); }
            }
            */

            Logger.Info("Caspar AMCInfoient Connected");
            //tmrConnectionCheck.Enabled = true;
            tmrClockStarts.Enabled = true;
            changeControlStatus(true);

            Invoke(new MethodInvoker(delegate
                                         {
                                             toolstriplabelAMCPConnected.BackColor = Color.Green;
                                             toolstriplabelAMCPConnected.Text = "AMCP Connected";
                                         }));
        }

        private void caspar_AMCP_Disconnected(object sender, NetworkEventArgs e)
        {
            Logger.Info("Caspar AMCInfoient Disconnected");
            casparDevice_AMCPFailed_Connect(sender, e);
        }

        private void casparDevice_AMCPFailed_Connect(object sender, NetworkEventArgs e)
        {
            tmrConnectionCheck.Enabled = false;
            tmrClockStarts.Enabled = false;

            Invoke(new MethodInvoker(delegate
                                         {
                                             toolstriplabelAMCPConnected.BackColor = Color.Red;
                                             toolstriplabelAMCPConnected.Text = Resources.Form1_casparDevice_AMCPFailed_Connect_AMCP_Disconnected;
                                         }));
            //MessageBox.Show("Caspar Device has gone away");
            changeControlStatus(false);

            DialogResult result =
                MessageBox.Show(
                    Resources.Form1_casparDevice_AMCPFailed_Connect_Cannot_connect_to_CasparCG_Server__ + casparServerIp + "\nDo you wish to wait to reconnect?",
                    Resources.Form1_tmrConnectionCheck_Tick_Important_Query, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result != DialogResult.Yes)
            {
                Application.Exit();
            }

            _casparDevice.Connect();
        }

        #endregion

        #region Misc

        private void changeControlStatus(bool p)
        {
            Invoke(new MethodInvoker(delegate
                                         {
                                             foreach (Control c in Controls)
                                             {
                                                 c.Enabled = p;
                                                 //c.Visible = p;
                                             }
                                         }));
        }

        #endregion

        #region Timer Ticks

        private void tmrNoItemDurationTick(object sender, EventArgs e)
        {
            panel4.BackColor = panel4.BackColor == Color.AliceBlue ? Color.Red : Color.AliceBlue;
        }

        private void tmrLastSecondsTick(object sender, EventArgs e)
        {
            Logger.Debug("tmrLastSeconds Tick");
            if (panel3.BackColor == SystemColors.ControlDark)
            {
                panel3.BackColor = Color.Red;
                lblCountDown.BackColor = Color.Red;
                lblRemainDesc.BackColor = Color.Red;
            }
            else
            {
                panel3.BackColor = SystemColors.ControlDark;
                lblCountDown.BackColor = SystemColors.ControlDark;
                lblRemainDesc.BackColor = SystemColors.ControlDark;
            }
        }

        private void tmrConnectionCheckTick(object sender, EventArgs e)
        {
            if (_connection == null)
            {
                Invoke(new MethodInvoker(delegate
                                             {
                                                 toolstriplabelDBConnected.BackColor = Color.Red;
                                                 toolstriplabelDBConnected.Text = Resources.Form1_tmrConnectionCheckTick_Disconnected;
                                             }));
                //MessageBox.Show("Caspar Device has gone away");
                changeControlStatus(false);

                DialogResult result =
                    MessageBox.Show(
                        Resources.Form1_tmrConnectionCheck_Tick_Cannot_connect_to_Database_Server__ + casparDatabaseServerHostname +
                        Resources.Form1_tmrConnectionCheck_Tick_, Resources.Form1_tmrConnectionCheck_Tick_Important_Query, MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);
                if (result != DialogResult.Yes)
                {
                    Application.Exit();
                }

                //connection.Open();
            }
            else
            {
                toolstriplabelDBConnected.BackColor = Color.Green;
                toolstriplabelDBConnected.Text = Resources.Form1_tmrConnectionCheck_Tick_Connected;
            }

            Logger.Debug("ConnectionCheck Ticker Ticking");
        }

        private void timer1Tick1(object sender, EventArgs e)
        {
            /* DateTime basetime = DateTime.Now;

            

            
            exListBox1.Items[exListBox1.SelectedIndex];
            List<Rectangle> rects = firstitem.GetRects();
            firstitem.TimeStart = TimeUtils.Fix0Padding(basetime.Hour) + ":" + TimeUtils.Fix0Padding(basetime.Minute) + ":" + TimeUtils.Fix0Padding(basetime.Second);
            exListBox1.Invalidate(rects[0]);

            DateTime dt = basetime;

            for (int i = exListBox1.SelectedIndex + 1; i < (exListBox1.Items.Count - exListBox1.SelectedIndex); i++)
            {
                exListBoxVideoItem item = ((exListBoxVideoItem)exListBox1.Items[i]);
                dt = dt.Add(item.LengthOfClip);

                item.TimeStart = TimeUtils.Fix0Padding(dt.Hour) + ":" + TimeUtils.Fix0Padding(dt.Minute) + ":" + TimeUtils.Fix0Padding(dt.Second);
                List<Rectangle> r = item.GetRects();
                exListBox1.Invalidate(r[0]);
            }
            */
        }

        private void cgTimerfinish(object source, ElapsedEventArgs e)
        {
            ((System.Timers.Timer) source).Stop();
            _casparDevice.Channels[_channelId].CG.Stop(NowNextCgLayer);
            tmrCGTiming.Enabled = false;
            //Wait for CG to finish somehow... Very Hacky 
            Thread.Sleep(1000);

            GoNext();
        }

        #endregion

        #region BtnOnClicks

        private void btnGoNextClick(object sender, EventArgs e)
        {
            _cpi.index = ListRunningOrder.SelectedIndex;
            GoNext();
        }

        private void btnDogToggleClick(object sender, EventArgs e)
        {
           
            if (!channelDogVisible)
            {
                btnDogToggle.Text = Resources.Form1_btnDogToggle_Click_Channel_Dog_Enabled;
                btnDogToggle.BackColor = _playingGreen;
                channelDogVisible = true;

                //ICGComponentData cg = new ICGComponentData();
                
               

                //_casparDevice.Channels[_channelId].CG.Add(cg, false);
                //_casparDevice.Channels[_channelId].CG.Play(cg.Layer);
            }
            else
            {
                if (Resources.Form1_btnDogToggle_Click_Channel_Dog_Disabled != null)
                    btnDogToggle.Text = Resources.Form1_btnDogToggle_Click_Channel_Dog_Disabled;
                btnDogToggle.BackColor = _stoppedGrey;
                channelDogVisible = false;
                _casparDevice.Channels[_channelId].CG.Stop(500);
            }
        }

        #endregion

        #region Nested type: lblDownMessageDel

        private delegate void LblDownMessageDel(String info);

        #endregion

        #region Nested type: lblLengthMessageDel

        private delegate void LblLengthMessageDel(String info);

        #endregion

        #region Nested type: lblUpMessageDel

        private delegate void LblUpMessageDel(String info);

        #endregion
    }
}