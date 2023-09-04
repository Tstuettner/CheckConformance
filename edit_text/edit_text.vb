Imports edit_text.DynaPDF

Module Module1

   'Note that an error callback function does not work if the error occurs outside of this module.
   'While C# has no problems with callback functions VB has its own meaning how callback functions
   'can be used. It seems that VB invalidates all pointers if memory must be reallocated...

   'To avoid problems we use the event handling of DynaPDF instead.

   Friend WithEvents m_PDF As CPDF

   ' Error event handler
   ' If the function name should not appear at the beginning of the error message set
   ' the flag emNoFuncNames (m_PDF.SetErrorMode(TErrMode.emNoFuncNames)).
   Private Sub m_PDF_Error(ByVal Description As String, ByVal ErrType As Integer, ByRef DoBreak As Boolean) Handles m_PDF.PDFError
      ' We ignore warnings like "Font not found!" since this case is handled in the function CPDFEditText.SetFont().
      If (ErrType And CPDF.E_WARNING) <> 0 Then Exit Sub
      Console.WriteLine("{0}", Description)
   End Sub

   Sub Main()
      Try
         m_PDF = New CPDF()
         Dim parser As CPDFEditText = New CPDFEditText(m_PDF)

         m_PDF.CreateNewPDF(Nothing) ' We open the output file later if no error occurrs.

         ' We import the contents only, without conversion of pages to templates
         m_PDF.SetImportFlags(TImportFlags.ifContentOnly Or TImportFlags.ifImportAsPage)
         If m_PDF.OpenImportFile("../../../test_files/taxform.pdf", TPwdType.ptOpen, Nothing) < 0 Then
            Console.WriteLine("Input file ""../../../test_files/taxform.pdf"" not found!")
            Console.Read()
            Exit Sub
         End If
         m_PDF.ImportPDFFile(1, 1.0, 1.0)
         m_PDF.CloseImportFile()

         Dim i As Integer, found As Integer
         Dim pageCount As Integer = m_PDF.GetPageCount()
         For i = 1 To pageCount
            If Not m_PDF.EditPage(i) Then Exit For

            'The function FindPattern() searches for a string and stores all records in which it
            'was found in the array CPDFEditText.m_Records. Each record in the array represents
            'exactly one position on the page where the string was found. The coordinates of
            'text records are not stored at this time but it should be no problem to extend
            'the class if necessary.

            'The search run is case-sensitive. If you want to change the text search algorithm
            'you must modify the protected functions CPDFEditText.FindPattern() and
            'CPDFEditText.FindEndPattern().
            found = parser.FindPattern("2002")

            'ReplacePattern() replaces the text with a new one or deletes it if the new string
            'is an empty string. The new text is written in red color so that you can better
            'find it. Take a look into the function CPDFEditText.WriteText() if you want to
            'write the text in the orignal color...
            If found > 0 Then parser.ReplacePattern(Date.Today.Year.ToString())
            Console.WriteLine("Replaced text on page {0} {1} times!\n", i, found)
            m_PDF.EndPage()
         Next i
         If m_PDF.HaveOpenDoc() Then
            ' We write the output file into the current directory.
            Dim filePath As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
            ' OK, now we can open the output file.
            If Not m_PDF.OpenOutputFile(filePath) Then Return
            If m_PDF.CloseFile() Then
               Console.Write("PDF file ""{0}"" successfully created!" + Chr(10), filePath)
               Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
               p.StartInfo.FileName = filePath
               p.Start()
            End If
         End If
      Catch e As Exception
         ' DynaPDF error messages are already handled in the error event function.
         ' We display the error message only if a non-DynaPDF error occurred.
         If e.Message <> "-1" Then
            Console.WriteLine(e.Message)
         End If
      End Try
      Console.Read()
   End Sub

End Module
