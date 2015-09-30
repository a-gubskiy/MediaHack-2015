using MediaHack.Core;
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MediaUploader
{
    public partial class MainForm : Form
    {
        private readonly Cloud _cloud;


        private string SiteUrl = "http://example.com/video/";

        public MainForm()
        {
            InitializeComponent();
            var configPath = @"d:\config.json";

            _cloud = new Cloud(configPath);
        }

        private void BrowseFile(object sender, EventArgs e)
        {
            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                txtPath.Text = openFileDialog.FileName;
            }
        }

        private void UploadVideo(object sender, EventArgs e)
        {
            var path = txtPath.Text;

            if (!File.Exists(path))
            {
                MessageBox.Show("Please select file", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var worker = new BackgroundWorker();

            EnableControls(false);

            worker.DoWork += delegate
            {
                _cloud.Upload(path);
            };

            worker.RunWorkerCompleted += delegate
            {
                EnableControls(true);
                LoadVideoList();

                MessageBox.Show("Video uploaded!", this.Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            worker.RunWorkerAsync();

        }

        private void OpenVideoOnWebApp(object sender, EventArgs e)
        {
            var video = lstVideos.SelectedItem as Video;

            if (video != null)
            {
                var url = String.Format("{0}{1}", SiteUrl, video.Name);
                System.Diagnostics.Process.Start(url);
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            LoadVideoList();
        }

        private void LoadVideoList()
        {
            var worker = new BackgroundWorker();

            EnableControls(false);

            worker.DoWork += delegate
            {
                var videos = _cloud.GetVideos().ToArray();

                SafeInvoke(lstVideos, delegate
                {
                    lstVideos.Items.Clear();
                    lstVideos.Items.AddRange(videos);
                });
            };

            worker.RunWorkerCompleted += delegate
            {
                EnableControls(true);
            };

            worker.RunWorkerAsync();

        }

        private void EnableControls(bool state)
        {   
            progressBar.Visible = !state;
            btnBrowse.Enabled = state;
            btnUpload.Enabled = state;
            txtPath.Enabled = state;
            btnRefresh.Enabled = state;
            btnOpenVideo.Enabled = state;
        }

        private static void SafeInvoke(Control control, Action action)
        {
            if (control.InvokeRequired)
            {
                control.Invoke((MethodInvoker)delegate { action(); });
            }
            else
            {
                action();
            }
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            LoadVideoList();
        }
    }
}
