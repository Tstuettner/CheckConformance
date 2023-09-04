Imports softmask.DynaPDF

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
         Dim gs As Integer, grp As Integer, sh As Integer
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
               pdf.WriteText(50.0, 50.0, "Transparency effect with a soft mask.")

               pdf.InsertImageEx(50.0, 80.0, pdf.GetPageWidth() - 100.0, 0.0, "../../../../test_files/images/meadow-110719_640.jpg", 1)

               ' Note that a transparency group that it used as soft mask has no own coordinate system. The esiest way to avoid coordinate
               ' issues is to create the transparency group in the full size of the page or template in which it is used. The real bounding
               ' box can be computed after the transparency group was fully defined.
               grp = pdf.BeginTransparencyGroup(0.0, 0.0, pdf.GetPageWidth(), pdf.GetPageHeight(), True, False, TExtColorSpace.esDeviceGray, -1)
                  pdf.SetColorSpace(TPDFColorSpace.csDeviceGray)
                  sh = pdf.CreateRadialShading(400.0, 230.0, 20.0, 400.0, 230.0, 150.0, 1.0, 255, 0, True, False)
                  pdf.ApplyShading(sh)
                  ' Optional but recommended: Compute the real bounding of the group if it is used as soft mask.
                  Dim bbox As TPDFRect = New TPDFRect()
                  pdf.ComputeBBox(bbox, TCompBBoxFlags.cbfNone)
                  pdf.SetBBox(TPageBoundary.pbMediaBox, bbox.Left, bbox.Bottom, bbox.Right, bbox.Top)
               pdf.EndTemplate()

               g = New TPDFExtGState()
               pdf.InitExtGState(g)
               g.SoftMask = pdf.CreateSoftMask(grp, TSoftMaskType.smtLuminosity, 0)
               gs = pdf.CreateExtGState(g)

               pdf.SetExtGState(gs)
               pdf.InsertImageEx(220.0, 80.0, 500.0, 0.0, "../../../../test_files/images/tree-frog-69813_640.jpg", 1)

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
