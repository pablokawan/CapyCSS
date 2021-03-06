# CapyCSS
![CapyCSS](https://user-images.githubusercontent.com/63950487/99184540-e5d56300-2786-11eb-8868-dd06a950d7d6.png)

## 最新の変更

型関連の扱いを整理しました。
<br>保存データの形式が変わり、過去のバージョンに対応しなくなりました。
<br>型を選択するときに自由な型を選択できるようになりました。
<br>ただし、ジェネリック型は、型制約に未対応です。

## 特徴
* ビジュアルなスクリプトを作成することができます。
* c#で書かれたソースを修正してメソッドを追加することでノードとして使用できるようにインポートする機能があります（属性の指定とビルドが必要です）。
* dllをインポートしてメソッドをスクリプトで使うことができます。
* クラス指定でメソッドをインポートしてスクリプトで使うことができます。
* NuGetからパッケージをインポートしてスクリプトで使うことができます。

![CapyCSS ver 0 1 2 0](https://user-images.githubusercontent.com/63950487/105629319-ab263000-5e85-11eb-8716-58e26444338b.png)

## ターゲット環境
* .Net 5.0
* c＃

## 実行オプション
保存したスクリプトファイルをコマンド引数で指定すると、起動時に自動的に読み込まれます。
オプションとして-asが併せて指定されている場合、「Entry」をチェックされたノードは、スクリプトのロード後に自動で実行されます。
オプションとして-aeを指定した場合、スクリプト実行後に自動的に終了します。
オプションとして-aseを指定した場合、-asと-aeを併せて実行した場合と同様になります。
```
CapyCSS.exe script.cbs
CapyCSS.exe -as script.cbs
CapyCSS.exe -as -ae script.cbs
CapyCSS.exe -ase script.cbs
```
「SetExitCode」ノードを使って終了コードをセットできます。

## コンソール出力

コンソールから起動した場合は、「Call File」や「ConsoleOut」系のノードからの出力は、コンソールにも出力されます。

## ノードのインポート
本ツールでは、c# の多くのメソッドを簡単にノード化することができます。
下記は、属性を使用してメソッドをノード化する例です。
```
[ScriptMethod]
public static System.IO.StreamReader GetReadStream(string path, string encoding = "utf-8")
{
    var encodingCode = Encoding.GetEncoding(encoding);
    return new System.IO.StreamReader(path, encodingCode);
}
```

## 操作方法
* スペースキーもしくはホイールボタンの押下でコマンドウインドウを表示できます。
* コマンドウインドウを表示し、コマンドメニューの「Program」下から項目をクリックしてノードを配置できます。
* ノードの○を左クリックしてドラッグし、ノードの□に接続できます。
* Ctrlキーを押しながら□を左クリックすると、接続しているノードを切断できます。
* Shiftキーを押しながらマウスをドラッグして範囲内のノードを複数選択します。
* Deleteキーで選択されているノードを削除できます。
* ノード名を左クリックしてノードをドラッグ移動できます（Ctrlキーを押下した状態だと、ガイドに沿って移動します）。
* 画面を左クリックして画面全体をドラッグ移動できます（選択されたノードがある場合、選択されているノードが移動します）。
* ホイールボタンの回転で表示を拡大もしくは縮小できます。
* Ctrl + Aで全選択
* Ctrl + Spaceで選択解除
* Ctrl + Cで選択したノードをコピーできます。
* Ctrl + Vでコピーされているノードを貼り付けられます。
* Ctrl + Sでスクリプトを上書き保存できます。
* Ctrl + Oでスクリプトの読み込みができます。
* Ctrl + Nで新規にスクリプトを作成できます。
* Ctrl + Shift + Nでスクリプトをクリアできます。
* jキーでスクリプトの表示を画面の中央に画面に収まるように調整します。
* Ctrl + jキーでスクリプトの表示位置を画面の左上に調整します。
* F5でEntryをチェックされたノードをすべて実行できます。

※ノード名、引数名はダブルクリックで編集できます。ただし、変数名は画面左側の変数名一覧でのみダブルクリックで編集できます。

## スクリプトの便利な機能
* 引数の上にカーソルを置くと内容を確認できます。
* 数字と文字をドラッグアンドドロップするだけで定数ノードを作成できます。
* ノードの接続時に型キャストが可能です。
* object型からのキャストをできるようにしました。

## ヒント表示
英語と日本語をサポートします。ただし、初期状態では英語のみの機械翻訳です。
日本語で表示するには、実行後に作成されるapp.xmlファイルの内容を変更します。
```<Language>en-US</Language>```
上記の内容を次のように変更します。
```<Language>ja-JP</Language>```
再起動すると設定が適用されます。

## メソッドのインポートでサポートされるメソッド
* Static method
* Class method(thisを受け取る引数が追加されます)

※メソッドの所属するクラスは、パブリックである必要があります。
<br>※メソッドはパブリックである必要があります。
<br>※象徴クラスやジェネリッククラスのメソッド及びジェネリックなメソッドは、対象外です。

## スクリプトが対応するメソッドの引数の型（及び修飾子など）
* Type: int, string, double, byte, sbyte, long, short, ushort, uint, ulong, char, float, decimal, bool, object
* 配列
* Class
* Struct
* Interface
* Enum
* Generics
* デフォルト値
* ref（リファレンス）
* outパラメーター修飾子
* inパラメーター修飾子
* paramsパラメータ配列
* null許容型

※リテラルノードのみ独自のtext型を用意しています。string型とobject型へ代入可能です。
<br>※オーバーロードに対応しています。

## スクリプトが対応するメソッドの戻り値の型
* Type: int, string, double, byte, sbyte, long, short, ushort, uint, ulong, char, float, decimal, bool, object
* 配列
* Class
* Struct
* Interface
* Enum
* Generics
* void
* null許容型

## メソッドからのメソッド呼び出し
Func<> および Action<> タイプの引数は、ノードを外部プロセスとして呼び出すことができます。Func<> の場合、ノード型と戻り値型が一致した場合に接続できます。Action だと無条件に接続できます。

## Hello World!
「Hello World!」と出力するサンプルです。<br>
![CapyCSS01](https://user-images.githubusercontent.com/63950487/97863495-6f7a3f00-1d4a-11eb-9ef4-0017be21d13e.png)
<br>ホイールボタンをクリックするかスペースキーでコマンドウインドウが表示されます。
その中からProgram→.Net Function→Input/Output→ConsoleOut→ConsoleOutをクリックします。

![CapyCSS02](https://user-images.githubusercontent.com/63950487/97861283-d4cc3100-1d46-11eb-9aed-1bf981d57ad3.png)
<br>作業エリアをクリックすると型選択ウインドウが表示されますので、その中からstringを選択します。

![CapyCSS03](https://user-images.githubusercontent.com/63950487/97861311-deee2f80-1d46-11eb-8352-ef904e6d8818.png)
<br>引数nの部分に「Hello World!」と入力し、Runをクリックするとスクリプトが実行されます。

![CapyCSS04](https://user-images.githubusercontent.com/63950487/97861328-e7df0100-1d46-11eb-9f44-2c72d97f86a1.png)
<br>このように「Hello World!」と出力されます。

![CapyCSS05](https://user-images.githubusercontent.com/63950487/97861338-eca3b500-1d46-11eb-8c6e-5cc957366621.png)
<br>EntryをチェックするとF5キーで実行できるようになります。

## 簡単な足し算

![CapyCSS](https://user-images.githubusercontent.com/63950487/98212435-ef8ce880-1f86-11eb-9e4b-d2a6612d86ac.gif)
<br>ノードを繋ぐことによって、一連のノードを実行できます。

## グループ化及びテキスト

![CapyCSS02](https://user-images.githubusercontent.com/63950487/98465611-6b986200-220d-11eb-8184-bd8b6a2e9bca.gif)
<br>Test Areaを使うとノードのグループ化とテキストの書き込みが行えます。

## ライセンス
MITライセンス
