Imports migration_states.DynaPDF

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
         Dim annot As Integer, reply As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               ' To see the migration state, right click on the annotation and then on Review History.
               ' You need Reader X or higher to see the result. Acrobat or Reader 9 use another state model that is no longer supported.
               annot = pdf.SquareAnnot(50.0, 50.0, 200.0, 100.0, 3.0, CPDF.NO_COLOR, 255, TPDFColorSpace.csDeviceRGB, "Jim", "Test", "Just test...")
               reply = pdf.SetAnnotMigrationState(annot, TAnnotState.asCompleted, "Harry")
               pdf.SetAnnotString(reply, TAnnotString.asContent, "The state was set to Completed!")

               reply = pdf.SetAnnotMigrationState(reply, TAnnotState.asAccepted, "Jim")
               pdf.SetAnnotString(reply, TAnnotString.asContent, "The state was set to Accepted!")
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
