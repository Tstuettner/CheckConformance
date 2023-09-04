Imports split_pdf.DynaPDF

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
         Dim i As Integer, count As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage) ' Import anything and avoid the conversion of pages to templates
         pdf.SetImportFlags2(TImportFlags2.if2UseProxy)                              ' This flag reduces the memory usage.

         If pdf.OpenImportFile(System.IO.Path.GetFullPath("../../../../license.pdf"), TPwdType.ptOpen, Nothing) < 0 Then
            pdf = Nothing
            Return
         End If

         ' Very important: This property makes sure that the open import file will not be closed when CloseFile() is called.
         pdf.SetUseGlobalImpFiles(True)

         If Not System.IO.Directory.Exists("out") Then
            System.IO.Directory.CreateDirectory("out")
         End If
         count = pdf.GetInPageCount()
         For i = 1 To count
            pdf.CreateNewPDF(String.Format("out/{0:0000}.pdf", i))
               pdf.Append()
                  pdf.ImportPageEx(i, 1.0, 1.0)
               pdf.EndPage()
            pdf.CloseFile()
         Next i
         ' You should never forget to set the property back to false when finish.
         ' In this example it makes no difference but if the PDF instance is used
         ' to create or import other PDF files then make sure that the parser instance
         ' can be deleted.
         pdf.SetUseGlobalImpFiles(False)

         ' Open the output directory
         Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
         p.StartInfo.FileName = System.IO.Path.GetFullPath("out")
         p.Start()

         pdf = Nothing
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
