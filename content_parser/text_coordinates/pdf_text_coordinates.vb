Option Strict On
Option Explicit On 
Imports System.Runtime.InteropServices
Imports System.Text
Imports text_coordinates.DynaPDF

Friend Class CTextCoordinates

   Private Const MAX_LINE_ERROR As Double = 4.0 ' This must be the square of the allowed error (2 * 2 in this case).

   Public Structure TGState
      Dim ActiveFont As IntPtr
      Dim CharSpacing As Single
      Dim FontSize As Double
      Dim FontType As TFontType
      Dim Matrix As TCTM
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
      m_GState.TextDrawMode = TDrawMode.dmNormal
      m_GState.TextScale = 100.0F
      m_GState.WordSpacing = 0.0F
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

   Public Function MarkCoordinates(ByVal Matrix As TCTM, ByVal Source() As TTextRecordA, ByVal Kerning() As TTextRecordW, ByVal Count As Integer, ByVal Width As Double, ByVal Decoded As Boolean) As Integer
      If Not Decoded Then Return 0
      Try
         Dim i As Integer
         Dim x1 As Double, y1 As Double, x2 As Double, y2 As Double, textWidth As Double
         ' Transform the text matrix to user space
         Dim m As TCTM = MulMatrix(m_GState.Matrix, Matrix)
         Transform(m, x1, y1) ' Start point of the text record

         'This code draws lines under each text record of a PDF file to check whether the coordinates are correct.
         'It shows also how word spacing must be handled. You need an algorithm like this one if you want to
         'develop a text extraction algorithm that tries to preserve the original text layout. Note that word
         'spacing must be ignored when a CID font is selected. In addition, word spacing is applied to the space
         'character (32) of the non-translated source string only. The Unicode string cannot be used to determine
         'whether word spacing must be applied because the character can be encoded to an arbitrary Unicode character.
         If m_GState.FontType = TFontType.ftType0 Then
            ' Word spacing must be ignored if a CID font is selected!
            For i = 0 To Count - 1
               If Kerning(i).Advance <> 0.0F Then
                  textWidth -= Kerning(i).Advance
                  x1 = textWidth
                  y1 = 0.0
                  Transform(m, x1, y1)
               End If
               textWidth += Kerning(i).Width
               x2 = textWidth
               y2 = 0.0
               Transform(m, x2, y2)

               m_PDF.MoveTo(x1, y1)
               m_PDF.LineTo(x2, y2)
               If (m_Count And 1) <> 0 Then
                  m_PDF.SetStrokeColor(CPDF.PDF_RED)
               Else
                  m_PDF.SetStrokeColor(CPDF.PDF_BLUE)
               End If
               If Not m_PDF.StrokePath() Then Return -1
               x1 = x2
               y1 = y2
            Next i
         Else
            Dim j As Integer, last As Integer
            Dim ptr As Long
            Dim src As String
            ' This code draws lines under line segments which are separated by one or more space characters. This is important
            ' to handle word spacing correctly. The same code can be used to compute word boundaries of Ansi strings.
            For i = 0 To Count - 1
               j = 0
               last = 0
               If Source(i).Advance <> 0.0F Then
                  textWidth -= Source(i).Advance
                  x1 = textWidth
                  y1 = 0.0
                  Transform(m, x1, y1)
               End If
               src = Marshal.PtrToStringAnsi(Source(i).Text, Source(i).Length)
               ptr = Source(i).Text.ToInt64
               Do While j < src.Length
                  If src.Chars(j) <> Chr(32) Then
                     j += 1
                  Else
                     If j > last Then
                        ' Note that the text must be taken from the Source array!
                        textWidth += m_PDF.GetTextWidth(m_GState.ActiveFont, _
                                                        New IntPtr(ptr + last), _
                                                        j - last, _
                                                        m_GState.CharSpacing, _
                                                        m_GState.WordSpacing, _
                                                        m_GState.TextScale)
                        x2 = textWidth
                        y2 = 0.0
                        Transform(m, x2, y2)
                        m_PDF.MoveTo(x1, y1)
                        m_PDF.LineTo(x2, y2)
                        If (m_Count And 1) <> 0 Then
                           m_PDF.SetStrokeColor(CPDF.PDF_RED)
                        Else
                           m_PDF.SetStrokeColor(CPDF.PDF_BLUE)
                        End If
                        If Not m_PDF.StrokePath() Then Return -1
                     End If
                     last = j
                     j += 1
                     Do While j < src.Length AndAlso src.Chars(j) = Chr(32)
                        j += 1
                     Loop
                     textWidth += m_PDF.GetTextWidth(m_GState.ActiveFont, _
                                                     New IntPtr(ptr + last), _
                                                     j - last, _
                                                     m_GState.CharSpacing, _
                                                     m_GState.WordSpacing, _
                                                     m_GState.TextScale)
                     last = j
                     x1 = textWidth
                     y1 = 0.0
                     Transform(m, x1, y1)
                  End If
               Loop
               If j > last Then
                  textWidth += m_PDF.GetTextWidth(m_GState.ActiveFont, _
                                                  New IntPtr(ptr + last), _
                                                  j - last, _
                                                  m_GState.CharSpacing, _
                                                  m_GState.WordSpacing, _
                                                  m_GState.TextScale)
                  x2 = textWidth
                  y2 = 0.0
                  Transform(m, x2, y2)
                  m_PDF.MoveTo(x1, y1)
                  m_PDF.LineTo(x2, y2)
                  If (m_Count And 1) <> 0 Then
                     m_PDF.SetStrokeColor(CPDF.PDF_RED)
                  Else
                     m_PDF.SetStrokeColor(CPDF.PDF_BLUE)
                  End If
                  If Not m_PDF.StrokePath() Then Return -1
               End If
               x1 = x2
               y1 = y2
            Next i
         End If
         m_Count += 1
         Return 0
      Catch
         Return -1
      End Try
   End Function

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
      m_GState.TextDrawMode = TDrawMode.dmNormal
      m_GState.TextScale = 100.0F
      m_GState.WordSpacing = 0.0F
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

   Protected m_Count As Integer
   Protected m_GState As TGState
   Protected m_PDF As CPDF
   Protected m_Stack As CStack
End Class
