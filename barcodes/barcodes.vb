Imports barcodes.DynaPDF

Module Module1

   Private Structure TTestBarcode
      Dim BarcodeType As DynaPDF.TPDFBarcodeType
      Dim BarcodeName As String
      Dim DataType As DynaPDF.TPDFBarcodeDataType
      Dim Data As String
      Dim Primary As String

      Public Sub New(ByVal BcdType As DynaPDF.TPDFBarcodeType, ByVal Name As String, ByVal DtaType As DynaPDF.TPDFBarcodeDataType, ByVal Dta As String, ByVal Prmy As String)
         BarcodeType = BcdType
         BarcodeName = Name
         DataType = DtaType
         Data = Dta
         Primary = Prmy
      End Sub
   End Structure

   Dim TEST_CODES() As TTestBarcode = _
   { _
      New TTestBarcode(TPDFBarcodeType.bctAustraliaPost, "Australia Post", TPDFBarcodeDataType.bcdtBinary, "12345678", ""), _
      New TTestBarcode(TPDFBarcodeType.bctAustraliaRedir, "Australia Redirect Code", TPDFBarcodeDataType.bcdtBinary, "12345678", ""), _
      New TTestBarcode(TPDFBarcodeType.bctAustraliaReply, "Australia Reply-Paid", TPDFBarcodeDataType.bcdtBinary, "12345678", ""), _
      New TTestBarcode(TPDFBarcodeType.bctAustraliaRout, "Australia Routing Code", TPDFBarcodeDataType.bcdtBinary, "12345678", ""), _
      New TTestBarcode(TPDFBarcodeType.bctAztec, "Aztec Unicode mode", TPDFBarcodeDataType.bcdtBinary, "123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctAztec, "Aztec GS1 Mode", TPDFBarcodeDataType.bcdtGS1Mode, "[01]03453120000011[17]120508[10]ABCD1234[410]9501101020917", ""), _
      New TTestBarcode(TPDFBarcodeType.bctAztecRunes, "Aztec Runes", TPDFBarcodeDataType.bcdtBinary, "123", ""), _
      New TTestBarcode(TPDFBarcodeType.bctC2Of5IATA, "Code 2 of 5 IATA", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctC2Of5Industrial, "Code 2 of 5 Industrial", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctC2Of5Interleaved, "Code 2 of 5 Interleaved", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctC2Of5Logic, "Code 2 of 5 Data Logic", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctC2Of5Matrix, "Code 2 of 5 Matrix", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctChannelCode, "Channel Code", TPDFBarcodeDataType.bcdtBinary, "1234567", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCodabar, "Codabar", TPDFBarcodeDataType.bcdtBinary, "A123456789B", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCodablockF, "Codablock-F", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdefghijklmnopqrstuvwxyz", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode11, "Code 11", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode128, "Code 128", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode128B, "Code 128", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode16K, "Code 16K Unicode mode", TPDFBarcodeDataType.bcdtBinary, "[90]A1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode16K, "Code 16K GS1 mode", TPDFBarcodeDataType.bcdtGS1Mode, "[90]A1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode32, "Code 32", TPDFBarcodeDataType.bcdtBinary, "12345678", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode39, "Code 39", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode49, "Code 49", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCode93, "Code 93", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctCodeOne, "Code One", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDAFT, "DAFT Code", TPDFBarcodeDataType.bcdtBinary, "aftdaftdftaft", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataBarOmniTrunc, "GS1 DataBar Omnidirectional", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataBarExpStacked, "GS1 DataBar Stacked", TPDFBarcodeDataType.bcdtBinary, "[90]1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataBarExpanded, "GS1 DataBar Expanded", TPDFBarcodeDataType.bcdtBinary, "[90]1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataBarLimited, "GS1 DataBar Limited", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataBarStacked, "GS1 DataBar Stacked", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataBarStackedO, "GS1 DataBar Stacked Omni", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDataMatrix, "Data Matrix ISO 16022", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDotCode, "DotCode", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDPD, "DPD Code", TPDFBarcodeDataType.bcdtBinary, "1234567890123456789012345678", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDPIdentCode, "Deutsche Post Identcode", TPDFBarcodeDataType.bcdtBinary, "12345678901", ""), _
      New TTestBarcode(TPDFBarcodeType.bctDPLeitcode, "Deutsche Post Leitcode", TPDFBarcodeDataType.bcdtBinary, "1234567890123", ""), _
      New TTestBarcode(TPDFBarcodeType.bctEAN128, "EAN 128", TPDFBarcodeDataType.bcdtBinary, "[90]0101234567890128TEC-IT", ""), _
      New TTestBarcode(TPDFBarcodeType.bctEAN128_CC, "EAN 128 Composite Code", TPDFBarcodeDataType.bcdtBinary, "[10]1234-1234", "[90]123456"), _
      New TTestBarcode(TPDFBarcodeType.bctEAN14, "EAN 14", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctEANX, "EAN X", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctEANX_CC, "EAN Composite Symbol", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "12345678"), _
      New TTestBarcode(TPDFBarcodeType.bctEANXCheck, "EAN + Check Digit", TPDFBarcodeDataType.bcdtBinary, "12345", ""), _
      New TTestBarcode(TPDFBarcodeType.bctExtCode39, "Ext. Code 3 of 9 (Code 39+)", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctFIM, "FIM", TPDFBarcodeDataType.bcdtBinary, "d", ""), _
      New TTestBarcode(TPDFBarcodeType.bctFlattermarken, "Flattermarken", TPDFBarcodeDataType.bcdtBinary, "11111111111111", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_Aztec, "HIBC Aztec Code", TPDFBarcodeDataType.bcdtBinary, "123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_CodablockF, "HIBC Codablock-F", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdefghijklmnopqrstuvwxyz", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_Code128, "HIBC Code 128", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_Code39, "HIBC Code 39", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_DataMatrix, "HIBC Data Matrix", TPDFBarcodeDataType.bcdtBinary, "0123456789012", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_MicroPDF417, "HIBC Micro PDF417", TPDFBarcodeDataType.bcdtBinary, "01234567890abcde", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_PDF417, "HIBC PDF417", TPDFBarcodeDataType.bcdtBinary, "01234567890abcde", ""), _
      New TTestBarcode(TPDFBarcodeType.bctHIBC_QR, "HIBC QR Code", TPDFBarcodeDataType.bcdtBinary, "01234567890abcde", ""), _
      New TTestBarcode(TPDFBarcodeType.bctISBNX, "ISBN (EAN-13 with validation)", TPDFBarcodeDataType.bcdtBinary, "0123456789", ""), _
      New TTestBarcode(TPDFBarcodeType.bctITF14, "ITF-14", TPDFBarcodeDataType.bcdtBinary, "0123456789", ""), _
      New TTestBarcode(TPDFBarcodeType.bctJapanPost, "Japanese Postal Code", TPDFBarcodeDataType.bcdtBinary, "0123456789", ""), _
      New TTestBarcode(TPDFBarcodeType.bctKIX, "Dutch Post KIX Code", TPDFBarcodeDataType.bcdtBinary, "0123456789", ""), _
      New TTestBarcode(TPDFBarcodeType.bctKoreaPost, "Korea Post", TPDFBarcodeDataType.bcdtBinary, "123456", ""), _
      New TTestBarcode(TPDFBarcodeType.bctLOGMARS, "LOGMARS", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctMailmark, "Royal Mail 4-State Mailmark", TPDFBarcodeDataType.bcdtBinary, "11210012341234567AB19XY1A", ""), _
      New TTestBarcode(TPDFBarcodeType.bctMaxicode, "Maxicode", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctMicroPDF417, "Micro PDF417", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctMicroQR, "Micro QR Code", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctMSIPlessey, "MSI Plessey", TPDFBarcodeDataType.bcdtBinary, "12345678901", ""), _
      New TTestBarcode(TPDFBarcodeType.bctNVE18, "NVE-18", TPDFBarcodeDataType.bcdtBinary, "1234567890123456", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPDF417, "PDF417", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPDF417Truncated, "PDF417 Truncated", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPharmaOneTrack, "Pharmacode One-Track", TPDFBarcodeDataType.bcdtBinary, "123456", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPharmaTwoTrack, "Pharmacode Two-Track", TPDFBarcodeDataType.bcdtBinary, "123456", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPLANET, "PLANET", TPDFBarcodeDataType.bcdtBinary, "12345678901", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPlessey, "Plessey", TPDFBarcodeDataType.bcdtBinary, "12345678901", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPostNet, "PostNet", TPDFBarcodeDataType.bcdtBinary, "12345678901", ""), _
      New TTestBarcode(TPDFBarcodeType.bctPZN, "PZN", TPDFBarcodeDataType.bcdtBinary, "1234567", ""), _
      New TTestBarcode(TPDFBarcodeType.bctQRCode, "QR Code", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctRMQR, "Rect. Micro QR Code (rMQR)", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctRoyalMail4State, "Royal Mail 4 State (RM4SCC)", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctRSS_EXP_CC, "CS GS1 DataBar Ext. component", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "[10]12345678"), _
      New TTestBarcode(TPDFBarcodeType.bctRSS_EXPSTACK_CC, "CS GS1 DataBar Exp. Stacked", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "[10]12345678"), _
      New TTestBarcode(TPDFBarcodeType.bctRSS_LTD_CC, "CS GS1 DataBar Limited", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "1234567"), _
      New TTestBarcode(TPDFBarcodeType.bctRSS14_CC, "CS GS1 DataBar-14 Linear", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "1234567"), _
      New TTestBarcode(TPDFBarcodeType.bctRSS14Stacked_CC, "CS GS1 DataBar-14 Stacked", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "1234567"), _
      New TTestBarcode(TPDFBarcodeType.bctRSS14StackOMNI_CC, "CS GS1 DataBar-14 Stacked Omni", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "1234567"), _
      New TTestBarcode(TPDFBarcodeType.bctTelepen, "Telepen Alpha", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUltracode, "Ultracode", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUPCA, "UPC A", TPDFBarcodeDataType.bcdtBinary, "1234567890", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUPCA_CC, "CS UPC A linear", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "1234567"), _
      New TTestBarcode(TPDFBarcodeType.bctUPCACheckDigit, "UPC A + Check Digit", TPDFBarcodeDataType.bcdtBinary, "12345678905", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUPCE, "UCP E", TPDFBarcodeDataType.bcdtBinary, "1234567", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUPCE_CC, "CS UPC E linear", TPDFBarcodeDataType.bcdtBinary, "[90]12341234", "1234567"), _
      New TTestBarcode(TPDFBarcodeType.bctUPCECheckDigit, "UCP E + Check Digit", TPDFBarcodeDataType.bcdtBinary, "12345670", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUPNQR, "UPNQR (Univ. Placilni Nalog QR)", TPDFBarcodeDataType.bcdtBinary, "1234567890abcdef", ""), _
      New TTestBarcode(TPDFBarcodeType.bctUSPSOneCode, "USPS OneCode", TPDFBarcodeDataType.bcdtBinary, "01234567094987654321", ""), _
      New TTestBarcode(TPDFBarcodeType.bctVIN, "Vehicle Ident Number (USA)", TPDFBarcodeDataType.bcdtBinary, "01234567094987654", "") _
   }


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

         pdf.SetPageCoords(TPageCoord.pcTopDown)

         Dim bcd As New DynaPDF.TPDFBarcode2()
         pdf.InitBarcode2(bcd)

         bcd.Options = DynaPDF.TPDFBarcodeOptions.bcoDefault Or DynaPDF.TPDFBarcodeOptions.bcoUseActiveFont
         ' Use this flag if you want to create image based barcodes
         'bcd.Options = DynaPDF.TPDFBarcodeOptions.bcoImageOutput

         Dim x, y As Double
         Dim i, xx, yy As Integer
         Dim b As TTestBarcode
         Dim cnt As Integer = TEST_CODES.Length
         Dim pw As Double = pdf.GetPageWidth - 100.0
         Dim ph As Double = pdf.GetPageHeight - 100.0
         Dim w As Double = 100.0
         Dim h As Double = 120.0
         Dim nx As Integer = Math.Truncate(pw / w)
         Dim ny As Integer = Math.Truncate(ph / h)
         Dim incX As Double = w + (pw - nx * w) / (nx - 1)
         Dim incY As Double = h + (ph - ny * h) / (ny - 1)
         h = 100.0
         i = 0

         While i < cnt
            pdf.Append()
               pdf.SetFont("Helvetica", TFStyle.fsRegular, 6.5, True, TCodepage.cp1252)
               pdf.SetLineWidth(0.0)
               y = 50.0
               For yy = 1 To ny
                  x = 50.0
                  For xx = 1 To nx
                     b = TEST_CODES(i)
                     bcd.BarcodeType = b.BarcodeType
                     bcd.Data = b.Data
                     bcd.DataType = b.DataType
                     bcd.Primary = b.Primary
                     pdf.WriteFTextEx(x, y - 10.0, w, -1.0, TTextAlign.taCenter, b.BarcodeName)
                     pdf.Rectangle(x, y, w, h, TPathFillMode.fmStroke)
                     If pdf.InsertBarcode(x, y, w, h, TCellAlign.coCenter, TCellAlign.coCenter, bcd) < 0 Then
                        Exit Sub
                     End If
                     i += 1
                     x += incX
                     If i = cnt Then Exit For
                  Next xx
                  y += incY
                  If i = cnt Then Exit For
               Next yy
            pdf.EndPage()
         End While

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
