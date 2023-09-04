Imports highlight_annotations.DynaPDF

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
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               Dim text As String = "Some text on a page..."
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 20.0, False, TCodepage.cp1252)

               Dim d As Double = pdf.GetDescent()
               Dim w As Double = pdf.GetTextWidth(text)

               pdf.WriteText(50.0, 50.0, text)
               pdf.HighlightAnnot(TAnnotType.atHighlight, 50.0, 50.0 + d, w, 20.0, CPDF.PDF_YELLOW, "Test app", "Highligh Annotations", "This is a highlight annotation")

               pdf.WriteText(50.0, 80.0, text)
               pdf.HighlightAnnot(TAnnotType.atSquiggly, 50.0, 80.0 + d, w, 20.0, CPDF.PDF_RED, "Test app", "Squiggly Annotations", "This is a squiggly annotation")

               pdf.WriteText(50.0, 110.0, text)
               pdf.HighlightAnnot(TAnnotType.atStrikeOut, 50.0, 110.0 + d, w, 20.0, CPDF.PDF_RED, "Test app", "Strikeout Annotations", "This is a strikeout annotation")

               pdf.WriteText(50.0, 140.0, text)
               pdf.HighlightAnnot(TAnnotType.atUnderline, 50.0, 140.0 + d, w, 20.0, CPDF.PDF_RED, "Test app", "Underline Annotations", "This is a underline annotation")
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
