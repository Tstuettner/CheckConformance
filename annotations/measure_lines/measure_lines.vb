Imports measure_lines.DynaPDF

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
         Dim p As TLineAnnotParms
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               Dim w As Double = 300.0
               Dim h As Double = 100.0
               Dim x As Double = pdf.GetPageWidth() / 2
               Dim y As Double = pdf.GetPageHeight() / 2

               ' We save the graphics state because the coordinate system will be rotated.
               ' After RestoreGraphicState() we have the non-rotated coordinate system back.
               pdf.SaveGraphicState()

                  pdf.SetGStateFlags(TGStateFlags.gfRealTopDownCoords, False) ' This simplifies the handling a little bit.
                  pdf.RotateCoords(-30, x, y)

                  x = -w / 2
                  y = -h / 2

                  pdf.SetFillColor(CPDF.PDF_CREAM)
                  pdf.Rectangle(x, y, w, h, TPathFillMode.fmFillStroke)

                  Dim txt As String = String.Format("{0:0.0}", w)
                  a = pdf.LineAnnot(x, y, x + w, y, 1.0, TLineEndStyle.leClosedArrow, TLineEndStyle.leClosedArrow, CPDF.PDF_BLACK, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "This is a measure line", "Measure Line", txt)

                  p = New TLineAnnotParms()
                  p.Caption = 1     ' The parameter Content of LineAnnot() is used as caption.
                  p.LeaderLineLen = 10.0F
                  p.LeaderLineExtend = 4.0F  ' Try different values to understand what these parameters change.
                  p.LeaderLineOffset = 2.0F
                  pdf.SetLineAnnotParms(a, -1, 0.0, p)

                  txt = String.Format("{0:0.0}", h)
                  a = pdf.LineAnnot(x, y + h, x, y, 1.0, TLineEndStyle.leClosedArrow, TLineEndStyle.leClosedArrow, CPDF.PDF_BLACK, CPDF.PDF_BLACK, TPDFColorSpace.csDeviceRGB, "This is a measure line", "Measure Line", txt)
                  ' The parameters are exactly the same as above
                  pdf.SetLineAnnotParms(a, -1, 0.0, p)

               pdf.RestoreGraphicState()
            pdf.EndPage()
         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            If pdf.OpenOutputFile(filePath) Then
               If pdf.CloseFile() Then
                  Dim pcs As System.Diagnostics.Process = New System.Diagnostics.Process()
                  pcs.StartInfo.FileName = filePath
                  pcs.Start()
               End If
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
