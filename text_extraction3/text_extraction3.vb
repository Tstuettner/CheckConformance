Imports text_extraction3.DynaPDF

Module Module1

   Private Sub WritePageIdentifier(ByVal F As System.IO.BinaryWriter, ByVal PageNum As Integer)
      If PageNum > 1 Then
         F.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Chr(13) + Chr(10)))
      End If
      F.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(String.Format("%----------------------- Page {0} -----------------------------" + Chr(13) + Chr(10), PageNum)))
   End Sub

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
         Dim strm As System.IO.FileStream
         Dim file As System.IO.BinaryWriter

         strm = New System.IO.FileStream(outFile, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite)
         file = New System.IO.BinaryWriter(strm, System.Text.Encoding.Unicode)

         ' Write a Little Endian marker to the file (byte order mark)
         file.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(ChrW(&HFEFF)))

         Dim i As Integer
         Dim outText As New String("")
         For i = 1 To pdf.GetPageCount()
            WritePageIdentifier(file, i)
            If pdf.ExtractText(i, TTextExtractionFlags.tefDeleteOverlappingText Or TTextExtractionFlags.tefSortTextXY, Nothing, outText) Then
               file.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(outText))
            End If
         Next i
         file.Close()

         ' Tell the garbage collector that these objects can be released now
         file = Nothing
         strm = Nothing

         Console.Write("Text successfully extracted to ""{0}""!" + Chr(10), outFile)

         Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
         p.StartInfo.FileName = outFile
         p.Start()

         pdf = Nothing
      Catch e As Exception
         Console.WriteLine(e.Message)
      End Try
      Console.Read()
   End Sub

End Module
