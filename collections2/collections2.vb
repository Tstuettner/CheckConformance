Imports collections2.DynaPDF

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
         pdf.SetDocInfo(TDocumentInfo.diKeywords, "PDF Collections; PDF Packages")
         pdf.SetDocInfo(TDocumentInfo.diTitle, "PDF Collections")
         pdf.SetViewerPreferences(TViewerPreference.vpDisplayDocTitle, TViewPrefAddVal.avNone)

         ' The page of this file is shown when opening the file with an older version of Adobe's Acrobat.
         ' Otherwise, the default document of the collection is opened. Click on "Cover sheet" to view the
         ' contents of this page.
         pdf.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
         If (pdf.OpenImportFile("../../../test_files/collection_en.pdf", TPwdType.ptOpen, Nothing) < 0) Then
            Console.Write("Input file ""../../../test_files/collection_en.pdf"" not found!" + Chr(10))
            Console.Read()
            Exit Sub
         End If
         pdf.ImportPDFFile(1, 1.0, 1.0)
         pdf.CloseImportFile()

         Dim ef As Integer
         pdf.CreateCollection(TColView.civTile)
         ' We add a user defined field Index to the list so that we can sort it in every order we want.
         ef = pdf.CreateCollectionField(TColColumnType.cisCustomNumber, 0, "File index", "Index", False, True)
         pdf.SetColSortField(ef, True)

         pdf.CreateCollectionField(TColColumnType.cisFileName, 1, "File name", Nothing, True, True)
         pdf.CreateCollectionField(TColColumnType.cisSize, 2, "File size", Nothing, True, False)
         pdf.CreateCollectionField(TColColumnType.cisModDate, 3, "Modification date", Nothing, True, False)
         pdf.CreateCollectionField(TColColumnType.cisCreationDate, 4, "Creation date", Nothing, True, False)

         ef = pdf.AttachFile("../../../test_files/taxform.pdf", "A PDF file...", True)
         pdf.SetColDefFile(ef) ' This file is opened when viewing the file with Acrobat 8 or later
         pdf.CreateColItemNumber(ef, "Index", 0.0, Nothing)

         ef = pdf.AttachFile("../../../test_files/fulltest.emf", "An EMF file...", True)
         pdf.CreateColItemNumber(ef, "Index", 1.0, Nothing)

         ef = pdf.AttachFile("../../../test_files/sample.txt", "A text file...", True)
         pdf.CreateColItemNumber(ef, "Index", 2.0, Nothing)

         ' Let's check whether the collection is valid.
         ' We know that the collection is valid in this example, but when editing an imported collection
         ' this function can be very helpful.
         pdf.CheckCollection()

         ' No fatal error occurred?
         If pdf.HaveOpenDoc() Then
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
      End Try
      Console.Read()
   End Sub

End Module
