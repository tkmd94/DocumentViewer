# DocumentViewer
 
"DocumentViewer" is a standalone application that previews documents stored in the ARIA Document workspace.

# Features

* Display a list of documents stored in the ARIA Document workspace.
* Document preview using the WebView2 control (features include zoom in/out, print, save, etc.)
* Save the document to a specified folder.
* Launch the program by specifying the patient ID as a parameter.
* Supports system locale language settings (English or Japanese).
  

# Demo

![Screen capture of planCompare UI](https://github.com/tkmd94/DocumentViewer/blob/master/demo.gif)

# Requirement

* Oncology Services must be installed on ARIA.
* It is necessary to obtain the Oncology Services API key.
* WebView2 Runtime must be installed on the client environment to run this program.

# Installation
1. Set the hostname of the ARIA server you want to connect to. 
   https://github.com/tkmd94/DocumentViewer/blob/593dea2c146a5eb50a5966442d393c2790157f40/DocumentViewer/App.config#L7
2. Set the port number of the ARIA server you want to connect to. 
   https://github.com/tkmd94/DocumentViewer/blob/593dea2c146a5eb50a5966442d393c2790157f40/DocumentViewer/App.config#L8
3. Configure the API key.  
   https://github.com/tkmd94/DocumentViewer/blob/593dea2c146a5eb50a5966442d393c2790157f40/DocumentViewer/App.config#L9
4. Set the local save destination.  
   https://github.com/tkmd94/DocumentViewer/blob/593dea2c146a5eb50a5966442d393c2790157f40/DocumentViewer/App.config#L10
5. Copy the following DLL from Eclipse to the project folder:
   >C:\Program Files (x86)\Varian\OCS\XX.X\VMS.OIS.ARIALocal.WebServices.Document.Contracts.dll  
   >XX.X represents the version number.
6. Copy the following DLL from Eclipse to the project folder:
   >C:\Program Files (x86)\Varian\OCS\XX.X\VMS.SF.Gateway.Contracts.dll  
   >XX.X represents the version number.
7. Release build this project to generate the EXE file **DocumentViewer.exe**.
8. Copy the entire Release folder to the client environment.
9. Install WebView2 Runtime on the client environment. However, if Microsoft Edge (Chromium) is installed, there's no need to install the Runtime.
    
# Usage

**Please use this source code at your own risk.**

1. Launch **DocumentViewer.exe**, enter the patient ID and press the Enter key, or click the **Load** button.
2. When you select a document from the list, its content will be previewed on the right side.
3. Use the action buttons within the preview window to zoom in/out, print, and save.
4. By pressing the **Export** button, you can save all the documents displayed in the list to a specified folder.
5. Clicking **Open temp. folder** button will open the temporary storage folder for previews in Explorer.

**You can also launch the program by specifying the patient ID as a parameter as follows.**
  ```
  DocumentViewer.exe 12345678
  ```
 
# Author
 
* Takashi Kodama
 
# License
 
**DocumentViewer** is under [MIT license](https://en.wikipedia.org/wiki/MIT_License) 
