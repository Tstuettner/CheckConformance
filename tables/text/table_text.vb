Imports table_text.DynaPDF

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

         Dim i, rowNum As Integer

         Dim tbl As CPDFTable = New CPDFTable(pdf, 3, 3, 500.0, 100.0)
         tbl.SetBoxProperty(-1, -1, TTableBoxProperty.tbpBorderWidth, 1.0, 1.0, 1.0, 1.0)
         tbl.SetFont(-1, -1, "Arial", TFStyle.fsRegular, True, TCodepage.cp1252)
         tbl.SetFont(-1, 1, "Arial", TFStyle.fsBold, True, TCodepage.cp1252)
         tbl.SetGridWidth(1.0, 1.0)

         Dim text As String = "The cell alignment can be set for text, images, and templates..."

         ' -1.0 means use the default row height as specified in the CreateTable() call.
         rowNum = tbl.AddRow(-1.0)
         tbl.SetCellText(rowNum, 0, TTextAlign.taLeft, TCellAlign.coTop, text)
         tbl.SetCellText(rowNum, 1, TTextAlign.taCenter, TCellAlign.coTop, text)
         tbl.SetCellText(rowNum, 2, TTextAlign.taRight, TCellAlign.coTop, text)

         rowNum = tbl.AddRow(-1.0)
         tbl.SetCellText(rowNum, 0, TTextAlign.taLeft, TCellAlign.coCenter, text)
         tbl.SetCellText(rowNum, 1, TTextAlign.taCenter, TCellAlign.coCenter, text)
         tbl.SetCellText(rowNum, 2, TTextAlign.taRight, TCellAlign.coCenter, text)

         rowNum = tbl.AddRow(-1.0)
         tbl.SetCellText(rowNum, 0, TTextAlign.taLeft, TCellAlign.coBottom, text)
         tbl.SetCellText(rowNum, 1, TTextAlign.taCenter, TCellAlign.coBottom, text)
         tbl.SetCellText(rowNum, 2, TTextAlign.taRight, TCellAlign.coBottom, text)

         ' Draw the table now
         pdf.Append()
         tbl.DrawTable(50.0, 50.0, 742.0)
         While tbl.HaveMore()
            pdf.EndPage()
            pdf.Append()
            tbl.DrawTable(50.0, 50.0, 742.0)
         End While
         pdf.EndPage()


         ' Let's change the cell orientation to see what happens...
         tbl.SetCellOrientation(-1, -1, 90)
         pdf.Append()
         pdf.SetFont("Arial", TFStyle.fsRegular, 12.0, True, TCodepage.cp1252)
         pdf.WriteText(50.0, 50.0, "The same table but the cell orientation was changed to 90 degrees.")

         tbl.DrawTable(50.0, 65.0, 742.0)
         While tbl.HaveMore()
            pdf.EndPage()
            pdf.Append()
            tbl.DrawTable(50.0, 50.0, 737.0)
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
