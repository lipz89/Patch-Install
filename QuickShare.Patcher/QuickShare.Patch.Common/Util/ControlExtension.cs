using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace QuickShare.Patch.Common.Util
{
    public static class ControlExtension
    {
        public static void ChangeDataGridViewSelectedLinkColor(this DataGridView dataGridView, Color color, Color? nonSelectedColor = null)
        {
            nonSelectedColor = nonSelectedColor ?? Color.Blue;
            dataGridView.SelectionChanged += DataGridViewOnSelectionChanged;

            void DataGridViewOnSelectionChanged(object sender, EventArgs eventArgs)
            {
                foreach (DataGridViewRow row in dataGridView.Rows)
                {
                    foreach (DataGridViewCell cell in row.Cells)
                    {
                        if (cell is DataGridViewLinkCell link)
                        {
                            link.LinkColor = cell.Selected ? color : nonSelectedColor.Value;
                        }
                    }
                }
            }
        }

        public static void TryChangeComboItems(this ComboBox box, string[] items)
        {
            var titems = box.Items.OfType<string>();
            if (!titems.OrderBy(x => x).SequenceEqual(items.OrderBy(x => x)))
            {
                var selected = box.Text;
                box.Enabled = false;
                box.Items.Clear();
                box.Items.AddRange(items);
                if (items.Contains(selected))
                {
                    box.Text = selected;
                    box.Enabled = true;
                }
                else
                {
                    box.Enabled = true;
                    box.SelectedIndex = 0;
                }
            }
        }

        public static void EnableSort(this ListView listView)
        {
            listView.ColumnClick += ListViewColumnClick;
        }

        private static readonly IDictionary<ListView, int> sortedColumns = new Dictionary<ListView, int>();

        private static readonly IDictionary<ListView, bool> sortedOrder = new Dictionary<ListView, bool>();
        //private const char ASC = (char)0x25bc;
        //private const char DES = (char)0x25b2;

        private const char ASC = (char)0xe1fe;
        private const char DES = (char)0xe1fc;

        private static void ListViewColumnClick(object sender, ColumnClickEventArgs e)
        {
            if (sender is ListView listview)
            {
                string asc = (ASC).ToString().PadLeft(2, ' ');
                string des = (DES).ToString().PadLeft(2, ' ');
                if (!sortedOrder.ContainsKey(listview)) sortedOrder.Add(listview, true);
                if (!sortedColumns.ContainsKey(listview)) sortedColumns.Add(listview, -1);
                var currentCol = sortedColumns[listview];
                if (sortedOrder[listview])
                {
                    sortedOrder[listview] = false;
                    string oldStr = listview.Columns[e.Column].Text.TrimEnd(ASC, DES, ' ');
                    listview.Columns[e.Column].Text = oldStr + des;
                }
                else
                {
                    sortedOrder[listview] = true;
                    string oldStr = listview.Columns[e.Column].Text.TrimEnd(ASC, DES, ' ');
                    listview.Columns[e.Column].Text = oldStr + asc;
                }

                listview.ListViewItemSorter = new ListViewItemComparer(e.Column, sortedOrder[listview]);
                listview.Sort();
                int rowCount = listview.Items.Count;
                if (currentCol != -1)
                {
                    for (int i = 0; i < rowCount; i++)
                    {
                        if (e.Column != currentCol)
                            listview.Columns[currentCol].Text =
                                listview.Columns[currentCol].Text.TrimEnd(ASC, DES, ' ');
                    }
                }

                sortedColumns[listview] = e.Column;
            }
        }

        private class ListViewItemComparer : IComparer
        {
            private readonly bool? sort;
            private readonly int column;

            public ListViewItemComparer(int column, bool? sort)
            {
                this.column = column;
                this.sort = sort;
            }

            public int Compare(object x, object y)
            {
                if (!sort.HasValue)
                {
                    return 0;
                }

                if (x is ListViewItem itemx && y is ListViewItem itemy)
                {
                    string valx = null, valy = null;
                    if (itemx.SubItems.Count > column)
                    {
                        valx = itemx.SubItems[column].Text;
                    }

                    if (itemy.SubItems.Count > column)
                    {
                        valy = itemy.SubItems[column].Text;
                    }

                    var rst = string.CompareOrdinal(valx, valy);
                    return sort.Value ? rst : -rst;
                }

                return 0;
            }
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr window, int message, int wParam, ref LvHittestInfo lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr window, int message, int wParam, IntPtr lParam);
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        static extern int SendMessage(IntPtr window, int message, int wParam, int lParam);

        private static int LVM_SETGROUPINFO = 0x00001093;// #define LVM_SETGROUPINFO (LVM_FIRST + 147) 
        //private static int LVM_GETGROUPINFO = 0x00001095;
        //private static int LVM_GETGROUPINFOBYINDEX = 0x00001099;
        private static int LVM_GETGROUPSTATE = 0x0000105C;

        private static int Collapsible = 8;
        private static int Collapsed = 1;
        //private static int Expanded = 0;

        private static PropertyInfo propListVireGroupID = typeof(ListViewGroup).GetProperty("ID", BindingFlags.NonPublic | BindingFlags.Instance);
        private static void SetGroupCollapsible(ListView listView, int state)
        {
            if (listView == null) return;
            foreach (ListViewGroup grp in listView.Groups)
            {
                object tmprtnval = propListVireGroupID?.GetValue(grp, null);
                if (tmprtnval is int id)
                {
                    LvGroup group = new LvGroup();
                    group.cbSize = Marshal.SizeOf(group);
                    group.state = (int)state; // LVGS_COLLAPSIBLE 
                    group.mask = 4; // LVGF_STATE 
                    group.iGroupId = id;
                    IntPtr ip = IntPtr.Zero;
                    try
                    {
                        ip = Marshal.AllocHGlobal(group.cbSize);
                        Marshal.StructureToPtr(group, ip, false);
                        SendMessage(listView.Handle, LVM_SETGROUPINFO, id, ip);
                    }
                    catch (Exception ex)
                    {
                        Trace.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                    }
                    finally
                    {
                        Marshal.FreeHGlobal(ip);
                    }
                }
            }
        }
        public static void EnableGroupEvent(this ListView listView)
        {
            Application.DoEvents();
            SetGroupCollapsible(listView, Collapsible);
            if (binded.ContainsKey(listView))
            {
                return;
            }
            listView.MouseDown += ListViewMouseDown;
            binded.Add(listView, true);
        }

        public static void AddGroupRightClick(this ListView listView, Action<ListViewGroup> action)
        {
            if (listView == null || action == null)
            {
                return;
            }
            if (!groupRightClick.ContainsKey(listView))
            {
                groupRightClick[listView] = new HashSet<Action<ListViewGroup>>();
            }

            groupRightClick[listView].Add(action);
        }

        private static readonly IDictionary<ListView, HashSet<Action<ListViewGroup>>> groupRightClick = new Dictionary<ListView, HashSet<Action<ListViewGroup>>>();
        private static readonly IDictionary<ListView, bool> binded = new Dictionary<ListView, bool>();
        private static void ListViewMouseDown(object sender, MouseEventArgs e)
        {
            if (sender is ListView listview)
            {
                LvHittestInfo lvHitInfo = new LvHittestInfo();
                Point p = new Point(e.X, e.Y);
                lvHitInfo.pt = p;
                try
                {
                    int id = SendMessage(listview.Handle, 0x1000 + 18, -1, ref lvHitInfo);
                    if (e.Button == MouseButtons.Right)
                    {
                        InvokeRightClick(listview, id);
                    }
                    else if (e.Button == MouseButtons.Left && lvHitInfo.flags == 0x50000000)
                    {
                        ToggleGroupCollapseState(listview.Handle, id);
                    }
                }
                catch (Exception ex)
                {
                    Trace.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
                }
            }
        }

        private static void InvokeRightClick(ListView listView, int id)
        {
            if (groupRightClick.ContainsKey(listView))
            {
                var actions = groupRightClick[listView];
                foreach (ListViewGroup grp in listView.Groups)
                {
                    object tmprtnval = propListVireGroupID?.GetValue(grp, null);
                    if (tmprtnval is int gid && gid == id)
                    {
                        foreach (var action in actions)
                        {
                            action(grp);
                        }
                    }
                }
            }
        }

        private static void ToggleGroupCollapseState(IntPtr handle, int id)
        {
            int i = id;
            LvGroup group = new LvGroup();
            group.cbSize = Marshal.SizeOf(group);
            group.mask = 4; // LVGF_STATE 
            group.iGroupId = i;
            IntPtr ip = IntPtr.Zero;
            try
            {
                ip = Marshal.AllocHGlobal(group.cbSize);
                Marshal.StructureToPtr(group, ip, false);
                var rst = SendMessage(handle, LVM_GETGROUPSTATE, i, Collapsed); // #define LVM_GETGROUPINFO (LVM_FIRST + 149) 

                if (rst == Collapsed)
                {
                    group.state = Collapsible;
                }
                else
                {
                    group.state = Collapsible + Collapsed;
                }
                Marshal.StructureToPtr(group, ip, false);
                SendMessage(handle, LVM_SETGROUPINFO, i, ip); // #define LVM_SETGROUPINFO (LVM_FIRST + 147) 
            }
            catch (Exception ex)
            {
                Trace.WriteLine(ex.Message + Environment.NewLine + ex.StackTrace);
            }
            finally
            {
                Marshal.FreeHGlobal(ip);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct LvGroup
        {
            public int cbSize;
            public int mask;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszHeader;
            public int cchHeader;
            [MarshalAs(UnmanagedType.LPTStr)]
            public string pszFooter;
            public int cchFooter;
            public int iGroupId;
            public int stateMask;
            public int state;
            public int uAlign;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct LvHittestInfo
        {
            public Point pt;
            public int flags;
            public int iItem;
            public int iSubItem;
            public int iGroup;
        }
    }

    /// <summary>
    /// 可分组的列 (该列必须是 Grid 的第一列)
    /// </summary>
    public class DataGridViewGroupColumn : DataGridViewTextBoxColumn
    {
        public DataGridViewGroupColumn()
        {
            CellTemplate = DataGridViewGroupCell.Template;
            ReadOnly = true;
        }

        public override DataGridViewCell CellTemplate
        {
            get
            {
                return base.CellTemplate;
            }
            set
            {
                base.CellTemplate = DataGridViewGroupCell.Template;
            }
        }
    }

    /// <summary>
    /// 可分组的单元格
    /// </summary>
    public class DataGridViewGroupCell : DataGridViewTextBoxCell
    {
        internal static DataGridViewGroupCell Template = new DataGridViewGroupCell();
        #region Variant

        /// <summary>
        /// 标示的宽度
        /// </summary>
        const int PLUS_WIDTH = 24;

        /// <summary>
        /// 标示的区域
        /// </summary>
        Rectangle groupPlusRect;

        #endregion

        #region Init

        public DataGridViewGroupCell()
        {
            GroupLevel = 1;
        }

        #endregion

        #region Property

        /// <summary>
        /// 组级别(以1开始)
        /// </summary>
        public int GroupLevel { get; set; }

        /// <summary>
        /// 父节点
        /// </summary>
        public DataGridViewGroupCell ParentCell { get; private set; }

        /// <summary>
        /// 是否收起
        /// </summary>
        public bool Collapsed { get; private set; }

        /// <summary>
        /// 所有的子结点
        /// </summary>
        public List<DataGridViewGroupCell> ChildCells { get; private set; }

        /// <summary>
        /// 是否重绘
        /// </summary>
        public bool BPaint { get; set; } = true;

        #endregion

        #region 添加子节点

        /// <summary>
        /// 添加子结点
        /// </summary>
        /// <param name="cell"></param>
        /// <returns></returns>
        public int AddChildCell(DataGridViewGroupCell cell)
        {
            return AddChildCellRange(new[] { cell });
        }

        public int AddChildCellRange(IEnumerable<DataGridViewGroupCell> cells)
        {
            bool needRedraw = false;
            if (ChildCells == null)
            {
                //需要画一个加号
                ChildCells = new List<DataGridViewGroupCell>();
                needRedraw = true;
            }

            foreach (DataGridViewGroupCell cell in cells)
            {
                if (!ChildCells.Contains(cell))
                {
                    ChildCells.Add(cell);
                    cell.GroupLevel = GroupLevel + 1;
                    cell.ParentCell = this;
                }
            }

            if (needRedraw)
                DataGridView.InvalidateCell(this);
            return ChildCells.Count;
        }

        #endregion

        #region 绘制节点

        protected override void Paint(Graphics graphics, Rectangle clipBounds, Rectangle cellBounds, int rowIndex, DataGridViewElementStates cellState, object value, object formattedValue, string errorText, DataGridViewCellStyle cellStyle, DataGridViewAdvancedBorderStyle advancedBorderStyle, DataGridViewPaintParts paintParts)
        {
            if (!BPaint)
            {
                base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);
                return;
            }
            Pen gridPen = new Pen(DataGridView.GridColor);
            int width = GroupLevel * PLUS_WIDTH;
            Rectangle rectLeft = new Rectangle(cellBounds.Left, cellBounds.Top - 1, width, cellBounds.Height);
            cellBounds.X += width;
            cellBounds.Width -= width;
            base.Paint(graphics, clipBounds, cellBounds, rowIndex, cellState, value, formattedValue, errorText, cellStyle, advancedBorderStyle, paintParts);

            PaintGroupPlus(graphics, gridPen, cellStyle, rectLeft, Collapsed);

            gridPen.Dispose();
        }

        private void PaintGroupPlus(Graphics graphics, Pen gridPen, DataGridViewCellStyle cellStyle, Rectangle rectLeft, bool collapsed)
        {
            Brush bruBk = new SolidBrush(cellStyle.BackColor);
            Pen penBk = new Pen(cellStyle.BackColor);
            Pen pen = new Pen(DataGridView.GridColor, 1);
            pen.DashStyle = DashStyle.Dot;
            graphics.FillRectangle(bruBk, rectLeft);
            int left = rectLeft.Left + (GroupLevel - 1) * PLUS_WIDTH;
            //画 Left, Top, Right 三根

            graphics.DrawLine(gridPen, rectLeft.Left, rectLeft.Top, rectLeft.Right, rectLeft.Top);
            //graphics.DrawLine(gridPen, left, rectLeft.Top, rectLeft.Right, rectLeft.Top);//上
            graphics.DrawLine(gridPen, rectLeft.Right, rectLeft.Bottom, rectLeft.Left, rectLeft.Bottom);

            //最左边的一条线

            //如果是该级别的最后一个节点，则需要画一个底线，以便将整个组封闭起来
            //bool drawBottomLine = false;
            for (int i = 1; i <= GroupLevel; i++)
            {
                if (!IsLastCell(i)) //上层不是最后一个节点
                {
                    if (i < GroupLevel)
                    {
                        graphics.DrawLine(pen, rectLeft.Left + (i - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Top,
                            rectLeft.Left + (i - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Bottom);
                    }
                    else if (ChildCells != null && ChildCells.Count > 0)
                    {
                        graphics.DrawLine(pen, rectLeft.Left + (i - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Top,
                            rectLeft.Left + (i - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Top + (rectLeft.Height - 12) / 2);
                        graphics.DrawLine(pen, rectLeft.Left + (i - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Top + (rectLeft.Height + 12) / 2,
                            rectLeft.Left + (i - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Bottom);
                    }
                }
            }
            //如果有子结点， 则需要画一个方框, 里面有+号或-号
            if (ChildCells != null && ChildCells.Count > 0)
            {
                groupPlusRect = new Rectangle((GroupLevel - 1) * PLUS_WIDTH + rectLeft.Left + (PLUS_WIDTH - 12) / 2, rectLeft.Top + (rectLeft.Height - 12) / 2, 12, 12);
                graphics.DrawRectangle(gridPen, groupPlusRect);

                graphics.DrawLine(Pens.Black, groupPlusRect.Left + 3, groupPlusRect.Top + groupPlusRect.Height / 2, groupPlusRect.Right - 3, groupPlusRect.Top + groupPlusRect.Height / 2);
                if (collapsed)
                {
                    graphics.DrawLine(Pens.Black, groupPlusRect.Left + groupPlusRect.Width / 2, groupPlusRect.Top + 3, groupPlusRect.Left + groupPlusRect.Width / 2, groupPlusRect.Bottom - 3);
                }
            }
            else
            {
                //横
                graphics.DrawLine(pen, rectLeft.Left + (GroupLevel - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Top + rectLeft.Height / 2, rectLeft.Left + (GroupLevel - 1) * PLUS_WIDTH + PLUS_WIDTH, rectLeft.Top + rectLeft.Height / 2);

                //竖
                if (!IsLastCell((GroupLevel)))
                {
                    graphics.DrawLine(pen, rectLeft.Left + (GroupLevel - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Top, rectLeft.Left + (GroupLevel - 1) * PLUS_WIDTH + PLUS_WIDTH / 2, rectLeft.Bottom);
                }
                else
                {
                    graphics.DrawLine(pen, (GroupLevel - 1) * PLUS_WIDTH + rectLeft.Left + (PLUS_WIDTH - 8) / 2 + 4,
                        rectLeft.Top + (rectLeft.Height - 8) / 2 - 6,
                        (GroupLevel - 1) * PLUS_WIDTH + rectLeft.Left + (PLUS_WIDTH - 8) / 2 + 4,
                        rectLeft.Top + (rectLeft.Height - 8) / 2 + 4);
                }
            }
            pen.Dispose();
            bruBk.Dispose();
            penBk.Dispose();
        }

        #endregion

        #region 判断

        /// <summary>
        /// 该节点是否为某一级节点的最后一个子结点
        /// </summary>
        /// <param name="level">节点层级</param>
        /// <returns></returns>
        private bool IsLastCell(int level)
        {
            if (RowIndex == DataGridView.Rows.Count - 1)
                return true;

            var cell = this;
            while (level < cell.GroupLevel)
            {
                cell = cell.ParentCell;
            }

            for (int i = RowIndex + 1; i < DataGridView.Rows.Count; i++)
            {
                DataGridViewGroupCell cel = DataGridView.Rows[i].Cells[0] as DataGridViewGroupCell;
                if (cel.GroupLevel == level && cel.ParentCell == cell.ParentCell)
                    return false;
            }
            return true;
        }

        #endregion

        #region 点击 Cell

        protected override void OnMouseDown(DataGridViewCellMouseEventArgs e)
        {
            Rectangle rect = DataGridView.GetCellDisplayRectangle(ColumnIndex, RowIndex, false);
            Point pt = new Point(rect.Left + e.Location.X, rect.Top + e.Location.Y);
            if (groupPlusRect.Contains(pt))
            {
                if (Collapsed)
                {
                    Expand();
                }
                else
                {
                    Collapse();
                }
            }
            base.OnMouseDown(e);
        }

        #endregion

        #region 展开/收起节点

        /// <summary>
        /// 展开节点
        /// </summary>
        public void Expand()
        {
            if (ParentCell != null)
            {
                ParentCell.CollapseAll();
            }
            Collapsed = false;
            ShowChild(true);
            base.DataGridView.InvalidateCell(this);
        }

        private void ShowChild(bool show)
        {
            if (ChildCells == null)
                return;
            //CurrencyManager cm = (CurrencyManager)DataGridView.BindingContext[DataGridView.DataSource];
            //cm.SuspendBinding(); //挂起数据绑定
            foreach (DataGridViewGroupCell cel in ChildCells)
            {
                if (cel.RowIndex == -1)
                {
                    continue;
                }
                DataGridView.Rows[cel.RowIndex].Visible = show;
                if (!cel.Collapsed)
                    cel.ShowChild(show);
            }
            //cm.ResumeBinding(); //继续数据绑定
        }

        /// <summary>
        /// 收起节点
        /// </summary>
        public void Collapse()
        {
            Collapsed = true;
            ShowChild(false);
            base.DataGridView.InvalidateCell(this);
        }

        /// <summary>
        /// 展开节点及子结点
        /// </summary>
        public void ExpandAll()
        {
            if (ChildCells == null)
                return;
            foreach (DataGridViewGroupCell cel in ChildCells)
            {
                cel.ExpandAll();
            }
        }

        public void CollapseAll()
        {
            if (ChildCells == null)
                return;
            foreach (DataGridViewGroupCell cel in ChildCells)
            {
                cel.CollapseAll();
            }
        }

        #endregion
    }

}
