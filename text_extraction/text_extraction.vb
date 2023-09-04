Imports text_extraction.DynaPDF

Module Module1
   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         Dim parser As CTextExtraction = New CTextExtraction(pdf)
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

         ' External CMaps should always be loaded when processing text from PDF files.
         ' See the description of GetPageText() for further information.
         pdf.SetCMapDir(System.IO.Path.GetFullPath("../../../../Resource/CMap/"), TLoadCMapFlags.lcmRecursive Or TLoadCMapFlags.lcmDelayed)

         ' We avoid the conversion of pages to templates
         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
         If (pdf.OpenImportFile("../../../../dynapdf_help.pdf", TPwdType.ptOpen, Nothing) < 0) Then
            Console.Write("Input file ""../../../../dynapdf_help.pdf"" not found!" + Chr(10))
            Console.Read()
            Exit Sub
         End If
         pdf.ImportPDFFile(1, 1.0, 1.0)
         pdf.CloseImportFile()

         ' We flatten markup annotations and form fields so that we can extract the text in these objects too.
         pdf.FlattenAnnots(TAnnotFlattenFlags.affMarkupAnnots)
         pdf.FlattenForm()

         ' We write the output file into the current directory.
         Dim outFile As String = System.IO.Directory.GetCurrentDirectory() + "\out.txt"
         parser.Open(outFile)

         Dim i As Integer
         For i = 1 To pdf.GetPageCount()
            pdf.EditPage(i) ' Open the page
            parser.WritePageIdentifier(i)
            parser.ParsePage()
            pdf.EndPage() ' Close the page
         Next i
         parser.Close()
         Console.Write("Text successfully extracted to ""{0}""!" + Chr(10), outFile)
         Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
         p.StartInfo.FileName = outFile
         p.Start()
      Catch e As Exception
         Console.WriteLine(e.Message)
      End Try
      Console.Read()
   End Sub

End Module
