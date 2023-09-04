Imports check_boxes.DynaPDF

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
         Dim y As Double
         Dim act As Integer, f As Integer, r As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 10.0, False, TCodepage.cp1252)
               pdf.WriteText(50.0, 50.0, "Normal check boxes.")

               pdf.ChangeFontSize(1.0)
               f = pdf.CreateCheckBox("N1", "C1", True, -1, 50.0, 70.0, 20.0, 20.0)
               pdf.SetCheckBoxDefState(f, True)
               f = pdf.CreateCheckBox("N2", "C2", True, -1, 80.0, 70.0, 20.0, 20.0)
               pdf.SetCheckBoxDefState(f, True)
               f = pdf.CreateCheckBox("N3", "C1", True, -1, 110.0, 70.0, 20.0, 20.0)
               pdf.SetCheckBoxDefState(f, True)

               pdf.ChangeFontSize(10.0)
               pdf.WriteText(50.0, 100.0, "Field group with check boxes.")

               pdf.ChangeFontSize(1.0)
               pdf.CreateCheckBox("G1", "C1", False, -1, 50.0, 120.0, 20.0, 20.0)
               pdf.CreateCheckBox("G1", "C2", False, -1, 80.0, 120.0, 20.0, 20.0)
               pdf.CreateCheckBox("G1", "C1", True, -1, 110.0, 120.0, 20.0, 20.0)

               pdf.ChangeFontSize(10.0)
               pdf.WriteFTextEx(50.0, 150.0, 220.0, -1.0, TTextAlign.taLeft, "This group works like a radio button but only radio buttons get a round border if the check box character is set to ccCircle. No problem, set the border width to zero and draw the circle in background if needed.")

               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0

               pdf.ChangeFontSize(1.0)
               pdf.SetCheckBoxChar(TCheckBoxChar.ccCircle)
               pdf.CreateCheckBox("G2", "C1", False, -1, 50.0, y, 20.0, 20.0)
               pdf.CreateCheckBox("G2", "C2", False, -1, 80.0, y, 20.0, 20.0)
               pdf.CreateCheckBox("G2", "C3", True, -1, 110.0, y, 20.0, 20.0)


               pdf.ChangeFontSize(10.0)
               pdf.WriteFTextEx(300.0, 50.0, 250.0, -1.0, TTextAlign.taLeft, "This is a radio button. Since Acrobat 7 it is no longer possible to deselect the active check box, except with a reset form or Javascript action.")

               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0

               pdf.ChangeFontSize(15.0)
               r = pdf.CreateRadioButton("Radio1", "R1", True, -1, 300.0, y, 20.0, 20.0)
               pdf.SetCheckBoxDefState(r, False)
               pdf.CreateCheckBox(Nothing, "R2", False, r, 330.0, y, 20.0, 20.0)
               pdf.CreateCheckBox(Nothing, "R3", False, r, 360.0, y, 20.0, 20.0)

               pdf.ChangeFontSize(10.0)
               f = pdf.CreateButton("Reset", "Reset", -1, 400.0, y, 60.0, 20.0)
               pdf.SetFieldColor(f, TFieldColor.fcBackColor, TPDFColorSpace.csDeviceRGB, CPDF.PDF_LTGRAY)
               pdf.SetFieldBorderStyle(f, TBorderStyle.bsBevelled)

               act = pdf.CreateResetAction()
               pdf.AddActionToObj(TObjType.otField, TObjEvent.oeOnMouseUp, act, f)
               pdf.AddFieldToFormAction(act, r, True)

               y += 40.0
               pdf.ChangeFontSize(10.0)
               pdf.WriteFTextEx(300.0, y, 250.0, -1.0, TTextAlign.taLeft, "The RadioIsUnion flag has only an effect if at least two check boxes use the same export value.")
               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0

               pdf.ChangeFontSize(15.0)
               r = pdf.CreateRadioButton("Radio2", "R1", True, -1, 300.0, y, 20.0, 20.0)
               pdf.SetFieldFlags(r, TFieldFlags.ffRadioIsUnion, False)
               pdf.CreateCheckBox(Nothing, "R2", False, r, 330.0, y, 20.0, 20.0)
               pdf.CreateCheckBox(Nothing, "R1", True, r, 360.0, y, 20.0, 20.0)
               pdf.CreateCheckBox(Nothing, "R2", False, r, 390.0, y, 20.0, 20.0)
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
