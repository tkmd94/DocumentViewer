using MahApps.Metro.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VMS.OIS.ARIALocal.WebServices.Document.Contracts;

namespace DocumentViewer
{

    public partial class MainWindow : Window
    {
        private async void InitializeAsync()
        {
            System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var cacheFolderPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), assembly.GetName().Name + ".webview2cache");
            var webView2Environment = await CoreWebView2Environment.CreateAsync(null, cacheFolderPath);
            await WebView.EnsureCoreWebView2Async(webView2Environment);
        }
        public MainWindow(string patid)
        {
            InitializeComponent();
            InitializeAsync();
            this.WindowState = WindowState.Maximized;
            exportPath = ConfigurationManager.AppSettings["exportPath"];
            apiKeyDoc = ConfigurationManager.AppSettings["docKey"];
            PatientIdTextBox.Text = patid;
            //LoadList();
        }
        public List<DocumentInfo> documents;
        public string exportPath;
        public string apiKeyDoc;
        public bool LoadList()
        {
            documents = new List<DocumentInfo>();
            string patid = "";
            this.Dispatcher.Invoke((Action)(() =>
            {
                patid = PatientIdTextBox.Text;
            }));
            if (!Directory.Exists(exportPath))
            {
                Directory.CreateDirectory(exportPath);
            }
            string[] deleteFiles = Directory.GetFiles(exportPath, "*");
            foreach (string deletefile in deleteFiles)
            {
                FileInfo file = new FileInfo(deletefile);

                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    file.Attributes = FileAttributes.Normal;
                }
                try
                {
                    file.Delete();
                }
                catch (Exception err)
                {
                    Console.WriteLine("The process failed: {0}", err.Message);
                }
            }

            string request = "{\"__type\":\"GetDocumentsRequest:http://services.varian.com/Patient/Documents\",\"Attributes\":[]"
                            + ",\"PatientId\":{ \"ID1\":\"" + patid + "\"}"
                            + "}";
            string response = CustomInsertDocumentsParameter.SendData(request, true, apiKeyDoc);
            var VisitNoteList = new List<string>();
            int visitnoteloc = response.IndexOf("PtVisitNoteId");
            if (visitnoteloc < 0)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    documentDataGrid.ItemsSource = documents;
                }));
                return false;
            }
            while (visitnoteloc > 0)
            {
                VisitNoteList.Add(response.Substring(visitnoteloc + 15, 2).Replace(",", ""));
                visitnoteloc = response.IndexOf("PtVisitNoteId", visitnoteloc + 1);
            }

            //MessageBox.Show(response);
            var response_Doc = JsonConvert.DeserializeObject<DocumentsResponse>(response);
            if (response_Doc.Documents.Count() == 0)
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    documentDataGrid.ItemsSource = documents;
                }));
                return false;
            }

            int loopnum = 0;
            foreach (var document in response_Doc.Documents)
            {
                string thePtId = document.PtId;
                string thePtVisitId = document.PtVisitId.ToString();
                string theVisitNoteId = VisitNoteList[loopnum];
                loopnum++;

                string request_docdetails = "{\"__type\":\"GetDocumentRequest:http://services.varian.com/Patient/Documents\",\"Attributes\":[]"
                                            + ",\"PatientId\":{ \"PtId\":\"" + thePtId + "\"}"
                                            + ",\"PatientVisitId\":" + thePtVisitId
                                            + ",\"VisitNoteId\":" + theVisitNoteId
                                            + "}";
                string response_docdetails = CustomInsertDocumentsParameter.SendData(request_docdetails, true, apiKeyDoc);
                var docdetails = JsonConvert.DeserializeObject<DocumentDetailResponse>(response_docdetails);
                if (docdetails.DocumentDetails != null)
                {
                    bool isMarkedAsError = docdetails.DocumentDetails.IsMarkedAsError;
                    if (!isMarkedAsError)
                    {
                        string PatientName = docdetails.DocumentDetails.PatientLastName + docdetails.DocumentDetails.PatientFirstName;
                        this.Dispatcher.Invoke((Action)(() =>
                        {
                            PatientNameTextBlock.Text = PatientName;
                        }));
                        DateTime dateOfService = docdetails.DocumentDetails.DateOfService;
                        string documentType = docdetails.DocumentDetails.DocumentType;
                        string templateName = docdetails.DocumentDetails.TemplateName;
                        if (templateName == null)
                        {
                            templateName = "No name";
                        }
                        else
                        {
                            templateName = templateName.Replace(':', '_').Replace('/', '_');
                        }
                        string fileFortmat = docdetails.DocumentDetails.FileFormat;
                        string authoredBy = docdetails.DocumentDetails.AuthoredBy;
                        string saveFilePath = patid + "_" + templateName + "(" + documentType + ")";
                        var binaryContent = docdetails.DocumentDetails.BinaryContent;
                        var byteData = Convert.FromBase64String(binaryContent);

                        documents.Add(
                            new DocumentInfo
                            {
                                PatientID = thePtId,
                                PatientVisitID = thePtVisitId,
                                VisitNoteID = theVisitNoteId,
                                IsMarkedAsError = isMarkedAsError,
                                Date = dateOfService,
                                DocumentType = documentType,
                                TemplateName = templateName,
                                Format = fileFortmat,
                                AuthoredBy = authoredBy,
                                SaveFilePath = saveFilePath,
                                Number = 0,
                                ByteData = byteData
                            });
                    }

                }
            }
            for (int i = 1; i < documents.Count(); i++)
            {
                string new_saveFilePath = documents[i].SaveFilePath;
                for (int j = 0; j < i; j++)
                {
                    if (documents[i].SaveFilePath == documents[j].SaveFilePath)
                    {
                        documents[i].Number++;
                    }
                }
            }
            for (int i = 1; i < documents.Count(); i++)
            {
                if (documents[i].Number > 0)
                {
                    documents[i].SaveFilePath = $"{documents[i].SaveFilePath} ({documents[i].Number + 1})";
                }

            }
            var sorted = documents.OrderByDescending(x => x.Date);
            documents = sorted.ToList();
            this.Dispatcher.Invoke((Action)(() =>
            {
                documentDataGrid.ItemsSource = documents;
            }));
            return true;

        }
        private async void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            if (PatientIdTextBox.Text != "")
            {
                ProcessRingExecute();
                bool result = await Task.Run(() => LoadList());
                ProcessRingExecute();
            }
        }
        public void ProcessRingExecute()
        {
            if (ProgressRing.IsActive == true)
            {
                ProgressRing.IsActive = false;
                documentDataGrid.IsEnabled = true;
                LoadButton.IsEnabled = true;
                ExportButton.IsEnabled = true;
                OpenTempButton.IsEnabled = true;
                PatientIdTextBox.IsEnabled = true;
            }
            else
            {
                ProgressRing.IsActive = true;
                documentDataGrid.IsEnabled = false;
                LoadButton.IsEnabled = false;
                ExportButton.IsEnabled = false;
                OpenTempButton.IsEnabled = false;
                PatientIdTextBox.IsEnabled = false;
            }
        }

        private void documentDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DataGrid dgc = sender as DataGrid;
            if (dgc == null)
            {
                return;
            }
            int RowCnt = dgc.Items.IndexOf(dgc.SelectedItem);
            if (RowCnt >= 0)
            {
                string exportAbsPath = exportPath + @"\" + documents[RowCnt].SaveFilePath + "." + documents[RowCnt].Format;
                FileInfo file = new FileInfo(exportAbsPath);
                if (!file.Exists)
                {
                    File.WriteAllBytes(exportAbsPath, documents[RowCnt].ByteData);
                }

                System.Diagnostics.Process.Start(exportAbsPath);

            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            string[] deleteFiles = Directory.GetFiles(exportPath, "*");
            foreach (string deletefile in deleteFiles)
            {
                FileInfo file = new FileInfo(deletefile);

                if ((file.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                {
                    file.Attributes = FileAttributes.Normal;
                }
                try
                {
                    file.Delete();
                }
                catch (Exception err)
                {
                    Console.WriteLine("The process failed: {0}", err.Message);
                }
            }
        }

        private void ExportButton_Click(object sender, RoutedEventArgs e)
        {
            string newExportPath = "";
            var dlg = new CommonOpenFileDialog();
            dlg.IsFolderPicker = true;
            dlg.InitialDirectory = exportPath;
            dlg.DefaultDirectory = @"C:\Users\vms\Desktop";

            if (dlg.ShowDialog() == CommonFileDialogResult.Ok)
            {
                newExportPath = dlg.FileName;
            }
            if (newExportPath != "")
            {
                foreach (var doc in documents)
                {
                    string exportAbsPath = newExportPath + @"\" + doc.SaveFilePath + "." + doc.Format;
                    FileInfo file = new FileInfo(exportAbsPath);
                    if (!file.Exists)
                    {
                        File.WriteAllBytes(exportAbsPath, doc.ByteData);
                    }

                }
                System.Diagnostics.Process.Start(newExportPath);
            }
        }

        private void OpenTempButton_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(exportPath);
        }

        private void DocumentDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            DataGrid dgc = sender as DataGrid;
            if (dgc == null)
            {
                return;
            }
            int RowCnt = dgc.Items.IndexOf(dgc.SelectedItem);
            if (RowCnt >= 0)
            {
                string exportAbsPath = exportPath + @"\" + documents[RowCnt].SaveFilePath + "." + documents[RowCnt].Format;
                FileInfo file = new FileInfo(exportAbsPath);
                if (!file.Exists)
                {
                    File.WriteAllBytes(exportAbsPath, documents[RowCnt].ByteData);
                }

                this.WebView.Source = new Uri(exportAbsPath);
            }
        }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {
                WebView.Width = SystemParameters.PrimaryScreenWidth - documentDataGrid.ActualWidth - 50;
                WebView.Height = SystemParameters.PrimaryScreenHeight - 170;
            }
            else
            {
                WebView.Width = mainWindow.ActualWidth - documentDataGrid.ActualWidth - 50;
                WebView.Height = mainWindow.ActualHeight - 170;
            }
        }

        private void GridSplitter_DragCompleted(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {
            DataGrid dgc = documentDataGrid;
            if (dgc == null)
            {
                return;
            }
            int RowCnt = dgc.Items.IndexOf(dgc.SelectedItem);
            if (RowCnt >= 0)
            {
                string exportAbsPath = exportPath + @"\" + documents[RowCnt].SaveFilePath + "." + documents[RowCnt].Format;
                FileInfo file = new FileInfo(exportAbsPath);
                if (!file.Exists)
                {
                    File.WriteAllBytes(exportAbsPath, documents[RowCnt].ByteData);
                }
                WebView.Width = mainWindow.ActualWidth - documentDataGrid.ActualWidth - 50;
                WebView.Height = mainWindow.ActualHeight - 170;
                this.WebView.Source = new Uri(exportAbsPath);
            }
        }

        private async void PatientIdTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                if (PatientIdTextBox.Text != "")
                {
                    ProcessRingExecute();
                    bool result = await Task.Run(() => LoadList());
                    ProcessRingExecute();
                }
            }
        }
    }
}
