﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Serialization;

namespace CapybaraVS.Controls.BaseControls
{
    /// <summary>
    /// Movable.xaml の相互作用ロジック
    /// </summary>
    public partial class Movable : UserControl, IMovableControl, IAsset, IDisposable
    {
        #region ID管理
        private AssetIdProvider assetIdProvider = null;
        public int AssetId
        {
            get => assetIdProvider.AssetId;
            set { assetIdProvider.AssetId = value; }
        }
        #endregion

        #region XML定義
        [XmlRoot(nameof(Movable))]
        public class _AssetXML<OwnerClass>
            where OwnerClass : Movable
        {
            [XmlIgnore]
            public Action WriteAction = null;
            [XmlIgnore]
            public Action<OwnerClass> ReadAction = null;
            public _AssetXML()
            {
                ReadAction = (self) =>
                {
                    self.AssetId = AssetId;
                    self.XPos = XPos;
                    self.YPos = YPos;
                    if (MultiRootConnector != null)
                    {
                        var obj = new MultiRootConnector();
                        self.SetControl(obj);
                        obj.AssetXML = MultiRootConnector;
                        obj.AssetXML.ReadAction?.Invoke(obj);
                    }
                    if (SingleRootConnector != null)
                    {
                        var obj = new SingleRootConnector();
                        self.SetControl(obj);
                        obj.AssetXML = SingleRootConnector;
                        obj.AssetXML.ReadAction?.Invoke(obj);
                    }
                    if (MultiLinkConnector != null)
                    {
                        var obj = new MultiLinkConnector();
                        self.SetControl(obj);
                        obj.AssetXML = MultiLinkConnector;
                        obj.AssetXML.ReadAction?.Invoke(obj);
                    }
                    if (SingleLinkConnector != null)
                    {
                        var obj = new SingleLinkConnector();
                        self.SetControl(obj);
                        obj.AssetXML = SingleLinkConnector;
                        obj.AssetXML.ReadAction?.Invoke(obj);
                    }
                    if (RunableControl != null)
                    {
                        var obj = new RunableControl();
                        self.SetControl(obj);
                        obj.AssetXML = RunableControl;
                        obj.AssetXML.ReadAction?.Invoke(obj);
                    }

                    // 次回の為の初期化
                    self.AssetXML = new _AssetXML<Movable>(self);
                };
            }
            public _AssetXML(OwnerClass self)
            {
                WriteAction = () =>
                {
                    AssetId = self.AssetId;
                    XPos = self.XPos;
                    YPos = self.YPos;
                    if (self.myElement != null)
                    {
                        if (self.myElement is MultiRootConnector target)
                        {
                            target.AssetXML.WriteAction?.Invoke();
                            MultiRootConnector = target.AssetXML;
                        }
                        if (self.myElement is SingleRootConnector target2)
                        {
                            target2.AssetXML.WriteAction?.Invoke();
                            SingleRootConnector = target2.AssetXML;
                        }
                        if (self.myElement is MultiLinkConnector target3)
                        {
                            target3.AssetXML.WriteAction?.Invoke();
                            MultiLinkConnector = target3.AssetXML;
                        }
                        if (self.myElement is SingleLinkConnector target4)
                        {
                            target4.AssetXML.WriteAction?.Invoke();
                            SingleLinkConnector = target4.AssetXML;
                        }
                        if (self.myElement is RunableControl target5)
                        {
                            target5.AssetXML.WriteAction?.Invoke();
                            RunableControl = target5.AssetXML;
                        }
                    }
                };
            }
            [XmlAttribute("Id")]
            public int AssetId { get; set; } = 0;
            #region 固有定義
            [XmlAttribute("X")]
            public double XPos { get; set; } = 0;
            [XmlAttribute("Y")]
            public double YPos { get; set; } = 0;
            public MultiRootConnector._AssetXML<MultiRootConnector> MultiRootConnector { get; set; } = null;
            public SingleRootConnector._AssetXML<SingleRootConnector> SingleRootConnector { get; set; } = null;
            public MultiLinkConnector._AssetXML<MultiLinkConnector> MultiLinkConnector { get; set; } = null;
            public SingleLinkConnector._AssetXML<SingleLinkConnector> SingleLinkConnector { get; set; } = null;
            public RunableControl._AssetXML<RunableControl> RunableControl { get; set; } = null;
            #endregion
        }
        public _AssetXML<Movable> AssetXML { get; set; } = null;
        #endregion

        #region SelectedObject 添付プロパティ実装

        private static ImplementDependencyProperty<Movable, bool> impSelectedObject =
            new ImplementDependencyProperty<Movable, bool>(
                nameof(SelectedObject),
                (self, getValue) =>
                {
                    bool value = getValue(self);
                    self.waku.Visibility = value ? Visibility.Visible : Visibility.Collapsed;
                });

        public static readonly DependencyProperty SelectedObjectProperty = impSelectedObject.Regist(false);

        public bool SelectedObject
        {
            get { return impSelectedObject.GetValue(this); }
            set { impSelectedObject.SetValue(this, value); }
        }

        #endregion

        private UIElement myElement = null;

        public Movable()
        {
            InitializeComponent();
            assetIdProvider = new AssetIdProvider(this);
            AssetXML = new _AssetXML<Movable>(this);
        }

        public bool IsContains(Rect rect)
        {
            Rect myRect = new Rect(Canvas.GetLeft(this), Canvas.GetTop(this), ActualWidth, ActualHeight);
            return rect.Contains(myRect);
        }

        public double XPos
        {
            get => Canvas.GetLeft(this);
            set { Canvas.SetLeft(this, value); }
        }

        public double YPos
        {
            get => Canvas.GetTop(this);
            set { Canvas.SetTop(this, value); }
        }

        public void SetControl(UIElement element)
        {
            if (myElement is null)
            {
                MainGrid.Children.Add(myElement = element);
            }
        }

        #region IDisposable Support
        private bool disposedValue = false; // 重複する呼び出しを検出するには

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (myElement is IDisposable obj)
                        obj.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}