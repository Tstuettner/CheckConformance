Imports text_coordinates.DynaPDF

Module Module1

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   ' This class extracts the text from a PDF page.
   Private m_TextCoords As CTextCoordinates

   Private Function parseBeginTemplate(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Handle As Integer, ByRef BBox As TPDFRect, ByVal Matrix As IntPtr) As Integer
      Return m_TextCoords.BeginTemplate(BBox, Matrix)
   End Function

   Private Sub parseMulMatrix(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByRef M As TCTM)
      m_TextCoords.MulMatrix(M)
   End Sub

   Private Function parseRestoreGraphicState(ByVal Data As IntPtr) As Integer
      m_TextCoords.RestoreGState()
      Return 0
   End Function

   Private Function parseSaveGraphicState(ByVal Data As IntPtr) As Integer
      Return m_TextCoords.SaveGState()
   End Function

   Private Sub parseSetCharSpacing(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextCoords.SetCharSpacing(Value)
   End Sub

   Private Sub parseSetFont(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Type As TFontType, ByVal Embedded As Integer, ByVal FontName As IntPtr, ByVal Style As TFStyle, ByVal FontSize As Double, ByVal IFont As IntPtr)
      m_TextCoords.SetFont(FontSize, Type, IFont)
   End Sub

   Private Sub parseSetTextDrawMode(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Mode As TDrawMode)
      m_TextCoords.SetTextDrawMode(Mode)
   End Sub

   Private Sub parseSetTextScale(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextCoords.SetTextScale(Value)
   End Sub

   Private Sub parseSetWordSpacing(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextCoords.SetWordSpacing(Value)
   End Sub

   Private Function parseShowTextArrayW(ByVal Data As IntPtr, ByVal Source() As TTextRecordA, ByRef Matrix As TCTM, ByVal Kerning() As TTextRecordW, ByVal Count As Integer, ByVal Width As Double, ByVal Decoded As Integer) As Integer
      Try
         Return m_TextCoords.MarkCoordinates(Matrix, Source, Kerning, Count, Width, Decoded <> 0)
      Catch
         Return -1
      End Try
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         m_TextCoords = New CTextCoordinates(pdf)

         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The output file is opened later

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

         ' We flatten markup annotations and form fields so that we can extract the text of these objects too.
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

         Dim i As Integer
         For i = 1 To pdf.GetPageCount()
            pdf.EditPage(i)
            m_TextCoords.Init()
            pdf.ParseContent(stack, TParseFlags.pfNone)
            pdf.EndPage()
         Next i
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            ' OK, now we can open the output file.
            If Not pdf.OpenOutputFile(filePath) Then Return
            If pdf.CloseFile() Then
               Console.Write("PDF file ""{0}"" successfully created!" + Chr(10), filePath)
               Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
               p.StartInfo.FileName = filePath
               p.Start()
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
