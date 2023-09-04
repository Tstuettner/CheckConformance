Imports alpha_transparency.DynaPDF

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
         Dim gs As Integer, img As Integer
         Dim g As TPDFExtGState
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We open the output file later if no error occurrs.

            ' Disable color key masking for images
            pdf.SetUseTransparency(False)

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()

               pdf.SetFont("Helvetica", TFStyle.fsRegular, 12.0, False, TCodepage.cp1252)
               pdf.WriteText(50.0, 50.0, "Fill Alpha = 0.5")

               pdf.Rectangle(50.0, 70.0, 110.0, 160.0, TPathFillMode.fmFill)
               pdf.SetFillColor(CPDF.PDF_WHITE)
               pdf.WriteText(55.0, 75.0, "Background")

               g = New TPDFExtGState()
               pdf.InitExtGState(g)
               g.FillAlpha = 0.5F
               gs = pdf.CreateExtGState(g)
               pdf.SetExtGState(gs)

               img = pdf.InsertImageEx(60.0, 84.0, 200.0, 0.0, "../../../../test_files/images/tree-frog-69813_640.jpg", 0)

               ' To restore an extended graphics state, create a second one that restores the changes made before and activate this state.
               g.FillAlpha = 1.0F
               gs = pdf.CreateExtGState(g)
               pdf.SetExtGState(gs)

               pdf.SetFillColor(CPDF.PDF_BLACK)
               pdf.WriteText(340.0, 50.0, "Fill Alpha = 1.0 (default)")
               pdf.Rectangle(340.0, 70.0, 110.0, 160.0, TPathFillMode.fmFill)
               pdf.SetFillColor(CPDF.PDF_WHITE)
               pdf.WriteText(345.0, 75.0, "Background")
               pdf.PlaceImage(img, 350.0, 84.0, 200.0, 0.0)

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
