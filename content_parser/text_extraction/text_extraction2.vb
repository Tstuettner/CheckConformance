Imports text_extraction2.DynaPDF

Module Module1

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   ' This class extracts the text from a PDF page.
   Private m_TextFile As CPDFToText

   Private Function parseBeginTemplate(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Handle As Integer, ByRef BBox As TPDFRect, ByVal Matrix As IntPtr) As Integer
      Return m_TextFile.BeginTemplate(BBox, Matrix)
   End Function

   Private Sub parseMulMatrix(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByRef M As TCTM)
      m_TextFile.MulMatrix(M)
   End Sub

   Private Function parseRestoreGraphicState(ByVal Data As IntPtr) As Integer
      m_TextFile.RestoreGState()
      Return 0
   End Function

   Private Function parseSaveGraphicState(ByVal Data As IntPtr) As Integer
      Return m_TextFile.SaveGState()
   End Function

   Private Sub parseSetCharSpacing(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextFile.SetCharSpacing(Value)
   End Sub

   Private Sub parseSetFont(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Type As TFontType, ByVal Embedded As Integer, ByVal FontName As IntPtr, ByVal Style As TFStyle, ByVal FontSize As Double, ByVal IFont As IntPtr)
      m_TextFile.SetFont(FontSize, Type, IFont)
   End Sub

   Private Sub parseSetTextDrawMode(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Mode As TDrawMode)
      m_TextFile.SetTextDrawMode(Mode)
   End Sub

   Private Sub parseSetTextScale(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextFile.SetTextScale(Value)
   End Sub

   Private Sub parseSetWordSpacing(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextFile.SetWordSpacing(Value)
   End Sub

   Private Function parseShowTextArrayW(ByVal Data As IntPtr, ByVal Source() As TTextRecordA, ByRef Matrix As TCTM, ByVal Kerning() As TTextRecordW, ByVal Count As Integer, ByVal Width As Double, ByVal Decoded As Integer) As Integer
      Try
         Return m_TextFile.AddText(Matrix, Source, Kerning, Count, Width, Decoded <> 0)
      Catch
         Return -1
      End Try
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         m_TextFile = New CPDFToText(pdf)

         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

         ' External CMaps should always be loaded when processing text from PDF files.
         ' See the description of ParseContent() for further information.
         pdf.SetCMapDir("../../../../../Resource/CMap/", TLoadCMapFlags.lcmRecursive Or TLoadCMapFlags.lcmDelayed)

         ' We avoid the conversion of pages to templates
         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
         If (pdf.OpenImportFile("../../../../../dynapdf_help.pdf", TPwdType.ptOpen, Nothing) < 0) Then
            Console.Write("Input file ""../../../../../dynapdf_help.pdf"" not found!" + Chr(10))
            Console.Read()
            Exit Sub
         End If
         pdf.ImportPDFFile(1, 1.0, 1.0)
         pdf.CloseImportFile()

         ' We flatten markup annotations and form fields so that we can extract the text in these objects too.
         pdf.FlattenAnnots(TAnnotFlattenFlags.affMarkupAnnots)
         pdf.FlattenForm()

         Dim stack As TPDFParseInterface = New TPDFParseInterface
         stack.BeginTemplate = AddressOf parseBeginTemplate
         stack.MulMatrix = AddressOf parseMulMatrix
         stack.RestoreGraphicState = AddressOf parseRestoreGraphicState
         stack.SaveGraphicState = AddressOf parseSaveGraphicState
         stack.SetCharSpacing = AddressOf parseSetCharSpacing
         stack.SetFont = AddressOf parseSetFont
         stack.SetTextDrawMode = AddressOf parseSetTextDrawMode
         stack.SetTextScale = AddressOf parseSetTextScale
         stack.SetWordSpacing = AddressOf parseSetWordSpacing
         stack.ShowTextArrayW = AddressOf parseShowTextArrayW

         ' We write the output file into the current directory.
         Dim outFile As String = System.IO.Directory.GetCurrentDirectory() + "\out.txt"
         m_TextFile.Open(outFile)

         Dim i As Integer
         For i = 1 To pdf.GetPageCount()
            pdf.EditPage(i)
            m_TextFile.Init()
            m_TextFile.WritePageIdentifier(i)
            ' The flag pfTranslateStrings is highly recommended to get human readable strings!
            pdf.ParseContent(stack, TParseFlags.pfNone)
            pdf.EndPage()
         Next i
         m_TextFile.Close()
         Console.Write("Text successfully extracted to ""{0}""!" + Chr(10), outFile)
         Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
         p.StartInfo.FileName = outFile
         p.Start()
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
      End Try
      Console.Read()
   End Sub

End Module
