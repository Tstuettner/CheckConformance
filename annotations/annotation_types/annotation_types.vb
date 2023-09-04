Imports annotation_types.DynaPDF

Module Module1

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(CPDF.TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Private Sub AddHighlightAnnot(ByVal PDF As CPDF, ByVal Type As TAnnotType, ByVal Color As Integer, ByVal x As Double, ByVal y As Double, ByVal Text As String, ByVal Subject As String, ByVal Comment As String)
      Dim w As Double = PDF.GetTextWidth(Text)
      PDF.WriteText(x, y, Text)
      PDF.HighlightAnnot(Type, x, y + PDF.GetDescent(), w, 20.0, Color, "Test app", Subject, Comment)
   End Sub

   Sub Main()
      Try
         Dim y As Double = 50.0
         Dim i As Integer, a As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 20.0, False, TCodepage.cp1252)
               AddHighlightAnnot(pdf, TAnnotType.atHighlight, CPDF.PDF_YELLOW, 50.0, y, "Highlight Annotation", "Highlight Annotations", "This is a highlight annotation")
               AddHighlightAnnot(pdf, TAnnotType.atSquiggly, CPDF.PDF_RED, 300.0, y, "Squiggly Annotation", "Highlight Annotations", "This is a squiggly annotation")
               y += 30.0
               AddHighlightAnnot(pdf, TAnnotType.atStrikeOut, CPDF.PDF_RED, 50.0, y, "Strikeout Annotation", "Highlight Annotations", "This is a strikeout annotation")
               AddHighlightAnnot(pdf, TAnnotType.atUnderline, CPDF.PDF_RED, 300.0, y, "Underline Annotation", "Highlight Annotations", "This is a underline annotation")

               y += 40.0
               pdf.CircleAnnot(50.0, y, 200.0, 100.0, 1.0, CPDF.PDF_CREAM, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Circle Annotations", "This is a circle annotation")
               pdf.SquareAnnot(300.0, y, 200.0, 100.0, 1.0, CPDF.PDF_CREAM, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Square Annotations", "This is a square annotation")

               y += 130.0
               pdf.ChangeFontSize(12.0)
               pdf.WriteFTextEx(50.0, y, pdf.GetPageWidth() - 100.0, -1.0, TTextAlign.taLeft, "The icon color of text and file attachment annotations can be changed if " & _
                               "necessary with SetAnnotColor(). The background color must be set.\n\nText Annotations:")

               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0
               ' The default icon color can be changed if necessary
               pdf.TextAnnot(50.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiComment, False)
               a = pdf.TextAnnot(100.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiHelp, False)
               pdf.SetAnnotColor(a, TAnnotColor.acBackColor, TPDFColorSpace.csDeviceRGB, RGB(200, 20, 30))

               pdf.TextAnnot(150.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiInsert, False)
               a = pdf.TextAnnot(200.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiKey, False)
               pdf.SetAnnotColor(a, TAnnotColor.acBackColor, TPDFColorSpace.csDeviceRGB, RGB(50, 200, 30))
               pdf.TextAnnot(250.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiNewParagraph, False)
               a = pdf.TextAnnot(300.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiNote, False)
               pdf.SetAnnotColor(a, TAnnotColor.acBackColor, TPDFColorSpace.csDeviceRGB, RGB(70, 120, 210))
               pdf.TextAnnot(350.0, y, 200.0, 100.0, "Test app", "This is a text annotation", TAnnotIcon.aiParagraph, False)

               y += 50.0
               pdf.WriteText(50.0, y, "File Attachment Annotations:")

               y += 20.0
               Dim filePath As String = "../../../../test_files/gdi.emf"
               pdf.FileAttachAnnot(50.0, y, TFileAttachIcon.faiGraph, "Test app", "An example attachment", filePath, True)
               pdf.FileAttachAnnot(100.0, y, TFileAttachIcon.faiPaperClip, "Test app", "An example attachment", filePath, True)
               a = pdf.FileAttachAnnot(150.0, y, TFileAttachIcon.faiPushPin, "Test app", "An example attachment", filePath, True)
               pdf.SetAnnotColor(a, TAnnotColor.acBackColor, TPDFColorSpace.csDeviceRGB, RGB(70, 120, 210))
               pdf.FileAttachAnnot(200.0, y, TFileAttachIcon.faiTag, "Test app", "An example attachment", filePath, True)

               y += 60.0
               a = pdf.FreeTextAnnot(50.0, y, 300.0, 80.0, "Test app", "This is a FreeText Annotation.", TTextAlign.taCenter)
               pdf.SetAnnotBorderWidth(a, 3.0)
               pdf.SetAnnotColor(a, TAnnotColor.acBorderColor, TPDFColorSpace.csDeviceRGB, CPDF.PDF_GRAY)

               y += 120.0
               pdf.WriteText(50.0, y, "Line Annotations:")

               y += 30.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leNone, TLineEndStyle.leNone, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leButt, TLineEndStyle.leButt, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leCircle, TLineEndStyle.leCircle, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leClosedArrow, TLineEndStyle.leClosedArrow, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leRClosedArrow, TLineEndStyle.leRClosedArrow, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leDiamond, TLineEndStyle.leDiamond, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leOpenArrow, TLineEndStyle.leOpenArrow, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leROpenArrow, TLineEndStyle.leROpenArrow, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leSlash, TLineEndStyle.leSlash, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")
               y += 20.0
               pdf.LineAnnot(50.0, y, 350.0, y, 1.0, TLineEndStyle.leSquare, TLineEndStyle.leSquare, CPDF.PDF_RED, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "Test app", "Line Annotations", "This is a line annotation")

            pdf.EndPage()

            pdf.Append()

               Dim points() As TFltPoint
               ReDim points(4)
               points(0) = New TFltPoint(50.0F, 200.0F)
               points(1) = New TFltPoint(50.0F, 100.0F)
               points(2) = New TFltPoint(150.0F, 50.0F)
               points(3) = New TFltPoint(250.0F, 100.0F)
               points(4) = New TFltPoint(250.0F, 200.0F)

               pdf.PolyLineAnnot(points, 1.0, TLineEndStyle.leDiamond, TLineEndStyle.leCircle, CPDF.PDF_WHITE, CPDF.PDF_RED, TPDFColorSpace.csDeviceRGB, "Test app", "Polyline Annotations", "This is a polyline annotation")

               For i = 0 To points.Length - 1
                  points(i).x += 250.0F
               Next i

               pdf.PolygonAnnot(points, 1.0, CPDF.PDF_CREAM, CPDF.PDF_RED, TPDFColorSpace.csDeviceRGB, "Test app", "Polygon Annotations", "This is a polygon annotation")

               For i = 0 To points.Length - 1
                  points(i).x -= 250.0F
                  points(i).y += 230.0F
               Next i

               y = 230.0
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 12.0, False, TCodepage.cp1252)
               pdf.WriteFTextEx(50.0, y, pdf.GetPageWidth() - 100.0F, -1.0, TTextAlign.taLeft, "The points of an Ink Annotation will be approximated with bezier curves." & _
                  "The result is a smooth path. A polyline annotation is almost the same annotation type without the approximation with bezier curves.")

               pdf.InkAnnot(points, 1.0, CPDF.PDF_BLUE, TPDFColorSpace.csDeviceRGB, "Test app", "Ink Annotations", "This is an ink annotation")

               y += 230.0
               pdf.WriteText(50.0, y, "Stamp Annotations:")
               y += 20.0
               pdf.WriteFTextEx(50.0, y, pdf.GetPageWidth() - 100.0, -1.0, TTextAlign.taLeft, "The default color of a stamp annotation can be changed with SetAnnotColor(). The border or text color must be set.")

               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0
               pdf.StampAnnot(TRubberStamp.rsApproved, 50.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsAsIs, 220.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsConfidential, 390.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")

               y += 60.0
               pdf.StampAnnot(TRubberStamp.rsDepartmental, 50.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsDraft, 220.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsExperimental, 390.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")

               y += 60.0
               pdf.StampAnnot(TRubberStamp.rsExpired, 50.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               a = pdf.StampAnnot(TRubberStamp.rsFinal, 220.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.SetAnnotColor(a, TAnnotColor.acBorderColor, TPDFColorSpace.csDeviceRGB, RGB(170, 10, 160))
               pdf.StampAnnot(TRubberStamp.rsForComment, 390.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")

               y += 60.0
               pdf.StampAnnot(TRubberStamp.rsForPublicRelease, 50.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsNotApproved, 220.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsNotForPublicRelease, 390.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")

               y += 60.0
               pdf.StampAnnot(TRubberStamp.rsSold, 50.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")
               pdf.StampAnnot(TRubberStamp.rsTopSecret, 220.0, y, 150.0, 50.0, "Test app", "Stamp Annotations", "This is a stamp annotation")

            pdf.EndPage()
         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            filePath = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            If pdf.OpenOutputFile(filePath) Then
               If pdf.CloseFile() Then
                  Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
                  p.StartInfo.FileName = filePath
                  p.Start()
               End If
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
