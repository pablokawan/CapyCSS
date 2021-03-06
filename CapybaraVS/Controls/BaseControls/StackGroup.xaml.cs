﻿using CapybaraVS.Script;
using CapyCSS.Controls;
using CbVS;
using CbVS.Script;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace CapybaraVS.Controls.BaseControls
{
    /// <summary>
    /// StackGroup.xaml の相互作用ロジック
    /// </summary>
    public partial class StackGroup
        : UserControl
        , IHaveCommandCanvas
    {
        public ObservableCollection<StackGroup> ListData { get; set; } = new ObservableCollection<StackGroup>();

        #region EnableAdd 添付プロパティ実装

        private static ImplementDependencyProperty<StackGroup, bool> impEnableAdd =
            new ImplementDependencyProperty<StackGroup, bool>(
                nameof(EnableAdd),
                (self, getValue) =>
                {
                    bool value = getValue(self);
                    self.AddOption.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                });

        public static readonly DependencyProperty EnableAddOptionProperty = impEnableAdd.Regist(false);

        public bool EnableAdd
        {
            get { return impEnableAdd.GetValue(this); }
            set { impEnableAdd.SetValue(this, value); }
        }

        #endregion

        #region UpdateListEvent 添付プロパティ実装

        private static ImplementDependencyProperty<StackGroup, Action> impUpdateListEvent =
            new ImplementDependencyProperty<StackGroup, Action>(
                nameof(UpdateListEvent),
                (self, getValue) =>
                {
                    //Action value = getValue(self);
                });

        public static readonly DependencyProperty UpdateListEventProperty = impUpdateListEvent.Regist(null);

        public Action UpdateListEvent
        {
            get { return impUpdateListEvent.GetValue(this); }
            set { impUpdateListEvent.SetValue(this, value); }
        }

        #endregion

        private Func<StackNode> addEvent = null;
        public Func<StackNode> AddEvent
        {
            get => addEvent;
            set
            {
                addEvent = value;
                if (addEvent != null)
                {
                    EnableAdd = true;
                }
            }
        }

        public Func<bool> IsEnableDelete = null;

        public Action DeleteEvent = null;

        private StackNode CbValue = null;
        private StackNode CbListValue = null;

        private CommandCanvas _OwnerCommandCanvas = null;

        public CommandCanvas OwnerCommandCanvas
        {
            get => _OwnerCommandCanvas;
            set
            {
                Debug.Assert(value != null);
                SetOunerCanvas(ListData, value);
                if (_OwnerCommandCanvas is null)
                    _OwnerCommandCanvas = value;
            }
        }

        private void SetOunerCanvas(IEnumerable<StackGroup> list, CommandCanvas value)
        {
            if (list is null)
                return;

            foreach (var node in list)
            {
                if (node.OwnerCommandCanvas is null)
                    node.OwnerCommandCanvas = value;
            }
        }

        public StackNode stackNode
        {
            get
            {
                if (CbListValue != null)
                    return CbListValue;
                return CbValue;
            }
        }

        public StackGroup()
        {
            InitializeComponent();
            HideAccordion();
            ListView.ItemsSource = ListData;
            AddOption.Visibility = Visibility.Collapsed;
        }

        private HoldActionManager<StackGroup> HoldAction = new HoldActionManager<StackGroup>();

        private void Accordion_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (Accordion1.Visibility == Visibility.Visible)
            {
                OpenAccordion();
            }
            else
            {
                CloseAccordion();
            }
        }

        public void UpdateValueData()
        {
            if (OwnerCommandCanvas.StackGroupHoldAction.Enabled)
            {
                // 画面反映はあとから一括で行う

                OwnerCommandCanvas.StackGroupHoldAction.Add(this, () =>
                    {
                        if (EnableAdd)
                            CloseAccordion();   // スクリプト実行後にアコーディオンを閉じる

                        _UpdateValueData();
                    }
                    );
                return;
            }
            _UpdateValueData();
        }

        private void _UpdateValueData()
        {
            if (HoldAction.Enabled)
            {
                // アコーディオンが閉じているので開いたときに更新する。


                HoldAction.Add(this, () => _UpdateValueData());
                return;
            }

            stackNode.UpdateValueData();
            if (CbListValue != null && CbListValue.ValueData is ICbList target)
            {
                if (ListData.Count > 0 && target.Count > 0 &&
                    ListData[0].stackNode.ValueData.OriginalType == target[0].OriginalType)
                {
                    // 差分のコピー
                    int len = Math.Min(ListData.Count, target.Count);
                    int i = 0;
                    for (; i < len; ++i)
                    {
                        ListData[i].stackNode.ValueData = target[i];
                        ListData[i].stackNode.UpdateValueData();
                    }
                    int remaining = ListData.Count - target.Count;
                    if (remaining != 0)
                    {
                        if (remaining > 0)
                        {
                            // 多すぎる配列を消す

                            while (remaining-- > 0)
                            {
                                ListData.RemoveAt(i);
                            }
                        }
                        else
                        {
                            // 足りない配列を足す

                            remaining = Math.Abs(remaining);
                            for (int j = 0; j < remaining; ++j)
                            {
                                AddListNode(CbList.ConvertStackNode(OwnerCommandCanvas, target[i + j]));
                            }
                        }
                    }
                }
                else
                {
                    ListData.Clear();
                    foreach (var node in target.Value)
                    {
                        AddListNode(CbList.ConvertStackNode(OwnerCommandCanvas, node));
                    }
                }
            }
        }

        private bool IsCancelHoldAction = false;

        public StackNode AddListNode(StackNode node)
        {
            if (node is null)
                return null;

            if (node.ValueData is ICbList cbList)
            {
                Debug.Assert(CbListValue is null);
                CbListValue = node;
                AddEvent = () =>
                {
                    var listNode = cbList.NodeTF();
                    cbList.Value.Add(listNode);
                    var node = new StackNode(OwnerCommandCanvas, listNode);
                    node.OwnerCommandCanvas = OwnerCommandCanvas;
                    return node;
                };
                InnerList.Margin = new Thickness(12, 0, 0, 0);

                ListPanel.Children.Insert(0, node);
                ListPanel.Children[0].Visibility = Visibility.Visible;
            }
            else
            {
                var grp = new StackGroup();
                grp.OwnerCommandCanvas = OwnerCommandCanvas;
                grp.ListPanel.Children.Insert(0, node);
                grp.ListPanel.Children[0].Visibility = Visibility.Visible;

                grp.MainPanelFrame.Visibility = Visibility.Collapsed;
                grp.MainPanel.Margin = new Thickness();
                grp.InnerList.Visibility = Visibility.Collapsed;

                if (CbListValue != null && CbListValue.ValueData is ICbList target)
                {
                    grp.IsEnableDelete = () => true;
                    grp.DeleteEvent = () =>
                    {
                        foreach (var lc in target.Value)
                        {
                            target.Value.Remove(node.ValueData);
                            break;
                        }
                        DeleteNode(node);
                    };
                }
                else
                {
                    CbValue = node;
                }

                grp.CbValue = node;
                if (ListData.Count == 0 || IsCancelHoldAction)
                {
                    ListData.Add(grp);
                }
                else
                {
                    // リスト構造の場合、アコーディオンを閉じて処理を保留する

                    CloseAccordion();
                    if (HoldAction.Enabled)
                    {
                        HoldAction.Add(() => ListData.Add(grp));
                    }
                }
            }
            if (ListData.Count > 1)
                ConnectorBackground.Visibility = Visibility.Visible;
            return node;
        }

        public void DeleteNode(StackNode node)
        {
            foreach (var nd in ListData)
            {
                if (nd is StackGroup target)
                {
                    if (target.stackNode == node)
                    {
                        ListData.Remove(nd);
                        break;
                    }
                }
            }

            if (ListData.Count == 0)
            {
                Accordion1.Visibility = Visibility.Collapsed;
                Accordion2.Visibility = Visibility.Collapsed;
            }

            if (ListData.Count <= 1)
                ConnectorBackground.Visibility = Visibility.Collapsed;
        }

        private void OpenAccordion()
        {
            CommandCanvasList.OwnerWindow.Cursor = Cursors.Wait;
            Dispatcher.BeginInvoke(new Action(() =>
                {
                    CommandCanvasList.OwnerWindow.Cursor = null;
                }),
                DispatcherPriority.ApplicationIdle
            );
            HoldAction.Enabled = false;
            Accordion1.Visibility = Visibility.Collapsed;
            Accordion2.Visibility = Visibility.Visible;
            if (ListData.Count > 1)
                ConnectorBackground.Visibility = Visibility.Visible;
            ListView.Visibility = Visibility.Visible;
            AddOption.Visibility = Visibility.Visible;
        }

        private void CloseAccordion()
        {
            HoldAction.Enabled = true;
            Accordion1.Visibility = Visibility.Visible;
            Accordion2.Visibility = Visibility.Collapsed;
            ConnectorBackground.Visibility = Visibility.Collapsed;
            ListView.Visibility = Visibility.Collapsed;
            AddOption.Visibility = Visibility.Collapsed;
        }

        private void HideAccordion()
        {
            Accordion1.Visibility = Visibility.Collapsed;
            Accordion2.Visibility = Visibility.Collapsed;
            ListView.Visibility = Visibility.Visible;
            ListPanel.Children[0].Visibility = Visibility.Collapsed;
        }

        private void AddOption_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AddEvent is null)
                return;
            IsCancelHoldAction = true;  // アコーディオンが閉じないようにする
            AddListNode(AddEvent());
            IsCancelHoldAction = false;
        }

        private void Delete_MouseDown(object sender, MouseButtonEventArgs e)
        {
            DeleteEvent?.Invoke();
        }

        private void StackPanel_MouseEnter(object sender, MouseEventArgs e)
        {
            if (IsEnableDelete is null)
                return;
            if (IsEnableDelete())
                Delete.Visibility = Visibility.Visible;
        }

        private void StackPanel_MouseLeave(object sender, MouseEventArgs e)
        {
            Delete.Visibility = Visibility.Collapsed;
        }

        private void ListView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                while (ListView.SelectedItems.Count != 0)
                {
                    if (ListView.SelectedItems[0] is StackGroup stackGroup)
                        stackGroup.DeleteEvent?.Invoke();
                }
            }
            else if (e.Key == Key.Insert)
            {
                AddListNode(AddEvent());
            }
        }
    }
}
