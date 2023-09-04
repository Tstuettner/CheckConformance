Imports hello_world.DynaPDF

Module Module1

   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.Write("{0}" + Chr(10), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetDocInfo(TDocumentInfo.diCreator, "VB .Net test application")
         pdf.SetDocInfo(TDocumentInfo.diSubject, "How to create a pdf file")
         pdf.SetDocInfo(TDocumentInfo.diTitle, "My first VB .Net output")
         pdf.SetViewerPreferences(TViewerPreference.vpDisplayDocTitle, TViewPrefAddVal.avNone)

         pdf.Append()
         pdf.SetFont("Arial", TFStyle.fsItalic, 30, False, TCodepage.cp1252)
         pdf.WriteText(50.0#, 750.0#, "My first Visual Basic output")
         pdf.SetFont("Arial", TFStyle.fsNone, 12.0#, False, TCodepage.cp1252)
         pdf.SetFillColor(RGB(0, 0, 255))
         pdf.WriteText(50.0#, 680.0#, "Creation date: " + Date.Now.ToString())
         pdf.EndPage()

         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory()
            filePath += "\out.pdf"
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
      End Try
      Console.Read()
   End Sub

End Module
