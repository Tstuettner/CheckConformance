Imports checkconformance.DynaPDF

Module Module1

    Class CConvToPDFA

        Dim m_PDF As CPDF

        Public Sub New()
            MyBase.New()
            m_PDF = New CPDF()
            m_PDF.SetOnErrorProc(AddressOf PDFError)
            ' Set the license key here if you have one
            m_PDF.SetLicenseKey("1491938-07102025-2-1-0-5C72FF0DEE995FFDEE998F459CD29AD7-62958F93CB8445CB7255D881C2E1C092")

            ' Non embedded CID fonts depend usually on the availability of external cmaps.
            ' External cmaps should be loaded if possible.
            m_PDF.SetCMapDir(System.IO.Path.GetFullPath("../../../../../Resource/CMap"), TLoadCMapFlags.lcmDelayed Or TLoadCMapFlags.lcmRecursive)
        End Sub

        Public Function GetPDFInstance() As CPDF
            Return m_PDF
        End Function

        ' Error callback function.
        ' If the function name should not appear at the beginning of the error message then set
        ' the flag emNoFuncNames (pdf.SetErrorMode(CPDF.TErrMode.emNoFuncNames)).
        Private Function PDFError(ByVal Data As IntPtr, ByVal ErrCode As Integer, ByVal ErrMessage As IntPtr, ByVal ErrType As Integer) As Integer
            Console.WriteLine("{0}", System.Runtime.InteropServices.Marshal.PtrToStringAnsi(ErrMessage))
            Return 0 ' We try to continue if an error occurrs. Any other return value breaks processing.
        End Function

        Function FontNotFoundProc(ByVal Data As IntPtr, ByVal PDFFont As IntPtr, ByVal FontName As IntPtr, ByVal Style As TFStyle, ByVal StdFontIndex As Integer, ByVal IsSymbolFont As Integer) As Integer
            If m_PDF.WeightFromStyle(Style) < 500 Then
                Style = Style And &HF
                Style = Style Or TFStyle.fsRegular
            End If
            Return m_PDF.ReplaceFont(PDFFont, "Arial", Style, True)
        End Function

        Function ReplaceICCProfileProc(ByVal Data As IntPtr, ByVal Type As TICCProfileType, ByVal ColorSpace As Integer) As Integer
            ' The most important ICC profiles are available free of charge from Adobe. Just seach for "Adobe icc profiles".
            Select Case Type
                Case TICCProfileType.ictRGB
                    Return m_PDF.ReplaceICCProfile(ColorSpace, "sRGB.icc")
                Case TICCProfileType.ictCMYK
                    Return m_PDF.ReplaceICCProfile(ColorSpace, "ISOcoated_v2_bas.ICC") ' This is just an example CMYK profile that can be delivered with DynaPDF
                Case Else
                    Return m_PDF.ReplaceICCProfile(ColorSpace, "gray.icc")
            End Select
        End Function

        Public Function ConvertFile(ByVal Type As TConformanceType, ByVal InFile As String, ByVal OutFile As String, doCompress As Boolean) As Boolean
            Dim retval As Integer, convFlags As Integer

            m_PDF.CreateNewPDF(Nothing)                         ' The output file will be created later
            m_PDF.SetDocInfo(TDocumentInfo.diProducer, Nothing) ' No need to override the original producer

            Select Case Type
                Case TConformanceType.ctNormalize
                    convFlags = TCheckOptions.coAllowDeviceSpaces ' For normalization it is not required to convert device spaces to ICC based color spaces.
                Case TConformanceType.ctPDFA_1b_2005
                    convFlags = TCheckOptions.coDefault Or TCheckOptions.coFlattenLayers        ' Presentations are not prohibited in PDF/A 1.
                Case TConformanceType.ctPDFA_2b
                    convFlags = TCheckOptions.coDefault Or TCheckOptions.coDeletePresentation
                Case TConformanceType.ctPDFA_3b To TConformanceType.ctFacturX_Extended
                    convFlags = TCheckOptions.coDefault Or TCheckOptions.coDeletePresentation
                    convFlags = convFlags And Not TCheckOptions.coDeleteEmbeddedFiles          ' Embedded files are allowed in PDF/A 3.
            End Select

            ' These flags require some processing time but they are very useful.
            convFlags = convFlags Or TCheckOptions.coCheckImages
            convFlags = convFlags Or TCheckOptions.coRepairDamagedImages

            If Type <> TConformanceType.ctNormalize Then
                ' The flag ifPrepareForPDFA is required. The flag ifImportAsPage makes sure that pages will not be converted to templates.
                m_PDF.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage Or TImportFlags.ifPrepareForPDFA)
                ' The flag if2UseProxy reduces the memory usage. The duplicate check is optional but recommended.
                m_PDF.SetImportFlags2(TImportFlags2.if2UseProxy Or TImportFlags2.if2DuplicateCheck)
            Else
                m_PDF.SetImportFlags(TImportFlags.ifImportAll Or TImportFlags.ifImportAsPage)
                m_PDF.SetImportFlags2(TImportFlags2.if2UseProxy Or TImportFlags2.if2DuplicateCheck Or TImportFlags2.if2Normalize)
            End If
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
            If doCompress Then
                m_PDF.SetCompressionLevel(TCompressionLevel.clDefault)
                m_PDF.SetGStateFlags(TGStateFlags.gfNoObjCompression, True)
            Else
                m_PDF.SetCompressionLevel(TCompressionLevel.clNone)
                m_PDF.SetGStateFlags(TGStateFlags.gfNoObjCompression, False)

            End If


            ' The CMYK profile is just an example profile that can be delivered with DynaPDF.
            ' Note that this code requires the PDF/A Extension for DynaPDF.
            retval = m_PDF.CheckConformance(Type, convFlags, IntPtr.Zero, AddressOf FontNotFoundProc, AddressOf ReplaceICCProfileProc)
            Select Case retval
                Case 1
                    m_PDF.AddOutputIntent("sRGB.icc")
                Case 2
                    m_PDF.AddOutputIntent("ISOcoated_v2_bas.ICC")
                Case 3
                    m_PDF.AddOutputIntent("gray.icc")             ' A gray, RGB, or CMYK profile can be used here 
            End Select
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
        Dim outFile As String
        outFile = "??"
        Try
            Dim c As CConvToPDFA = New CConvToPDFA()
            Dim args As String()
            args = Environment.GetCommandLineArgs
            If args.Length < 5 Then
                ReDim Preserve args(4)
                args(1) = "merged.pdf"
                args(2) = "test.pdf"
                args(3) = "Normal"
                args(4) = "NOCOMPRESS"
            End If

            Dim inFile As String
            Dim pdfValue As String
            Dim compress As String
            Dim doCompress As Boolean


            Dim pdfType As TConformanceType

            inFile = args(1)
            outFile = args(2)
            pdfValue = args(3)
            compress = args(4)
            Select Case compress.ToUpper
                Case "COMPRESS"
                    doCompress = True
                Case Else
                    doCompress = False
            End Select




            Select Case pdfValue.ToUpper
                Case "1B"
                    pdfType = TConformanceType.ctPDFA_1b_2005
                Case "2B"
                    pdfType = TConformanceType.ctPDFA_2b
                Case "NORMAL"
                    pdfType = TConformanceType.ctNormalize
                Case Else
                    pdfType = TConformanceType.ctPDFA_3b
            End Select




            ' One instance should be used to convert as many files as possible to improve processing speed.

            ' -------------------------------------------------- ZUGFeRD invoice creation ---------------------------------------------------------
            ' To create a ZUGFeRD invoice attach the required XML invoice here and set the conversion type to the ZUGFeRD profile that you need.

            ' Example (ZUGFeRD 2.1):
            ' c.GetPDFInstance().AttachFile("c:/invoices/test/factur-x.xml", "ZUGFeRD 2.1 Rechnung", True)
            ' If c.ConvertFile(TConformanceType.ctFacturX_Comfort, "c:/invoices/test/TestInvoice.pdf", outFile) Then
            '    Dim p As System.Diagnostics.Process = New System.Diagnostics.Process()
            '    p.StartInfo.FileName = outFile
            '    p.Start()
            ' End If

            ' The file name of the XML invoice must be factur-x.xml. If the file has another name then rename it or use AttachFileEx() instead.
            ' -------------------------------------------------------------------------------------------------------------------------------------
            If c.ConvertFile(pdfType, inFile, outFile, doCompress) Then
                Console.Write("pdfa written " & outFile)
            Else
                Console.Write("Problem writing pdfa " & outFile)
            End If
            c = Nothing
        Catch e As Exception
            Console.Write("Problem writing pdfa " + e.Message + Chr(10))

        End Try
   End Sub

End Module
