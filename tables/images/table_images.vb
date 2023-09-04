Imports tables_images.DynaPDF

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

         Dim i, n, rowNum As Integer

         pdf.SetPageCoords(TPageCoord.pcTopDown)
         pdf.SetResolution(300)

         Dim tbl As CPDFTable = New CPDFTable(pdf, 100, 4, 500.0F, 125.0F)
         tbl.SetBoxProperty(-1, -1, TTableBoxProperty.tbpBorderWidth, 1.0F, 1.0F, 1.0F, 1.0F)
         tbl.SetBoxProperty(-1, -1, TTableBoxProperty.tbpCellPadding, 5.0F, 5.0F, 5.0F, 5.0F)
         tbl.SetGridWidth(1.0F, 1.0F)
         tbl.SetFlags(-1, -1, TTableFlags.tfUseImageCS)

         Dim files As String() = System.IO.Directory.GetFiles("../../../../test_files/images", "*.jpg")

         rowNum = tbl.AddRow(125.0F)
         n = 0
         For i = 0 To files.Length - 1
            If n = 4 Then
               rowNum = tbl.AddRow(100.0F)
               n = 0
            End If
            tbl.SetCellImage(rowNum, n, True, TCellAlign.coCenter, TCellAlign.coCenter, 0.0F, 0.0F, files(i), 1)
            n += 1
         Next i
         files = Nothing

         pdf.Append()

         tbl.DrawTable(50.0F, 50.0F, 742.0F)
         While tbl.HaveMore()
            pdf.EndPage()
            pdf.Append()
            tbl.DrawTable(50.0F, 50.0F, 742.0F)
         End While
         pdf.EndPage()


         ' We draw the same table again but this time with the flag tfScaleToRect
         tbl.SetFlags(-1, -1, TTableFlags.tfScaleToRect Or TTableFlags.tfUseImageCS)
         pdf.Append()

         pdf.SetFont("Arial", TFStyle.fsRegular, 12.0, True, TCodepage.cp1252)
         pdf.WriteText(50.0, 50.0, "The same table but the flag tfScaleToRect was set.")

         tbl.DrawTable(50.0F, 65.0F, 742.0F)
         While tbl.HaveMore()
            pdf.EndPage()
            pdf.Append()
            tbl.DrawTable(50.0F, 50.0F, 742.0F)
         End While
         pdf.EndPage()

         tbl = Nothing

         ' A table stores errors and warnings in the error log
         Dim err As TPDFError = New TPDFError

         n = pdf.GetErrLogMessageCount()
         For i = 0 To n - 1
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
