using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using QuickShare.Patch.Common;
using QuickShare.Patch.Common.Models;
using QuickShare.Patch.Common.Util;

namespace QuickShare.Patcher.IISPlugin
{
    public partial class FrmPage : Form, IPagePlugin
    {
        private readonly List<PageItem> pageItems = new List<PageItem>();
        private PageItem itmSiteName;
        private PageItem itmPoolName;
        private PageItem itmLogicPath;
        private PageItem itmType;
        private PageItem itmIp;
        private PageItem itmPort;
        private PageItem itmHostName;

        public FrmPage()
        {
            InitializeComponent();

            InitPageItems();

            InitControls();

            InitEvents();

            this.VisibleChanged += FrmPage_VisibleChanged;
        }

        private void InitControls()
        {
            this.cmbType.Items.AddRange(new[] { "http", "https" });
            this.cmbType.SelectedIndex = 0;
            this.itmType.Value = this.cmbType.Text.Trim();

            this.cmbIpAddress.Items.Add("全部未分配");
            IPHostEntry he = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ipAddress in he.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork))
            {
                var ip = ipAddress.ToString();
                this.cmbIpAddress.Items.Add(ip);
            }
            this.cmbIpAddress.SelectedIndex = 0;
            this.itmIp.Value = "*";
        }

        private void FrmPage_VisibleChanged(object sender, EventArgs e)
        {
            if (this.Visible)
            {
                InvokeInstallChange();
            }
        }

        private void InvokeInstallChange()
        {
            var hasInvalid = pageItems.Any(x => x.IsRequired && string.IsNullOrEmpty(x.Value));
            this.InstallChanged?.Invoke(this, new InstallEventArgs()
            {
                AllowNext = !hasInvalid
            });
        }

        private void InitEvents()
        {
            this.cmbType.SelectedIndexChanged += OnChanged;
            this.txtHostName.TextChanged += OnChanged;
            this.cmbIpAddress.SelectedIndexChanged += OnChanged;
            this.txtLogicPath.TextChanged += OnChanged;
            this.txtPoolName.TextChanged += OnChanged;
            this.txtSiteName.TextChanged += OnChanged;
            this.txtPort.TextChanged += OnChanged;
        }

        private readonly Regex ipRegex = new Regex(@"^((1[0-9]{2}|[1-9][0-9]|[0-9]|2([0-4][0-9]|5[0-5]))\.){3}(1[0-9]{2}|[1-9][0-9]|[0-9]|2([0-4][0-9]|5[0-5]))$");
        private void OnChanged(object sender, EventArgs args)
        {
            if (sender == this.txtSiteName && this.itmPoolName.Value == this.itmSiteName.Value)
            {
                this.txtPoolName.Text = this.txtSiteName.Text;
            }
            this.itmType.Value = this.cmbType.Text.Trim();
            this.itmHostName.Value = this.txtHostName.Text.Trim();
            this.itmLogicPath.Value = this.txtLogicPath.Text.Trim();
            this.itmSiteName.Value = this.txtSiteName.Text.Trim();
            this.itmPoolName.Value = this.txtPoolName.Text.Trim();
            var ip = this.cmbIpAddress.Text.Trim();
            itmIp.Value = ipRegex.IsMatch(ip) ? ip : itmIp.DefaultValue;
            itmPort.Value = int.TryParse(this.txtPort.Text.Trim(), out int i) ? i.ToString() : itmPort.DefaultValue;
            InvokeInstallChange();
        }


        private void InitPageItems()
        {
            itmSiteName = new PageItem()
            {
                Type = PageItemType.TextBox,
                Index = 0,
                IsRequired = true,
                Label = lblSiteName.Text,
                Key = "IIS_SiteName",
            };
            txtSiteName.Tag = itmSiteName;
            this.pageItems.Add(itmSiteName);

            itmPoolName = new PageItem()
            {
                Type = PageItemType.TextBox,
                Index = 1,
                IsRequired = true,
                Label = lblPoolName.Text,
                Key = "IIS_PoolName",
            };
            txtPoolName.Tag = itmPoolName;
            this.pageItems.Add(itmPoolName);

            itmLogicPath = new PageItem()
            {
                Type = PageItemType.TextBox,
                Index = 2,
                IsRequired = true,
                Label = lblLogicPath.Text,
                Key = "IIS_LogicPath",
                DefaultValue = "${InstPath}"
            };
            txtLogicPath.Tag = itmLogicPath;
            txtLogicPath.Text = itmLogicPath.DefaultValue;
            this.pageItems.Add(itmLogicPath);

            itmType = new PageItem()
            {
                Type = PageItemType.Other,
                Index = 3,
                IsRequired = true,
                Label = lblPoolName.Text,
                Key = "IIS_BindType",
                DefaultValue = "http"
            };
            cmbType.Tag = itmType;
            this.pageItems.Add(itmType);

            itmIp = new PageItem()
            {
                Type = PageItemType.TextBox,
                Index = 4,
                IsRequired = true,
                Label = lblIp.Text,
                Key = "IIS_IpAddress",
                DefaultValue = "*"
            };
            cmbIpAddress.Tag = itmIp;
            this.pageItems.Add(itmIp);

            itmPort = new PageItem()
            {
                Type = PageItemType.TextBox,
                Index = 5,
                IsRequired = true,
                Label = lblPort.Text,
                Key = "IIS_Port",
                DefaultValue = "80"
            };
            txtPort.Tag = itmPort;
            txtPort.Text = itmPort.DefaultValue;
            this.pageItems.Add(itmPort);

            itmHostName = new PageItem()
            {
                Type = PageItemType.TextBox,
                Index = 6,
                Label = lblHostName.Text,
                Key = "IIS_HostName",
            };
            txtHostName.Tag = itmHostName;
            this.pageItems.Add(itmHostName);
        }

        public string Title { get; } = "部署IIS应用程序";
        public string SubTitle { get; } = "部署IIS的应用程序池，以及网站配置";
        public List<PageItem> GetItems()
        {
            return pageItems;
        }

        public Form GelForm()
        {
            return this;
        }

        public event EventHandler<InstallEventArgs> InstallChanged;

        public string Privoder { get; } = "Lpz";
    }
}
