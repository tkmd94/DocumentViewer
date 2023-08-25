using System;

namespace DocumentViewer
{
    public class DocumentInfo
    {
        public string PatientID { get; set; }
        public string PatientVisitID { get; set; }
        public string VisitNoteID { get; set; }
        public bool IsMarkedAsError { get; set; }
        public DateTime Date { get; set; }
        public string DocumentType { get; set; }
        public string TemplateName { get; set; }
        public string Format { get; set; }
        public string AuthoredBy { get; set; }
        public string SaveFilePath { get; set; }
        public int Number { get; set; }
        public byte[] ByteData { get; set; }
    }
}
