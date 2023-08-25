# DocumentViewer
 
「DocumentViewer」は、ARIA Document workspaceに保存されているドキュメントをプレビュー表示するスタンドアローンアプリケーションです。

# Features

* ARIA Document workspaceに保存されているドキュメントの一覧を表示できます
* WebView2コントロールによるドキュメントのプレビュー表示（拡大・縮小表示、印刷、保存など）ができます
* ドキュメントを指定フォルダへ保存することができます
* 患者IDをパラメータに指定してプログラムを起動することができます
* システム・ロケールの言語設定に対応（英語または日本語）
  

# Demo

![Screen capture of planCompare UI](https://github.com/tkmd94/DocumentViewer/blob/master/demo.gif)

# Requirement

* ARIAにOncology Servicesがインストールされている必要があります。
* Oncology Services APIキーの取得が必要です。
* 本プログラムを実行するクライアント環境にWebView2 Runtimeがインストールされている必要があります。

# Installation
1. 接続するARIAサーバーのホスト名を設定する。
   https://github.com/tkmd94/DocumentViewer/blob/1fca929c98a946531d709a9d1c5808f8531b4623/DocumentViewer/App.config#L7
2. 接続するARIAサーバーのポート番号を設定する。
   https://github.com/tkmd94/DocumentViewer/blob/1fca929c98a946531d709a9d1c5808f8531b4623/DocumentViewer/App.config#L8
4. APIキーを設定する。
   https://github.com/tkmd94/DocumentViewer/blob/1fca929c98a946531d709a9d1c5808f8531b4623/DocumentViewer/App.config#L9
5. ローカル保存先を設定する。
   https://github.com/tkmd94/DocumentViewer/blob/1fca929c98a946531d709a9d1c5808f8531b4623/DocumentViewer/App.config#L10
6. Eclipseの下記のDLLをプロジェクトフォルダにコピーします。
   >C:\Program Files (x86)\Varian\OCS\XX.X\VMS.OIS.ARIALocal.WebServices.Document.Contracts.dll  
   >XX.Xはバージョン番号
7. Eclipseの下記のDLLをプロジェクトフォルダにコピーします。
   >C:\Program Files (x86)\Varian\OCS\XX.X\VMS.SF.Gateway.Contracts.dll  
   >XX.Xはバージョン番号
8. このプロジェクトをリリースビルドして、EXEファイル **DocumentViewer.exe** を生成します。
9. クライアント環境にReleaseフォルダごとコピーします。
10. クライアント環境にWebView2 Runtimeをインストールします。ただし、Microsoft　Edge（Chromium）がインストールされている場合は、Runtimeのインストールは不要となります。

# Usage

**※本ソースコードは自己責任で使用してください。**

1. **DocumentViewer.exe**を起動し、患者ID欄に入力してエンターキーを押す、もしくは**読込**ボタンを押す。
2. 一覧からドキュメントを選択すると、右側に内容がプレビュー表示される。
3. プレビューウィンドウ内の各アクションボタンで拡大縮小表示、印刷、保存を行います。
4. **エクスポート**ボタンを押すと、一覧に表示されている全ドキュメントを指定フォルダに保存することができます。
5. **テンポラリフォルダを表示**ボタンを押すと、プレビュー用一時保存フォルダをエクスプローラーで開きます。


**以下の様に患者IDをパラメータに指定してプログラムを起動することもできます。**
  ```
  DocumentViewer.exe 12345678
  ```
 
# Author
 
* Takashi Kodama
 
# License
 
**DocumentViewer** は [MIT ライセンス](https://en.wikipedia.org/wiki/MIT_License) の下にあります。
