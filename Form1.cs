using AutoBao.Helper;
using KAutoHelper;
using IronOcr;

namespace AutoBao
{
    public partial class Form1 : Form
    {
        #region data
        bool isStop = false;
        string linkText;
        decimal timeDelay;
        Bitmap btnPlayBitMap;

        #endregion

        public Form1()
        {
            InitializeComponent();
            btnPlayBitMap = ImageScanOpenCV.GetImage(Constants.Img.btnPlay);
        }


        private void eventBtnStart(object sender, EventArgs e)
        {
            //
            richTextBox1.Text = convertImageToText("C:\\Users\\pro\\Desktop\\aaaaa.png");
            //
            return;
            Task t = new Task(() => {
                isStop = false;
                RunStart();
            });
            t.Start();
        }

        private string convertImageToText(string pathImage)
        {
            var result = string.Empty;
            var Ocr = new IronTesseract(); // nothing to configure
            Ocr.Language = OcrLanguage.English;
            Ocr.Configuration.TesseractVersion = TesseractVersion.Tesseract5;
            using (var Input = new OcrInput())
            {
                Input.AddImage(pathImage);
                var Result = Ocr.Read(Input);
                result = Result.Text;
            }

            return result;
        }
        void RunStart()
        {
            try
            {
                if (button1.InvokeRequired)
                {
                    button1.Invoke(new Action(RunStart));
                    return;
                }

                linkText = richTextBox1.Text;
                timeDelay = numericUpDown1.Value;
                if (string.IsNullOrEmpty(linkText) || !Regex.isValidUrl(linkText))
                {
                    MessageBox.Show("Nhập Link Zô");
                    return;
                }
                Auto();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        void Auto()
        {
            List<string> devices = new List<string>();
            devices = ADBHelper.GetDevices();

            foreach (var deviceID in devices)
            {
                Task t = new Task(() => {
                    while (true)
                    {
                        if (isStop)
                            return;
                        ADBHelper.Key(deviceID, ADBKeyEvent.KEYCODE_HOME); // home

                        if (isStop)
                            return;
                        ADBHelper.TapByPercent(deviceID, 39.4, 43.0); // click vào zalo

                        if (isStop)
                            return;
                        ADBHelper.TapByPercent(deviceID, 48.5, 76.0); // click vao dang nhap

                        if (isStop)
                            return;
                        ADBHelper.TapByPercent(deviceID, 17.0, 30.3); // click quyen pass

                        if (isStop)
                            return;
                        ADBHelper.TapByPercent(deviceID, 15.5, 17.7); // click input

                        if (isStop)
                            return;
                        ADBHelper.InputText(deviceID, linkText); // input phone

                        if (isStop)
                            return;
                        ADBHelper.Key(deviceID, ADBKeyEvent.KEYCODE_ENTER); // enter url

                        if (isStop)
                            return;
                        ADBHelper.TapByPercent(deviceID, 71.3, 59.4); // click xac nhan

                        if (isStop)
                            return;
                        ADBHelper.TapByPercent(deviceID, 71.3, 59.4); // click xac nhan

                        if (isStop)
                            return;

                        while (true)
                        {
                            var mainBitmap = ADBHelper.ScreenShoot(deviceID);
                            mainBitmap.Save(Constants.Img.mainScreen);
                            var positionBtnPlay = ImageScanOpenCV.FindOutPoint(mainBitmap, btnPlayBitMap, 0.5);

                            if (positionBtnPlay != null)
                            {
                                ADBHelper.Tap(deviceID, positionBtnPlay.Value.X, positionBtnPlay.Value.Y);
                                break;
                            }

                            ADBHelper.SwipeByPercent(deviceID, 49.3, 72.4, 49.3, 59.7); // scroll
                        }

                        ADBHelper.ExecuteCMD(" adb shell svc data disable");
                        Delay((int)timeDelay*1000);
                        ADBHelper.ExecuteCMD(" adb shell svc data enable");
                        Delay(3);
                    }
                });
                t.Start();
            }
        }

        void Delay(int delay)
        {
            while (delay > 0)
            {
                Thread.Sleep(TimeSpan.FromSeconds(1));
                delay--;
                if (isStop)
                    break;
            }
        }

        private void eventBtnStop(object sender, EventArgs e)
        {
            isStop = true;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {

        }
    }
}