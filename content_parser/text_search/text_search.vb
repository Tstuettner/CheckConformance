Imports text_search.DynaPDF

Module Module1

   Private m_TextSearch As CTextSearch

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Private Function parseBeginTemplate(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Handle As Integer, ByRef BBox As TPDFRect, ByVal Matrix As IntPtr) As Integer
      Return m_TextSearch.BeginTemplate(BBox, Matrix)
   End Function

   Private Sub parseMulMatrix(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByRef M As TCTM)
      m_TextSearch.MulMatrix(M)
   End Sub

   Private Function parseRestoreGraphicState(ByVal Data As IntPtr) As Integer
      m_TextSearch.RestoreGState()
      Return 0
   End Function

   Private Function parseSaveGraphicState(ByVal Data As IntPtr) As Integer
      Return m_TextSearch.SaveGState()
   End Function

   Private Sub parseSetCharSpacing(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextSearch.SetCharSpacing(Value)
   End Sub

   Private Sub parseSetFont(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Type As TFontType, ByVal Embedded As Integer, ByVal FontName As IntPtr, ByVal Style As TFStyle, ByVal FontSize As Double, ByVal IFont As IntPtr)
      m_TextSearch.SetFont(FontSize, Type, IFont)
   End Sub

   Private Sub parseSetTextDrawMode(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Mode As TDrawMode)
      m_TextSearch.SetTextDrawMode(Mode)
   End Sub

   Private Sub parseSetTextScale(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextSearch.SetTextScale(Value)
   End Sub

   Private Sub parseSetWordSpacing(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByVal Value As Double)
      m_TextSearch.SetWordSpacing(Value)
   End Sub

   Private Function parseShowTextArrayA(ByVal Data As IntPtr, ByVal Obj As IntPtr, ByRef Matrix As TCTM, ByVal Source() As TTextRecordA, ByVal Count As Integer, ByVal Width As Double) As Integer
      Try
         Return m_TextSearch.MarkText(Matrix, Source, Count, Width)
      Catch
         Return -1
      End Try
   End Function

   Sub Main()
      Dim selCount As Integer = 0
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The output file is opened later
         ' We avoid the conversion of pages to templates
         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
         If (pdf.OpenImportFile("../../../../../dynapdf_help.pdf", TPwdType.ptOpen, Nothing) < 0) Then
            Console.Write("Input file ""../../../../../dynapdf_help.pdf"" not found!" + Chr(10))
            Console.Read()
            Exit Sub
         End If
         pdf.ImportPDFFile(1, 1.0, 1.0)
         pdf.CloseImportFile()

         ' We flatten markup annotations and form fields so that we can search the text in these objects too.
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
         stack.ShowTextArrayA = AddressOf parseShowTextArrayA

         ' We draw rectangles on the position where the search string was found. To make the text
         ' in background visible we use the blend mode bmMultiply. Adobes Acrobat rasters a page
         ' without anti-aliasing when a blend mode is used. Don't wonder that the rasterizing
         ' quality is worse in comparison to normal PDF files.
         Dim g As TPDFExtGState
         pdf.InitExtGState(g)
         g.BlendMode = TBlendMode.bmMultiply
         Dim gs As Integer = pdf.CreateExtGState(g)

         ' We try to find this string in the PDF file
         m_TextSearch = New CTextSearch(pdf)
         m_TextSearch.SetSearchText("PDF")
         Dim i As Integer
         For i = 1 To pdf.GetPageCount()
            pdf.EditPage(i)

            pdf.SetExtGState(gs)
            pdf.SetFillColor(CPDF.PDF_YELLOW)

            m_TextSearch.Init()
            pdf.ParseContent(stack, TParseFlags.pfNone)
            pdf.EndPage()
            If m_TextSearch.GetSelCount() > 0 Then
               selCount += m_TextSearch.GetSelCount()
               Console.Write("Found string on Page: {0} {1} times!" + Chr(10), i, m_TextSearch.GetSelCount())
            End If
         Next i
         Console.Write(Chr(10) + "Found string in the file {0} times!" + Chr(10), selCount)
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            ' OK, now we can open the output file.
            If Not pdf.OpenOutputFile(filePath) Then Return
            If pdf.CloseFile() Then
               Console.Write(Chr(10) + "PDF file ""{0}"" successfully created!" + Chr(10), filePath)
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
