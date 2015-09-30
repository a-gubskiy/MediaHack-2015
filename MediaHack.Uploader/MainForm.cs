using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MediaHack.Core;

namespace MediaUploader
{
    public partial class MainForm : Form
    {
        private Cloud _cloud;

        public MainForm()
        {
            InitializeComponent();

            _cloud = new Cloud();
        }



        private void BrowseFile(object sender, EventArgs e)
        {

        }

        private void UploadVideo(object sender, EventArgs e)
        {
            var path = txtPath.Text;

            if (!File.Exists(path))
            {
                MessageBox.Show("Please selec file");
            }
            else
            {
                _cloud.Upload(path);
                LoadVideoList();
                MessageBox.Show("Video uploadad");
            }
        }

        private void OpenVideoOnWebApp(object sender, EventArgs e)
        {

        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void LoadVideoList()
        {
            var videos = _cloud.GetVideos();
            lstVideos.Items.AddRange();
        }
    }
}
