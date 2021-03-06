﻿using CapybaraVS.Controls;
using CbVS.Script;
using System;
using System.Collections.Generic;
using System.Text;
using static CapybaraVS.Controls.MultiRootConnector;

namespace CapybaraVS.Script
{
    /// <summary>
    /// アセット実装登録インターフェイス
    /// </summary>
    public interface IFuncAssetDef
    {
        /// <summary>
        /// アセット型識別コード
        /// </summary>
        string AssetCode { get; }

        /// <summary>
        /// 説明
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// アセット処理を実装する
        /// </summary>
        /// <param name="col"></param>
        bool ImplAsset(MultiRootConnector col, bool notheradMode = false);
    }

    /// <summary>
    /// 最もシンプルなファンクションアセット
    /// </summary>
    public interface IFuncCreateAssetDef : IFuncAssetDef
    {
        /// <summary>
        /// アセットツリー上の名前
        /// </summary>
        string MenuTitle { get; }
    }

    /// <summary>
    /// 変数対応型ファンクションアセット
    /// </summary>
    public interface IFuncCreateVariableAssetDef : IFuncAssetDef
    {
        /// <summary>
        /// アセットツリー上の名前
        /// </summary>
        string MenuTitle { get; }

        /// <summary>
        /// 型名
        /// </summary>
        string ValueType { get; }

        /// <summary>
        /// 型選択リストの受け入れ項目
        /// ※必要がないなら null にする
        /// </summary>
        Func<Type, bool> IsAccept { get; }
    }

    /// <summary>
    /// 配列変数参照型ファンクションアセット
    /// ※対象変数選択リストに配列変数のみピックアップされる
    /// </summary>
    public interface IFuncCreateVariableListAssetDef : IFuncAssetDef
    {
        /// <summary>
        /// アセットツリー上の名前
        /// </summary>
        string MenuTitle { get; }

        /// <summary>
        /// 型名
        /// </summary>
        string ValueType { get; }

        /// <summary>
        /// 型選択リストの受け入れ項目
        /// ※必要がないなら null にする
        /// </summary>
        Func<Type, bool> IsAccept { get; }
    }

    /// <summary>
    /// 引数対応型ファンクションアセット
    /// </summary>
    public interface IFuncAssetWithArgumentDef : IFuncAssetDef
    {
        /// <summary>
        /// アセットツリー上の名前
        /// </summary>
        string MenuTitle { get; }

        /// <summary>
        /// 型名
        /// </summary>
        string ValueType { get; }

        /// <summary>
        /// 型選択リストの受け入れ項目
        /// ※必要がないなら null にする
        /// </summary>
        Func<Type, bool> IsAccept { get; }
    }

    /// <summary>
    /// リテラル定義側ファンクションアセット
    /// </summary>
    public interface IFuncAssetLiteralDef
    {
        /// <summary>
        /// 説明
        /// </summary>
        string HelpText { get; }

        /// <summary>
        /// アセットツリー上の名前
        /// </summary>
        string MenuTitle { get; }

        /// <summary>
        /// 型名
        /// </summary>
        string ValueType { get; }

        /// <summary>
        /// 型選択リストの受け入れ項目
        /// ※必要がないなら null にする
        /// </summary>
        Func<Type, bool> IsAccept { get; }
    }

    public class FuncAssetSub
    {
        /// <summary>
        /// 乱数発生器
        /// </summary>
        public static Random random = new System.Random();

        protected void TryArgListProc(ICbValue variable, Action<ICbValue> func)
        {
            if (func is null)
                return;

            var argList = (variable as ICbList).Value;
            foreach (var node in argList)
            {
                if (node.IsError)
                    throw new Exception(node.ErrorMessage);

                func?.Invoke(node);
            }
        }

        protected void TryArgListCancelableProc(ICbValue variable, Func<ICbValue, bool> func)
        {
            if (func is null)
                return;

            var argList = (variable as ICbList).Value;
            foreach (var node in argList)
            {
                if (node.IsError)
                    throw new Exception(node.ErrorMessage);

                if (func.Invoke(node))
                    break;
            }
        }

        /// <summary>
        /// コールバックを呼び出します。
        /// </summary>
        /// <param name="variable">変数</param>
        /// <param name="dummyArgumentstack">仮引数スタック</param>
        /// <returns>コールバックの返し値</returns>
        protected ICbValue CallEvent(ICbValue variable, DummyArgumentsStack dummyArgumentstack)
        {
            ICbEvent cbEvent = variable as ICbEvent;
            cbEvent.CallCallBack(dummyArgumentstack);
            return cbEvent.Value;
        }

        /// <summary>
        /// コールバックかを判断します。
        /// </summary>
        /// <param name="variable">変数</param>
        /// <returns>true = コールバック</returns>
        protected bool CanCallBack(ICbValue variable)
        {
            ICbEvent cbEvent = variable as ICbEvent;
            if (cbEvent is null)
                return false;
            return cbEvent.IsCallBack;
        }

        /// <summary>
        /// 引数が異常かをチェックし内容を参照します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="variable">引数</param>
        /// <returns>値</returns>
        protected T GetArgument<T>(ICbValue variable)
        {
            CheckArgument(variable);
            return (T)variable.Data;
        }

        /// <summary>
        /// 変数が異常かをチェックします。
        /// </summary>
        /// <param name="variable">変数</param>
        protected void CheckArgument(ICbValue variable)
        {
            if (variable is null)
                throw new Exception("Argument is null.");
            if (variable.IsError)
                throw new Exception(variable.ErrorMessage);
        }

        /// <summary>
        /// 引数を参照します。
        /// </summary>
        /// <typeparam name="T">引数の型</typeparam>
        /// <param name="arguments">引数リスト</param>
        /// <param name="index">引数のインデックス</param>
        /// <returns>値</returns>
        protected T GetArgument<T>(List<ICbValue> arguments, int index)
        {
            return GetArgument<T>(arguments[index]);
        }

        /// <summary>
        /// 引数からリストを参照します。
        /// </summary>
        /// <param name="arguments">引数リスト</param>
        /// <param name="index">引数のインデックス</param>
        /// <returns>配列</returns>
        protected List<ICbValue> GetArgumentList(List<ICbValue> arguments, int index)
        {
            ICbValue valueData = arguments[index];
            CheckArgument(valueData);
            return (valueData as ICbList).Value;
        }

        /// <summary>
        /// コールバックを呼びます。
        /// </summary>
        /// <param name="cagt">仮引数スタック</param>
        /// <param name="func">コールバック変数</param>
        protected void CallCallBack(DummyArgumentsStack cagt, ICbValue func)
        {
            CallEvent(func, cagt);
        }

        /// <summary>
        /// 可能ならコールバックを呼びます。
        /// </summary>
        /// <param name="cagt">仮引数スタック</param>
        /// <param name="func">コールバック変数</param>
        protected void TryCallCallBack(DummyArgumentsStack cagt, ICbValue func)
        {
            if (CanCallBack(func))
            {
                CallCallBack(cagt, func);
            }
        }

        /// <summary>
        /// 引数付きでコールバックを呼びます。
        /// </summary>
        /// <param name="dummyArgumentsControl">仮引数コントロール</param>
        /// <param name="cagt">仮引数スタック</param>
        /// <param name="func">コールバック変数</param>
        /// <param name="arg">コールバック引数</param>
        protected void CallCallBack(DummyArgumentsControl dummyArgumentsControl, DummyArgumentsStack cagt, ICbValue func, object arg)
        {
            if (arg is ICbValue cbValue)
            {
                dummyArgumentsControl.EnableCbValue(cagt, cbValue);    // 仮引数に引数を登録
            }
            else
            {
                dummyArgumentsControl.Enable(cagt, (dynamic)arg);    // 仮引数に引数を登録
            }
            CallEvent(func, cagt);
            dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
        }

        /// <summary>
        /// 可能なら引数付きでコールバックを呼びます。
        /// </summary>
        /// <param name="dummyArgumentsControl">仮引数コントロール</param>
        /// <param name="cagt">仮引数スタック</param>
        /// <param name="func">コールバック変数</param>
        /// <param name="arg">コールバック引数</param>
        protected void TryCallCallBack(DummyArgumentsControl dummyArgumentsControl, DummyArgumentsStack cagt, ICbValue func, object arg)
        {
            if (CanCallBack(func))
            {
                CallCallBack(dummyArgumentsControl, cagt, func, arg);
            }
        }

        /// <summary>
        /// コールバックを呼んで返り値を得ます。
        /// </summary>
        /// <typeparam name="R">返り値の型</typeparam>
        /// <param name="dummyArgumentsControl">仮引数コントロール</param>
        /// <param name="cagt">仮引数スタック</param>
        /// <param name="func">コールバック変数</param>
        /// <param name="def">コールバックの返り値を得られなかったときの返り値</param>
        /// <returns>コールバックの返り値</returns>
        protected R GetCallBackResult<R>(DummyArgumentsStack cagt, ICbValue func, R def)
        {
            if (!CanCallBack(func))
            {
                return def;
            }
            if (typeof(R) == typeof(ICbValue))
            {
                ICbValue result = CallEvent(func, cagt);
                return (R)result;
            }
            else
            {
                R result = (R)CallEvent(func, cagt).Data;
                return result;
            }
        }

        /// <summary>
        /// 引数付きでコールバックを呼んで返り値を得ます。
        /// </summary>
        /// <typeparam name="R">返り値の型</typeparam>
        /// <param name="dummyArgumentsControl">仮引数コントロール</param>
        /// <param name="cagt">仮引数スタック</param>
        /// <param name="func">コールバック変数</param>
        /// <param name="arg">コールバック引数</param>
        /// <param name="def">コールバックの返り値を得られなかったときの返り値</param>
        /// <returns>コールバックの返り値</returns>
        protected R GetCallBackResult<R>(DummyArgumentsControl dummyArgumentsControl, DummyArgumentsStack cagt, ICbValue func, object arg, R def)
        {
            if (!CanCallBack(func))
            {
                return def;
            }
            if (arg is ICbValue cbValue)
            {
                dummyArgumentsControl.EnableCbValue(cagt, cbValue);    // 仮引数に引数を登録
            }
            else
            {
                dummyArgumentsControl.Enable(cagt, (dynamic)arg);    // 仮引数に引数を登録
            }
            if (typeof(R) == typeof(ICbValue))
            {
                ICbValue result = CallEvent(func, cagt);
                dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                return (R)result;
            }
            else
            {
                R result = (R)CallEvent(func, cagt).Data;
                dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                return result;
            }
        }
    }
}
