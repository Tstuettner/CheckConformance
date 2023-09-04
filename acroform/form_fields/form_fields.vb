Imports form_fields.DynaPDF

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
         Dim f As Integer, r As Integer
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' We do not create a PDF file in this example

            pdf.SetPageCoords(TPageCoord.pcTopDown)

            pdf.Append()
               y = 50.0
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 10.0, False, TCodepage.cp1252)
               pdf.WriteText(50.0, y, "Text fields:")

               y += 15.0
               f = pdf.CreateTextField("Text1", -1, False, 0, 50.0, y, 200.0, 20.0)
               pdf.SetTextFieldValue(f, Nothing, "Single line text...", TTextAlign.taLeft)

               y += 30.0
               f = pdf.CreateTextField("Text2", -1, True, 0, 50.0, y, 200.0, 50.0)
               pdf.SetTextFieldValue(f, Nothing, "This field accepts multi-line text. The maximum text length can be restricted if necessary.", TTextAlign.taLeft)

               y += 60.0
               pdf.WriteText(50.0, y, "A password field:")
               y += 15.0
               f = pdf.CreateTextField("Text3", -1, False, 0, 50.0, y, 200.0, 20.0)
               pdf.SetFieldFlags(f, TFieldFlags.ffPassword, False)
               pdf.SetTextFieldValue(f, Nothing, "**********", TTextAlign.taLeft)

               y += 30.0
               pdf.WriteText(50.0, y, "A fixed length field separated into combs:")
               y += 15.0
               f = pdf.CreateTextField("Text4", -1, False, 10, 50.0, y, 200.0, 20.0)
               pdf.SetTextFieldValue(f, Nothing, "Test value", TTextAlign.taLeft)
               pdf.SetFieldFlags(f, TFieldFlags.ffComb, False)


               y = 50.0
               pdf.WriteText(350.0, y, "Choice fields:")
               y += 15.0
               f = pdf.CreateComboBox("Combo1", True, -1, 350.0, y, 200.0, 20.0)
               pdf.AddValToChoiceField(f, Nothing, " Select a value...", True)
               pdf.AddValToChoiceField(f, "Apple", "Apple", False)
               pdf.AddValToChoiceField(f, "Banana", "Banana", False)
               pdf.AddValToChoiceField(f, "Pear", "Pear", False)
               pdf.AddValToChoiceField(f, "Grape", "Grape", False)
               pdf.AddValToChoiceField(f, "Orange", "Orange", False)

               y += 30.0
               f = pdf.CreateListBox("List", True, -1, 350.0, y, 200.0, 50.0)
               pdf.AddValToChoiceField(f, "Apple", "Apple", False)
               pdf.AddValToChoiceField(f, "Banana", "Banana", True)
               pdf.AddValToChoiceField(f, "Pear", "Pear", False)
               pdf.AddValToChoiceField(f, "Grape", "Grape", False)
               pdf.AddValToChoiceField(f, "Orange", "Orange", False)

               y += 60.0
               pdf.WriteText(350.0, y, "Editable combo box:")
               y += 15.0
               f = pdf.CreateComboBox("Combo2", True, -1, 350.0, y, 200.0, 20.0)
               pdf.AddValToChoiceField(f, "Apple", "Apple", False)
               pdf.AddValToChoiceField(f, "Banana", "Banana", False)
               pdf.AddValToChoiceField(f, "Pear", "Pear", False)
               pdf.AddValToChoiceField(f, "Grape", "Grape", False)
               pdf.AddValToChoiceField(f, "Orange", "Orange", False)
               pdf.SetFieldFlags(f, TFieldFlags.ffEdit, False)
               pdf.SetFieldExpValue(f, 1000, "Select or enter a value...", Nothing, True)

               y += 30.0
               pdf.WriteText(350.0, y, "Check boxes / Radio buttons:")

               y += 15.0
               pdf.ChangeFontSize(1.0)
               f = pdf.CreateCheckBox("N1", "C1", True, -1, 350.0, y, 20.0, 20.0)
               f = pdf.CreateCheckBox("N2", "C2", True, -1, 380.0, y, 20.0, 20.0)
               f = pdf.CreateCheckBox("N3", "C1", True, -1, 410.0, y, 20.0, 20.0)

               pdf.CreateCheckBox("G1", "C1", False, -1, 450.0, y, 20.0, 20.0)
               pdf.CreateCheckBox("G1", "C2", False, -1, 480.0, y, 20.0, 20.0)
               pdf.CreateCheckBox("G1", "C1", True, -1, 510.0, y, 20.0, 20.0)
               pdf.CreateCheckBox("G1", "C2", False, -1, 540.0, y, 20.0, 20.0)

               y += 30.0
               pdf.ChangeFontSize(15.0)
               pdf.SetCheckBoxChar(TCheckBoxChar.ccCircle)
               r = pdf.CreateRadioButton("Radio1", "R1", True, -1, 350.0, y, 20.0, 20.0)
               pdf.SetCheckBoxDefState(r, False)
               pdf.CreateCheckBox(Nothing, "R2", False, r, 380.0, y, 20.0, 20.0)
               pdf.CreateCheckBox(Nothing, "R3", False, r, 410.0, y, 20.0, 20.0)

               r = pdf.CreateRadioButton("Radio2", "R1", True, -1, 450.0, y, 20.0, 20.0)
               pdf.SetFieldFlags(r, TFieldFlags.ffRadioIsUnion, False)
               pdf.CreateCheckBox(Nothing, "R2", False, r, 480.0, y, 20.0, 20.0)
               pdf.CreateCheckBox(Nothing, "R1", True, r, 510.0, y, 20.0, 20.0)
               pdf.CreateCheckBox(Nothing, "R2", False, r, 540.0, y, 20.0, 20.0)
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
