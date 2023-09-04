Imports table_templates.DynaPDF

Module Module1
   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.Write("{0}" + Chr(10), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   Sub Main()
      Dim timeStart As Long = Date.Now.Ticks
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetPageCoords(TPageCoord.pcTopDown)

         pdf.SetImportFlags2(TImportFlags2.if2UseProxy) ' Reduce the memory usage
         If pdf.OpenImportFile("../../../../../dynapdf_help.pdf", TPwdType.ptOpen, Nothing) < 0 Then
            Console.Read()
            Exit Sub
         End If

         Dim i, rowNum, tmpl, pageCount As Integer
         pageCount = pdf.GetInPageCount()

         Dim tbl As CPDFTable = New CPDFTable(pdf, pageCount / 4 + 1, 2, 512.12, 0.0)
         tbl.SetBoxProperty(-1, -1, TTableBoxProperty.tbpBorderWidth, 1.0, 1.0, 1.0, 1.0)
         tbl.SetBoxProperty(-1, -1, TTableBoxProperty.tbpCellPadding, 5.0, 5.0, 5.0, 5.0)
         tbl.SetGridWidth(1.0, 1.0)
         tbl.SetFlags(-1, -1, TTableFlags.tfScaleToRect)

         pdf.SetPageFormat(TPageFormat.pfUS_Letter)

         rowNum = 0
         For i = 1 To pageCount
            tmpl = pdf.ImportPage(i)
            If (i And 1) <> 0 Then rowNum = tbl.AddRow(335.0)
            tbl.SetCellTemplate(rowNum, (i - 1) And 1, True, TCellAlign.coCenter, TCellAlign.coCenter, tmpl, 0.0, 0.0)
         Next i

         ' Draw the table now
         pdf.Append()
         tbl.DrawTable(50.0, 50.0, 742.0)
         While tbl.HaveMore
            pdf.EndPage()
            pdf.Append()
            tbl.DrawTable(50.0, 50.0, 742.0)
         End While
         pdf.EndPage()

         tbl = Nothing

         ' A table stores errors and warnings in the error log
         Dim err As TPDFError = New TPDFError
         For i = 0 To pdf.GetErrLogMessageCount() - 1
            pdf.GetErrLogMessage(i, err)
            Console.Write(err.Message + Chr(10))
         Next i

         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            ' OK, now we can open the output file.
            If Not pdf.OpenOutputFile(filePath) Then Return
            If pdf.CloseFile() Then
               timeStart = Date.Now.Ticks - timeStart
               Console.Write("Processing time: {0:N0} ms" + Chr(10), timeStart / 10000)
               Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
               p.StartInfo.FileName = filePath
               p.Start()
            End If
         End If
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
