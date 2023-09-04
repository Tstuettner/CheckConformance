Imports stamps.DynaPDF

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
               ' A pre-defined stamp is scaled to the given width. The language can be set right before creating the stamp.
               a = pdf.StampAnnot(TRubberStamp.rsApproved, 135.0, 50.0, 300.0, 10.0, "Test app", "Stamp Annotations", "The default language is English!")
               pdf.SetAnnotColor(a, TAnnotColor.acBorderColor, TPDFColorSpace.csDeviceRGB, RGB(120, 190, 92))

               pdf.SetLanguage("DE")
               a = pdf.StampAnnot(TRubberStamp.rsApproved, 135.0, 150.0, 300.0, 10.0, "Test app", "Stamp Annotations", "The same stamp in German!")
               pdf.SetAnnotColor(a, TAnnotColor.acBorderColor, TPDFColorSpace.csDeviceRGB, RGB(230, 65, 132))

               pdf.SetLanguage("FR")
               a = pdf.StampAnnot(TRubberStamp.rsApproved, 135.0, 250.0, 300.0, 10.0, "Test app", "Stamp Annotations", "The same stamp in French!")
               pdf.SetAnnotColor(a, TAnnotColor.acBorderColor, TPDFColorSpace.csDeviceRGB, RGB(78, 157, 232))
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
