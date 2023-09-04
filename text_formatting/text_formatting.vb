Imports text_formatting.DynaPDF

Module Module1
   ' Error callback function.
   ' If the function name should not appear at the beginning of the error message then set
   ' the flag emNoFuncNames (pdf.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
      Console.Write("{0}" + Chr(10), System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
      Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
   End Function

   ' This callback function is executed if the page is full or if a page break tag was found in the text.
   Private Function PageBreakProc(ByVal Data As IntPtr, ByVal LastPosX As Double, ByVal LastPosY As Double, ByVal PageBreak As Integer) As Integer
      m_OutRect.pdf.SetPageCoords(TPageCoord.pcTopDown) ' we use top down coordinates
      m_OutRect.Column = m_OutRect.Column + 1
      ' PageBreak is 1 if the string contains a page break tag (see help file for further information).
      If PageBreak = 0 And m_OutRect.Column < m_OutRect.ColCount Then
         ' Calulate the x-coordinate of the column
         Dim posX As Double
         posX = m_OutRect.PosX + m_OutRect.Column * (m_OutRect.Width + m_OutRect.Distance)
         ' change the output rectangle, do not close the page!
         m_OutRect.pdf.SetTextRect(posX, m_OutRect.PosY, m_OutRect.Width, m_OutRect.Height)
         Return 0 ' we do not change the alignment
      Else
         ' the page is full, close the current one and append a new page
         m_OutRect.pdf.EndPage()
         m_OutRect.pdf.Append()
         m_OutRect.pdf.SetTextRect(m_OutRect.PosX, m_OutRect.PosY, m_OutRect.Width, m_OutRect.Height)
         m_OutRect.Column = 0
         Return 0
      End If
   End Function

   Private Structure TOutRect
      Dim pdf As CPDF         ' Active PDF instance
      Dim PosX As Double      ' Original x-coordinate of first output rectangle
      Dim PosY As Double      ' Original y-coordinate of first output rectangle
      Dim Width As Double     ' Original width of first output rectangle
      Dim Height As Double    ' Original height of first output rectangle
      Dim Distance As Double  ' Space between columns
      Dim Column As Integer   ' Current column
      Dim ColCount As Integer ' Number of colummns
   End Structure

   Private m_OutRect As TOutRect

   Sub Main()
      Try
         Dim pdf As CPDF = New CPDF()
         ' You can either use events or declare a callback function.
         pdf.SetOnErrorProc(AddressOf PDFError)
         pdf.CreateNewPDF(Nothing) ' The ouput file is opened later

         pdf.SetDocInfo(TDocumentInfo.diCreator, "VB .Net test application")
         pdf.SetDocInfo(TDocumentInfo.diTitle, "Multi-column text")
         pdf.SetViewerPreferences(TViewerPreference.vpDisplayDocTitle, TViewPrefAddVal.avNone)

         Dim f As System.IO.FileStream = New System.IO.FileStream("../../../test_files/sample.txt", System.IO.FileMode.Open, System.IO.FileAccess.Read)
         Dim b As System.IO.BinaryReader = New System.IO.BinaryReader(f, System.Text.Encoding.Default)
         Dim buffer As Char()
         ReDim buffer(f.Length - 1)
         b.Read(buffer, 0, f.Length)
         f.Close()

         pdf.SetPageCoords(TPageCoord.pcTopDown)
         pdf.Append()

         ' We us a private data type to store the properties of the output retangle.
         ' The structure is required to calculate the new string position within the
         ' callback function. In other programming languages we would pass this structure as a pointer
         ' to the callback function. However, such a concept does not work with .Net languages...
         m_OutRect.pdf = pdf
         m_OutRect.ColCount = 3    ' Set the number of columns
         m_OutRect.Column = 0      ' Current column
         m_OutRect.Distance = 10.0 ' Distance between two columns
         m_OutRect.PosX = 50.0
         m_OutRect.PosY = 50.0
         m_OutRect.Height = pdf.GetPageHeight() - m_OutRect.PosY * 2.0
         m_OutRect.Width = (pdf.GetPageWidth() - m_OutRect.PosX * 2.0 - (m_OutRect.ColCount - 1) * m_OutRect.Distance) / m_OutRect.ColCount

         ' The callback function is called if either the page is full or if a page break operator is found.
         ' It is also possible to use events instead but a callback is faster and I think easier to use...
         pdf.SetOnPageBreakProc(AddressOf PageBreakProc)
         ' Set the output rectangle first
         pdf.SetTextRect(m_OutRect.PosX, m_OutRect.PosY, m_OutRect.Width, m_OutRect.Height)

         pdf.SetFont("Arial", TFStyle.fsNone, 10.0, True, TCodepage.cp1252)
         pdf.WriteFText(TTextAlign.taJustify, New String(buffer)) ' Now we can print the text
         pdf.EndPage()

         ' No fatal error occurred?
         If pdf.HaveOpenDoc Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
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
         Console.Read()
      End Try
   End Sub

End Module
