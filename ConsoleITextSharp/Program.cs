using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32.SafeHandles;

namespace ConsoleITextSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            ManagerPdf.CreatePdf();
        }
    }

    static class ManagerPdf
    {
        static string path = Path.GetFullPath("ocean.pdf");
        static string pathXXX = Path.GetFullPath("xxx.pdf");
        static string target = Path.GetFullPath("Chapter1_Example1.pdf");
        public static void CreatePdf()
        {
            using (FileStream fs = new FileStream("Chapter1_Example1.pdf", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                Document document = new Document(PageSize.A4, 10, 10, 10, 10);

                PdfWriter writer = PdfWriter.GetInstance(document, fs);
                document.Open();

                Phrase phrase = new Phrase("This is from Phrase.");
                document.Add(phrase);

//                //**********************merge
//                Document documentCopy = new Document();
//                PdfCopy copy;
//                if (true)
//                    copy = new PdfSmartCopy(documentCopy, new FileStream(path, FileMode.Open));
//                else
//                    copy = new PdfCopy(documentCopy, new FileStream(path, FileMode.Open));
//                documentCopy.Open();
//
//                PdfReader reader = new PdfReader(pathXXX);
//
//                //reader = new PdfReader(pathXXX);
//                copy.AddDocument(reader);
//                copy.FreeReader(reader);
//                reader.Close();
//                //**********************

                document.Close();
                fs.Close();
            }
//            PdfMerger.MergeFiles(new List { path, pathXXX });

            string[] array = new[] { path, pathXXX };
            PdfMerger.Merge(target, array);

        }

        public static class PdfMerger
        {
            /// <summary>
            /// Merge pdf files.
            /// </summary>
            /// <param name="sourceFiles">PDF files being merged.</param>
            /// <returns></returns>
            public static byte[] MergeFiles(List<byte[]> sourceFiles)
            {
                Document document = new Document();
                using (MemoryStream ms = new MemoryStream())
                {
                    PdfCopy copy = new PdfCopy(document, ms);
                    document.Open();
                    int documentPageCounter = 0;

                    // Iterate through all pdf documents
                    for (int fileCounter = 0; fileCounter < sourceFiles.Count; fileCounter++)
                    {
                        // Create pdf reader
                        PdfReader reader = new PdfReader(sourceFiles[fileCounter]);
                        int numberOfPages = reader.NumberOfPages;

                        // Iterate through all pages
                        for (int currentPageIndex = 1; currentPageIndex <= numberOfPages; currentPageIndex++)
                        {
                            documentPageCounter++;
                            PdfImportedPage importedPage = copy.GetImportedPage(reader, currentPageIndex);
                            PdfCopy.PageStamp pageStamp = copy.CreatePageStamp(importedPage);

                            // Write header
                            ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                                new Phrase("PDF Merger by Helvetic Solutions"), importedPage.Width / 2, importedPage.Height - 30,
                                importedPage.Width < importedPage.Height ? 0 : 1);

                            // Write footer
                            ColumnText.ShowTextAligned(pageStamp.GetOverContent(), Element.ALIGN_CENTER,
                                new Phrase(String.Format("Page {0}", documentPageCounter)), importedPage.Width / 2, 30,
                                importedPage.Width < importedPage.Height ? 0 : 1);

                            pageStamp.AlterContents();

                            copy.AddPage(importedPage);
                        }

                        copy.FreeReader(reader);
                        reader.Close();
                    }

                    document.Close();
                    return ms.GetBuffer();
                }
            }

            /// 
            ///		Combina una serie de archivos PDF
            /// 
            internal static bool Merge(string strFileTarget, string[] arrStrFilesSource)
            {
                bool blnMerged = false;

                // Crea el PDF de salida
                try
                {
                    using (System.IO.FileStream stmFile = new System.IO.FileStream(strFileTarget, System.IO.FileMode.Create))
                    {
                        Document objDocument = null;
                        PdfWriter objWriter = null;

                        // Recorre los archivos
                        for (int intIndexFile = 0; intIndexFile < arrStrFilesSource.Length; intIndexFile++)
                        {
                            PdfReader objReader = new PdfReader(arrStrFilesSource[intIndexFile]);
                            int intNumberOfPages = objReader.NumberOfPages;

                            // La primera vez, inicializa el documento y el escritor
                            if (intIndexFile == 0)
                            { // Asigna el documento y el generador
                                objDocument = new Document(objReader.GetPageSizeWithRotation(1));
                                objWriter = PdfWriter.GetInstance(objDocument, stmFile);
                                // Abre el documento
                                objDocument.Open();
                            }
                            // Añade las páginas
                            for (int intPage = 0; intPage < intNumberOfPages; intPage++)
                            {
                                int intRotation = objReader.GetPageRotation(intPage + 1);
                                PdfImportedPage objPage = objWriter.GetImportedPage(objReader, intPage + 1);

                                // Asigna el tamaño de la página
                                objDocument.SetPageSize(objReader.GetPageSizeWithRotation(intPage + 1));
                                // Crea una nueva página
                                objDocument.NewPage();
                                // Añade la página leída
                                if (intRotation == 90 || intRotation == 270)
                                    objWriter.DirectContent.AddTemplate(objPage, 0, -1f, 1f, 0, 0,
                                        objReader.GetPageSizeWithRotation(intPage + 1).Height);
                                else
                                    objWriter.DirectContent.AddTemplate(objPage, 1f, 0, 0, 1f, 0, 0);
                            }
                        }
                        // Cierra el documento
                        if (objDocument != null)
                            objDocument.Close();
                        // Cierra el stream del archivo
                        stmFile.Close();
                    }
                    // Indica que se ha creado el documento
                    blnMerged = true;
                }
                catch (Exception objException)
                {
                    System.Diagnostics.Debug.WriteLine(objException.Message);
                }
                // Devuelve el valor que indica si se han mezclado los archivos
                return blnMerged;
            }
        }
    }
}
