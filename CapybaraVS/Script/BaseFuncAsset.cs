﻿using CapybaraVS.Controls;
using CapybaraVS.Controls.BaseControls;
using CbVS.Script;
using System;
using System.Collections.Generic;
using System.Text;
using static CapybaraVS.Controls.MultiRootConnector;

namespace CapybaraVS.Script
{
    /// <summary>
    /// 基本的なファンクションアセットをアセットリストに追加する
    /// </summary>
    class ImplementBaseAsset : ImplementAsset
    {
        public ImplementBaseAsset()
        {
            var assetNode = CreateGroup("Asset");

            CreateAssetMenu(assetNode, new Subroutine());

            {
                var literalNode = CreateGroup(assetNode, "Literal");
                CreateAssetMenu(literalNode, new LiteralType());
                CreateAssetMenu(literalNode, new LiteralListType());
            }

            {
                var variableNode = CreateGroup(assetNode, "Variable");
                CreateAssetMenu(variableNode, new CreateVariable());
                CreateAssetMenu(variableNode, new CreateVariableFunc());
                CreateAssetMenu(variableNode, new GetVariable());
                CreateAssetMenu(variableNode, new SetVariable());

                {
                    var variableListNode = CreateGroup(variableNode, "Variable List");
                    CreateAssetMenu(variableListNode, new CreateVariableList());
                    CreateAssetMenu(variableListNode, new GetVariableFromIndex());
                    CreateAssetMenu(variableListNode, new SetVariableToIndex());
                    CreateAssetMenu(variableListNode, new AppendVariableList());
                }
            }

            {
                var flowOperation = CreateGroup(assetNode, "Flow");
                CreateAssetMenu(flowOperation, new If());
                CreateAssetMenu(flowOperation, new If_Func());
                CreateAssetMenu(flowOperation, new If_Action());
                CreateAssetMenu(flowOperation, new For());
                CreateAssetMenu(flowOperation, new Foreach());
            }

            {
                var listNode = CreateGroup(assetNode, "List");
                CreateAssetMenu(listNode, new Count());
                CreateAssetMenu(listNode, new Contains());
                CreateAssetMenu(listNode, new GetListIndex());
                CreateAssetMenu(listNode, new GetListLast());
                CreateAssetMenu(listNode, new SetListIndex());
                CreateAssetMenu(listNode, new Append());
            }

            {
                var funcNode = CreateGroup(assetNode, "f(x)");
                CreateAssetMenu(funcNode, new CallerArgument());
                CreateAssetMenu(funcNode, new CallerArguments());
                CreateAssetMenu(funcNode, new Invoke());
                CreateAssetMenu(funcNode, new InvokeWithArg());
                CreateAssetMenu(funcNode, new InvokeAction());
                CreateAssetMenu(funcNode, new InvokeActionWithArg());
            }

            {
                var embeddedNode = CreateGroup(assetNode, "Embedded");

                {
                    var mathNode = CreateGroup(embeddedNode, "Math");
                    CreateAssetMenu(mathNode, new Abs());
                    CreateAssetMenu(mathNode, new Inc());
                    CreateAssetMenu(mathNode, new Dec());
                    CreateAssetMenu(mathNode, new Sum());
                    CreateAssetMenu(mathNode, new Sum_Func());
                    CreateAssetMenu(mathNode, new Sub());
                    CreateAssetMenu(mathNode, new Mul());
                    CreateAssetMenu(mathNode, new Div());
                    CreateAssetMenu(mathNode, new Pow());
                    CreateAssetMenu(mathNode, new Mod());
                    CreateAssetMenu(mathNode, new Rand());
                }

                {
                    var logicalOperation = CreateGroup(embeddedNode, "Logical operation");
                    CreateAssetMenu(logicalOperation, new And());
                    CreateAssetMenu(logicalOperation, new Or());
                    CreateAssetMenu(logicalOperation, new Not());
                }

                {
                    var comparisonNode = CreateGroup(embeddedNode, "Comparison");
                    CreateAssetMenu(comparisonNode, new Eq());
                    CreateAssetMenu(comparisonNode, new Gt());
                    CreateAssetMenu(comparisonNode, new Ge());
                    CreateAssetMenu(comparisonNode, new Lt());
                    CreateAssetMenu(comparisonNode, new Le());
                }
            }

            {
                var functionNode = CreateGroup(assetNode, "Function");

                {
                    var fileLib = CreateGroup(functionNode, "File");
                    CreateAssetMenu(fileLib, new CallFile());
                    CreateAssetMenu(fileLib, new ConsoleOut());
                }

                ScriptImplement.ImplemantScriptMethods(functionNode);
            }
        }
    }

    //-----------------------------------------------------------------
    class LiteralType : IFuncAssetLiteralDef
    {
        public string MenuTitle => "Literal";

        public string HelpText { get; } = Language.GetInstance["LiteralType"];

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;
    }

    //-----------------------------------------------------------------
    class LiteralListType : IFuncAssetLiteralDef
    {
        public string MenuTitle => "Literal List";

        public string HelpText { get; } = Language.GetInstance["LiteralListType"];

        public CbST TargetType => CbST.FreeListType;

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;
    }

    //-----------------------------------------------------------------
    class Sum : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Sum);

        public string HelpText { get; } = Language.GetInstance["Sum"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbAddableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            TryArgListProc(argument[0],
                                (valueData) =>
                                {
                                    ret.Add(valueData);
                                });
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Sum_Func : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Sum_Func);

        public string HelpText { get; } = Language.GetInstance["Sum_Func"];

        public string MenuTitle => "Sum<Func>";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbAddableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            // 仮引数コントロールを作成
            DummyArgumentsControl dummyArgumentsControl = new DummyArgumentsControl(col);

            col.MakeFunction(
                $"Sum Func<{col.SelectedVariableTypeName[0]},{col.SelectedVariableTypeName[0]}>.Invoke",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "base"),
                    CbList.Create(typeof(Func<,>).MakeGenericType(
                        col.SelectedVariableType[0],
                        col.SelectedVariableType[0]),
                        "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        if (dummyArgumentsControl.IsInvalid(cagt))
                            return ret; // 実行環境が有効でない

                        try
                        {
                            ret.Set(argument[0]);

                            TryArgListProc(argument[1],
                                (valueData) =>
                                {
                                    dummyArgumentsControl.Enable(cagt, ret.Data);    // 仮引数に引数を登録

                                    if (IsCallBack(valueData))
                                        ret.Add(CallEvent(valueData, cagt));

                                    dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                                });
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Subroutine : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Subroutine);

        public string HelpText { get; } = Language.GetInstance["Subroutine"];

        public string MenuTitle => "Sequence";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbList.Create(typeof(object), "call list"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            var argList = GetArgumentList(argument, 0);
                            if (argList.Count != 0)
                                ret.Set(argList[argList.Count - 1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            // 実行を可能にする
            col.LinkConnectorControl.IsRunable = true;

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Inc : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Inc);

        public string HelpText { get; } = Language.GetInstance["Inc"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);
                            ret.Add(CbInt.Create(1));
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Dec : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Dec);

        public string HelpText { get; } = Language.GetInstance["Dec"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);
                            ret.Subtract(CbInt.Create(1));
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Mod : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Mod);

        public string HelpText { get; } = Language.GetInstance["Mod"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Modulo",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n2"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);
                            ret.Modulo(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Eq : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Eq);

        public string HelpText { get; } = Language.GetInstance["Eq"];

        public string MenuTitle => "==";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Comparison ==",
                HelpText,
                CbST.CbCreateTF<bool>(),        // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n2"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var temp = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        var ret = CbST.CbCreate<bool>();
                        try
                        {
                            temp.Set(argument[0]);
                            ret.Data = temp.Equal(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Ge : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Ge);

        public string HelpText { get; } = Language.GetInstance["Ge"];

        public string MenuTitle => ">=";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Comparison >=",
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n2"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var temp = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        var ret = CbST.CbCreate<bool>();
                        try
                        {
                            temp.Set(argument[0]);
                            ret.Data = temp.GreaterThanOrEqual(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Gt : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Gt);

        public string HelpText { get; } = Language.GetInstance["Gt"];

        public string MenuTitle => ">";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Comparison >",
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n2"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var temp = CbST.CbCreate(col.SelectedVariableType[0]);
                        var ret = CbST.CbCreate<bool>();    // 返し値
                        try
                        {
                            temp.Set(argument[0]);
                            ret.Data = temp.GreaterThan(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Le : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Le);

        public string HelpText { get; } = Language.GetInstance["Le"];

        public string MenuTitle => "<=";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Comparison <=",
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()    // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n2"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var temp = CbST.CbCreate(col.SelectedVariableType[0]);
                        var ret = CbST.CbCreate<bool>();    // 返し値
                        try
                        {
                            temp.Set(argument[0]);
                            ret.Data = temp.LessThanOrEqual(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Lt : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Lt);

        public string HelpText { get; } = Language.GetInstance["Lt"];

        public string MenuTitle => "<";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Comparison <",
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n2"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var temp = CbST.CbCreate(col.SelectedVariableType[0]);
                        var ret = CbST.CbCreate<bool>();    // 返し値
                        try
                        {
                            temp.Set(argument[0]);
                            ret.Data = temp.LessThan(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class And : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(And);

        public string HelpText { get; } = Language.GetInstance["And"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.Bool;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbList.Create(typeof(bool), "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        bool temp = true;
                        var ret = CbST.CbCreate<bool>();    // 返し値
                        try
                        {
                            TryArgListProc(argument[0],
                                (valueData) =>
                                {
                                    temp = temp && GetArgument<bool>(valueData);
                                });

                            ret.Data = temp;
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Or : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Or);

        public string HelpText { get; } = Language.GetInstance["Or"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.Bool;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbList.Create(typeof(bool), "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        bool temp = false;
                        var ret = CbST.CbCreate<bool>();    // 返し値
                        try
                        {
                            TryArgListProc(argument[0],
                                (valueData) =>
                                {
                                    temp = temp || GetArgument<bool>(valueData);
                                });

                            ret.Data = temp;
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Not : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Not);

        public string HelpText { get; } = Language.GetInstance["Not"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.Bool;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbST.CbCreate<bool>("sample", false),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate<bool>();    // 返し値
                        try
                        {
                            ret.Data = !GetArgument<bool>(argument, 0);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Mul : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Mul);

        public string HelpText { get; } = Language.GetInstance["Mul"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Multiply",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "base"),
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);

                            TryArgListProc(argument[1],
                                (valueData) =>
                                {
                                    ret.Multiply(valueData);
                                });
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Div : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Div);

        public string HelpText { get; } = Language.GetInstance["Div"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Divide",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n1"),
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);
                            TryArgListProc(argument[1],
                               (valueData) =>
                               {
                                   ret.Divide(valueData);
                               });
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Sub : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Sub);

        public string HelpText { get; } = Language.GetInstance["Sub"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "Subtract",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "base"),
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);

                            TryArgListProc(argument[1],
                                (valueData) =>
                                {
                                    ret.Subtract(valueData);
                                });
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class CallFile : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(CallFile);

        public string HelpText { get; } = Language.GetInstance["CallFile"];

        public string MenuTitle => "Call File";

        public CbST TargetType => CbST.String;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbFunc<object, CbInt>.TF,   // 返し値の型
                new List<ICbValue>()      // 引数
                {
                    CbST.CbCreate<string>("path"),
                    CbST.CbCreate<bool>("redirect"),
                    CbST.CbCreate<List<string>>("arguments"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate<Func<object, int>>() as ICbEvent;    // 返し値
                        try
                        {
                            string path = GetArgument<string>(argument, 0);
                            bool redirect = GetArgument<bool>(argument, 1);

                            ToolExec toolExec = new ToolExec(path);

                            TryArgListProc(argument[2],
                                (valueData) =>
                                {
                                    toolExec.ParamData.Add(GetArgument<string>(valueData));
                                });

                            ret.CallBack = (cagt2) =>
                            {
                                return CbInt.Create(toolExec.Start(col.LinkConnectorControl.Caption, redirect));
                            };
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class If_Func : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(If_Func);

        public string HelpText { get; } = Language.GetInstance["If_Func"];

        public string MenuTitle => $"If<{CbSTUtils.FUNC_STR}>";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                $"If {CbSTUtils.FUNC_STR}<{col.SelectedVariableTypeName[0]}>.Invoke",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<bool>("conditions", false),
                    CbFunc.CreateFunc(col.SelectedVariableType[0], "func true"),
                    CbFunc.CreateFunc(col.SelectedVariableType[0], "func false"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            if (GetArgument<bool>(argument, 0))
                            {
                                if (IsCallBack(argument[1]))
                                    ret.Set(CallEvent(argument[1], cagt));
                            }
                            else
                            {
                                if (IsCallBack(argument[2]))
                                    ret.Set(CallEvent(argument[2], cagt));
                            }
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class If_Action : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(If_Action);

        public string HelpText { get; } = Language.GetInstance["If_Action"];

        public string MenuTitle => $"If<{CbSTUtils.ACTION_STR}>";

        public CbST TargetType => CbST.Int;    // dummy

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                $"If {CbSTUtils.ACTION_STR}<{col.SelectedVariableTypeName[0]}>.Invoke",
                HelpText,
                CbVoid.TF,  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<bool>("conditions", false),
                    CbFunc.CreateAction(col.SelectedVariableType[0], "func true"),
                    CbFunc.CreateAction(col.SelectedVariableType[0], "func false"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        try
                        {
                            if (GetArgument<bool>(argument, 0))
                            {
                                if (IsCallBack(argument[1]))
                                    CallEvent(argument[1], cagt);
                            }
                            else
                            {
                                if (IsCallBack(argument[2]))
                                    CallEvent(argument[2], cagt);
                            }
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(null, ex);
                        }
                        return null;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class If : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(If);

        public string HelpText { get; } = Language.GetInstance["If"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "If<" + col.SelectedVariableTypeName[0] + ">",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<bool>("conditions", false),
                    CbST.CbCreate(col.SelectedVariableType[0], "return true"),
                    CbST.CbCreate(col.SelectedVariableType[0], "return false"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            if (GetArgument<bool>(argument, 0))
                            {
                                ret.Set(argument[1]);
                            }
                            else
                            {
                                ret.Set(argument[2]);
                            }
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Invoke : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Invoke);

        public string HelpText { get; } = Language.GetInstance["Invoke"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                $"{CbSTUtils.FUNC_STR}<{col.SelectedVariableTypeName[0]}>.Invoke",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbFunc.CreateFunc(col.SelectedVariableType[0], "func"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            if (IsCallBack(argument[0]))
                                ret.Set(CallEvent(argument[0], cagt));
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            // 実行を可能にする
            col.LinkConnectorControl.IsRunable = true;

            return true;
        }
    }

    //-----------------------------------------------------------------
    class InvokeWithArg : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(InvokeWithArg);

        public string HelpText { get; } = Language.GetInstance["InvokeWithArg"];

        public string MenuTitle => "Invoke With Argument";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            // 仮引数コントロールを作成
            DummyArgumentsControl dummyArgumentsControl = new DummyArgumentsControl(col);

            col.MakeFunction(
                $"{CbSTUtils.FUNC_STR}<{CbSTUtils.OBJECT_STR},{col.SelectedVariableTypeName[0]}>.Invoke",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                   CbST.CbCreate(col.SelectedVariableType[0], "argument"),
                   CbFunc.CreateFunc(typeof(object), col.SelectedVariableType[0], "func"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        if (dummyArgumentsControl.IsInvalid(cagt))
                            return ret; // 実行環境が有効でない

                        try
                        {
                            dummyArgumentsControl.EnableCbValue(cagt, argument[0]);    // 仮引数に引数を登録

                            if (IsCallBack(argument[1]))
                                ret.Set(CallEvent(argument[1], cagt));

                            dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            // 実行を可能にする
            col.LinkConnectorControl.IsRunable = true;

            return true;
        }
    }

    //-----------------------------------------------------------------
    class InvokeAction : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(InvokeAction);

        public string HelpText { get; } = Language.GetInstance["Invoke"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.Int; // dummy

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                $"{CbSTUtils.ACTION_STR}.Invoke",
                HelpText,
                CbVoid.TF,  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbFunc.CreateAction("func"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        try
                        {
                            if (IsCallBack(argument[0]))
                                CallEvent(argument[0], cagt);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(null, ex);
                        }
                        return null;
                    }
                    )
                );

            // 実行を可能にする
            col.LinkConnectorControl.IsRunable = true;

            return true;
        }
    }

    //-----------------------------------------------------------------
    class InvokeActionWithArg : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(InvokeActionWithArg);

        public string HelpText { get; } = Language.GetInstance["InvokeWithArg"];

        public string MenuTitle => "Invoke With Argument";

        public CbST TargetType => CbST.Object;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            // 仮引数コントロールを作成
            DummyArgumentsControl dummyArgumentsControl = new DummyArgumentsControl(col);

            col.MakeFunction(
                $"{CbSTUtils.ACTION_STR}<{CbSTUtils.OBJECT_STR}>.Invoke",
                HelpText,
                CbVoid.TF,    // 返し値の型
                new List<ICbValue>()          // 引数
                {
                   CbST.CbCreate(col.SelectedVariableType[0], "argument"),
                   CbFunc.CreateAction(typeof(object), "func"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        if (dummyArgumentsControl.IsInvalid(cagt))
                            return null; // 実行環境が有効でない

                        try
                        {
                            dummyArgumentsControl.EnableCbValue(cagt, argument[0]);    // 仮引数に引数を登録

                            if (IsCallBack(argument[1]))
                                CallEvent(argument[1], cagt);

                            dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(null, ex);
                        }
                        return null;
                    }
                    )
                );

            // 実行を可能にする
            col.LinkConnectorControl.IsRunable = true;

            return true;
        }
    }

    //-----------------------------------------------------------------
    class For : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(For);

        public string HelpText { get; } = Language.GetInstance["For"];

        public string MenuTitle => $"For<{CbSTUtils.ACTION_STR}>";

        public CbST TargetType => CbST.Int;    // dummy

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbCalcableTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            // 仮引数コントロールを作成
            DummyArgumentsControl dummyArgumentsControl = new DummyArgumentsControl(col);

            col.MakeFunction(
                $"For {CbSTUtils.ACTION_STR}<{CbSTUtils.INT_STR}>.Invoke",
                HelpText,
                CbVoid.TF,        // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<int>("[from", 0),
                    CbST.CbCreate<int>("to)", 0),
                    CbST.CbCreate<int>("step", 1),
                    CbFunc.CreateAction(typeof(int), "func f(index)"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        if (dummyArgumentsControl.IsInvalid(cagt))
                            return null; // 実行環境が有効でない

                        try
                        {
                            int n1 = GetArgument<int>(argument, 0);
                            int n2 = GetArgument<int>(argument, 1);
                            int step = GetArgument<int>(argument, 2);
                            for (int i = n1; i < n2; i += step)
                            {
                                if (argument[3] is ICbEvent cbEvent && cbEvent.CallBack is null)
                                {

                                }
                                else
                                {
                                    dummyArgumentsControl.Enable(cagt, i);    // 仮引数に引数を登録
                                    if (IsCallBack(argument[3]))
                                        CallEvent(argument[3], cagt);
                                    dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(null, ex);
                        }
                        return null;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class _GetVariable : FuncAssetSub
    {
        public string AssetCode => nameof(GetVariable);

        public string HelpText { get; } = "";

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            VariableGetter variableGetter = new VariableGetter(col);
            if (variableGetter.IsError)
                return false;

            col.MakeFunction(
                variableGetter.MakeName,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                null,   // 引数はなし
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        ICbValue ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ICbValue cbVSValue = CommandCanvas.ScriptWorkStack.Find(variableGetter.Id);
                            ret.CopyValue(cbVSValue);
                            col.LinkConnectorControl.UpdateValueData();
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class CreateVariable : _GetVariable, IFuncCreateVariableAssetDef
    {
        public string MenuTitle => "Create Variable";

        public new string HelpText { get; } = Language.GetInstance["CreateVariable"];

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;
    }

    //-----------------------------------------------------------------
    class CreateVariableList : _GetVariable, IFuncCreateVariableAssetDef
    {
        public string MenuTitle => "Create VariableList";

        public new string HelpText { get; } = Language.GetInstance["CreateVariableList"];

        public CbST TargetType => CbST.FreeListType;

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;
    }

    //-----------------------------------------------------------------
    class CreateVariableFunc : _GetVariable, IFuncCreateVariableAssetDef
    {
        public CbST TargetType => CbST.FreeFuncType;

        public new string HelpText { get; } = Language.GetInstance["CreateVariableFunc"];

        public string MenuTitle => $"Create Variable<{CbSTUtils.FUNC_STR}>";

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;
    }

    //-----------------------------------------------------------------
    class GetVariable : _GetVariable, IFuncCreateVariableAssetDef
    {
        public string MenuTitle => "Get Variable";

        public new string HelpText { get; } = Language.GetInstance["GetVariable"];

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => null;
    }

    //-----------------------------------------------------------------
    class SetVariable : FuncAssetSub, IFuncCreateVariableAssetDef
    {
        public string AssetCode => nameof(SetVariable);

        public string HelpText { get; } = Language.GetInstance["SetVariable"];

        public string MenuTitle => "Set Variable";

        public CbST TargetType => CbST.FreeFuncType;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            VariableGetter variableGetter = new VariableGetter(col);
            if (variableGetter.IsError)
                return false;

            col.MakeFunction(
                variableGetter.MakeName,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        ICbValue ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ICbValue cbVSValue = CommandCanvas.ScriptWorkStack.Find(variableGetter.Id);

                            if (argument[0] is ICbList cbList && cbVSValue is ICbList toList)
                            {
                                // リストのコピー

                                cbList.CopyTo(toList);
                            }
                            else
                            {
                                // 値のコピー

                                cbVSValue.CopyValue(argument[0]);
                            }
                            CommandCanvas.ScriptWorkStack.UpdateValueData(variableGetter.Id);
                            ret.Set(argument[0]);
                            col.LinkConnectorControl.UpdateValueData();
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class CallerArgument : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(CallerArgument);

        public string HelpText { get; } = Language.GetInstance["CallerArgument"];

        public string MenuTitle => "DummyArgument";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "DummyArgument<" + col.SelectedVariableTypeName[0] + ">",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                null,   // 引数はなし
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        if (cagt.IsEmpty())
                        {
                            cagt.InvalidReturn();   // 有効でないまま返す
                            return ret;
                        }
                        try
                        {
                            // 呼び元引数をセット

                            if (cagt.IsGetValue())
                                ret.Set(cagt.GetValue());
                            else
                                cagt.InvalidReturn();   // 有効でないまま返す
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class CallerArguments : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(CallerArguments);

        public string HelpText { get; } = Language.GetInstance["CallerArgument"];

        public string MenuTitle => "DummyArguments";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                "DummyArguments<" + col.SelectedVariableTypeName[0] + ">",
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<CbFuncArguments.INDEX>("select"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値

                        var select = GetArgument<CbFuncArguments.INDEX>(argument, 0);

                        if (cagt.IsEmpty())
                        {
                            cagt.InvalidReturn();   // 有効でないまま返す
                            return ret;
                        }
                        try
                        {
                            // 呼び元引数をセット

                            if (cagt.IsGetValue())
                                ret.Set(cagt.GetValue(select));
                            else
                                cagt.InvalidReturn();   // 有効でないまま返す
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Foreach : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Foreach);

        public string HelpText { get; } = Language.GetInstance["Foreach"];

        public string MenuTitle => $"Foreach<{CbSTUtils.ACTION_STR}>";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            // 仮引数コントロールを作成
            DummyArgumentsControl dummyArgumentsControl = new DummyArgumentsControl(col);

            col.MakeFunction(
                $"Foreach {CbSTUtils.ACTION_STR}<{col.SelectedVariableTypeName[0]}>.Invoke",
                HelpText,
                CbVoid.TF,        // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                    CbFunc.CreateAction(col.SelectedVariableType[0], "func f(node)"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        if (dummyArgumentsControl.IsInvalid(cagt))
                            return null; // 実行環境が有効でない

                        try
                        {
                            var sample = GetArgumentList(argument, 0);
                            foreach (var node in sample)
                            {
                                dummyArgumentsControl.Enable(cagt, node.Data);    // 仮引数に引数を登録
                                if (IsCallBack(argument[1]))
                                    CallEvent(argument[1], cagt);
                                dummyArgumentsControl.Invalidated(cagt);    // 仮引数後処理
                            }
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(null, ex);
                        }

                        return null;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class ConsoleOut : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(ConsoleOut);

        public string HelpText { get; } = Language.GetInstance["ConsoleOut"];

        public string MenuTitle => "ConsoleOut";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.ObjectDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Set(argument[0]);
                            string str = argument[0].ValueString;
                            MainWindow.Instance.MainLog.OutLine("Console Out", str);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Abs : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Abs);

        public string HelpText { get; } = Language.GetInstance["Abs"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.MathDeleteNotCbSignedNumberTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            ret.Data = Math.Abs((dynamic)argument[0].Data);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class GetVariableFromIndex : FuncAssetSub, IFuncCreateVariableListAssetDef
    {
        public string AssetCode => nameof(GetVariableFromIndex);

        public string HelpText { get; } = Language.GetInstance["GetVariableFromIndex"];

        public string MenuTitle => "Get VariableList[index]";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            VariableGetter variableGetter = new VariableGetter(col, (name) => "[ " + name + " [index] ]");
            if (variableGetter.IsError)
                return false;

            col.MakeFunction(
               variableGetter.MakeName,
               HelpText,
               CbST.CreateTF(col.AssetLiteralType.GetObjectTypeNone()), // 返し値の型   Func<> 型の場合に返し値の型で接続するために ObjectType を落としている（方法に置き換える予定）
               new List<ICbValue>()       // 引数
               {
                   CbST.CbCreate<int>("index", 0),
               },
               new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                   (argument, cagt) =>
                   {
                       var ret = CbST.Create(col.AssetLiteralType.GetObjectTypeNone());    // 返し値
                       try
                       {
                           int index = GetArgument<int>(argument, 0);

                           ICbValue cbVSValue = CommandCanvas.ScriptWorkStack.Find(variableGetter.Id);
                           var argList = (cbVSValue as ICbList).Value;

                           ret.CopyValue(argList[index]);
                           col.LinkConnectorControl.UpdateValueData();
                       }
                       catch (Exception ex)
                       {
                           col.ExceptionFunc(ret, ex);
                       }
                       return ret;
                   }
                   )
               );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class SetVariableToIndex : FuncAssetSub, IFuncCreateVariableListAssetDef
    {
        public string AssetCode => nameof(SetVariableToIndex);

        public string HelpText { get; } = Language.GetInstance["SetVariableToIndex"];

        public string MenuTitle => "Set VariableList[index]";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            VariableGetter variableGetter = new VariableGetter(col, (name) => "[ " + name + " [index] ]");
            if (variableGetter.IsError)
                return false;

            col.MakeFunction(
               variableGetter.MakeName,
               HelpText,
               CbST.CreateTF(col.AssetLiteralType.GetObjectTypeNone()), // 返し値の型    Func<> 型の場合に返し値の型で接続するために ObjectType を落としている（方法に置き換える予定）
               new List<ICbValue>()       // 引数
               {
                    CbST.CbCreate<int>("index", 0),
                    CbST.Create(col.AssetLiteralType.GetObjectTypeNone(), "n"),  // Func<> 型の場合に返し値の型で接続するために ObjectType を落としている（方法に置き換える予定）
               },
               new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                   (argument, cagt) =>
                   {
                       var ret = CbST.Create(col.AssetLiteralType.GetObjectTypeNone());    // 返し値
                       try
                       {
                           int index = GetArgument<int>(argument, 0);

                           ICbValue cbVSValue = CommandCanvas.ScriptWorkStack.Find(variableGetter.Id);
                           var argList = (cbVSValue as ICbList).Value;

                           argList[index].CopyValue(argument[1]);
                           CommandCanvas.ScriptWorkStack.UpdateValueData(variableGetter.Id);
                           ret.Set(argument[1]);
                           col.LinkConnectorControl.UpdateValueData();

                       }
                       catch (Exception ex)
                       {
                           col.ExceptionFunc(ret, ex);
                       }
                       return ret;
                   }
                   )
               );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class AppendVariableList : FuncAssetSub, IFuncCreateVariableListAssetDef
    {
        public string AssetCode => nameof(AppendVariableList);

        public string HelpText { get; } = Language.GetInstance["AppendVariableList"];

        public string MenuTitle => "Append VariableList";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            VariableGetter variableGetter = new VariableGetter(col, (name) => "Append [ " + name + " ]");
            if (variableGetter.IsError)
                return false;

            col.MakeFunction(
               variableGetter.MakeName,
               HelpText,
               CbST.CbCreateTF(col.SelectedVariableType[0]),   // 返し値の型
               new List<ICbValue>()   // 引数
               {
                    CbST.Create(col.AssetLiteralType.GetObjectTypeNone(), "n"), // Func<> 型の場合に返し値の型で接続するために ObjectType を落としている（方法に置き換える予定）
               },
               new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                   (argument, cagt) =>
                   {
                       var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                       try
                       {
                           ICbValue cbVSValue = CommandCanvas.ScriptWorkStack.Find(variableGetter.Id);
                           var argList = cbVSValue as ICbList;
                           argList.Append(argument[0]);

                           CommandCanvas.ScriptWorkStack.UpdateValueData(variableGetter.Id);
                           argList.CopyTo(ret as ICbList);
                           col.LinkConnectorControl.UpdateValueData();
                       }
                       catch (Exception ex)
                       {
                           col.ExceptionFunc(ret, ex);
                       }
                       return ret;
                   }
                   )
               );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Count : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Count);

        public string HelpText { get; } = Language.GetInstance["Count"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<int>(),   // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate<int>();    // 返し値
                        try
                        {
                            ret.Data = (argument[0] as ICbList).Count;
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Contains : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Contains);

        public string HelpText { get; } = Language.GetInstance["Contains"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<bool>(),  // 返し値の型
                new List<ICbValue>()          // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbBool.Create(false);    // 返し値
                        try
                        {
                            ret.Data = (argument[0] as ICbList).Contains(argument[1]);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class GetListIndex : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(GetListIndex);

        public string HelpText { get; } = Language.GetInstance["GetListIndex"];

        public string MenuTitle => "Get List[index]";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),    // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                    CbST.CbCreate<int>("index"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            int index = GetArgument<int>(argument, 1);
                            ret.Data = (argument[0] as ICbList)[index].Data;
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class GetListLast : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(GetListLast);

        public string HelpText { get; } = Language.GetInstance["GetListLast"];

        public string MenuTitle => "Get List[last]";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            var argList = argument[0] as ICbList;
                            ret.Data = argList[argList.Count - 1].Data;
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class SetListIndex : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(SetListIndex);

        public string HelpText { get; } = Language.GetInstance["SetListIndex"];

        public string MenuTitle => "Set List[index]";

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF(col.SelectedVariableType[0]),  // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbList.Create(col.SelectedVariableType[0], "sample"),
                    CbST.CbCreate<int>("index"),
                    CbST.CbCreate(col.SelectedVariableType[0], "n"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate(col.SelectedVariableType[0]);    // 返し値
                        try
                        {
                            int index = GetArgument<int>(argument, 1);
                            (argument[0] as ICbList)[index].Data = argument[2].Data;
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Append : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Append);

        public string HelpText { get; } = Language.GetInstance["Append"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.FreeType;    // 型の選択を要求する

        public CbType[] DeleteSelectItems => CbScript.BaseDeleteCbTypes;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
               MenuTitle,
               HelpText,
               CbList.CreateTF(col.SelectedVariableType[0]),  // 返し値の型
               new List<ICbValue>()   // 引数
               {
                    CbList.Create(col.SelectedVariableType[0], "list"),
                    CbST.Create(col.AssetLiteralType.GetObjectTypeNone(), "n"), // Func<> 型の場合に返し値の型で接続するために ObjectType を落としている（方法に置き換える予定）
               },
               new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                   (argument, cagt) =>
                   {
                       var ret = CbList.Create(col.SelectedVariableType[0]);    // 返し値
                       try
                       {
                           var argList = argument[0] as ICbList;
                           argList.CopyTo(ret as ICbList);
                           (ret as ICbList).Append(argument[1]);

                           col.LinkConnectorControl.UpdateValueData();
                       }
                       catch (Exception ex)
                       {
                           col.ExceptionFunc(ret, ex);
                       }
                       return ret;
                   }
                   )
               );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Pow : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Pow);

        public string HelpText { get; } = Language.GetInstance["Pow"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.Double;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<double>(),      // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<double>("n"),
                    CbST.CbCreate<double>("p"),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate<double>();    // 返し値
                        try
                        {
                            var n = GetArgument<double>(argument, 0);
                            var p = GetArgument<double>(argument, 1);

                            ret.Data = Math.Pow(n, p);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }

    //-----------------------------------------------------------------
    class Rand : FuncAssetSub, IFuncAssetWithArgumentDef
    {
        public string AssetCode => nameof(Rand);

        public string HelpText { get; } = Language.GetInstance["Rand"];

        public string MenuTitle => AssetCode;

        public CbST TargetType => CbST.Int;

        public CbType[] DeleteSelectItems => null;

        public bool ImplAsset(MultiRootConnector col, bool notheradMode = false)
        {
            col.MakeFunction(
                MenuTitle,
                HelpText,
                CbST.CbCreateTF<int>(),   // 返し値の型
                new List<ICbValue>()  // 引数
                {
                    CbST.CbCreate<int>("max", 0),
                    CbST.CbCreate<int>("int", 1),
                },
                new Func<List<ICbValue>, DummyArgumentsStack, ICbValue>(
                    (argument, cagt) =>
                    {
                        var ret = CbST.CbCreate<int>();   // 返し値
                        try
                        {
                            var min = GetArgument<int>(argument, 0);
                            var max = GetArgument<int>(argument, 1);
                            ret.Data = random.Next(min, max + 1);
                        }
                        catch (Exception ex)
                        {
                            col.ExceptionFunc(ret, ex);
                        }
                        return ret;
                    }
                    )
                );

            return true;
        }
    }
}