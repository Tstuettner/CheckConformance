Imports field_groups.DynaPDF

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
         Dim y As Double, b As Double
         Dim act As Integer, f As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 12.0, False, TCodepage.cp1252)
               pdf.SetLeading(12.0)
               pdf.WriteFTextEx(50.0, 50.0, pdf.GetPageWidth() - 100.0, -1.0, TTextAlign.taJustify, "The six text fields share the same value. " & _
                  "Such an array of fields is called a field group. All fields in the group must be of the same type." + Chr(13) + Chr(13) & _
                  "A field group can be created in two different ways: either create two or more fields with the same name or pass the handle of the y field as Parent to the children. " & _
                  "The latter way is more efficient since it is not required to search for the parent field when a child will be created." + Chr(13) + Chr(13) & _
                  "Enter some more text into a field to see the difference between auto size and fixed font size.")

               ' GetLastTextPosY() returns bottom up coordinates. We must subtract the value from the page height.
               b = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 20.0

               pdf.WriteFTextEx(50.0, b, 200.0, -1.0, TTextAlign.taLeft, "Font size <= 1.0 means auto size.")

               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0

               pdf.ChangeFontSize(1.0)
               f = pdf.CreateTextField("Auto", -1, False, 0, 50.0, y, 200.0, 20.0)
               pdf.SetTextFieldValue(f, "Some text...", "Some text...", TTextAlign.taLeft)

               y += 30.0
               pdf.CreateTextField(Nothing, f, False, 0, 50.0, y, 200.0, 30.0)

               y += 40.0
               pdf.CreateTextField(Nothing, f, False, 0, 50.0, y, 200.0, 40.0)

               pdf.ChangeFontSize(12.0)
               pdf.WriteFTextEx(345.0, b, 200.0, -1.0, TTextAlign.taLeft, "The same fields with a fixed font size.")

               y = pdf.GetPageHeight() - pdf.GetLastTextPosY() + 10.0

               pdf.ChangeFontSize(12.0)
               pdf.CreateTextField(Nothing, f, False, 0, 345.0, y, 200.0, 20.0)

               y += 30.0
               pdf.ChangeFontSize(24.0)
               pdf.CreateTextField(Nothing, f, False, 0, 345.0, y, 200.0, 30.0)

               y += 40.0
               pdf.ChangeFontSize(34.0)
               pdf.CreateTextField(Nothing, f, False, 0, 345.0, y, 200.0, 40.0)

               pdf.ChangeFontSize(18.0)
               f = pdf.CreateButton("Reset", "Reset", -1, 222.5, y + 80.0, 150.0, 25.0)
               pdf.SetFieldColor(f, TFieldColor.fcBackColor, TPDFColorSpace.csDeviceRGB, CPDF.PDF_LTGRAY)
               pdf.SetFieldBorderStyle(f, TBorderStyle.bsBevelled)

               act = pdf.CreateResetAction()
               pdf.AddActionToObj(TObjType.otField, TObjEvent.oeOnMouseUp, act, f)
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
