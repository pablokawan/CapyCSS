﻿using CapybaraVS.Controls;
using CapybaraVS.Controls.BaseControls;
using CapybaraVS.Script;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace CbVS.Script
{
    public interface ICbList : ICbValue
    {
        /// <summary>
        /// List<適切な型> に変換します。
        /// </summary>
        /// <returns>List<適切な型>のインスタンス</returns>
        object ConvertOriginalTypeList(MultiRootConnector col, DummyArgumentsStack cagt);

        /// <summary>
        /// List<適切な型> に変換します。
        /// </summary>
        /// <param name="dummyArgumentsControl"></param>
        /// <param name="cagt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        object ConvertOriginalTypeList(DummyArgumentsControl dummyArgumentsControl, DummyArgumentsStack cagt, MultiRootConnector col = null);

        ObservableCollection<LinkConnector> LinkConnectors { get; }

        List<ICbValue> Value { get; set; }

        string ItemName { get; }

        public Func<ICbValue> CreateTF { get; }

        ICbValue this[int index] { get; set; }

        void RemoveAt(int index);

        /// <summary>
        /// リストの要素数
        /// </summary>
        int Count { get; }

        /// <summary>
        /// リストのコピー
        /// </summary>
        /// <param name="toList"></param>
        void CopyTo(ICbList toList);

        /// <summary>
        /// リストに要素を追加します。
        /// </summary>
        /// <param name="cbVSValue"></param>
        void Append(ICbValue cbVSValue);

        /// <summary>
        /// リストの持つ変数の持つ値に cbVSValue が持つ値が含まれるかを返します。
        /// </summary>
        /// <param name="cbVSValue">調べる変数</param>
        /// <returns>true なら含まれる</returns>
        bool Contains(ICbValue cbVSValue);

        /// <summary>
        /// List<> のインスタンスから内容をコピーします。
        /// </summary>
        /// <param name="list"></param>
        void CopyFrom(object list);

        /// <summary>
        /// リストをクリアします。
        /// </summary>
        void Clear();
    }

    public class CbList
    {
        /// <summary>
        /// CbXXX型の List<> 型を作成します。
        /// </summary>
        /// <param name="original">オリジナルのList<T>のTの型</param>
        /// <returns>型</returns>
        public static Type GetCbType(Type original)
        {
            return typeof(CbList<>).MakeGenericType(original);
        }

        /// <summary>
        /// CbXXX型の List<> 変数を作成します。
        /// </summary>
        /// <param name="original">オリジナルのList<T>のTの型</param>
        /// <returns>CbList<original>型の変数</returns>
        public static ICbValue Create(Type original, string name = "")
        {
            object result = CbList.GetCbType(original).InvokeMember(
                        "GetCbFunc",
                        BindingFlags.InvokeMethod,
                        null,
                        null,
                        new object[] { name }
                        );
            return result as ICbValue;
        }

        /// <summary>
        /// CbXXX型の List<> 変数の型を作成します。
        /// </summary>
        /// <param name="original">オリジナルのList<T>のTの型</param>
        /// <returns>CbList<original>型の型</returns>
        public static Func<ICbValue> CreateTF(Type original)
        {
            return () => CbList.Create(original);
        }

        /// <summary>
        /// CbXXX型の List<> 変数の型を作成します。
        /// </summary>
        /// <param name="original">オリジナルのList<T>のTの型</param>
        /// <returns>CbList<original>型の型</returns>
        public static Func<string, ICbValue> CreateNTF(Type original)
        {
            return (name) => CbList.Create(original, name);
        }

        public static LinkConnector ConvertLinkConnector(ICbValue cbVSValue)
        {
            var linkConnector = new LinkConnector()
            {
                ValueData = cbVSValue
            };
            return linkConnector;
        }

        public static StackNode ConvertStackNode(ICbValue cbVSValue)
        {
            var stackNode = new StackNode()
            {
                ValueData = cbVSValue
            };
            return stackNode;
        }
    }

    /// <summary>
    /// リスト型
    /// </summary>
    public class CbList<T> : BaseCbValueClass<List<ICbValue>>, ICbValueListClass<List<ICbValue>>, ICbShowValue, ICbList
    {
        private bool nullFlg = true;

        public override Type MyType => typeof(CbList<T>);

        public override CbST CbType
        {
            get
            {
                return new CbST(
                    CapybaraVS.Script.CbType.Func,
                    OriginalType.FullName   // 型名を持っていないとスクリプト読み込み時に再現できない
                    );
            }
        }

        public CbList()
        {
            _ListNodeType = CbST.CbCreateTF(typeof(T));
        }

        public CbList(List<ICbValue> n, string name = "")
        {
            Value = n;
            _ListNodeType = CbST.CbCreateTF(typeof(T));
            Name = name;
        }

        public string DataString
        {
            get
            {
#if !SHOW_LINK_ARRAY
                string text = $"{CbSTUtils.CbTypeNameList[nameof(CbList)]} {Value.Count}-{ItemName}" + Environment.NewLine;
                foreach (var node in Value)
                {
                    text += node.ValueString + Environment.NewLine;
                }
#else
                string text = TypeName + Environment.NewLine;
#endif
                return text;
            }
        }

        public override Type OriginalReturnType => typeof(T);

        public override Type OriginalType => typeof(List<>).MakeGenericType(typeof(T));

        /// <summary>
        /// ノードの型名
        /// </summary>
        public string ItemName => CbSTUtils._GetTypeName(typeof(T));

        private Func<ICbValue> _ListNodeType = null;

        public override Func<ICbValue> NodeTF => _ListNodeType;

        public override string TypeName
        {
            get
            {
                if (NodeTF is null)
                {
                    return $"{CbSTUtils.CbTypeNameList[nameof(CbList)]}<>";
                }
                else
                {
                    return $"{CbSTUtils.CbTypeNameList[nameof(CbList)]}<{ItemName}>";
                }
            }
        }

        public override List<ICbValue> Value
        {
            get => _value;
            set
            {
                if (value is null)
                {
                    nullFlg = true;
                    value = null;
                }
                else
                {
                    nullFlg = false;
                }
                _value = value;
            }
        }

        public override string ValueString
        {
            get
            {
                string baseName = "[" + TypeName + "()]";
                if (IsError)
                    return ERROR_STR;
                if (nullFlg)
                    return baseName + NULL_STR;
                return baseName;
            }
            set => new NotImplementedException();
        }

        public override bool IsReadOnlyValue { get; set; } = true;

        /// <summary>
        /// リストの管理する要素数を参照します。
        /// </summary>
        public int Count => Value.Count;

        public ICbValue this[int index]
        {
            get => Value[index];
            set { Value[index] = value; }
        }

        /// <summary>
        /// リストから指定位置の要素を取り除きます。
        /// </summary>
        /// <param name="index">要素を取り除く位置</param>
        public void RemoveAt(int index)
        {
            Value.RemoveAt(index);
        }

        /// <summary>
        /// リストの内容をtoListに比較的高速にコピーします。
        /// </summary>
        /// <param name="toList">コピー先のリスト</param>
        public void CopyTo(ICbList toList)
        {
            if (toList.Count > 0 && Count > 0 && (toList as CbList<T>) != null)
            {
                // 差分のコピー

                int len = Math.Min(toList.Count, Value.Count);
                int i = 0;
                for (; i < len; ++i)
                {
                    toList[i].CopyValue(Value[i]);
                }
                int remaining = toList.Count - Value.Count;
                if (remaining != 0)
                {
                    if (remaining > 0)
                    {
                        // 多すぎる配列を消す

                        while (remaining-- > 0)
                        {
                            toList.RemoveAt(i);
                        }
                    }
                    else
                    {
                        // 足りない配列を足す

                        remaining = Math.Abs(remaining);
                        for (int j = 0; j < remaining; ++j)
                        {
                            var addNode = NodeTF();
                            addNode.CopyValue(Value[i + j]);
                            toList.Append(addNode);
                        }
                    }
                }
            }
            else
            {
                (toList as CbList<T>).Value = new List<ICbValue>(Value);
            }
        }

        /// <summary>
        /// リストに要素を追加します。
        /// </summary>
        /// <param name="cbVSValue">追加要素</param>
        public void Append(ICbValue cbVSValue)
        {
            var addData = NodeTF();
            addData.CopyValue(cbVSValue);
            Value.Add(addData);
        }

        public ObservableCollection<LinkConnector> LinkConnectors
        {
            get
            {
                ObservableCollection<LinkConnector> ret = new ObservableCollection<LinkConnector>();
                foreach (var node in Value)
                {
                    ret.Add(CbList.ConvertLinkConnector(node));
                }
                return ret;
            }
        }

        /// <summary>
        /// リストの持つ変数の持つ値に cbVSValue が持つ値が含まれるかを返します。
        /// </summary>
        /// <param name="cbVSValue">調べる変数</param>
        /// <returns>true なら含まれる</returns>
        public bool Contains(ICbValue cbVSValue)
        {
            foreach (var node in Value)
            {
                if (node.Equal(cbVSValue))
                    return true;
            }
            return false;
        }

        /// <summary>
        /// List<適切な型> に変換します。
        /// </summary>
        /// <returns>List<適切な型>のインスタンス</returns>
        public object ConvertOriginalTypeList(MultiRootConnector col, DummyArgumentsStack cagt)
        {
            return ConvertOriginalTypeList(null, cagt, col);
        }

        /// <summary>
        /// List<適切な型> に変換します。
        /// </summary>
        /// <param name="dummyArgumentsControl"></param>
        /// <param name="cagt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public object ConvertOriginalTypeList(DummyArgumentsControl dummyArgumentsControl, DummyArgumentsStack cagt, MultiRootConnector col = null)
        {
            var listNodeType = NodeTF();
            object instanct = null;
            if (listNodeType is ICbEvent)
            {
                // CbFunc のノードを持つ CbList を Func<> 及び Action<> 用の List に変換する 
                Debug.Assert(listNodeType.CbType.LiteralType == CapybaraVS.Script.CbType.Func);

                // 仮引数用の型を作成
                MethodInfo addMethod = null;

                // Func<> 及び Action<> 用の変換
                var genericType = typeof(List<>).MakeGenericType(typeof(T));
                instanct = Activator.CreateInstance(genericType);
                addMethod = genericType.GetMethod("Add");

                // 仮引数コントロールを作成
                dummyArgumentsControl ??= new DummyArgumentsControl(col);

                foreach (var node in Value)
                {
                    ICbEvent cbEvent2 = node as ICbEvent;
                    if (cbEvent2.CallBack is null)
                        addMethod.Invoke(instanct, new Object[] { null });
                    else
                        addMethod.Invoke(instanct, new Object[]
                        {
                            cbEvent2.GetCallBackOriginalType(dummyArgumentsControl, cagt)
                        });
                }
            }
            else
            {
                // CbList を List に変換する

                var genericType = typeof(List<>).MakeGenericType(typeof(T));
                instanct = Activator.CreateInstance(genericType);
                MethodInfo addMethod = genericType.GetMethod("Add");
                foreach (var node in Value)
                {
                    addMethod.Invoke(instanct, new Object[] { node.Data });
                }
            }

            return instanct;
        }

        /// <summary>
        /// List<> のインスタンスから内容をコピーします。
        /// </summary>
        /// <param name="list"></param>
        public void CopyFrom(object list)
        {
            Clear();
            var genericType = typeof(List<>).MakeGenericType(typeof(T));
            var instanct = Activator.CreateInstance(genericType);
            PropertyInfo countProp = genericType.GetProperty("Count");

            int count = (int)countProp.GetValue(list, null);
            for (int i = 0; i < count; i++)
            {
                PropertyInfo indexer = genericType.GetProperty("Item");
                ICbValue val = NodeTF();
                val.Data = indexer.GetValue(list, new Object[] { i });
                Append(val);
            }
        }

        /// <summary>
        /// リストを空にします。
        /// </summary>
        public void Clear()
        {
            Value?.Clear();
        }

        public static CbList<T> Create(string name = "")
        {
            return new CbList<T>(new List<ICbValue>(), name);
        }

        public static CbList<T> Create(List<ICbValue> n, string name = "")
        {
            return new CbList<T>(n, name);
        }

        public Func<ICbValue> CreateTF => () => Create();
        public Func<ICbValue> CreateNTF(string name) => () => Create(name);

        public static Func<ICbValue> TF => () => CbList<T>.Create();
        public static Func<string, ICbValue> NTF => (name) => CbList<T>.Create(name);

        public static ICbValue GetCbFunc(string name) => Create(name);    // リフレクションで参照されている。
    }
}