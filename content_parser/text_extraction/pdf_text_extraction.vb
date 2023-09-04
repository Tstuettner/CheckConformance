Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices
Imports System.Text
Imports text_extraction2.DynaPDF

Friend Class CPDFToText

   Private Const MAX_LINE_ERROR As Double = 4.0 ' This must be the square of the allowed error (2 * 2 in this case).

   Public Structure TGState
      Dim ActiveFont As IntPtr
      Dim CharSpacing As Single
      Dim FontSize As Double
      Dim FontType As TFontType
      Dim Matrix As TCTM
      Dim SpaceWidth As Single
      Dim TextDrawMode As TDrawMode
      Dim TextScale As Single
      Dim WordSpacing As Single
   End Structure

   Protected Class CStack
      Protected Overrides Sub Finalize()
         m_Items = Nothing
      End Sub

      Public Function Restore(ByRef F As TGState) As Boolean
         If m_Count > 0 Then
            m_Count -= 1
            F = m_Items(m_Count)
            Return True
         End If
         Return False
      End Function

      Public Function Save(ByRef F As TGState) As Integer
         If m_Count = m_Capacity Then
            m_Capacity += 16
            Try
               ReDim m_Items(m_Capacity)
            Catch
               m_Capacity -= 16
               Return -1
            End Try
         End If
         m_Items(m_Count) = F
         m_Count += 1
         Return 0
      End Function
      Private m_Capacity As Integer
      Private m_Count As Integer
      Private m_Items As TGState()
   End Class

   Public Sub New(ByVal PDFInst As CPDF)
      m_GState.ActiveFont = IntPtr.Zero
      m_GState.CharSpacing = 0.0F
      m_GState.FontSize = 1.0
      m_GState.FontType = TFontType.ftType1
      m_GState.Matrix.a = 1.0
      m_GState.Matrix.b = 0.0
      m_GState.Matrix.c = 0.0
      m_GState.Matrix.d = 1.0
      m_GState.Matrix.x = 0.0
      m_GState.Matrix.y = 0.0
      m_GState.SpaceWidth = 0.0F
      m_GState.TextDrawMode = TDrawMode.dmNormal
      m_GState.TextScale = 100.0F
      m_GState.WordSpacing = 0.0F
      m_LastTextDir = TTextDir.tfNotInitialized
      m_PDF = PDFInst
      m_Stack = New CStack()
   End Sub

   Public Function BeginTemplate(ByVal BBox As TPDFRect, ByVal Matrix As IntPtr) As Integer
      Reset()
      If Not IntPtr.Zero.Equals(Matrix) Then
         Dim m As TCTM
         m = CType(Marshal.PtrToStructure(Matrix, GetType(TCTM)), TCTM)
         m_GState.Matrix = MulMatrix(m_GState.Matrix, m)
      End If
      Return 0
   End Function

   Protected Function CalcDistance(ByVal x1 As Double, ByVal y1 As Double, ByVal x2 As Double, ByVal y2 As Double) As Double
      Dim dx As Double = x2 - x1
      Dim dy As Double = y2 - y1
      Return Math.Sqrt(dx * dx + dy * dy)
   End Function

   Public Function AddText(ByVal Matrix As TCTM, ByVal Source() As TTextRecordA, ByVal Kerning() As TTextRecordW, ByVal Count As Integer, ByVal Width As Double, ByVal Decoded As Boolean) As Integer
      If Not Decoded Then Return 0
      Try
         Dim i As Integer
         Dim textDir As TTextDir
         Dim x1 As Double = 0.0
         Dim y1 As Double = 0.0
         Dim x2 As Double = 0.0
         Dim y2 As Double = m_GState.FontSize
         ' Transform the text matrix to user space
         Dim m As TCTM = MulMatrix(m_GState.Matrix, Matrix)
         Transform(m, x1, y1) ' Start point of the text record
         'The second point to determine the text direction can also be used to
         'calculate the visible font size measured in user space:
         '  Dim realFontSize as Double = CalcDistance(x1, y1, x2, y2)
         Transform(m, x2, y2) ' Second point to determine the text direction
         ' Determine the text direction
         If y1 = y2 Then
            textDir = CType((System.Convert.ToInt32(x1 > x2) + 1) * 2, TTextDir)
         Else
            textDir = CType(System.Convert.ToInt32(y1 > y2), TTextDir)
         End If

         If textDir <> m_LastTextDir OrElse Not IsPointOnLine(x1, y1, m_LastTextEndX, m_LastTextEndY, m_LastTextInfX, m_LastTextInfY) Then
            ' Extend the x-coordinate to an infinite point
            m_LastTextInfX = 1000000.0
            m_LastTextInfY = 0.0
            Transform(m, m_LastTextInfX, m_LastTextInfY)
            If m_LastTextDir <> TTextDir.tfNotInitialized Then
               ' Add a new line to the output file
               m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Chr(13) + Chr(10)))
            End If
         Else
            'The space width is measured in text space but the distance between two text
            'records is measured in user space! We must transform the space width to user
            'space before we can compare the values.
            Dim distance As Double, spaceWidth As Double
            ' Note that we use the full space width here because the end position of the last record
            ' was set to the record width minus the half space width.
            Dim x3 As Double = m_GState.SpaceWidth
            Dim y3 As Double = 0.0
            Transform(m, x3, y3)
            spaceWidth = CalcDistance(x1, y1, x3, y3)
            distance = CalcDistance(m_LastTextEndX, m_LastTextEndY, x1, y1)
            If distance > spaceWidth Then
               'Add a space to the output file
               m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(" "))
            End If
         End If
         ' We use the half space width to determine whether a space must be inserted at
         ' a specific position. This produces better results in most cases.
         Dim spw As Single = -m_GState.SpaceWidth * 0.5F
         Dim rec As TTextRecordW
         For i = 0 To Count - 1
            rec = Kerning(i)
            If rec.Advance < spw Then
               ' Add a space to the output file
               m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(" "))
            End If
            m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Marshal.PtrToStringUni(rec.Text, rec.Length)))
         Next i
         ' We don't set the cursor to the real end of the string because applications like MS Word
         ' add often a space to the end of a text record and this space can slightly overlap the next
         ' record. IsPointOnLine() would return false in this case.
         m_LastTextEndX = Width + spw ' spw is a negative value!
         m_LastTextEndY = 0.0
         m_LastTextDir = textDir
         ' Calculate the end coordinate of the text record
         Transform(m, m_LastTextEndX, m_LastTextEndY)
         Return 0
      Catch
         Return -1
      End Try
   End Function

   Public Sub Close()
      m_File.Flush()
      m_File.Close()
      m_File = Nothing
      m_Stream = Nothing
   End Sub

   Public Sub Init()
      Do While RestoreGState()
      Loop
      m_GState.ActiveFont = IntPtr.Zero
      m_GState.CharSpacing = 0.0F
      m_GState.FontSize = 1.0
      m_GState.Matrix.a = 1.0
      m_GState.Matrix.b = 0.0
      m_GState.Matrix.c = 0.0
      m_GState.Matrix.d = 1.0
      m_GState.Matrix.x = 0.0
      m_GState.Matrix.y = 0.0
      m_GState.SpaceWidth = 0.0F
      m_GState.TextDrawMode = TDrawMode.dmNormal
      m_GState.TextScale = 100.0F
      m_GState.WordSpacing = 0.0F
      m_LastTextDir = TTextDir.tfNotInitialized
   End Sub

   Private Function IsPointOnLine(ByVal x As Double, ByVal y As Double, ByVal x0 As Double, ByVal y0 As Double, ByVal x1 As Double, ByVal y1 As Double) As Boolean
      Dim dx As Double, dy As Double, di As Double
      x -= x0
      y -= y0
      dx = x1 - x0
      dy = y1 - y0
      di = (x * dx + y * dy) / (dx * dx + dy * dy)
      If di < 0.0 Then
         di = 0.0
      ElseIf di > 1.0 Then
         di = 1.0
      End If
      dx = x - di * dx
      dy = y - di * dy
      di = dx * dx + dy * dy
      Return (di < MAX_LINE_ERROR)
   End Function

   Public Sub MulMatrix(ByVal M As TCTM)
      m_GState.Matrix = MulMatrix(m_GState.Matrix, M)
   End Sub

   Protected Function MulMatrix(ByVal M1 As TCTM, ByVal M2 As TCTM) As TCTM
      Dim retval As TCTM
      retval.a = M2.a * M1.a + M2.b * M1.c
      retval.b = M2.a * M1.b + M2.b * M1.d
      retval.c = M2.c * M1.a + M2.d * M1.c
      retval.d = M2.c * M1.b + M2.d * M1.d
      retval.x = M2.x * M1.a + M2.y * M1.c + M1.x
      retval.y = M2.x * M1.b + M2.y * M1.d + M1.y
      Return retval
   End Function

   Public Sub Open(ByVal FileName As String)
      m_Stream = New System.IO.FileStream(FileName, System.IO.FileMode.Create, System.IO.FileAccess.ReadWrite)
      m_File = New System.IO.BinaryWriter(m_Stream, System.Text.Encoding.Unicode)
      ' Write a Little Endian marker to the file (byte order mark)
      m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(ChrW(&HFEFF)))
   End Sub

   Public Function RestoreGState() As Boolean
      Return m_Stack.Restore(m_GState)
   End Function

   Public Function SaveGState() As Integer
      Return m_Stack.Save(m_GState)
   End Function

   Public Sub SetCharSpacing(ByVal Value As Double)
      m_GState.CharSpacing = CType(Value, Single)
   End Sub

   Public Sub SetFont(ByVal FontSize As Double, ByVal Type As TFontType, ByVal Font As IntPtr)
      m_GState.ActiveFont = Font
      m_GState.FontSize = FontSize
      m_GState.FontType = Type
      m_GState.SpaceWidth = CType(m_PDF.GetSpaceWidth(Font, FontSize), Single)
      If FontSize < 0.0 Then m_GState.SpaceWidth = -m_GState.SpaceWidth
   End Sub

   Public Sub SetTextDrawMode(ByVal Mode As TDrawMode)
      m_GState.TextDrawMode = Mode
   End Sub

   Public Sub SetTextScale(ByVal Value As Double)
      m_GState.TextScale = CType(Value, Single)
   End Sub

   Public Sub SetWordSpacing(ByVal Value As Double)
      m_GState.WordSpacing = CType(Value, Single)
   End Sub

   Protected Sub Transform(ByRef M As TCTM, ByRef x As Double, ByRef y As Double)
      Dim tx As Double
      tx = x
      x = tx * M.a + y * M.c + M.x
      y = tx * M.b + y * M.d + M.y
   End Sub

   Public Sub WritePageIdentifier(ByVal PageNum As Integer)
      If PageNum > 1 Then
         m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(Chr(13) + Chr(10)))
      End If
      m_File.Write(System.Text.UnicodeEncoding.Unicode.GetBytes(String.Format("%----------------------- Page {0} -----------------------------" + Chr(13) + Chr(10), PageNum)))
   End Sub

   Protected Enum TTextDir
      tfLeftToRight = 0
      tfRightToLeft = 1
      tfTopToBottom = 2
      tfBottomToTop = 4
      tfNotInitialized = -1
   End Enum

   Protected m_File As System.IO.BinaryWriter
   Protected m_GState As TGState
   Protected m_HavePos As Boolean
   Protected m_LastTextDir As TTextDir
   Protected m_LastTextEndX As Double
   Protected m_LastTextEndY As Double
   Protected m_LastTextInfX As Double
   Protected m_LastTextInfY As Double
   Protected m_PDF As CPDF
   Protected m_Stack As CStack
   Protected m_Stream As System.IO.FileStream
End Class
