Imports optimize.DynaPDF

Module Module1

   Class COptimize

      Dim m_PDF As CPDF

      Public Sub New()
         MyBase.New()
         m_PDF = New CPDF()
         m_PDF.SetOnErrorProc(AddressOf PDFError)
         ' Set the license key here if you have one
         ' m_PDF.SetLicenseKey("")

         ' Non embedded CID fonts depend usually on the availability of external cmaps.
         ' So, external cmaps should be loaded if possible.
         m_PDF.SetCMapDir(System.IO.Path.GetFullPath("../../../../Resource/CMap"), TLoadCMapFlags.lcmDelayed Or TLoadCMapFlags.lcmRecursive)
      End Sub

      ' Error callback function.
      ' If the function name should not appear at the beginning of the error message then set
      ' the flag emNoFuncNames (pdf.SetErrorMode(CPDF.TErrMode.emNoFuncNames)).
      Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
         Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
         Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
      End Function

      Public Function Optimize(ByVal InFile As String, ByVal OutFile As String) As Boolean
         Dim retval As Integer

         m_PDF.CreateNewPDF(Nothing)                         ' The output file will be created later
         m_PDF.SetDocInfo(TDocumentInfo.diProducer, Nothing) ' No need to override the original producer

         ' The flag ifImportAsPage makes sure that pages will not be converted to templates.
         ' We don't import a piece info dictionary here since this dictionary contains private data that
         ' is only usable in the application that created the data. InDesign, for example, stores original
         ' images and document files in this dictionary, if external resources were placed on a page.
         m_PDF.SetImportFlags((TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage) And Not TImportFlags.ifPieceInfo)
         ' The flag if2UseProxy reduces the memory usage. The duplicate check is optional but recommended.
         ' The resource name check can be omitted when we optimize a PDF file.
         m_PDF.SetImportFlags2(TImportFlags2.if2UseProxy Or TImportFlags2.if2DuplicateCheck Or TImportFlags2.if2NoResNameCheck)
         retval = m_PDF.OpenImportFile(InFile, TPwdType.ptOpen, Nothing)
         If retval < 0 Then
            If m_PDF.IsWrongPwd(retval) Then
               Console.Write("PDFError File is encrypted!")
            End If
            m_PDF.FreePDF()
            Return False
         End If
         m_PDF.ImportPDFFile(1, 1.0, 1.0)
         m_PDF.CloseImportFile()

         ' It is not allowed to call this function twice for the same document in memory!
         ' In addition, we can either optimize the entire PDF file or nothing.
         retval = m_PDF.Optimize(TOptimizeFlags.ofInMemory Or TOptimizeFlags.ofNewLinkNames Or TOptimizeFlags.ofDeleteInvPaths, Nothing)

         ' No fatal error occurred?
         If m_PDF.HaveOpenDoc() Then
            If Not m_PDF.OpenOutputFile(OutFile) Then
               m_PDF.FreePDF()
               Return False
            End If
            Return m_PDF.CloseFile()
         End If
         Return False
      End Function

   End Class

   Sub Main()
      Try
         Dim c As COptimize = New COptimize()
         Dim outFile As String = System.IO.Directory.GetCurrentDirectory() + "\out.pdf"
         ' One instance should be used to convert as many files as possible to improve processing speed.
         If c.Optimize("../../../../dynapdf_help.pdf", outFile) Then
            Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
            p.StartInfo.FileName = outFile
            p.Start()
         End If
         c = Nothing
      Catch e As Exception
         Console.Write(e.Message + Chr(10))
         Console.Read()
      End Try
   End Sub

End Module
