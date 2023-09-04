Imports quad_points.DynaPDF

Module Module1

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(CPDF.TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim a As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               pdf.SaveGraphicState()
                  pdf.SetGStateFlags(TGStateFlags.gfRealTopDownCoords, False) ' This simplifies the handling a little bit.
                  pdf.RotateCoords(-30, 50.0, 200.0)

                  Dim text As String = "Some rotated text on a page..."
                  pdf.SetFont("Helvetica", TFStyle.fsRegular, 20.0, False, TCodepage.cp1252)

                  Dim d As Single = Convert.ToSingle(pdf.GetDescent())
                  Dim w As Single = Convert.ToSingle(pdf.GetTextWidth(text))

                  ' Highlight annotations do not consider coordinate transformations made on a page.
                  ' To get such annotations rotated we must set the annotation's quad points.
                  ' SetAnnotQuadPoints() considers transformations of the coordinate system.

                  pdf.WriteText(0.0, 0.0, text)
                  a = pdf.HighlightAnnot(TAnnotType.atHighlight, 50.0, 50.0 + d, w, 20.0, CPDF.PDF_YELLOW, "Test app", "Highligh Annotations", "This is a highlight annotation")
                  Dim points() As TFltPoint
                  ReDim points(3)
                  ' Consider the unusual order of the points!
                  points(0).x = 0.0F      ' x1 -> Top left corner
                  points(0).y = d         ' y1 -> Top left corner
                  points(1).x = w         ' x2 -> Top right corner
                  points(1).y = d         ' y2 -> Top right corner
                  points(2).x = 0.0F      ' x3 -> Bottom left corner
                  points(2).y = 20.0F + d ' y3 -> Bottom left corner
                  points(3).x = w         ' x4 -> Bottom right corner
                  points(3).y = 20.0F + d ' y4 -> Bottom right corner
                  pdf.SetAnnotQuadPoints(a, points)

                  pdf.WriteText(0.0, 30.0, text)
                  a = pdf.HighlightAnnot(TAnnotType.atSquiggly, 50.0, 80.0, w, 20.0, CPDF.PDF_RED, "Test app", "Squiggly Annotations", "This is a squiggly annotation")
                  ' Update the y-coordinates
                  points(0).y += 30.0F
                  points(1).y += 30.0F
                  points(2).y += 30.0F
                  points(3).y += 30.0F
                  pdf.SetAnnotQuadPoints(a, points)

                  pdf.WriteText(0.0, 60.0, text)
                  a = pdf.HighlightAnnot(TAnnotType.atStrikeOut, 50.0, 110.0, w, 20.0, CPDF.PDF_RED, "Test app", "Strikeout Annotations", "This is a strikeout annotation")
                  ' Update the y-coordinates
                  points(0).y += 30.0F
                  points(1).y += 30.0F
                  points(2).y += 30.0F
                  points(3).y += 30.0F
                  pdf.SetAnnotQuadPoints(a, points)

                  pdf.WriteText(0.0, 90.0, text)
                  a = pdf.HighlightAnnot(TAnnotType.atUnderline, 50.0, 140.0, w, 20.0, CPDF.PDF_RED, "Test app", "Underline Annotations", "This is a underline annotation")
                  ' Update the y-coordinates
                  points(0).y += 30.0F
                  points(1).y += 30.0F
                  points(2).y += 30.0F
                  points(3).y += 30.0F
                  pdf.SetAnnotQuadPoints(a, points)

                  text = "Link annotations support quad points too"
                  w = Convert.ToSingle(pdf.GetTextWidth(text))
                  pdf.WriteText(0.0, 120.0, text)
                  ' Link annotations support quad points too.
                  a = pdf.WebLink(0.0, 120.0, w, 20, "www.dynaforms.com")
                  pdf.SetAnnotBorderWidth(a, 1.0)
                  pdf.SetAnnotColor(a, TAnnotColor.acBorderColor, TPDFColorSpace.csDeviceRGB, CPDF.PDF_BLUE)
                  points(0).x = 0.0F       ' x1 -> Top left corner
                  points(0).y = 120.0F + d ' y1 -> Top left corner
                  points(1).x = w          ' x2 -> Top right corner
                  points(1).y = 120.0F + d ' y2 -> Top right corner
                  points(2).x = 0.0F       ' x3 -> Bottom left corner
                  points(2).y = 140.0F + d ' y3 -> Bottom left corner
                  points(3).x = w          ' x4 -> Bottom right corner
                  points(3).y = 140.0F + d ' y4 -> Bottom right corner
                  pdf.SetAnnotQuadPoints(a, points)
               pdf.RestoreGraphicState()
            pdf.EndPage()
         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
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
