using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using QuickShare.Patch.Common.Models;

namespace QuickShare.Patch.Common.Util
{
    public static class PageHelper
    {
        public static List<IPagePlugin> GetPluginPages()
        {
            return IocContainer.Get<IPagePlugin>();
        }
        public static PageInfo GetPageInfo(IPagePlugin pagePlugin)
        {
            return new PageInfo()
            {
                IsPlugin = true,
                CustomPageTypeName = pagePlugin.GetType().AssemblyQualifiedName,
                Items = pagePlugin.GetItems(),
                SubTitle = pagePlugin.SubTitle,
                Title = pagePlugin.Title,
                Privoder = pagePlugin.Privoder,
                Editable = false
            };
        }
        public static Page CreatePage(IWin32Window owner, PageInfo info)
        {
            if (info.IsPlugin)
            {
                bool retry = false;
                do
                {
                    var type = Type.GetType(info.CustomPageTypeName);
                    if (type != null)
                    {
                        var page = new Page() { PageInfo = info };
                        var p = (IPagePlugin)Activator.CreateInstance(type);
                        info.Items = p.GetItems();
                        var form = p.GelForm();
                        form.TopLevel = false;
                        form.Dock = DockStyle.Fill;
                        form.FormBorderStyle = FormBorderStyle.None;
                        page.Form = form;
                        form.Tag = page;
                        p.InstallChanged += (sender, args) => page.OnChanged(sender, args);
                        return page;
                    }

                    var message = $"找不到页面类型 {info.CustomPageTypeName}，请检查是否缺少插件，{Environment.NewLine}单击忽略会尝试自行创建页面。";

                    var da = Helper.AbortRetryIgnore(owner, message);
                    if (da == DialogResult.Ignore)
                    {
                        return CreatePageInner(info);
                    }

                    if (da == DialogResult.Retry)
                    {
                        retry = true;
                    }
                    else
                    {
                        Application.Exit();
                    }

                } while (retry);
            }

            return CreatePageInner(info);
        }

        private static Page CreatePageInner(PageInfo info)
        {
            var pnl = new Panel()
            {
                Dock = DockStyle.Fill,
                Size = new Size(100, 100),
                AutoScroll = true
            };
            var form = new Form()
            {
                Dock = DockStyle.Fill,
                TopLevel = false,
                FormBorderStyle = FormBorderStyle.None
            };
            form.Controls.Add(form);
            var page = new Page() { Form = form, PageInfo = info };
            var offsetY = 4;
            Control ctn = pnl;
            if (!string.IsNullOrEmpty(info.EnableText))
            {
                var grp = new GroupBox()
                {
                    Location = new Point(12, 8),
                    Size = new Size(76, 76),
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
                };
                pnl.Controls.Add(grp);
                var chk = new CheckBox()
                {
                    Text = info.EnableText,
                    Height = 19,
                    AutoSize = true,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Location = new Point(25, 13),
                    Checked = true
                };
                chk.CheckedChanged += (sender, args) =>
                {
                    grp.Enabled = chk.Checked;
                    page.Init(chk.Checked);
                };
                pnl.Controls.Add(chk);
                ctn = grp;
                chk.BringToFront();
                offsetY = 16;
            }

            AddItems(ctn, page, info, ref offsetY);
            offsetY += 12;
            ctn.Height = offsetY;
            pnl.Tag = page;
            form.Tag = page;
            return page;
        }

        private static void AddItems(Control control, Page page, PageInfo info, ref int offsetY)
        {
            foreach (var item in info.Items.OrderBy(x => x.Index))
            {
                if (item.Type == PageItemType.TextBox)
                {
                    AddTextBox(control, page, item, ref offsetY);
                }
                else if (item.Type == PageItemType.CheckBox)
                {
                    AddCheckBox(control, item, ref offsetY);
                }
            }
        }

        private static void AddTextBox(Control control, Page page, PageItem item, ref int offsetY)
        {
            offsetY += 8;
            var left = 16;
            if (item.IsRequired)
            {
                var lblRequied = new Label()
                {
                    Text = "*",
                    Location = new Point(left, offsetY),
                    ForeColor = Color.Red,
                    Size = new Size(15, 15),
                    TextAlign = ContentAlignment.MiddleLeft
                };
                left += lblRequied.Width;
                control.Controls.Add(lblRequied);
            }
            var lbl = new Label()
            {
                Text = item.Label,
                Location = new Point(left, offsetY),
                Height = 15,
                AutoSize = true,
                TextAlign = ContentAlignment.MiddleLeft
            };
            offsetY += lbl.Height;
            control.Controls.Add(lbl);
            offsetY += 4;
            var txt = new TextBox()
            {
                Text = item.DefaultValue,
                Location = new Point(16, offsetY),
                Width = control.Width - 32,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                TextAlign = HorizontalAlignment.Left,
            };
            txt.TextChanged += (sender, args) =>
            {
                item.Value = txt.Text.Trim();
                page.Init();
            };
            offsetY += txt.Height;
            control.Controls.Add(txt);
        }

        private static void AddCheckBox(Control control, PageItem item, ref int offsetY)
        {
            offsetY += 12;
            var chk = new CheckBox()
            {
                Text = item.Label,
                Height = 19,
                AutoSize = true,
                Location = new Point(16, offsetY),
                TextAlign = ContentAlignment.MiddleLeft,
                Checked = "True".Equals(item.DefaultValue, StringComparison.OrdinalIgnoreCase)
            };
            if (!string.IsNullOrEmpty(item.DefaultValue))
            {
                chk.Checked = string.Equals(item.DefaultValue, "True", StringComparison.OrdinalIgnoreCase);
            }

            chk.CheckedChanged += (sender, args) => item.Value = chk.Checked.ToString();
            offsetY += chk.Height;
            control.Controls.Add(chk);
        }
    }
}